using CSV_reader.Models;
using CSV_reader.ViewModels;
using CSV_reader.database;
using Microsoft.AspNetCore.Http.HttpResults;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.ComponentModel;
using Microsoft.Identity.Client.Cache;
using System.Threading.Tasks.Dataflow;
using Microsoft.AspNetCore.Mvc.Rendering;
using iText.Layout.Element;

namespace CSV_reader.Services
{
    public class CalculationsService : ICalculationsService
    {
        private readonly ApplicationContext _appContext;
        private readonly IWebHostEnvironment _env;
        private readonly IClaimsService _claimsService;

        public CalculationsService(ApplicationContext appContext, IWebHostEnvironment env, IClaimsService claimsService)
        {
            _appContext = appContext;
            _env = env;
            _claimsService = claimsService;
        }

        double accidentFreq3Year;
        double accidentFreq5Year;
        double InfCCPVYTotal3Year;
        double InfCCPVYTotal5Year;
        double InfCPID3Year;
        double InfCPID5Year;
        double InfCPC3Year;

        public CalculationsModel GetCalculations(int quoteId, string selectedNumOfMonths, string projYears, string chargeCOIFee, string pricingMetric, string priceBy) 
        {
            // Retrieve data from the database based on quoteId
            var tableData = _appContext.ClientDetails.FirstOrDefault(x => x.Id == quoteId);

            Console.WriteLine($"quoteId: {quoteId}");

            if (tableData == null)
            {
                throw new InvalidOperationException("table data is null");
            }



            var calculationsModel = new CalculationsModel();
            // this creates a new instance of the CalculationsModel class as a variable
            // and holds the results of the calculations using db data

            calculationsModel.ClientName = tableData.ClientName;
            calculationsModel.QuoteNumber = $"PAQ{tableData.Id}";  // PAQ = Pretium Agency Quote

            // --------------- historic years data ----------------
            // the following code is assigning the table data, which is the data from the excel file, to the historic years data properties in the CalculationsModel. This is done to display the historic data table within the IndexCalculations view.
            
            calculationsModel.Y1RentalDaysCOI = tableData.Y1RentalDaysCOI;
            calculationsModel.Y2RentalDaysCOI = tableData.Y2RentalDaysCOI;
            calculationsModel.Y3RentalDaysCOI = tableData.Y3RentalDaysCOI;
            calculationsModel.Y4RentalDaysCOI = tableData.Y4RentalDaysCOI;
            calculationsModel.Y5RentalDaysCOI = tableData.Y5RentalDaysCOI;
            calculationsModel.ThreeY_RDaysCOI = tableData.ThreeY_RDaysCOI;
            calculationsModel.FiveY_RDaysCOI = tableData.FiveY_RDaysCOI;
            
            calculationsModel.Y1RentalDaysNonCOI = tableData.Y1RentalDaysNonCOI;
            calculationsModel.Y2RentalDaysNonCOI = tableData.Y2RentalDaysNonCOI;
            calculationsModel.Y3RentalDaysNonCOI = tableData.Y3RentalDaysNonCOI;
            calculationsModel.Y4RentalDaysNonCOI = tableData.Y4RentalDaysNonCOI;
            calculationsModel.Y5RentalDaysNonCOI = tableData.Y5RentalDaysNonCOI;
            calculationsModel.ThreeY_RDaysNonCOI = tableData.ThreeY_RDaysNonCOI;
            calculationsModel.FiveY_RDaysNonCOI = tableData.FiveY_RDaysNonCOI;

            calculationsModel.Y1TO_COI = tableData.Y1TO_COI;
            calculationsModel.Y2TO_COI = tableData.Y2TO_COI;
            calculationsModel.Y3TO_COI = tableData.Y3TO_COI;
            calculationsModel.Y4TO_COI = tableData.Y4TO_COI;
            calculationsModel.Y5TO_COI = tableData.Y5TO_COI;
            calculationsModel.ThreeY_TO_COI = tableData.ThreeY_TO_COI;
            calculationsModel.FiveY_TO_COI = tableData.FiveY_TO_COI;

            calculationsModel.Y1TO_NonCOI = tableData.Y1TO_NonCOI;
            calculationsModel.Y2TO_NonCOI = tableData.Y2TO_NonCOI;
            calculationsModel.Y3TO_NonCOI = tableData.Y3TO_NonCOI;
            calculationsModel.Y4TO_NonCOI = tableData.Y4TO_NonCOI;
            calculationsModel.Y5TO_NonCOI = tableData.Y5TO_NonCOI;
            calculationsModel.ThreeY_TO_NonCOI = tableData.ThreeY_TO_NonCOI;
            calculationsModel.FiveY_TO_NonCOI = tableData.FiveY_TO_NonCOI;

            calculationsModel.Y1UT = tableData.Y1UT;
            calculationsModel.Y2UT = tableData.Y2UT;
            calculationsModel.Y3UT = tableData.Y3UT;
            calculationsModel.Y4UT = tableData.Y4UT;
            calculationsModel.Y5UT = tableData.Y5UT;
            calculationsModel.ThreeY_UT = tableData.ThreeY_UT;
            calculationsModel.FiveY_UT = tableData.FiveY_UT;

            calculationsModel.Y1VYrs = tableData.Y1VYrs;
            calculationsModel.Y2VYrs = tableData.Y2VYrs;
            calculationsModel.Y3VYrs = tableData.Y3VYrs;
            calculationsModel.Y4VYrs = tableData.Y4VYrs;
            calculationsModel.Y5VYrs = tableData.Y5VYrs;
            calculationsModel.ThreeY_VYrs = tableData.ThreeY_VYrs;
            calculationsModel.FiveY_VYrs = tableData.FiveY_VYrs;

            calculationsModel.Y1ClaimsOpen = tableData.Y1ClaimsOpen;
            calculationsModel.Y2ClaimsOpen = tableData.Y2ClaimsOpen;
            calculationsModel.Y3ClaimsOpen = tableData.Y3ClaimsOpen;
            calculationsModel.Y4ClaimsOpen = tableData.Y4ClaimsOpen;
            calculationsModel.Y5ClaimsOpen = tableData.Y5ClaimsOpen;
            calculationsModel.ThreeY_ClaimsOpen = tableData.ThreeY_ClaimsOpen;
            calculationsModel.FiveY_ClaimsOpen = tableData.FiveY_ClaimsOpen;

            calculationsModel.Y1ClaimsClosed = tableData.Y1ClaimsClosed;
            calculationsModel.Y2ClaimsClosed = tableData.Y2ClaimsClosed;
            calculationsModel.Y3ClaimsClosed = tableData.Y3ClaimsClosed;
            calculationsModel.Y4ClaimsClosed = tableData.Y4ClaimsClosed;
            calculationsModel.Y5ClaimsClosed = tableData.Y5ClaimsClosed;
            calculationsModel.ThreeY_ClaimsClo = tableData.ThreeY_ClaimsClo;
            calculationsModel.FiveY_ClaimsClo = tableData.FiveY_ClaimsClo;

            calculationsModel.Y1ADPaid = tableData.Y1ADPaid;
            calculationsModel.Y2ADPaid = tableData.Y2ADPaid;
            calculationsModel.Y3ADPaid = tableData.Y3ADPaid;
            calculationsModel.Y4ADPaid = tableData.Y4ADPaid;
            calculationsModel.Y5ADPaid = tableData.Y5ADPaid;
            calculationsModel.ThreeY_AD = tableData.ThreeY_AD;
            calculationsModel.FiveY_AD = tableData.FiveY_AD;

            calculationsModel.Y1FTPaid = tableData.Y1FTPaid;
            calculationsModel.Y2FTPaid = tableData.Y2FTPaid;
            calculationsModel.Y3FTPaid = tableData.Y3FTPaid;
            calculationsModel.Y4FTPaid = tableData.Y4FTPaid;
            calculationsModel.Y5FTPaid = tableData.Y5FTPaid;
            calculationsModel.ThreeY_FT = tableData.ThreeY_FT;
            calculationsModel.FiveY_FT = tableData.FiveY_FT;

            calculationsModel.Y1TPPD = tableData.Y1TPPD;
            calculationsModel.Y2TPPD = tableData.Y2TPPD;
            calculationsModel.Y3TPPD = tableData.Y3TPPD;
            calculationsModel.Y4TPPD = tableData.Y4TPPD;
            calculationsModel.Y5TPPD = tableData.Y5TPPD;
            calculationsModel.ThreeY_TPPD = tableData.ThreeY_TPPD;
            calculationsModel.FiveY_TPPD = tableData.FiveY_TPPD;

            calculationsModel.Y1TPCH = tableData.Y1TPCH;
            calculationsModel.Y2TPCH = tableData.Y2TPCH;
            calculationsModel.Y3TPCH = tableData.Y3TPCH;
            calculationsModel.Y4TPCH = tableData.Y4TPCH;
            calculationsModel.Y5TPCH = tableData.Y5TPCH;
            calculationsModel.ThreeY_TPCH = tableData.ThreeY_TPCH;
            calculationsModel.FiveY_TPCH = calculationsModel.FiveY_TPCH;

            calculationsModel.Y1TPPI = calculationsModel.Y1TPPI;
            calculationsModel.Y2TPPI = calculationsModel.Y2TPPI;
            calculationsModel.Y3TPPI = calculationsModel.Y3TPPI;
            calculationsModel.Y4TPPI = calculationsModel.Y4TPPI;
            calculationsModel.Y5TPPI = calculationsModel.Y5TPPI;
            calculationsModel.ThreeY_TPPI = calculationsModel.ThreeY_TPPI;
            calculationsModel.FiveY_TPPI = calculationsModel.FiveY_TPPI;

            calculationsModel.Y1ADOS = calculationsModel.Y1ADOS;
            calculationsModel.Y2ADOS = calculationsModel.Y2ADOS;
            calculationsModel.Y3ADOS = calculationsModel.Y3ADOS;
            calculationsModel.Y4ADOS = calculationsModel.Y4ADOS;
            calculationsModel.Y5ADOS = calculationsModel.Y5ADOS;
            calculationsModel.ThreeY_ADOS = calculationsModel.ThreeY_ADOS;
            calculationsModel.FiveY_ADOS = calculationsModel.FiveY_ADOS;

            calculationsModel.Y1FTOS = tableData.Y1FTOS;
            calculationsModel.Y2FTOS = tableData.Y2FTOS;
            calculationsModel.Y3FTOS = tableData.Y3FTOS;
            calculationsModel.Y4FTOS = tableData.Y4FTOS;
            calculationsModel.Y5FTOS = tableData.Y5FTOS;
            calculationsModel.ThreeY_FTOS = tableData.ThreeY_FTOS;
            calculationsModel.FiveY_FTOS = calculationsModel.FiveY_FTOS;

            calculationsModel.Y1TPPDOS = tableData.Y1TPPDOS;
            calculationsModel.Y2TPPDOS = tableData.Y2TPPDOS;
            calculationsModel.Y3TPPDOS = tableData.Y3TPPDOS;
            calculationsModel.Y4TPPDOS = tableData.Y4TPPDOS;
            calculationsModel.Y5TPPDOS = tableData.Y5TPPDOS;
            calculationsModel.ThreeY_TPPDOS = tableData.ThreeY_TPPDOS;
            calculationsModel.FiveY_TPPDOS = tableData.FiveY_TPPDOS;

            calculationsModel.Y1TPCHOS = tableData.Y1TPCHOS;
            calculationsModel.Y2TPCHOS = tableData.Y2TPCHOS;
            calculationsModel.Y3TPCHOS = tableData.Y3TPCHOS;
            calculationsModel.Y4TPCHOS = tableData.Y4TPCHOS;
            calculationsModel.Y5TPCHOS = tableData.Y5TPCHOS;
            calculationsModel.ThreeY_TPCHOS = tableData.ThreeY_TPCHOS;
            calculationsModel.FiveY_TPCHOS = tableData.FiveY_TPCHOS;

            calculationsModel.Y1TPPIOS = tableData.Y1TPPIOS;
            calculationsModel.Y2TPPIOS = tableData.Y2TPPIOS;
            calculationsModel.Y3TPPIOS = tableData.Y3TPPIOS;
            calculationsModel.Y4TPPIOS = tableData.Y4TPPIOS;
            calculationsModel.Y5TPPIOS = tableData.Y5TPPIOS;
            calculationsModel.ThreeY_TPPIOS = tableData.ThreeY_TPPIOS;
            calculationsModel.FiveY_TPPIOS = tableData.FiveY_TPPIOS;

            calculationsModel.Y1Total = tableData.Y1Total;
            calculationsModel.Y2Total = tableData.Y2Total;
            calculationsModel.Y3Total = tableData.Y3Total;
            calculationsModel.Y4Total = tableData.Y4Total;
            calculationsModel.Y5Total = tableData.Y5Total;
            calculationsModel.ThreeY_Total = tableData.ThreeY_Total;
            calculationsModel.FiveY_Total = tableData.FiveY_Total;


            // Parse the selectedNumOfMonths and assign it to InfMonth
            calculationsModel.SelectedNumOfMonths = selectedNumOfMonths;
            if (int.TryParse(selectedNumOfMonths, out int parsedMonths))
            {
                calculationsModel.InfMonth = parsedMonths;
            }
            else
            {
                // Handle invalid number of months, default to 12 if parsing fails
                calculationsModel.InfMonth = 12;
            }
            calculationsModel.SelectedProjYears = projYears;
            if (int.TryParse(projYears, out int parsedYears))
            {
                calculationsModel.ProjYear = parsedYears;
            }
            else
            {
                calculationsModel.ProjYear = 3;
            }
            calculationsModel.SelectedCOIFee = chargeCOIFee;
            if (int.TryParse(chargeCOIFee, out int parsedCOIFee))
            {
                calculationsModel.ChargeCOIFeeValue = parsedCOIFee;
            }
            else
            {
                calculationsModel.ChargeCOIFeeValue = 1;
            }
            calculationsModel.SelectedPricingMetric = pricingMetric;
            if (int.TryParse(pricingMetric, out int parsedPricingMetric))
            {
                calculationsModel.PricingMetricValue = parsedPricingMetric;
            }
            else
            {
                calculationsModel.PricingMetricValue = 1;
            }
            calculationsModel.SelectedPriceBy = priceBy;
            if (int.TryParse(priceBy, out int parsedPriceBy))
            {
                calculationsModel.PriceByValue = parsedPriceBy;
            }
            else
            {
                calculationsModel.PriceByValue = 1;
            }

            // Accident Frequency is calculated by dividing the number of claims by the Vehicle Years. This will be picked up from the 3 or 5 year line of the top section of the screen.
            // multiplied by 100 to show it as a percentage
            calculationsModel.Y1AccFreq = tableData.Y1VYrs == 0 ? 0 :(double)(tableData.Y1ClaimsOpen + tableData.Y1ClaimsClosed) / tableData.Y1VYrs * 100;
            calculationsModel.Y2AccFreq = tableData.Y2VYrs == 0 ? 0 : (double)(tableData.Y2ClaimsOpen + tableData.Y2ClaimsClosed) / tableData.Y2VYrs * 100;
            calculationsModel.Y3AccFreq = tableData.Y3VYrs == 0 ? 0 : (double)(tableData.Y3ClaimsOpen + tableData.Y3ClaimsClosed) / tableData.Y3VYrs * 100;
            calculationsModel.Y4AccFreq = tableData.Y4VYrs == 0 ? 0 : (double)(tableData.Y4ClaimsOpen + tableData.Y4ClaimsClosed) / tableData.Y4VYrs * 100;
            calculationsModel.Y5AccFreq = tableData.Y5VYrs == 0 ? 0 : (double)(tableData.Y5ClaimsOpen + tableData.Y5ClaimsClosed) / tableData.Y5VYrs * 100;

            calculationsModel.ThreeY_AccFreq = tableData.ThreeY_VYrs == 0 ? 0 : (double)(tableData.ThreeY_ClaimsOpen + tableData.ThreeY_ClaimsClo) / tableData.ThreeY_VYrs * 100;
            accidentFreq3Year = calculationsModel.ThreeY_AccFreq / 100;
            calculationsModel.FiveY_AccFreq = tableData.FiveY_VYrs == 0? 0 : (double)(tableData.FiveY_ClaimsOpen + tableData.FiveY_ClaimsClo) / tableData.FiveY_VYrs * 100;
            accidentFreq5Year = calculationsModel.FiveY_AccFreq / 100;

            // Cost Per Claims is calculated by taking the Total Incurred Value and dividing it by the Total Number of Claims
            calculationsModel.Y1CPC = (double)tableData.Y1Total / (tableData.Y1ClaimsOpen + tableData.Y1ClaimsClosed);
            calculationsModel.Y2CPC = (double)tableData.Y2Total / (tableData.Y2ClaimsOpen + tableData.Y2ClaimsClosed);
            calculationsModel.Y3CPC = (double)tableData.Y3Total / (tableData.Y3ClaimsOpen + tableData.Y3ClaimsClosed);
            calculationsModel.Y4CPC = (double)tableData.Y4Total / (tableData.Y4ClaimsOpen + tableData.Y4ClaimsClosed);
            calculationsModel.Y5CPC = (double)tableData.Y5Total / (tableData.Y5ClaimsOpen + tableData.Y5ClaimsClosed);

            calculationsModel.ThreeY_CPC = (double)tableData.ThreeY_Total / (tableData.ThreeY_ClaimsOpen + tableData.ThreeY_ClaimsClo);
            calculationsModel.FiveY_CPC = (double)tableData.FiveY_Total / (tableData.FiveY_ClaimsOpen + tableData.FiveY_ClaimsClo);

            // Cost Per Insurable Day is calculated by taking the Total Incurred Value and dividing it by the Total Risk (Insurable) Days.            
            calculationsModel.Y1CPID = (tableData.Y1RentalDaysNonCOI == 0) ? 0 : tableData.Y1Total / tableData.Y1RentalDaysNonCOI;
            calculationsModel.Y2CPID = (tableData.Y2RentalDaysNonCOI == 0) ? 0 : tableData.Y2Total / tableData.Y2RentalDaysNonCOI;
            calculationsModel.Y3CPID = (tableData.Y3RentalDaysNonCOI == 0) ? 0 : tableData.Y3Total / tableData.Y3RentalDaysNonCOI;
            calculationsModel.Y4CPID = (tableData.Y4RentalDaysNonCOI == 0) ? 0 : tableData.Y4Total / tableData.Y4RentalDaysNonCOI;
            calculationsModel.Y5CPID = (tableData.Y5RentalDaysNonCOI == 0) ? 0 : tableData.Y5Total / tableData.Y5RentalDaysNonCOI;

            calculationsModel.ThreeY_CPID = (tableData.ThreeY_RDaysNonCOI == 0) ? 0 : tableData.ThreeY_Total / tableData.ThreeY_RDaysNonCOI;
            calculationsModel.FiveY_CPID = (tableData.FiveY_RDaysNonCOI == 0) ? 0 : tableData.FiveY_Total / tableData.FiveY_RDaysNonCOI;

            // Claims Cost by Turnover is calculated by taking the Total Incurred Value and dividing it by (Risk Turnover/1000). If we are not provided with turnover information from the client, this will be fixed at zero.
            calculationsModel.Y1CCTO = (tableData.Y1TO_NonCOI == 0) ? 0 : tableData.Y1Total / (tableData.Y1TO_NonCOI / 1000);
            calculationsModel.Y2CCTO = (tableData.Y2TO_NonCOI == 0) ? 0 : tableData.Y2Total / (tableData.Y2TO_NonCOI / 1000);
            calculationsModel.Y3CCTO = (tableData.Y3TO_NonCOI == 0) ? 0 : tableData.Y3Total / (tableData.Y3TO_NonCOI / 1000);
            calculationsModel.Y4CCTO = (tableData.Y4TO_NonCOI == 0) ? 0 : tableData.Y4Total / (tableData.Y4TO_NonCOI / 1000);
            calculationsModel.Y5CCTO = (tableData.Y5TO_NonCOI == 0) ? 0 : tableData.Y5Total / (tableData.Y5TO_NonCOI / 1000);

            calculationsModel.ThreeY_CCTO = (tableData.ThreeY_TO_NonCOI == 0) ? 0 : tableData.ThreeY_Total / (tableData.ThreeY_TO_NonCOI / 1000);
            calculationsModel.FiveY_CCTO = (tableData.FiveY_TO_NonCOI == 0) ? 0 : tableData.FiveY_Total / (tableData.FiveY_TO_NonCOI / 1000);

            // Claims Cost per Vehicle Year AD is calculated by taking the (AD Paid + AD Outstanding) and dividing by the Vehicle Years.
            calculationsModel.Y1_CCPVY_AD = tableData.Y1VYrs == 0 ? 0 : (tableData.Y1ADPaid + tableData.Y1ADOS) / tableData.Y1VYrs;
            calculationsModel.Y2_CCPVY_AD = tableData.Y2VYrs == 0 ? 0 : (tableData.Y2ADPaid + tableData.Y2ADOS) / tableData.Y2VYrs;
            calculationsModel.Y3_CCPVY_AD = tableData.Y3VYrs == 0 ? 0 : (tableData.Y3ADPaid + tableData.Y3ADOS) / tableData.Y3VYrs;
            calculationsModel.Y4_CCPVY_AD = tableData.Y4VYrs == 0 ? 0 : (tableData.Y4ADPaid + tableData.Y4ADOS) / tableData.Y4VYrs;
            calculationsModel.Y5_CCPVY_AD = tableData.Y5VYrs == 0 ? 0 : (tableData.Y5ADPaid + tableData.Y5ADOS) / tableData.Y5VYrs;

            calculationsModel.ThreeY_CCPVY_AD = (calculationsModel.Y1_CCPVY_AD + calculationsModel.Y2_CCPVY_AD + calculationsModel.Y3_CCPVY_AD) / 3;
            calculationsModel.FiveY_CCPVY_AD = (calculationsModel.Y1_CCPVY_AD + calculationsModel.Y2_CCPVY_AD + calculationsModel.Y3_CCPVY_AD + calculationsModel.Y4_CCPVY_AD + calculationsModel.Y5_CCPVY_AD) / 5;

            // Claims Cost per Vehicle Year TP is calculated by taking the (TP Paid + TP Outstanding) and dividing by the Vehicle Years.
            calculationsModel.Y1_CCPVY_TP = tableData.Y1VYrs == 0 ? 0 : (tableData.Y1TPPD + tableData.Y1TPCH + tableData.Y1TPPI + tableData.Y1TPPDOS + tableData.Y1TPCHOS + tableData.Y1TPPIOS) / tableData.Y1VYrs;
            calculationsModel.Y2_CCPVY_TP = tableData.Y2VYrs == 0 ? 0 : (tableData.Y2TPPD + tableData.Y2TPCH + tableData.Y2TPPI + tableData.Y2TPPDOS + tableData.Y2TPCHOS + tableData.Y2TPPIOS) / tableData.Y2VYrs;
            calculationsModel.Y3_CCPVY_TP = tableData.Y3VYrs == 0 ? 0 : (tableData.Y3TPPD + tableData.Y3TPCH + tableData.Y3TPPI + tableData.Y3TPPDOS + tableData.Y3TPCHOS + tableData.Y3TPPIOS) / tableData.Y3VYrs;
            calculationsModel.Y4_CCPVY_TP = tableData.Y4VYrs == 0 ? 0 : (tableData.Y4TPPD + tableData.Y4TPCH + tableData.Y4TPPI + tableData.Y4TPPDOS + tableData.Y4TPCHOS + tableData.Y4TPPIOS) / tableData.Y4VYrs;
            calculationsModel.Y5_CCPVY_TP = tableData.Y5VYrs == 0 ? 0 : (tableData.Y5TPPD + tableData.Y5TPCH + tableData.Y5TPPI + tableData.Y5TPPDOS + tableData.Y5TPCHOS + tableData.Y5TPPIOS) / tableData.Y5VYrs;

            calculationsModel.ThreeY_CCPVY_TP = (calculationsModel.Y1_CCPVY_TP + calculationsModel.Y2_CCPVY_TP + calculationsModel.Y3_CCPVY_TP) / 3;
            calculationsModel.FiveY_CCPVY_TP = (calculationsModel.Y1_CCPVY_TP + calculationsModel.Y2_CCPVY_TP + calculationsModel.Y3_CCPVY_TP + calculationsModel.Y4_CCPVY_TP + calculationsModel.Y5_CCPVY_TP) / 5;

            // Total CCPVY is calculated by CCPVYAD + CCPVYTP
            calculationsModel.Y1_TotalCCPVY = (calculationsModel.Y1_CCPVY_AD + calculationsModel.Y1_CCPVY_TP);
            calculationsModel.Y2_TotalCCPVY = (calculationsModel.Y2_CCPVY_AD + calculationsModel.Y2_CCPVY_TP);
            calculationsModel.Y3_TotalCCPVY = (calculationsModel.Y3_CCPVY_AD + calculationsModel.Y3_CCPVY_TP);
            calculationsModel.Y4_TotalCCPVY = (calculationsModel.Y4_CCPVY_AD + calculationsModel.Y4_CCPVY_TP);
            calculationsModel.Y5_TotalCCPVY = (calculationsModel.Y5_CCPVY_AD + calculationsModel.Y5_CCPVY_TP);

            calculationsModel.ThreeY_TotalCCPVY = (calculationsModel.Y1_TotalCCPVY + calculationsModel.Y2_TotalCCPVY + calculationsModel.Y3_TotalCCPVY) / 3;
            calculationsModel.FiveY_TotalCCPVY = (calculationsModel.Y1_TotalCCPVY + calculationsModel.Y2_TotalCCPVY + calculationsModel.Y3_TotalCCPVY + calculationsModel.Y4_TotalCCPVY + calculationsModel.Y5_TotalCCPVY) / 5;

            
            // -------------------- INFLATION VALUES --------------------------------------

            // Inflated values
            var Y1InfRate = 1.06;
            var Y2InfRate = 1.079;
            var Y3InfRate = 1.09;
            var Y4InfRate = 1.076;
            var Y5InfRate = 1.076;
            // selected months from dropdown
            
            // Inflated Total cost values
            var Y1TotalInf = (tableData.Y1Total / (double)calculationsModel.InfMonth) * 12 * Y1InfRate;
            var Y2TotalInf = tableData.Y2Total * Y1InfRate * Y2InfRate;
            var Y3TotalInf = tableData.Y3Total * Y1InfRate * Y2InfRate * Y3InfRate;
            var Y4TotalInf = tableData.Y4Total * Y1InfRate * Y2InfRate * Y3InfRate * Y4InfRate;
            var Y5TotalInf = tableData.Y5Total * Y1InfRate * Y2InfRate * Y3InfRate * Y4InfRate * Y5InfRate;

            // Inflated AD
            var infY1AD = ((tableData.Y1ADPaid + tableData.Y1ADOS) / (double)calculationsModel.InfMonth) * 12 * Y1InfRate;
            var infY2AD = (tableData.Y2ADPaid + tableData.Y2ADOS) * Y1InfRate * Y2InfRate;
            var infY3AD = (tableData.Y3ADPaid + tableData.Y3ADOS) * Y1InfRate * Y2InfRate * Y3InfRate;
            var infY4AD = (tableData.Y4ADPaid + tableData.Y4ADOS) * Y1InfRate * Y2InfRate * Y3InfRate * Y4InfRate;
            var infY5AD = (tableData.Y5ADPaid + tableData.Y5ADOS) * Y1InfRate * Y2InfRate * Y3InfRate * Y4InfRate * Y5InfRate;

            //Inflated TP
            var infY1TP = ((tableData.Y1TPPD + tableData.Y1TPCH + tableData.Y1TPPI + tableData.Y1TPPDOS + tableData.Y1TPCHOS + tableData.Y1TPPIOS) / (double)calculationsModel.InfMonth) * 12 * Y1InfRate;
            var infY2TP = (tableData.Y2TPPD + tableData.Y2TPCH + tableData.Y2TPPI + tableData.Y2TPPDOS + tableData.Y2TPCHOS + tableData.Y2TPPIOS) * Y1InfRate * Y2InfRate;
            var infY3TP = (tableData.Y3TPPD + tableData.Y3TPCH + tableData.Y3TPPI + tableData.Y3TPPDOS + tableData.Y3TPCHOS + tableData.Y3TPPIOS) * Y1InfRate * Y2InfRate * Y3InfRate;
            var infY4TP = (tableData.Y4TPPD + tableData.Y4TPCH + tableData.Y4TPPI + tableData.Y4TPPDOS + tableData.Y4TPCHOS + tableData.Y4TPPIOS) * Y1InfRate * Y2InfRate * Y3InfRate * Y4InfRate;
            var infY5TP = (tableData.Y5TPPD + tableData.Y5TPCH + tableData.Y5TPPI + tableData.Y5TPPDOS + tableData.Y5TPCHOS + tableData.Y5TPPIOS) * Y1InfRate * Y2InfRate * Y3InfRate * Y4InfRate * Y5InfRate;


            // CPC inf
            calculationsModel.Y1CPC_Inf = Y1TotalInf / ((double)tableData.Y1ClaimsOpen + (double)tableData.Y1ClaimsClosed);
            calculationsModel.Y2CPC_Inf = Y2TotalInf / ((double)tableData.Y2ClaimsOpen + (double)tableData.Y2ClaimsClosed);
            calculationsModel.Y3CPC_Inf = Y3TotalInf / ((double)tableData.Y3ClaimsOpen + (double)tableData.Y3ClaimsClosed);
            calculationsModel.Y4CPC_Inf = Y4TotalInf / ((double)tableData.Y4ClaimsOpen + (double)tableData.Y4ClaimsClosed);
            calculationsModel.Y5CPC_Inf = Y5TotalInf / ((double)tableData.Y5ClaimsOpen + (double)tableData.Y5ClaimsClosed);
            calculationsModel.ThreeY_CPC_Inf = (calculationsModel.Y1CPC_Inf + calculationsModel.Y2CPC_Inf + calculationsModel.Y3CPC_Inf) / 3;
            InfCPC3Year = calculationsModel.ThreeY_CPC_Inf;
            calculationsModel.FiveY_CPC_Inf = (calculationsModel.Y1CPC_Inf + calculationsModel.Y2CPC_Inf + calculationsModel.Y3CPC_Inf + calculationsModel.Y4CPC_Inf + calculationsModel.Y5CPC_Inf) / 5;

            // CPID inf
            calculationsModel.Y1CPID_Inf = (tableData.Y1RentalDaysNonCOI == 0) ? 0 : Y1TotalInf / tableData.Y1RentalDaysNonCOI;
            calculationsModel.Y2CPID_Inf = (tableData.Y2RentalDaysNonCOI == 0) ? 0 : Y2TotalInf / tableData.Y2RentalDaysNonCOI;
            calculationsModel.Y3CPID_Inf = (tableData.Y3RentalDaysNonCOI == 0) ? 0 : Y3TotalInf / tableData.Y3RentalDaysNonCOI;
            calculationsModel.Y4CPID_Inf = (tableData.Y4RentalDaysNonCOI == 0) ? 0 : Y4TotalInf / tableData.Y4RentalDaysNonCOI;
            calculationsModel.Y5CPID_Inf = (tableData.Y5RentalDaysNonCOI == 0) ? 0 : Y5TotalInf / tableData.Y5RentalDaysNonCOI;
            calculationsModel.ThreeY_CPID_Inf = (calculationsModel.Y1CPID_Inf + calculationsModel.Y2CPID_Inf + calculationsModel.Y3CPID_Inf) / 3;
            InfCPID3Year = calculationsModel.ThreeY_CPID_Inf;
            calculationsModel.FiveY_CPID_Inf = (calculationsModel.Y1CPID_Inf + calculationsModel.Y2CPID_Inf + calculationsModel.Y3CPID_Inf + calculationsModel.Y4CPID_Inf + calculationsModel.Y5CPID_Inf) / 5;
            InfCPID5Year = calculationsModel.FiveY_CPID_Inf;

            //CCTO inf
            calculationsModel.Y1CCTO_Inf = (tableData.Y1TO_NonCOI == 0) ? 0 : (infY1AD + infY1TP) / (tableData.Y1TO_NonCOI / 1000);
            calculationsModel.Y2CCTO_Inf = (tableData.Y2TO_NonCOI == 0) ? 0 : (infY2AD + infY2TP) / (tableData.Y2TO_NonCOI / 1000);
            calculationsModel.Y3CCTO_Inf = (tableData.Y3TO_NonCOI == 0) ? 0 : (infY3AD + infY3TP) / (tableData.Y3TO_NonCOI / 1000);
            calculationsModel.Y4CCTO_Inf = (tableData.Y4TO_NonCOI == 0) ? 0 : (infY4AD + infY4TP) / (tableData.Y4TO_NonCOI / 1000);
            calculationsModel.Y5CCTO_Inf = (tableData.Y5TO_NonCOI == 0) ? 0 : (infY5AD + infY5TP) / (tableData.Y5TO_NonCOI / 1000);
            calculationsModel.ThreeY_CCTO_Inf = (calculationsModel.Y1CCTO_Inf + calculationsModel.Y2CCTO_Inf + calculationsModel.Y3CCTO_Inf) / 3;
            calculationsModel.FiveY_CCTO_Inf = (calculationsModel.Y1CCTO_Inf + calculationsModel.Y2CCTO_Inf + calculationsModel.Y3CCTO_Inf + calculationsModel.Y4CCTO_Inf + calculationsModel.Y5CCTO_Inf) / 5;

            // CCPVY AD inf 
            calculationsModel.Y1_CCPVYAD_Inf = tableData.Y1VYrs == 0 ? 0 : infY1AD / (double)tableData.Y1VYrs;
            calculationsModel.Y2_CCPVYAD_Inf = tableData.Y2VYrs == 0 ? 0 : infY2AD / (double)tableData.Y2VYrs;
            calculationsModel.Y3_CCPVYAD_Inf = tableData.Y3VYrs == 0 ? 0 : infY3AD / (double)tableData.Y3VYrs;
            calculationsModel.Y4_CCPVYAD_Inf = tableData.Y4VYrs == 0 ? 0 : infY4AD / (double)tableData.Y4VYrs;
            calculationsModel.Y5_CCPVYAD_Inf = tableData.Y5VYrs == 0 ? 0 : infY5AD / (double)tableData.Y5VYrs;
            calculationsModel.ThreeY_CCPVYAD_Inf = (calculationsModel.Y1_CCPVYAD_Inf + calculationsModel.Y2_CCPVYAD_Inf + calculationsModel.Y3_CCPVYAD_Inf) / 3;
            calculationsModel.FiveY_CCPVYAD_Inf = (calculationsModel.Y1_CCPVYAD_Inf + calculationsModel.Y2_CCPVYAD_Inf + calculationsModel.Y3_CCPVYAD_Inf + calculationsModel.Y4_CCPVYAD_Inf + calculationsModel.Y5_CCPVYAD_Inf) / 5;

            // CCPVY TP inf
            calculationsModel.Y1_CCPVYTP_Inf = tableData.Y1VYrs == 0 ? 0 : infY1TP / tableData.Y1VYrs;
            calculationsModel.Y2_CCPVYTP_Inf = tableData.Y2VYrs == 0 ? 0 : infY2TP / tableData.Y2VYrs;
            calculationsModel.Y3_CCPVYTP_Inf = tableData.Y3VYrs == 0 ? 0 : infY3TP / tableData.Y3VYrs;
            calculationsModel.Y4_CCPVYTP_Inf = tableData.Y4VYrs == 0 ? 0 : infY4TP / tableData.Y4VYrs;
            calculationsModel.Y5_CCPVYTP_Inf = tableData.Y5VYrs == 0 ? 0 : infY5TP / tableData.Y5VYrs;
            calculationsModel.ThreeY_CCPVYTP_Inf = (calculationsModel.Y1_CCPVYTP_Inf + calculationsModel.Y2_CCPVYTP_Inf + calculationsModel.Y3_CCPVYTP_Inf) / 3;
            calculationsModel.FiveY_CCPVYTP_Inf = (calculationsModel.Y1_CCPVYTP_Inf + calculationsModel.Y2_CCPVYTP_Inf + calculationsModel.Y3_CCPVYTP_Inf + calculationsModel.Y4_CCPVYTP_Inf + calculationsModel.Y5_CCPVYTP_Inf) / 5;

            // Total CCPVY
            calculationsModel.Y1_TotalCCPVY_Inf = calculationsModel.Y1_CCPVYAD_Inf + calculationsModel.Y1_CCPVYTP_Inf;
            calculationsModel.Y2_TotalCCPVY_Inf = calculationsModel.Y2_CCPVYAD_Inf + calculationsModel.Y2_CCPVYTP_Inf;
            calculationsModel.Y3_TotalCCPVY_Inf = calculationsModel.Y3_CCPVYAD_Inf + calculationsModel.Y3_CCPVYTP_Inf;
            calculationsModel.Y4_TotalCCPVY_Inf = calculationsModel.Y4_CCPVYAD_Inf + calculationsModel.Y4_CCPVYTP_Inf;
            calculationsModel.Y5_TotalCCPVY_Inf = calculationsModel.Y5_CCPVYAD_Inf + calculationsModel.Y5_CCPVYTP_Inf;
            calculationsModel.ThreeY_TotalCCPVY_Inf = (calculationsModel.Y1_TotalCCPVY_Inf + calculationsModel.Y2_TotalCCPVY_Inf + calculationsModel.Y3_TotalCCPVY_Inf) / 3;
            InfCCPVYTotal3Year = calculationsModel.ThreeY_TotalCCPVY_Inf;
            calculationsModel.FiveY_TotalCCPVY_Inf = (calculationsModel.Y1_TotalCCPVY_Inf + calculationsModel.Y2_TotalCCPVY_Inf + calculationsModel.Y3_TotalCCPVY_Inf + calculationsModel.Y4_TotalCCPVY_Inf + calculationsModel.Y5_TotalCCPVY_Inf) / 5;
            InfCCPVYTotal5Year = calculationsModel.FiveY_TotalCCPVY_Inf;

            // Populate the CalculationsYearsDataInflated for the view model
            calculationsModel.CalculationsYearsInflated = new List<CalculationsYearsDataInflated>
            {
                new CalculationsYearsDataInflated
                {
                    Label = "CPC",
                    Year1 = calculationsModel.Y1CPC_Inf.ToString("F2"),
                    Year2 = calculationsModel.Y2CPC_Inf.ToString("F2"),
                    Year3 = calculationsModel.Y3CPC_Inf.ToString("F2"),
                    Year4 = calculationsModel.Y4CPC_Inf.ToString("F2"),
                    Year5 = calculationsModel.Y5CPC_Inf.ToString("F2"),
                    ThreeYearAverage = calculationsModel.ThreeY_CPC_Inf.ToString("F2"),
                    FiveYearAverage = calculationsModel.FiveY_CPC_Inf.ToString("F2")
                },
                new CalculationsYearsDataInflated
                {
                    Label = "CPID",
                    Year1 = calculationsModel.Y1CPID_Inf.ToString("F2"),
                    Year2 = calculationsModel.Y2CPID_Inf.ToString("F2"),
                    Year3 = calculationsModel.Y3CPID_Inf.ToString("F2"),
                    Year4 = calculationsModel.Y4CPID_Inf.ToString("F2"),
                    Year5 = calculationsModel.Y5CPID_Inf.ToString("F2"),
                    ThreeYearAverage = calculationsModel.ThreeY_CPID_Inf.ToString("F2"),
                    FiveYearAverage = calculationsModel.FiveY_CPID_Inf.ToString("F2"),
                },
                new CalculationsYearsDataInflated
                {
                    Label = "CCTO",
                    Year1 = calculationsModel.Y1CCTO_Inf.ToString("F2"),
                    Year2 = calculationsModel.Y2CCTO_Inf.ToString("F2"),
                    Year3 = calculationsModel.Y3CCTO_Inf.ToString("F2"),
                    Year4 = calculationsModel.Y4CCTO_Inf.ToString("F2"),
                    Year5 = calculationsModel.Y5CCTO_Inf.ToString("F2"),
                    ThreeYearAverage = calculationsModel.ThreeY_CCTO_Inf.ToString("F2"),
                    FiveYearAverage = calculationsModel.FiveY_CCTO_Inf.ToString("F2")
                },
                new CalculationsYearsDataInflated
                {
                    Label = "CCPVY AD",
                    Year1 = calculationsModel.Y1_CCPVYAD_Inf.ToString("F2"),
                    Year2 = calculationsModel.Y2_CCPVYAD_Inf.ToString("F2"),
                    Year3 = calculationsModel.Y3_CCPVYAD_Inf.ToString("F2"),
                    Year4 = calculationsModel.Y4_CCPVYAD_Inf.ToString("F2"),
                    Year5 = calculationsModel.Y5_CCPVYAD_Inf.ToString("F2"),
                    ThreeYearAverage = calculationsModel.ThreeY_CCPVYAD_Inf.ToString("F2"),
                    FiveYearAverage = calculationsModel.FiveY_CCPVYAD_Inf.ToString("F2")
                },
                new CalculationsYearsDataInflated
                {
                    Label = "CCPVY TP",
                    Year1 = calculationsModel.Y1_CCPVYTP_Inf.ToString("F2"),
                    Year2 = calculationsModel.Y2_CCPVYTP_Inf.ToString("F2"),
                    Year3 = calculationsModel.Y3_CCPVYTP_Inf.ToString("F2"),
                    Year4 = calculationsModel.Y4_CCPVYTP_Inf.ToString("F2"),
                    Year5 = calculationsModel.Y5_CCPVYTP_Inf.ToString("F2"),
                    ThreeYearAverage = calculationsModel.ThreeY_CCPVYTP_Inf.ToString("F2"),
                    FiveYearAverage = calculationsModel.FiveY_CCPVYTP_Inf.ToString("F2")
                },
                new CalculationsYearsDataInflated
                {
                    Label = "Total CCPVY",
                    Year1 = calculationsModel.Y1_TotalCCPVY_Inf.ToString("F2"),
                    Year2 = calculationsModel.Y2_TotalCCPVY_Inf.ToString("F2"),
                    Year3 = calculationsModel.Y3_TotalCCPVY_Inf.ToString("F2"),
                    Year4 = calculationsModel.Y4_TotalCCPVY_Inf.ToString("F2"),
                    Year5 = calculationsModel.Y5_TotalCCPVY_Inf.ToString("F2"),
                    ThreeYearAverage = calculationsModel.ThreeY_TotalCCPVY_Inf.ToString("F2"),
                    FiveYearAverage = calculationsModel.FiveY_TotalCCPVY_Inf.ToString("F2")
                }
            };
           


            // ----------------------- Additional Stats ---------------------------------------------------------

            // Total Exposure
            calculationsModel.TotalExposure = (
                tableData.CarNums * tableData.CarExp +
                tableData.VanNums * tableData.VanExp +
                tableData.MinibusNums * tableData.MinibusExp +
                tableData.HGVNums * tableData.HGVExp);

            // Total Non COI Exposure
            calculationsModel.TotalNonCOIExposure = tableData.FCDaysCOI == 0 && tableData.FCDaysNonCOI == 0 ? 0 :
                (double)tableData.FCDaysNonCOI / (tableData.FCDaysCOI + tableData.FCDaysNonCOI) * calculationsModel.TotalExposure;            

            // CC 1000 Days (Client) and (Book)
            calculationsModel.CC1000DaysClient =
                tableData.FiveY_Total / ((double)tableData.FiveY_RDaysNonCOI / 1000);           

            calculationsModel.CC1000DaysBook = 2530.01;

            // Variance
            calculationsModel.Variance = calculationsModel.CC1000DaysClient / calculationsModel.CC1000DaysBook * 100;

            // New Exposure
            calculationsModel.NewExposure = calculationsModel.TotalNonCOIExposure * (calculationsModel.Variance / 100);

            // this creates a new instance of the CalculationsViewModel class and assigns the CalculationsModel
            // instance (which now contains all the calculated data) to the CalculationsModel property
            // the 'viewModel' then gets passed to the view in the return statement

            // Populate the CalculationsYearsData for the view model
            calculationsModel.CalculationsYears = new List<CalculationsYearsData>
            {
                new CalculationsYearsData
                {
                    Label = "Acc Freq",
                    Year1 = calculationsModel.Y1AccFreq.ToString("F2"),
                    Year2 = calculationsModel.Y2AccFreq.ToString("F2"),
                    Year3 = calculationsModel.Y3AccFreq.ToString("F2"),
                    Year4 = calculationsModel.Y4AccFreq.ToString("F2"),
                    Year5 = calculationsModel.Y5AccFreq.ToString("F2"),
                    ThreeYearTotal = calculationsModel.ThreeY_AccFreq.ToString("F2"),
                    FiveYearTotal = calculationsModel.FiveY_AccFreq.ToString("F2"),
                },
                new CalculationsYearsData
                {
                    Label = "CPC",
                    Year1 = calculationsModel.Y1CPC.ToString("F2"),
                    Year2 = calculationsModel.Y2CPC.ToString("F2"),
                    Year3 = calculationsModel.Y3CPC.ToString("F2"),
                    Year4 = calculationsModel.Y4CPC.ToString("F2"),
                    Year5 = calculationsModel.Y5CPC.ToString("F2"),
                    ThreeYearTotal = calculationsModel.ThreeY_CPC.ToString("F2"),
                    FiveYearTotal = calculationsModel.FiveY_CPC.ToString("F2"),
                },
                new CalculationsYearsData
                {
                    Label = "CPID",
                    Year1 = calculationsModel.Y1CPID.ToString("F2"),
                    Year2 = calculationsModel.Y2CPID.ToString("F2"),
                    Year3 = calculationsModel.Y3CPID.ToString("F2"),
                    Year4 = calculationsModel.Y4CPID.ToString("F2"),
                    Year5 = calculationsModel.Y5CPID.ToString("F2"),
                    ThreeYearTotal = calculationsModel.ThreeY_CPID.ToString("F2"),
                    FiveYearTotal = calculationsModel.FiveY_CPID.ToString("F2"),
                },
                new CalculationsYearsData
                {
                    Label = "CCTO",
                    Year1 = calculationsModel.Y1CCTO.ToString("F2"),
                    Year2 = calculationsModel.Y2CCTO.ToString("F2"),
                    Year3 = calculationsModel.Y3CCTO.ToString("F2"),
                    Year4 = calculationsModel.Y4CCTO.ToString("F2"),
                    Year5 = calculationsModel.Y5CCTO.ToString("F2"),
                    ThreeYearTotal = calculationsModel.ThreeY_CCTO.ToString("F2"),
                    FiveYearTotal = calculationsModel.FiveY_CCTO.ToString("F2")
                },
                new CalculationsYearsData
                {
                    Label = "CCPVY AD",
                    Year1 = calculationsModel.Y1_CCPVY_AD.ToString("F2"),
                    Year2 = calculationsModel.Y2_CCPVY_AD.ToString("F2"),
                    Year3 = calculationsModel.Y3_CCPVY_AD.ToString("F2"),
                    Year4 = calculationsModel.Y4_CCPVY_AD.ToString("F2"),
                    Year5 = calculationsModel.Y5_CCPVY_AD.ToString("F2"),
                    ThreeYearTotal = calculationsModel.ThreeY_CCPVY_AD.ToString("F2"),
                    FiveYearTotal = calculationsModel.FiveY_CCPVY_AD.ToString("F2")
                },
                new CalculationsYearsData
                {
                    Label = "CCPVY TP",
                    Year1 = calculationsModel.Y1_CCPVY_TP.ToString("F2"),
                    Year2 = calculationsModel.Y2_CCPVY_TP.ToString("F2"),
                    Year3 = calculationsModel.Y3_CCPVY_TP.ToString("F2"),
                    Year4 = calculationsModel.Y4_CCPVY_TP.ToString("F2"),
                    Year5 = calculationsModel.Y5_CCPVY_TP.ToString("F2"),
                    ThreeYearTotal = calculationsModel.ThreeY_CCPVY_TP.ToString("F2"),
                    FiveYearTotal = calculationsModel.FiveY_CCPVY_TP.ToString("F2"),
                },
                new CalculationsYearsData
                {
                    Label = "Total CCPVY",
                    Year1 = calculationsModel.Y1_TotalCCPVY.ToString("F2"),
                    Year2 = calculationsModel.Y2_TotalCCPVY.ToString("F2"),
                    Year3 = calculationsModel.Y3_TotalCCPVY.ToString("F2"),
                    Year4 = calculationsModel.Y4_TotalCCPVY.ToString("F2"),
                    Year5 = calculationsModel.Y5_TotalCCPVY.ToString("F2"),
                    ThreeYearTotal = calculationsModel.ThreeY_TotalCCPVY.ToString("F2"),
                    FiveYearTotal = calculationsModel.FiveY_TotalCCPVY.ToString("F2")
                }
            };


            // ---------------- TECHNICAL PRICE POINTS ---------------------------
            

            

            // Projected Claims
            var totalVehicleNums = tableData.CarNums + tableData.VanNums + tableData.MinibusNums + tableData.HGVNums;           

            if (tableData.FCTO_NonCOI == 0 && tableData.FCDaysNonCOI == 0)
            {
                calculationsModel.ProjectedClaimsTech = (calculationsModel.ProjYear == 3) 
                    ? 
                    (totalVehicleNums * accidentFreq3Year) 
                    : 
                    (totalVehicleNums * accidentFreq3Year);
            }
            else if (tableData.FCTO_NonCOI == 0)
            {
                calculationsModel.ProjectedClaimsTech = (calculationsModel.ProjYear == 3)
                    ?
                    ((double)(tableData.ThreeY_ClaimsOpen + tableData.ThreeY_ClaimsClo) / tableData.ThreeY_RDaysNonCOI) * tableData.FCDaysNonCOI
                    :
                    ((double)(tableData.FiveY_ClaimsOpen + tableData.FiveY_ClaimsClo) / tableData.FiveY_RDaysNonCOI) * tableData.FCDaysNonCOI;
                
            }
            else
            {
                calculationsModel.ProjectedClaimsTech = (tableData.FCTO_NonCOI >= 1000000)
                    ?
                    (tableData.FCTO_NonCOI / 10000) * ((calculationsModel.ProjYear == 3) ? accidentFreq3Year : accidentFreq5Year)
                    :
                    (tableData.FCTO_NonCOI / 10000) * ((calculationsModel.ProjYear == 3) ? accidentFreq3Year : accidentFreq5Year);
            }

            // Projected CCPVY
            calculationsModel.ProjectedCCPVYTech = (calculationsModel.ProjYear == 3) ? InfCCPVYTotal3Year : InfCCPVYTotal5Year;

            // Projected CPIRD
            calculationsModel.ProjectedCPIRDTech = (calculationsModel.ProjYear == 3) ? InfCPID3Year : InfCPID5Year;

            // Projected IBNR
            calculationsModel.ProjectedIBNRTech = calculationsModel.ProjectedClaimsTech / 10;

            // Projected Exposure
            calculationsModel.ProjectedExposureTech = tableData.ExpPercentage;




            // ---------------- ULTIMATE PROJECTIONS --------------------------------
            var totalLL = tableData.CarNums * tableData.CarLLL + tableData.VanNums * tableData.VanLLL + tableData.MinibusNums * tableData.MinibusLLL + tableData.HGVNums * tableData.HGVLLL;
            

            // Contingent Fee
            if (calculationsModel.ChargeCOIFeeValue == 2)
            {
                calculationsModel.COI_Contingent = 0;
            }
            else if (tableData.FCTO_COI == 0)
            {
                calculationsModel.COI_Contingent = 0;
            }
            else if (tableData.FCTO_COI / (tableData.FCTO_COI + tableData.FCTO_NonCOI) > 0.5)
            {
                calculationsModel.COI_Contingent = (tableData.FCTO_COI / 100) * 1;
            }
            else
            {
                calculationsModel.COI_Contingent = (tableData.FCTO_COI / 100) * 0.5;
            }
            // Projected LL Fund
            if (tableData.FCDaysNonCOI == 0)
            {
                calculationsModel.ProjLLFund = totalLL;
            }
            else
            {
                calculationsModel.ProjLLFund = totalLL * ((double)tableData.FCDaysNonCOI / (tableData.FCDaysNonCOI + tableData.FCDaysCOI));
            }
       
            

            // Projected Claims Amount
            if (calculationsModel.PriceByValue == 1)  // experience
            {                                
                if (calculationsModel.PricingMetricValue == 1 )
                {
                    if (tableData.FCTO_NonCOI == 0 && tableData.FCDaysNonCOI == 0)
                    {
                        calculationsModel.ProjClaimsAmount = (double)totalVehicleNums * calculationsModel.ProjectedCCPVYTech;
                    }
                    else if (tableData.FCTO_NonCOI == 0)
                    {
                        calculationsModel.ProjClaimsAmount = ((double)tableData.FCDaysNonCOI / 365) * calculationsModel.ProjectedCCPVYTech;
                    }
                    else
                    {
                        calculationsModel.ProjClaimsAmount = ((double)tableData.FCTO_NonCOI / 1000) * ((calculationsModel.ProjYear == 3) ? calculationsModel.ThreeY_CCTO_Inf : calculationsModel.FiveY_CCTO_Inf);
                    }
                }
                else if (calculationsModel.PricingMetricValue == 2)
                {
                    calculationsModel.ProjClaimsAmount = calculationsModel.ProjectedClaimsTech * ((calculationsModel.ProjYear == 3) ? calculationsModel.ThreeY_CPC_Inf : calculationsModel.FiveY_CPC_Inf);
                }
                else
                {
                    calculationsModel.ProjClaimsAmount = calculationsModel.ProjectedCPIRDTech * (double)tableData.FCDaysNonCOI;
                }

                calculationsModel.PretiumExpenses = ((calculationsModel.ProjClaimsAmount + calculationsModel.ProjLLFund + calculationsModel.COI_Contingent + calculationsModel.ProjIBNR + calculationsModel.ProjExposure) / 40) * 10;


                calculationsModel.ProjExposure = 0;
                calculationsModel.ProjIBNR = calculationsModel.ProjectedIBNRTech * ((calculationsModel.ProjYear == 3) ? calculationsModel.ThreeY_CPC_Inf : calculationsModel.FiveY_CPC_Inf);
                calculationsModel.Profit = ((calculationsModel.ProjClaimsAmount + calculationsModel.ProjLLFund + calculationsModel.COI_Contingent + calculationsModel.ProjIBNR + calculationsModel.ProjExposure) / 40) * 18;
                calculationsModel.ReinsuranceCosts = ((calculationsModel.ProjClaimsAmount + calculationsModel.ProjLLFund + calculationsModel.COI_Contingent + calculationsModel.ProjIBNR + calculationsModel.ProjExposure) / 40) * 26;
                calculationsModel.Commission = ((calculationsModel.ProjClaimsAmount + calculationsModel.ProjLLFund + calculationsModel.COI_Contingent + calculationsModel.ProjIBNR + calculationsModel.ProjExposure) / 40) * 1;
                           
            }
            else if (calculationsModel.PriceByValue == 2) // exposure
            {
                calculationsModel.ProjClaimsAmount = calculationsModel.NewExposure * 0.4;
                calculationsModel.ProjIBNR = 0;
                calculationsModel.ProjLLFund = 0;
                calculationsModel.ProjExposure = 0;
                calculationsModel.COI_Contingent = 0;
                calculationsModel.Profit = (calculationsModel.NewExposure / 100) * 18;
                calculationsModel.ReinsuranceCosts = (calculationsModel.NewExposure / 100) * 26;
                calculationsModel.Commission = calculationsModel.NewExposure / 100;
            }
            else // blend
            {
                if (calculationsModel.PricingMetricValue == 1)
                {
                    if (tableData.FCTO_NonCOI == 0 && tableData.FCDaysNonCOI == 0)
                    {
                        calculationsModel.ProjClaimsAmount = totalVehicleNums * calculationsModel.ProjectedCCPVYTech;
                    }
                    else if (tableData.FCTO_NonCOI == 0) 
                    {
                        calculationsModel.ProjClaimsAmount = (tableData.FCDaysNonCOI / 365) * calculationsModel.ProjectedCCPVYTech;
                    }
                    else
                    {
                        calculationsModel.ProjClaimsAmount = (tableData.FCTO_NonCOI / 1000) * ((calculationsModel.ProjYear == 3) ? calculationsModel.ThreeY_CCTO_Inf : calculationsModel.FiveY_CCTO_Inf);
                    }
                } 
                else if (calculationsModel.PricingMetricValue == 2)
                {
                    calculationsModel.ProjClaimsAmount = calculationsModel.ProjectedClaimsTech * ((calculationsModel.ProjYear == 3) ? calculationsModel.ThreeY_CPC_Inf : calculationsModel.FiveY_CPC_Inf);
                }
                else
                {
                    calculationsModel.ProjClaimsAmount = calculationsModel.ProjectedCPIRDTech * tableData.FCDaysNonCOI;
                }
                

                calculationsModel.ProjExposure = tableData.ExpPercentage * calculationsModel.NewExposure;
                calculationsModel.ProjIBNR = calculationsModel.ProjectedIBNRTech * ((calculationsModel.ProjYear == 3) ? calculationsModel.ThreeY_CPC_Inf : calculationsModel.FiveY_CPC_Inf);
                calculationsModel.Profit = ((calculationsModel.ProjClaimsAmount + calculationsModel.ProjLLFund + calculationsModel.COI_Contingent + calculationsModel.ProjIBNR + calculationsModel.ProjExposure) / 40) * 18;
                calculationsModel.ReinsuranceCosts = ((calculationsModel.ProjClaimsAmount + calculationsModel.ProjLLFund + calculationsModel.COI_Contingent + calculationsModel.ProjIBNR + calculationsModel.ProjExposure) / 40) * 26;
                calculationsModel.Commission = ((calculationsModel.ProjClaimsAmount + calculationsModel.ProjLLFund + calculationsModel.COI_Contingent + calculationsModel.ProjIBNR + calculationsModel.ProjExposure) / 40) * 1;

            }


            calculationsModel.ProjClaimsHandlingFee = ((calculationsModel.ProjClaimsAmount + calculationsModel.ProjLLFund + calculationsModel.COI_Contingent + calculationsModel.ProjIBNR + calculationsModel.ProjExposure) / 40) * 5;

            calculationsModel.PretiumExpenses = ((calculationsModel.ProjClaimsAmount + calculationsModel.ProjLLFund + calculationsModel.COI_Contingent + calculationsModel.ProjIBNR + calculationsModel.ProjExposure) / 40) * 10;

            calculationsModel.NetPremium = calculationsModel.ProjClaimsAmount + calculationsModel.ProjLLFund + calculationsModel.COI_Contingent + calculationsModel.ProjIBNR + calculationsModel.ProjExposure + calculationsModel.ProjClaimsHandlingFee + calculationsModel.PretiumExpenses + calculationsModel.Profit + calculationsModel.ReinsuranceCosts;

            calculationsModel.GrossPremium = calculationsModel.NetPremium + calculationsModel.Commission;

            calculationsModel.GrossPremiumPlusIPT = calculationsModel.GrossPremium + (calculationsModel.GrossPremium / 100) * 12; // IPT is 12% of gross premium

            calculationsModel.Levies = calculationsModel.NetPremium * 0.047;


            calculationsModel.InflationMonths = new List<SelectListItem>
            {
                new SelectListItem { Text = "1", Value = "1", Selected = calculationsModel.SelectedNumOfMonths == "1" },
                new SelectListItem { Text = "2", Value = "2", Selected = calculationsModel.SelectedNumOfMonths == "2" },
                new SelectListItem { Text = "3", Value = "3", Selected = calculationsModel.SelectedNumOfMonths == "3"},
                new SelectListItem { Text = "4", Value = "4", Selected = calculationsModel.SelectedNumOfMonths == "4" },
                new SelectListItem { Text = "5", Value = "5", Selected = calculationsModel.SelectedNumOfMonths == "5" },
                new SelectListItem { Text = "6", Value = "6", Selected = calculationsModel.SelectedNumOfMonths == "6" },
                new SelectListItem { Text = "7", Value = "7", Selected = calculationsModel.SelectedNumOfMonths == "7" },
                new SelectListItem { Text = "8", Value = "8", Selected = calculationsModel.SelectedNumOfMonths == "8" },
                new SelectListItem { Text = "9", Value = "9", Selected = calculationsModel.SelectedNumOfMonths == "9" },
                new SelectListItem { Text = "10", Value = "10", Selected = calculationsModel.SelectedNumOfMonths == "10" },
                new SelectListItem { Text = "11", Value = "11", Selected = calculationsModel.SelectedNumOfMonths == "11" },
                new SelectListItem { Text = "12", Value = "12", Selected = calculationsModel.SelectedNumOfMonths == "12" },
            };
            calculationsModel.PriceByFilters = new List<SelectListItem>
            {
                new SelectListItem {Text = "Experience", Value = "1", Selected = calculationsModel.PriceByValue == 1},
                new SelectListItem {Text = "Exposure",   Value = "2", Selected = calculationsModel.PriceByValue == 2},
                new SelectListItem {Text = "Blend",      Value = "3", Selected = calculationsModel.PriceByValue == 3},
            };
            calculationsModel.PricingMetrics = new List<SelectListItem>
            {
                new SelectListItem {Text = "CCPVY", Value = "1", Selected = calculationsModel.PricingMetricValue == 1},
                new SelectListItem {Text = "Claims", Value = "2", Selected = calculationsModel.PricingMetricValue == 2},
                new SelectListItem {Text = "Days", Value = "3", Selected = calculationsModel.PricingMetricValue == 3},
            };
            calculationsModel.ChargeCOIFee = new List<SelectListItem>
            {
                new SelectListItem {Text = "Yes", Value = "1", Selected = calculationsModel.ChargeCOIFeeValue == 1},
                new SelectListItem {Text = "No", Value = "2", Selected = calculationsModel.ChargeCOIFeeValue == 2}
            };
            calculationsModel.ProjectedYears = new List<SelectListItem>
            {
                new SelectListItem { Text = "3", Value = "3", Selected = calculationsModel.ProjYear == 3 },
                new SelectListItem { Text = "5", Value = "5", Selected = calculationsModel.ProjYear == 5 },
            };





            return calculationsModel;
        }




