using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using LaboratoryDesktopApp.Models;

namespace LaboratoryDesktopApp.Reports
{
    public class ReportPreviewForm : Form
    {
        private readonly Color Primary = ColorTranslator.FromHtml("#1976D2");
        private readonly Color Background = ColorTranslator.FromHtml("#F5F7FA");
        private readonly Color Surface = Color.White;
        private readonly Color MainText = ColorTranslator.FromHtml("#263238");

        private readonly TestOrder _order;
        private readonly Patient _patient;
        private readonly List<TestResult> _results;

        private readonly FlowLayoutPanel _pages = new();

        private const int PageWidth = 794; // عرض A4 تقريبًا بالبكسل

        public ReportPreviewForm(
            TestOrder order,
            Patient patient,
            List<TestResult> results)
        {
            _order = order ?? throw new ArgumentNullException(nameof(order));
            _patient = patient ?? throw new ArgumentNullException(nameof(patient));
            _results = results ?? new List<TestResult>();

            Text = "Laboratory - Report Preview";
            Size = new Size(1000, 850);
            MinimumSize = new Size(850, 650);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Background;
            Font = new Font("Segoe UI", 10);

            BuildUI();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            LoadPreview();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            foreach (Control c in _pages.Controls)
                if (c is PictureBox pb)
                    pb.Image?.Dispose();

            base.OnFormClosed(e);
        }

        private Button MakeButton(string text, bool primary)
        {
            var button = new Button
            {
                Text = text,
                Width = 190,
                Height = 40,
                BackColor = primary ? Primary : Surface,
                ForeColor = primary ? Color.White : MainText,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(0, 0, 12, 0),
                Cursor = Cursors.Hand
            };

            button.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#D7DEE7");
            if (primary) button.FlatAppearance.BorderSize = 0;
            return button;
        }

