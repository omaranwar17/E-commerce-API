using E_com_Core.Dtos;
using E_com_Core.Interfases;
using E_com_Core.Model.Checkout;
using Microsoft.Extensions.Options;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;

namespace E_com_Infrastructurer.Repository
{
    public class InvoiceService : IInvoiceService
    {
        private readonly InvoiceSettings _store;
        private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

        // ===== إعدادات الخطوط (عدّل من هنا فقط) =====
        private const float LineWidth = 0.5f;
        private static readonly string LineClr = Colors.Grey.Darken3;

        public InvoiceService(IOptions<InvoiceSettings> store) => _store = store.Value;

        // ===== ستايل الخلايا =====
        // رأس الجدول: الخط الوحيد الخاص بالجدول
        private static IContainer HeaderCell(IContainer c) => c
            .Background(Colors.Grey.Lighten3)
            .BorderBottom(LineWidth).BorderColor(LineClr)
            .PaddingVertical(8).PaddingHorizontal(6)
            .AlignMiddle();

        // صفوف الجدول: بدون خطوط
        private static IContainer BodyCell(IContainer c) => c
            .PaddingVertical(8).PaddingHorizontal(6)
            .AlignMiddle();

        private static string Money(decimal v) => v.ToString("N2", Inv);

        // عنوان قسم + خط تحته
        private static void SectionTitle(ColumnDescriptor col, string title) =>
            col.Item().BorderBottom(LineWidth).BorderColor(LineClr)
               .PaddingBottom(6).Text(title).Bold().FontSize(12);

        // سطر بيانات: بدون خط
        private static void InfoRow(ColumnDescriptor col, string label, string? value) =>
            col.Item().PaddingVertical(5).Row(r =>
            {
                r.ConstantItem(90).Text(label).Bold();
                r.RelativeItem().Text(string.IsNullOrWhiteSpace(value) ? "-" : value);
            });

        // سطر إجمالي: بدون خط
        private static void TotalRow(ColumnDescriptor col, string label, string value) =>
            col.Item().PaddingVertical(5).Row(r =>
            {
                r.RelativeItem().Text(label);
                r.ConstantItem(100).AlignLeft().Text(value);
            });

        public byte[] Generate(OrderDto o)
        {
            if (o.InvoiceNumber is null)
                throw new InvalidOperationException("الأوردر ملوش فاتورة.");

            var a = o.ShippingAddress;
            var area = string.Join(" - ",
                new[] { a.Governorate, a.City }.Where(x => !string.IsNullOrWhiteSpace(x)));

            return Document.Create(doc => doc.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.ContentFromRightToLeft();
                page.DefaultTextStyle(t => t
                    .FontFamily("Cairo", "Arial", "Lato")
                    .FontSize(11)
                    .FontColor(Colors.Black));

                // ================= الهيدر =================
                page.Header().Column(c =>
                {
                    c.Item().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text(_store.StoreName).FontSize(24).Bold();

                            if (!string.IsNullOrWhiteSpace(_store.StoreAddress))
                                col.Item().PaddingTop(4).Text(_store.StoreAddress);

                            if (!string.IsNullOrWhiteSpace(_store.StorePhone))
                                col.Item().Text(t =>
                                {
                                    t.Span("هاتف: ");
                                    t.Span(_store.StorePhone);
                                });
                        });

                        row.ConstantItem(170).Column(col =>
                        {
                            col.Item().AlignLeft().Text("فاتورة").FontSize(32).Bold();
                            col.Item().AlignLeft().Text("INVOICE").FontSize(11).LetterSpacing(0.2f);
                        });
                    });