        public List<IndivClaimDB> GetClaimsByBatchId(string batchId)
        {
            return _appContext.ClaimsTable
                        .Where(c => c.BatchId == batchId)
                        .ToList();
        }


        























        // -------------- method to get the historic years data to display again on this page ---------------------

        /*public CalculationsModel GetHistoricYearsDataTable(int quoteId)
        {
            var tableData = _appContext.ClientDetails.FirstOrDefault(x => x.Id == quoteId);

            var calculationsModel = new CalculationsModel();

            var clientDetail = new ClientDetail
            {
                QuoteNumber = tableData.QuoteNumber
            };

            return calculationsModel;
        }*/

        // --------------------- need a function to get the input model

        /*public InputModel GetInputModelData(int quoteId)
        {
            var tableData = _appContext.ClientDetails.FirstOrDefault(x => x.Id == quoteId);

            //var inputModel = new InputModel();

            *//*string clientListFilePath = Path.Combine(_env.ContentRootPath, "AdditionalMaterial", "clientNameList.txt");
            string[] lines = System.IO.File.ReadAllLines(clientListFilePath);

            if (int.TryParse(tableData.ClientName, out int parsedClientName))
            {
                inputModel.ClientName = lines[parsedClientName];
            }
            else
            {
                inputModel.ClientName = tableData.ClientName;
            }*/

