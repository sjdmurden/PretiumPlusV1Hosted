using Microsoft.AspNetCore.Mvc;

namespace CSV_reader.Services
{
    public interface IExcelFileService
    {
        Task<string> UploadFile_Service(IFormFile excelFile);
        //void UpdateAppSettings(string filePath);
        bool DeleteFileByPath(string filePath);
        void CleanupExcelFilePath();
        void UpdateExcelFilePathInCache(string filePath);
        string? GetExcelFilePath();
    }
}
