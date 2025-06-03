using ClosedXML.Excel;
using CSV_reader.Models;
using CSV_reader.database;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using CSV_reader.Configurations;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CSV_reader.Services
{
    public class ClaimsService : IClaimsService
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationContext _appContext;
        private readonly IServiceScopeFactory _scopeFactory;

        public ClaimsService(IConfiguration configuration, ApplicationContext appContext, IServiceScopeFactory scopeFactory)
        {
            _configuration = configuration;
            _appContext = appContext;
            _scopeFactory = scopeFactory;
        }

        //------------- READING THE .XLSX FILE -------------------------
        /* The following method reads the .xlsx file and creates 5 different dictionaries for the five worksheets: claims, days, turnover, vehicles, forecast. 
         * The rentalDaysDict has a key of policy year, and its value is an object containing rental days COI and non COI.
         * The turnoverDict also has a key of policy year, and its value is an object containing the turnover COI and non COI.
         * The vehiclesDict has a key of the client name, and its value is an object containing the car, van, minibus and HGV vehicles nums.
         * The forecastDict also has a key of the client name, and its value is an object containing the forecast days and turnover both COI and non COI.
         * Then for the claims worksheet, an instance of the ClaimsModel is created for each row of the worksheet, and is assigned to the variable 'records'. Doing this creates a collection of objects for each row of the worksheet where the properties of the ClaimsModel are assigned the values in the columns of the worksheet (an object is created for each claim as each row is a claim).
         * Since the ClaimsModel has properties which the claims worksheet doesn't have, the data from the other dictionaries are merged (eg. the rental days properties' values are obtained from the rentalDaysDict by looking up the policy year as the key).
         * At the start, the 'data' variable initialises an empty list. After each row of the claims worksheet is read and the 'records' variable is populated, data.Add(records) adds this to the list.
         * 
         * After this, the GetPolicyYearSummaries method is run.
         * This consolidates the list of ClaimsModel objects by creating a dictionary with the key of policy year and the value is an object which is an instance of the PolicyYearSummary. It is essentially a summary for each year with some properties being a sum of a property from the list of ClaimsModel objects like the AD-Paid and others just being the first value like the rental days property since it is the same for each object in the list.
        
        */

        double carExposure = 2200;
        double vanExposure = 2320;
        double minibusExposure = 3333;
        double hgvExposure = 4750;   // hard coded for now

        public List<ClaimsModel> ReadClaimsExcel(string filePath)
         {

            var data = new List<ClaimsModel>(); // this initialises an empty list 'data' where each row from the Excel sheet will be stored as a ClientModel instance

            // Check if the file exists before trying to read it
            if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
            {
                throw new FileNotFoundException("Excel file not found at the specified path", filePath);
            }

            using (var workbook = new XLWorkbook(filePath))
            {
                var claimsWorksheet = workbook.Worksheet("ImportClaims");
                var daysWorksheet = workbook.Worksheet("ImportDays");
                var turnoverWorksheet = workbook.Worksheet("ImportTurnover");
                var vehiclesWorksheet = workbook.Worksheet("ImportVehicles");
                var forecastWorksheet = workbook.Worksheet("ImportForecast");

                // the following dictionaries have the policy years as keys


                // ------------- DAYS WORKSHEET ---------------------
                // this dict's values is an anonymous object with the COI and non COI ints from the ImportDays excel sheet               
                // alternative method which gets the excel sheet values from the name of the column headers instead of col numbers for the rental days

                // get the headers row of the excel worksheet
                var daysSheetHeaderRow = daysWorksheet.FirstRowUsed();
                // create a list of these headers
                var daysSheetHeaders = daysSheetHeaderRow.Cells().Select(c => c.GetValue<string>().Trim()).ToList();               

                // get the column number based on the index of the list + 1.
                // these numbers will be used when using Cell()
                // this is the alternative to hard coding the number of the column to be used. This is because some of the claims worksheets have additional columns that others don't have - so using the name of the header is better because it should always be the same header in each worksheet
                // Either way, we are hardcoding either the col number or col header string
                int claimsSheetPolicyYearColNumber = daysSheetHeaders.IndexOf("PolicyYear") + 1;
                int rentalDaysCOICol = daysSheetHeaders.IndexOf("COI") + 1;
                int rentalDaysNonCOICol = daysSheetHeaders.IndexOf("NonCOI") + 1;

                // create dict
                var rentalDaysDict = daysWorksheet.RowsUsed().Skip(1)
                    .ToDictionary(                    
                    row => row.Cell(claimsSheetPolicyYearColNumber).GetValue<string>(),
                    row => new
                    {
                        RentalDaysCOI = row.Cell(rentalDaysCOICol).TryGetValue<int>(out var RDaysCOI) ? RDaysCOI : 0,
                        RentalDaysNonCOI = row.Cell(rentalDaysNonCOICol).TryGetValue<int>(out var RDaysNonCOI) ? RDaysNonCOI : 0,
                    })
                    .OrderByDescending(x => x.Key) // Order by PolicyYear (descending)
                    .Take(5) // Take the 5 most recent entries
                    .ToDictionary(x => x.Key, x => x.Value);


                // ------------- TURNOVER WORKSHEET ---------------------

                var turnoverSheetHeadersRow = turnoverWorksheet.FirstRowUsed();
                var turnoverSheetHeaders = turnoverSheetHeadersRow.Cells().Select(c => c.GetValue<string>().Trim()).ToList();

                int turnoverSheetPolicyYearCol = turnoverSheetHeaders.IndexOf("PolicyYear") + 1;
                int turnoverCOICol = turnoverSheetHeaders.IndexOf("COI") + 1;
                int turnoverNonCOICol = turnoverSheetHeaders.IndexOf("NonCOI") + 1;

                var turnoverDict = turnoverWorksheet.RowsUsed().Skip(1)
                    .ToDictionary(
                    row => row.Cell(turnoverSheetPolicyYearCol).GetValue<string>(),
                    row => new
                    {
                        TurnoverCOI = row.Cell(turnoverCOICol).TryGetValue<double>(out var TOCOI) ? TOCOI : 0,
                        TurnoverNonCOI = row.Cell(turnoverNonCOICol).TryGetValue<double>(out var TONonCOI) ? TONonCOI : 0,
                    })
                    .OrderByDescending(x => x.Key) // Order by PolicyYear (descending)
                    .Take(5) // Take the 5 most recent entries
                    .ToDictionary(x => x.Key, x => x.Value);


                // ------------- VEHICLES WORKSHEET ---------------------

                var vehiclesSheetHeadersRow = vehiclesWorksheet.FirstRowUsed();
                var vehiclesSheetHeaders = vehiclesSheetHeadersRow.Cells().Select(c => c.GetValue<string>().Trim()).ToList();

                int vehiclesSheetClientNameCol = vehiclesSheetHeaders.IndexOf("ClientName") + 1;
                int carNumsCol = vehiclesSheetHeaders.IndexOf("Cars") + 1;
                int vanNumsCol = vehiclesSheetHeaders.IndexOf("Vans") + 1;
                int minibusNumsCol = vehiclesSheetHeaders.IndexOf("Minibuses") + 1;
                int hgvNumsCol = vehiclesSheetHeaders.IndexOf("HGVs") + 1;

                var vehiclesDict = vehiclesWorksheet.RowsUsed().Skip(1)
                    .ToDictionary(
                    row => row.Cell(vehiclesSheetClientNameCol).GetValue<string>(),
                    row => new
                    {
                        CarNums = row.Cell(carNumsCol).TryGetValue<int>(out var carNums) ? carNums : 0,
                        VanNums = row.Cell(vanNumsCol).TryGetValue<int>(out var vanNums) ? vanNums : 0,
                        MinibusNums = row.Cell(minibusNumsCol).TryGetValue<int>(out var mBusNums) ? mBusNums : 0,
                        HGVNums = row.Cell(hgvNumsCol).TryGetValue<int>(out var hgvNums) ? hgvNums : 0,
                    });


                // ------------- FORECAST WORKSHEET ---------------------

                var forecastSheetHeadersRow = forecastWorksheet.FirstRowUsed();
                var forecastSheetHeaders = forecastSheetHeadersRow.Cells().Select(c => c.GetValue<string>().Trim()).ToList();

                int forecastSheetClientNameCol = forecastSheetHeaders.IndexOf("ClientName") + 1;
                int FCTO_COICol = forecastSheetHeaders.IndexOf("COI - Turnover") + 1;
                int FCTO_NonCOICol = forecastSheetHeaders.IndexOf("NonCOI - Turnover") + 1;
                int FCDaysCOICol = forecastSheetHeaders.IndexOf("COI - Days") + 1;
                int FCDaysNonCOICol = forecastSheetHeaders.IndexOf("NonCOI - Days") + 1;

                var forecastDict = forecastWorksheet.RowsUsed().Skip(1)
                    .ToDictionary(
                    row => row.Cell(forecastSheetClientNameCol).GetValue<string>(),
                    row => new
                    {
                        FCTO_COI = row.Cell(FCTO_COICol).TryGetValue<double>(out var fctoCOI) ? fctoCOI : 0,
                        FCTO_NonCOI = row.Cell(FCTO_NonCOICol).TryGetValue<double>(out var fctoNonCOI) ? fctoNonCOI : 0,
                        FCDaysCOI = row.Cell(FCDaysCOICol).TryGetValue<int>(out var fcDaysCOI) ? fcDaysCOI : 0,
                        FCDaysNonCOI = row.Cell(FCDaysNonCOICol).TryGetValue<int>(out var fcDaysNonCOI) ? fcDaysNonCOI : 0,
                    });


                // ------------- CLAIMS WORKSHEET ---------------------

                var claimsSheetHeadersRow = claimsWorksheet.FirstRowUsed();
                var claimsSheetHeaders = claimsSheetHeadersRow.Cells().Select(c => c.GetValue<string>().Trim()).ToList();

                int claimsSheetPolicyYearCol = claimsSheetHeaders.IndexOf("PolicyYear") + 1;
                int claimsSheetClientNameCol = claimsSheetHeaders.IndexOf("ClientName") + 1;
                int claimsSheetClaimRefCol = claimsSheetHeaders.IndexOf("ClaimReference") + 1;
                int claimsSheetLossDateCol = claimsSheetHeaders.IndexOf("LossDate") + 1;
                int claimsSheetReportedDateCol = claimsSheetHeaders.IndexOf("ReportedDate") + 1;
                int claimsSheetRegCol = claimsSheetHeaders.IndexOf("Registration") + 1;
                int claimsSheetMakeCol = claimsSheetHeaders.IndexOf("Make") + 1;
                int claimsSheetModelCol= claimsSheetHeaders.IndexOf("Model") + 1;
                int claimsSheetVehTypeCol = claimsSheetHeaders.IndexOf("VehicleType") + 1;
                int claimsSheetIncidentTypeCol = claimsSheetHeaders.IndexOf("IncidentType") + 1;
                int claimsSheetStatusCol = claimsSheetHeaders.IndexOf("Status") + 1;

                int claimsSheetADPaidCol = claimsSheetHeaders.IndexOf("AD - Paid") + 1;
                int claimsSheetFTPaidCol = claimsSheetHeaders.IndexOf("F&T - Paid") + 1;

                int claimsSheetTPPDPaidCol = claimsSheetHeaders.IndexOf("TPPD - Paid") + 1;
                int claimsSheetTPCHPaidCol = claimsSheetHeaders.IndexOf("TPCH - Paid") + 1;
                int claimsSheetTPPIPaidCol = claimsSheetHeaders.IndexOf("TPPI - Paid") + 1;              

                int claimsSheetADOSCol = claimsSheetHeaders.IndexOf("AD - OS") + 1;
                int claimsSheetFTOSCol = claimsSheetHeaders.IndexOf("F&T - OS") + 1;

                int claimsSheetTPPDOSCol = claimsSheetHeaders.IndexOf("TPPD - OS") + 1;
                int claimsSheetTPCHOSCol = claimsSheetHeaders.IndexOf("TPCH - OS") + 1;
                int claimsSheetTPPIOSCol = claimsSheetHeaders.IndexOf("TPPI - OS") + 1;

                int claimsSheetTotalCol = claimsSheetHeaders.IndexOf("Total Incurred") + 1;

                // manual code for Links van hire docs
                int claimsSheetTPPaidTotal = claimsSheetHeaders.IndexOf("TP - Paid") + 1;
                int claimsSheetTPOSTotal = claimsSheetHeaders.IndexOf("TP - OS") + 1;

                Console.WriteLine($"claimsSheetHeadersRow: {claimsSheetHeadersRow}");

                foreach (var row in claimsWorksheet.RowsUsed().Skip(1))
                {
                    // Setting these as variables first since they're used to merge the other dicts together
                    var policyYear = row.Cell(claimsSheetPolicyYearCol).GetValue<string>();
                    var clientName = row.Cell(claimsSheetClientNameCol).GetValue<string>();

                    var records = new ClaimsModel
                    {
                        ClientNameCol = clientName,
                        PolicyYearCol = policyYear,
                        ClaimReferenceCol = row.Cell(claimsSheetClaimRefCol).GetValue<string>(),
                        LossDateCol = row.Cell(claimsSheetLossDateCol).GetValue<string>().Split(' ')[0],
                        ReportedDateCol = row.Cell(claimsSheetReportedDateCol).GetValue<string>().Split(' ')[0],
                        RegCol = row.Cell(claimsSheetRegCol).GetValue<string>(),
                        MakeCol = claimsSheetMakeCol == 0 ? "Not Given" : row.Cell(claimsSheetMakeCol).GetValue<string>(),
                        ModelCol = claimsSheetModelCol == 0 ? "Not Given" : row.Cell(claimsSheetModelCol).GetValue<string>(),
                        VehicleTypeCol = claimsSheetVehTypeCol == 0 ? "Not Given" : row.Cell(claimsSheetVehTypeCol).GetValue<string>(),
                        IncidentTypeCol = row.Cell(claimsSheetIncidentTypeCol).GetValue<string>(),
                        StatusCol = row.Cell(claimsSheetStatusCol).GetValue<string>(),

                        AD_PaidCol = row.Cell(claimsSheetADPaidCol).TryGetValue<double>(out var adPaid) ? adPaid : 0,
                        ADOSCol = row.Cell(claimsSheetADOSCol).TryGetValue<double>(out var ados) ? ados : 0,
                        FT_PaidCol = row.Cell(claimsSheetFTPaidCol).TryGetValue<double>(out var ftPaid) ? ftPaid : 0,
                        FTOSCol = row.Cell(claimsSheetFTOSCol).TryGetValue<double>(out var ftos) ? ftos : 0,

                        // the following ternary statements are for when there is only a 'TP - Paid' column and no third party subsections like TPPD, TPCH, etc
                        TPPD_PaidCol =
                            claimsSheetTPPDPaidCol == 0 && claimsSheetTPCHPaidCol == 0 && claimsSheetTPPIPaidCol == 0
                            ?
                            row.Cell(claimsSheetTPPaidTotal).TryGetValue<double>(out var tppdPaidTotal) ? tppdPaidTotal : 0
                            :
                            row.Cell(claimsSheetTPPDPaidCol).TryGetValue<double>(out var tppdPaid) ? tppdPaid : 0,
                        TPCH_PaidCol =
                            claimsSheetTPCHPaidCol > 0 && row.Cell(claimsSheetTPCHPaidCol).TryGetValue<double>(out var tpchPaid) 
                            ? 
                            tpchPaid : 0,
                        TPPI_PaidCol =
                            claimsSheetTPPIPaidCol > 0 && row.Cell(claimsSheetTPPIPaidCol).TryGetValue<double>(out var tppiPaid) 
                            ? 
                            tppiPaid : 0,

                        // the following ternary statements are for when there is only a 'TP - OS' column and no third party subsections like TPPDOS, TPCHOS, etc
                        TPPD_OSCol = 
                            claimsSheetTPPDOSCol == 0 && claimsSheetTPCHOSCol == 0 && claimsSheetTPPIOSCol == 0
                            ?
                            row.Cell(claimsSheetTPOSTotal).TryGetValue<double>(out var tposTotal) ? tposTotal : 0
                            :
                            row.Cell(claimsSheetTPPDOSCol).TryGetValue<double>(out var tppdOS) ? tppdOS : 0,
                        TPCH_OSCol =
                            claimsSheetTPCHOSCol > 0 && row.Cell(claimsSheetTPCHOSCol).TryGetValue<double>(out var tpchOS) 
                            ? tpchOS : 0,
                        TPPI_OSCol =
                            claimsSheetTPPIOSCol > 0 && row.Cell(claimsSheetTPPIOSCol).TryGetValue<double>(out var tppiOS) 
                            ? tppiOS : 0,

                        TotalCol = row.Cell(claimsSheetTotalCol).TryGetValue<double>(out var total) ? total : 0,
                       
                    }; 

                    // Merge data from other dicts to populate the ClaimsModel props that the claims worksheet doesn't contain
                    if (rentalDaysDict.TryGetValue(policyYear, out var rentalDays))
                    {
                        records.RentalDaysCOICol = rentalDays.RentalDaysCOI;
                        records.RentalDaysNonCOICol = rentalDays.RentalDaysNonCOI;
                    }
                    if (turnoverDict.TryGetValue(policyYear, out var turnover))
                    {
                        records.TurnoverCOICol = turnover.TurnoverCOI;
                        records.TurnoverNonCOICol = turnover.TurnoverNonCOI;
                    }
                    if (vehiclesDict.TryGetValue(clientName, out var vehicles))
                    {
                        records.CarNumsCol = vehicles.CarNums;
                        records.VanNumsCol = vehicles.VanNums;
                        records.MinibusNumsCol = vehicles.MinibusNums;
                        records.HGVNumsCol = vehicles.HGVNums;
                    }
                    if (forecastDict.TryGetValue(clientName, out var forecast))
                    {
                        records.FCTO_COICol = forecast.FCTO_COI;
                        records.FCTO_NonCOICol = forecast.FCTO_NonCOI;
                        records.FCDaysCOICol = forecast.FCDaysCOI;
                        records.FCDaysNonCOICol = forecast.FCDaysNonCOI;
                    }

                    data.Add(records);

                    Console.WriteLine(JsonConvert.SerializeObject(records, Formatting.Indented));
                }
            }

           Console.WriteLine(JsonConvert.SerializeObject(data, Formatting.Indented));

            return data;

        }


        // need a function to add each claim data to the database table as a row

        public string SaveClaimsToDatabase(List<ClaimsModel> claimsData, InputModel inputModel)
        {
        

            // Creating a unique Batch ID using Client Name + Timestamp
            string clientName = claimsData.First().ClientNameCol.Replace(" ", "_");
            string quoteNumber = GenerateQuoteNumber(clientName);
            string batchId = $"{quoteNumber}_{DateTime.UtcNow:yyyyMMddHHmm}";

            // This converts each ClaimsModel into an IndivClaimDB object
            var claimsToInsert = claimsData.Select(claim => new IndivClaimDB
            {
                BatchId = batchId,

                ClientName = claim.ClientNameCol,
                PolicyYear = claim.PolicyYearCol,
                ClaimRef = claim.ClaimReferenceCol,
                LossDate = claim.LossDateCol,
                ReportedDate = claim.ReportedDateCol,
                Registration = claim.RegCol,
                Make = claim.MakeCol ?? "",
                Model = claim.ModelCol ?? "",
                VehicleType = claim.VehicleTypeCol ?? "",
                IncidentType = claim.IncidentTypeCol,
                Status = claim.StatusCol,

                AD_Paid = claim.AD_PaidCol,
                FT_Paid = claim.FT_PaidCol,
                TPPD_Paid = claim.TPPD_PaidCol,
                TPCH_Paid = claim.TPCH_PaidCol,
                TPPI_Paid = claim.TPPI_PaidCol,
                ADOS = claim.ADOSCol,
                FTOS = claim.FTOSCol,
                TPPD_OS = claim.TPPD_OSCol,
                TPCH_OS = claim.TPCH_OSCol,
                TPPI_OS = claim.TPPI_OSCol,
                Total = claim.TotalCol,

                RDaysCOI = claim.RentalDaysCOICol,
                RDaysNonCOI = claim.RentalDaysNonCOICol,
                TurnoverCOI = claim.TurnoverCOICol,
                TurnoverNonCOI = claim.TurnoverNonCOICol,

                ForecastTO_COI = claim.FCTO_COICol,
                ForecastTO_NonCOI = claim.FCTO_NonCOICol,
                ForecastDaysCOI = claim.FCDaysCOICol,
                ForecastDaysNonCOI = claim.FCDaysNonCOICol,

                CarNums = claim.CarNumsCol,
                VanNums = claim.VanNumsCol,
                MinibusNums = claim.MinibusNumsCol,
                HGVNums = claim.HGVNumsCol,

                // data from input model:
                ClientCoverType = inputModel.SelectedCoverType,
                ClientExcess = inputModel.Excess,
                ClientStartDate = inputModel.StartDate?.ToString("yyyy-MM-dd"),
                ClientEndDate = inputModel.EndDate?.ToString("yyyy-MM-dd"),

                ClientCarLLL = inputModel.CarLLL,
                ClientVanLLL = inputModel.VanLLL,
                ClientMBusLLL = inputModel.MinibusLLL,
                ClientHGVLLL = inputModel.HGVLLL,

                CarExposure = carExposure,
                VanExposure = vanExposure,
                MinibusExposure = minibusExposure,
                HGVExposure = hgvExposure,

                ExpoPercentage = Convert.ToDouble(inputModel.SelectedPercentage),
            }).ToList();

            var indivClaimDataToInsert = claimsData.Select(claim => new IndivClaimDataDB
            {
                BatchId = batchId,

                ClientName = claim.ClientNameCol,
                PolicyYear = claim.PolicyYearCol,
                ClaimRef = claim.ClaimReferenceCol,
                LossDate = claim.LossDateCol,
                ReportedDate = claim.ReportedDateCol,
                Registration = claim.RegCol,
                Make = claim.MakeCol ?? "",
                Model = claim.ModelCol ?? "",
                VehicleType = claim.VehicleTypeCol ?? "",
                IncidentType = claim.IncidentTypeCol,
                Status = claim.StatusCol,

                AD_Paid = claim.AD_PaidCol,
                FT_Paid = claim.FT_PaidCol,
                TPPD_Paid = claim.TPPD_PaidCol,
                TPCH_Paid = claim.TPCH_PaidCol,
                TPPI_Paid = claim.TPPI_PaidCol,
                ADOS = claim.ADOSCol,
                FTOS = claim.FTOSCol,
                TPPD_OS = claim.TPPD_OSCol,
                TPCH_OS = claim.TPCH_OSCol,
                TPPI_OS = claim.TPPI_OSCol,
                Total = claim.TotalCol,

                RDaysCOI = claim.RentalDaysCOICol,
                RDaysNonCOI = claim.RentalDaysNonCOICol,
                TurnoverCOI = claim.TurnoverCOICol,
                TurnoverNonCOI = claim.TurnoverNonCOICol,
            }).ToList();

            var firstClaim = claimsData.First();

            var staticDataToInsert = new StaticClientDataDB
            {
                BatchId = batchId,                

                ForecastTO_COI = firstClaim.FCTO_COICol,
                ForecastTO_NonCOI = firstClaim.FCTO_NonCOICol,
                ForecastDaysCOI = firstClaim.FCDaysCOICol,
                ForecastDaysNonCOI = firstClaim.FCDaysNonCOICol,

                CarNums = firstClaim.CarNumsCol,
                VanNums = firstClaim.VanNumsCol,
                MinibusNums = firstClaim.MinibusNumsCol,
                HGVNums = firstClaim.HGVNumsCol,

                // data from input model:
                ClientCoverType = inputModel.SelectedCoverType,
                ClientExcess = inputModel.Excess,
                ClientStartDate = inputModel.StartDate?.ToString("yyyy-MM-dd"),
                ClientEndDate = inputModel.EndDate?.ToString("yyyy-MM-dd"),

                ClientCarLLL = inputModel.CarLLL,
                ClientVanLLL = inputModel.VanLLL,
                ClientMBusLLL = inputModel.MinibusLLL,
                ClientHGVLLL = inputModel.HGVLLL,

                CarExposure = carExposure,
                VanExposure = vanExposure,
                MinibusExposure = minibusExposure,
                HGVExposure = hgvExposure,

                ExpoPercentage = Convert.ToDouble(inputModel.SelectedPercentage),
            };

            _appContext.ClaimsTable.AddRange(claimsToInsert);

            _appContext.IndivClaimData.AddRange(indivClaimDataToInsert);
            _appContext.StaticClientDataDB.AddRange(staticDataToInsert);
            // AddRange adds all claims from the list to the db in one go

            _appContext.SaveChanges();

            return batchId;
            
        }
    
        public string GenerateQuoteNumber(string clientName)
        {
            string[] wordsToIgnore = { "and", "and" };

            string initials = string.Concat(clientName
              .Split('_') 
              .Where(word => !wordsToIgnore.Contains(word.ToLower())) 
              .Take(3)
              .Select(word => word[0].ToString().ToUpper()));

            // This line creates a 6 character string with the first character being a number so there is no other letter next to the uppercase client name initials
            string uniqueId = $"{new Random().Next(1, 10)}{Guid.NewGuid().ToString("N").Substring(0, 5)}";

            return $"{initials}{uniqueId}";  // Ex: RVH81f3b6

        }






    // creating a function to get one years' claims from the previous method's data list

    /*public List<ClaimsModel> GetOneYearsClaims(string filePath, string year)
    {

        var data = ReadClaimsExcel(filePath);

        var filteredClaims = data.Where( c => c.PolicyYearCol == year ).ToList();

        return filteredClaims;

    }*/


    // This method returns a dictionary where the keys are the policy years and the values are objects containing all calculated summary data for that policy year.
    public Dictionary<string, PolicyYearSummary> GetPolicyYearSummaries(string filePath)
        {
            var data = ReadClaimsExcel(filePath);

            var result = data
                .GroupBy(record => record.PolicyYearCol) // group by policy year col which sets the key to be the policy year
                .ToDictionary(
                    group => group.Key,
                    group => new PolicyYearSummary // Create a new PolicyYearSummary for each group
                    {
                        PolicyYearName = group.First().PolicyYearCol,
                        ClientName = group.First().ClientNameCol,
                        RentalDaysCOI = group.First().RentalDaysCOICol,
                        RentalDaysNonCOI = group.First().RentalDaysNonCOICol,
                        TurnoverCOI = group.First().TurnoverCOICol,
                        TurnoverNonCOI = group.First().TurnoverNonCOICol,
                        OpenClaims = group.Count(r => r.StatusCol == "O" || r.StatusCol == "Opened" || r.StatusCol == "Active" || r.StatusCol == "Reopened" || r.StatusCol == "ACTIVE"),
                        ClosedClaims = group.Count(r => r.StatusCol == "S" || r.StatusCol == "Settled" || r.StatusCol == "Closed" || r.StatusCol == "Information Only" || r.StatusCol == "SETTLED" || r.StatusCol == "CANCELLED"),
                        Total_AD_Paid = group.Sum(r => r.AD_PaidCol),
                        Total_FT_Paid = group.Sum(r => r.FT_PaidCol),
                        Total_TPPD_Paid = group.Sum(r => r.TPPD_PaidCol),
                        Total_TPCH_Paid = group.Sum(r => r.TPCH_PaidCol),
                        Total_TPPI_Paid = group.Sum(r => r.TPPI_PaidCol),
                        Total_AD_OS = group.Sum(r => r.ADOSCol),
                        Total_FT_OS = group.Sum(r => r.FTOSCol),
                        Total_TPPD_OS = group.Sum(r => r.TPPD_OSCol),
                        Total_TPCH_OS = group.Sum(r => r.TPCH_OSCol),
                        Total_TPPI_OS = group.Sum(r => r.TPPI_OSCol),
                        Total = group.Sum(r => r.TotalCol),

                        CarNums = group.First().CarNumsCol,
                        VanNums = group.First().VanNumsCol,
                        MinibusNums = group.First().MinibusNumsCol,
                        HGVNums = group.First().HGVNumsCol,
                       
                        CarExposure = carExposure,
                        VanExposure = vanExposure,
                        MinibusExposure = minibusExposure,
                        HGVExposure = hgvExposure,   // hard coded for now

                        FCTO_COI = group.First().FCTO_COICol,
                        FCTO_NonCOI = group.First().FCTO_NonCOICol,
                        FCDaysCOI = group.First().FCDaysCOICol,
                        FCDaysNonCOI = group.First().FCDaysNonCOICol,
                    }
                );

            return result; // Returns a dictionary with policy year as key and PolicyYearSummary as value
        }

        // This method returns each years' summarised data and also the rolling aggregates for three and five years
        public HistoricYearsData Historic3Years5YearsData(string filePath)
        {
            var policyYearSummaries = GetPolicyYearSummaries(filePath);

            var recentYears = policyYearSummaries
                .OrderByDescending(kvp => kvp.Key)  // most recent year first
                .Take(5)  // takes only the available years
                .Select(kvp => kvp.Value)  // extract the values (policyYearSummary objects)
                .ToList();  //having it as a list makes the summaries to be accessable by index

            var year1Data = recentYears.ElementAtOrDefault(0);
            var year2Data = recentYears.ElementAtOrDefault(1);
            var year3Data = recentYears.ElementAtOrDefault(2);
            var year4Data = recentYears.Count > 3 ? recentYears[3] : new PolicyYearSummary();
            var year5Data = recentYears.Count > 4 ? recentYears[4] : new PolicyYearSummary();

            var historicData = new HistoricYearsData
            {
                Year1Name = year1Data.PolicyYearName,
                Year2Name = year2Data.PolicyYearName,
                Year3Name = year3Data.PolicyYearName,
                Year4Name = year4Data.PolicyYearName,
                Year5Name = year5Data.PolicyYearName,

                Y1RentalDaysCOI = year1Data.RentalDaysCOI,
                Y2RentalDaysCOI = year2Data.RentalDaysCOI,
                Y3RentalDaysCOI = year3Data.RentalDaysCOI,
                Y4RentalDaysCOI = year4Data.RentalDaysCOI,
                Y5RentalDaysCOI = year5Data.RentalDaysCOI,
                ThreeY_RDaysCOI = new[] { year1Data, year2Data, year3Data }.Sum(d => d?.RentalDaysCOI ?? 0),
                FiveY_RDaysCOI = new[] { year1Data, year2Data, year3Data, year4Data, year5Data }.Sum(d => d?.RentalDaysCOI ?? 0),

                Y1RentalDaysNonCOI = year1Data.RentalDaysNonCOI,
                Y2RentalDaysNonCOI = year2Data.RentalDaysNonCOI,
                Y3RentalDaysNonCOI = year3Data.RentalDaysNonCOI,
                Y4RentalDaysNonCOI = year4Data.RentalDaysNonCOI,
                Y5RentalDaysNonCOI = year5Data.RentalDaysNonCOI,
                ThreeY_RDaysNonCOI = new[] { year1Data, year2Data, year3Data }.Sum(d => d?.RentalDaysNonCOI ?? 0),
                FiveY_RDaysNonCOI = new[] { year1Data, year2Data, year3Data, year4Data, year5Data }.Sum(d => d?.RentalDaysNonCOI ?? 0),

                Y1TO_COI = year1Data.TurnoverCOI,
                Y2TO_COI = year2Data.TurnoverCOI,
                Y3TO_COI = year3Data.TurnoverCOI,
                Y4TO_COI = year4Data.TurnoverCOI,
                Y5TO_COI = year5Data.TurnoverCOI,
                ThreeY_TO_COI = new[] { year1Data, year2Data, year3Data }.Sum(d => d?.TurnoverCOI ?? 0),
                FiveY_TO_COI = new[] { year1Data, year2Data, year3Data, year4Data, year5Data }.Sum(d => d?.TurnoverCOI ?? 0),

                Y1TO_NonCOI = year1Data.TurnoverNonCOI,
                Y2TO_NonCOI = year2Data.TurnoverNonCOI,
                Y3TO_NonCOI = year3Data.TurnoverNonCOI,
                Y4TO_NonCOI = year4Data.TurnoverNonCOI,
                Y5TO_NonCOI = year5Data.TurnoverNonCOI,
                ThreeY_TO_NonCOI = new[] { year1Data, year2Data, year3Data }.Sum(d => d?.TurnoverNonCOI ?? 0),
                FiveY_TO_NonCOI = new[] { year1Data, year2Data, year3Data, year4Data, year5Data }.Sum(d => d?.TurnoverNonCOI ?? 0),

                Y1ClaimsOpen = year1Data.OpenClaims,
                Y2ClaimsOpen = year2Data.OpenClaims,
                Y3ClaimsOpen = year3Data.OpenClaims,
                Y4ClaimsOpen = year4Data.OpenClaims,
                Y5ClaimsOpen = year5Data.OpenClaims,
                ThreeY_ClaimsOpen = new[] { year1Data, year2Data, year3Data }.Sum(x => x?.OpenClaims ?? 0),
                FiveY_ClaimsOpen = new[] { year1Data, year2Data, year3Data, year4Data, year5Data }.Sum(x => x?.OpenClaims ?? 0),

                Y1ClaimsClosed = year1Data.ClosedClaims,
                Y2ClaimsClosed = year2Data.ClosedClaims,
                Y3ClaimsClosed = year3Data.ClosedClaims,
                Y4ClaimsClosed = year4Data.ClosedClaims,
                Y5ClaimsClosed = year5Data.ClosedClaims,
                ThreeY_ClaimsClo = new[] { year1Data, year2Data, year3Data }.Sum( x => x?.ClosedClaims ?? 0),
                FiveY_ClaimsClo = new[] { year1Data, year2Data, year3Data, year4Data, year5Data }.Sum(x => x?.ClosedClaims ?? 0),

                Y1ADPaid = year1Data.Total_AD_Paid,
                Y2ADPaid = year2Data.Total_AD_Paid,
                Y3ADPaid = year3Data.Total_AD_Paid,
                Y4ADPaid = year4Data.Total_AD_Paid,
                Y5ADPaid = year5Data.Total_AD_Paid,
                ThreeY_AD = new[] {year1Data, year2Data, year3Data }.Sum(x => x?.Total_AD_Paid ?? 0),
                FiveY_AD = new[] {year1Data, year2Data, year3Data, year4Data, year5Data }.Sum(x => x?.Total_AD_Paid ?? 0),

                Y1FTPaid = year1Data.Total_FT_Paid,
                Y2FTPaid = year2Data.Total_FT_Paid,
                Y3FTPaid = year3Data.Total_FT_Paid,
                Y4FTPaid = year4Data.Total_FT_Paid,
                Y5FTPaid = year5Data.Total_FT_Paid,
                ThreeY_FT = new[] { year1Data, year2Data, year3Data }.Sum(x => x?.Total_FT_Paid ?? 0),
                FiveY_FT = new[] { year1Data, year2Data, year3Data, year4Data, year5Data }.Sum(x => x?.Total_FT_Paid ?? 0),

                Y1TPPD = year1Data.Total_TPPD_Paid,
                Y2TPPD = year2Data.Total_TPPD_Paid,
                Y3TPPD = year3Data.Total_TPPD_Paid,
                Y4TPPD = year4Data.Total_TPPD_Paid,
                Y5TPPD = year5Data.Total_TPPD_Paid,
                ThreeY_TPPD = new[] { year1Data, year2Data, year3Data }.Sum(x => x?.Total_TPPD_Paid ?? 0),
                FiveY_TPPD = new[] { year1Data, year2Data, year3Data, year4Data, year5Data }.Sum(x => x?.Total_TPPD_Paid ?? 0),

                Y1TPCH = year1Data.Total_TPCH_Paid,
                Y2TPCH = year2Data.Total_TPCH_Paid,
                Y3TPCH = year3Data.Total_TPCH_Paid,
                Y4TPCH = year4Data.Total_TPCH_Paid,
                Y5TPCH = year5Data.Total_TPCH_Paid,
                ThreeY_TPCH = new[] { year1Data, year2Data, year3Data }.Sum(x => x?.Total_TPCH_Paid ?? 0),
                FiveY_TPCH = new[] { year1Data, year2Data, year3Data, year4Data, year5Data }.Sum(x => x?.Total_TPCH_Paid ?? 0),

                Y1TPPI = year1Data.Total_TPPI_Paid,
                Y2TPPI = year2Data.Total_TPPI_Paid,
                Y3TPPI = year3Data.Total_TPPI_Paid,
                Y4TPPI = year4Data.Total_TPPI_Paid,
                Y5TPPI = year5Data.Total_TPPI_Paid,
                ThreeY_TPPI = new[] { year1Data, year2Data, year3Data }.Sum(x => x?.Total_TPPI_Paid ?? 0),
                FiveY_TPPI = new[] { year1Data, year2Data, year3Data, year4Data, year5Data }.Sum(x => x?.Total_TPPI_Paid ?? 0),

                Y1ADOS = year1Data.Total_AD_OS,
                Y2ADOS = year2Data.Total_AD_OS,
                Y3ADOS = year3Data.Total_AD_OS,
                Y4ADOS = year4Data.Total_AD_OS,
                Y5ADOS = year5Data.Total_AD_OS,
                ThreeY_ADOS = new[] { year1Data, year2Data, year3Data }.Sum(x => x?.Total_AD_OS ?? 0),
                FiveY_ADOS = new[] { year1Data, year2Data, year3Data, year4Data, year5Data }.Sum(x => x?.Total_AD_OS ?? 0),

                Y1FTOS = year1Data.Total_FT_OS,
                Y2FTOS = year2Data.Total_FT_OS,
                Y3FTOS = year3Data.Total_FT_OS,
                Y4FTOS = year4Data.Total_FT_OS,
                Y5FTOS = year5Data.Total_FT_OS,
                ThreeY_FTOS = new[] { year1Data, year2Data, year3Data }.Sum(x => x?.Total_FT_OS ?? 0),
                FiveY_FTOS  = new[] { year1Data, year2Data, year3Data, year4Data, year5Data }.Sum(x => x?.Total_FT_OS ?? 0),

                Y1TPPDOS = year1Data.Total_TPPD_OS,
                Y2TPPDOS = year2Data.Total_TPPD_OS,
                Y3TPPDOS = year3Data.Total_TPPD_OS,
                Y4TPPDOS = year4Data.Total_TPPD_OS,
                Y5TPPDOS = year5Data.Total_TPPD_OS,
                ThreeY_TPPDOS = new[] { year1Data, year2Data, year3Data }.Sum(x => x?.Total_TPPD_OS ?? 0),
                FiveY_TPPDOS = new[] { year1Data, year2Data, year3Data, year4Data, year5Data }.Sum(x => x?.Total_FT_OS ?? 0),

                Y1TPCHOS = year1Data.Total_TPCH_OS,
                Y2TPCHOS = year2Data.Total_TPCH_OS,
                Y3TPCHOS = year3Data.Total_TPCH_OS,
                Y4TPCHOS = year4Data.Total_TPCH_OS,
                Y5TPCHOS = year5Data.Total_TPCH_OS,
                ThreeY_TPCHOS = new[] { year1Data, year2Data, year3Data }.Sum(x => x?.Total_TPCH_OS ?? 0),
                FiveY_TPCHOS = new[] { year1Data, year2Data, year3Data, year4Data, year5Data }.Sum(x => x?.Total_TPCH_OS ?? 0),

                Y1TPPIOS = year1Data.Total_TPPI_OS,
                Y2TPPIOS = year2Data.Total_TPPI_OS,
                Y3TPPIOS = year3Data.Total_TPPI_OS,
                Y4TPPIOS = year4Data.Total_TPPI_OS,
                Y5TPPIOS = year5Data.Total_TPPI_OS,
                ThreeY_TPPIOS = new[] { year1Data, year2Data, year3Data }.Sum(x => x?.Total_TPPI_OS ?? 0),
                FiveY_TPPIOS = new[] { year1Data, year2Data, year3Data, year4Data, year5Data }.Sum(x => x?.Total_TPPI_OS ?? 0),
            };

            // ------ Calculate totals row -------------
            var fieldsToSum = new string[]
            {
                "ADPaid", "FTPaid", "TPPD", "TPCH", "TPPI", "ADOS", "FTOS", "TPPDOS", "TPCHOS", "TPPIOS"
            };

            for (int i = 1; i <= 5; i++)
            {
                double total = 0;

                foreach (var field in fieldsToSum)
                {
                    var propertyName = $"Y{i}{field}";
                    var propertyInfo = historicData.GetType().GetProperty(propertyName);

                    if (propertyInfo != null)
                    {
                        var value = (double)(propertyInfo.GetValue(historicData) ?? 0);
                        total += value;
                    }
                }
                var totalProperty = historicData.GetType().GetProperty($"Y{i}Total");
                if (totalProperty != null)
                {
                    totalProperty.SetValue(historicData, total);
                }
            }

            historicData.ThreeY_Total = historicData.Y1Total + historicData.Y2Total + historicData.Y3Total;
            historicData.FiveY_Total = historicData.Y1Total + historicData.Y2Total + historicData.Y3Total + historicData.Y4Total + historicData.Y5Total;

            // vehicle years
            historicData.Y1VYrs = Math.Round((double)historicData.Y1RentalDaysNonCOI / 365);
            historicData.Y2VYrs = Math.Round((double)historicData.Y2RentalDaysNonCOI / 365);
            historicData.Y3VYrs = Math.Round((double)historicData.Y3RentalDaysNonCOI / 365);
            historicData.Y4VYrs = Math.Round((double)historicData.Y4RentalDaysNonCOI / 365);
            historicData.Y5VYrs = Math.Round((double)historicData.Y5RentalDaysNonCOI / 365);
            historicData.ThreeY_VYrs = Math.Round(historicData.Y1VYrs) + Math.Round(historicData.Y2VYrs) + Math.Round(historicData.Y3VYrs);
            historicData.FiveY_VYrs = Math.Round(historicData.Y1VYrs) + Math.Round(historicData.Y2VYrs) + Math.Round(historicData.Y3VYrs) + Math.Round(historicData.Y4VYrs) + Math.Round(historicData.Y5VYrs);

            /*if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }*/

            // return the HistoricYearsData object
            return historicData;
        }


    }

}


