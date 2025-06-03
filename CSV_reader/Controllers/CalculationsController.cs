using Microsoft.AspNetCore.Mvc;
using CSV_reader.database;
using CSV_reader.Models;
using CSV_reader.Services;
using CSV_reader.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace CSV_reader.Controllers
{
    [Authorize]
    public class CalculationsController : Controller
    {
        private readonly ICalculationsService _calculationsService;
        private readonly IClaimsService _claimsService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationContext _appContext;
        private readonly IGetHistoricDataForQuoteSearchService _getHistoricDataForQuoteSearchService;
        private readonly IExcelFileService _excelFileService;
        private readonly IClaimsCalculationsService _claimsCalculationsService;

        public CalculationsController(ICalculationsService calculationsService, IClaimsService claimsService, IConfiguration configuration, IWebHostEnvironment webHostEnvironment, ApplicationContext appContext, IGetHistoricDataForQuoteSearchService getHistoricDataForQuoteSearchService, IExcelFileService excelFileService, IClaimsCalculationsService claimsCalculationsService)
        {
            _calculationsService = calculationsService;
            _claimsService = claimsService;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
            _appContext = appContext;
            _getHistoricDataForQuoteSearchService = getHistoricDataForQuoteSearchService;
            _excelFileService = excelFileService;
            _claimsCalculationsService = claimsCalculationsService;
        }
        
        

        /*[HttpGet]
        public IActionResult IndexCalculations(
            int quoteId, 
            string selectedNumOfMonths, 
            string projYears, 
            string chargeCOIFee, 
            string pricingMetric, 
            string priceBy, 
            string isSearched)   // the query string params will be automatically mapped to the method args
        {

            string excelFilePath = _excelFileService.GetExcelFilePath();

            var viewModel = new ClaimsViewModel();
            if (isSearched == "0")
            {

                var historicData = _claimsService.Historic3Years5YearsData(excelFilePath);
                viewModel.HistoricYearsData = historicData;
            }
            else if (isSearched == "1")
            {
                var historicData = _getHistoricDataForQuoteSearchService.GetHistoricYearsDataForQuoteSearch(quoteId);
                viewModel.HistoricYearsData = historicData;

            }
            

            //var inflationModel = _calculationsService.GetInflationCalculations(quoteId, selectedNumOfMonths);
            var calculationsModel = _calculationsService.GetCalculations(quoteId, selectedNumOfMonths, projYears, chargeCOIFee, pricingMetric, priceBy);
            viewModel.CalculationsModel = calculationsModel;




            //var historicYearsData = _calculationsService.GetHistoricYearsDataTable(quoteId);

            *//*var viewModel = new ClaimsViewModel
            {
                CalculationsModel = calculationsModel,
                HistoricYearsData = historicData,
                //InputModel = inputModel // Ensure InputModel is set here
            };*//*
            

            calculationsModel.SelectedNumOfMonths = string.IsNullOrEmpty(selectedNumOfMonths) ? "12" : selectedNumOfMonths;

            *//*var pdfBytes = _pdfService.CreatePDFReport(quoteId, viewModel);
            var pdfBase64 = Convert.ToBase64String(pdfBytes);
            ViewBag.PdfBase64 = pdfBase64;*//*

            if (viewModel == null)
            {
                return NotFound();
            }


            _excelFileService.DeleteFileByPath(excelFilePath);

            return View("IndexCalculations2", viewModel);
        }*/

    

       /* public IActionResult IndexCalculationsPlusClaims(
            List<IndivClaimDB> batchClaims,
            string batchId,
            string selectedNumOfMonths,
            string projYears,
            string chargeCOIFee,
            string pricingMetric,
            string priceBy)
        {

            // get all claims from the claims table
            batchClaims = _appContext.ClaimsTable
                        .Where(c => c.BatchId == batchId)
                        .ToList();            

            ViewBag.BatchId = batchId;

            string[] batchIdParts = batchId.Split("_");
            string quoteId = batchIdParts[0];
            Console.WriteLine($"QuoteId = {quoteId}");

            ViewBag.QuoteId = quoteId;

            var clientName = _appContext.ClaimsTable
                        .Where(c => c.BatchId == batchId)
                        .Select(c => c.ClientName)
                        .FirstOrDefault();

            ViewBag.ClientName = clientName;

            var calculations = _claimsCalculationsService.GetCalculationsFromClaims(batchClaims, batchId, selectedNumOfMonths, projYears, chargeCOIFee, pricingMetric, priceBy);

            var viewModel = new ClaimsPlusCalcsViewModel
            {
                BatchClaims = batchClaims,
                ClaimsCalculationsModel = calculations,
            };

            viewModel.ClaimsCalculationsModel.SelectedNumOfMonths = string.IsNullOrEmpty(selectedNumOfMonths) ? "12" : selectedNumOfMonths;

            // the following defines the dropdown lists for the view.
            // This guarantees that the dropdowns are populated before the view is rendered, making sure they are never null or empty which would lead to errors.
            // It also centralises the data preparation meaning if more dropdown values are needed, they can be added here easily.

            viewModel.ClaimsCalculationsModel.InflationMonths = new List<SelectListItem>
            {
                new SelectListItem { Text = "1", Value = "1", Selected = viewModel.ClaimsCalculationsModel.SelectedNumOfMonths == "1" },
                new SelectListItem { Text = "2", Value = "2", Selected = viewModel.ClaimsCalculationsModel.SelectedNumOfMonths == "2" },
                new SelectListItem { Text = "3", Value = "3", Selected = viewModel.ClaimsCalculationsModel.SelectedNumOfMonths == "3" },
                new SelectListItem { Text = "4", Value = "4", Selected = viewModel.ClaimsCalculationsModel.SelectedNumOfMonths == "4" },
                new SelectListItem { Text = "5", Value = "5", Selected = viewModel.ClaimsCalculationsModel.SelectedNumOfMonths == "5" },
                new SelectListItem { Text = "6", Value = "6", Selected = viewModel.ClaimsCalculationsModel.SelectedNumOfMonths == "6" },
                new SelectListItem { Text = "7", Value = "7", Selected = viewModel.ClaimsCalculationsModel.SelectedNumOfMonths == "7" },
                new SelectListItem { Text = "8", Value = "8", Selected = viewModel.ClaimsCalculationsModel.SelectedNumOfMonths == "8" },
                new SelectListItem { Text = "9", Value = "9", Selected = viewModel.ClaimsCalculationsModel.SelectedNumOfMonths == "9" },
                new SelectListItem { Text = "10", Value = "10", Selected = viewModel.ClaimsCalculationsModel.SelectedNumOfMonths == "10" },
                new SelectListItem { Text = "11", Value = "11", Selected = viewModel.ClaimsCalculationsModel.SelectedNumOfMonths == "11" },
                new SelectListItem { Text = "12", Value = "12", Selected = viewModel.ClaimsCalculationsModel.SelectedNumOfMonths == "12" }
            };

            viewModel.ClaimsCalculationsModel.ProjectedYears = new List<SelectListItem>
            {
                new SelectListItem { Text = "3", Value = "3", Selected = viewModel.ClaimsCalculationsModel.SelectedProjYears == "3" },
                new SelectListItem { Text = "5", Value = "5", Selected = viewModel.ClaimsCalculationsModel.SelectedProjYears == "5" }
            };

            viewModel.ClaimsCalculationsModel.ChargeCOIFee = new List<SelectListItem>
            {
                new SelectListItem { Text = "Yes", Value = "Yes", Selected = viewModel.ClaimsCalculationsModel.SelectedCOIFee == "Yes" },
                new SelectListItem { Text = "No", Value = "No", Selected = viewModel.ClaimsCalculationsModel.SelectedCOIFee == "No" }
            };

            viewModel.ClaimsCalculationsModel.PricingMetrics = new List<SelectListItem>
            {
                new SelectListItem { Text = "CCPVY", Value = "CCPVY", Selected = viewModel.ClaimsCalculationsModel.SelectedPricingMetric == "CCPVY" },
                new SelectListItem { Text = "Claims", Value = "Claims", Selected = viewModel.ClaimsCalculationsModel.SelectedPricingMetric == "Claims" },
                new SelectListItem { Text = "Days", Value = "Days", Selected = viewModel.ClaimsCalculationsModel.SelectedPricingMetric == "Days" }
            };

            viewModel.ClaimsCalculationsModel.PriceByFilters = new List<SelectListItem>
            {
                new SelectListItem { Text = "Experience", Value = "Experience", Selected = viewModel.ClaimsCalculationsModel.SelectedPriceBy == "Experience" },
                new SelectListItem { Text = "Exposure", Value = "Exposure", Selected = viewModel.ClaimsCalculationsModel.SelectedPriceBy == "Exposure" },
                new SelectListItem { Text = "Blend", Value = "Blend", Selected = viewModel.ClaimsCalculationsModel.SelectedPriceBy == "Blend" }
            };

            _excelFileService.CleanupExcelFilePath();


            return View(viewModel);
        }*/

        public IActionResult IndexCalculationsPlusClaims2(
            List<IndivClaimDataDB> batchClaimsData,
            List<StaticClientDataDB> staticClientData,
            string batchId,
            string selectedNumOfMonths,
            string projYears,
            string chargeCOIFee,
            string pricingMetric,
            string priceBy)
        {

            // get all claims from the IndivClaimData table            
            batchClaimsData = _appContext.IndivClaimData
                            .Where(c => c.BatchId == batchId)
                            .ToList();

            // get static info from staticClientDataDB table
            staticClientData = _appContext.StaticClientDataDB
                            .Where(c => c.BatchId == batchId)
                            .ToList();

            ViewBag.BatchId = batchId;

            string[] batchIdParts = batchId.Split("_");
            string quoteId = batchIdParts[0];
            Console.WriteLine($"QuoteId = {quoteId}");

            ViewBag.QuoteId = quoteId;

            var clientName = _appContext.IndivClaimData
                        .Where(c => c.BatchId == batchId)
                        .Select(c => c.ClientName)
                        .FirstOrDefault();

            ViewBag.ClientName = clientName;

            var calculations = 
                _claimsCalculationsService.GetCalculationsFromClaims(
                    batchClaimsData, 
                    staticClientData,
                    batchId, 
                    selectedNumOfMonths, 
                    projYears, 
                    chargeCOIFee, 
                    pricingMetric, 
                    priceBy);

            var viewModel = new ClaimsPlusCalcsViewModel
            {
                BatchClaims = batchClaimsData,
                StaticClientData = staticClientData,
                ClaimsCalculationsModel = calculations,
            };

            viewModel.ClaimsCalculationsModel.SelectedNumOfMonths = string.IsNullOrEmpty(selectedNumOfMonths) ? "12" : selectedNumOfMonths;

            // the following defines the dropdown lists for the view.
            // This guarantees that the dropdowns are populated before the view is rendered, making sure they are never null or empty which would lead to errors.
            // It also centralises the data preparation meaning if more dropdown values are needed, they can be added here easily.

            viewModel.ClaimsCalculationsModel.InflationMonths = new List<SelectListItem>
            {
                new SelectListItem { Text = "1", Value = "1", Selected = viewModel.ClaimsCalculationsModel.SelectedNumOfMonths == "1" },
                new SelectListItem { Text = "2", Value = "2", Selected = viewModel.ClaimsCalculationsModel.SelectedNumOfMonths == "2" },
                new SelectListItem { Text = "3", Value = "3", Selected = viewModel.ClaimsCalculationsModel.SelectedNumOfMonths == "3" },
                new SelectListItem { Text = "4", Value = "4", Selected = viewModel.ClaimsCalculationsModel.SelectedNumOfMonths == "4" },
                new SelectListItem { Text = "5", Value = "5", Selected = viewModel.ClaimsCalculationsModel.SelectedNumOfMonths == "5" },
                new SelectListItem { Text = "6", Value = "6", Selected = viewModel.ClaimsCalculationsModel.SelectedNumOfMonths == "6" },
                new SelectListItem { Text = "7", Value = "7", Selected = viewModel.ClaimsCalculationsModel.SelectedNumOfMonths == "7" },
                new SelectListItem { Text = "8", Value = "8", Selected = viewModel.ClaimsCalculationsModel.SelectedNumOfMonths == "8" },
                new SelectListItem { Text = "9", Value = "9", Selected = viewModel.ClaimsCalculationsModel.SelectedNumOfMonths == "9" },
                new SelectListItem { Text = "10", Value = "10", Selected = viewModel.ClaimsCalculationsModel.SelectedNumOfMonths == "10" },
                new SelectListItem { Text = "11", Value = "11", Selected = viewModel.ClaimsCalculationsModel.SelectedNumOfMonths == "11" },
                new SelectListItem { Text = "12", Value = "12", Selected = viewModel.ClaimsCalculationsModel.SelectedNumOfMonths == "12" }
            };

            viewModel.ClaimsCalculationsModel.ProjectedYears = new List<SelectListItem>
            {
                new SelectListItem { Text = "3", Value = "3", Selected = viewModel.ClaimsCalculationsModel.SelectedProjYears == "3" },
                new SelectListItem { Text = "5", Value = "5", Selected = viewModel.ClaimsCalculationsModel.SelectedProjYears == "5" }
            };

            viewModel.ClaimsCalculationsModel.ChargeCOIFee = new List<SelectListItem>
            {
                new SelectListItem { Text = "Yes", Value = "Yes", Selected = viewModel.ClaimsCalculationsModel.SelectedCOIFee == "Yes" },
                new SelectListItem { Text = "No", Value = "No", Selected = viewModel.ClaimsCalculationsModel.SelectedCOIFee == "No" }
            };

            viewModel.ClaimsCalculationsModel.PricingMetrics = new List<SelectListItem>
            {
                new SelectListItem { Text = "CCPVY", Value = "CCPVY", Selected = viewModel.ClaimsCalculationsModel.SelectedPricingMetric == "CCPVY" },
                new SelectListItem { Text = "Claims", Value = "Claims", Selected = viewModel.ClaimsCalculationsModel.SelectedPricingMetric == "Claims" },
                new SelectListItem { Text = "Days", Value = "Days", Selected = viewModel.ClaimsCalculationsModel.SelectedPricingMetric == "Days" }
            };

            viewModel.ClaimsCalculationsModel.PriceByFilters = new List<SelectListItem>
            {
                new SelectListItem { Text = "Experience", Value = "Experience", Selected = viewModel.ClaimsCalculationsModel.SelectedPriceBy == "Experience" },
                new SelectListItem { Text = "Exposure", Value = "Exposure", Selected = viewModel.ClaimsCalculationsModel.SelectedPriceBy == "Exposure" },
                new SelectListItem { Text = "Blend", Value = "Blend", Selected = viewModel.ClaimsCalculationsModel.SelectedPriceBy == "Blend" }
            };

            _excelFileService.CleanupExcelFilePath();


            return View(viewModel);
        }



        /*[HttpPost]
        public IActionResult GetFilteredClaims([FromBody] BatchClaimsRequest request)
        {
            List<IndivClaimDB> batchFilteredClaims = _appContext.ClaimsTable
                .Where(c => c.BatchId == request.BatchId && request.SelectedClaims.Contains(c.ClaimRef))
                .ToList();

            var calculations = _claimsCalculationsService.GetCalculationsFromClaims(
                batchFilteredClaims,
                request.BatchId,
                request.SelectedNumOfMonths,
                request.ProjYears,
                request.ChargeCOIFee,
                request.PricingMetric,
                request.PriceBy
            );

            var viewModel = new ClaimsPlusCalcsViewModel
            {
                BatchClaims = batchFilteredClaims,
                ClaimsCalculationsModel = calculations
            };

            return View("_CalculationsTablesPartial", viewModel);
        }*/

        [HttpPost]
        public IActionResult GetFilteredClaims([FromBody] BatchClaimsRequest request)
        {
            List<IndivClaimDataDB> batchFilteredClaims = _appContext.IndivClaimData
                .Where(c => c.BatchId == request.BatchId && request.SelectedClaims.Contains(c.ClaimRef))
                .ToList();

            List<StaticClientDataDB> staticClientData = _appContext.StaticClientDataDB
                .Where(c => c.BatchId == request.BatchId)
                .ToList();

            Console.WriteLine("selected claims: " + request.SelectedClaims);

            var calculations = _claimsCalculationsService.GetCalculationsFromClaims(
                batchFilteredClaims,
                staticClientData,
                request.BatchId,
                request.SelectedNumOfMonths,
                request.ProjYears,
                request.ChargeCOIFee,
                request.PricingMetric,
                request.PriceBy
            );

            var viewModel = new ClaimsPlusCalcsViewModel
            {
                FilteredBatchClaims = batchFilteredClaims,
                StaticClientData = staticClientData,
                ClaimsCalculationsModel = calculations
            };

            return View("_CalculationsTablesPartial", viewModel);
        }


        /*public IActionResult UpdateCOIContingent([FromBody] request)
        {
            var batchClaims = _appContext.ClaimsTable
                        .Where(c => c.BatchId == request.batchId)
                        .ToList();

            var fcDaysCOI = batchClaims.FirstOrDefault().ForecastDaysCOI;

            var newCOIContingent = fcDaysCOI * Request.
        }*/


    }
}

