using CSV_reader.database;
using CSV_reader.Models;
using CSV_reader.Services;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;


namespace CSV_reader.Controllers
{
    [Authorize]
    public class ExcelController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly IClaimsService _claimsService;
        private readonly ApplicationContext _appContext;
        private readonly IExcelFileService _excelFileService;

        public ExcelController(IWebHostEnvironment webHostEnvironment, IConfiguration configuration, IClaimsService claimsService, ApplicationContext appContext, IExcelFileService excelFileService)
        {
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
            _claimsService = claimsService;
            _appContext = appContext;
            _excelFileService = excelFileService;
        }

        // Action to render the file upload view
        public IActionResult IndexExcel()
        {            

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile_Controller(IFormFile excelFile)
        {
            try
            {
                // Use the service to upload the file
                string filePath = await _excelFileService.UploadFile_Service(excelFile);         
                Console.WriteLine($"excel file path {filePath}");

                // Redirect to Index action method inside Home controller
                return RedirectToAction("Index2", "Home");
            }
            catch (ArgumentException ex)
            {
                // Handle invalid file upload error
                ModelState.AddModelError("", ex.Message);
                return View("Error");
            }
        }

        


        /*[HttpGet]
        public JsonResult CheckQuoteId(int quoteId)
        {
            //var tableData = _appContext.ClientDetails.FirstOrDefault(x => x.Id == quoteId);
            var quoteExists = _appContext.ClientDetails.Any(x => x.Id == quoteId);  // using Any returns a true/false value and is faster than FirstOrDefault
          
            return Json(new { exists = quoteExists });
            // this returns { "exists": true or flase }
        }*/

        [HttpGet]
        public JsonResult CheckQuoteIdExists(string quoteId)
        {
            var batchExists = _appContext.StaticClientDataDB.Any(x => x.BatchId.Contains(quoteId));  // using Any returns a true/false value and is faster than FirstOrDefault

            var batchId = _appContext.StaticClientDataDB
                .Where(x => x.BatchId.Contains(quoteId))
                .Select(x => x.BatchId)
                .FirstOrDefault();

            return Json(
                new {
                    exists = batchExists,
                    batchId = batchId,
                });
            // this returns { "exists": true or flase }
        }




    }
}