/*        public HistoricYearsData GetHistoricYearsData(string filePath)
        {
            var data = ReadClaimsExcel(filePath);

            var groupedByPolicyYear = data.GroupBy(record => record.PolicyYearCol)
                                          .ToDictionary(group => group.Key, group => group.ToList());

            *//*foreach (var group in groupedByPolicyYear)
            {
                Console.WriteLine($"Policy Year: {group.Key}, Count: {group.Value.Count()}");
                foreach (var record in group.Value)
                {
                    Console.WriteLine($"Client: {record.ClientNameCol}, Claims Open: {record.OpenClaims}");
                }
            };*//*
            var policyYears = groupedByPolicyYear.Keys
            .Select(year => new { Original = year, StartYear = int.Parse(year.Split('/')[0]) })
            .OrderByDescending(x => x.StartYear)
            .Select(x => x.Original)
            .ToList();
            
            // this retrieves a list of claims records for the associated year and assigns that list to the yearXData variable
            var year1Data = groupedByPolicyYear.TryGetValue(policyYears[0], out var y1Data) ? y1Data : null;
            var year2Data = groupedByPolicyYear.TryGetValue(policyYears[1], out var y2Data) ? y2Data : null;
            var year3Data = groupedByPolicyYear.TryGetValue(policyYears[2], out var y3Data) ? y3Data : null;            
            var year4Data = groupedByPolicyYear.TryGetValue(policyYears[3], out var y4Data) ? y4Data : null;
            var year5Data = groupedByPolicyYear.TryGetValue(policyYears[4], out var y5Data) ? y5Data : null;

            var historicData = new HistoricYearsData
            {
                Year1Name = year1Data?.FirstOrDefault()?.PolicyYearCol,
                Year2Name = year2Data?.FirstOrDefault()?.PolicyYearCol,
                Year3Name = year3Data?.FirstOrDefault()?.PolicyYearCol,
                Year4Name = year4Data?.FirstOrDefault()?.PolicyYearCol,
                Year5Name = year5Data?.FirstOrDefault()?.PolicyYearCol,

                // Rental Days COI/Non-COI
                Y1RentalDaysCOI = year1Data?.FirstOrDefault()?.RentalDaysCOICol ?? 0,
                Y2RentalDaysCOI = year2Data?.FirstOrDefault()?.RentalDaysCOICol ?? 0,
                Y3RentalDaysCOI = year3Data?.FirstOrDefault()?.RentalDaysCOICol ?? 0,
                Y4RentalDaysCOI = year4Data?.FirstOrDefault()?.RentalDaysCOICol ?? 0,
                Y5RentalDaysCOI = year5Data?.FirstOrDefault()?.RentalDaysCOICol ?? 0,
                ThreeY_RDaysCOI = new[] { year1Data, year2Data, year3Data }.Sum(d => d?.FirstOrDefault()?.RentalDaysCOICol ?? 0),
                FiveY_RDaysCOI = new[] { year1Data, year2Data, year3Data, year4Data, year5Data }.Sum(d => d?.FirstOrDefault()?.RentalDaysCOICol ?? 0),

                Y1RentalDaysNonCOI = year1Data?.FirstOrDefault()?.RentalDaysNonCOICol ?? 0,
                Y2RentalDaysNonCOI = year2Data?.FirstOrDefault()?.RentalDaysNonCOICol ?? 0,
                Y3RentalDaysNonCOI = year3Data?.FirstOrDefault()?.RentalDaysNonCOICol ?? 0,
                Y4RentalDaysNonCOI = year4Data?.FirstOrDefault()?.RentalDaysNonCOICol ?? 0,
                Y5RentalDaysNonCOI = year5Data?.FirstOrDefault()?.RentalDaysNonCOICol ?? 0,
                ThreeY_RDaysNonCOI = new[] { year1Data, year2Data, year3Data }.Sum(d => d?.FirstOrDefault()?.RentalDaysNonCOICol ?? 0),
                FiveY_RDaysNonCOI = new[] { year1Data, year2Data, year3Data, year4Data, year5Data }.Sum(d => d?.FirstOrDefault()?.RentalDaysNonCOICol ?? 0),

                // Turnover
                Y1TO_COI = year1Data?.FirstOrDefault()?.TurnoverCOICol ?? 0,
                Y2TO_COI = year2Data?.FirstOrDefault()?.TurnoverCOICol ?? 0,
                Y3TO_COI = year3Data?.FirstOrDefault()?.TurnoverCOICol ?? 0,
                Y4TO_COI = year4Data?.FirstOrDefault()?.TurnoverCOICol ?? 0,
                Y5TO_COI = year5Data?.FirstOrDefault()?.TurnoverCOICol ?? 0,
                ThreeY_TO_COI = new[] { year1Data, year2Data, year3Data }.Sum(d => d?.FirstOrDefault()?.TurnoverCOICol ?? 0),
                FiveY_TO_COI = new[] { year1Data, year2Data, year3Data, year4Data, year5Data }.Sum(d => d?.FirstOrDefault()?.TurnoverCOICol ?? 0),

                Y1TO_NonCOI = year1Data?.FirstOrDefault()?.TurnoverNonCOICol ?? 0,
                Y2TO_NonCOI = year2Data?.FirstOrDefault()?.TurnoverNonCOICol ?? 0,
                Y3TO_NonCOI = year3Data?.FirstOrDefault()?.TurnoverNonCOICol ?? 0,
                Y4TO_NonCOI = year4Data?.FirstOrDefault()?.TurnoverNonCOICol ?? 0,
                Y5TO_NonCOI = year5Data?.FirstOrDefault()?.TurnoverNonCOICol ?? 0,
                ThreeY_TO_NonCOI = new[] { year1Data, year2Data, year3Data }.Sum(d => d?.FirstOrDefault()?.TurnoverNonCOICol ?? 0),
                FiveY_TO_NonCOI = new[] { year1Data, year2Data, year3Data, year4Data, year5Data }.Sum(d => d?.FirstOrDefault()?.TurnoverNonCOICol ?? 0),

                // open & closed claims
                Y1ClaimsOpen = year1Data?.Count(r => r.StatusCol == "O") ?? 0,
                Y2ClaimsOpen = year2Data?.Count(r => r.StatusCol == "O") ?? 0,
                Y3ClaimsOpen = year3Data?.Count(r => r.StatusCol == "O") ?? 0,
                Y4ClaimsOpen = year4Data?.Count(r => r.StatusCol == "O") ?? 0,
                Y5ClaimsOpen = year5Data?.Count(r => r.StatusCol == "O") ?? 0,
                ThreeY_ClaimsOpen = new[] { year1Data, year2Data, year3Data }.Sum(d => d.Count(r => r.StatusCol == "O")),
                FiveY_ClaimsOpen = new[] { year1Data, year2Data, year3Data, year4Data, year5Data }.Sum(d => d.Count(r => r.StatusCol == "O")),

                Y1ClaimsClosed = year1Data?.Count(r => r.StatusCol == "S") ?? 0,
                Y2ClaimsClosed = year2Data?.Count(r => r.StatusCol == "S") ?? 0,
                Y3ClaimsClosed = year3Data?.Count(r => r.StatusCol == "S") ?? 0,
                Y4ClaimsClosed = year4Data?.Count(r => r.StatusCol == "S") ?? 0,
                Y5ClaimsClosed = year5Data?.Count(r => r.StatusCol == "S") ?? 0,
                ThreeY_ClaimsClo = new[] { year1Data, year2Data, year3Data }.Sum(d => d.Count(r => r.StatusCol == "S")),
                FiveY_ClaimsClo = new[] { year1Data, year2Data, year3Data, year4Data, year5Data }.Sum(d => d.Count(r => r.StatusCol == "S")),

                // AD Paid
                Y1ADPaid = year1Data.Sum(r => r.AD_PaidCol),
                Y2ADPaid = year2Data.Sum(r => r.AD_PaidCol),
                Y3ADPaid = year3Data.Sum(r => r.AD_PaidCol),
                Y4ADPaid = year4Data.Sum(r => r.AD_PaidCol),
                Y5ADPaid = year5Data.Sum(r => r.AD_PaidCol),
                ThreeY_AD = new[] { year1Data, year2Data, year3Data }.Sum(d => d.Sum(r => r.AD_PaidCol)),
                FiveY_AD = new[] { year1Data, year2Data, year3Data, year4Data, year5Data }.Sum(d => d.Sum(r => r.AD_PaidCol)),

                // F&T Paid
                Y1FTPaid = year1Data.Sum(r => r.FT_PaidCol),
                Y2FTPaid = year2Data.Sum(r => r.FT_PaidCol),
                Y3FTPaid = year3Data.Sum(r => r.FT_PaidCol),
                Y4FTPaid = year4Data.Sum(r => r.FT_PaidCol),
                Y5FTPaid = year5Data.Sum(r => r.FT_PaidCol),
                ThreeY_FT = new[] { year1Data, year2Data, year3Data }.Sum(d => d.Sum(r => r.FT_PaidCol)),
                FiveY_FT = new[] { year1Data, year2Data, year3Data, year4Data, year5Data }.Sum(d => d.Sum(r => r.FT_PaidCol)),

                // TP PD Paid
                Y1TPPD = year1Data.Sum(r => r.TPPD_PaidCol),
                Y2TPPD = year2Data.Sum(r => r.TPPD_PaidCol),
                Y3TPPD = year3Data.Sum(r => r.TPPD_PaidCol),
                Y4TPPD = year4Data.Sum(r => r.TPPD_PaidCol),
                Y5TPPD = year5Data.Sum(r => r.TPPD_PaidCol),
                ThreeY_TPPD = new[] { year1Data, year2Data, year3Data }.Sum(d => d.Sum(r => r.TPPD_PaidCol)),
                FiveY_TPPD = new[] { year1Data, year2Data, year3Data, year4Data, year5Data }.Sum(d => d.Sum(r => r.TPPD_PaidCol)),

                // TP CH Paid
                Y1TPCH = year1Data.Sum(r => r.TPCH_PaidCol),
                Y2TPCH = year2Data.Sum(r => r.TPCH_PaidCol),
                Y3TPCH = year3Data.Sum(r => r.TPCH_PaidCol),
                Y4TPCH = year4Data.Sum(r => r.TPCH_PaidCol),
                Y5TPCH = year5Data.Sum(r => r.TPCH_PaidCol),
                ThreeY_TPCH = new[] { year1Data, year2Data, year3Data }.Sum(d => d.Sum(r => r.TPCH_PaidCol)),
                FiveY_TPCH = new[] { year1Data, year2Data, year3Data, year4Data, year5Data }.Sum(d => d.Sum(r => r.TPCH_PaidCol)),

                // TP PI Paid
                Y1TPPI = year1Data.Sum(r => r.TPPI_PaidCol),
                Y2TPPI = year2Data.Sum(r => r.TPPI_PaidCol),
                Y3TPPI = year3Data.Sum(r => r.TPPI_PaidCol),
                Y4TPPI = year4Data.Sum(r => r.TPPI_PaidCol),
                Y5TPPI = year5Data.Sum(r => r.TPPI_PaidCol),
                ThreeY_TPPI = new[] { year1Data, year2Data, year3Data }.Sum(d => d.Sum(r => r.TPPI_PaidCol)),
                FiveY_TPPI = new[] { year1Data, year2Data, year3Data, year4Data, year5Data }.Sum(d => d.Sum(r => r.TPPI_PaidCol)),

                // AD OS
                Y1ADOS = year1Data.Sum(r => r.ADOSCol),
                Y2ADOS = year2Data.Sum(r => r.ADOSCol),
                Y3ADOS = year3Data.Sum(r => r.ADOSCol),
                Y4ADOS = year4Data.Sum(r => r.ADOSCol),
                Y5ADOS = year5Data.Sum(r => r.ADOSCol),
                ThreeY_ADOS = new[] { year1Data, year2Data, year3Data }.Sum(d => d.Sum(r => r.ADOSCol)),
                FiveY_ADOS = new[] { year1Data, year2Data, year3Data, year4Data, year5Data }.Sum(d => d.Sum(r => r.ADOSCol)),

                // F&T OS
                Y1FTOS = year1Data.Sum(r => r.FTOSCol),
                Y2FTOS = year2Data.Sum(r => r.FTOSCol),
                Y3FTOS = year3Data.Sum(r => r.FTOSCol),
                Y4FTOS = year4Data.Sum(r => r.FTOSCol),
                Y5FTOS = year5Data.Sum(r => r.FTOSCol),
                ThreeY_FTOS = new[] { year1Data, year2Data, year3Data }.Sum(d => d.Sum(r => r.FTOSCol)),
                FiveY_FTOS = new[] { year1Data, year2Data, year3Data, year4Data, year5Data }.Sum(d => d.Sum(r => r.FTOSCol)),

                // TP PD OS
                Y1TPPDOS = year1Data.Sum(r => r.TPPD_OSCol),
                Y2TPPDOS = year2Data.Sum(r => r.TPPD_OSCol),
                Y3TPPDOS = year3Data.Sum(r => r.TPPD_OSCol),
                Y4TPPDOS = year4Data.Sum(r => r.TPPD_OSCol),
                Y5TPPDOS = year5Data.Sum(r => r.TPPD_OSCol),
                ThreeY_TPPDOS = new[] { year1Data, year2Data, year3Data }.Sum(d => d.Sum(r => r.TPPD_OSCol)),
                FiveY_TPPDOS = new[] { year1Data, year2Data, year3Data, year4Data, year5Data }.Sum(d => d.Sum(r => r.TPPD_OSCol)),

                // TP CH OS
                Y1TPCHOS = year1Data.Sum(r => r.TPCH_OSCol),
                Y2TPCHOS = year2Data.Sum(r => r.TPCH_OSCol),
                Y3TPCHOS = year3Data.Sum(r => r.TPCH_OSCol),
                Y4TPCHOS = year4Data.Sum(r => r.TPCH_OSCol),
                Y5TPCHOS = year5Data.Sum(r => r.TPCH_OSCol),
                ThreeY_TPCHOS = new[] { year1Data, year2Data, year3Data }.Sum(d => d.Sum(r => r.TPCH_OSCol)),
                FiveY_TPCHOS = new[] { year1Data, year2Data, year3Data, year4Data, year5Data }.Sum(d => d.Sum(r => r.TPCH_OSCol)),

                // TP PI OS
                Y1TPPIOS = year1Data.Sum(r => r.TPPI_OSCol),
                Y2TPPIOS = year2Data.Sum(r => r.TPPI_OSCol),
                Y3TPPIOS = year3Data.Sum(r => r.TPPI_OSCol),
                Y4TPPIOS = year4Data.Sum(r => r.TPPI_OSCol),
                Y5TPPIOS = year5Data.Sum(r => r.TPPI_OSCol),
                ThreeY_TPPIOS = new[] { year1Data, year2Data, year3Data }.Sum(d => d.Sum(r => r.TPPI_OSCol)),
                FiveY_TPPIOS = new[] { year1Data, year2Data, year3Data, year4Data, year5Data }.Sum(d => d.Sum(r => r.TPPI_OSCol)),

            };

            

            // ------ Calculate totals row -------------
            var fieldsToSum = new string[]
            {
                "ADPaid", "FTPaid", "TPPD", "TPCH", "TPPI", "ADOS", "FTOS", "TPPDOS", "TPCHOS", "TPPIOS"
            };

            for (int i = 1; i <= 5; i++)
            {
                double total = 0;

                foreach (var field in fieldsToSum)
                {
                    var propertyName = $"Y{i}{field}";
                    var propertyInfo = historicData.GetType().GetProperty(propertyName);

                    if (propertyInfo != null)
                    {
                        var value = (double)(propertyInfo.GetValue(historicData) ?? 0);
                        total += value;
                    }
                }
                var totalProperty = historicData.GetType().GetProperty($"Y{i}Total");
                if (totalProperty != null)
                {
                    totalProperty.SetValue(historicData, total);
                }
            }

            historicData.ThreeY_Total = historicData.Y1Total + historicData.Y2Total + historicData.Y3Total;
            historicData.FiveY_Total = historicData.Y1Total + historicData.Y2Total + historicData.Y3Total + historicData.Y4Total + historicData.Y5Total;

            historicData.Y1VYrs = Math.Round((double)historicData.Y1RentalDaysNonCOI / 365);
            historicData.Y2VYrs = Math.Round((double)historicData.Y2RentalDaysNonCOI / 365);
            historicData.Y3VYrs = Math.Round((double)historicData.Y3RentalDaysNonCOI / 365);
            historicData.Y4VYrs = Math.Round((double)historicData.Y4RentalDaysNonCOI / 365);
            historicData.Y5VYrs = Math.Round((double)historicData.Y5RentalDaysNonCOI / 365);
            historicData.ThreeY_VYrs = Math.Round(historicData.Y1VYrs) + Math.Round(historicData.Y2VYrs) + Math.Round(historicData.Y3VYrs);
            historicData.FiveY_VYrs = Math.Round(historicData.Y1VYrs) + Math.Round(historicData.Y2VYrs) + Math.Round(historicData.Y3VYrs) + Math.Round(historicData.Y4VYrs) + Math.Round(historicData.Y5VYrs);


            return historicData; // Return the populated HistoricYearsData instance
        }*/

 

/* public Dictionary<string, double> GetSumByPolicyYear(string filePath)
 {
     var data = ReadExcel(filePath); // This calls your existing method to read the Excel file

     // Grouping by PolicyYearCol and summing the TotalCol for each group
     var result = data
         .GroupBy(record => record.PolicyYearCol) // Group by policy year
         .ToDictionary(
             group => group.Key,                 // The key (PolicyYearCol)
             group => group.Sum(r => r.TotalCol) // Sum of TotalCol for that policy year
         );

     return result; // Returns a dictionary with policy year as key and sum of totals as value
 }

 public Dictionary<string, (int OpenClaims, int ClosedClaims)> GetSumOfStatusOfClaims(string filePath)
 {
     var data = ReadExcel(filePath); 

     var result = data
         .GroupBy(record => record.PolicyYearCol) 
         .ToDictionary(
             group => group.Key, 
             group => (          
                 OpenClaims: group.Count(r => r.StatusCol == "O"), 
                 ClosedClaims: group.Count(r => r.StatusCol == "S")  
         )
 );

     return result; 
 }*/
