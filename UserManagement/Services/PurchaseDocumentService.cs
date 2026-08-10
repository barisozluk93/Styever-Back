using MigraDoc;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PdfSharp.Fonts;
using UserManagement.Entity;
using UserManagement.Interfaces;
using UserManagement.Model;

namespace UserManagement.Services
{
    public class PurchaseDocumentService : IPurchaseDocumentService
    {
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<PurchaseDocumentService> _logger;

        private const string FontFamilyName = "StyeverSans";
        private const string ContractFileName = "DistanceSalesContract.tr.md";
        private static readonly object FontLock = new();
        private static bool _fontResolverConfigured;

        public PurchaseDocumentService(
            IUserService userService,
            IWebHostEnvironment environment,
            ILogger<PurchaseDocumentService> logger)
        {
            _userService = userService;
            _environment = environment;
            _logger = logger;

            EnsureFontResolver();
        }

        public async Task SendPurchaseDocumentsAsync(
            ShopierPayment payment,
            string? shopierOrderId,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(payment.BuyerEmail))
                throw new InvalidOperationException("Sözleşme gönderimi için alıcı e-posta adresi bulunamadı.");

            var markdown = await LoadContractMarkdownAsync(cancellationToken);
            var (preInformationMarkdown, distanceSalesMarkdown) = SplitContract(markdown);

            var transactionInfo = new PurchaseDocumentTransactionInfo
            {
                BuyerEmail = payment.BuyerEmail,
                Reference = payment.Reference,
                ShopierOrderId = shopierOrderId,
                PurchaseType = payment.PurchaseType,
                PlanId = payment.PlanId,
                CompletedDate = payment.CompletedDate ?? DateTime.UtcNow
            };

            var preInformationPdf = BuildPdf(
                "Ön Bilgilendirme Formu",
                preInformationMarkdown,
                transactionInfo);

            var distanceSalesPdf = BuildPdf(
                "Mesafeli Satış Sözleşmesi",
                distanceSalesMarkdown,
                transactionInfo);

            var attachments = new List<MailAttachment>
            {
                new()
                {
                    FileName = "Styever-On-Bilgilendirme-Formu.pdf",
                    Content = preInformationPdf,
                    ContentType = "application/pdf"
                },
                new()
                {
                    FileName = "Styever-Mesafeli-Satis-Sozlesmesi.pdf",
                    Content = distanceSalesPdf,
                    ContentType = "application/pdf"
                }
            };

            await _userService.SendMailAsync(
                payment.BuyerEmail,
                $"Styever ödeme sözleşmeleriniz - {payment.Reference}",
                BuildHtmlBody(transactionInfo),
                BuildTextBody(transactionInfo),
                attachments,
                cancellationToken);

            _logger.LogInformation(
                "Satış sözleşmeleri e-posta ile gönderildi. Reference: {Reference}, Email: {Email}",
                payment.Reference,
                payment.BuyerEmail);
        }

        private async Task<string> LoadContractMarkdownAsync(CancellationToken cancellationToken)
        {
            var path = Path.Combine(
                _environment.ContentRootPath,
                "Legal",
                ContractFileName);

            if (!System.IO.File.Exists(path))
                throw new FileNotFoundException("Mesafeli satış sözleşmesi dosyası bulunamadı.", path);

            return await System.IO.File.ReadAllTextAsync(path, cancellationToken);
        }

        private static (string PreInformation, string DistanceSales) SplitContract(string markdown)
        {
            const string sectionTwoMarker = "## BÖLÜM II: MESAFELİ SATIŞ SÖZLEŞMESİ";
            var sectionTwoIndex = markdown.IndexOf(sectionTwoMarker, StringComparison.Ordinal);

            if (sectionTwoIndex < 0)
                throw new InvalidOperationException("Sözleşme metninde BÖLÜM II başlığı bulunamadı.");

            var preInformation = markdown[..sectionTwoIndex].Trim();
            var distanceSales = markdown[sectionTwoIndex..].Trim();

            return (preInformation, distanceSales);
        }

