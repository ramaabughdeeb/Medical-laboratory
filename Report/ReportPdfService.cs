using System.Globalization;
using LaboratoryDesktopApp.DemoData;
using LaboratoryDesktopApp.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace LaboratoryDesktopApp.Reports
{
    public static class ReportPdfService
    {
        // ===== بيانات المركز (عدّليها هون مرة وحدة) =====
        public const string CenterNameAr = "مختبرات بروفيشينال الطبية";
        public const string CenterNameEn = "Professional Lab";
        private const string TaglineAr = "متخصصون في أمراض الدم والأورام";
        private const string TaglineEn = "Hematology Consultant";

        // العنوان: اتركيهم فاضيين إذا ما بدك يظهروا، أو عبّيهم لما تتأكدي منهم
        private const string CityAr = "";
        private const string AddressAr = "";

        private const string Whatsapp = "+972598210617";
        private const string Email = "info@professional-lab.ps";
        private const string FacebookPage = "مختبرات بروفيشنال الطبية التخصصية Professional Lab";
        private const string InstagramHandle = "Professinallab";
        private const string RequestedBy =  CenterNameAr;

        // الشعار: Assets/logo.png (Copy to output directory)
        private const string LogoRelativePath = "Assets/logo.png";

        private const string FontName = "Tahoma";   // بيدعم العربي على Windows
        private const string Blue = "#1976D2";

        private sealed record ResultRow(bool IsGroup, string Test, string Result, string Range);

        // ===================== واجهات عامة =====================

        public static void Generate(
            string filePath,
            TestOrder order,
            Patient patient,
            IEnumerable<TestResult> results)
        {
            var dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            BuildDocument(order, patient, results).GeneratePdf(filePath);
        }

        /// صور PNG لكل صفحة، تُستخدم بشاشة المعاينة عشان تطابق الـ PDF تمامًا
        public static List<byte[]> RenderPreviewImages(
            TestOrder order,
            Patient patient,
            IEnumerable<TestResult> results)
        {
            return BuildDocument(order, patient, results).GenerateImages().ToList();
        }

        // ===================== بناء المستند =====================

        private static IDocument BuildDocument(
            TestOrder order,
            Patient patient,
            IEnumerable<TestResult> results)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            // من QuestPDF 2026.9 خطوط النظام مطفية افتراضيًا، فلازم نفعّلها عشان Tahoma
            QuestPDF.Settings.UseSystemFonts = true;

            var rows = BuildRows(results.ToList());
            var logo = LoadLogo();

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.MarginHorizontal(30);
                    page.MarginVertical(25);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontFamily(FontName).FontSize(10));

                    page.Header().Element(c => ComposeHeader(c, logo));
                    page.Content().Element(c => ComposeContent(c, order, patient, rows));

                    page.Footer().Column(f =>
                    {
                        f.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);

                        f.Item().PaddingTop(4).AlignCenter().ContentFromRightToLeft().Text(t =>
                        {
                            t.DefaultTextStyle(x => x.FontSize(8).FontColor(Colors.Grey.Darken1));
                            t.Span("فيسبوك : ").Bold();
                            t.Span(FacebookPage + "      ");
                            t.Span("انستغرام : ").Bold();
                            t.Span(InstagramHandle);
                        });

                        f.Item().PaddingTop(2).AlignCenter().Text(t =>
                        {
                            t.DefaultTextStyle(x => x.FontSize(8).FontColor(Colors.Grey.Medium));
                            t.CurrentPageNumber();
                            t.Span(" / ");
                            t.TotalPages();
                        });
                    });
                });
            });
        }

        // ===================== الترويسة =====================

        private static void ComposeHeader(IContainer container, byte[]? logo)
        {
            container.Column(col =>
            {
                col.Item().Row(row =>
                {
                    // يسار: الاسم الإنجليزي
                    row.RelativeItem().AlignMiddle().Column(left =>
                    {
                        left.Item().Text(CenterNameEn).Bold().FontSize(13).AlignCenter();
                        left.Item().PaddingTop(4).Text(TaglineEn).FontSize(9).AlignCenter();
                    });

                    // وسط: الشعار
                    row.ConstantItem(85).Padding(4).Element(e =>
                    {
                        if (logo != null)
                            e.Image(logo).FitArea();
                    });

                    // يمين: الاسم العربي
                    row.RelativeItem().AlignMiddle().ContentFromRightToLeft().Column(right =>
                    {
                        right.Item().Text(CenterNameAr).Bold().FontSize(14).AlignCenter();
                        right.Item().PaddingTop(4).Text(TaglineAr).FontSize(9).AlignCenter();
                    });
                });

                col.Item().PaddingTop(8).LineHorizontal(1.5f).LineColor(Blue);

                // سطر التواصل (واتساب + إيميل) والعنوان إذا موجود
                col.Item().PaddingVertical(3).Row(row =>
                {
                    row.RelativeItem().ContentFromRightToLeft().Text(t =>
                    {
                        t.DefaultTextStyle(x => x.FontSize(7.5f));
                        t.Span("واتساب : ").Bold();
                        t.Span("\u200E" + Whatsapp + "   ");   // \u200E عشان علامة + ما تنقلب
                        t.Span("الإيميل : ").Bold();
                        t.Span(Email);
                    });

                    row.RelativeItem().AlignLeft().ContentFromRightToLeft().Text(t =>
                    {
                        t.DefaultTextStyle(x => x.FontSize(7.5f));

                        if (!string.IsNullOrWhiteSpace(CityAr))
                        {
                            t.Span("المدينة : ").Bold();
                            t.Span(CityAr + "   ");
                        }

                        if (!string.IsNullOrWhiteSpace(AddressAr))
                        {
                            t.Span("العنوان : ").Bold();
                            t.Span(AddressAr);
                        }
                    });
                });

                col.Item().LineHorizontal(1.5f).LineColor(Blue);
                col.Item().PaddingTop(2).LineHorizontal(1f).LineColor(Blue);
            });
        }

        // ===================== المحتوى =====================

        private static void ComposeContent(
            IContainer container,
            TestOrder order,
            Patient patient,
            List<ResultRow> rows)
        {
            container.PaddingTop(18).Column(col =>
            {
                col.Spacing(28);

                col.Item().Element(e => ComposePatientBox(e, order, patient));
                col.Item().Element(e => ComposeResultsTable(e, rows));

                col.Item().AlignRight().Text(
                        DateTime.Now.ToString("dd-MM-yyyy @ hh:mm tt", CultureInfo.InvariantCulture))
                    .Bold();
            });
        }

        private static void ComposePatientBox(IContainer container, TestOrder order, Patient patient)
        {
            container
                .Border(0.7f).BorderColor(Colors.Grey.Lighten1)
                .Padding(12)
                .ContentFromRightToLeft()
                .Table(t =>
                {
                    t.ColumnsDefinition(c =>
                    {
                        c.RelativeColumn(1.1f);
                        c.RelativeColumn(1f);
                        c.RelativeColumn(1.4f);
                    });

                    // الصف الأول (من اليمين لليسار)
                    InfoCell(t, "الاسم", patient.FullName);
                    InfoCell(t, "العمر", AgeText(patient.DateOfBirth));
                    InfoCell(t, "العنوان", patient.Address);

                    // الصف الثاني
                    InfoCell(t, "رقم التواصل", patient.Phone);
                    InfoCell(t, "التاريخ", order.CreatedAt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
                    InfoCell(t, "مقدم الطلب", RequestedBy);
                });
        }

        private static void InfoCell(TableDescriptor table, string label, string? value)
        {
            table.Cell().PaddingVertical(6).Text(x =>
            {
                x.Span($"{label} : ").Bold();
                x.Span(string.IsNullOrWhiteSpace(value) ? "-" : value);
            });
        }

        private static void ComposeResultsTable(IContainer container, List<ResultRow> rows)
        {
            container.Table(t =>
            {
                t.ColumnsDefinition(c =>
                {
                    c.RelativeColumn(3);
                    c.RelativeColumn(2);
                    c.RelativeColumn(4);
                });

                t.Header(h =>
                {
                    h.Cell().Element(CellStyle).Text("Test").Bold();
                    h.Cell().Element(CellStyle).Text("Results").Bold();
                    h.Cell().Element(CellStyle).Text("Reference Range").Bold();
                });

                foreach (var r in rows)
                {
                    if (r.IsGroup)
                    {
                        t.Cell().ColumnSpan(3)
                            .Background(Colors.Grey.Lighten4)
                            .Element(CellStyle)
                            .Text(r.Test).Bold();
                        continue;
                    }

                    t.Cell().Element(CellStyle).AlignMiddle().Text(r.Test);
                    t.Cell().Element(CellStyle).AlignMiddle().Text(r.Result);
                    t.Cell().Element(CellStyle).Text(r.Range);
                }
            });
        }

        private static IContainer CellStyle(IContainer c) =>
            c.Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(7);

        // ===================== مساعدات =====================

        private static List<ResultRow> BuildRows(List<TestResult> results)
        {
            var rows = new List<ResultRow>();

            foreach (var group in results.GroupBy(r => r.TestCode))
            {
                var lab = DemoDataProvider.Tests.FirstOrDefault(t => t.Code == group.Key);
                var testName = lab?.Name ?? group.Key;
                var items = group.ToList();

                if (items.Count == 1)
                {
                    // فحص بنتيجة وحدة: اسم الفحص نفسه (زي B-HCG Quantitative)
                    rows.Add(new ResultRow(false, testName, FormatValue(items[0]), CleanRange(items[0].ReferenceRange)));
                }
                else
                {
                    // فحص بعدة بارامترات (مثل CBC): عنوان الفحص ثم البارامترات
                    rows.Add(new ResultRow(true, testName, "", ""));
                    foreach (var r in items)
                        rows.Add(new ResultRow(false, r.Parameter, FormatValue(r), CleanRange(r.ReferenceRange)));
                }
            }

            return rows;
        }

        private static string FormatValue(TestResult r) =>
            string.IsNullOrWhiteSpace(r.Unit) ? r.Value : $"{r.Value} {r.Unit}";

        private static string CleanRange(string? range) =>
            string.IsNullOrWhiteSpace(range) || range.Trim() == "Demo only" ? "-" : range;

        private static string AgeText(DateTime dob)
        {
            if (dob == default || dob.Date > DateTime.Today) return "-";

            int age = DateTime.Today.Year - dob.Year;
            if (dob.Date > DateTime.Today.AddYears(-age)) age--;
            return $"{age} سنة";
        }

        private static byte[]? LoadLogo()
        {
            var path = Path.Combine(AppContext.BaseDirectory, LogoRelativePath);
            return File.Exists(path) ? File.ReadAllBytes(path) : null;
        }
    }
}