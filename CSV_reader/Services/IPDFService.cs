using CSV_reader.Models;
using CSV_reader.ViewModels;

namespace CSV_reader.Services
{
    public interface IPDFService
    {
        byte[] CreatePDFReport(
            string batchId,
            decimal claimsAmount,
            decimal largeLossFund,
            decimal reinsuranceCosts,
            decimal claimsHandlingFee,
            decimal levies,
            decimal expenses,
            decimal profit,
            decimal netPremium,
            decimal commissions,
            decimal grossPremium,
            decimal updatedGrossPremiumPlusIPT, 
            string adjustmentNotes,
            int FCDaysCOI,
            int FCDaysNonCOI,
            decimal FCTurnoverCOI,
            decimal FCTurnoverNonCOI
            );


    }
}
