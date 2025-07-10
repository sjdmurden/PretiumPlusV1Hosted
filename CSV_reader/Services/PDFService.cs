using CSV_reader.database;
using CSV_reader.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Newtonsoft.Json.Linq;



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

            var clientName = _appContext.ClaimsTable
                        .Where(c => c.BatchId == batchId)
                        .Select(c => c.ClientName)
                        .FirstOrDefault() ?? "Unknown Client";
            var clientStartDate = _appContext.ClaimsTable
                        .Where(c => c.BatchId == batchId)
                        .Select(c => c.ClientStartDate)
                        .FirstOrDefault() ?? "Unknown Start Date";
            var clientEndDate = _appContext.ClaimsTable
                        .Where(c => c.BatchId == batchId)
                        .Select(c => c.ClientEndDate)
                        .FirstOrDefault();
            var clientCoverType = _appContext.ClaimsTable
                        .Where(c => c.BatchId == batchId)
                        .Select(c => c.ClientCoverType)
                        .FirstOrDefault() ?? "Unknown Cover Type";
            var clientExcess = _appContext.ClaimsTable
                        .Where(c => c.BatchId == batchId)
                        .Select(c => c.ClientExcess)
                        .FirstOrDefault();
            var carNums = _appContext.ClaimsTable
                        .Where(c => c.BatchId == batchId)
                        .Select(c => c.CarNums)
                        .FirstOrDefault();
            var carExp = _appContext.ClaimsTable
                        .Where(c => c.BatchId == batchId)
                        .Select(c => c.CarExposure)
                        .FirstOrDefault();
            var vanNums = _appContext.ClaimsTable
                        .Where(c => c.BatchId == batchId)
                        .Select(c => c.VanNums)
                        .FirstOrDefault();
            var vanExp = _appContext.ClaimsTable
                        .Where(c => c.BatchId == batchId)
                        .Select(c => c.VanExposure)
                        .FirstOrDefault();
            var minibusNums = _appContext.ClaimsTable
                        .Where(c => c.BatchId == batchId)
                        .Select(c => c.MinibusNums)
                        .FirstOrDefault();
            var minibusExp = _appContext.ClaimsTable
                        .Where(c => c.BatchId == batchId)
                        .Select(c => c.MinibusExposure)
                        .FirstOrDefault();
            var hgvNums = _appContext.ClaimsTable
                        .Where(c => c.BatchId == batchId)
                        .Select(c => c.HGVNums)
                        .FirstOrDefault();
            var hgvExp = _appContext.ClaimsTable
                        .Where(c => c.BatchId == batchId)
                        .Select(c => c.HGVExposure)
                        .FirstOrDefault();

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
            var document = Document.Create(container =>
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
                            var pretiumLogo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "pretium+nobackground.png");
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


                        // Quote Specifics Card
                        /*   column.Item().PaddingTop(20).Background(QuestPDF.Helpers.Colors.Grey.Lighten4).Padding(15)
                               .Column(card =>
                               {
                                   card.Item().Text("Quote Details")
                                       .FontSize(14).Bold().FontColor(QuestPDF.Helpers.Colors.Blue.Darken4);

                                   card.Item().Table(table =>
                                   {
                                       table.ColumnsDefinition(columns =>
                                       {
                                           columns.RelativeColumn();
                                           columns.RelativeColumn();
                                       });

                                       table.Cell().Text("Policy Start Date").SemiBold();
                                       table.Cell().Text($"{clientStartDate}");

                                       table.Cell().Text("Cover").SemiBold();
                                       table.Cell().Text($"{clientCoverType}");

                                       table.Cell().Text("Excess").SemiBold();
                                       table.Cell().Text($"£{clientExcess}");

                                       table.Cell().Text("Drivers").SemiBold();
                                       table.Cell().Text("Any Driver Aged 21+ with 2+ years' license.");

                                       table.Cell().Text("Vehicles").SemiBold();
                                       table.Cell().Text("Any motor vehicle in policyholder's custody or control.");

                                       table.Cell().Text("Use").SemiBold();
                                       table.Cell().Text("Social, Domestic & Pleasure and the Business Use of the Policyholder or any person to whom the vehicle has been hired under a hire agreement.");
                                   });
                               });*/



                        // Load the images from the file path
                        var imagePath1 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "traffic1.jpg");
                        var imagePath2 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "road.jpg");

                        // Image section with 2 images
                        /*column.Item().PaddingTop(20).Row(row =>
                        {
                            
                            row.RelativeItem().Image(File.ReadAllBytes(imagePath1));
                            row.RelativeItem().Image(File.ReadAllBytes(imagePath2));
                        });*/

                        /*column.Item().Row(row =>
                        {
                            row.RelativeItem().Width(150).Height(200)
                                .Image(File.ReadAllBytes(imagePath1))
                                .FitHeight();

                            row.RelativeItem().Width(150).Height(200)
                                .Image(File.ReadAllBytes(imagePath2))
                                .FitHeight();
                        });*/

                        // Quote specifics 

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

                            for (int i = 0; i<vehNums.Length; i++)
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
                                "Claims Amount", "Large Loss Fund", "Reinsurance Costs", "Claims Handling Fee",
                                "Levies", "Expenses", "Profit", "Net Premium", "Commissions",
                                "Gross Premium", "Gross Premium Plus IPT"
                            ];

                            decimal[] values =
                            [
                                claimsAmount, largeLossFund, reinsuranceCosts, claimsHandlingFee,
                                levies, expenses, profit, netPremium, commissions,
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
            return document.GeneratePdf();
        }
    }

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
