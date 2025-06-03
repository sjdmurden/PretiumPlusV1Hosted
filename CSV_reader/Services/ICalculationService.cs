using CSV_reader.Models;
using CSV_reader.ViewModels;

namespace CSV_reader.Services
{
    public interface ICalculationsService
    {
        CalculationsModel GetCalculations(int quoteId, string selectedNumOfMonths, string projYears, string chargeCOIFee, string pricingMetric, string priceBy);
        //CalculationsModel GetHistoricYearsDataTable(int quoteId);

        //InputModel GetInputModelData(int quoteId);

        public List<IndivClaimDB> GetClaimsByBatchId(string batchId);

      

    }
}