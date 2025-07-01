using Microsoft.AspNetCore.Mvc;
using CSV_reader.Services;
using CSV_reader.Models;
using System.Collections.Generic;
using CSV_reader.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using CSV_reader.database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using DocumentFormat.OpenXml.Bibliography;

namespace CSV_reader.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IClaimsService _claimsService;
        private readonly ApplicationContext _appContext;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IExcelFileService _excelFileService;

        // Single constructor combining both dependencies:
        public HomeController(IClaimsService claimsService, ApplicationContext appContext, IConfiguration configuration, IWebHostEnvironment webHostEnvironment, IExcelFileService excelFileService)
        {
            _claimsService = claimsService;
            _appContext = appContext;
            _configuration = configuration;            
            _webHostEnvironment = webHostEnvironment;
            _excelFileService = excelFileService;
        }

       
       

        [HttpGet]
        public IActionResult Index2()
        {
            
            // get the filepath from memory cache instead of appsettings.json
            string excelFilePath = _excelFileService.GetExcelFilePath();

            var claimsViewModel = new ClaimsViewModel
            {
                ClaimsRecords = _claimsService.ReadClaimsExcel(excelFilePath),
                PolicyYearSummaries = _claimsService.GetPolicyYearSummaries(excelFilePath),
                HistoricYearsData = _claimsService.Historic3Years5YearsData(excelFilePath),

                InputModel = new InputModel(),

            };

            // the following defines the dropdown lists for the view.
            // This guarantees that the dropdowns are populated before the view is rendered, making sure they are never null or empty which would lead to errors.
            // It also centralises the data preparation meaning if more dropdown values are needed, they can be added here easily.

            claimsViewModel.InputModel.Percentages = new List<SelectListItem>
            {
                new SelectListItem { Text = "0%", Value = "0", Selected = claimsViewModel.InputModel.SelectedPercentage == "0" },
                new SelectListItem { Text = "5%", Value = "0.05", Selected = claimsViewModel.InputModel.SelectedPercentage == "0.05" },
                new SelectListItem { Text = "10%", Value = "0.1", Selected = claimsViewModel.InputModel.SelectedPercentage == "0.1" },
                new SelectListItem { Text = "20%", Value = "0.2", Selected = claimsViewModel.InputModel.SelectedPercentage == "0.2" },
                new SelectListItem { Text = "25%", Value = "0.25", Selected = claimsViewModel.InputModel.SelectedPercentage == "0.25" },
                new SelectListItem { Text = "50%", Value = "0.5", Selected = claimsViewModel.InputModel.SelectedPercentage == "0.5" },
                new SelectListItem { Text = "75%", Value = "0.75", Selected = claimsViewModel.InputModel.SelectedPercentage == "0.75" },
                new SelectListItem { Text = "100%", Value = "1", Selected = claimsViewModel.InputModel.SelectedPercentage == "1" }
            };

            claimsViewModel.InputModel.CoverTypes = new List<SelectListItem>
            {
                new SelectListItem { Text = "Comprehensive", Value = "Comprehensive", Selected = claimsViewModel.InputModel.SelectedCoverType == "Comprehensive"},
                new SelectListItem { Text = "Self Drive Hire", Value = "Self Drive Hire", Selected = claimsViewModel.InputModel.SelectedCoverType == "Self Drive Hire"},
            };           

            return View(claimsViewModel);
        }


        // method to get just one years claims
        [HttpGet]
        public JsonResult GetOneYearsClaimsController(string year)
        {
            var filePath = _excelFileService.GetExcelFilePath();

            Console.WriteLine($"Year Filter: {year}");

            var data = _claimsService.ReadClaimsExcel(filePath); // Read directly
            var filteredClaims = data.Where(c => c.PolicyYearCol == year).ToList();

            return Json(filteredClaims);
        }


        [HttpPost]
        public IActionResult ImportClaims(ClaimsViewModel viewModel)
        {

            string userEmail = User.Identity!.Name!;

            string excelFilePath = _excelFileService.GetExcelFilePath();

            var claimsData = _claimsService.ReadClaimsExcel(excelFilePath);
            string batchId = _claimsService.SaveClaimsToDatabase(claimsData, viewModel.InputModel, userEmail);

            // just before loading the view, delete the file from the uploads folder
            _excelFileService.DeleteFileByPath(excelFilePath);

            // Redirect page
            return RedirectToAction("IndexCalculationsPlusClaims2", "Calculations", new
            {
                batchId,
                selectedNumOfMonths = "12",
                projYears = "3",
                chargeCOIFee = "Yes",
                pricingMetric = "CCPVY",
                priceBy = "Experience",
            });

            
        }



        /*[HttpPost]
        public IActionResult SaveClientDetails(ClaimsViewModel viewModel)
        {
            string excelFilePath = _excelFileService.GetExcelFilePath();

            // Call the GetHistoricYearsData method from the service layer
            var historicData = _claimsService.Historic3Years5YearsData(excelFilePath);
            var policyYearSummaries = _claimsService.GetPolicyYearSummaries(excelFilePath);

            // Assign the returned data to the viewModel's HistoricYearsData
            viewModel.HistoricYearsData = historicData;
            viewModel.PolicyYearSummaries = policyYearSummaries;

            if (viewModel.InputModel.CarLLL <= 0)
            {
                ModelState.AddModelError("CarLLL", "Car LLL value must be greater than zero.");
                return View("Index", viewModel); // Return to the same view with validation error
            }

            if (ModelState.IsValid)
            {
                // Create a new ClientDetail object with the data from viewModel
                var clientDetail = new ClientDetail
                {
                    //QuoteNumber = viewModel.InputModel.QuoteNumber,
                    ClientName = viewModel.PolicyYearSummaries.Values.FirstOrDefault()?.ClientName,
                    //ClientName = viewModel.InputModel.ClientName,
                    CoverType = viewModel.InputModel.SelectedCoverType,
                    StartDate = viewModel.InputModel.StartDate?.ToString("yyyy-MM-dd"),
                    EndDate = viewModel.InputModel.EndDate?.ToString("yyyy-MM-dd"),
                    Excess = viewModel.InputModel.Excess,

                    CarNums = viewModel.PolicyYearSummaries.Values.FirstOrDefault()?.CarNums ?? 0,
                    VanNums = viewModel.PolicyYearSummaries.Values.FirstOrDefault()?.VanNums ?? 0,
                    MinibusNums = viewModel.PolicyYearSummaries.Values.FirstOrDefault()?.MinibusNums ?? 0,
                    HGVNums = viewModel.PolicyYearSummaries.Values.FirstOrDefault()?.HGVNums ?? 0,

                    CarLLL = viewModel.InputModel.CarLLL,
                    VanLLL = viewModel.InputModel.VanLLL,
                    MinibusLLL = viewModel.InputModel.MinibusLLL,
                    HGVLLL = viewModel.InputModel.HGVLLL,

                    CarExp = viewModel.PolicyYearSummaries.Values.FirstOrDefault().CarExposure,
                    VanExp = viewModel.PolicyYearSummaries.Values.FirstOrDefault().VanExposure,
                    MinibusExp = viewModel.PolicyYearSummaries.Values.FirstOrDefault().MinibusExposure,
                    HGVExp = viewModel.PolicyYearSummaries.Values.FirstOrDefault().HGVExposure,

                    FCDaysCOI = viewModel.PolicyYearSummaries.Values.FirstOrDefault()?.FCDaysCOI ?? 0,
                    FCDaysNonCOI = viewModel.PolicyYearSummaries.Values.FirstOrDefault()?.FCDaysNonCOI ?? 0,
                    FCTO_COI = viewModel.PolicyYearSummaries.Values.FirstOrDefault()?.FCTO_COI ?? 0,
                    FCTO_NonCOI = viewModel.PolicyYearSummaries.Values.FirstOrDefault()?.FCTO_NonCOI ?? 0,
                    ExpPercentage = Convert.ToDouble(viewModel.InputModel.SelectedPercentage),

                    Year1Name = viewModel.HistoricYearsData.Year1Name,
                    Year2Name = viewModel.HistoricYearsData.Year2Name,
                    Year3Name = viewModel.HistoricYearsData.Year3Name,
                    Year4Name = viewModel.HistoricYearsData.Year4Name,
                    Year5Name = viewModel.HistoricYearsData.Year5Name,

                    Y1RentalDaysCOI = viewModel.HistoricYearsData.Y1RentalDaysCOI,
                    Y2RentalDaysCOI = viewModel.HistoricYearsData.Y2RentalDaysCOI,
                    Y3RentalDaysCOI = viewModel.HistoricYearsData.Y3RentalDaysCOI,
                    Y4RentalDaysCOI = viewModel.HistoricYearsData.Y4RentalDaysCOI,
                    Y5RentalDaysCOI = viewModel.HistoricYearsData.Y5RentalDaysCOI,
                    ThreeY_RDaysCOI = viewModel.HistoricYearsData.ThreeY_RDaysCOI,
                    FiveY_RDaysCOI = viewModel.HistoricYearsData.FiveY_RDaysCOI,

                    Y1RentalDaysNonCOI = viewModel.HistoricYearsData.Y1RentalDaysNonCOI,
                    Y2RentalDaysNonCOI = viewModel.HistoricYearsData.Y2RentalDaysNonCOI,
                    Y3RentalDaysNonCOI = viewModel.HistoricYearsData.Y3RentalDaysNonCOI,
                    Y4RentalDaysNonCOI = viewModel.HistoricYearsData.Y4RentalDaysNonCOI,
                    Y5RentalDaysNonCOI = viewModel.HistoricYearsData.Y5RentalDaysNonCOI,
                    ThreeY_RDaysNonCOI = viewModel.HistoricYearsData.ThreeY_RDaysNonCOI,
                    FiveY_RDaysNonCOI = viewModel.HistoricYearsData.FiveY_RDaysNonCOI,

                    Y1TO_COI = viewModel.HistoricYearsData.Y1TO_COI,
                    Y2TO_COI = viewModel.HistoricYearsData.Y2TO_COI,
                    Y3TO_COI = viewModel.HistoricYearsData.Y3TO_COI,
                    Y4TO_COI = viewModel.HistoricYearsData.Y4TO_COI,
                    Y5TO_COI = viewModel.HistoricYearsData.Y5TO_COI,
                    ThreeY_TO_COI = viewModel.HistoricYearsData.ThreeY_TO_COI,
                    FiveY_TO_COI = viewModel.HistoricYearsData.FiveY_TO_COI,

                    Y1TO_NonCOI = viewModel.HistoricYearsData.Y1TO_NonCOI,
                    Y2TO_NonCOI = viewModel.HistoricYearsData.Y2TO_NonCOI,
                    Y3TO_NonCOI = viewModel.HistoricYearsData.Y3TO_NonCOI,
                    Y4TO_NonCOI = viewModel.HistoricYearsData.Y4TO_NonCOI,
                    Y5TO_NonCOI = viewModel.HistoricYearsData.Y5TO_NonCOI,
                    ThreeY_TO_NonCOI = viewModel.HistoricYearsData.ThreeY_TO_NonCOI,
                    FiveY_TO_NonCOI = viewModel.HistoricYearsData.FiveY_TO_NonCOI,

                    Y1UT = viewModel.HistoricYearsData.Y1UT,
                    Y2UT = viewModel.HistoricYearsData.Y2UT,
                    Y3UT = viewModel.HistoricYearsData.Y3UT,
                    Y4UT = viewModel.HistoricYearsData.Y4UT,
                    Y5UT = viewModel.HistoricYearsData.Y5UT,
                    ThreeY_UT = viewModel.HistoricYearsData.ThreeY_UT,
                    FiveY_UT = viewModel.HistoricYearsData.FiveY_UT,

                    Y1VYrs = viewModel.HistoricYearsData.Y1VYrs,
                    Y2VYrs = viewModel.HistoricYearsData.Y2VYrs,
                    Y3VYrs = viewModel.HistoricYearsData.Y3VYrs,
                    Y4VYrs = viewModel.HistoricYearsData.Y4VYrs,
                    Y5VYrs = viewModel.HistoricYearsData.Y5VYrs,
                    ThreeY_VYrs = viewModel.HistoricYearsData.ThreeY_VYrs,
                    FiveY_VYrs = viewModel.HistoricYearsData.FiveY_VYrs,

                    Y1ClaimsOpen = viewModel.HistoricYearsData.Y1ClaimsOpen,
                    Y2ClaimsOpen = viewModel.HistoricYearsData.Y2ClaimsOpen,
                    Y3ClaimsOpen = viewModel.HistoricYearsData.Y3ClaimsOpen,
                    Y4ClaimsOpen = viewModel.HistoricYearsData.Y4ClaimsOpen,
                    Y5ClaimsOpen = viewModel.HistoricYearsData.Y5ClaimsOpen,
                    ThreeY_ClaimsOpen = viewModel.HistoricYearsData.ThreeY_ClaimsOpen,
                    FiveY_ClaimsOpen = viewModel.HistoricYearsData.FiveY_ClaimsOpen,

                    Y1ClaimsClosed = viewModel.HistoricYearsData.Y1ClaimsClosed,
                    Y2ClaimsClosed = viewModel.HistoricYearsData.Y2ClaimsClosed,
                    Y3ClaimsClosed = viewModel.HistoricYearsData.Y3ClaimsClosed,
                    Y4ClaimsClosed = viewModel.HistoricYearsData.Y4ClaimsClosed,
                    Y5ClaimsClosed = viewModel.HistoricYearsData.Y5ClaimsClosed,
                    ThreeY_ClaimsClo = viewModel.HistoricYearsData.ThreeY_ClaimsClo,
                    FiveY_ClaimsClo = viewModel.HistoricYearsData.FiveY_ClaimsClo,

                    Y1ADPaid = viewModel.HistoricYearsData.Y1ADPaid,
                    Y2ADPaid = viewModel.HistoricYearsData.Y2ADPaid,
                    Y3ADPaid = viewModel.HistoricYearsData.Y3ADPaid,
                    Y4ADPaid = viewModel.HistoricYearsData.Y4ADPaid,
                    Y5ADPaid = viewModel.HistoricYearsData.Y5ADPaid,
                    ThreeY_AD = viewModel.HistoricYearsData.ThreeY_AD,
                    FiveY_AD = viewModel.HistoricYearsData.FiveY_AD,

                    Y1FTPaid = viewModel.HistoricYearsData.Y1FTPaid,
                    Y2FTPaid = viewModel.HistoricYearsData.Y2FTPaid,
                    Y3FTPaid = viewModel.HistoricYearsData.Y3FTPaid,
                    Y4FTPaid = viewModel.HistoricYearsData.Y4FTPaid,
                    Y5FTPaid = viewModel.HistoricYearsData.Y5FTPaid,
                    ThreeY_FT = viewModel.HistoricYearsData.ThreeY_FT,
                    FiveY_FT = viewModel.HistoricYearsData.FiveY_FT,

                    Y1TPPD = viewModel.HistoricYearsData.Y1TPPD,
                    Y2TPPD = viewModel.HistoricYearsData.Y2TPPD,
                    Y3TPPD = viewModel.HistoricYearsData.Y3TPPD,
                    Y4TPPD = viewModel.HistoricYearsData.Y4TPPD,
                    Y5TPPD = viewModel.HistoricYearsData.Y5TPPD,
                    ThreeY_TPPD = viewModel.HistoricYearsData.ThreeY_TPPD,
                    FiveY_TPPD = viewModel.HistoricYearsData.FiveY_TPPD,

                    Y1TPCH = viewModel.HistoricYearsData.Y1TPCH,
                    Y2TPCH = viewModel.HistoricYearsData.Y2TPCH,
                    Y3TPCH = viewModel.HistoricYearsData.Y3TPCH,
                    Y4TPCH = viewModel.HistoricYearsData.Y4TPCH,
                    Y5TPCH = viewModel.HistoricYearsData.Y5TPCH,
                    ThreeY_TPCH = viewModel.HistoricYearsData.ThreeY_TPCH,
                    FiveY_TPCH = viewModel.HistoricYearsData.FiveY_TPCH,

                    Y1TPPI = viewModel.HistoricYearsData.Y1TPPI,
                    Y2TPPI = viewModel.HistoricYearsData.Y2TPPI,
                    Y3TPPI = viewModel.HistoricYearsData.Y3TPPI,
                    Y4TPPI = viewModel.HistoricYearsData.Y4TPPI,
                    Y5TPPI = viewModel.HistoricYearsData.Y5TPPI,
                    ThreeY_TPPI = viewModel.HistoricYearsData.ThreeY_TPPI,
                    FiveY_TPPI = viewModel.HistoricYearsData.FiveY_TPPI,

                    Y1ADOS = viewModel.HistoricYearsData.Y1ADOS,
                    Y2ADOS = viewModel.HistoricYearsData.Y2ADOS,
                    Y3ADOS = viewModel.HistoricYearsData.Y3ADOS,
                    Y4ADOS = viewModel.HistoricYearsData.Y4ADOS,
                    Y5ADOS = viewModel.HistoricYearsData.Y5ADOS,
                    ThreeY_ADOS = viewModel.HistoricYearsData.ThreeY_ADOS,
                    FiveY_ADOS = viewModel.HistoricYearsData.FiveY_ADOS,

                    Y1FTOS = viewModel.HistoricYearsData.Y1FTOS,
                    Y2FTOS = viewModel.HistoricYearsData.Y2FTOS,
                    Y3FTOS = viewModel.HistoricYearsData.Y3FTOS,
                    Y4FTOS = viewModel.HistoricYearsData.Y4FTOS,
                    Y5FTOS = viewModel.HistoricYearsData.Y5FTOS,
                    ThreeY_FTOS = viewModel.HistoricYearsData.ThreeY_FTOS,
                    FiveY_FTOS = viewModel.HistoricYearsData.FiveY_FTOS,

                    Y1TPPDOS = viewModel.HistoricYearsData.Y1TPPDOS,
                    Y2TPPDOS = viewModel.HistoricYearsData.Y2TPPDOS,
                    Y3TPPDOS = viewModel.HistoricYearsData.Y3TPPDOS,
                    Y4TPPDOS = viewModel.HistoricYearsData.Y4TPPDOS,
                    Y5TPPDOS = viewModel.HistoricYearsData.Y5TPPDOS,
                    ThreeY_TPPDOS = viewModel.HistoricYearsData.ThreeY_TPPDOS,
                    FiveY_TPPDOS = viewModel.HistoricYearsData.FiveY_TPPDOS,

                    Y1TPCHOS = viewModel.HistoricYearsData.Y1TPCHOS,
                    Y2TPCHOS = viewModel.HistoricYearsData.Y2TPCHOS,
                    Y3TPCHOS = viewModel.HistoricYearsData.Y3TPCHOS,
                    Y4TPCHOS = viewModel.HistoricYearsData.Y4TPCHOS,
                    Y5TPCHOS = viewModel.HistoricYearsData.Y5TPCHOS,
                    ThreeY_TPCHOS = viewModel.HistoricYearsData.ThreeY_TPCHOS,
                    FiveY_TPCHOS = viewModel.HistoricYearsData.FiveY_TPCHOS,

                    Y1TPPIOS = viewModel.HistoricYearsData.Y1TPPIOS,
                    Y2TPPIOS = viewModel.HistoricYearsData.Y2TPPIOS,
                    Y3TPPIOS = viewModel.HistoricYearsData.Y3TPPIOS,
                    Y4TPPIOS = viewModel.HistoricYearsData.Y4TPPIOS,
                    Y5TPPIOS = viewModel.HistoricYearsData.Y5TPPIOS,
                    ThreeY_TPPIOS = viewModel.HistoricYearsData.ThreeY_TPPIOS,
                    FiveY_TPPIOS = viewModel.HistoricYearsData.FiveY_TPPIOS,

                    Y1Total = viewModel.HistoricYearsData.Y1Total,
                    Y2Total = viewModel.HistoricYearsData.Y2Total,
                    Y3Total = viewModel.HistoricYearsData.Y3Total,
                    Y4Total = viewModel.HistoricYearsData.Y4Total,
                    Y5Total = viewModel.HistoricYearsData.Y5Total,
                    ThreeY_Total = viewModel.HistoricYearsData.ThreeY_Total,
                    FiveY_Total = viewModel.HistoricYearsData.FiveY_Total,
                };

                // Save to the database
                _appContext.ClientDetails.Add(clientDetail);


                _appContext.SaveChanges();


                // Redirect page
                return RedirectToAction("IndexCalculations", "Calculations", new    // param1 is the action method, param2 is the controller
                {
                    quoteId = clientDetail.Id,
                    selectedNumOfMonths = "12",
                    projYears = "3",
                    chargeCOIFee = "1",
                    pricingMetric = "1",
                    priceBy = "1",
                    isSearched = "0",
                });
            }

            if (!ModelState.IsValid)
            {
                // Log the errors
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }
            var errorMessage = ModelState
                .Values
                .FirstOrDefault(ms => ms.Errors.Count > 0)?
                .Errors
                .First()
                .ErrorMessage ?? "An unknown validation error occured.";

            return View("Index", viewModel);
        }*/



        





















        /*  [HttpGet]
          public IActionResult IndexCalculations(int quoteId, string selectedNumOfMonths, string projYears, string chargeCOIFee, string pricingMetric, string priceBy)   // the query string params will be automatically mapped to the method args
          {


              //var inflationModel = _calculationsService.GetInflationCalculations(quoteId, selectedNumOfMonths);
              var calculationsModel = _calculationsService.GetCalculations(quoteId, selectedNumOfMonths, projYears, chargeCOIFee, pricingMetric, priceBy);
              //var inputModel = _calculationsService.GetInputModelData(quoteId);

              var viewModel = new ClaimsViewModel
              {
                  CalculationsModel = calculationsModel,
                  //InputModel = inputModel // Ensure InputModel is set here
              };




              calculationsModel.SelectedNumOfMonths = string.IsNullOrEmpty(selectedNumOfMonths) ? "12" : selectedNumOfMonths;

              var pdfBytes = _pdfService.CreatePDFReport(quoteId, viewModel);
              var pdfBase64 = Convert.ToBase64String(pdfBytes);
              ViewBag.PdfBase64 = pdfBase64;

              if (viewModel == null)
              {
                  return NotFound();
              }
              return PartialView("_IndexCalculationsPartial", viewModel);
          }*/
    }
}
