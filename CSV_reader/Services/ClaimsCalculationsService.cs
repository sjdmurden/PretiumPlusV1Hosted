using CSV_reader.Models;
using CSV_reader.database;
using Microsoft.AspNetCore.Mvc.Rendering;
using CSV_reader.ViewModels;

namespace CSV_reader.Services
{
    public class ClaimsCalculationsService : IClaimsCalculationsService
    {
        private readonly ApplicationContext _appContext;
        private readonly IWebHostEnvironment _env;
        private readonly IClaimsService _claimsService;

        public ClaimsCalculationsService(ApplicationContext appContext, IWebHostEnvironment env, IClaimsService claimsService)
        {
            _appContext = appContext;
            _env = env;
            _claimsService = claimsService;
        }

        public List<IndivClaimDB> GetClaimsByBatchId(string batchId)
        {
            return _appContext.ClaimsTable
                        .Where(c => c.BatchId == batchId)
                        .ToList();
        }

        public List<IndivClaimDB> GetFilteredClaims(string batchId, List<string> selectedClaims)
        {
            return _appContext.ClaimsTable
                .Where(c => c.BatchId == batchId && selectedClaims.Contains(c.ClaimRef))
                .ToList();
        }


        public ClaimsCalculationsModel GetCalculationsFromClaims(
            List<IndivClaimDataDB> batchClaimsData,
            List<StaticClientDataDB> staticClientData,
            string batchId,
            string selectedNumOfMonths,
            string projYears, 
            string chargeCOIFee,
            string pricingMetric,
            string priceBy
            )
        {

            
            // Create dict with policy years as keys
            var claimsByYear = batchClaimsData
                .GroupBy(c => c.PolicyYear) // Group by policy year
                .ToDictionary(g => g.Key, g => g.ToList()); // Convert to dict

            var staticClientData_fromTable = _appContext.StaticClientDataDB
                .FirstOrDefault(x => x.BatchId == batchId);

            if (staticClientData_fromTable == null)
            {
                throw new InvalidOperationException($"Static client data not found for BatchId: {batchId}");
            }

            /*foreach (var kvp in claimsByYear)
            {
                Console.WriteLine($"Policy Year: {kvp.Key}, Claims Count: {kvp.Value.Count}");

                foreach (var claim in kvp.Value)
                {
                    Console.WriteLine($"ClaimRef: {claim.ClaimRef}, Total: {claim.Total}, RDaysCOI: {claim.RDaysCOI}, RDaysNonCOI: {claim.RDaysNonCOI}");
                }

                Console.WriteLine("--------------------------------------------------");
            }*/

            List<string> orderedPolicyYears = claimsByYear.Keys
                .OrderByDescending(year => year)
                .ToList();

            /*Console.WriteLine("Policy Years: " + string.Join(", ", orderedPolicyYears));
            Console.WriteLine(orderedPolicyYears[0]);*/
           

            string? year1 = null, year2 = null, year3 = null, year4 = null, year5 = null;

            if (orderedPolicyYears.Count >= 3)
            {
                year1 = orderedPolicyYears[0];
                year2 = orderedPolicyYears[1];
                year3 = orderedPolicyYears[2];
            }

            if (orderedPolicyYears.Count == 5)
            {
                year4 = orderedPolicyYears[3];
                year5 = orderedPolicyYears[4];
            }

            var claimsCalculationsModel = new ClaimsCalculationsModel();

            claimsCalculationsModel.Year1Name = year1;
            claimsCalculationsModel.Year2Name = year2;
            claimsCalculationsModel.Year3Name = year3;
            claimsCalculationsModel.Year4Name = year4;
            claimsCalculationsModel.Year5Name = year5;

            // ------------------ HISTORIC YEARS DATA TABLE ---------------------
            // NEW LOGIC FOR CLIENTS WITH ONLY 3 YEARS OF DATA ( 5 YEARS STILL WORKS )

            if (year1 is null || year2 is null || year3 is null)
            {
                throw new InvalidOperationException("At least three policy years are required for calculations.");
            }

            // rental days COI
            claimsCalculationsModel.Y1RentalDaysCOI = claimsByYear[year1].FirstOrDefault()?.RDaysCOI ?? 0;
            claimsCalculationsModel.Y2RentalDaysCOI = claimsByYear[year2].FirstOrDefault()?.RDaysCOI ?? 0;
            claimsCalculationsModel.Y3RentalDaysCOI = claimsByYear[year3].FirstOrDefault()?.RDaysCOI ?? 0;
            claimsCalculationsModel.ThreeY_RDaysCOI = claimsCalculationsModel.Y1RentalDaysCOI + claimsCalculationsModel.Y2RentalDaysCOI + claimsCalculationsModel.Y3RentalDaysCOI;
            if (year4 != null && claimsByYear.ContainsKey(year4))
            {
                claimsCalculationsModel.Y4RentalDaysCOI = claimsByYear[year4].FirstOrDefault()?.RDaysCOI ?? 0;
            }
            else { claimsCalculationsModel.Y4RentalDaysCOI = 0; }
            if (year5 != null && claimsByYear.ContainsKey(year5))
            {
                claimsCalculationsModel.Y5RentalDaysCOI = claimsByYear[year5].FirstOrDefault()?.RDaysCOI ?? 0;
            }
            else { claimsCalculationsModel.Y5RentalDaysCOI = 0; }
            claimsCalculationsModel.FiveY_RDaysCOI = claimsCalculationsModel.ThreeY_RDaysCOI + claimsCalculationsModel.Y4RentalDaysCOI + claimsCalculationsModel.Y5RentalDaysCOI;

            // rental days non COI
            claimsCalculationsModel.Y1RentalDaysNonCOI = claimsByYear[year1].FirstOrDefault()?.RDaysNonCOI ?? 0;
            claimsCalculationsModel.Y2RentalDaysNonCOI = claimsByYear[year2].FirstOrDefault()?.RDaysNonCOI ?? 0;
            claimsCalculationsModel.Y3RentalDaysNonCOI = claimsByYear[year3].FirstOrDefault()?.RDaysNonCOI ?? 0;
            claimsCalculationsModel.ThreeY_RDaysNonCOI = claimsCalculationsModel.Y1RentalDaysNonCOI + claimsCalculationsModel.Y2RentalDaysNonCOI + claimsCalculationsModel.Y3RentalDaysNonCOI;
            if (year4 != null && claimsByYear.ContainsKey(year4))
            {
                claimsCalculationsModel.Y4RentalDaysNonCOI = claimsByYear[year4].FirstOrDefault()?.RDaysNonCOI ?? 0;
            }
            else { claimsCalculationsModel.Y4RentalDaysNonCOI = 0; }
            if (year5 != null && claimsByYear.ContainsKey(year5))
            {
                claimsCalculationsModel.Y5RentalDaysNonCOI = claimsByYear[year5].FirstOrDefault()?.RDaysNonCOI ?? 0;
            }
            else { claimsCalculationsModel.Y5RentalDaysNonCOI = 0; }
            claimsCalculationsModel.FiveY_RDaysNonCOI = claimsCalculationsModel.ThreeY_RDaysNonCOI + claimsCalculationsModel.Y4RentalDaysNonCOI + claimsCalculationsModel.Y5RentalDaysNonCOI;

            // turnover COI
            claimsCalculationsModel.Y1TO_COI = claimsByYear[year1].FirstOrDefault()?.TurnoverCOI ?? 0;
            claimsCalculationsModel.Y2TO_COI = claimsByYear[year2].FirstOrDefault()?.TurnoverCOI ?? 0;
            claimsCalculationsModel.Y3TO_COI = claimsByYear[year3].FirstOrDefault()?.TurnoverCOI ?? 0;

            claimsCalculationsModel.ThreeY_TO_COI = claimsCalculationsModel.Y1TO_COI + claimsCalculationsModel.Y2TO_COI + claimsCalculationsModel.Y3TO_COI;

            if (year4 != null && claimsByYear.ContainsKey(year4))
            {
                claimsCalculationsModel.Y4TO_COI = claimsByYear[year4].FirstOrDefault()?.TurnoverCOI ?? 0;
            }
            else { claimsCalculationsModel.Y4TO_COI = 0; }
            if (year5 != null && claimsByYear.ContainsKey(year5))
            {
                claimsCalculationsModel.Y5TO_COI = claimsByYear[year5].FirstOrDefault()?.TurnoverCOI ?? 0;
            }
            else { claimsCalculationsModel.Y5TO_COI = 0; }

            claimsCalculationsModel.FiveY_TO_COI = claimsCalculationsModel.ThreeY_TO_COI + claimsCalculationsModel.Y4TO_COI + claimsCalculationsModel.Y5TO_COI;

            // turnover non COI
            claimsCalculationsModel.Y1TO_NonCOI = claimsByYear[year1].FirstOrDefault()?.TurnoverNonCOI ?? 0;
            claimsCalculationsModel.Y2TO_NonCOI = claimsByYear[year2].FirstOrDefault()?.TurnoverNonCOI ?? 0;
            claimsCalculationsModel.Y3TO_NonCOI = claimsByYear[year3].FirstOrDefault()?.TurnoverNonCOI ?? 0;

            claimsCalculationsModel.ThreeY_TO_NonCOI = claimsCalculationsModel.Y1TO_NonCOI + claimsCalculationsModel.Y2TO_NonCOI + claimsCalculationsModel.Y3TO_NonCOI;

            if (year4 != null && claimsByYear.ContainsKey(year4))
            {
                claimsCalculationsModel.Y4TO_NonCOI = claimsByYear[year4].FirstOrDefault()?.TurnoverNonCOI ?? 0;
            }
            else { claimsCalculationsModel.Y4TO_NonCOI = 0; }
            if (year5 != null && claimsByYear.ContainsKey(year5))
            {
                claimsCalculationsModel.Y5TO_NonCOI = claimsByYear[year5].FirstOrDefault()?.TurnoverNonCOI ?? 0;
            }
            else { claimsCalculationsModel.Y5TO_NonCOI = 0; }

            claimsCalculationsModel.FiveY_TO_NonCOI = claimsCalculationsModel.ThreeY_TO_NonCOI + claimsCalculationsModel.Y4TO_NonCOI + claimsCalculationsModel.Y5TO_NonCOI;

            // Utlisation rate is hardcoded in claimsCalculationsModel

            // vehicle years
            claimsCalculationsModel.Y1VYrs = (double)claimsCalculationsModel.Y1RentalDaysNonCOI / 365;
            claimsCalculationsModel.Y2VYrs = (double)claimsCalculationsModel.Y2RentalDaysNonCOI / 365;
            claimsCalculationsModel.Y3VYrs = (double)claimsCalculationsModel.Y3RentalDaysNonCOI / 365;
            claimsCalculationsModel.Y4VYrs = (double)claimsCalculationsModel.Y4RentalDaysNonCOI / 365;
            claimsCalculationsModel.Y5VYrs = (double)claimsCalculationsModel.Y5RentalDaysNonCOI / 365;

            claimsCalculationsModel.ThreeY_VYrs = claimsCalculationsModel.Y1VYrs + claimsCalculationsModel.Y2VYrs + claimsCalculationsModel.Y3VYrs;
            claimsCalculationsModel.FiveY_VYrs = claimsCalculationsModel.Y1VYrs + claimsCalculationsModel.Y2VYrs + claimsCalculationsModel.Y3VYrs + claimsCalculationsModel.Y4VYrs + claimsCalculationsModel.Y5VYrs;

            // number of claims open
            claimsCalculationsModel.Y1ClaimsOpen = claimsByYear[year1]
                .Count(claim => claim.Status == "O" || claim.Status == "Opened" || claim.Status == "Active" || claim.Status == "Reopened" || claim.Status == "ACTIVE");
            claimsCalculationsModel.Y2ClaimsOpen = claimsByYear[year2]
                .Count(claim => claim.Status == "O" || claim.Status == "Opened" || claim.Status == "Active" || claim.Status == "Reopened" || claim.Status == "ACTIVE");
            claimsCalculationsModel.Y3ClaimsOpen = claimsByYear[year3]
                .Count(claim => claim.Status == "O" || claim.Status == "Opened" || claim.Status == "Active" || claim.Status == "Reopened" || claim.Status == "ACTIVE");

            claimsCalculationsModel.ThreeY_ClaimsOpen = claimsCalculationsModel.Y1ClaimsOpen + claimsCalculationsModel.Y2ClaimsOpen + claimsCalculationsModel.Y3ClaimsOpen;

            if (year4 != null && claimsByYear.ContainsKey(year4))
            {
                claimsCalculationsModel.Y4ClaimsOpen = claimsByYear[year4]
                .Count(claim => claim.Status == "O" || claim.Status == "Opened" || claim.Status == "Active" || claim.Status == "Reopened" || claim.Status == "ACTIVE");
            } else { claimsCalculationsModel.Y4ClaimsOpen = 0; }
            if (year5 != null && claimsByYear.ContainsKey(year5))
            {
                claimsCalculationsModel.Y5ClaimsOpen = claimsByYear[year5]
                .Count(claim => claim.Status == "O" || claim.Status == "Opened" || claim.Status == "Active" || claim.Status == "Reopened" || claim.Status == "ACTIVE");
            } else { claimsCalculationsModel.Y5ClaimsOpen = 0; }

            claimsCalculationsModel.FiveY_ClaimsOpen = claimsCalculationsModel.ThreeY_ClaimsOpen + claimsCalculationsModel.Y4ClaimsOpen + claimsCalculationsModel.Y5ClaimsOpen;

            // number of claims closed
            claimsCalculationsModel.Y1ClaimsClosed = claimsByYear[year1]
                .Count(claim => claim.Status == "S" || claim.Status == "Settled" || claim.Status == "Closed" || claim.Status == "Information Only" || claim.Status == "SETTLED" || claim.Status == "CANCELLED");
            claimsCalculationsModel.Y2ClaimsClosed = claimsByYear[year2]
                .Count(claim => claim.Status == "S" || claim.Status == "Settled" || claim.Status == "Closed" || claim.Status == "Information Only" || claim.Status == "SETTLED" || claim.Status == "CANCELLED");
            claimsCalculationsModel.Y3ClaimsClosed = claimsByYear[year3]
                .Count(claim => claim.Status == "S" || claim.Status == "Settled" || claim.Status == "Closed" || claim.Status == "Information Only" || claim.Status == "SETTLED" || claim.Status == "CANCELLED");

            claimsCalculationsModel.ThreeY_ClaimsClo = claimsCalculationsModel.Y1ClaimsClosed + claimsCalculationsModel.Y2ClaimsClosed + claimsCalculationsModel.Y3ClaimsClosed;

            if (year4 != null && claimsByYear.ContainsKey(year4))
            {
                claimsCalculationsModel.Y4ClaimsClosed = claimsByYear[year4]
                .Count(claim => claim.Status == "S" || claim.Status == "Settled" || claim.Status == "Closed" || claim.Status == "Information Only" || claim.Status == "SETTLED" || claim.Status == "CANCELLED");
            }
            else { claimsCalculationsModel.Y4ClaimsClosed = 0; }
            if (year5 != null && claimsByYear.ContainsKey(year5))
            {
                claimsCalculationsModel.Y5ClaimsClosed = claimsByYear[year5]
                .Count(claim => claim.Status == "S" || claim.Status == "Settled" || claim.Status == "Closed" || claim.Status == "Information Only" || claim.Status == "SETTLED" || claim.Status == "CANCELLED");
            } else { claimsCalculationsModel.Y5ClaimsClosed = 0; }

            claimsCalculationsModel.FiveY_ClaimsClo = claimsCalculationsModel.ThreeY_ClaimsClo + claimsCalculationsModel.Y4ClaimsClosed + claimsCalculationsModel.Y5ClaimsClosed;

            // AD paid
            claimsCalculationsModel.Y1ADPaid = claimsByYear[year1].Sum(claim => claim.AD_Paid);
            claimsCalculationsModel.Y2ADPaid = claimsByYear[year2].Sum(claim => claim.AD_Paid);
            claimsCalculationsModel.Y3ADPaid = claimsByYear[year3].Sum(claim => claim.AD_Paid);

            claimsCalculationsModel.ThreeY_ADPaid = claimsCalculationsModel.Y1ADPaid + claimsCalculationsModel.Y2ADPaid + claimsCalculationsModel.Y3ADPaid;

            if (year4 != null && claimsByYear.ContainsKey(year4))
            {
                claimsCalculationsModel.Y4ADPaid = claimsByYear[year4].Sum(claim => claim.AD_Paid);
            }
            else { claimsCalculationsModel.Y4ADPaid = 0; }
            if (year5 != null && claimsByYear.ContainsKey(year5))
            {
                claimsCalculationsModel.Y5ADPaid = claimsByYear[year5].Sum(claim => claim.AD_Paid);
            }
            else { claimsCalculationsModel.Y5ADPaid = 0; }

            claimsCalculationsModel.FiveY_ADPaid = claimsCalculationsModel.ThreeY_ADPaid + claimsCalculationsModel.Y4ADPaid + claimsCalculationsModel.Y5ADPaid;

            // FT paid
            claimsCalculationsModel.Y1FTPaid = claimsByYear[year1].Sum(claim => claim.FT_Paid);
            claimsCalculationsModel.Y2FTPaid = claimsByYear[year2].Sum(claim => claim.FT_Paid);
            claimsCalculationsModel.Y3FTPaid = claimsByYear[year3].Sum(claim => claim.FT_Paid);

            claimsCalculationsModel.ThreeY_FTPaid = claimsCalculationsModel.Y1FTPaid + claimsCalculationsModel.Y2FTPaid + claimsCalculationsModel.Y3FTPaid;

            if (year4 != null && claimsByYear.ContainsKey(year4))
            {
                claimsCalculationsModel.Y4FTPaid = claimsByYear[year4].Sum(claim => claim.FT_Paid);
            }
            else
            {
                claimsCalculationsModel.Y4FTPaid = 0;
            }

            if (year5 != null && claimsByYear.ContainsKey(year5))
            {
                claimsCalculationsModel.Y5FTPaid = claimsByYear[year5].Sum(claim => claim.FT_Paid);
            }
            else
            {
                claimsCalculationsModel.Y5FTPaid = 0;
            }

            claimsCalculationsModel.FiveY_FTPaid = claimsCalculationsModel.ThreeY_FTPaid + claimsCalculationsModel.Y4FTPaid + claimsCalculationsModel.Y5FTPaid;

            // TPPD Paid
            claimsCalculationsModel.Y1TPPD = claimsByYear[year1].Sum(claim => claim.TPPD_Paid);
            claimsCalculationsModel.Y2TPPD = claimsByYear[year2].Sum(claim => claim.TPPD_Paid);
            claimsCalculationsModel.Y3TPPD = claimsByYear[year3].Sum(claim => claim.TPPD_Paid);

            claimsCalculationsModel.ThreeY_TPPD = claimsCalculationsModel.Y1TPPD + claimsCalculationsModel.Y2TPPD + claimsCalculationsModel.Y3TPPD;

            claimsCalculationsModel.Y4TPPD = (year4 != null && claimsByYear.ContainsKey(year4))
                ? claimsByYear[year4].Sum(claim => claim.TPPD_Paid)
                : 0;

            claimsCalculationsModel.Y5TPPD = (year5 != null && claimsByYear.ContainsKey(year5))
                ? claimsByYear[year5].Sum(claim => claim.TPPD_Paid)
                : 0;

            claimsCalculationsModel.FiveY_TPPD = claimsCalculationsModel.ThreeY_TPPD + claimsCalculationsModel.Y4TPPD + claimsCalculationsModel.Y5TPPD;

            // TPCH Paid
            claimsCalculationsModel.Y1TPCH = claimsByYear[year1].Sum(claim => claim.TPCH_Paid);
            claimsCalculationsModel.Y2TPCH = claimsByYear[year2].Sum(claim => claim.TPCH_Paid);
            claimsCalculationsModel.Y3TPCH = claimsByYear[year3].Sum(claim => claim.TPCH_Paid);

            claimsCalculationsModel.ThreeY_TPCH = claimsCalculationsModel.Y1TPCH + claimsCalculationsModel.Y2TPCH + claimsCalculationsModel.Y3TPCH;

            claimsCalculationsModel.Y4TPCH = (year4 != null && claimsByYear.ContainsKey(year4))
                ? claimsByYear[year4].Sum(claim => claim.TPCH_Paid)
                : 0;

            claimsCalculationsModel.Y5TPCH = (year5 != null && claimsByYear.ContainsKey(year5))
                ? claimsByYear[year5].Sum(claim => claim.TPCH_Paid)
                : 0;

            claimsCalculationsModel.FiveY_TPCH = claimsCalculationsModel.ThreeY_TPCH + claimsCalculationsModel.Y4TPCH + claimsCalculationsModel.Y5TPCH;

            // TPPI Paid
            claimsCalculationsModel.Y1TPPI = claimsByYear[year1].Sum(claim => claim.TPPI_Paid);
            claimsCalculationsModel.Y2TPPI = claimsByYear[year2].Sum(claim => claim.TPPI_Paid);
            claimsCalculationsModel.Y3TPPI = claimsByYear[year3].Sum(claim => claim.TPPI_Paid);

            claimsCalculationsModel.ThreeY_TPPI = claimsCalculationsModel.Y1TPPI + claimsCalculationsModel.Y2TPPI + claimsCalculationsModel.Y3TPPI;

            claimsCalculationsModel.Y4TPPI = (year4 != null && claimsByYear.ContainsKey(year4))
                ? claimsByYear[year4].Sum(claim => claim.TPPI_Paid)
                : 0;

            claimsCalculationsModel.Y5TPPI = (year5 != null && claimsByYear.ContainsKey(year5))
                ? claimsByYear[year5].Sum(claim => claim.TPPI_Paid)
                : 0;

            claimsCalculationsModel.FiveY_TPPI = claimsCalculationsModel.ThreeY_TPPI + claimsCalculationsModel.Y4TPPI + claimsCalculationsModel.Y5TPPI;

            // ADOS
            claimsCalculationsModel.Y1ADOS = claimsByYear[year1].Sum(claim => claim.ADOS);
            claimsCalculationsModel.Y2ADOS = claimsByYear[year2].Sum(claim => claim.ADOS);
            claimsCalculationsModel.Y3ADOS = claimsByYear[year3].Sum(claim => claim.ADOS);

            claimsCalculationsModel.ThreeY_ADOS = claimsCalculationsModel.Y1ADOS + claimsCalculationsModel.Y2ADOS + claimsCalculationsModel.Y3ADOS;

            claimsCalculationsModel.Y4ADOS = (year4 != null && claimsByYear.ContainsKey(year4))
                ? claimsByYear[year4].Sum(claim => claim.ADOS)
                : 0;

            claimsCalculationsModel.Y5ADOS = (year5 != null && claimsByYear.ContainsKey(year5))
                ? claimsByYear[year5].Sum(claim => claim.ADOS)
                : 0;

            claimsCalculationsModel.FiveY_ADOS = claimsCalculationsModel.ThreeY_ADOS + claimsCalculationsModel.Y4ADOS + claimsCalculationsModel.Y5ADOS;

            // FTOS
            claimsCalculationsModel.Y1FTOS = claimsByYear[year1].Sum(claim => claim.FTOS);
            claimsCalculationsModel.Y2FTOS = claimsByYear[year2].Sum(claim => claim.FTOS);
            claimsCalculationsModel.Y3FTOS = claimsByYear[year3].Sum(claim => claim.FTOS);

            claimsCalculationsModel.ThreeY_FTOS = claimsCalculationsModel.Y1FTOS + claimsCalculationsModel.Y2FTOS + claimsCalculationsModel.Y3FTOS;

            claimsCalculationsModel.Y4FTOS = (year4 != null && claimsByYear.ContainsKey(year4))
                ? claimsByYear[year4].Sum(claim => claim.FTOS)
                : 0;

            claimsCalculationsModel.Y5FTOS = (year5 != null && claimsByYear.ContainsKey(year5))
                ? claimsByYear[year5].Sum(claim => claim.FTOS)
                : 0;

            claimsCalculationsModel.FiveY_FTOS = claimsCalculationsModel.ThreeY_FTOS + claimsCalculationsModel.Y4FTOS + claimsCalculationsModel.Y5FTOS;

            // TPPD_OS
            claimsCalculationsModel.Y1TPPDOS = claimsByYear[year1].Sum(claim => claim.TPPD_OS);
            claimsCalculationsModel.Y2TPPDOS = claimsByYear[year2].Sum(claim => claim.TPPD_OS);
            claimsCalculationsModel.Y3TPPDOS = claimsByYear[year3].Sum(claim => claim.TPPD_OS);

            claimsCalculationsModel.ThreeY_TPPDOS = claimsCalculationsModel.Y1TPPDOS + claimsCalculationsModel.Y2TPPDOS + claimsCalculationsModel.Y3TPPDOS;

            claimsCalculationsModel.Y4TPPDOS = (year4 != null && claimsByYear.ContainsKey(year4))
                ? claimsByYear[year4].Sum(claim => claim.TPPD_OS)
                : 0;

            claimsCalculationsModel.Y5TPPDOS = (year5 != null && claimsByYear.ContainsKey(year5))
                ? claimsByYear[year5].Sum(claim => claim.TPPD_OS)
                : 0;

            claimsCalculationsModel.FiveY_TPPDOS = claimsCalculationsModel.ThreeY_TPPDOS + claimsCalculationsModel.Y4TPPDOS + claimsCalculationsModel.Y5TPPDOS;

            // TPCH_OS
            claimsCalculationsModel.Y1TPCHOS = claimsByYear[year1].Sum(claim => claim.TPCH_OS);
            claimsCalculationsModel.Y2TPCHOS = claimsByYear[year2].Sum(claim => claim.TPCH_OS);
            claimsCalculationsModel.Y3TPCHOS = claimsByYear[year3].Sum(claim => claim.TPCH_OS);

            claimsCalculationsModel.ThreeY_TPCHOS = claimsCalculationsModel.Y1TPCHOS + claimsCalculationsModel.Y2TPCHOS + claimsCalculationsModel.Y3TPCHOS;

            claimsCalculationsModel.Y4TPCHOS = (year4 != null && claimsByYear.ContainsKey(year4))
                ? claimsByYear[year4].Sum(claim => claim.TPCH_OS)
                : 0;

            claimsCalculationsModel.Y5TPCHOS = (year5 != null && claimsByYear.ContainsKey(year5))
                ? claimsByYear[year5].Sum(claim => claim.TPCH_OS)
                : 0;

            claimsCalculationsModel.FiveY_TPCHOS = claimsCalculationsModel.ThreeY_TPCHOS + claimsCalculationsModel.Y4TPCHOS + claimsCalculationsModel.Y5TPCHOS;

            // TPPI_OS
            claimsCalculationsModel.Y1TPPIOS = claimsByYear[year1].Sum(c => c.TPPI_OS);
            claimsCalculationsModel.Y2TPPIOS = claimsByYear[year2].Sum(c => c.TPPI_OS);
            claimsCalculationsModel.Y3TPPIOS = claimsByYear[year3].Sum(c => c.TPPI_OS);

            claimsCalculationsModel.ThreeY_TPPIOS = claimsCalculationsModel.Y1TPPIOS + claimsCalculationsModel.Y2TPPIOS + claimsCalculationsModel.Y3TPPIOS;

            claimsCalculationsModel.Y4TPPIOS = (year4 != null && claimsByYear.ContainsKey(year4))
                ? claimsByYear[year4].Sum(c => c.TPPI_OS)
                : 0;

            claimsCalculationsModel.Y5TPPIOS = (year5 != null && claimsByYear.ContainsKey(year5))
                ? claimsByYear[year5].Sum(c => c.TPPI_OS)
                : 0;

            claimsCalculationsModel.FiveY_TPPIOS = claimsCalculationsModel.ThreeY_TPPIOS + claimsCalculationsModel.Y4TPPIOS + claimsCalculationsModel.Y5TPPIOS;

            // Total
            claimsCalculationsModel.Y1Total = claimsByYear[year1].Sum(c => c.Total);
            claimsCalculationsModel.Y2Total = claimsByYear[year2].Sum(c => c.Total);
            claimsCalculationsModel.Y3Total = claimsByYear[year3].Sum(c => c.Total);

            claimsCalculationsModel.ThreeY_Total = claimsCalculationsModel.Y1Total + claimsCalculationsModel.Y2Total + claimsCalculationsModel.Y3Total;

            claimsCalculationsModel.Y4Total = (year4 != null && claimsByYear.ContainsKey(year4))
                ? claimsByYear[year4].Sum(c => c.Total)
                : 0;

            claimsCalculationsModel.Y5Total = (year5 != null && claimsByYear.ContainsKey(year5))
                ? claimsByYear[year5].Sum(c => c.Total)
                : 0;

            claimsCalculationsModel.FiveY_Total = claimsCalculationsModel.ThreeY_Total + claimsCalculationsModel.Y4Total + claimsCalculationsModel.Y5Total;




            

            // business forecast

            /*claimsCalculationsModel.FCDaysCOI = claimsByYear[year1].FirstOrDefault().ForecastDaysCOI;
            claimsCalculationsModel.FCDaysNonCOI = claimsByYear[year1].FirstOrDefault().ForecastDaysNonCOI;
            claimsCalculationsModel.FCTurnoverCOI = claimsByYear[year1].FirstOrDefault().ForecastTO_COI;
            claimsCalculationsModel.FCTurnoverNonCOI = claimsByYear[year1].FirstOrDefault().ForecastTO_NonCOI;*/

            claimsCalculationsModel.FCDaysCOI = staticClientData_fromTable.ForecastDaysCOI;
            claimsCalculationsModel.FCDaysNonCOI = staticClientData_fromTable.ForecastDaysNonCOI;
            claimsCalculationsModel.FCTurnoverCOI = staticClientData_fromTable.ForecastTO_COI;
            claimsCalculationsModel.FCTurnoverNonCOI = staticClientData_fromTable.ForecastTO_NonCOI;



            // Parse the selectedNumOfMonths and assign it to InfMonth
            claimsCalculationsModel.SelectedNumOfMonths = selectedNumOfMonths;
            if (int.TryParse(selectedNumOfMonths, out int parsedMonths))
            {
                claimsCalculationsModel.InfMonth = parsedMonths;
            }
            else
            {
                // Handle invalid number of months, default to 12 if parsing fails
                claimsCalculationsModel.InfMonth = 12;
            }
            claimsCalculationsModel.SelectedProjYears = projYears;
            if (int.TryParse(projYears, out int parsedYears))
            {
                claimsCalculationsModel.ProjYear = parsedYears;
            }
            else
            {
                claimsCalculationsModel.ProjYear = 3;
            }

            // charge COI fee:
            if(chargeCOIFee == "Yes")
            {
                claimsCalculationsModel.ChargeCOIFeeValue = 1;
            }
            else if (chargeCOIFee == "No")
            {
                claimsCalculationsModel.ChargeCOIFeeValue = 0;
            }

            // pricing metric:
            if (pricingMetric == "CCPVY")
            {
                claimsCalculationsModel.PricingMetricValue = 1;
            }
            else if (pricingMetric == "Claims")
            {
                claimsCalculationsModel.PricingMetricValue = 2;
            }
            else if (pricingMetric == "Days")
            {
                claimsCalculationsModel.PricingMetricValue = 3;
            }


            // priceBy:
            if (priceBy == "Experience")
            {
                claimsCalculationsModel.PriceByValue = 1;
            }
            else if (priceBy == "Exposure")
            {
                claimsCalculationsModel.PriceByValue = 2;
            }
            else if (priceBy == "Blend")
            {
                claimsCalculationsModel.PriceByValue = 3;
            }

            /*claimsCalculationsModel.SelectedCOIFee = chargeCOIFee;
            if (int.TryParse(chargeCOIFee, out int parsedCOIFee))
            {
                claimsCalculationsModel.ChargeCOIFeeValue = parsedCOIFee;
            }
            else
            {
                claimsCalculationsModel.ChargeCOIFeeValue = 1;
            }
            
            claimsCalculationsModel.SelectedPriceBy = priceBy;
            if (int.TryParse(priceBy, out int parsedPriceBy))
            {
                claimsCalculationsModel.PriceByValue = parsedPriceBy;
            }
            else
            {
                claimsCalculationsModel.PriceByValue = 1;
            }
            
            claimsCalculationsModel.SelectedPricingMetric = pricingMetric;
            if (int.TryParse(pricingMetric, out int parsedPricingMetric))
            {
                claimsCalculationsModel.PricingMetricValue = parsedPricingMetric;
            }
            else
            {
                claimsCalculationsModel.PricingMetricValue = 1;
            }
            */




            // ----------------------- DATA TABLE -----------------------------------

            // Accident Frequency is calculated by dividing the number of claims by the Vehicle Years
            claimsCalculationsModel.Y1AccFreq = 
                claimsCalculationsModel.Y1VYrs == 0 ? 0 : (double)(claimsCalculationsModel.Y1ClaimsOpen + claimsCalculationsModel.Y1ClaimsClosed) / claimsCalculationsModel.Y1VYrs * 100;            
            claimsCalculationsModel.Y2AccFreq =
                claimsCalculationsModel.Y2VYrs == 0 ? 0 : (double)(claimsCalculationsModel.Y2ClaimsOpen + claimsCalculationsModel.Y2ClaimsClosed) / claimsCalculationsModel.Y2VYrs * 100;
            claimsCalculationsModel.Y3AccFreq =
                claimsCalculationsModel.Y3VYrs == 0 ? 0 : (double)(claimsCalculationsModel.Y3ClaimsOpen + claimsCalculationsModel.Y3ClaimsClosed) / claimsCalculationsModel.Y3VYrs * 100;
            claimsCalculationsModel.Y4AccFreq = 
                claimsCalculationsModel.Y4VYrs == 0 ? 0 : (double)(claimsCalculationsModel.Y4ClaimsOpen + claimsCalculationsModel.Y4ClaimsClosed) / claimsCalculationsModel.Y4VYrs * 100;
            claimsCalculationsModel.Y5AccFreq = 
                claimsCalculationsModel.Y5VYrs == 0 ? 0 : (double)(claimsCalculationsModel.Y5ClaimsOpen + claimsCalculationsModel.Y5ClaimsClosed) / claimsCalculationsModel.Y5VYrs * 100;

            claimsCalculationsModel.ThreeY_AccFreq = (claimsCalculationsModel.Y1AccFreq + claimsCalculationsModel.Y2AccFreq + claimsCalculationsModel.Y3AccFreq) / 3;
            claimsCalculationsModel.FiveY_AccFreq = (claimsCalculationsModel.Y1AccFreq + claimsCalculationsModel.Y2AccFreq + claimsCalculationsModel.Y3AccFreq + claimsCalculationsModel.Y4AccFreq + claimsCalculationsModel.Y5AccFreq) / 5;

            

            // Cost Per Claims is calculated by taking the Total Incurred Value and dividing it by the Total Number of Claims
            claimsCalculationsModel.Y1CPC = (double)claimsCalculationsModel.Y1Total / (claimsCalculationsModel.Y1ClaimsOpen + claimsCalculationsModel.Y1ClaimsClosed);

            claimsCalculationsModel.Y2CPC = (double)claimsCalculationsModel.Y2Total / (claimsCalculationsModel.Y2ClaimsOpen + claimsCalculationsModel.Y2ClaimsClosed);
            claimsCalculationsModel.Y3CPC = (double)claimsCalculationsModel.Y3Total / (claimsCalculationsModel.Y3ClaimsOpen + claimsCalculationsModel.Y3ClaimsClosed);
            claimsCalculationsModel.Y4CPC = (double)claimsCalculationsModel.Y4Total / (claimsCalculationsModel.Y4ClaimsOpen + claimsCalculationsModel.Y4ClaimsClosed);
            claimsCalculationsModel.Y5CPC = (double)claimsCalculationsModel.Y5Total / (claimsCalculationsModel.Y5ClaimsOpen + claimsCalculationsModel.Y5ClaimsClosed);

            claimsCalculationsModel.ThreeY_CPC = (double)claimsCalculationsModel.ThreeY_Total / (claimsCalculationsModel.ThreeY_ClaimsOpen + claimsCalculationsModel.ThreeY_ClaimsClo);
            claimsCalculationsModel.FiveY_CPC = (double)claimsCalculationsModel.FiveY_Total / (claimsCalculationsModel.FiveY_ClaimsOpen + claimsCalculationsModel.FiveY_ClaimsClo);

            // Cost Per Insurable Day is calculated by taking the Total Incurred Value and dividing it by the Total Risk (Insurable) Days.            
            claimsCalculationsModel.Y1CPID = (claimsCalculationsModel.Y1RentalDaysNonCOI == 0) ? 0 : claimsCalculationsModel.Y1Total / claimsCalculationsModel.Y1RentalDaysNonCOI;
            claimsCalculationsModel.Y2CPID = (claimsCalculationsModel.Y2RentalDaysNonCOI == 0) ? 0 : claimsCalculationsModel.Y2Total / claimsCalculationsModel.Y2RentalDaysNonCOI;
            claimsCalculationsModel.Y3CPID = (claimsCalculationsModel.Y3RentalDaysNonCOI == 0) ? 0 : claimsCalculationsModel.Y3Total / claimsCalculationsModel.Y3RentalDaysNonCOI;
            claimsCalculationsModel.Y4CPID = (claimsCalculationsModel.Y4RentalDaysNonCOI == 0) ? 0 : claimsCalculationsModel.Y4Total / claimsCalculationsModel.Y4RentalDaysNonCOI;
            claimsCalculationsModel.Y5CPID = (claimsCalculationsModel.Y5RentalDaysNonCOI == 0) ? 0 : claimsCalculationsModel.Y5Total / claimsCalculationsModel.Y5RentalDaysNonCOI;

            claimsCalculationsModel.ThreeY_CPID = (claimsCalculationsModel.ThreeY_RDaysNonCOI == 0) ? 0 : claimsCalculationsModel.ThreeY_Total / claimsCalculationsModel.ThreeY_RDaysNonCOI;
            claimsCalculationsModel.FiveY_CPID = 
                (claimsCalculationsModel.FiveY_RDaysNonCOI == 0) ? 0 : 
                claimsCalculationsModel.FiveY_Total / claimsCalculationsModel.FiveY_RDaysNonCOI;


            // Claims Cost by Turnover is calculated by taking the Total Incurred Value and dividing it by (Risk Turnover/1000). If we are not provided with turnover information from the client, this will be fixed at zero.
            claimsCalculationsModel.Y1CCTO = (claimsCalculationsModel.Y1TO_NonCOI == 0) ? 0 : claimsCalculationsModel.Y1Total / (claimsCalculationsModel.Y1TO_NonCOI / 1000);
            claimsCalculationsModel.Y2CCTO = (claimsCalculationsModel.Y2TO_NonCOI == 0) ? 0 : claimsCalculationsModel.Y2Total / (claimsCalculationsModel.Y2TO_NonCOI / 1000);
            claimsCalculationsModel.Y3CCTO = (claimsCalculationsModel.Y3TO_NonCOI == 0) ? 0 : claimsCalculationsModel.Y3Total / (claimsCalculationsModel.Y3TO_NonCOI / 1000);
            claimsCalculationsModel.Y4CCTO = (claimsCalculationsModel.Y4TO_NonCOI == 0) ? 0 : claimsCalculationsModel.Y4Total / (claimsCalculationsModel.Y4TO_NonCOI / 1000);
            claimsCalculationsModel.Y5CCTO = (claimsCalculationsModel.Y5TO_NonCOI == 0) ? 0 : claimsCalculationsModel.Y5Total / (claimsCalculationsModel.Y5TO_NonCOI / 1000);

            claimsCalculationsModel.ThreeY_CCTO = (claimsCalculationsModel.ThreeY_TO_NonCOI == 0) ? 0 : claimsCalculationsModel.ThreeY_Total / (claimsCalculationsModel.ThreeY_TO_NonCOI / 1000);
            claimsCalculationsModel.FiveY_CCTO = (claimsCalculationsModel.FiveY_TO_NonCOI == 0) ? 0 : claimsCalculationsModel.FiveY_Total / (claimsCalculationsModel.FiveY_TO_NonCOI / 1000);

            // Claims Cost per Vehicle Year AD is calculated by taking the (AD Paid + AD Outstanding) and dividing by the Vehicle Years.
            claimsCalculationsModel.Y1_CCPVY_AD = claimsCalculationsModel.Y1VYrs == 0 ? 0 : (claimsCalculationsModel.Y1ADPaid + claimsCalculationsModel.Y1ADOS) / claimsCalculationsModel.Y1VYrs;
            claimsCalculationsModel.Y2_CCPVY_AD = claimsCalculationsModel.Y2VYrs == 0 ? 0 : (claimsCalculationsModel.Y2ADPaid + claimsCalculationsModel.Y2ADOS) / claimsCalculationsModel.Y2VYrs;
            claimsCalculationsModel.Y3_CCPVY_AD = claimsCalculationsModel.Y3VYrs == 0 ? 0 : (claimsCalculationsModel.Y3ADPaid + claimsCalculationsModel.Y3ADOS) / claimsCalculationsModel.Y3VYrs;
            claimsCalculationsModel.Y4_CCPVY_AD = claimsCalculationsModel.Y4VYrs == 0 ? 0 : (claimsCalculationsModel.Y4ADPaid + claimsCalculationsModel.Y4ADOS) / claimsCalculationsModel.Y4VYrs;
            claimsCalculationsModel.Y5_CCPVY_AD = claimsCalculationsModel.Y5VYrs == 0 ? 0 : (claimsCalculationsModel.Y5ADPaid + claimsCalculationsModel.Y5ADOS) / claimsCalculationsModel.Y5VYrs;

            claimsCalculationsModel.ThreeY_CCPVY_AD = (claimsCalculationsModel.Y1_CCPVY_AD + claimsCalculationsModel.Y2_CCPVY_AD + claimsCalculationsModel.Y3_CCPVY_AD) / 3;
            claimsCalculationsModel.FiveY_CCPVY_AD = (claimsCalculationsModel.Y1_CCPVY_AD + claimsCalculationsModel.Y2_CCPVY_AD + claimsCalculationsModel.Y3_CCPVY_AD + claimsCalculationsModel.Y4_CCPVY_AD + claimsCalculationsModel.Y5_CCPVY_AD) / 5;

            // Claims Cost per Vehicle Year TP is calculated by taking the (TP Paid + TP Outstanding) and dividing by the Vehicle Years.
            claimsCalculationsModel.Y1_CCPVY_TP = 
                claimsCalculationsModel.Y1VYrs == 0 ? 0 : (claimsCalculationsModel.Y1TPPD + claimsCalculationsModel.Y1TPCH + claimsCalculationsModel.Y1TPPI + claimsCalculationsModel.Y1TPPDOS + claimsCalculationsModel.Y1TPCHOS + claimsCalculationsModel.Y1TPPIOS) / claimsCalculationsModel.Y1VYrs;
            claimsCalculationsModel.Y2_CCPVY_TP = 
                claimsCalculationsModel.Y2VYrs == 0 ? 0 : (claimsCalculationsModel.Y2TPPD + claimsCalculationsModel.Y2TPCH + claimsCalculationsModel.Y2TPPI + claimsCalculationsModel.Y2TPPDOS + claimsCalculationsModel.Y2TPCHOS + claimsCalculationsModel.Y2TPPIOS) / claimsCalculationsModel.Y2VYrs;
            claimsCalculationsModel.Y3_CCPVY_TP = 
                claimsCalculationsModel.Y3VYrs == 0 ? 0 : (claimsCalculationsModel.Y3TPPD + claimsCalculationsModel.Y3TPCH + claimsCalculationsModel.Y3TPPI + claimsCalculationsModel.Y3TPPDOS + claimsCalculationsModel.Y3TPCHOS + claimsCalculationsModel.Y3TPPIOS) / claimsCalculationsModel.Y3VYrs;
            claimsCalculationsModel.Y4_CCPVY_TP = claimsCalculationsModel.Y4VYrs == 0 ? 0 : (claimsCalculationsModel.Y4TPPD + claimsCalculationsModel.Y4TPCH + claimsCalculationsModel.Y4TPPI + claimsCalculationsModel.Y4TPPDOS + claimsCalculationsModel.Y4TPCHOS + claimsCalculationsModel.Y4TPPIOS) / claimsCalculationsModel.Y4VYrs;
            claimsCalculationsModel.Y5_CCPVY_TP = 
                claimsCalculationsModel.Y5VYrs == 0 ? 0 : (claimsCalculationsModel.Y5TPPD + claimsCalculationsModel.Y5TPCH + claimsCalculationsModel.Y5TPPI + claimsCalculationsModel.Y5TPPDOS + claimsCalculationsModel.Y5TPCHOS + claimsCalculationsModel.Y5TPPIOS) / claimsCalculationsModel.Y5VYrs;

            claimsCalculationsModel.ThreeY_CCPVY_TP = (claimsCalculationsModel.Y1_CCPVY_TP + claimsCalculationsModel.Y2_CCPVY_TP + claimsCalculationsModel.Y3_CCPVY_TP) / 3;
            claimsCalculationsModel.FiveY_CCPVY_TP = (claimsCalculationsModel.Y1_CCPVY_TP + claimsCalculationsModel.Y2_CCPVY_TP + claimsCalculationsModel.Y3_CCPVY_TP + claimsCalculationsModel.Y4_CCPVY_TP + claimsCalculationsModel.Y5_CCPVY_TP) / 5;

            // Total CCPVY is calculated by CCPVYAD + CCPVYTP
            claimsCalculationsModel.Y1_TotalCCPVY = (claimsCalculationsModel.Y1_CCPVY_AD + claimsCalculationsModel.Y1_CCPVY_TP);
            claimsCalculationsModel.Y2_TotalCCPVY = (claimsCalculationsModel.Y2_CCPVY_AD + claimsCalculationsModel.Y2_CCPVY_TP);
            claimsCalculationsModel.Y3_TotalCCPVY = (claimsCalculationsModel.Y3_CCPVY_AD + claimsCalculationsModel.Y3_CCPVY_TP);
            claimsCalculationsModel.Y4_TotalCCPVY = (claimsCalculationsModel.Y4_CCPVY_AD + claimsCalculationsModel.Y4_CCPVY_TP);
            claimsCalculationsModel.Y5_TotalCCPVY = (claimsCalculationsModel.Y5_CCPVY_AD + claimsCalculationsModel.Y5_CCPVY_TP);

            claimsCalculationsModel.ThreeY_TotalCCPVY = 
                (claimsCalculationsModel.Y1_TotalCCPVY + claimsCalculationsModel.Y2_TotalCCPVY + claimsCalculationsModel.Y3_TotalCCPVY) / 3;
            claimsCalculationsModel.FiveY_TotalCCPVY = 
                (claimsCalculationsModel.Y1_TotalCCPVY + claimsCalculationsModel.Y2_TotalCCPVY + claimsCalculationsModel.Y3_TotalCCPVY + claimsCalculationsModel.Y4_TotalCCPVY + claimsCalculationsModel.Y5_TotalCCPVY) / 5;



            // ------------------- INFLATED DATA TABLE --------------------------------

            // Inflated values - these were obtained from the Access file VBA
            var Y1InfRate = 1.06;
            var Y2InfRate = 1.079;
            var Y3InfRate = 1.09;
            var Y4InfRate = 1.076;
            var Y5InfRate = 1.076;

            // Inflated Total cost values
            var Y1TotalInf = (claimsCalculationsModel.Y1Total / (double)claimsCalculationsModel.InfMonth) * 12 * Y1InfRate;
            var Y2TotalInf = claimsCalculationsModel.Y2Total * Y1InfRate * Y2InfRate;
            var Y3TotalInf = claimsCalculationsModel.Y3Total * Y1InfRate * Y2InfRate * Y3InfRate;
            var Y4TotalInf = claimsCalculationsModel.Y4Total * Y1InfRate * Y2InfRate * Y3InfRate * Y4InfRate;
            var Y5TotalInf = claimsCalculationsModel.Y5Total * Y1InfRate * Y2InfRate * Y3InfRate * Y4InfRate * Y5InfRate;

            // Inflated AD
            var infY1AD = ((claimsCalculationsModel.Y1ADPaid + claimsCalculationsModel.Y1ADOS) / (double)claimsCalculationsModel.InfMonth) * 12 * Y1InfRate;
            var infY2AD = (claimsCalculationsModel.Y2ADPaid + claimsCalculationsModel.Y2ADOS) * Y1InfRate * Y2InfRate;
            var infY3AD = (claimsCalculationsModel.Y3ADPaid + claimsCalculationsModel.Y3ADOS) * Y1InfRate * Y2InfRate * Y3InfRate;
            var infY4AD = (claimsCalculationsModel.Y4ADPaid + claimsCalculationsModel.Y4ADOS) * Y1InfRate * Y2InfRate * Y3InfRate * Y4InfRate;
            var infY5AD = (claimsCalculationsModel.Y5ADPaid + claimsCalculationsModel.Y5ADOS) * Y1InfRate * Y2InfRate * Y3InfRate * Y4InfRate * Y5InfRate;

            //Inflated TP
            var infY1TP = ((claimsCalculationsModel.Y1TPPD + claimsCalculationsModel.Y1TPCH + claimsCalculationsModel.Y1TPPI + claimsCalculationsModel.Y1TPPDOS + claimsCalculationsModel.Y1TPCHOS + claimsCalculationsModel.Y1TPPIOS) / (double)claimsCalculationsModel.InfMonth) * 12 * Y1InfRate;
            var infY2TP = (claimsCalculationsModel.Y2TPPD + claimsCalculationsModel.Y2TPCH + claimsCalculationsModel.Y2TPPI + claimsCalculationsModel.Y2TPPDOS + claimsCalculationsModel.Y2TPCHOS + claimsCalculationsModel.Y2TPPIOS) * Y1InfRate * Y2InfRate;
            var infY3TP = (claimsCalculationsModel.Y3TPPD + claimsCalculationsModel.Y3TPCH + claimsCalculationsModel.Y3TPPI + claimsCalculationsModel.Y3TPPDOS + claimsCalculationsModel.Y3TPCHOS + claimsCalculationsModel.Y3TPPIOS) * Y1InfRate * Y2InfRate * Y3InfRate;
            var infY4TP = (claimsCalculationsModel.Y4TPPD + claimsCalculationsModel.Y4TPCH + claimsCalculationsModel.Y4TPPI + claimsCalculationsModel.Y4TPPDOS + claimsCalculationsModel.Y4TPCHOS + claimsCalculationsModel.Y4TPPIOS) * Y1InfRate * Y2InfRate * Y3InfRate * Y4InfRate;
            var infY5TP = (claimsCalculationsModel.Y5TPPD + claimsCalculationsModel.Y5TPCH + claimsCalculationsModel.Y5TPPI + claimsCalculationsModel.Y5TPPDOS + claimsCalculationsModel.Y5TPCHOS + claimsCalculationsModel.Y5TPPIOS) * Y1InfRate * Y2InfRate * Y3InfRate * Y4InfRate * Y5InfRate;

            // Inflated CPC 
            claimsCalculationsModel.Y1CPC_Inf = Y1TotalInf / ((double)claimsCalculationsModel.Y1ClaimsOpen + (double)claimsCalculationsModel.Y1ClaimsClosed);

            claimsCalculationsModel.Y2CPC_Inf = Y2TotalInf / ((double)claimsCalculationsModel.Y2ClaimsOpen + (double)claimsCalculationsModel.Y2ClaimsClosed);
            claimsCalculationsModel.Y3CPC_Inf = Y3TotalInf / ((double)claimsCalculationsModel.Y3ClaimsOpen + (double)claimsCalculationsModel.Y3ClaimsClosed);
            claimsCalculationsModel.Y4CPC_Inf = Y4TotalInf / ((double)claimsCalculationsModel.Y4ClaimsOpen + (double)claimsCalculationsModel.Y4ClaimsClosed);
            claimsCalculationsModel.Y5CPC_Inf = Y5TotalInf / ((double)claimsCalculationsModel.Y5ClaimsOpen + (double)claimsCalculationsModel.Y5ClaimsClosed);

            claimsCalculationsModel.ThreeY_CPC_Inf = (claimsCalculationsModel.Y1CPC_Inf + claimsCalculationsModel.Y2CPC_Inf + claimsCalculationsModel.Y3CPC_Inf) / 3;
            claimsCalculationsModel.FiveY_CPC_Inf = (claimsCalculationsModel.Y1CPC_Inf + claimsCalculationsModel.Y2CPC_Inf + claimsCalculationsModel.Y3CPC_Inf + claimsCalculationsModel.Y4CPC_Inf + claimsCalculationsModel.Y5CPC_Inf) / 5;

            var InfCPC3Year = claimsCalculationsModel.ThreeY_CPC_Inf;

            // Inflated CPID
            claimsCalculationsModel.Y1CPID_Inf = (claimsCalculationsModel.Y1RentalDaysNonCOI == 0) ? 0 : Y1TotalInf / claimsCalculationsModel.Y1RentalDaysNonCOI;
            claimsCalculationsModel.Y2CPID_Inf = (claimsCalculationsModel.Y2RentalDaysNonCOI == 0) ? 0 : Y2TotalInf / claimsCalculationsModel.Y2RentalDaysNonCOI;
            claimsCalculationsModel.Y3CPID_Inf = (claimsCalculationsModel.Y3RentalDaysNonCOI == 0) ? 0 : Y3TotalInf / claimsCalculationsModel.Y3RentalDaysNonCOI;
            claimsCalculationsModel.Y4CPID_Inf = (claimsCalculationsModel.Y4RentalDaysNonCOI == 0) ? 0 : Y4TotalInf / claimsCalculationsModel.Y4RentalDaysNonCOI;
            claimsCalculationsModel.Y5CPID_Inf = (claimsCalculationsModel.Y5RentalDaysNonCOI == 0) ? 0 : Y5TotalInf / claimsCalculationsModel.Y5RentalDaysNonCOI;

            claimsCalculationsModel.ThreeY_CPID_Inf = (claimsCalculationsModel.Y1CPID_Inf + claimsCalculationsModel.Y2CPID_Inf + claimsCalculationsModel.Y3CPID_Inf) / 3;
            claimsCalculationsModel.FiveY_CPID_Inf = (claimsCalculationsModel.Y1CPID_Inf + claimsCalculationsModel.Y2CPID_Inf + claimsCalculationsModel.Y3CPID_Inf + claimsCalculationsModel.Y4CPID_Inf + claimsCalculationsModel.Y5CPID_Inf) / 5;

            var InfCPID3Year = claimsCalculationsModel.ThreeY_CPID_Inf;
            var InfCPID5Year = claimsCalculationsModel.FiveY_CPID_Inf;

            // Inflated CCTO 
            claimsCalculationsModel.Y1CCTO_Inf = (claimsCalculationsModel.Y1TO_NonCOI == 0) ? 0 : (infY1AD + infY1TP) / (claimsCalculationsModel.Y1TO_NonCOI / 1000);
            claimsCalculationsModel.Y2CCTO_Inf = (claimsCalculationsModel.Y2TO_NonCOI == 0) ? 0 : (infY2AD + infY2TP) / (claimsCalculationsModel.Y2TO_NonCOI / 1000);
            claimsCalculationsModel.Y3CCTO_Inf = (claimsCalculationsModel.Y3TO_NonCOI == 0) ? 0 : (infY3AD + infY3TP) / (claimsCalculationsModel.Y3TO_NonCOI / 1000);
            claimsCalculationsModel.Y4CCTO_Inf = (claimsCalculationsModel.Y4TO_NonCOI == 0) ? 0 : (infY4AD + infY4TP) / (claimsCalculationsModel.Y4TO_NonCOI / 1000);
            claimsCalculationsModel.Y5CCTO_Inf = (claimsCalculationsModel.Y5TO_NonCOI == 0) ? 0 : (infY5AD + infY5TP) / (claimsCalculationsModel.Y5TO_NonCOI / 1000);

            claimsCalculationsModel.ThreeY_CCTO_Inf = (claimsCalculationsModel.Y1CCTO_Inf + claimsCalculationsModel.Y2CCTO_Inf + claimsCalculationsModel.Y3CCTO_Inf) / 3;
            claimsCalculationsModel.FiveY_CCTO_Inf = (claimsCalculationsModel.Y1CCTO_Inf + claimsCalculationsModel.Y2CCTO_Inf + claimsCalculationsModel.Y3CCTO_Inf + claimsCalculationsModel.Y4CCTO_Inf + claimsCalculationsModel.Y5CCTO_Inf) / 5;

            // Inflated CCPVY AD
            claimsCalculationsModel.Y1_CCPVYAD_Inf = claimsCalculationsModel.Y1VYrs == 0 ? 0 : infY1AD / (double)claimsCalculationsModel.Y1VYrs;
            claimsCalculationsModel.Y2_CCPVYAD_Inf = claimsCalculationsModel.Y2VYrs == 0 ? 0 : infY2AD / (double)claimsCalculationsModel.Y2VYrs;
            claimsCalculationsModel.Y3_CCPVYAD_Inf = claimsCalculationsModel.Y3VYrs == 0 ? 0 : infY3AD / (double)claimsCalculationsModel.Y3VYrs;
            claimsCalculationsModel.Y4_CCPVYAD_Inf = claimsCalculationsModel.Y4VYrs == 0 ? 0 : infY4AD / (double)claimsCalculationsModel.Y4VYrs;
            claimsCalculationsModel.Y5_CCPVYAD_Inf = claimsCalculationsModel.Y5VYrs == 0 ? 0 : infY5AD / (double)claimsCalculationsModel.Y5VYrs;

            claimsCalculationsModel.ThreeY_CCPVYAD_Inf = (claimsCalculationsModel.Y1_CCPVYAD_Inf + claimsCalculationsModel.Y2_CCPVYAD_Inf + claimsCalculationsModel.Y3_CCPVYAD_Inf) / 3;
            claimsCalculationsModel.FiveY_CCPVYAD_Inf = (claimsCalculationsModel.Y1_CCPVYAD_Inf + claimsCalculationsModel.Y2_CCPVYAD_Inf + claimsCalculationsModel.Y3_CCPVYAD_Inf + claimsCalculationsModel.Y4_CCPVYAD_Inf + claimsCalculationsModel.Y5_CCPVYAD_Inf) / 5;

            // Inflated CCPVY TP
            claimsCalculationsModel.Y1_CCPVYTP_Inf = claimsCalculationsModel.Y1VYrs == 0 ? 0 : infY1TP / claimsCalculationsModel.Y1VYrs;
            claimsCalculationsModel.Y2_CCPVYTP_Inf = claimsCalculationsModel.Y2VYrs == 0 ? 0 : infY2TP / claimsCalculationsModel.Y2VYrs;
            claimsCalculationsModel.Y3_CCPVYTP_Inf = claimsCalculationsModel.Y3VYrs == 0 ? 0 : infY3TP / claimsCalculationsModel.Y3VYrs;
            claimsCalculationsModel.Y4_CCPVYTP_Inf = claimsCalculationsModel.Y4VYrs == 0 ? 0 : infY4TP / claimsCalculationsModel.Y4VYrs;
            claimsCalculationsModel.Y5_CCPVYTP_Inf = claimsCalculationsModel.Y5VYrs == 0 ? 0 : infY5TP / claimsCalculationsModel.Y5VYrs;

            claimsCalculationsModel.ThreeY_CCPVYTP_Inf = (claimsCalculationsModel.Y1_CCPVYTP_Inf + claimsCalculationsModel.Y2_CCPVYTP_Inf + claimsCalculationsModel.Y3_CCPVYTP_Inf) / 3;
            claimsCalculationsModel.FiveY_CCPVYTP_Inf = (claimsCalculationsModel.Y1_CCPVYTP_Inf + claimsCalculationsModel.Y2_CCPVYTP_Inf + claimsCalculationsModel.Y3_CCPVYTP_Inf + claimsCalculationsModel.Y4_CCPVYTP_Inf + claimsCalculationsModel.Y5_CCPVYTP_Inf) / 5;

            // Total Inflated CCPVY
            claimsCalculationsModel.Y1_TotalCCPVY_Inf = claimsCalculationsModel.Y1_CCPVYAD_Inf + claimsCalculationsModel.Y1_CCPVYTP_Inf;
            claimsCalculationsModel.Y2_TotalCCPVY_Inf = claimsCalculationsModel.Y2_CCPVYAD_Inf + claimsCalculationsModel.Y2_CCPVYTP_Inf;
            claimsCalculationsModel.Y3_TotalCCPVY_Inf = claimsCalculationsModel.Y3_CCPVYAD_Inf + claimsCalculationsModel.Y3_CCPVYTP_Inf;
            claimsCalculationsModel.Y4_TotalCCPVY_Inf = claimsCalculationsModel.Y4_CCPVYAD_Inf + claimsCalculationsModel.Y4_CCPVYTP_Inf;
            claimsCalculationsModel.Y5_TotalCCPVY_Inf = claimsCalculationsModel.Y5_CCPVYAD_Inf + claimsCalculationsModel.Y5_CCPVYTP_Inf;

            claimsCalculationsModel.ThreeY_TotalCCPVY_Inf = (claimsCalculationsModel.Y1_TotalCCPVY_Inf + claimsCalculationsModel.Y2_TotalCCPVY_Inf + claimsCalculationsModel.Y3_TotalCCPVY_Inf) / 3;
            claimsCalculationsModel.FiveY_TotalCCPVY_Inf = (claimsCalculationsModel.Y1_TotalCCPVY_Inf + claimsCalculationsModel.Y2_TotalCCPVY_Inf + claimsCalculationsModel.Y3_TotalCCPVY_Inf + claimsCalculationsModel.Y4_TotalCCPVY_Inf + claimsCalculationsModel.Y5_TotalCCPVY_Inf) / 5;

            var InfCCPVYTotal3Year = claimsCalculationsModel.ThreeY_TotalCCPVY_Inf;
            var InfCCPVYTotal5Year = claimsCalculationsModel.FiveY_TotalCCPVY_Inf;



            // Populate the CalculationsYearsDataInflated for the view model
            claimsCalculationsModel.CalculationsYearsInflated = new List<CalculationsYearsDataInflated>
            {
                new CalculationsYearsDataInflated
                {
                    Label = "CPC",
                    Year1 = claimsCalculationsModel.Y1CPC_Inf.ToString("F2"),
                    Year2 = claimsCalculationsModel.Y2CPC_Inf.ToString("F2"),
                    Year3 = claimsCalculationsModel.Y3CPC_Inf.ToString("F2"),
                    Year4 = claimsCalculationsModel.Y4CPC_Inf.ToString("F2"),
                    Year5 = claimsCalculationsModel.Y5CPC_Inf.ToString("F2"),
                    ThreeYearAverage = claimsCalculationsModel.ThreeY_CPC_Inf.ToString("F2"),
                    FiveYearAverage = claimsCalculationsModel.FiveY_CPC_Inf.ToString("F2")
                },
                new CalculationsYearsDataInflated
                {
                    Label = "CPID",
                    Year1 = claimsCalculationsModel.Y1CPID_Inf.ToString("F2"),
                    Year2 = claimsCalculationsModel.Y2CPID_Inf.ToString("F2"),
                    Year3 = claimsCalculationsModel.Y3CPID_Inf.ToString("F2"),
                    Year4 = claimsCalculationsModel.Y4CPID_Inf.ToString("F2"),
                    Year5 = claimsCalculationsModel.Y5CPID_Inf.ToString("F2"),
                    ThreeYearAverage = claimsCalculationsModel.ThreeY_CPID_Inf.ToString("F2"),
                    FiveYearAverage = claimsCalculationsModel.FiveY_CPID_Inf.ToString("F2"),
                },
                new CalculationsYearsDataInflated
                {
                    Label = "CCTO",
                    Year1 = claimsCalculationsModel.Y1CCTO_Inf.ToString("F2"),
                    Year2 = claimsCalculationsModel.Y2CCTO_Inf.ToString("F2"),
                    Year3 = claimsCalculationsModel.Y3CCTO_Inf.ToString("F2"),
                    Year4 = claimsCalculationsModel.Y4CCTO_Inf.ToString("F2"),
                    Year5 = claimsCalculationsModel.Y5CCTO_Inf.ToString("F2"),
                    ThreeYearAverage = claimsCalculationsModel.ThreeY_CCTO_Inf.ToString("F2"),
                    FiveYearAverage = claimsCalculationsModel.FiveY_CCTO_Inf.ToString("F2")
                },
                new CalculationsYearsDataInflated
                {
                    Label = "CCPVY AD",
                    Year1 = claimsCalculationsModel.Y1_CCPVYAD_Inf.ToString("F2"),
                    Year2 = claimsCalculationsModel.Y2_CCPVYAD_Inf.ToString("F2"),
                    Year3 = claimsCalculationsModel.Y3_CCPVYAD_Inf.ToString("F2"),
                    Year4 = claimsCalculationsModel.Y4_CCPVYAD_Inf.ToString("F2"),
                    Year5 = claimsCalculationsModel.Y5_CCPVYAD_Inf.ToString("F2"),
                    ThreeYearAverage = claimsCalculationsModel.ThreeY_CCPVYAD_Inf.ToString("F2"),
                    FiveYearAverage = claimsCalculationsModel.FiveY_CCPVYAD_Inf.ToString("F2")
                },
                new CalculationsYearsDataInflated
                {
                    Label = "CCPVY TP",
                    Year1 = claimsCalculationsModel.Y1_CCPVYTP_Inf.ToString("F2"),
                    Year2 = claimsCalculationsModel.Y2_CCPVYTP_Inf.ToString("F2"),
                    Year3 = claimsCalculationsModel.Y3_CCPVYTP_Inf.ToString("F2"),
                    Year4 = claimsCalculationsModel.Y4_CCPVYTP_Inf.ToString("F2"),
                    Year5 = claimsCalculationsModel.Y5_CCPVYTP_Inf.ToString("F2"),
                    ThreeYearAverage = claimsCalculationsModel.ThreeY_CCPVYTP_Inf.ToString("F2"),
                    FiveYearAverage = claimsCalculationsModel.FiveY_CCPVYTP_Inf.ToString("F2")
                },
                new CalculationsYearsDataInflated
                {
                    Label = "Total CCPVY",
                    Year1 = claimsCalculationsModel.Y1_TotalCCPVY_Inf.ToString("F2"),
                    Year2 = claimsCalculationsModel.Y2_TotalCCPVY_Inf.ToString("F2"),
                    Year3 = claimsCalculationsModel.Y3_TotalCCPVY_Inf.ToString("F2"),
                    Year4 = claimsCalculationsModel.Y4_TotalCCPVY_Inf.ToString("F2"),
                    Year5 = claimsCalculationsModel.Y5_TotalCCPVY_Inf.ToString("F2"),
                    ThreeYearAverage = claimsCalculationsModel.ThreeY_TotalCCPVY_Inf.ToString("F2"),
                    FiveYearAverage = claimsCalculationsModel.FiveY_TotalCCPVY_Inf.ToString("F2")
                }
            };



            // ----------------------- ADDITIONAL STATS TABLE ----------------------------

            var carNums = staticClientData_fromTable.CarNums;
            var vanNums = staticClientData_fromTable.VanNums;
            var minibusNums = staticClientData_fromTable.MinibusNums;
            var hgvNums = staticClientData_fromTable.HGVNums;
            var totalVehicleNums = carNums + vanNums + minibusNums + hgvNums;

            var carExposure = staticClientData_fromTable.CarExposure;
            var vanExposure = staticClientData_fromTable.VanExposure;
            var minibusExposure = staticClientData_fromTable.MinibusExposure;
            var hgvExposure = staticClientData_fromTable.HGVExposure;

            var fcDaysCOI = staticClientData_fromTable.ForecastDaysCOI;
            var fcDaysNonCOI = staticClientData_fromTable.ForecastDaysNonCOI;

            // Total Exposure
            claimsCalculationsModel.TotalExposure = (
                carNums * carExposure +
                vanNums * vanExposure +
                minibusNums * minibusExposure +
                hgvNums * hgvExposure);

            // Total Non COI Exposure
            claimsCalculationsModel.TotalNonCOIExposure = fcDaysCOI == 0 && fcDaysNonCOI == 0 ? 0 :
                (double)fcDaysNonCOI / (fcDaysCOI + fcDaysNonCOI) * claimsCalculationsModel.TotalExposure;

            // CC 1000 Days (Client) and (Book)
            claimsCalculationsModel.CC1000DaysClient =
                claimsCalculationsModel.FiveY_Total / ((double)claimsCalculationsModel.FiveY_RDaysNonCOI / 1000);

            // hard coded value
            claimsCalculationsModel.CC1000DaysBook = 2530.01;

            // Variance
            claimsCalculationsModel.Variance = claimsCalculationsModel.CC1000DaysClient / claimsCalculationsModel.CC1000DaysBook * 100;

            // New Exposure
            claimsCalculationsModel.NewExposure = claimsCalculationsModel.TotalNonCOIExposure * (claimsCalculationsModel.Variance / 100);



            // Populate the CalculationsYearsData for the view model
            claimsCalculationsModel.CalculationsYears = new List<CalculationsYearsData>
            {
                new CalculationsYearsData
                {
                    Label = "Acc Freq",
                    Year1 = claimsCalculationsModel.Y1AccFreq.ToString("F2"),
                    Year2 = claimsCalculationsModel.Y2AccFreq.ToString("F2"),
                    Year3 = claimsCalculationsModel.Y3AccFreq.ToString("F2"),
                    Year4 = claimsCalculationsModel.Y4AccFreq.ToString("F2"),
                    Year5 = claimsCalculationsModel.Y5AccFreq.ToString("F2"),
                    ThreeYearTotal = claimsCalculationsModel.ThreeY_AccFreq.ToString("F2"),
                    FiveYearTotal = claimsCalculationsModel.FiveY_AccFreq.ToString("F2"),
                },
                new CalculationsYearsData
                {
                    Label = "CPC",
                    Year1 = claimsCalculationsModel.Y1CPC.ToString("F2"),
                    Year2 = claimsCalculationsModel.Y2CPC.ToString("F2"),
                    Year3 = claimsCalculationsModel.Y3CPC.ToString("F2"),
                    Year4 = claimsCalculationsModel.Y4CPC.ToString("F2"),
                    Year5 = claimsCalculationsModel.Y5CPC.ToString("F2"),
                    ThreeYearTotal = claimsCalculationsModel.ThreeY_CPC.ToString("F2"),
                    FiveYearTotal = claimsCalculationsModel.FiveY_CPC.ToString("F2"),
                },
                new CalculationsYearsData
                {
                    Label = "CPID",
                    Year1 = claimsCalculationsModel.Y1CPID.ToString("F2"),
                    Year2 = claimsCalculationsModel.Y2CPID.ToString("F2"),
                    Year3 = claimsCalculationsModel.Y3CPID.ToString("F2"),
                    Year4 = claimsCalculationsModel.Y4CPID.ToString("F2"),
                    Year5 = claimsCalculationsModel.Y5CPID.ToString("F2"),
                    ThreeYearTotal = claimsCalculationsModel.ThreeY_CPID.ToString("F2"),
                    FiveYearTotal = claimsCalculationsModel.FiveY_CPID.ToString("F2"),
                },
                new CalculationsYearsData
                {
                    Label = "CCTO",
                    Year1 = claimsCalculationsModel.Y1CCTO.ToString("F2"),
                    Year2 = claimsCalculationsModel.Y2CCTO.ToString("F2"),
                    Year3 = claimsCalculationsModel.Y3CCTO.ToString("F2"),
                    Year4 = claimsCalculationsModel.Y4CCTO.ToString("F2"),
                    Year5 = claimsCalculationsModel.Y5CCTO.ToString("F2"),
                    ThreeYearTotal = claimsCalculationsModel.ThreeY_CCTO.ToString("F2"),
                    FiveYearTotal = claimsCalculationsModel.FiveY_CCTO.ToString("F2")
                },
                new CalculationsYearsData
                {
                    Label = "CCPVY AD",
                    Year1 = claimsCalculationsModel.Y1_CCPVY_AD.ToString("F2"),
                    Year2 = claimsCalculationsModel.Y2_CCPVY_AD.ToString("F2"),
                    Year3 = claimsCalculationsModel.Y3_CCPVY_AD.ToString("F2"),
                    Year4 = claimsCalculationsModel.Y4_CCPVY_AD.ToString("F2"),
                    Year5 = claimsCalculationsModel.Y5_CCPVY_AD.ToString("F2"),
                    ThreeYearTotal = claimsCalculationsModel.ThreeY_CCPVY_AD.ToString("F2"),
                    FiveYearTotal = claimsCalculationsModel.FiveY_CCPVY_AD.ToString("F2")
                },
                new CalculationsYearsData
                {
                    Label = "CCPVY TP",
                    Year1 = claimsCalculationsModel.Y1_CCPVY_TP.ToString("F2"),
                    Year2 = claimsCalculationsModel.Y2_CCPVY_TP.ToString("F2"),
                    Year3 = claimsCalculationsModel.Y3_CCPVY_TP.ToString("F2"),
                    Year4 = claimsCalculationsModel.Y4_CCPVY_TP.ToString("F2"),
                    Year5 = claimsCalculationsModel.Y5_CCPVY_TP.ToString("F2"),
                    ThreeYearTotal = claimsCalculationsModel.ThreeY_CCPVY_TP.ToString("F2"),
                    FiveYearTotal = claimsCalculationsModel.FiveY_CCPVY_TP.ToString("F2"),
                },
                new CalculationsYearsData
                {
                    Label = "Total CCPVY",
                    Year1 = claimsCalculationsModel.Y1_TotalCCPVY.ToString("F2"),
                    Year2 = claimsCalculationsModel.Y2_TotalCCPVY.ToString("F2"),
                    Year3 = claimsCalculationsModel.Y3_TotalCCPVY.ToString("F2"),
                    Year4 = claimsCalculationsModel.Y4_TotalCCPVY.ToString("F2"),
                    Year5 = claimsCalculationsModel.Y5_TotalCCPVY.ToString("F2"),
                    ThreeYearTotal = claimsCalculationsModel.ThreeY_TotalCCPVY.ToString("F2"),
                    FiveYearTotal = claimsCalculationsModel.FiveY_TotalCCPVY.ToString("F2")
                }
            };




            // ---------------- TECHNICAL PRICE POINTS TABLE ---------------------------

            // Projected Claims

            var fcTurnoverCOI = staticClientData_fromTable.ForecastTO_COI;
            var fcTurnoverNonCOI = staticClientData_fromTable.ForecastTO_NonCOI;

            if (fcTurnoverNonCOI == 0 && fcDaysNonCOI == 0)
            {
                claimsCalculationsModel.ProjectedClaimsTech = (claimsCalculationsModel.ProjYear == 3)
                    ?
                    (totalVehicleNums * (claimsCalculationsModel.ThreeY_AccFreq / 100))
                    :
                    (totalVehicleNums * (claimsCalculationsModel.ThreeY_AccFreq / 100));
            }
            else if (fcTurnoverNonCOI == 0)
            {
                claimsCalculationsModel.ProjectedClaimsTech = (claimsCalculationsModel.ProjYear == 3)
                    ?
                    ((double)(claimsCalculationsModel.ThreeY_ClaimsOpen + claimsCalculationsModel.ThreeY_ClaimsClo) / claimsCalculationsModel.ThreeY_RDaysNonCOI) * fcDaysNonCOI
                    :
                    ((double)(claimsCalculationsModel.FiveY_ClaimsOpen + claimsCalculationsModel.FiveY_ClaimsClo) / claimsCalculationsModel.FiveY_RDaysNonCOI) * fcDaysNonCOI;

            }
            else
            {
                claimsCalculationsModel.ProjectedClaimsTech = (fcTurnoverNonCOI >= 1000000)
                    ?
                    (fcTurnoverNonCOI / 10000) * ((claimsCalculationsModel.ProjYear == 3) ? (claimsCalculationsModel.ThreeY_AccFreq / 100) : (claimsCalculationsModel.FiveY_AccFreq / 100))
                    :
                    (fcTurnoverNonCOI / 10000) * ((claimsCalculationsModel.ProjYear == 3) ? (claimsCalculationsModel.ThreeY_AccFreq / 100) : (claimsCalculationsModel.FiveY_AccFreq / 100));
            }

            // Projected CCPVY
            claimsCalculationsModel.ProjectedCCPVYTech = (claimsCalculationsModel.ProjYear == 3) ? claimsCalculationsModel.ThreeY_TotalCCPVY_Inf : claimsCalculationsModel.FiveY_TotalCCPVY_Inf;

            // Projected CPIRD
            claimsCalculationsModel.ProjectedCPIRDTech = (claimsCalculationsModel.ProjYear == 3) ? claimsCalculationsModel.ThreeY_CPID_Inf : claimsCalculationsModel.FiveY_CPID_Inf;

            // Projected IBNR
            claimsCalculationsModel.ProjectedIBNRTech = claimsCalculationsModel.ProjectedClaimsTech / 10;

            // Projected Exposure
            claimsCalculationsModel.ProjectedExposureTech = staticClientData_fromTable.ExpoPercentage;




            // ---------------- ULTIMATE COSTS TABLE --------------------------------

            /*
            Claims amount: 47.1%
            LL Fund = 13.1%
            Reinsurance costs: 17.4%
            Claims Handling Fee: 3.6%
            Levies: 4.7%
            Expenses: 8.5%
            Profit: 5.6%
            Net Premium = 100%
            */

            var carLLL = staticClientData_fromTable.ClientCarLLL;
            var vanLLL = staticClientData_fromTable.ClientVanLLL;
            var minibusLLL = staticClientData_fromTable.ClientMBusLLL;
            var hgvLLL = staticClientData_fromTable.ClientHGVLLL;

            var totalLLL = carNums * carLLL + vanNums * vanLLL + minibusNums * minibusLLL + hgvNums * hgvLLL;


            // Contingent Fee
            /*if (claimsCalculationsModel.ChargeCOIFeeValue == 2)
            {
                claimsCalculationsModel.COI_Contingent = 0;
            }
            else if (fcTurnoverCOI == 0)
            {
                claimsCalculationsModel.COI_Contingent = 0;
            }
            else if (fcTurnoverCOI / (fcTurnoverCOI + fcTurnoverNonCOI) > 0.5)
            {
                claimsCalculationsModel.COI_Contingent = (fcTurnoverCOI / 100) * 1;
            }
            else
            {
                claimsCalculationsModel.COI_Contingent = (fcTurnoverCOI / 100) * 0.5;
            }
            // Projected LL Fund
            if (fcDaysNonCOI == 0)
            {
                claimsCalculationsModel.ProjLLFund = totalLLL;
            }
            else
            {
                claimsCalculationsModel.ProjLLFund = totalLLL * ((double)fcDaysNonCOI / (fcDaysNonCOI + fcDaysCOI));
            }*/


            // Projected Claims Amount
            if (claimsCalculationsModel.PriceByValue == 1)  // experience
            {
                if (claimsCalculationsModel.PricingMetricValue == 1) // CCPVY
                {
                    if (fcTurnoverNonCOI == 0 && fcDaysNonCOI == 0)
                    {
                        claimsCalculationsModel.ProjClaimsAmount = (double)totalVehicleNums * claimsCalculationsModel.ProjectedCCPVYTech;
                        claimsCalculationsModel.ProjLLFund = claimsCalculationsModel.ProjClaimsAmount * (13.1 / 47.1);
                        claimsCalculationsModel.ReinsuranceCosts = claimsCalculationsModel.ProjClaimsAmount * (17.4 / 47.1);
                        claimsCalculationsModel.ProjClaimsHandlingFee = claimsCalculationsModel.ProjClaimsAmount * (3.6 / 47.1);
                        claimsCalculationsModel.Levies = claimsCalculationsModel.ProjClaimsAmount * (4.7 / 47.1);
                        claimsCalculationsModel.PretiumExpenses = claimsCalculationsModel.ProjClaimsAmount * (8.5 / 47.1);
                        claimsCalculationsModel.Profit = claimsCalculationsModel.ProjClaimsAmount * (5.6 / 47.1);
                    }
                    else if (fcTurnoverNonCOI == 0)
                    {
                        claimsCalculationsModel.ProjClaimsAmount = ((double)fcDaysNonCOI / 365) * claimsCalculationsModel.ProjectedCCPVYTech;
                        claimsCalculationsModel.ProjLLFund = claimsCalculationsModel.ProjClaimsAmount * (13.1 / 47.1);
                        claimsCalculationsModel.ReinsuranceCosts = claimsCalculationsModel.ProjClaimsAmount * (17.4 / 47.1);
                        claimsCalculationsModel.ProjClaimsHandlingFee = claimsCalculationsModel.ProjClaimsAmount * (3.6 / 47.1);
                        claimsCalculationsModel.Levies = claimsCalculationsModel.ProjClaimsAmount * (4.7 / 47.1);
                        claimsCalculationsModel.PretiumExpenses = claimsCalculationsModel.ProjClaimsAmount * (8.5 / 47.1);
                        claimsCalculationsModel.Profit = claimsCalculationsModel.ProjClaimsAmount * (5.6 / 47.1);
                    }
                    else
                    {
                        claimsCalculationsModel.ProjClaimsAmount = ((double)fcTurnoverNonCOI / 1000) * ((claimsCalculationsModel.ProjYear == 3) ? claimsCalculationsModel.ThreeY_CCTO_Inf : claimsCalculationsModel.FiveY_CCTO_Inf);
                        claimsCalculationsModel.ProjLLFund = claimsCalculationsModel.ProjClaimsAmount * (13.1 / 47.1);
                        claimsCalculationsModel.ReinsuranceCosts = claimsCalculationsModel.ProjClaimsAmount * (17.4 / 47.1);
                        claimsCalculationsModel.ProjClaimsHandlingFee = claimsCalculationsModel.ProjClaimsAmount * (3.6 / 47.1);
                        claimsCalculationsModel.Levies = claimsCalculationsModel.ProjClaimsAmount * (4.7 / 47.1);
                        claimsCalculationsModel.PretiumExpenses = claimsCalculationsModel.ProjClaimsAmount * (8.5 / 47.1);
                        claimsCalculationsModel.Profit = claimsCalculationsModel.ProjClaimsAmount * (5.6 / 47.1);
                    }
                }
                else if (claimsCalculationsModel.PricingMetricValue == 2) // claims
                {
                    claimsCalculationsModel.ProjClaimsAmount = claimsCalculationsModel.ProjectedClaimsTech * ((claimsCalculationsModel.ProjYear == 3) ? claimsCalculationsModel.ThreeY_CPC_Inf : claimsCalculationsModel.FiveY_CPC_Inf);
                    claimsCalculationsModel.ProjLLFund = claimsCalculationsModel.ProjClaimsAmount * (13.1 / 47.1);
                    claimsCalculationsModel.ReinsuranceCosts = claimsCalculationsModel.ProjClaimsAmount * (17.4 / 47.1);
                    claimsCalculationsModel.ProjClaimsHandlingFee = claimsCalculationsModel.ProjClaimsAmount * (3.6 / 47.1);
                    claimsCalculationsModel.Levies = claimsCalculationsModel.ProjClaimsAmount * (4.7 / 47.1);
                    claimsCalculationsModel.PretiumExpenses = claimsCalculationsModel.ProjClaimsAmount * (8.5 / 47.1);
                    claimsCalculationsModel.Profit = claimsCalculationsModel.ProjClaimsAmount * (5.6 / 47.1);
                }
                else // days
                {
                    claimsCalculationsModel.ProjClaimsAmount = claimsCalculationsModel.ProjectedCPIRDTech * (double)fcDaysNonCOI;
                    claimsCalculationsModel.ProjLLFund = claimsCalculationsModel.ProjClaimsAmount * (13.1 / 47.1);
                    claimsCalculationsModel.ReinsuranceCosts = claimsCalculationsModel.ProjClaimsAmount * (17.4 / 47.1);
                    claimsCalculationsModel.ProjClaimsHandlingFee = claimsCalculationsModel.ProjClaimsAmount * (3.6 / 47.1);
                    claimsCalculationsModel.Levies = claimsCalculationsModel.ProjClaimsAmount * (4.7 / 47.1);
                    claimsCalculationsModel.PretiumExpenses = claimsCalculationsModel.ProjClaimsAmount * (8.5 / 47.1);
                    claimsCalculationsModel.Profit = claimsCalculationsModel.ProjClaimsAmount * (5.6 / 47.1);
                }

                // coi cointingent here
                if (claimsCalculationsModel.ChargeCOIFeeValue == 2)
                {
                    claimsCalculationsModel.COI_Contingent = 0;
                }
                else if (fcTurnoverCOI == 0)
                {
                    claimsCalculationsModel.COI_Contingent = 0;
                }
                else if ((fcTurnoverCOI / (fcTurnoverCOI + fcTurnoverNonCOI) > 0.5))
                {
                    claimsCalculationsModel.COI_Contingent = fcTurnoverCOI / 100 * 1;
                }
                else
                {
                    claimsCalculationsModel.COI_Contingent = fcTurnoverCOI / 100 * 0.5;
                }

                claimsCalculationsModel.COI_ContingentPerDay = claimsCalculationsModel.COI_Contingent / (fcDaysCOI);


                claimsCalculationsModel.PretiumExpenses = ((claimsCalculationsModel.ProjClaimsAmount + claimsCalculationsModel.ProjLLFund + claimsCalculationsModel.COI_Contingent + claimsCalculationsModel.ProjIBNR + claimsCalculationsModel.ProjExposure) / 40) * 10;


                claimsCalculationsModel.ProjExposure = 0;

                claimsCalculationsModel.ProjIBNR = claimsCalculationsModel.ProjectedIBNRTech * ((claimsCalculationsModel.ProjYear == 3) ? claimsCalculationsModel.ThreeY_CPC_Inf : claimsCalculationsModel.FiveY_CPC_Inf);

                claimsCalculationsModel.Profit = ((claimsCalculationsModel.ProjClaimsAmount + claimsCalculationsModel.ProjLLFund + claimsCalculationsModel.COI_Contingent + claimsCalculationsModel.ProjIBNR + claimsCalculationsModel.ProjExposure) / 40) * 18;

                claimsCalculationsModel.ReinsuranceCosts = ((claimsCalculationsModel.ProjClaimsAmount + claimsCalculationsModel.ProjLLFund + claimsCalculationsModel.COI_Contingent + claimsCalculationsModel.ProjIBNR + claimsCalculationsModel.ProjExposure) / 40) * 26;

                /*claimsCalculationsModel.Commission = ((claimsCalculationsModel.ProjClaimsAmount + claimsCalculationsModel.ProjLLFund + claimsCalculationsModel.COI_Contingent + claimsCalculationsModel.ProjIBNR + claimsCalculationsModel.ProjExposure) / 40) * 1;*/

            }
            else if (claimsCalculationsModel.PriceByValue == 2) // exposure
            {
                claimsCalculationsModel.ProjClaimsAmount = claimsCalculationsModel.NewExposure * 0.4;
                claimsCalculationsModel.ProjIBNR = 0;
                claimsCalculationsModel.ProjLLFund = 0;
                claimsCalculationsModel.ProjExposure = 0;
                claimsCalculationsModel.COI_Contingent = 0;
                claimsCalculationsModel.Profit = (claimsCalculationsModel.NewExposure / 100) * 18;
                claimsCalculationsModel.ReinsuranceCosts = (claimsCalculationsModel.NewExposure / 100) * 26;

                claimsCalculationsModel.ProjClaimsHandlingFee = claimsCalculationsModel.ProjClaimsAmount * (3.6 / 47.1);
                claimsCalculationsModel.Levies = claimsCalculationsModel.ProjClaimsAmount * (4.6 / 47.1);
                claimsCalculationsModel.PretiumExpenses = claimsCalculationsModel.ProjClaimsAmount * (8.5 / 47.1);
            }
            else // blend
            {
                if (claimsCalculationsModel.PricingMetricValue == 1)
                {
                    if (fcTurnoverNonCOI == 0 && fcDaysNonCOI == 0)
                    {
                        claimsCalculationsModel.ProjClaimsAmount = totalVehicleNums * claimsCalculationsModel.ProjectedCCPVYTech;
                        claimsCalculationsModel.ProjLLFund = claimsCalculationsModel.ProjClaimsAmount * (13.1 / 47.1);
                        claimsCalculationsModel.ReinsuranceCosts = claimsCalculationsModel.ProjClaimsAmount * (17.4 / 47.1);
                        claimsCalculationsModel.ProjClaimsHandlingFee = claimsCalculationsModel.ProjClaimsAmount * (3.6 / 47.1);
                        claimsCalculationsModel.Levies = claimsCalculationsModel.ProjClaimsAmount * (4.7 / 47.1);
                        claimsCalculationsModel.PretiumExpenses = claimsCalculationsModel.ProjClaimsAmount * (8.5 / 47.1);
                        claimsCalculationsModel.Profit = claimsCalculationsModel.ProjClaimsAmount * (5.6 / 47.1);
                    }
                    else if (fcTurnoverNonCOI == 0)
                    {
                        claimsCalculationsModel.ProjClaimsAmount = (fcDaysNonCOI / 365) * claimsCalculationsModel.ProjectedCCPVYTech;
                        claimsCalculationsModel.ProjLLFund = claimsCalculationsModel.ProjClaimsAmount * (13.1 / 47.1);
                        claimsCalculationsModel.ReinsuranceCosts = claimsCalculationsModel.ProjClaimsAmount * (17.4 / 47.1);
                        claimsCalculationsModel.ProjClaimsHandlingFee = claimsCalculationsModel.ProjClaimsAmount * (3.6 / 47.1);
                        claimsCalculationsModel.Levies = claimsCalculationsModel.ProjClaimsAmount * (4.7 / 47.1);
                        claimsCalculationsModel.PretiumExpenses = claimsCalculationsModel.ProjClaimsAmount * (8.5 / 47.1);
                        claimsCalculationsModel.Profit = claimsCalculationsModel.ProjClaimsAmount * (5.6 / 47.1);
                    }
                    else
                    {
                        claimsCalculationsModel.ProjClaimsAmount = (fcTurnoverNonCOI / 1000) * ((claimsCalculationsModel.ProjYear == 3) ? claimsCalculationsModel.ThreeY_CCTO_Inf : claimsCalculationsModel.FiveY_CCTO_Inf);
                        claimsCalculationsModel.ProjLLFund = claimsCalculationsModel.ProjClaimsAmount * (13.1 / 47.1);
                        claimsCalculationsModel.ReinsuranceCosts = claimsCalculationsModel.ProjClaimsAmount * (17.4 / 47.1);
                        claimsCalculationsModel.ProjClaimsHandlingFee = claimsCalculationsModel.ProjClaimsAmount * (3.6 / 47.1);
                        claimsCalculationsModel.Levies = claimsCalculationsModel.ProjClaimsAmount * (4.7 / 47.1);
                        claimsCalculationsModel.PretiumExpenses = claimsCalculationsModel.ProjClaimsAmount * (8.5 / 47.1);
                        claimsCalculationsModel.Profit = claimsCalculationsModel.ProjClaimsAmount * (5.6 / 47.1);
                    }
                }
                else if (claimsCalculationsModel.PricingMetricValue == 2)
                {
                    claimsCalculationsModel.ProjClaimsAmount = claimsCalculationsModel.ProjectedClaimsTech * ((claimsCalculationsModel.ProjYear == 3) ? claimsCalculationsModel.ThreeY_CPC_Inf : claimsCalculationsModel.FiveY_CPC_Inf);
                    claimsCalculationsModel.ProjLLFund = claimsCalculationsModel.ProjClaimsAmount * (13.1 / 47.1);
                    claimsCalculationsModel.ReinsuranceCosts = claimsCalculationsModel.ProjClaimsAmount * (17.4 / 47.1);
                    claimsCalculationsModel.ProjClaimsHandlingFee = claimsCalculationsModel.ProjClaimsAmount * (3.6 / 47.1);
                    claimsCalculationsModel.Levies = claimsCalculationsModel.ProjClaimsAmount * (4.7 / 47.1);
                    claimsCalculationsModel.PretiumExpenses = claimsCalculationsModel.ProjClaimsAmount * (8.5 / 47.1);
                    claimsCalculationsModel.Profit = claimsCalculationsModel.ProjClaimsAmount * (5.6 / 47.1);
                }
                else
                {
                    claimsCalculationsModel.ProjClaimsAmount = claimsCalculationsModel.ProjectedCPIRDTech * fcDaysNonCOI;
                    claimsCalculationsModel.ProjLLFund = claimsCalculationsModel.ProjClaimsAmount * (13.1 / 47.1);
                    claimsCalculationsModel.ReinsuranceCosts = claimsCalculationsModel.ProjClaimsAmount * (17.4 / 47.1);
                    claimsCalculationsModel.ProjClaimsHandlingFee = claimsCalculationsModel.ProjClaimsAmount * (3.6 / 47.1);
                    claimsCalculationsModel.Levies = claimsCalculationsModel.ProjClaimsAmount * (4.7 / 47.1);
                    claimsCalculationsModel.PretiumExpenses = claimsCalculationsModel.ProjClaimsAmount * (8.5 / 47.1);
                    claimsCalculationsModel.Profit = claimsCalculationsModel.ProjClaimsAmount * (5.6 / 47.1);
                }

                // coi contingent here
                if (claimsCalculationsModel.ChargeCOIFeeValue == 0)
                {
                    claimsCalculationsModel.COI_Contingent = 0;
                }
                else if (fcTurnoverCOI == 0)
                {
                    claimsCalculationsModel.COI_Contingent = 0;
                }
                else if ((fcTurnoverCOI / (fcTurnoverCOI + fcTurnoverNonCOI) > 0.5))
                {
                    claimsCalculationsModel.COI_Contingent = fcTurnoverCOI / 100 * 1;
                }
                else
                {
                    claimsCalculationsModel.COI_Contingent = fcTurnoverCOI / 100 * 0.5;
                }

                var expoPercentage = staticClientData_fromTable.ExpoPercentage;

                claimsCalculationsModel.ProjExposure = expoPercentage * claimsCalculationsModel.NewExposure;

                claimsCalculationsModel.ProjIBNR = 
                    claimsCalculationsModel.ProjectedIBNRTech 
                    * ((claimsCalculationsModel.ProjYear == 3) ? claimsCalculationsModel.ThreeY_CPC_Inf : claimsCalculationsModel.FiveY_CPC_Inf);

                /*claimsCalculationsModel.Profit = 
                    ((
                        claimsCalculationsModel.ProjClaimsAmount 
                        + claimsCalculationsModel.ProjLLFund 
                        + claimsCalculationsModel.COI_Contingent 
                        + claimsCalculationsModel.ProjIBNR 
                        + claimsCalculationsModel.ProjExposure
                    ) / 40) * 18;

                claimsCalculationsModel.ReinsuranceCosts = 
                    ((
                        claimsCalculationsModel.ProjClaimsAmount 
                        + claimsCalculationsModel.ProjLLFund 
                        + claimsCalculationsModel.COI_Contingent 
                        + claimsCalculationsModel.ProjIBNR 
                        + claimsCalculationsModel.ProjExposure
                    ) / 40) * 26;

                claimsCalculationsModel.Commission = 
                    ((
                        claimsCalculationsModel.ProjClaimsAmount 
                        + claimsCalculationsModel.ProjLLFund 
                        + claimsCalculationsModel.COI_Contingent 
                        + claimsCalculationsModel.ProjIBNR 
                        + claimsCalculationsModel.ProjExposure
                    ) / 40) * 1;*/

            }


            /* claimsCalculationsModel.ProjClaimsHandlingFee = 
                 ((
                     claimsCalculationsModel.ProjClaimsAmount 
                     + claimsCalculationsModel.ProjLLFund 
                     + claimsCalculationsModel.COI_Contingent 
                     + claimsCalculationsModel.ProjIBNR 
                     + claimsCalculationsModel.ProjExposure
                 ) / 40) * 5;

             claimsCalculationsModel.PretiumExpenses =
                 ((
                     claimsCalculationsModel.ProjClaimsAmount 
                     + claimsCalculationsModel.ProjLLFund 
                     + claimsCalculationsModel.COI_Contingent 
                     + claimsCalculationsModel.ProjIBNR 
                     + claimsCalculationsModel.ProjExposure
                 ) / 40) * 10;*/

            claimsCalculationsModel.NetPremium =
                claimsCalculationsModel.ProjClaimsAmount * (100 / 47.1);

            claimsCalculationsModel.Commission = claimsCalculationsModel.NetPremium * 0.08;  // commission is 8% of net

            claimsCalculationsModel.GrossPremium = 
                claimsCalculationsModel.NetPremium 
                + claimsCalculationsModel.Commission;

            claimsCalculationsModel.GrossPremiumPlusIPT = 
                claimsCalculationsModel.GrossPremium 
                + (claimsCalculationsModel.GrossPremium / 100) * 12; // IPT is 12% of gross premium





            claimsCalculationsModel.InflationMonths = new List<SelectListItem>
            {
                new SelectListItem { Text = "1", Value = "1", Selected = claimsCalculationsModel.SelectedNumOfMonths == "1" },
                new SelectListItem { Text = "2", Value = "2", Selected = claimsCalculationsModel.SelectedNumOfMonths == "2" },
                new SelectListItem { Text = "3", Value = "3", Selected = claimsCalculationsModel.SelectedNumOfMonths == "3"},
                new SelectListItem { Text = "4", Value = "4", Selected = claimsCalculationsModel.SelectedNumOfMonths == "4" },
                new SelectListItem { Text = "5", Value = "5", Selected = claimsCalculationsModel.SelectedNumOfMonths == "5" },
                new SelectListItem { Text = "6", Value = "6", Selected = claimsCalculationsModel.SelectedNumOfMonths == "6" },
                new SelectListItem { Text = "7", Value = "7", Selected = claimsCalculationsModel.SelectedNumOfMonths == "7" },
                new SelectListItem { Text = "8", Value = "8", Selected = claimsCalculationsModel.SelectedNumOfMonths == "8" },
                new SelectListItem { Text = "9", Value = "9", Selected = claimsCalculationsModel.SelectedNumOfMonths == "9" },
                new SelectListItem { Text = "10", Value = "10", Selected = claimsCalculationsModel.SelectedNumOfMonths == "10" },
                new SelectListItem { Text = "11", Value = "11", Selected = claimsCalculationsModel.SelectedNumOfMonths == "11" },
                new SelectListItem { Text = "12", Value = "12", Selected = claimsCalculationsModel.SelectedNumOfMonths == "12" },
            };
            claimsCalculationsModel.PriceByFilters = new List<SelectListItem>
            {
                new SelectListItem {Text = "Experience", Value = "1", Selected = claimsCalculationsModel.PriceByValue == 1},
                new SelectListItem {Text = "Exposure",   Value = "2", Selected = claimsCalculationsModel.PriceByValue == 2},
                new SelectListItem {Text = "Blend",      Value = "3", Selected = claimsCalculationsModel.PriceByValue == 3},
            };
            claimsCalculationsModel.PricingMetrics = new List<SelectListItem>
            {
                new SelectListItem {Text = "CCPVY", Value = "1", Selected = claimsCalculationsModel.PricingMetricValue == 1},
                new SelectListItem {Text = "Claims", Value = "2", Selected = claimsCalculationsModel.PricingMetricValue == 2},
                new SelectListItem {Text = "Days", Value = "3", Selected = claimsCalculationsModel.PricingMetricValue == 3},
            };
            claimsCalculationsModel.ChargeCOIFee = new List<SelectListItem>
            {
                new SelectListItem {Text = "Yes", Value = "1", Selected = claimsCalculationsModel.ChargeCOIFeeValue == 1},
                new SelectListItem {Text = "No", Value = "2", Selected = claimsCalculationsModel.ChargeCOIFeeValue == 2}
            };
            claimsCalculationsModel.ProjectedYears = new List<SelectListItem>
            {
                new SelectListItem { Text = "3", Value = "3", Selected = claimsCalculationsModel.ProjYear == 3 },
                new SelectListItem { Text = "5", Value = "5", Selected = claimsCalculationsModel.ProjYear == 5 },
            };






            
            
            return claimsCalculationsModel;

        }
    }
}


