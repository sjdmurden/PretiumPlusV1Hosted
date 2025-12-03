using CSV_reader.database;
using CSV_reader.Models;
using CSV_reader.Services;
using Newtonsoft.Json.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Security.Claims;


namespace CSV_reader.Services
{
    public class PDFService : IPDFService
    {
        private readonly ApplicationContext _appContext;
        private readonly IWebHostEnvironment _env;

        public PDFService(ApplicationContext appContext, IWebHostEnvironment env)
        {
            _appContext = appContext;
            _env = env;
        }

        public byte[] CreatePDFReport(
            string userEmail,
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
            )
        {
            Console.WriteLine($"CreatePDFReport inside PDFService:");
            Console.WriteLine($"batchId: {batchId}");
            Console.WriteLine($"updatedGrossPremium: {updatedGrossPremiumPlusIPT}");

            //var tableData = _appContext.ClientDetails.FirstOrDefault(x => x.Id == quoteId);
            var batchOfClaims = _appContext.ClaimsTable
                                    .Where(c => c.BatchId == batchId)
                                    .ToList();

            var quoteId = batchId.Split("_")[0];

            if (batchOfClaims == null)
            {
                throw new Exception($"No data found for QuoteId: {batchId}");
            }

            var clientData = _appContext.ClaimsTable
                .Where(c => c.BatchId == batchId)
                .Select(c => new
                {
                    c.ClientName,
                    c.ClientStartDate,
                    c.ClientEndDate,
                    c.ClientCoverType,
                    c.ClientExcess,
                    c.CarNums,
                    c.CarExposure,
                    c.VanNums,
                    c.VanExposure,
                    c.MinibusNums,
                    c.MinibusExposure,
                    c.HGVNums,
                    c.HGVExposure
                })
                .FirstOrDefault();

            if (clientData == null)
            {
                throw new Exception($"No data found for Batch ID: {batchId}");
            }

            var clientName = clientData.ClientName ?? "Unknown Client";
            var clientStartDate = clientData.ClientStartDate ?? "Unknown Start Date";
            var clientEndDate = clientData.ClientEndDate;
            var clientCoverType = clientData.ClientCoverType ?? "Unknown Cover Type";
            var clientExcess = clientData.ClientExcess;
            var carNums = clientData.CarNums;
            var carExp = clientData.CarExposure;
            var vanNums = clientData.VanNums;
            var vanExp = clientData.VanExposure;
            var minibusNums = clientData.MinibusNums;
            var minibusExp = clientData.MinibusExposure;
            var hgvNums = clientData.HGVNums;
            var hgvExp = clientData.HGVExposure;

            var jsonFilePath = Path.Combine(_env.ContentRootPath, "AdditionalMaterial", "clientDetails.json");
            var json = System.IO.File.ReadAllText(jsonFilePath);
            var jsonObj = JObject.Parse(json);

            string clientAddress = "Unknown Address";
            string clientRegNum = "Unknown Registration Number";
            string clientVATNum = "Unknown VAT Number";
            // the following checks if the client name is not null or empty
            // then after the &&, we try to get a value from the jsonObj dictionary using the clientName key. 
            // In this case the JToken will hold the JObject of Address, RegNum and VatNum
            // This is then assigned to the clientObject variable
            if (!string.IsNullOrEmpty(clientName) && jsonObj.TryGetValue(clientName, out JToken? clientObject))
            {
                clientAddress = clientObject["Address"]?.ToString() ?? "Unknown Address";
                clientRegNum = clientObject["RegistrationNum"]?.ToString() ?? "Unknown Registration Number";
                clientVATNum = clientObject["VATNum"]?.ToString() ?? "Unknown VAT Number";
            }


            var pretiumLogo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "pretium+nobackground.png");

            var currentDate = DateTime.Now.ToString("dd MMMM yyyy");


