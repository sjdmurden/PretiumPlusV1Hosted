using CSV_reader.Services;
using Microsoft.AspNetCore.Mvc;
using CSV_reader.Models;
using System.Collections.Generic;
using CSV_reader.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using CSV_reader.database;
using static System.Net.WebRequestMethods;
using System.Security.Claims;

namespace CSV_reader.Controllers
    //The controller focuses on handling HTTP requests. It doesn't handle PDF creation directly, the service does that.
{
    public class PDFController : Controller
    {
        private readonly IPDFService _pdfService;
        private readonly ApplicationContext _appContext;

        // dependency injection to recieve instance of IPDFService
        public PDFController(IPDFService pdfService, ApplicationContext appContext)
        {
            _pdfService = pdfService;
            _appContext = appContext;
        }


        [HttpPost]
        public IActionResult GenerateQuoteDoc([FromBody] GeneratePdfRequestViewModel request)
        {
            Console.WriteLine("GenerateQuoteDoc method inside PDFController:");          

            try
            {
                var userEmail = User.FindFirstValue(ClaimTypes.Name) ?? "Unknown User";

                var pdfBytes = _pdfService.CreatePDFReport(
                    userEmail,
                    request.BatchId,
                    request.ClaimsAmount,
                    request.LargeLossFund,
                    request.ReinsuranceCosts,
                    request.ClaimsHandlingFee,
                    request.Levies,
                    request.Expenses,
                    request.Profit,
                    request.NetPremium,
                    request.Commissions,
                    request.GrossPremium,
                    request.UpdatedGrossPremiumPlusIPT,
                    request.AdjustmentNotes,
                    request.FCDaysCOI,
                    request.FCDaysNonCOI,
                    request.FCTurnoverCOI,
                    request.FCTurnoverNonCOI
                );

                var pdfBase64 = Convert.ToBase64String(pdfBytes);
                return Json(new { pdfBase64 });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating PDF: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return StatusCode(500, new { message = "PDF generation failed", error = ex.Message });
            }
        }




        // ---------------- POLICY DOC ---------------------

        [HttpPost]
        public IActionResult GeneratePolicyDoc([FromBody] PolicyDocPDFViewModel request)
        {
            Console.WriteLine($"GeneratePolicyDoc inside PDFController:");

            var userEmail = User.FindFirstValue(ClaimTypes.Name) ?? "Unknown User";

            var pdfBytes =
                _pdfService.CreatePolicyDoc
                (
                    userEmail,
                    request.BatchId,
                    request.PolicyNumber,

                    request.ClaimsAmount,
                    request.LargeLossFund,
                    request.ReinsuranceCosts,
                    request.ClaimsHandlingFee,
                    request.Levies,
                    request.Expenses,
                    request.Profit,
                    request.NetPremium,
                    request.Commissions,
                    request.GrossPremium,
                    request.UpdatedGrossPremiumPlusIPT,
                    request.AdjustmentNotes,
                    request.FCDaysCOI,
                    request.FCDaysNonCOI,
                    request.FCTurnoverCOI,
                    request.FCTurnoverNonCOI
                );

            var pdfBase64 = Convert.ToBase64String(pdfBytes);

            return Json(new { pdfBase64 });
        }


        // -------------- SAVING DOC DETAILS TO DOC DB TABLE -------------------------
        [HttpPost]
        public IActionResult SaveDocumentToDB([FromBody] DocumentSaveToDBViewModel request)
        {
            if (request == null)
                return BadRequest("Invalid request data");

            Console.WriteLine("inside SaveDocumentToDB in PDF controller");

            try
            {
                // Create a new document record
                var document = new DocumentDBTable
                {
                    DocumentId = new Random().Next(100000, 999999), // placeholder for now
                    CreatedDate = DateTime.UtcNow,
                    DocumentType = request.DocumentType, // or "Policy PDF" depending on the doc type
                    QuoteNumber = request.QuoteNumber,
                    PolicyNumber = request.PolicyNumber,
                    BatchId = request.BatchId,
                    ClientName = request.ClientName,
                    UserEmail = request.UserEmail
                };

                _appContext.DocumentDBTable.Add(document);
                _appContext.SaveChanges();

                return Ok(new { success = true, message = "Document saved successfully." });
            }
            catch (Exception ex)
            {
                // log excep msg
                return StatusCode(500, new { success = false, message = "Error saving document.", error = ex.Message });
            }
        }



        

    }
}
