using Humanizer;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;
using WebApi.Data;
using WebApi.Entities;

namespace WebApi.Handlers;

public class PdfHandler
{
    private readonly IConfiguration _configuration;
    private readonly string _storageLocation;
    private readonly ApplicationDbContext _context;
    
    public PdfHandler(IConfiguration configuration, ApplicationDbContext context)
    {
        _configuration = configuration;
        _storageLocation = _configuration.GetValue<string>("StorageLocation" + "/luckystrikeStorage/Pdf/");
        _context = context;
        QuestPDF.Settings.License = LicenseType.Community;
    }
    
    public async Task CreatePdf(int reservationId)
    {
        //get reservation
        var reservation = _context.Reservations
            .Include(reservation => reservation.Orders)
            .ThenInclude(order => order.ProductOrders)
            .Include(reservation => reservation.Lanes)
            .ThenInclude(lane => lane.Timeframe)
            .FirstOrDefault(x => x.Id == reservationId);
        var products = _context.Products.ToList();
        
        if (reservation == null || products == null)
        {
            // Handle the case where the reservation is not found.
            return;
        }
        
        //create document and generate pdf
        var document = new InvoiceDocument(reservation,products);
        //save pdf
       document.GeneratePdf(Path.Combine(Environment.CurrentDirectory, $"luckystrikeStorage/pdf/rekening-{reservationId}.pdf"));
    }
    
    public class InvoiceDocument : IDocument
    {
        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public DocumentSettings GetSettings() => DocumentSettings.Default;
        public Reservation Reservation { get; set; }
        public List<Product> Products { get; set; }
        public double Price { get; set; }

        public InvoiceDocument(Reservation reservation, List<Product> products)
        {
            Reservation = reservation;
            Products = products;
        }
        // FIll the document with content
        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    //Page settings
                    page.Margin(50);
                    page.Size(PageSizes.A4);
                    page.Background(Colors.Grey.Lighten2);
            
                    //generate Header
                    page.Header().Element(ComposeHeader);
                    //generate content
                    page.Content().Element(ComposeTable);
                });
        }

        void ComposeHeader(IContainer container)
        {
            var titleStyle = TextStyle.Default.FontSize(20).SemiBold().FontColor(Colors.Red.Darken3);
    
            container.Row(row =>
            {
                //add header text
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text($"Factuur {Reservation.Id}").Style(titleStyle);

                    column.Item().Text(text =>
                    {
                        text.Span("Datum: ").SemiBold();
                        text.Span($"{DateTime.Now:d}");
                        
                    });
                    
                });
                    //add logo
                row.ConstantItem(60).AlignRight().Image("luckystrikeStorage/logo/luckystrike-logo.png");
            });
        }

        void ComposeTable(IContainer container)
        {
            container.Table(table =>
            {
                // set table header and column width
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });
            
                //set table headers
                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("#");
                    header.Cell().Element(CellStyle).Text("Product");
                    header.Cell().Element(CellStyle).AlignRight().Text("Aantal");
                    header.Cell().Element(CellStyle).AlignRight().Text("Prijs");
                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });
                // loop for each reservation if day is weekend
                if (Reservation.ReservationDate.DayOfWeek == DayOfWeek.Friday || Reservation.ReservationDate.DayOfWeek == DayOfWeek.Saturday || Reservation.ReservationDate.DayOfWeek == DayOfWeek.Sunday)
                {
                  //loop for each lane
                    foreach (var lane in Reservation.Lanes)
                    {
                        //add lane number to table
                        table.Cell().Element(CellStyle).Text(Reservation.Lanes.IndexOf(lane) + 1);
                        table.Cell().Element(CellStyle).Text($"Bowlingbaan {lane.LaneNumber}");
                        //get timeframe of lane
                        var timeFrame = lane.Timeframe;
                        double cost = 0;
                        var hours = 0;
                        //loop for each time slot
                        foreach (var timeSlot in timeFrame)
                        {
                            //increment hours and add cost to total price based on time
                            hours += 1;
                            if (timeSlot.Time <= 17)
                            {
                                cost += 28;
                            }
                            else
                            {
                                cost += 33.50;
                            }

                            table.Cell().Element(CellStyle).AlignRight().Text(hours);
                            table.Cell().Element(CellStyle).AlignRight().Text($"\u20ac{cost}");
                            Price += cost;
                        }
                    }
                    static IContainer CellStyle(IContainer container)
                    {
                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(10);
                    }
                }
                //if day is not weekend
                else
                {
                    //loop for each lane
                    foreach (var lane in Reservation.Lanes)
                    {
                        //add lane number to table
                        table.Cell().Element(CellStyle).Text($"Baan: {Reservation.Lanes.IndexOf(lane) + 1}");
                        table.Cell().Element(CellStyle).Text($"Bowlingbaan {lane.LaneNumber}");
                        //get timeframe of lane
                        var timeFrame = lane.Timeframe;
                        table.Cell().Element(CellStyle).AlignRight().Text($"{timeFrame.Count} uur");
                        table.Cell().Element(CellStyle).AlignRight().Text($"\u20ac{timeFrame.Count * 24}");
                        //increment hours and add cost to total price based
                        Price += timeFrame.Count * 24;
                        static IContainer CellStyle(IContainer container)
                        {
                            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                        }
                    }
                }
                    //check if there are orders
                if (Reservation.Orders.Count != 0)
                {
                    //loop for each order
                    foreach (var order in Reservation.Orders)
                    {
                        //add order number to table
                        table.Cell().Element(CellStyle).Text($"Bestelling: {Reservation.Orders.IndexOf(order) + 1}");
                        table.Cell().Element(CellStyle).Text("");
                        table.Cell().Element(CellStyle).Text("");
                        table.Cell().Element(CellStyle).Text("");
                        //loop for each product in order
                        foreach (var productOrder in order.ProductOrders)
                        {
                            //add product to table
                            table.Cell().Element(CellStyle).Text("");
                            table.Cell().Element(CellStyle).Text($"{productOrder.Product.Name}");
                            table.Cell().Element(CellStyle).AlignRight().Text($"{productOrder.Amount}");
                            table.Cell().Element(CellStyle).AlignRight().Text($"\u20ac{productOrder.Product.Price * productOrder.Amount}");
                            //add price to total price
                            Price += productOrder.Product.Price * productOrder.Amount;
                        } 
                        static IContainer CellStyle(IContainer container)
                        {
                            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                        } 
                    }
                }
                //align total price to right
                table.Cell().Element(TotalePrice).Text("");
                table.Cell().Element(TotalePrice).Text("");
                table.Cell().Element(TotalePrice).Text("");
                //add total price to table
                table.Cell().Element(TotalePrice).AlignRight().Text($"Totaal: \u20ac{Price}");
                static IContainer TotalePrice(IContainer container)
                {
                    return container.Background(Colors.Grey.Lighten2);
                }   
            });
        }
    }
}