using Assessment.Model;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;
//using System.Reflection.Metadata;

namespace Assessment.Services;

public class PdfService
{
    public byte[] GenerateReceipt(Payment payment, Submission submission, Form form, string? userEmail)
    {
        var bytes = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A4);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header()
                    .Text("Payment Receipt")
                    .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                page.Content().Column(col =>
                {
                    col.Item().Text($"Receipt ID: {payment.Id}");
                    col.Item().Text($"Date (UTC): {payment.CreatedAtUtc:yyyy-MM-dd HH:mm:ss}");
                    col.Item().Text($"User: {userEmail}");
                    col.Item().Text($"Form: {form.Title}");
                    col.Item().Text($"Payment Provider: {payment.Provider}");
                    col.Item().Text($"Provider Payment Id: {payment.ProviderPaymentId}");
                    col.Item().Text($"Status: {payment.Status}");
                    col.Item().Text($"Amount: {FormatAmount(payment.AmountMinor, payment.Currency)}");
                });

                page.Footer().AlignCenter().Text("Thank you!");
            });
        }).GeneratePdf();

        return bytes;
    }

    private static string FormatAmount(long amountMinor, string currency)
    {
        var amount = amountMinor / 100m;
        return $"{amount.ToString("N2", CultureInfo.InvariantCulture)} {currency.ToUpperInvariant()}";
    }
}


//namespace Assessment.Services
//{
//    public class PdfService
//    {
//    }
//}
