using CSV_reader.database;
using CSV_reader.Models;
using Microsoft.Extensions.Options;
using CSV_reader.Configurations;
using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CSV_reader.Services
{
    public class GetHistoricDataForQuoteSearchService : IGetHistoricDataForQuoteSearchService
    {
        private readonly ApplicationContext _appContext;
        private readonly IWebHostEnvironment _env;

        public GetHistoricDataForQuoteSearchService(ApplicationContext appContext, IWebHostEnvironment env)
        {
            _appContext = appContext;
            _env = env;
        }

        public HistoricYearsData GetHistoricYearsDataForQuoteSearch(int quoteId) {

            var tableData = _appContext.ClientDetails.FirstOrDefault(x => x.Id == quoteId);

            var historicData = new HistoricYearsData();

            historicData.Year1Name = tableData.Year1Name;
            historicData.Year2Name = tableData.Year2Name;
            historicData.Year3Name = tableData.Year3Name;
            historicData.Year4Name = tableData.Year4Name;
            historicData.Year5Name = tableData.Year5Name;

            historicData.Y1RentalDaysCOI = tableData.Y1RentalDaysCOI;
            historicData.Y2RentalDaysCOI = tableData.Y2RentalDaysCOI;
            historicData.Y3RentalDaysCOI = tableData.Y3RentalDaysCOI;
            historicData.Y4RentalDaysCOI = tableData.Y4RentalDaysCOI;
            historicData.Y5RentalDaysCOI = tableData.Y5RentalDaysCOI;
            historicData.ThreeY_RDaysCOI = tableData.ThreeY_RDaysCOI;
            historicData.FiveY_RDaysCOI = tableData.FiveY_RDaysCOI;

            historicData.Y1RentalDaysNonCOI = tableData.Y1RentalDaysNonCOI;
            historicData.Y2RentalDaysNonCOI = tableData.Y2RentalDaysNonCOI;
            historicData.Y3RentalDaysNonCOI = tableData.Y3RentalDaysNonCOI;
            historicData.Y4RentalDaysNonCOI = tableData.Y4RentalDaysNonCOI;
            historicData.Y5RentalDaysNonCOI = tableData.Y5RentalDaysNonCOI;
            historicData.ThreeY_RDaysNonCOI = tableData.ThreeY_RDaysNonCOI;
            historicData.FiveY_RDaysNonCOI = tableData.FiveY_RDaysNonCOI;

            historicData.Y1TO_COI = tableData.Y1TO_COI;
            historicData.Y2TO_COI = tableData.Y2TO_COI;
            historicData.Y3TO_COI = tableData.Y3TO_COI;
            historicData.Y4TO_COI = tableData.Y4TO_COI;
            historicData.Y5TO_COI = tableData.Y5TO_COI;
            historicData.ThreeY_TO_COI = tableData.ThreeY_TO_COI;
            historicData.FiveY_TO_COI = tableData.FiveY_TO_COI;

            historicData.Y1TO_NonCOI = tableData.Y1TO_NonCOI;
            historicData.Y2TO_NonCOI = tableData.Y2TO_NonCOI;
            historicData.Y3TO_NonCOI = tableData.Y3TO_NonCOI;
            historicData.Y4TO_NonCOI = tableData.Y4TO_NonCOI;
            historicData.Y5TO_NonCOI = tableData.Y5TO_NonCOI;
            historicData.ThreeY_TO_NonCOI = tableData.ThreeY_TO_NonCOI;
            historicData.FiveY_TO_NonCOI = tableData.FiveY_TO_NonCOI;

            historicData.Y1UT = tableData.Y1UT;
            historicData.Y2UT = tableData.Y2UT;
            historicData.Y3UT = tableData.Y3UT;
            historicData.Y4UT = tableData.Y4UT;
            historicData.Y5UT = tableData.Y5UT;
            historicData.ThreeY_UT = tableData.ThreeY_UT;
            historicData.FiveY_UT = tableData.FiveY_UT;

            historicData.Y1VYrs = tableData.Y1VYrs;
            historicData.Y2VYrs = tableData.Y2VYrs;
            historicData.Y3VYrs = tableData.Y3VYrs;
            historicData.Y4VYrs = tableData.Y4VYrs;
            historicData.Y5VYrs = tableData.Y5VYrs;
            historicData.ThreeY_VYrs = tableData.ThreeY_VYrs;
            historicData.FiveY_VYrs = tableData.FiveY_VYrs;

            historicData.Y1ClaimsOpen = tableData.Y1ClaimsOpen;
            historicData.Y2ClaimsOpen = tableData.Y2ClaimsOpen;
            historicData.Y3ClaimsOpen = tableData.Y3ClaimsOpen;
            historicData.Y4ClaimsOpen = tableData.Y4ClaimsOpen;
            historicData.Y5ClaimsOpen = tableData.Y5ClaimsOpen;
            historicData.ThreeY_ClaimsOpen = tableData.ThreeY_ClaimsOpen;
            historicData.FiveY_ClaimsOpen = tableData.FiveY_ClaimsOpen;

            historicData.Y1ClaimsClosed = tableData.Y1ClaimsClosed;
            historicData.Y2ClaimsClosed = tableData.Y2ClaimsClosed;
            historicData.Y3ClaimsClosed = tableData.Y3ClaimsClosed;
            historicData.Y4ClaimsClosed = tableData.Y4ClaimsClosed;
            historicData.Y5ClaimsClosed = tableData.Y5ClaimsClosed;
            historicData.ThreeY_ClaimsClo = tableData.ThreeY_ClaimsClo;
            historicData.FiveY_ClaimsClo = tableData.FiveY_ClaimsClo;

            historicData.Y1ADPaid = tableData.Y1ADPaid;
            historicData.Y2ADPaid = tableData.Y2ADPaid;
            historicData.Y3ADPaid = tableData.Y3ADPaid;
            historicData.Y4ADPaid = tableData.Y4ADPaid;
            historicData.Y5ADPaid = tableData.Y5ADPaid;
            historicData.ThreeY_AD = tableData.ThreeY_AD;
            historicData.FiveY_AD = tableData.FiveY_AD;

            historicData.Y1FTPaid = tableData.Y1FTPaid;
            historicData.Y2FTPaid = tableData.Y2FTPaid;
            historicData.Y3FTPaid = tableData.Y3FTPaid;
            historicData.Y4FTPaid = tableData.Y4FTPaid;
            historicData.Y5FTPaid = tableData.Y5FTPaid;
            historicData.ThreeY_FT = tableData.ThreeY_FT;
            historicData.FiveY_FT = tableData.FiveY_FT;

            historicData.Y1TPPD = tableData.Y1TPPD;
            historicData.Y2TPPD = tableData.Y2TPPD;
            historicData.Y3TPPD = tableData.Y3TPPD;
            historicData.Y4TPPD = tableData.Y4TPPD;
            historicData.Y5TPPD = tableData.Y5TPPD;
            historicData.ThreeY_TPPD = tableData.ThreeY_TPPD;
            historicData.FiveY_TPPD = tableData.FiveY_TPPD;

            historicData.Y1TPCH = tableData.Y1TPCH;
            historicData.Y2TPCH = tableData.Y2TPCH;
            historicData.Y3TPCH = tableData.Y3TPCH;
            historicData.Y4TPCH = tableData.Y4TPCH;
            historicData.Y5TPCH = tableData.Y5TPCH;
            historicData.ThreeY_TPCH = tableData.ThreeY_TPCH;
            historicData.FiveY_TPCH = tableData.FiveY_TPCH;

            historicData.Y1TPPI = tableData.Y1TPPI;
            historicData.Y2TPPI = tableData.Y2TPPI;
            historicData.Y3TPPI = tableData.Y3TPPI;
            historicData.Y4TPPI = tableData.Y4TPPI;
            historicData.Y5TPPI = tableData.Y5TPPI;
            historicData.ThreeY_TPPI = tableData.ThreeY_TPPI;
            historicData.FiveY_TPPI = tableData.FiveY_TPPI;

            historicData.Y1ADOS = tableData.Y1ADOS;
            historicData.Y2ADOS = tableData.Y2ADOS;
            historicData.Y3ADOS = tableData.Y3ADOS;
            historicData.Y4ADOS = tableData.Y4ADOS;
            historicData.Y5ADOS = tableData.Y5ADOS;
            historicData.ThreeY_ADOS = tableData.ThreeY_ADOS;
            historicData.FiveY_ADOS = tableData.FiveY_ADOS;

            historicData.Y1FTOS = tableData.Y1FTOS;
            historicData.Y2FTOS = tableData.Y2FTOS;
            historicData.Y3FTOS = tableData.Y3FTOS;
            historicData.Y4FTOS = tableData.Y4FTOS;
            historicData.Y5FTOS = tableData.Y5FTOS;
            historicData.ThreeY_FTOS = tableData.ThreeY_FTOS;
            historicData.FiveY_FTOS = tableData.FiveY_FTOS;

            historicData.Y1TPPDOS = tableData.Y1TPPDOS;
            historicData.Y2TPPDOS = tableData.Y2TPPDOS;
            historicData.Y3TPPDOS = tableData.Y3TPPDOS;
            historicData.Y4TPPDOS = tableData.Y4TPPDOS;
            historicData.Y5TPPDOS = tableData.Y5TPPDOS;
            historicData.ThreeY_TPPDOS = tableData.ThreeY_TPPDOS;
            historicData.FiveY_TPPDOS = tableData.FiveY_TPPDOS;

            historicData.Y1TPCHOS = tableData.Y1TPCHOS;
            historicData.Y2TPCHOS = tableData.Y2TPCHOS;
            historicData.Y3TPCHOS = tableData.Y3TPCHOS;
            historicData.Y4TPCHOS = tableData.Y4TPCHOS;
            historicData.Y5TPCHOS = tableData.Y5TPCHOS;
            historicData.ThreeY_TPCHOS = tableData.ThreeY_TPCHOS;
            historicData.FiveY_TPCHOS = tableData.FiveY_TPCHOS;

            historicData.Y1TPPIOS = tableData.Y1TPPIOS;
            historicData.Y2TPPIOS = tableData.Y2TPPIOS;
            historicData.Y3TPPIOS = tableData.Y3TPPIOS;
            historicData.Y4TPPIOS = tableData.Y4TPPIOS;
            historicData.Y5TPPIOS = tableData.Y5TPPIOS;
            historicData.ThreeY_TPPIOS = tableData.ThreeY_TPPIOS;
            historicData.FiveY_TPPIOS = tableData.FiveY_TPPIOS;

            historicData.Y1Total = tableData.Y1Total;
            historicData.Y2Total = tableData.Y2Total;
            historicData.Y3Total = tableData.Y3Total;
            historicData.Y4Total = tableData.Y4Total;
            historicData.Y5Total = tableData.Y5Total;
            historicData.ThreeY_Total = tableData.ThreeY_Total;
            historicData.FiveY_Total = tableData.FiveY_Total;

            return historicData;
        }


    }
}
