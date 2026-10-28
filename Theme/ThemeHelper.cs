using System.Drawing;
using System.Windows.Forms;

namespace LaboratoryDesktopApp.Theme;

public static class ThemeHelper
{
    public static void StylePage(Control control)
    {
        control.BackColor = AppTheme.Background; control.ForeColor = AppTheme.MainText; control.Font = AppTheme.Body;
    }
    public static Button Button(string text, bool primary = false)
    {
        var b = new Button { Text = text, Height = 40, AutoSize = true, MinimumSize = new Size(90,40), Padding = new Padding(14,0,14,0), FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, Font = new Font("Segoe UI",10,FontStyle.Bold), BackColor = primary ? AppTheme.Primary : AppTheme.Surface, ForeColor = primary ? Color.White : AppTheme.MainText, UseVisualStyleBackColor = false, Margin = new Padding(4) };
        b.FlatAppearance.BorderColor = AppTheme.Border; b.FlatAppearance.BorderSize = 1; if(primary) b.FlatAppearance.BorderColor = AppTheme.Primary; return b;
    }
    public static void StyleGrid(DataGridView grid)
    {
        grid.BackgroundColor = AppTheme.Surface; grid.BorderStyle = BorderStyle.None; grid.EnableHeadersVisualStyles = false; grid.ColumnHeadersDefaultCellStyle.BackColor = AppTheme.PrimaryLight; grid.ColumnHeadersDefaultCellStyle.ForeColor = AppTheme.MainText; grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI",9.5F,FontStyle.Bold); grid.DefaultCellStyle.Font = AppTheme.Body; grid.DefaultCellStyle.SelectionBackColor = AppTheme.PrimaryLight; grid.DefaultCellStyle.SelectionForeColor = AppTheme.MainText; grid.GridColor = AppTheme.Border; grid.RowHeadersVisible = false; grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; grid.RowTemplate.Height = 38; grid.ColumnHeadersHeight = 42; grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect; grid.MultiSelect = false; grid.AllowUserToAddRows = false; grid.ReadOnly = true;
    }
    public static TextBox TextBox() => new() { Height = 40, BorderStyle = BorderStyle.FixedSingle, Font = AppTheme.Body, Margin = new Padding(4,6,12,6), Anchor = AnchorStyles.Left | AnchorStyles.Right };
    public static Label Label(string text) => new() { Text = text, AutoSize = true, Font = AppTheme.Label, ForeColor = AppTheme.SecondaryText, Margin = new Padding(4,8,4,2) };
}