            // Define a new PDF document
            var quoteDocument = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header().Padding(5).Column(headerCol =>
                    {
                        headerCol.Item().Row(row =>
                        {
                            var pretiumLogo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "pretium_nobackground.png");
                            var logoBytes = File.ReadAllBytes(pretiumLogo);

                            row.RelativeItem().AlignLeft().Column(column =>
                            {
                                column.Item().PaddingBottom(10).Text("Fleet Insurance Quote")
                                    .FontSize(16)
                                    .Bold();
                                column.Item().Text($"{clientName}")
                                    .FontSize(16)
                                    .Bold();
                                column.Item().PaddingBottom(10).Text($"PAQ - {quoteId}")
                                    .FontSize(12)
                                    .Bold();
                                column.Item().Text($"{currentDate}")
                                    .FontSize(12)
                                    .Bold();
                                column.Item().Text($"{userEmail}")
                                    .FontSize(12)
                                    .Bold();
                            });

                            row.RelativeItem().AlignRight().Column(column =>
                            {
                                column.Item().PaddingBottom(10).Text("Pretium Agency")
                                    .FontSize(12)
                                    .Bold()
                                    .FontColor(QuestPDF.Helpers.Colors.Blue.Medium);

                                column.Item()
                                    .Height(40)
                                    .AlignRight()
                                    .Image(Image.FromBinaryData(logoBytes))
                                    .FitArea();
                            });
                        });


                        headerCol.Item().PaddingTop(10).LineHorizontal(1).LineColor(QuestPDF.Helpers.Colors.Grey.Lighten2);
                    });



                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(column =>
                    {


                        // Client Details Card
                        column.Item().PaddingVertical(15)
                            .Background(QuestPDF.Helpers.Colors.Grey.Lighten4)
                            .Padding(15)
                            .Column(card =>
                            {
                                // Card header
                                card.Item().PaddingBottom(10).Text("Client Details")
                                    .FontSize(14)
                                    .Bold()
                                    .FontColor(QuestPDF.Helpers.Colors.Blue.Darken4);

                                // Client info table with alternating row colors
                                card.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                    });

                                    // Data entries
                                    string[,] clientDetails =
                                    {
                                        { "Company Name", clientName },
                                        { "Company Address", clientAddress },
                                        { "Registration Number", clientRegNum },
                                        { "VAT Number", clientVATNum },
                                        { "Contact Details", "Dean M - 07436 423050 - alec.mason@gmail.com" }
                                    };

                                    var rowColors = new[] { QuestPDF.Helpers.Colors.White, QuestPDF.Helpers.Colors.Grey.Lighten3 };

                                    for (int i = 0; i < clientDetails.GetLength(0); i++)
                                    {
                                        var bgColor = rowColors[i % 2];

                                        table.Cell().Background(bgColor).PaddingVertical(6).PaddingHorizontal(5)
                                            .Text(clientDetails[i, 0])
                                            .FontSize(11)
                                            .SemiBold();

                                        table.Cell().Background(bgColor).PaddingVertical(6).PaddingHorizontal(5)
                                            .Text(clientDetails[i, 1])
                                            .FontSize(11);
                                    }
                                });

