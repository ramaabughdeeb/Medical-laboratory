using FontAwesome.Sharp;
using LaboratoryDesktopApp.Theme;

namespace LaboratoryDesktopApp.Controls;

public class PlaceholderControl : UserControl
{
    public PlaceholderControl(string title, string description)
    {
        Dock = DockStyle.Fill; ThemeHelper.StylePage(this); Padding = new Padding(28);
        var layout = new TableLayoutPanel { Dock=DockStyle.Fill, ColumnCount=1, RowCount=3, BackColor=AppTheme.Background }; layout.RowStyles.Add(new RowStyle(SizeType.Absolute,45)); layout.RowStyles.Add(new RowStyle(SizeType.Absolute,36)); layout.RowStyles.Add(new RowStyle(SizeType.Percent,100));
        layout.Controls.Add(new Label { Text=title, Font=AppTheme.PageTitle, ForeColor=AppTheme.MainText, Dock=DockStyle.Fill },0,0);
        layout.Controls.Add(new Label { Text=description, Font=AppTheme.Body, ForeColor=AppTheme.SecondaryText, Dock=DockStyle.Fill },0,1);
        var card = new Panel { Dock=DockStyle.Top, Height=190, BackColor=AppTheme.Surface, Padding=new Padding(24), Margin=new Padding(0,18,0,0) }; card.Paint += (_,e)=>ControlPaint.DrawBorder(e.Graphics,card.ClientRectangle,AppTheme.Border,ButtonBorderStyle.Solid);
        var icon = new IconPictureBox { IconChar=IconChar.Flask, IconColor=AppTheme.Primary, IconSize=42, Size=new Size(56,56), BackColor=AppTheme.Surface, Location=new Point(24,24) };
        var heading = new Label { Text="Preview section", Font=AppTheme.SectionTitle, AutoSize=true, Location=new Point(92,28) };
        var note = new Label { Text="This section is a UI placeholder. No real patient, sample, result or report operations are implemented.", Font=AppTheme.Body, ForeColor=AppTheme.SecondaryText, AutoSize=true, MaximumSize=new Size(700,0), Location=new Point(92,62) };
        card.Controls.Add(icon); card.Controls.Add(heading); card.Controls.Add(note); layout.Controls.Add(card,0,2); Controls.Add(layout);
    }
}