        private void BuildUI()
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(24)
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 75));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 65));
            Controls.Add(layout);

            // HEADER
            var header = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown
            };
            header.Controls.Add(new Label
            {
                Text = "Report Preview",
                AutoSize = true,
                ForeColor = MainText,
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                Margin = new Padding(0, 0, 0, 6)
            });
            header.Controls.Add(new Label
            {
                Text = $"Order {_order.OrderCode} - {_patient.FullName}",
                AutoSize = true,
                ForeColor = MainText
            });
            layout.Controls.Add(header, 0, 0);

            // PREVIEW (صفحات الـ PDF الفعلية)
            _pages.Dock = DockStyle.Fill;
            _pages.FlowDirection = FlowDirection.TopDown;
            _pages.WrapContents = false;
            _pages.AutoScroll = true;
            _pages.BackColor = ColorTranslator.FromHtml("#E3E8EF");
            _pages.Padding = new Padding(16);
            _pages.Resize += (_, _) => CenterPages();
            layout.Controls.Add(_pages, 0, 1);

            // ACTION BUTTONS
            var actions = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true
            };

            var pdfButton = MakeButton("Generate PDF", true);
            var printButton = MakeButton("Print Report", false);
            var whatsappButton = MakeButton("Send via WhatsApp", false);

            pdfButton.Click += (_, _) => SavePdf();
            printButton.Click += (_, _) => PrintReport();
            whatsappButton.Click += (_, _) => SendWhatsApp();

            actions.Controls.Add(pdfButton);
            actions.Controls.Add(printButton);
            actions.Controls.Add(whatsappButton);
            layout.Controls.Add(actions, 0, 2);
        }

        // ===================== المعاينة =====================

        private void LoadPreview()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                var images = ReportPdfService.RenderPreviewImages(_order, _patient, _results);

                foreach (var png in images)
                {
                    using var ms = new MemoryStream(png);
                    using var tmp = Image.FromStream(ms);
                    var bmp = new Bitmap(tmp);

                    int h = (int)(PageWidth * (double)bmp.Height / bmp.Width);

                    _pages.Controls.Add(new PictureBox
                    {
                        Image = bmp,
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Width = PageWidth,
                        Height = h,
                        BackColor = Color.White,
                        Margin = new Padding(0, 0, 0, 16)
                    });
                }

                CenterPages();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Failed to render report preview:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void CenterPages()
        {
            foreach (Control c in _pages.Controls)
            {
                int left = Math.Max(0, (_pages.ClientSize.Width - c.Width - _pages.Padding.Horizontal) / 2);
                c.Margin = new Padding(left, 0, 0, 16);
            }
        }

        // ===================== الأزرار =====================

        private void SavePdf()
        {
            using var dialog = new SaveFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf",
                FileName = $"LabReport_{_order.OrderCode}.pdf"
            };

            if (dialog.ShowDialog() != DialogResult.OK) return;

            try
            {
                ReportPdfService.Generate(dialog.FileName, _order, _patient, _results);

                MessageBox.Show(
                    "PDF report generated successfully!",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Failed to generate PDF:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintReport()
        {
            try
            {
                var path = Path.Combine(
                    Path.GetTempPath(),
                    $"LabReport_{_order.OrderCode}_{DateTime.Now:HHmmss}.pdf");

                ReportPdfService.Generate(path, _order, _patient, _results);

                Process.Start(new ProcessStartInfo
                {
                    FileName = path,
                    UseShellExecute = true
                });

                MessageBox.Show(
                    "Report opened. Press Ctrl+P to print.",
                    "Print Report", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to open report:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===================== واتساب =====================

        // كود الدولة اللي بينضاف للأرقام المحلية اللي بتبدأ بصفر (970 فلسطين، 972 إذا بدك)
        private const string DefaultCountryCode = "970";

        private static string NormalizePhone(string raw)
        {
            var digits = new string(raw.Where(char.IsDigit).ToArray());

            if (digits.StartsWith("00")) return digits.Substring(2);        // 00970...
            if (raw.TrimStart().StartsWith("+")) return digits;             // +970...
            if (digits.StartsWith("0")) return DefaultCountryCode + digits.Substring(1); // 0594...
            if (digits.Length <= 9) return DefaultCountryCode + digits;     // 594...
            return digits;                                                  // 970594...
        }

        private void SendWhatsApp()
        {
            if (string.IsNullOrWhiteSpace(_patient.Phone))
            {
                MessageBox.Show(
                    "This patient has no phone number.",
                    "WhatsApp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string phone = NormalizePhone(_patient.Phone);

            try
            {
                // 1) نولّد الـ PDF
                var folder = Path.Combine(Path.GetTempPath(), "LabReports");
                Directory.CreateDirectory(folder);
                var pdfPath = Path.Combine(folder, $"LabReport_{_order.OrderCode}.pdf");
                ReportPdfService.Generate(pdfPath, _order, _patient, _results);

                // 2) ننسخ الملف للـ Clipboard عشان يتلصق بالمحادثة بـ Ctrl+V
                Clipboard.SetFileDropList(new StringCollection { pdfPath });

                MessageBox.Show(
                    "تم تجهيز ملف التقرير ونسخه.\n\n" +
                    "رح تنفتح محادثة المريض بالواتساب، اضغطي Ctrl+V لإرفاق التقرير ثم Enter للإرسال.",
                    "WhatsApp", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 3) نفتح محادثة هاد الرقم
                string message =
                    $"مرحبًا {_patient.FullName}، تقرير الفحوصات الخاص بك من {ReportPdfService.CenterNameAr} جاهز.";

                OpenWhatsAppChat(phone, message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to send via WhatsApp:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void OpenWhatsAppChat(string phone, string message)
        {
            string text = Uri.EscapeDataString(message);

            try
            {
                // يفتح تطبيق واتساب ديسكتوب مباشرة على المحادثة
                Process.Start(new ProcessStartInfo
                {
                    FileName = $"whatsapp://send?phone={phone}&text={text}",
                    UseShellExecute = true
                });
            }
            catch (Win32Exception)
            {
                // إذا واتساب ديسكتوب مش منصّب: نفتحها بالمتصفح (WhatsApp Web)
                Process.Start(new ProcessStartInfo
                {
                    FileName = $"https://wa.me/{phone}?text={text}",
                    UseShellExecute = true
                });
            }
        }
    }
}