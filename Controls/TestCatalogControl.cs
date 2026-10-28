using LaboratoryDesktopApp.DemoData;
using LaboratoryDesktopApp.Models;
using LaboratoryDesktopApp.Theme;

namespace LaboratoryDesktopApp.Controls;

public class TestCatalogControl : UserControl
{
    private readonly DataGridView grid = new(); private readonly TextBox search = ThemeHelper.TextBox(); private readonly ComboBox category = new(); private readonly ComboBox status = new(); private readonly Label empty = new();
    public TestCatalogControl()
    {
        Dock=DockStyle.Fill; ThemeHelper.StylePage(this); Padding=new Padding(26);
        var root=new TableLayoutPanel { Dock=DockStyle.Fill, ColumnCount=1, RowCount=4, BackColor=AppTheme.Background }; root.RowStyles.Add(new RowStyle(SizeType.Absolute,48)); root.RowStyles.Add(new RowStyle(SizeType.Absolute,34)); root.RowStyles.Add(new RowStyle(SizeType.Absolute,64)); root.RowStyles.Add(new RowStyle(SizeType.Percent,100));
        var titleRow=new Panel { Dock=DockStyle.Fill }; titleRow.Controls.Add(new Label { Text="Test Catalog", Font=AppTheme.PageTitle, AutoSize=true, Location=new Point(0,4) }); var add=ThemeHelper.Button("+  Add Test",true); add.Anchor=AnchorStyles.Top|AnchorStyles.Right; add.Location=new Point(Width-160,0); add.Click+=(_,_)=>EditTest(null); titleRow.Resize+=(_,_)=>add.Left=titleRow.ClientSize.Width-add.Width; titleRow.Controls.Add(add); root.Controls.Add(titleRow,0,0);
        root.Controls.Add(new Label { Text="Manage available laboratory tests. All prices and values shown are demo data.", ForeColor=AppTheme.SecondaryText, Dock=DockStyle.Fill },0,1);
        var filters=new FlowLayoutPanel { Dock=DockStyle.Fill, WrapContents=false, Padding=new Padding(0,6,0,6), BackColor=AppTheme.Background }; search.Width=260; search.PlaceholderText="Search by test name or code..."; category.DropDownStyle=ComboBoxStyle.DropDownList; category.Width=180; status.DropDownStyle=ComboBoxStyle.DropDownList; status.Width=150; category.Items.Add("All categories"); category.Items.AddRange(DemoDataProvider.Categories.Select(c=>c.Name).ToArray()); category.SelectedIndex=0; status.Items.AddRange(new object[]{"All statuses","Active","Inactive"}); status.SelectedIndex=0; filters.Controls.Add(search); filters.Controls.Add(category); filters.Controls.Add(status); root.Controls.Add(filters,0,2);
        var tablePanel=new Panel { Dock=DockStyle.Fill, BackColor=AppTheme.Surface, Padding=new Padding(8) }; grid.Dock=DockStyle.Fill; ThemeHelper.StyleGrid(grid); grid.Columns.Add("Code","Test Code"); grid.Columns.Add("Name","Test Name"); grid.Columns.Add("Category","Category"); grid.Columns.Add("Sample","Sample Type"); grid.Columns.Add("Price","Demo Price"); grid.Columns.Add("Tat","Turnaround"); grid.Columns.Add("Status","Status"); grid.Columns.Add(new DataGridViewButtonColumn { Name="View", HeaderText="View", Text="View", UseColumnTextForButtonValue=true }); grid.Columns.Add(new DataGridViewButtonColumn { Name="Edit", HeaderText="Edit", Text="Edit", UseColumnTextForButtonValue=true }); grid.Columns.Add(new DataGridViewButtonColumn { Name="Toggle", HeaderText="Status", Text="Toggle", UseColumnTextForButtonValue=true }); grid.CellClick+=Grid_CellClick; tablePanel.Controls.Add(grid); empty.Text="No tests match your filters."; empty.TextAlign=ContentAlignment.MiddleCenter; empty.Dock=DockStyle.Fill; empty.ForeColor=AppTheme.SecondaryText; empty.Visible=false; tablePanel.Controls.Add(empty); root.Controls.Add(tablePanel,0,3); Controls.Add(root);
        search.TextChanged+=(_,_)=>RefreshGrid(); category.SelectedIndexChanged+=(_,_)=>RefreshGrid(); status.SelectedIndexChanged+=(_,_)=>RefreshGrid(); RefreshGrid();
    }
    private void RefreshGrid()
    {
        grid.Rows.Clear(); var q=search.Text.Trim(); foreach(var t in DemoDataProvider.Tests.Where(t=>(string.IsNullOrWhiteSpace(q)||t.Name.Contains(q,StringComparison.OrdinalIgnoreCase)||t.Code.Contains(q,StringComparison.OrdinalIgnoreCase))&&(category.SelectedIndex<=0||t.Category==category.Text)&&(status.SelectedIndex<=0||(status.SelectedIndex==1&&t.IsActive)||(status.SelectedIndex==2&&!t.IsActive)))) grid.Rows.Add(t.Code,t.Name,t.Category,t.SampleType,t.Price.ToString("0.00"),$"{t.TurnaroundHours} h",t.IsActive?"● Active":"○ Inactive","View","Edit","Toggle"); empty.Visible=grid.Rows.Count==0; grid.Visible=!empty.Visible;
    }
    private void Grid_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if(e.RowIndex<0)return; var code=Convert.ToString(grid.Rows[e.RowIndex].Cells["Code"].Value); var test=DemoDataProvider.Tests.FirstOrDefault(t=>t.Code==code); if(test==null)return; var x=e.ColumnIndex; if(x==7) new TestDetailsControl(test).ShowDetailsDialog(); else if(x==8) EditTest(test); else if(x==9){test.IsActive=!test.IsActive;RefreshGrid();}
    }
    private void EditTest(LabTest? test) { using var form=new TestEditorForm(test); if(form.ShowDialog()==DialogResult.OK) RefreshGrid(); }
}