                    c.Item().PaddingTop(12).LineHorizontal(LineWidth).LineColor(LineClr);
                });

                // ================= المحتوى =================
                page.Content().PaddingTop(24).Column(c =>
                {
                    c.Spacing(28);

                    // ---- العميل + تفاصيل الفاتورة ----
                    c.Item().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            SectionTitle(col, "فاتورة إلى");
                            InfoRow(col, "الاسم", a.FullName);
                            InfoRow(col, "الموبايل", a.Phone);
                            InfoRow(col, "المنطقة", area);
                            InfoRow(col, "العنوان", a.Street);
                        });

                        row.ConstantItem(40);

                        row.RelativeItem().Column(col =>
                        {
                            SectionTitle(col, "تفاصيل الفاتورة");
                            InfoRow(col, "رقم الفاتورة", o.InvoiceNumber);
                            InfoRow(col, "تاريخ الإصدار", $"{o.InvoiceDate:yyyy-MM-dd}");
                            InfoRow(col, "رقم الطلب", o.Id.ToString());
                            InfoRow(col, "طريقة الدفع", o.PaymentMethod?.ToString());
                        });
                    });

                    // ---- جدول المنتجات ----
                    c.Item().Table(t =>
                    {
                        t.ColumnsDefinition(cd =>
                        {
                            cd.ConstantColumn(30);   // #
                            cd.RelativeColumn(4);    // المنتج
                            cd.RelativeColumn(1.3f); // المقاس
                            cd.RelativeColumn(1.1f); // الكمية
                            cd.RelativeColumn(2);    // السعر
                            cd.RelativeColumn(2);    // الإجمالي
                        });

                        t.Header(h =>
                        {
                            h.Cell().Element(HeaderCell).AlignCenter().Text("#").Bold();
                            h.Cell().Element(HeaderCell).Text("المنتج").Bold();
                            h.Cell().Element(HeaderCell).AlignCenter().Text("المقاس").Bold();
                            h.Cell().Element(HeaderCell).AlignCenter().Text("الكمية").Bold();
                            h.Cell().Element(HeaderCell).AlignCenter().Text("السعر (ج.م)").Bold();
                            h.Cell().Element(HeaderCell).AlignCenter().Text("الإجمالي (ج.م)").Bold();
                        });

                        var index = 1;
                        foreach (var i in o.Items)
                        {
                            t.Cell().Element(BodyCell).AlignCenter().Text(index++.ToString());
                            t.Cell().Element(BodyCell).Text(i.ProductName);
                            t.Cell().Element(BodyCell).AlignCenter().Text(i.Size ?? "-");
                            t.Cell().Element(BodyCell).AlignCenter().Text(i.Quantity.ToString());
                            t.Cell().Element(BodyCell).AlignCenter().Text(Money(i.UnitPrice));
                            t.Cell().Element(BodyCell).AlignCenter().Text(Money(i.UnitPrice * i.Quantity)).Bold();
                        }
                    });

                    // ---- الإجماليات ----
                    c.Item().Row(row =>
                    {
                        row.RelativeItem();

                        row.ConstantItem(240).Column(col =>
                        {
                            TotalRow(col, "المجموع (ج.م)", Money(o.Subtotal));
                            TotalRow(col, "الشحن (ج.م)", Money(o.ShippingFee));

                            // خط واحد فوق الإجمالي النهائي
                            col.Item().PaddingTop(4).BorderTop(LineWidth).BorderColor(LineClr)
                               .PaddingTop(8).Row(r =>
                               {
                                   r.RelativeItem().Text("الإجمالي المستحق (ج.م)").Bold().FontSize(13);
                                   r.ConstantItem(100).AlignLeft().Text(Money(o.Total)).Bold().FontSize(13);
                               });
                        });
                    });
                });

                // ================= الفوتر =================
                page.Footer().Column(c =>
                {
                    c.Item().LineHorizontal(LineWidth).LineColor(LineClr);
                    c.Item().PaddingTop(8).Row(row =>
                    {
                        row.RelativeItem().Text("شكراً لتعاملكم معنا").Bold();
                        row.RelativeItem().AlignLeft().Text(t =>
                        {
                            t.CurrentPageNumber();
                            t.Span(" / ");
                            t.TotalPages();
                        });
                    });
                });
            })).GeneratePdf();
        }
    }
}