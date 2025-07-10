using CSV_reader.Models;
using System.ComponentModel.DataAnnotations;

namespace CSV_reader.ViewModels
{    
    public class ClaimsPlusCalcsViewModel
    {
        public List<IndivClaimDataDB> BatchClaims { get; set; } = new List<IndivClaimDataDB>();
        public List<StaticClientDataDB> StaticClientData { get; set; } = new List<StaticClientDataDB>(); 
        public ClaimsCalculationsModel ClaimsCalculationsModel { get; set; } = new ClaimsCalculationsModel();

        public List<IndivClaimDataDB> FilteredBatchClaims { get; set; } = new List<IndivClaimDataDB>();
    }
}
