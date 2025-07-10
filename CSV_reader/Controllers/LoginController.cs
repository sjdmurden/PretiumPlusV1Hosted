using CSV_reader.Models;
using CSV_reader.Services;
using CSV_reader.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using DocumentFormat.OpenXml.Drawing.Wordprocessing;
using DocumentFormat.OpenXml.Spreadsheet;
using iText.Commons.Utils;
using Microsoft.VisualBasic;
using Org.BouncyCastle.Crypto;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace CSV_reader.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUserService _userService;
        private readonly IExcelFileService _excelFileService;

        public LoginController(IUserService userService, IExcelFileService excelFileService)
        {
            _userService = userService;
            _excelFileService = excelFileService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // If user still authenticated, redirect to Excel file uploader page
            if (User?.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("IndexExcel", "Excel");
            }
            return View(new LoginViewModel());
        }

        

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            var user = await _userService.Authenticate(viewModel.UserEmail, viewModel.UserPassword);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(viewModel);
            }

            // Set session or claims-based authentication
            /*HttpContext.Session.SetString("UserName", user.UserEmail);
            HttpContext.Session.SetInt32("UserType", user.UserType);*/

            // Redirect based on user type for futrue
            // return user.UserType == 1 ? RedirectToAction("AdminDashboard") : RedirectToAction("UserDashboard");



            // Create claims for the logged-in user
            /*
            Claims – user login authentication

            A claim is a key - value pair that represents specific information about a user. In claims-based authentication, claims are used to store user-related information, like their email, role, or permissions. Unlike session variables, claims are part of the user’s identity and can be automatically verified with each request to validate and authorise the user’s access to certain parts of the application.
            */
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserEmail),
                new Claim("UserType", user.UserType.ToString())
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Sign in the user with the created claims identity
            await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            new AuthenticationProperties
            {
                IsPersistent = true, // if false, session-based cookies are cleared on close
                ExpiresUtc = DateTime.UtcNow.AddMinutes(60) // how long until cookies expire
            }
            );


            // Redirect to Excel file uploader page
            return RedirectToAction("IndexExcel", "Excel");
        }

        // Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete(".AspNetCore.Cookies");
            _excelFileService.CleanupExcelFilePath();
            return RedirectToAction("Login", "Login");
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }
     
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _userService.RegisterUser(viewModel.UserEmail, viewModel.UserPassword);
                    // Redirect to a success page or login page
                    return RedirectToAction("Login", "Login");
                }
                catch (InvalidOperationException ex)
                {
                    // Add error to ModelState if username exists
                    ModelState.AddModelError("UserEmail", ex.Message);
                }
            }

            // getting to here must mean there is an error; redisplay the form
            return View(viewModel);
        }


    }

}