/*
 // Contingent Fee
            if (claimsCalculationsModel.ChargeCOIFeeValue == 2)
            {
                claimsCalculationsModel.COI_Contingent = 0;
            }
            else if (fcTurnoverCOI == 0)
            {
                claimsCalculationsModel.COI_Contingent = 0;
            }
            else if (fcTurnoverCOI / (fcTurnoverCOI + fcTurnoverNonCOI) > 0.5)
            {
                claimsCalculationsModel.COI_Contingent = (fcTurnoverCOI / 100) * 1;
            }
            else
            {
                claimsCalculationsModel.COI_Contingent = (fcTurnoverCOI / 100) * 0.5;
            }
            // Projected LL Fund
            if (fcDaysNonCOI == 0)
            {
                claimsCalculationsModel.ProjLLFund = totalLLL;
            }
            else
            {
                claimsCalculationsModel.ProjLLFund = totalLLL * ((double)fcDaysNonCOI / (fcDaysNonCOI + fcDaysCOI));
            }


            // Projected Claims Amount
            if (claimsCalculationsModel.PriceByValue == 1)  // experience
            {
                if (claimsCalculationsModel.PricingMetricValue == 1)
                {
                    if (fcTurnoverNonCOI == 0 && fcDaysNonCOI == 0)
                    {
                        claimsCalculationsModel.ProjClaimsAmount = (double)totalVehicleNums * claimsCalculationsModel.ProjectedCCPVYTech;
                    }
                    else if (fcTurnoverNonCOI == 0)
                    {
                        claimsCalculationsModel.ProjClaimsAmount = ((double)fcDaysNonCOI / 365) * claimsCalculationsModel.ProjectedCCPVYTech;
                    }
                    else
                    {
                        claimsCalculationsModel.ProjClaimsAmount = ((double)fcTurnoverNonCOI / 1000) * ((claimsCalculationsModel.ProjYear == 3) ? claimsCalculationsModel.ThreeY_CCTO_Inf : claimsCalculationsModel.FiveY_CCTO_Inf);
                    }
                }
                else if (claimsCalculationsModel.PricingMetricValue == 2)
                {
                    claimsCalculationsModel.ProjClaimsAmount = claimsCalculationsModel.ProjectedClaimsTech * ((claimsCalculationsModel.ProjYear == 3) ? claimsCalculationsModel.ThreeY_CPC_Inf : claimsCalculationsModel.FiveY_CPC_Inf);
                }
                else
                {
                    claimsCalculationsModel.ProjClaimsAmount = claimsCalculationsModel.ProjectedCPIRDTech * (double)fcDaysNonCOI;
                }

                claimsCalculationsModel.PretiumExpenses = ((claimsCalculationsModel.ProjClaimsAmount + claimsCalculationsModel.ProjLLFund + claimsCalculationsModel.COI_Contingent + claimsCalculationsModel.ProjIBNR + claimsCalculationsModel.ProjExposure) / 40) * 10;


                claimsCalculationsModel.ProjExposure = 0;

                claimsCalculationsModel.ProjIBNR = claimsCalculationsModel.ProjectedIBNRTech * ((claimsCalculationsModel.ProjYear == 3) ? claimsCalculationsModel.ThreeY_CPC_Inf : claimsCalculationsModel.FiveY_CPC_Inf);

                claimsCalculationsModel.Profit = ((claimsCalculationsModel.ProjClaimsAmount + claimsCalculationsModel.ProjLLFund + claimsCalculationsModel.COI_Contingent + claimsCalculationsModel.ProjIBNR + claimsCalculationsModel.ProjExposure) / 40) * 18;

                claimsCalculationsModel.ReinsuranceCosts = ((claimsCalculationsModel.ProjClaimsAmount + claimsCalculationsModel.ProjLLFund + claimsCalculationsModel.COI_Contingent + claimsCalculationsModel.ProjIBNR + claimsCalculationsModel.ProjExposure) / 40) * 26;

                claimsCalculationsModel.Commission = ((claimsCalculationsModel.ProjClaimsAmount + claimsCalculationsModel.ProjLLFund + claimsCalculationsModel.COI_Contingent + claimsCalculationsModel.ProjIBNR + claimsCalculationsModel.ProjExposure) / 40) * 1;

            }
            else if (claimsCalculationsModel.PriceByValue == 2) // exposure
            {
                claimsCalculationsModel.ProjClaimsAmount = claimsCalculationsModel.NewExposure * 0.4;
                claimsCalculationsModel.ProjIBNR = 0;
                claimsCalculationsModel.ProjLLFund = 0;
                claimsCalculationsModel.ProjExposure = 0;
                claimsCalculationsModel.COI_Contingent = 0;
                claimsCalculationsModel.Profit = (claimsCalculationsModel.NewExposure / 100) * 18;
                claimsCalculationsModel.ReinsuranceCosts = (claimsCalculationsModel.NewExposure / 100) * 26;
                claimsCalculationsModel.Commission = claimsCalculationsModel.NewExposure / 100;
            }
            else // blend
            {
                if (claimsCalculationsModel.PricingMetricValue == 1)
                {
                    if (fcTurnoverNonCOI == 0 && fcDaysNonCOI == 0)
                    {
                        claimsCalculationsModel.ProjClaimsAmount = totalVehicleNums * claimsCalculationsModel.ProjectedCCPVYTech;
                    }
                    else if (fcTurnoverNonCOI == 0)
                    {
                        claimsCalculationsModel.ProjClaimsAmount = (fcDaysNonCOI / 365) * claimsCalculationsModel.ProjectedCCPVYTech;
                    }
                    else
                    {
                        claimsCalculationsModel.ProjClaimsAmount = (fcTurnoverNonCOI / 1000) * ((claimsCalculationsModel.ProjYear == 3) ? claimsCalculationsModel.ThreeY_CCTO_Inf : claimsCalculationsModel.FiveY_CCTO_Inf);
                    }
                }
                else if (claimsCalculationsModel.PricingMetricValue == 2)
                {
                    claimsCalculationsModel.ProjClaimsAmount = claimsCalculationsModel.ProjectedClaimsTech * ((claimsCalculationsModel.ProjYear == 3) ? claimsCalculationsModel.ThreeY_CPC_Inf : claimsCalculationsModel.FiveY_CPC_Inf);
                }
                else
                {
                    claimsCalculationsModel.ProjClaimsAmount = claimsCalculationsModel.ProjectedCPIRDTech * fcDaysNonCOI;
                }

                var expoPercentage = claimsByYear[year1].FirstOrDefault().ExpoPercentage;

                claimsCalculationsModel.ProjExposure = expoPercentage * claimsCalculationsModel.NewExposure;

                claimsCalculationsModel.ProjIBNR = 
                    claimsCalculationsModel.ProjectedIBNRTech 
                    * ((claimsCalculationsModel.ProjYear == 3) ? claimsCalculationsModel.ThreeY_CPC_Inf : claimsCalculationsModel.FiveY_CPC_Inf);

                claimsCalculationsModel.Profit = 
                    ((
                        claimsCalculationsModel.ProjClaimsAmount 
                        + claimsCalculationsModel.ProjLLFund 
                        + claimsCalculationsModel.COI_Contingent 
                        + claimsCalculationsModel.ProjIBNR 
                        + claimsCalculationsModel.ProjExposure
                    ) / 40) * 18;

                claimsCalculationsModel.ReinsuranceCosts = 
                    ((
                        claimsCalculationsModel.ProjClaimsAmount 
                        + claimsCalculationsModel.ProjLLFund 
                        + claimsCalculationsModel.COI_Contingent 
                        + claimsCalculationsModel.ProjIBNR 
                        + claimsCalculationsModel.ProjExposure
                    ) / 40) * 26;

                claimsCalculationsModel.Commission = 
                    ((
                        claimsCalculationsModel.ProjClaimsAmount 
                        + claimsCalculationsModel.ProjLLFund 
                        + claimsCalculationsModel.COI_Contingent 
                        + claimsCalculationsModel.ProjIBNR 
                        + claimsCalculationsModel.ProjExposure
                    ) / 40) * 1;

            }


            claimsCalculationsModel.ProjClaimsHandlingFee = 
                ((
                    claimsCalculationsModel.ProjClaimsAmount 
                    + claimsCalculationsModel.ProjLLFund 
                    + claimsCalculationsModel.COI_Contingent 
                    + claimsCalculationsModel.ProjIBNR 
                    + claimsCalculationsModel.ProjExposure
                ) / 40) * 5;

            claimsCalculationsModel.PretiumExpenses =
                ((
                    claimsCalculationsModel.ProjClaimsAmount 
                    + claimsCalculationsModel.ProjLLFund 
                    + claimsCalculationsModel.COI_Contingent 
                    + claimsCalculationsModel.ProjIBNR 
                    + claimsCalculationsModel.ProjExposure
                ) / 40) * 10;

            claimsCalculationsModel.NetPremium = 
                claimsCalculationsModel.ProjClaimsAmount 
                + claimsCalculationsModel.ProjLLFund 
                + claimsCalculationsModel.COI_Contingent 
                + claimsCalculationsModel.ProjIBNR 
                + claimsCalculationsModel.ProjExposure 
                + claimsCalculationsModel.ProjClaimsHandlingFee 
                + claimsCalculationsModel.PretiumExpenses 
                + claimsCalculationsModel.Profit
                + claimsCalculationsModel.ReinsuranceCosts;

            claimsCalculationsModel.GrossPremium = 
                claimsCalculationsModel.NetPremium 
                + claimsCalculationsModel.Commission;

            claimsCalculationsModel.GrossPremiumPlusIPT = 
                claimsCalculationsModel.GrossPremium 
                + (claimsCalculationsModel.GrossPremium / 100) * 12; // IPT is 12% of gross premium

            claimsCalculationsModel.Levies = claimsCalculationsModel.NetPremium * 0.047;
*/