        /*inputModel.QuoteNumber = tableData.QuoteNumber;
        inputModel.StartDate = tableData.StartDate;
        inputModel.EndDate = tableData.EndDate;*//*

        CalculationsModel.ClientName

        return inputModel;
    }*/






        /*public TechnicalPricesModel GetTechnicalPrices(int quoteId, string selectedNumOfMonths, int projYears)
        {
            var tableData = _appContext.ClientDetails.FirstOrDefault(x => x.Id == quoteId);

            

            var calculationsModel = new TechnicalPricesModel();
            calculationsModel.SelectedNumOfMonths = selectedNumOfMonths;

            // Parse the selectedNumOfMonths and assign it to InfMonth
            if (int.TryParse(selectedNumOfMonths, out int parsedMonths))
            {
                calculationsModel.InfMonth = parsedMonths;
            }
            else
            {
                // Handle invalid number of months, default to 12 if parsing fails
                calculationsModel.InfMonth = 12;
            }

            calculationsModel.ProjectedMonths = new List<SelectListItem>
            {
                new SelectListItem { Text = "3", Value = "3", Selected = calculationsModel.SelectedProjMonths == "3" },
                new SelectListItem { Text = "5", Value = "5", Selected = calculationsModel.SelectedProjMonths == "5" },            
            };

            var totalVehicleNums = tableData.CarNums + tableData.VanNums + tableData.MinibusNums + tableData.HGVNums;

            //calculationsModel.ProjectedClaims = (tableData.FCTO_NonCOI == 0 || tableData.FCDaysNonCOI == 0) ? 
                //(calculationsModel.SelectedProjMonths == "3") ? (totalVehicleNums * 


            return calculationsModel;
        }*/
    }
}
