using System.Drawing;

namespace LaboratoryDesktopApp.Theme;

public static class AppTheme
{
    public static readonly Color Primary = ColorTranslator.FromHtml("#1976D2");
    public static readonly Color PrimaryLight = ColorTranslator.FromHtml("#EAF3FB");
    public static readonly Color Background = ColorTranslator.FromHtml("#F5F7FA");
    public static readonly Color Surface = Color.White;
    public static readonly Color MainText = ColorTranslator.FromHtml("#263238");
    public static readonly Color SecondaryText = ColorTranslator.FromHtml("#64748B");
    public static readonly Color Border = ColorTranslator.FromHtml("#D7DEE7");
    public static readonly Color Error = ColorTranslator.FromHtml("#D32F2F");
    public static readonly Color Success = ColorTranslator.FromHtml("#2E7D32");
    public static readonly Font Body = new("Segoe UI", 10F, FontStyle.Regular);
    public static readonly Font Label = new("Segoe UI", 9.5F, FontStyle.Bold);
    public static readonly Font SectionTitle = new("Segoe UI", 13.5F, FontStyle.Bold);
    public static readonly Font PageTitle = new("Segoe UI", 18F, FontStyle.Bold);
}
