using LaboratoryDesktopApp.Models;
using LaboratoryDesktopApp.Theme;

namespace LaboratoryDesktopApp.Controls;

public class TestDetailsControl : UserControl
{
    private readonly LabTest test;

    public TestDetailsControl(LabTest test)
    {
        this.test = test;
        Dock = DockStyle.Fill;
        ThemeHelper.StylePage(this);
        Padding = new Padding(28);

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 150));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        root.Controls.Add(new Label
        {
            Text = test.Name,
            Font = AppTheme.PageTitle,
            Dock = DockStyle.Fill
        }, 0, 0);

        root.Controls.Add(new Label
        {
            Text = $"Code: {test.Code}    •    Category: {test.Category}    •    Status: {(test.IsActive ? "Active" : "Inactive")}",
            Font = AppTheme.Body,
            ForeColor = AppTheme.SecondaryText,
            Dock = DockStyle.Fill
        }, 0, 1);

        var info = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.Surface,
            Padding = new Padding(18)
        };
        info.Controls.Add(new Label
        {
            Text = $"Sample type: {test.SampleType}\nDemo price: {test.Price:0.00}\nTurnaround: {test.TurnaroundHours} hours\n\n{test.Description}\n\nAll values are illustrative demo data, not clinical guidance.",
            AutoSize = true,
            Font = AppTheme.Body
        });
        root.Controls.Add(info, 0, 2);

        var grid = new DataGridView { Dock = DockStyle.Fill };
        ThemeHelper.StyleGrid(grid);
        grid.Columns.Add("Name", "Parameter");
        grid.Columns.Add("Code", "Code");
        grid.Columns.Add("Type", "Result Type");
        grid.Columns.Add("Unit", "Unit");
        grid.Columns.Add("Reference", "Reference info");

        foreach (var parameter in test.Parameters)
        {
            grid.Rows.Add(parameter.Name, parameter.Code, parameter.DataType, parameter.Unit, parameter.ReferenceInfo);
        }

        root.Controls.Add(grid, 0, 3);
        Controls.Add(root);
    }

    public void ShowDetailsDialog()
    {
        using var form = new Form
        {
            Text = "Test Details",
            StartPosition = FormStartPosition.CenterParent,
            Size = new Size(900, 650),
            BackColor = AppTheme.Background
        };

        var detailsControl = new TestDetailsControl(test);
        form.Controls.Add(detailsControl);
        form.ShowDialog();
    }
}