                                // Optional: small spacing after the card
                                card.Spacing(10);
                            });

                        // Quote Specifics Card
                        column.Item().PaddingVertical(15)
                            .Background(QuestPDF.Helpers.Colors.Grey.Lighten4)
                            .Padding(15)
                            .Column(card =>
                            {
                                // Card header
                                card.Item().PaddingBottom(10).Text("Quote Specifics")
                                    .FontSize(14)
                                    .Bold()
                                    .FontColor(QuestPDF.Helpers.Colors.Blue.Darken4);

                                // Client info table with alternating row colors
                                card.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                    });

                                    // Data entries
                                    string[,] quoteSpecifics =
                                    {
                                        { "Policy Start Date", clientStartDate },
                                        { "Cover", clientCoverType },
                                        { "Excess", $"£{clientExcess}" },
                                        { "Drivers", "Any Driver Aged 21+ with 2+ years' license." },
                                        { "Vehicles", "Any motor vehicle in policyholder's custody or                       control." },
                                        { "Use", "Social, Domestic & Pleasure and the Business Use of the Policyholder or any person to whom the vehicle has been hired under a hire agreement." }
                                    };

                                    var rowColors = new[] { QuestPDF.Helpers.Colors.White, QuestPDF.Helpers.Colors.Grey.Lighten3 };

                                    for (int i = 0; i < quoteSpecifics.GetLength(0); i++)
                                    {
                                        var bgColor = rowColors[i % 2];

                                        table.Cell().Background(bgColor).PaddingVertical(6).PaddingHorizontal(5)
                                            .Text(quoteSpecifics[i, 0])
                                            .FontSize(11)
                                            .SemiBold();

                                        table.Cell().Background(bgColor).PaddingVertical(6).PaddingHorizontal(5)
                                            .Text(quoteSpecifics[i, 1])
                                            .FontSize(11);
                                    }
                                });

                                // Optional: small spacing after the card
                                card.Spacing(10);
                            });


                        column.Item().PageBreak();






                        // Load the images from the file path
                        var imagePath1 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "traffic1.jpg");
                        var imagePath2 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "road.jpg");



                        // ------------ EXPOSURE -------------------

                        column.Item().Text("Exposure").FontSize(14).Bold().Underline().FontColor(QuestPDF.Helpers.Colors.Blue.Darken4);

                        column.Item().PaddingTop(10).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(); // left column
                                column.Item().PaddingVertical(5).LineHorizontal(1).LineColor(QuestPDF.Helpers.Colors.Grey.Medium);
                                columns.RelativeColumn(); // right column
                            });

                            string[] labels =
                            [
                                "Cars", "Vans", "Minibuses", "HGVs"
                            ];
                            int[] vehNums =
                            [
                                carNums, vanNums, minibusNums, hgvNums
                            ];
                            double[] vehExposures =
                            [
                                carExp, vanExp, minibusExp, hgvExp
                            ];

                            for (int i = 0; i < vehNums.Length; i++)
                            {
                                var bgColor = i % 2 == 0 ? QuestPDF.Helpers.Colors.Grey.Lighten4 : QuestPDF.Helpers.Colors.White;

                                table.Cell().Background(bgColor).Padding(4).Text(labels[i]).SemiBold();
                                table.Cell().Background(bgColor).Padding(4).Text($"{vehNums[i]} @ £{vehExposures[i]} each.").SemiBold();
                            }

                            table.Cell().Padding(4).Background(QuestPDF.Helpers.Colors.Grey.Lighten4).Text("Adjustment");
                            table.Cell().Padding(4).Background(QuestPDF.Helpers.Colors.Grey.Lighten4).Text("The annual premium will be adjusted on an annual basis calculated on the number of days as per the MID.");
                        });

                        // ------------- BUSINESS FORECAST ------------------
                        column.Item().Text("Business Forecast").FontSize(14).Bold().Underline().FontColor(QuestPDF.Helpers.Colors.Blue.Darken4);

                        column.Item().PaddingTop(10).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(); // left column
                                column.Item().PaddingVertical(5).LineHorizontal(1).LineColor(QuestPDF.Helpers.Colors.Grey.Medium);
                                columns.RelativeColumn(); // right column
                            });

                            string[] labels =
                            [
                                "Days COI", "Days Non COI", "Turnover COI", "Turnover Non COI"
                            ];
                            decimal[] values =
                            [
                                FCDaysCOI, FCDaysNonCOI, FCTurnoverCOI, FCTurnoverNonCOI
                            ];


                            for (int i = 0; i < values.Length; i++)
                            {
                                var bgColor = i % 2 == 0 ? QuestPDF.Helpers.Colors.Grey.Lighten4 : QuestPDF.Helpers.Colors.White;

                                var poundSign = i > 1 ? "£" : "";

                                table.Cell().Background(bgColor).Padding(4).Text(labels[i]).SemiBold();
                                table.Cell().Background(bgColor).Padding(4).Text($"{poundSign}{values[i]:N2}").SemiBold();
                            }

                        });


                        column.Item().PageBreak();


                        // -------------------- NEW PAGE ------------------------

                        // ------------------- ULTIMATE COSTS -------------------

                        column.Item().Text("Ultimate Costs").FontSize(14).Bold().Underline().FontColor(QuestPDF.Helpers.Colors.Blue.Darken4);

                        column.Item().PaddingTop(10).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            string[] labels =
                            [
                                "Net Premium", "Commissions",
                                "Gross Premium", "Gross Premium Plus IPT"
                            ];

                            decimal[] values =
                            [
                                netPremium, commissions,
                                grossPremium, updatedGrossPremiumPlusIPT
                            ];

                            for (int i = 0; i < labels.Length; i++)
                            {
                                var bgColor = i % 2 == 0 ? QuestPDF.Helpers.Colors.Grey.Lighten4 : QuestPDF.Helpers.Colors.White;

                                table.Cell().Background(bgColor).Padding(4).Text(labels[i]).SemiBold();
                                table.Cell().Background(bgColor).Padding(4).AlignRight()
                                .Text($"£{values[i]:N2}").SemiBold();
                            }
                        });


                        // Notes section
                        column.Item().PaddingTop(20).Border(1).Padding(15).Column(innerColumn =>
                        {
                            innerColumn.Item().Text("Notes:")
                                .FontSize(16)
                                .Bold()
                                .FontColor("#333333")
                                .AlignLeft();

                            innerColumn.Item().Text(adjustmentNotes)
                                .FontSize(12)
                                .FontColor("#666666")
                                .AlignLeft();

                        });



                        // -------------------- NEW PAGE ------------------------

                        // Endorsements section
                        column.Item().PaddingTop(10).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(); // left column
                                columns.RelativeColumn(); // right column
                            });

                            table.Cell().Text("This policy is based on our standard Policy Wording subject to the following Endorsements:");
                            table.Cell().Text("");

                            table.Cell().PaddingTop(10).Text("Endorsement 1:");
                            table.Cell().PaddingTop(10).Text("...........");

                            table.Cell().PaddingTop(10).Text("Endorsement 2:");
                            table.Cell().PaddingTop(10).Text("...........");

                            table.Cell().PaddingTop(10).Text("Endorsement 3:");
                            table.Cell().PaddingTop(10).Text("...........");

                            column.Item().PaddingTop(10).Text("Should you wish to discuss any aspect of this quotation, please contact ............ by telephone on ............. Otherwise we look forward to your instructions to incept cover.")
                                .FontSize(10).Italic();

                            column.Item().PaddingTop(10).Text("Yours faithfully,");
                            column.Item().PaddingTop(10).Text("Adrian Curd");
                            column.Item().PaddingTop(10).Text("Underwriter");
                        });
                    });

                    // Footer section
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Page ");
                        text.CurrentPageNumber();
                    });
                });
            });



            // Generate the PDF file as a byte array
            // GeneratePdf() is the library's own method to generate the PDF
            return quoteDocument.GeneratePdf();
        }




        // -------------------------- POLICY DOC ------------------------
        public byte[] CreatePolicyDoc(
                string userEmail,
                string batchId,
                int policyNumber,
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
            )
        {

            var batchOfClaims = _appContext.ClaimsTable
                                    .Where(c => c.BatchId == batchId)
                                    .ToList();

            var quoteId = batchId.Split("_")[0];

            if (batchOfClaims == null)
            {
                throw new Exception($"No data found for QuoteId: {batchId}");
            }

            var clientData = _appContext.ClaimsTable
                .Where(c => c.BatchId == batchId)
                .Select(c => new
                {
                    c.ClientName,
                    c.ClientStartDate,
                    c.ClientEndDate,
                    c.ClientCoverType,
                    c.ClientExcess,
                    c.CarNums,
                    c.CarExposure,
                    c.VanNums,
                    c.VanExposure,
                    c.MinibusNums,
                    c.MinibusExposure,
                    c.HGVNums,
                    c.HGVExposure
                })
                .FirstOrDefault();

            if (clientData == null)
            {
                throw new Exception($"No data found for Batch ID: {batchId}");
            }

            

            var clientName = clientData.ClientName ?? "Unknown Client";
            var clientStartDate = clientData.ClientStartDate ?? "Unknown Start Date";
            var clientEndDate = clientData.ClientEndDate;
            var clientCoverType = clientData.ClientCoverType ?? "Unknown Cover Type";
            var clientExcess = clientData.ClientExcess;
            var carNums = clientData.CarNums;
            var carExp = clientData.CarExposure;
            var vanNums = clientData.VanNums;
            var vanExp = clientData.VanExposure;
            var minibusNums = clientData.MinibusNums;
            var minibusExp = clientData.MinibusExposure;
            var hgvNums = clientData.HGVNums;
            var hgvExp = clientData.HGVExposure;

            var currentDate = DateTime.Now.ToString("dd MMMM yyyy");


            // --------------- DOCUMENT START ------------------------------------
            var policyDocument = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header()
                        .Text("Certificate of Motor Insurance")
                        .FontSize(30).Bold();

                    // put horizontal line here -------------------------

                    // ------------------- PAGE 1 --------------------------------
                    page.Content().Column(column =>
                    {
                        const float nestingSize = 25;
                        column.Spacing(10);

                        column.Item()
                            .Text($"POLICY NUMBER: {policyNumber}")
                            .FontSize(11)
                            .FontColor(Colors.Blue.Darken2);

                        column.Item()
                            .Text($"QUOTE NUMBER: {quoteId}")
                            .FontSize(11)
                            .FontColor(Colors.Blue.Darken2);

                        // 1
                        AddListItem(column, 0, "1.", "Description of vehicle(s)");
                        AddListItem(column, 1, "-", "Any motor vehicle the property of, or on hire or loan or lease to the policyholder");

                        // 2
                        AddListItem(column, 0, "2.", "Name of policyholder");
                        AddListItem(column, 1, "-", $"{clientName}");

                        // 3
                        AddListItem(column, 0, "3.", "Effective date and time of the commencement of insurance for the purpose of the relevant law");
                        AddListItem(column, 1, "-", $"{clientStartDate}");

                        // 4
                        AddListItem(column, 0, "4.", "Date of expiry of insurance");
                        AddListItem(column, 1, "-", $"{clientEndDate}");

                        // 5
                        AddListItem(column, 0, "5.", "Persons or classes of persons entitled to drive");
                        AddListItem(column, 1, "-", "Any person who is driving on the order or with the permission of the policyholder.");
                        AddListItem(column, 1, "-", "Providing that the person driving has a licence to drive the vehicle or has held and is not disqualified from or prohibited by law from holding or obtaining such a licence");

                        // 6
                        AddListItem(column, 0, "6.", "Limitations as to use");
                        AddListItem(column, 1, "-", "Use for social, domestic and pleasure purposes");
                        AddListItem(column, 1, "-", "Use in connection with the policyholder's business.");
                        AddListItem(column, 1, "-", "Use in connection with the business of any hirer including the carriage of goods for hire and reward.");

                        AddListItem(column, 0, "", "Unless specified under section 6 of this certificate of insurance, this policy does not cover:");
                        AddListItem(column, 0, "", "Use for hiring, the letting on hire, the carriage of passengers and goods for hire or reward, racing, pacemaking, use in any contest, reliability or speed trial or the use for any purpose in connection with the motor trade.");
                        AddListItem(column, 0, "", "I hereby certify that the policy to which this certificate of insurance relates satisfies the requirements of the relevant law applicable in Great Britain, Northern Ireland, Isle of Man and the Islands of Guernsey, Jersey and Alderney.");
                        AddListItem(column, 0, "", "For and on behalf of the Underwriter subscribing ERS, 30 Fenchurch Street, London EC3M 3BD");
                        AddListItem(column, 0, "", "Authorised Insurer");
                        AddListItem(column, 0, "", "--- signature here ---");
                        AddListItem(column, 0, "", "Active Underwriter");
                       


                        // Helper function
                        void AddListItem(ColumnDescriptor col, int nestingLevel, string bulletText, string text)
                        {
                            col.Item().Row(row =>
                            {
                                row.ConstantItem(nestingSize * nestingLevel); // indentation
                                row.ConstantItem(nestingSize).Text(bulletText).FontSize(11); // bullet
                                row.RelativeItem().Text(text).FontSize(11); // main text
                            });
                        }
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.CurrentPageNumber();
                            text.Span(" / ");
                            text.TotalPages();
                        });
                });


                // ------------------- PAGE 2 --------------------------------
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    // ----------------- HEADER -------------------
                    page.Header().Padding(5).Column(headerCol =>
                    {
                        headerCol.Item().Row(row =>
                        {
                            row.RelativeItem().AlignLeft().Column(column =>
                            {
                                column.Item().PaddingBottom(10).Text("Policy Document")
                                    .FontSize(16).Bold();
                                column.Item().Text($"{clientName}")
                                    .FontSize(16).Bold();
                                column.Item().PaddingBottom(10).Text($"Policy Number: {policyNumber}")
                                    .FontSize(12).Bold();
                                column.Item().PaddingBottom(10).Text($"Broker Agency Number: --broker num--")
                                    .FontSize(12).Bold();
                                column.Item().PaddingBottom(10).Text($"Scheme: -- scheme num here --")
                                    .FontSize(12).Bold();
                                column.Item().Text($"{currentDate}")
                                    .FontSize(12).Bold();
                            });

                            row.RelativeItem().AlignRight().Column(column =>
                            {
                                column.Item().PaddingBottom(10).Text("Pretium Agency")
                                    .FontSize(12).Bold()
                                    .FontColor(QuestPDF.Helpers.Colors.Blue.Medium);
                            });
                        });

                        headerCol.Item().PaddingTop(10)
                            .LineHorizontal(1)
                            .LineColor(QuestPDF.Helpers.Colors.Grey.Lighten2);
                    });
                    // ------------- HEADER END ------------------------------

                    
                    page.Content().PaddingTop(25).Column(column =>
                    {
                        // ---------- FIRST TABLE (Policy Details) ----------
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(50);
                                columns.RelativeColumn(50);
                            });

                            table.Header(header =>
                            {
                                header.Cell().BorderBottom(2).Padding(8).Text("Policy").Bold();
                                header.Cell().BorderBottom(2).Padding(8).Text("").Bold();
                            });

                            table.Cell().Padding(8).Text("Policyholder").Bold();
                            table.Cell().Padding(8).Text("policy holder name here");

                            table.Cell().Padding(8).Text("Address").Bold();
                            table.Cell().Padding(8).Text("address here");

                            table.Cell().Padding(8).Text("Commencement date and time").Bold();
                            table.Cell().Padding(8).Text("start date here");

                            table.Cell().Padding(8).Text("Expiry Date").Bold();
                            table.Cell().Padding(8).Text("end date here");

                            table.Cell().Padding(8).Text("Reason for issue").Bold();
                            table.Cell().Padding(8).Text("reason here");

                            table.Cell().Padding(8).Text("Declaration Frequency").Bold();
                            table.Cell().Padding(8).Text("declaration here");
                        });

                        // some spacing between tables
                        column.Item().PaddingTop(25);





                        // ---------- SECOND TABLE (Premium Details) ----------
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(50);
                                columns.RelativeColumn(50);
                            });

                            table.Header(header =>
                            {
                                header.Cell().BorderBottom(2).Padding(8).Text("Premium").Bold();
                                header.Cell().BorderBottom(2).Padding(8).Text("").Bold();
                            });

                            table.Cell().Padding(8).Text("Premium (excluding IPT)").Bold();
                            table.Cell().Padding(8).Text("gross premium here");

                            table.Cell().Padding(8).Text("IPT").Bold();
                            table.Cell().Padding(8).Text("IPT here");

                            table.Cell().Padding(8).Text("Total Premium Due").Bold();
                            table.Cell().Padding(8).Text("gross premium plus IPT here");
                        });

                        // some spacing between tables
                        column.Item().PaddingTop(25);

                        // ------------------- VEHICLE DETAILS TABLE -------------------

                        column.Item().DefaultTextStyle(x => x.FontSize(7)).Table(table =>
                        {                            

                            // define cols for table
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(50);
                                columns.RelativeColumn(50);
                                columns.RelativeColumn(50);
                                columns.RelativeColumn(50);
                                columns.RelativeColumn(50);
                                columns.RelativeColumn(50);
                                columns.RelativeColumn(50);
                                columns.RelativeColumn(50);
                                columns.RelativeColumn(50);
                            });

                            // define headers for table
                            table.Header(header =>
                            {
                                header.Cell().BorderBottom(2).Padding(8).Text("Vehicle Type").Bold();
                                header.Cell().BorderBottom(2).Padding(8).Text("Vehicle Numbers").Bold();
                                header.Cell().BorderBottom(2).Padding(8).Text("Registration Number").Bold();
                                header.Cell().BorderBottom(2).Padding(8).Text("CC").Bold();
                                header.Cell().BorderBottom(2).Padding(8).Text("GVW").Bold();
                                header.Cell().BorderBottom(2).Padding(8).Text("No. of Seats").Bold();
                                header.Cell().BorderBottom(2).Padding(8).Text("Cover").Bold();
                                header.Cell().BorderBottom(2).Padding(8).Text("Class of Use").Bold();
                                header.Cell().BorderBottom(2).Padding(8).Text("Annual rate per vehicle(excl. IPT").Bold();
                            });

                            // each chunk here is a row
                            // essentially each line is a cell in the table, going left to right, and overlaps to the next row
                            table.Cell().Padding(8).Text("Cars");
                            table.Cell().Padding(8).Text("car nums");
                            table.Cell().Padding(8).Text("All");
                            table.Cell().Padding(8).Text("");
                            table.Cell().Padding(8).Text("gvw?");
                            table.Cell().Padding(8).Text("");
                            table.Cell().Padding(8).Text("cover type here");
                            table.Cell().Padding(8).Text("class of use?");
                            table.Cell().Padding(8).Text("vehicle exposure rating?");

                            table.Cell().Padding(8).Text("Vans");
                            table.Cell().Padding(8).Text("van nums");
                            table.Cell().Padding(8).Text("All");
                            table.Cell().Padding(8).Text("");
                            table.Cell().Padding(8).Text("gvw?");
                            table.Cell().Padding(8).Text("");
                            table.Cell().Padding(8).Text("cover type here");
                            table.Cell().Padding(8).Text("class of use?");
                            table.Cell().Padding(8).Text("vehicle exposure rating?");

                            table.Cell().Padding(8).Text("HGVs");
                            table.Cell().Padding(8).Text("HGV nums");
                            table.Cell().Padding(8).Text("All");
                            table.Cell().Padding(8).Text("");
                            table.Cell().Padding(8).Text("gvw?");
                            table.Cell().Padding(8).Text("");
                            table.Cell().Padding(8).Text("cover type here");
                            table.Cell().Padding(8).Text("class of use?");
                            table.Cell().Padding(8).Text("vehicle exposure rating?");

                            table.Cell().Padding(8).Text("Minibuses");
                            table.Cell().Padding(8).Text("mbus nums");
                            table.Cell().Padding(8).Text("All");
                            table.Cell().Padding(8).Text("");
                            table.Cell().Padding(8).Text("gvw?");
                            table.Cell().Padding(8).Text("");
                            table.Cell().Padding(8).Text("cover type here");
                            table.Cell().Padding(8).Text("class of use?");
                            table.Cell().Padding(8).Text("vehicle exposure rating?");

                        });
                    });

                    // --------------------- END OF PAGE 2 ---------------------

                });

            });

            // ------------------ DOCUMENT END --------------------------------

            return policyDocument.GeneratePdf();
        }
    };







}


/*var document = Document.Create(container =>
{
    // Header section of the PDF
    container.Page(page =>
    {
        page.Size(PageSizes.A4);
        page.Margin(2, Unit.Centimetre);
        page.PageColor(Colors.White);
        page.DefaultTextStyle(x => x.FontSize(20));

        page.Header()
            .AlignCenter()
            .Text($"{pdfDataModel.ClientName}")
            .FontSize(25)
            .SemiBold()
            .FontColor(Colors.Red.Medium);
        page.Content().Text("Fleet Insurance Quote");

        page.Content()
        .PaddingVertical(1, Unit.Centimetre)
        .Column(x =>
        {
            x.Spacing(20);

            x.Item().Text(Placeholders.LoremIpsum());
            x.Item().Image(Placeholders.Image(200, 100));
        });

        // Add a footer with the page number
        page.Footer()
        .AlignCenter()
        .Text(x =>
        {
            x.Span("Page ");
            x.CurrentPageNumber();
        });
    });
});*/
