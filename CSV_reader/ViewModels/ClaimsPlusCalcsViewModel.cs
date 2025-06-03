using CSV_reader.Models;

namespace CSV_reader.ViewModels
{
    /*public class ClaimsPlusCalcsViewModel
    {
        public List<IndivClaimDB> BatchClaims { get; set; }
        public ClaimsCalculationsModel ClaimsCalculationsModel { get; set; }

        public List<IndivClaimDB> FilteredBatchClaims { get; set; }
    }*/

    public class ClaimsPlusCalcsViewModel
    {
        public List<IndivClaimDataDB> BatchClaims { get; set; }
        public List<StaticClientDataDB> StaticClientData { get; set; }
        public ClaimsCalculationsModel ClaimsCalculationsModel { get; set; }

        public List<IndivClaimDataDB> FilteredBatchClaims { get; set; }
    }
}
