using CSV_reader.Models;

namespace CSV_reader.Services
{
    public interface IClaimsCalculationsService
    {
        public List<IndivClaimDB> GetClaimsByBatchId(string batchId);

        public List<IndivClaimDB> GetFilteredClaims(string batchId, List<string> selectedClaims);
        public ClaimsCalculationsModel GetCalculationsFromClaims(
            List<IndivClaimDataDB> batchClaimsData,
            List<StaticClientDataDB> staticClientData,
            string batchId,
            string selectedNumOfMonths,
            string projYears,
            string chargeCOIFee,
            string pricingMetric,
            string priceBy
            );
    }
}
