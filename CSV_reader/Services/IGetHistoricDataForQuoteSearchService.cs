using CSV_reader.Models;
using CSV_reader.ViewModels;

namespace CSV_reader.Services
{
    public interface IGetHistoricDataForQuoteSearchService
    {
        HistoricYearsData GetHistoricYearsDataForQuoteSearch(int quoteId);
    }
}