        private static byte[] BuildPdf(
            string documentTitle,
            string markdown,
            PurchaseDocumentTransactionInfo transactionInfo)
        {
            var document = new Document();
            document.Info.Title = documentTitle;
            document.Info.Author = "Styever";

            var normalStyle = document.Styles[StyleNames.Normal];
            normalStyle.Font.Name = FontFamilyName;
            normalStyle.Font.Size = 9.5;
            normalStyle.ParagraphFormat.SpaceAfter = Unit.FromPoint(6);
            normalStyle.ParagraphFormat.LineSpacing = 1.1;

            var section = document.AddSection();
            section.PageSetup.PageFormat = PageFormat.A4;
            section.PageSetup.TopMargin = Unit.FromCentimeter(1.6);
            section.PageSetup.BottomMargin = Unit.FromCentimeter(1.6);
            section.PageSetup.LeftMargin = Unit.FromCentimeter(1.7);
            section.PageSetup.RightMargin = Unit.FromCentimeter(1.7);

            var header = section.Headers.Primary.AddParagraph("STYEVER");
            header.Format.Alignment = ParagraphAlignment.Right;
            header.Format.Font.Name = FontFamilyName;
            header.Format.Font.Size = 8;
            header.Format.Font.Bold = true;
            header.Format.Font.Color = Colors.DarkSlateGray;

            var footer = section.Footers.Primary.AddParagraph();
            footer.Format.Alignment = ParagraphAlignment.Center;
            footer.Format.Font.Name = FontFamilyName;
            footer.Format.Font.Size = 7.5;
            footer.AddText("Styever • Elektronik ortamda iletilmiştir • Sayfa ");
            footer.AddPageField();

            var title = section.AddParagraph(documentTitle);
            title.Format.Font.Name = FontFamilyName;
            title.Format.Font.Size = 18;
            title.Format.Font.Bold = true;
            title.Format.SpaceAfter = Unit.FromPoint(12);

            AddTransactionBox(section, transactionInfo);
            AddMarkdown(section, markdown);

            var renderer = new PdfDocumentRenderer
            {
                Document = document
            };

            renderer.RenderDocument();

            using var stream = new MemoryStream();
            renderer.PdfDocument.Save(stream, false);
            return stream.ToArray();
        }

        private static void AddTransactionBox(
            Section section,
            PurchaseDocumentTransactionInfo info)
        {
            var paragraph = section.AddParagraph();
            paragraph.Format.Font.Name = FontFamilyName;
            paragraph.Format.Font.Size = 8.5;
            paragraph.Format.LeftIndent = Unit.FromCentimeter(0.25);
            paragraph.Format.RightIndent = Unit.FromCentimeter(0.25);
            paragraph.Format.SpaceAfter = Unit.FromPoint(12);

            paragraph.AddFormattedText("İşlem Bilgileri\n", TextFormat.Bold);
            paragraph.AddText($"Ödeme yapan e-posta: {info.BuyerEmail}\n");
            paragraph.AddText($"Referans: {info.Reference}\n");

            if (!string.IsNullOrWhiteSpace(info.ShopierOrderId))
                paragraph.AddText($"Shopier Sipariş No: {info.ShopierOrderId}\n");

            paragraph.AddText($"İşlem türü: {GetPurchaseTypeText(info.PurchaseType)}\n");
            paragraph.AddText($"Paket: {GetPlanText(info.PlanId)}\n");
            paragraph.AddText($"Ödeme onay tarihi (UTC): {info.CompletedDate:dd.MM.yyyy HH:mm}");
        }

        private static void AddMarkdown(Section section, string markdown)
        {
            foreach (var rawLine in markdown.Replace("\r", string.Empty).Split('\n'))
            {
                var line = CleanMarkdownEscapes(rawLine.Trim());

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (line.StartsWith("#### ", StringComparison.Ordinal))
                {
                    AddHeading(section, line[5..], 17, 12);
                    continue;
                }

                if (line.StartsWith("### ", StringComparison.Ordinal))
                {
                    AddHeading(section, line[4..], 12.5, 8);
                    continue;
                }

                if (line.StartsWith("## ", StringComparison.Ordinal))
                {
                    AddHeading(section, line[3..], 14.5, 10);
                    continue;
                }

                if (line.StartsWith("- ", StringComparison.Ordinal))
                {
                    var bullet = section.AddParagraph();
                    bullet.Format.Font.Name = FontFamilyName;
                    bullet.Format.Font.Size = 9.5;
                    bullet.Format.LeftIndent = Unit.FromCentimeter(0.35);
                    bullet.Format.FirstLineIndent = Unit.FromCentimeter(-0.2);
                    bullet.Format.SpaceAfter = Unit.FromPoint(4);
                    bullet.AddText("• ");
                    bullet.AddText(line[2..]);
                    continue;
                }

                var paragraph = section.AddParagraph(line);
                paragraph.Format.Font.Name = FontFamilyName;
                paragraph.Format.Font.Size = 9.5;
                paragraph.Format.SpaceAfter = Unit.FromPoint(6);
                paragraph.Format.Alignment = ParagraphAlignment.Justify;
            }
        }

        private static void AddHeading(
            Section section,
            string text,
            double fontSize,
            double spaceBefore)
        {
            var paragraph = section.AddParagraph(text);
            paragraph.Format.Font.Name = FontFamilyName;
            paragraph.Format.Font.Size = fontSize;
            paragraph.Format.Font.Bold = true;
            paragraph.Format.SpaceBefore = Unit.FromPoint(spaceBefore);
            paragraph.Format.SpaceAfter = Unit.FromPoint(6);
        }

        private static string CleanMarkdownEscapes(string value) =>
            value
                .Replace("\\@", "@", StringComparison.Ordinal)
                .Replace("\\.", ".", StringComparison.Ordinal);

