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

        // dependency injection to recieve instance of IPDFService
        public PDFController(IPDFService pdfService)
        {
            _pdfService = pdfService;
        }


        [HttpPost]
        public IActionResult GeneratePDF([FromBody] GeneratePdfRequestViewModel request)
        {
            Console.WriteLine($"GeneratePDF inside PDFController:");
            Console.WriteLine($"quoteId: {request.BatchId}");
            Console.WriteLine($"updatedGrossPremium: {request.UpdatedGrossPremiumPlusIPT}");
            
            var userEmail = User.FindFirstValue(ClaimTypes.Name);

            var pdfBytes = 
                _pdfService.CreatePDFReport
                (
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



    }
}