// ----------------- HISTORIC YEARS DATA TABLE DATA -----------------------------

/*claimsCalculationsModel.Y1RentalDaysCOI = claimsByYear[year1].FirstOrDefault().RDaysCOI;
claimsCalculationsModel.Y2RentalDaysCOI = claimsByYear[year2].FirstOrDefault().RDaysCOI;
claimsCalculationsModel.Y3RentalDaysCOI = claimsByYear[year3].FirstOrDefault().RDaysCOI;
claimsCalculationsModel.Y4RentalDaysCOI = claimsByYear[year4].FirstOrDefault().RDaysCOI;
claimsCalculationsModel.Y5RentalDaysCOI = claimsByYear[year5].FirstOrDefault().RDaysCOI;

claimsCalculationsModel.ThreeY_RDaysCOI = claimsCalculationsModel.Y1RentalDaysCOI + claimsCalculationsModel.Y2RentalDaysCOI + claimsCalculationsModel.Y3RentalDaysCOI;
claimsCalculationsModel.FiveY_RDaysCOI = claimsCalculationsModel.ThreeY_RDaysCOI + claimsCalculationsModel.Y4RentalDaysCOI + claimsCalculationsModel.Y5RentalDaysCOI;


claimsCalculationsModel.Y1RentalDaysNonCOI = claimsByYear[year1].FirstOrDefault().RDaysNonCOI;
claimsCalculationsModel.Y2RentalDaysNonCOI = claimsByYear[year2].FirstOrDefault().RDaysNonCOI;
claimsCalculationsModel.Y3RentalDaysNonCOI = claimsByYear[year3].FirstOrDefault().RDaysNonCOI;
claimsCalculationsModel.Y4RentalDaysNonCOI = claimsByYear[year4].FirstOrDefault().RDaysNonCOI;
claimsCalculationsModel.Y5RentalDaysNonCOI = claimsByYear[year5].FirstOrDefault().RDaysNonCOI;

claimsCalculationsModel.ThreeY_RDaysNonCOI = claimsCalculationsModel.Y1RentalDaysNonCOI + claimsCalculationsModel.Y2RentalDaysNonCOI + claimsCalculationsModel.Y3RentalDaysNonCOI;
claimsCalculationsModel.FiveY_RDaysNonCOI = claimsCalculationsModel.ThreeY_RDaysNonCOI + claimsCalculationsModel.Y4RentalDaysNonCOI + claimsCalculationsModel.Y5RentalDaysNonCOI;

claimsCalculationsModel.Y1TO_COI = claimsByYear[year1].FirstOrDefault().TurnoverCOI;
claimsCalculationsModel.Y2TO_COI = claimsByYear[year2].FirstOrDefault().TurnoverCOI;
claimsCalculationsModel.Y3TO_COI = claimsByYear[year3].FirstOrDefault().TurnoverCOI;
claimsCalculationsModel.Y4TO_COI = claimsByYear[year4].FirstOrDefault().TurnoverCOI;
claimsCalculationsModel.Y5TO_COI = claimsByYear[year5].FirstOrDefault().TurnoverCOI;

claimsCalculationsModel.ThreeY_TO_COI = claimsCalculationsModel.Y1TO_COI + claimsCalculationsModel.Y2TO_COI + claimsCalculationsModel.Y3TO_COI;
claimsCalculationsModel.FiveY_TO_COI = claimsCalculationsModel.ThreeY_TO_COI + claimsCalculationsModel.Y4TO_COI + claimsCalculationsModel.Y5TO_COI;

claimsCalculationsModel.Y1TO_NonCOI = claimsByYear[year1].FirstOrDefault().TurnoverNonCOI;
claimsCalculationsModel.Y2TO_NonCOI = claimsByYear[year2].FirstOrDefault().TurnoverNonCOI;
claimsCalculationsModel.Y3TO_NonCOI = claimsByYear[year3].FirstOrDefault().TurnoverNonCOI;
claimsCalculationsModel.Y4TO_NonCOI = claimsByYear[year4].FirstOrDefault().TurnoverNonCOI;
claimsCalculationsModel.Y5TO_NonCOI = claimsByYear[year5].FirstOrDefault().TurnoverNonCOI;

claimsCalculationsModel.ThreeY_TO_NonCOI = claimsCalculationsModel.Y1TO_NonCOI + claimsCalculationsModel.Y2TO_NonCOI + claimsCalculationsModel.Y3TO_NonCOI;
claimsCalculationsModel.FiveY_TO_NonCOI = claimsCalculationsModel.ThreeY_TO_NonCOI + claimsCalculationsModel.Y4TO_NonCOI + claimsCalculationsModel.Y5TO_NonCOI;

// Utlisation rate is hardcoded in claimsCalculationsModel

claimsCalculationsModel.Y1VYrs = (double)claimsCalculationsModel.Y1RentalDaysNonCOI / 365;
claimsCalculationsModel.Y2VYrs = (double)claimsCalculationsModel.Y2RentalDaysNonCOI / 365;
claimsCalculationsModel.Y3VYrs = (double)claimsCalculationsModel.Y3RentalDaysNonCOI / 365;
claimsCalculationsModel.Y4VYrs = (double)claimsCalculationsModel.Y4RentalDaysNonCOI / 365;
claimsCalculationsModel.Y5VYrs = (double)claimsCalculationsModel.Y5RentalDaysNonCOI / 365;

claimsCalculationsModel.ThreeY_VYrs = claimsCalculationsModel.Y1VYrs + claimsCalculationsModel.Y2VYrs + claimsCalculationsModel.Y3VYrs;
claimsCalculationsModel.FiveY_VYrs = claimsCalculationsModel.Y1VYrs + claimsCalculationsModel.Y2VYrs + claimsCalculationsModel.Y3VYrs + claimsCalculationsModel.Y4VYrs + claimsCalculationsModel.Y5VYrs;

claimsCalculationsModel.Y1ClaimsOpen = claimsByYear[year1]
    .Count(claim => claim.Status == "O" || claim.Status == "Opened" || claim.Status == "Active" || claim.Status == "Reopened" || claim.Status == "ACTIVE");
claimsCalculationsModel.Y2ClaimsOpen = claimsByYear[year2]
    .Count(claim => claim.Status == "O" || claim.Status == "Opened" || claim.Status == "Active" || claim.Status == "Reopened" || claim.Status == "ACTIVE");
claimsCalculationsModel.Y3ClaimsOpen = claimsByYear[year3]
    .Count(claim => claim.Status == "O" || claim.Status == "Opened" || claim.Status == "Active" || claim.Status == "Reopened" || claim.Status == "ACTIVE");
claimsCalculationsModel.Y4ClaimsOpen = claimsByYear[year4]
    .Count(claim => claim.Status == "O" || claim.Status == "Opened" || claim.Status == "Active" || claim.Status == "Reopened" || claim.Status == "ACTIVE");
claimsCalculationsModel.Y5ClaimsOpen = claimsByYear[year5]
    .Count(claim => claim.Status == "O" || claim.Status == "Opened" || claim.Status == "Active" || claim.Status == "Reopened" || claim.Status == "ACTIVE");

claimsCalculationsModel.ThreeY_ClaimsOpen = claimsCalculationsModel.Y1ClaimsOpen + claimsCalculationsModel.Y2ClaimsOpen + claimsCalculationsModel.Y3ClaimsOpen;
claimsCalculationsModel.FiveY_ClaimsOpen = claimsCalculationsModel.ThreeY_ClaimsOpen + claimsCalculationsModel.Y4ClaimsOpen + claimsCalculationsModel.Y5ClaimsOpen;

claimsCalculationsModel.Y1ClaimsClosed = claimsByYear[year1]
    .Count(claim => claim.Status == "S" || claim.Status == "Settled" || claim.Status == "Closed" || claim.Status == "Information Only" || claim.Status == "SETTLED" || claim.Status == "CANCELLED");
claimsCalculationsModel.Y2ClaimsClosed = claimsByYear[year2]
    .Count(claim => claim.Status == "S" || claim.Status == "Settled" || claim.Status == "Closed" || claim.Status == "Information Only" || claim.Status == "SETTLED" || claim.Status == "CANCELLED");
claimsCalculationsModel.Y3ClaimsClosed = claimsByYear[year3]
    .Count(claim => claim.Status == "S" || claim.Status == "Settled" || claim.Status == "Closed" || claim.Status == "Information Only" || claim.Status == "SETTLED" || claim.Status == "CANCELLED");
claimsCalculationsModel.Y4ClaimsClosed = claimsByYear[year4]
    .Count(claim => claim.Status == "S" || claim.Status == "Settled" || claim.Status == "Closed" || claim.Status == "Information Only" || claim.Status == "SETTLED" || claim.Status == "CANCELLED");
claimsCalculationsModel.Y5ClaimsClosed = claimsByYear[year5]
    .Count(claim => claim.Status == "S" || claim.Status == "Settled" || claim.Status == "Closed" || claim.Status == "Information Only" || claim.Status == "SETTLED" || claim.Status == "CANCELLED");

claimsCalculationsModel.ThreeY_ClaimsClo = claimsCalculationsModel.Y1ClaimsClosed + claimsCalculationsModel.Y2ClaimsClosed + claimsCalculationsModel.Y3ClaimsClosed;
claimsCalculationsModel.FiveY_ClaimsClo = claimsCalculationsModel.ThreeY_ClaimsClo + claimsCalculationsModel.Y4ClaimsClosed + claimsCalculationsModel.Y5ClaimsClosed;

claimsCalculationsModel.Y1ADPaid = claimsByYear[year1].Sum(claim => claim.AD_Paid);
claimsCalculationsModel.Y2ADPaid = claimsByYear[year2].Sum(claim => claim.AD_Paid);
claimsCalculationsModel.Y3ADPaid = claimsByYear[year3].Sum(claim => claim.AD_Paid);
claimsCalculationsModel.Y4ADPaid = claimsByYear[year4].Sum(claim => claim.AD_Paid);
claimsCalculationsModel.Y5ADPaid = claimsByYear[year5].Sum(claim => claim.AD_Paid);

claimsCalculationsModel.ThreeY_ADPaid = claimsCalculationsModel.Y1ADPaid + claimsCalculationsModel.Y2ADPaid + claimsCalculationsModel.Y3ADPaid;
claimsCalculationsModel.FiveY_ADPaid = claimsCalculationsModel.ThreeY_ADPaid + claimsCalculationsModel.Y4ADPaid + claimsCalculationsModel.Y5ADPaid;

claimsCalculationsModel.Y1FTPaid = claimsByYear[year1].Sum(claim => claim.FT_Paid);
claimsCalculationsModel.Y2FTPaid = claimsByYear[year2].Sum(claim => claim.FT_Paid);
claimsCalculationsModel.Y3FTPaid = claimsByYear[year3].Sum(claim => claim.FT_Paid);
claimsCalculationsModel.Y4FTPaid = claimsByYear[year4].Sum(claim => claim.FT_Paid);
claimsCalculationsModel.Y5FTPaid = claimsByYear[year5].Sum(claim => claim.FT_Paid);

claimsCalculationsModel.ThreeY_FTPaid = claimsCalculationsModel.Y1FTPaid + claimsCalculationsModel.Y2FTPaid + claimsCalculationsModel.Y3FTPaid;
claimsCalculationsModel.FiveY_FTPaid = claimsCalculationsModel.ThreeY_FTPaid + claimsCalculationsModel.Y4FTPaid + claimsCalculationsModel.Y5FTPaid;

claimsCalculationsModel.Y1TPPD = claimsByYear[year1].Sum(claim => claim.TPPD_Paid);
claimsCalculationsModel.Y2TPPD = claimsByYear[year2].Sum(claim => claim.TPPD_Paid);
claimsCalculationsModel.Y3TPPD = claimsByYear[year3].Sum(claim => claim.TPPD_Paid);
claimsCalculationsModel.Y4TPPD = claimsByYear[year4].Sum(claim => claim.TPPD_Paid);
claimsCalculationsModel.Y5TPPD = claimsByYear[year5].Sum(claim => claim.TPPD_Paid);

claimsCalculationsModel.ThreeY_TPPD = claimsCalculationsModel.Y1TPPD + claimsCalculationsModel.Y2TPPD + claimsCalculationsModel.Y3TPPD;
claimsCalculationsModel.FiveY_TPPD = claimsCalculationsModel.ThreeY_TPPD + claimsCalculationsModel.Y4TPPD + claimsCalculationsModel.Y5TPPD;

claimsCalculationsModel.Y1TPCH = claimsByYear[year1].Sum(claim => claim.TPCH_Paid);
claimsCalculationsModel.Y2TPCH = claimsByYear[year2].Sum(claim => claim.TPCH_Paid);
claimsCalculationsModel.Y3TPCH = claimsByYear[year3].Sum(claim => claim.TPCH_Paid);
claimsCalculationsModel.Y4TPCH = claimsByYear[year4].Sum(claim => claim.TPCH_Paid);
claimsCalculationsModel.Y5TPCH = claimsByYear[year5].Sum(claim => claim.TPCH_Paid);

claimsCalculationsModel.ThreeY_TPCH = claimsCalculationsModel.Y1TPCH + claimsCalculationsModel.Y2TPCH + claimsCalculationsModel.Y3TPCH;
claimsCalculationsModel.FiveY_TPCH = claimsCalculationsModel.ThreeY_TPCH + claimsCalculationsModel.Y4TPCH + claimsCalculationsModel.Y5TPCH;

claimsCalculationsModel.Y1TPPI = claimsByYear[year1].Sum(claim => claim.TPPI_Paid);
claimsCalculationsModel.Y2TPPI = claimsByYear[year2].Sum(claim => claim.TPPI_Paid);
claimsCalculationsModel.Y3TPPI = claimsByYear[year3].Sum(claim => claim.TPPI_Paid);
claimsCalculationsModel.Y4TPPI = claimsByYear[year4].Sum(claim => claim.TPPI_Paid);
claimsCalculationsModel.Y5TPPI = claimsByYear[year5].Sum(claim => claim.TPPI_Paid);

claimsCalculationsModel.ThreeY_TPPI = claimsCalculationsModel.Y1TPPI + claimsCalculationsModel.Y2TPPI + claimsCalculationsModel.Y3TPPI;
claimsCalculationsModel.FiveY_TPPI = claimsCalculationsModel.ThreeY_TPPI + claimsCalculationsModel.Y4TPPI + claimsCalculationsModel.Y5TPPI;

claimsCalculationsModel.Y1ADOS = claimsByYear[year1].Sum(claim => claim.ADOS);
claimsCalculationsModel.Y2ADOS = claimsByYear[year2].Sum(claim => claim.ADOS);
claimsCalculationsModel.Y3ADOS = claimsByYear[year3].Sum(claim => claim.ADOS);
claimsCalculationsModel.Y4ADOS = claimsByYear[year4].Sum(claim => claim.ADOS);
claimsCalculationsModel.Y5ADOS = claimsByYear[year5].Sum(claim => claim.ADOS);

claimsCalculationsModel.ThreeY_ADOS = claimsCalculationsModel.Y1ADOS + claimsCalculationsModel.Y2ADOS + claimsCalculationsModel.Y3ADOS;
claimsCalculationsModel.FiveY_ADOS = claimsCalculationsModel.ThreeY_ADOS + claimsCalculationsModel.Y4ADOS + claimsCalculationsModel.Y5ADOS;

claimsCalculationsModel.Y1FTOS = claimsByYear[year1].Sum(claim => claim.FTOS);
claimsCalculationsModel.Y2FTOS = claimsByYear[year2].Sum(claim => claim.FTOS);
claimsCalculationsModel.Y3FTOS = claimsByYear[year3].Sum(claim => claim.FTOS);
claimsCalculationsModel.Y4FTOS = claimsByYear[year4].Sum(claim => claim.FTOS);
claimsCalculationsModel.Y5FTOS = claimsByYear[year5].Sum(claim => claim.FTOS);

claimsCalculationsModel.ThreeY_FTOS = claimsCalculationsModel.Y1FTOS + claimsCalculationsModel.Y2FTOS + claimsCalculationsModel.Y3FTOS;
claimsCalculationsModel.FiveY_FTOS = claimsCalculationsModel.ThreeY_FTOS + claimsCalculationsModel.Y4FTOS + claimsCalculationsModel.Y5FTOS;

claimsCalculationsModel.Y1TPPDOS = claimsByYear[year1].Sum(claim => claim.TPPD_OS);
claimsCalculationsModel.Y2TPPDOS = claimsByYear[year2].Sum(claim => claim.TPPD_OS);
claimsCalculationsModel.Y3TPPDOS = claimsByYear[year3].Sum(claim => claim.TPPD_OS);
claimsCalculationsModel.Y4TPPDOS = claimsByYear[year4].Sum(claim => claim.TPPD_OS);
claimsCalculationsModel.Y5TPPDOS = claimsByYear[year5].Sum(claim => claim.TPPD_OS);

claimsCalculationsModel.ThreeY_TPPDOS = claimsCalculationsModel.Y1TPPDOS + claimsCalculationsModel.Y2TPPDOS + claimsCalculationsModel.Y3TPPDOS;
claimsCalculationsModel.FiveY_TPPDOS = claimsCalculationsModel.ThreeY_TPPDOS + claimsCalculationsModel.Y4TPPDOS + claimsCalculationsModel.Y5TPPDOS;

claimsCalculationsModel.Y1TPCHOS = claimsByYear[year1].Sum(claim => claim.TPCH_OS);
claimsCalculationsModel.Y2TPCHOS = claimsByYear[year2].Sum(claim => claim.TPCH_OS);
claimsCalculationsModel.Y3TPCHOS = claimsByYear[year3].Sum(claim => claim.TPCH_OS);
claimsCalculationsModel.Y4TPCHOS = claimsByYear[year4].Sum(claim => claim.TPCH_OS);
claimsCalculationsModel.Y5TPCHOS = claimsByYear[year5].Sum(claim => claim.TPCH_OS);

claimsCalculationsModel.ThreeY_TPCHOS = claimsCalculationsModel.Y1TPCHOS + claimsCalculationsModel.Y2TPCHOS + claimsCalculationsModel.Y3TPCHOS;
claimsCalculationsModel.FiveY_TPCHOS = claimsCalculationsModel.ThreeY_TPCHOS + claimsCalculationsModel.Y4TPCHOS + claimsCalculationsModel.Y5TPCHOS;

claimsCalculationsModel.Y1TPPIOS = claimsByYear[year1].Sum(claim => claim.TPPI_OS);
claimsCalculationsModel.Y2TPPIOS = claimsByYear[year2].Sum(claim => claim.TPPI_OS);
claimsCalculationsModel.Y3TPPIOS = claimsByYear[year3].Sum(claim => claim.TPPI_OS);
claimsCalculationsModel.Y4TPPIOS = claimsByYear[year4].Sum(claim => claim.TPPI_OS);
claimsCalculationsModel.Y5TPPIOS = claimsByYear[year5].Sum(claim => claim.TPPI_OS);

claimsCalculationsModel.ThreeY_TPPIOS = claimsCalculationsModel.Y1TPPIOS + claimsCalculationsModel.Y2TPPIOS + claimsCalculationsModel.Y3TPPIOS;
claimsCalculationsModel.FiveY_TPPIOS = claimsCalculationsModel.ThreeY_TPPIOS + claimsCalculationsModel.Y4TPPIOS + claimsCalculationsModel.Y5TPPIOS;

claimsCalculationsModel.Y1Total = claimsByYear[year1].Sum(claim => claim.Total);
claimsCalculationsModel.Y2Total = claimsByYear[year2].Sum(claim => claim.Total);
claimsCalculationsModel.Y3Total = claimsByYear[year3].Sum(claim => claim.Total);
claimsCalculationsModel.Y4Total = claimsByYear[year4].Sum(claim => claim.Total);
claimsCalculationsModel.Y5Total = claimsByYear[year5].Sum(claim => claim.Total);

claimsCalculationsModel.ThreeY_Total = claimsCalculationsModel.Y1Total + claimsCalculationsModel.Y2Total + claimsCalculationsModel.Y3Total;
claimsCalculationsModel.FiveY_Total = claimsCalculationsModel.ThreeY_Total + claimsCalculationsModel.Y4Total + claimsCalculationsModel.Y5Total;

*/