        private static string GetPurchaseTypeText(string purchaseType) =>
            purchaseType switch
            {
                "Gift" => "Hediye Paketi",
                "Pay" => "Üyelik Ödemesi",
                "Package" => "Paket Satın Alma",
                _ => purchaseType
            };

        private static string GetPlanText(long planId) =>
            planId switch
            {
                2 => "Origin Package",
                3 => "Heart Package",
                4 => "Family Package",
                _ => $"Plan {planId}"
            };

        private static string BuildHtmlBody(PurchaseDocumentTransactionInfo info) => $@"
<!doctype html>
<html lang='tr'>
<body style='font-family:Arial,Helvetica,sans-serif;color:#26332d;line-height:1.6;'>
  <div style='max-width:640px;margin:auto;padding:24px;'>
    <h2 style='margin:0 0 12px;color:#173f35;'>Styever ödeme sözleşmeleriniz</h2>
    <p>Ödemeniz başarıyla onaylandı.</p>
    <p>Ön Bilgilendirme Formu ve Mesafeli Satış Sözleşmesi PDF olarak bu e-postaya eklenmiştir.</p>
    <p><strong>Referans:</strong> {info.Reference}<br/>
       <strong>Shopier Sipariş No:</strong> {System.Net.WebUtility.HtmlEncode(info.ShopierOrderId ?? "-")}<br/>
       <strong>Paket:</strong> {GetPlanText(info.PlanId)}</p>
    <p>Belgelerinizi saklamanızı öneririz.</p>
    <p>Sevgi ve saygıyla,<br/><strong>Styever Ekibi</strong></p>
  </div>
</body>
</html>";

        private static string BuildTextBody(PurchaseDocumentTransactionInfo info) => $@"
Styever ödeme sözleşmeleriniz

Ödemeniz başarıyla onaylandı.
Ön Bilgilendirme Formu ve Mesafeli Satış Sözleşmesi PDF olarak bu e-postaya eklenmiştir.

Referans: {info.Reference}
Shopier Sipariş No: {info.ShopierOrderId ?? "-"}
Paket: {GetPlanText(info.PlanId)}

Belgelerinizi saklamanızı öneririz.

Sevgi ve saygıyla,
Styever Ekibi";

        private static void EnsureFontResolver()
        {
            if (_fontResolverConfigured)
                return;

            lock (FontLock)
            {
                if (_fontResolverConfigured)
                    return;

                GlobalFontSettings.FontResolver = new StyeverFontResolver();

                /*
                 * MigraDoc 6.2 varsayılan olarak hata metinleri için
                 * "Courier New" kullanıyor.
                 *
                 * Bizim resolver sadece StyeverSans çözdüğü için
                 * render sırasında Courier New bulunamıyordu.
                 */
                PredefinedFontsAndChars.ErrorFontName = FontFamilyName;

                _fontResolverConfigured = true;
            }
        }
        private sealed class PurchaseDocumentTransactionInfo
        {
            public string BuyerEmail { get; set; } = string.Empty;
            public Guid Reference { get; set; }
            public string? ShopierOrderId { get; set; }
            public string PurchaseType { get; set; } = string.Empty;
            public long PlanId { get; set; }
            public DateTime CompletedDate { get; set; }
        }

        private sealed class StyeverFontResolver : IFontResolver
        {
            private const string RegularFace = "StyeverSans#Regular";
            private const string BoldFace = "StyeverSans#Bold";

            private readonly byte[] _regularFont;
            private readonly byte[] _boldFont;

            public StyeverFontResolver()
            {
                _regularFont = LoadFirstExistingFont(new[]
                {
            "/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf",
            "/usr/share/fonts/dejavu/DejaVuSans.ttf",
            @"C:\Windows\Fonts\arial.ttf"
        });

                _boldFont = LoadFirstExistingFont(new[]
                {
            "/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf",
            "/usr/share/fonts/dejavu/DejaVuSans-Bold.ttf",
            @"C:\Windows\Fonts\arialbd.ttf"
        });
            }

            public FontResolverInfo? ResolveTypeface(
                string familyName,
                bool isBold,
                bool isItalic)
            {
                return new FontResolverInfo(
                    isBold
                        ? BoldFace
                        : RegularFace
                );
            }

            public byte[]? GetFont(string faceName)
            {
                return faceName switch
                {
                    RegularFace => _regularFont,
                    BoldFace => _boldFont,
                    _ => null
                };
            }

            private static byte[] LoadFirstExistingFont(
                IEnumerable<string> paths)
            {
                foreach (var path in paths)
                {
                    if (System.IO.File.Exists(path))
                        return System.IO.File.ReadAllBytes(path);
                }

                throw new FileNotFoundException(
                    "PDF üretimi için Unicode font bulunamadı. " +
                    "Linux'ta DejaVu Sans veya Windows'ta Arial kurulu olmalıdır.");
            }
        }
    }
}
