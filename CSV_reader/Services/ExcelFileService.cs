using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using CSV_reader.Models;
using CSV_reader.Services;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Memory;
using Serilog;

namespace CSV_reader.Services
{
    public class ExcelFileService : IExcelFileService
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IClaimsService _claimsService;
        private readonly ILogger<ExcelFileService> _logger;
        private readonly IMemoryCache _memoryCache;

        private const string CacheKey = "ExcelFilePath";
        // This sets the CacheKey to be "ExcelFilePath"

        public ExcelFileService(IConfiguration configuration, IWebHostEnvironment webHostEnvironment, IClaimsService claimsService, ILogger<ExcelFileService> logger, IMemoryCache memoryCache)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
            _claimsService = claimsService;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        

        // when called it updates the in-memory cache with the new file path
        public void UpdateExcelFilePathInCache(string filePath)
        {
            // Store file path in cache and cache empties after 1 hour or if app is restarted
            // IMemoryCache is like a dictionary; the key is "ExcelFilePath" and the value is being set to the filepath
            var cacheExpirationMinutes = TimeSpan.FromMinutes(_configuration.GetValue<int>("CacheExpirationMins", 60));  // defaults to 60mins if it doesnt exist in json
            _memoryCache.Set(CacheKey, filePath, cacheExpirationMinutes);

        }

        // Retrieves the current ExcelFilePath from the cache as a string
        public string GetExcelFilePath()
        {
            return _memoryCache.TryGetValue(CacheKey, out string filePath)
                    ? filePath
                    : "No file path set.";
        }

        public void LogCache()
        {
            _logger.LogInformation($"Current cached ExcelFilePath: {GetExcelFilePath()}");
        }
        



        public async Task<string> UploadFile_Service(IFormFile excelFile)
        {
            // Ensure the excel file has contents and exists
            if (excelFile != null && excelFile.Length > 0)
            {
                // Create string filepath to the uploads folder
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");

                // Ensure the uploads folder exists
                Directory.CreateDirectory(uploadsFolder); 

                // Create string file path which is the excel file and uploads folder paths concatenated
                string filePath = Path.Combine(uploadsFolder, excelFile.FileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    // save file to uploads folder
                    await excelFile.CopyToAsync(fileStream);
                }

                // Update the ExcelFilePath in appsettings.json to be the file path to the excel file now in the uploads folder
                UpdateExcelFilePathInCache(filePath);

                // return the full path to the uploaded file - file path to the excel file inside the uploads folder
                return filePath;
                
            }

            throw new ArgumentException("Invalid file uploaded.");


        }

        

        public void UpdateAppSettings(string filePath)
         {
            var appSettingsPath = Path.Combine(_webHostEnvironment.ContentRootPath, "appsettings.json");

            var json = System.IO.File.ReadAllText(appSettingsPath);
            var jsonObj = JObject.Parse(json);
            jsonObj["ExcelFilePath"] = filePath;

            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
            System.IO.File.WriteAllText(appSettingsPath, output);

            
        }


        // return true if file deleted successfully, fals if not
        public bool DeleteFileByPath(string filePath)
        {
           
            // Check if the file exists at the provided path
            if (System.IO.File.Exists(filePath))
            {
                // Delete the file
                System.IO.File.Delete(filePath);
                return true; // File deleted successfully
            }
            else
            {
                Console.WriteLine($"File not found at path: {filePath}");
                return false; // File not found
            }
            
        }

       


        public void CleanupExcelFilePath()
        {
            
            var excelFilePathFromCache = GetExcelFilePath();

            if (!string.IsNullOrEmpty(excelFilePathFromCache))
            {
                // Attempt to delete the file
                if (DeleteFileByPath(excelFilePathFromCache))
                {
                    // Console.WriteLine($"Successfully deleted the file: {excelFilePathFromCache}");
                    Log.Information($"Successfully deleted the file: {excelFilePathFromCache}");
                }
                else
                {
                    // Console.WriteLine($"Failed to delete the file or file not found: {excelFilePathFromCache}");
                    Log.Warning($"Failed to delete the file or file not found: {excelFilePathFromCache}");
                }
            }

            // now clear the cache
            UpdateExcelFilePathInCache(string.Empty);
            // Console.WriteLine("ExcelFilePath cleared from cache");
            Log.Information("ExcelFilePath cleared in cache.");
        }

    }
}


/*
 * First attempt of using cache with static vairables :
 * 
 * public static class AppSettingsCache
        {
            // The ExcelFilePath property acts as a global variable that keps the file path in memory acorss the app's lifetime
            // "Static" means its value persists for as long as the app is running
            public static string ExcelFilePath { get; set; }
        }

  public static class AppSettingsCache
        {
            // The ExcelFilePath property acts as a global variable that keps the file path in memory acorss the app's lifetime
            // "Static" means its value persists for as long as the app is running
            public static string ExcelFilePath { get; set; }
        }

    // when called it updates the in-memory cache with the new file path
    public void UpdateExcelFilePath(string filePath)
    {
        AppSettingsCache.ExcelFilePath = filePath;

            
    }

    // Retrieves the current ExcelFilePath from the cache as a string
    public string GetExcelFilePath()
    {
        return AppSettingsCache.ExcelFilePath;
    }
 */