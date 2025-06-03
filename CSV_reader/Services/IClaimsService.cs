using CSV_reader.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CSV_reader.Services
{
    public interface IClaimsService
    {
        List<ClaimsModel> ReadClaimsExcel(string filePath);
        Dictionary<string, PolicyYearSummary> GetPolicyYearSummaries(string filePath);
        HistoricYearsData Historic3Years5YearsData(string filePath);
        string SaveClaimsToDatabase(List<ClaimsModel> claimsData, InputModel inputModel);
        /*Dictionary<string, double> GetSumByPolicyYear(string filePath);
        public Dictionary<string, (int OpenClaims, int ClosedClaims)> GetSumOfStatusOfClaims(string filePath);*/
        //public List<ClaimsModel> GetOneYearsClaims(string filePath, string year);
    }
}
