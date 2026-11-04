
using LaboratoryDesktopApp.Models;
using LaboratoryDesktopApp.Services;
using LaboratoryDesktopApp.Theme;

namespace LaboratoryDesktopApp.Controls;

public class PatientsControl : UserControl
{
    private readonly PatientService _service = new();
    private readonly DataGridView grid = new();
    private readonly Label hint = new();
    private readonly TextBox searchBox = new();

    public PatientsControl()
    {
        Dock = DockStyle.Fill;
        BackColor = AppTheme.Background;
        Padding = new Padding(24);

        Build();
        RefreshData();
    }

    private void Build()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 5
        };

        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));

        var title = new Label
        {
            Text = "Patients",
            Font = AppTheme.PageTitle,
            ForeColor = AppTheme.MainText,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };

        root.Controls.Add(title, 0, 0);

        // Action buttons
        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight
        };
        AddButton(actions, "Refresh", RefreshData);
        AddButton(actions, "Register patient", AddPatient);
        AddButton(actions, "Edit patient", EditPatient);
        AddButton(actions, "Delete patient", DeletePatient);
        root.Controls.Add(actions, 0, 1);

        // Search area
        var searchPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Padding = new Padding(0, 5, 0, 0)
        };

        var searchLabel = new Label
        {
            Text = "Search:",
            AutoSize = true,
            Font = AppTheme.Label,
            ForeColor = AppTheme.MainText,
            Margin = new Padding(0, 8, 10, 0)
        };

        searchBox.Width = 280;
        searchBox.Height = 30;
        searchBox.Font = AppTheme.Body;
        searchBox.PlaceholderText = "Search by name or phone...";

        searchBox.TextChanged += (_, _) =>
        {
            LoadPatients(searchBox.Text);
        };

        searchPanel.Controls.Add(searchLabel);
        searchPanel.Controls.Add(searchBox);

        root.Controls.Add(searchPanel, 0, 2);

        // Patients table
        grid.Dock = DockStyle.Fill;
        grid.BackgroundColor = Color.White;
        grid.BorderStyle = BorderStyle.None;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.ReadOnly = true;

        grid.AutoSizeColumnsMode =
            DataGridViewAutoSizeColumnsMode.Fill;

        grid.SelectionMode =
            DataGridViewSelectionMode.FullRowSelect;

        grid.MultiSelect = false;
        grid.RowHeadersVisible = false;
        grid.EnableHeadersVisualStyles = false;

        grid.ColumnHeadersDefaultCellStyle.BackColor =
            AppTheme.PrimaryLight;

        grid.ColumnHeadersDefaultCellStyle.ForeColor =
            AppTheme.MainText;

        grid.ColumnHeadersDefaultCellStyle.Font =
            AppTheme.Label;

        grid.DefaultCellStyle.Font = AppTheme.Body;
        grid.RowTemplate.Height = 36;

        root.Controls.Add(grid, 0, 3);

        // Bottom information
        hint.Text = "Patients are stored in SQLite database.";
        hint.ForeColor = AppTheme.SecondaryText;
        hint.Dock = DockStyle.Fill;
        hint.TextAlign = ContentAlignment.MiddleLeft;

        root.Controls.Add(hint, 0, 4);

        Controls.Add(root);
    }

    private static void AddButton(
        Control parent,
        string text,
        Action action)
    {
        var button = new Button
        {
            Text = text,
            AutoSize = true,
            Height = 36,
            Padding = new Padding(12, 0, 12, 0),
            FlatStyle = FlatStyle.Flat,
            BackColor = AppTheme.Primary,
            ForeColor = Color.White,
            Font = AppTheme.Label,
            Cursor = Cursors.Hand
        };

        button.FlatAppearance.BorderSize = 0;
        button.Click += (_, _) => action();

        parent.Controls.Add(button);
    }

    private void RefreshData()
    {
        LoadPatients(searchBox.Text);
    }

    private void LoadPatients(string keyword)
    {
        try
        {
            var patients = _service.SearchPatients(keyword);

            grid.DataSource = patients.Select(p => new
            {
                p.Id,
                Code = p.PatientCode,
                Name = p.FullName,
                Phone = p.Phone,
                Gender = p.Gender,
                DateOfBirth = p.DateOfBirth.ToString("yyyy-MM-dd"),
                Notes = p.Notes
            }).ToList();

            if (grid.Columns.Contains("Id"))
                grid.Columns["Id"].Visible = false;

            hint.Text = string.IsNullOrWhiteSpace(keyword)
                ? $"Total registered patients: {patients.Count}"
                : $"Search results: {patients.Count} patient(s)";
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Could not load patients:\n" + ex.Message,
                "Database Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }


private void DeletePatient()
{
    if (grid.SelectedRows.Count == 0)
    {
        MessageBox.Show(
            "Please select a patient first.",
            "Delete Patient",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);

        return;
    }

    try
    {
        var selectedRow = grid.SelectedRows[0];

        int patientId = Convert.ToInt32(
            selectedRow.Cells["Id"].Value);

        string patientName =
            selectedRow.Cells["Name"].Value?.ToString() ?? "";

        var result = MessageBox.Show(
            $"Are you sure you want to delete patient:\n{patientName}?",
            "Confirm Delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (result != DialogResult.Yes)
            return;

        _service.DeletePatient(patientId);

        MessageBox.Show(
            "Patient deleted successfully.",
            "Success",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);

        RefreshData();
    }
    catch (Exception ex)
    {
        MessageBox.Show(
            "Could not delete patient:\n" + ex.Message,
            "Database Error",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }
}

    private void EditPatient()
    {
        if (grid.SelectedRows.Count == 0)
        {
            MessageBox.Show(
                "Please select a patient first.",
                "Edit Patient",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        try
        {
            int patientId = Convert.ToInt32(
                grid.SelectedRows[0].Cells["Id"].Value);

            var patient = _service.GetPatientById(patientId);

            if (patient == null)
            {
                MessageBox.Show("Patient not found.");
                RefreshData();
                return;
            }

            using var form = new Form
            {
                Text = "Edit patient",
                Size = new Size(480, 430),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(18),
                ColumnCount = 2,
                RowCount = 6
            };

            table.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 130));

            table.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));

            for (int i = 0; i < 6; i++)
                table.RowStyles.Add(
                    new RowStyle(SizeType.Absolute, 48));

            var name = Field(table, "Full name", 0);
            name.Text = patient.FullName;

            var phone = Field(table, "Phone", 1);
            phone.Text = patient.Phone;

            var gender = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            gender.Items.AddRange(new object[]
            {
            "Female",
            "Male",
            "Other / not specified"
            });

            if (!gender.Items.Contains(patient.Gender))
                gender.Items.Add(patient.Gender);

            gender.SelectedItem = patient.Gender;

            table.Controls.Add(
                new Label
                {
                    Text = "Gender",
                    TextAlign = ContentAlignment.MiddleLeft
                }, 0, 2);

            table.Controls.Add(gender, 1, 2);

            var dob = new DateTimePicker
            {
                Dock = DockStyle.Fill,
                Format = DateTimePickerFormat.Short,
                MaxDate = DateTime.Today,
                Value = patient.DateOfBirth.Date > DateTime.Today
                    ? DateTime.Today
                    : patient.DateOfBirth.Date
            };

            table.Controls.Add(
                new Label
                {
                    Text = "Date of birth",
                    TextAlign = ContentAlignment.MiddleLeft
                }, 0, 3);

            table.Controls.Add(dob, 1, 3);

            var notes = Field(table, "Notes", 4);
            notes.Text = patient.Notes;

            var save = new Button
            {
                Text = "Save changes",
                Dock = DockStyle.Fill,
                BackColor = AppTheme.Primary,
                ForeColor = Color.White
            };

            table.Controls.Add(save, 1, 5);

            save.Click += (_, _) =>
            {
                try
                {
                    patient.FullName = name.Text;
                    patient.Phone = phone.Text;
                    patient.Gender = gender.Text;
                    patient.DateOfBirth = dob.Value.Date;
                    patient.Notes = notes.Text;

                    _service.UpdatePatient(patient);

                    form.DialogResult = DialogResult.OK;
                    form.Close();
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show(
                        ex.Message,
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Could not update patient:\n" + ex.Message,
                        "Database Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            };

            form.Controls.Add(table);

            if (form.ShowDialog(FindForm()) == DialogResult.OK)
                RefreshData();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Could not open patient details:\n" + ex.Message,
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void AddPatient()
    {
        using var form = new Form
        {
            Text = "Register patient",
            Size = new Size(480, 430),
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false
        };

        var table = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(18),
            ColumnCount = 2,
            RowCount = 6
        };

        table.ColumnStyles.Add(
            new ColumnStyle(SizeType.Absolute, 130));

        table.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        for (int i = 0; i < 6; i++)
            table.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 48));

        var name = Field(table, "Full name", 0);
        var phone = Field(table, "Phone", 1);

        var gender = new ComboBox
        {
            Dock = DockStyle.Fill,
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        gender.Items.AddRange(new object[]
        {
            "Female",
            "Male",
            "Other / not specified"
        });

        gender.SelectedIndex = 2;

        table.Controls.Add(
            new Label
            {
                Text = "Gender",
                TextAlign = ContentAlignment.MiddleLeft
            }, 0, 2);

        table.Controls.Add(gender, 1, 2);

        var dob = new DateTimePicker
        {
            Dock = DockStyle.Fill,
            Format = DateTimePickerFormat.Short,
            MaxDate = DateTime.Today,
            Value = DateTime.Today
        };

        table.Controls.Add(
            new Label
            {
                Text = "Date of birth",
                TextAlign = ContentAlignment.MiddleLeft
            }, 0, 3);

        table.Controls.Add(dob, 1, 3);

        var notes = Field(table, "Notes", 4);

        var save = new Button
        {
            Text = "Save patient",
            Dock = DockStyle.Fill,
            BackColor = AppTheme.Primary,
            ForeColor = Color.White
        };

        table.Controls.Add(save, 1, 5);

        save.Click += (_, _) =>
        {
            try
            {
                var patient = new Patient
                {
                    FullName = name.Text,
                    Phone = phone.Text,
                    Gender = gender.Text,
                    DateOfBirth = dob.Value.Date,
                    Notes = notes.Text
                };

                _service.RegisterPatient(patient);

                form.DialogResult = DialogResult.OK;
                form.Close();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not save patient:\n" + ex.Message,
                    "Database Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        };

        form.Controls.Add(table);

        if (form.ShowDialog(FindForm()) == DialogResult.OK)
            RefreshData();
    }

    private static TextBox Field(
        TableLayoutPanel table,
        string label,
        int row)
    {
        table.Controls.Add(
            new Label
            {
                Text = label,
                TextAlign = ContentAlignment.MiddleLeft
            }, 0, row);

        var box = new TextBox
        {
            Dock = DockStyle.Fill
        };

        table.Controls.Add(box, 1, row);

        return box;
    }
}
