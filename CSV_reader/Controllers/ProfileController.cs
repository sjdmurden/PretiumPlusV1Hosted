using CSV_reader.ViewModels;
using Microsoft.AspNetCore.Mvc;
using CSV_reader.database;
using CSV_reader.Models;
using System;
using DocumentFormat.OpenXml.InkML;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CSV_reader.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {

        private readonly ApplicationContext _appContext;

        public ProfileController(
            ApplicationContext appContext)
        {
            _appContext = appContext;
        }

        public IActionResult ProfileIndex()
        {
            var userEmail = User.Identity.Name;

            var staticClientData = _appContext.StaticClientDataDB
                .Where(x => x.UserEmail == userEmail)
                .ToList();

            var quotes = staticClientData
                .Where(data => data.BatchId.Contains("_"))
                .Select(data =>
                {
                    var parts = data.BatchId.Split('_');
                    var quoteIdString = parts[0];
                    var timestampStr = parts[1];

                    bool isValid = DateTime.TryParseExact(
                        timestampStr,
                        "yyyyMMddHHmm",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out DateTime timestamp);

                    return new ProfileViewModel.QuoteInfo
                    {
                        QuoteId = quoteIdString,
                        ClientName = data.ClientName,
                        CreatedDate = data.CreatedDate,
                    };
                })
                .ToList();

            var model = new ProfileViewModel
            {
                UserEmail = userEmail,
                Quotes = quotes
            };

            return View(model);
        }


        public IActionResult AdminProfileIndex()
        {
            var userEmail = User.Identity.Name;

            var userType = User.FindFirstValue("UserType");

            var allQuotesData = _appContext.StaticClientDataDB
                .Where(data => data.BatchId.Contains("_"))
                .ToList() 
                .Select(data =>
                {
                    var parts = data.BatchId.Split('_');
                    var batchIdString = parts[0];
                    var timestampStr = parts[1];

                    bool isValid = DateTime.TryParseExact(
                        timestampStr,
                        "yyyyMMddHHmm",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out DateTime timestamp);

                    return new AdminProfileViewModel.AdminQuoteInfo
                    {
                        UserEmail = data.UserEmail,
                        QuoteId = batchIdString,
                        ClientName = data.ClientName,
                        CreatedDate = data.CreatedDate,
                    };
                })
                .ToList();

            var allUsersInfo = _appContext.Users
                .Where(data => data.UserEmail.Contains("@"))
                .ToList()
                .Select(data =>
                {
                    var timestampStr = data.CreatedAt;

                    return new AdminProfileViewModel.AllUsers
                    {
                        UserEmail = data.UserEmail,
                        CreatedDate = data.CreatedAt
                    };
                })
                .ToList();

            var model = new AdminProfileViewModel
            {
                AllUsersQuotes = allQuotesData,
                AllUsersInfo = allUsersInfo
            };

            return View(model);
        }

    }
}
