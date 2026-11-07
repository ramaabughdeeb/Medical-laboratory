using LaboratoryDesktopApp.Theme;

namespace LaboratoryDesktopApp.Security;

public class AccountSettingsForm : Form
{
    private const int FieldWidth = 340;

    private readonly TextBox _currentPassword = new();
    private readonly TextBox _email = new();
    private readonly TextBox _newPassword = new();
    private readonly TextBox _confirm = new();
    private readonly CheckBox _show = new();
    private readonly Label _error = new();
    private readonly Button _save = new();

    public AccountSettingsForm()
    {
        Text = "Account settings";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = AppTheme.Background;
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;

        Build();
        AcceptButton = _save;
    }

    private void Build()
    {
        var panel = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(32)
        };

        panel.Controls.Add(new Label
        {
            Text = "Account settings",
            Font = AppTheme.PageTitle,
            ForeColor = AppTheme.MainText,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 4)
        });

        panel.Controls.Add(new Label
        {
            Text = "Change the center email and/or password. Your current password is required.",
            ForeColor = AppTheme.SecondaryText,
            AutoSize = true,
            MaximumSize = new Size(FieldWidth, 0),
            Margin = new Padding(0, 0, 0, 12)
        });

        panel.Controls.Add(Caption("Current password"));
        panel.Controls.Add(Field(_currentPassword, secret: true));

        panel.Controls.Add(Caption("Email"));
        _email.Text = AuthService.CurrentEmail ?? "";
        panel.Controls.Add(Field(_email, secret: false));

        panel.Controls.Add(Caption("New password (leave empty to keep the current one)"));
        panel.Controls.Add(Field(_newPassword, secret: true));

        panel.Controls.Add(Caption("Confirm new password"));
        panel.Controls.Add(Field(_confirm, secret: true));

        _show.Text = "Show passwords";
        _show.AutoSize = true;
        _show.ForeColor = AppTheme.SecondaryText;
        _show.Margin = new Padding(0, 10, 0, 0);
        _show.CheckedChanged += (_, _) =>
        {
            bool hide = !_show.Checked;
            _currentPassword.UseSystemPasswordChar = hide;
            _newPassword.UseSystemPasswordChar = hide;
            _confirm.UseSystemPasswordChar = hide;
        };
        panel.Controls.Add(_show);

        _error.ForeColor = Color.FromArgb(198, 40, 40);
        _error.AutoSize = true;
        _error.MaximumSize = new Size(FieldWidth, 0);
        _error.Margin = new Padding(0, 10, 0, 0);
        panel.Controls.Add(_error);

        _save.Text = "Save changes";
        _save.Width = FieldWidth;
        _save.Height = 42;
        _save.FlatStyle = FlatStyle.Flat;
        _save.BackColor = AppTheme.Primary;
        _save.ForeColor = Color.White;
        _save.Font = AppTheme.Label;
        _save.Cursor = Cursors.Hand;
        _save.Margin = new Padding(0, 16, 0, 0);
        _save.FlatAppearance.BorderSize = 0;
        _save.Click += (_, _) => Save();
        panel.Controls.Add(_save);

        Controls.Add(panel);
    }

    private static Label Caption(string text) => new()
    {
        Text = text,
        AutoSize = true,
        MaximumSize = new Size(FieldWidth, 0),
        ForeColor = AppTheme.SecondaryText,
        Font = AppTheme.Label,
        Margin = new Padding(0, 10, 0, 4)
    };

    private static TextBox Field(TextBox box, bool secret)
    {
        box.Width = FieldWidth;
        box.Font = AppTheme.Body;
        box.UseSystemPasswordChar = secret;
        return box;
    }

    private void Save()
    {
        _error.Text = "";

        if (_currentPassword.Text.Length == 0)
        {
            _error.Text = "Enter your current password.";
            return;
        }

        var error = AuthService.ChangeCredentials(
            _currentPassword.Text,
            _email.Text,
            _newPassword.Text,
            _confirm.Text);

        if (error != null)
        {
            _error.Text = error;
            _currentPassword.Clear();
            return;
        }

        MessageBox.Show(
            "Saved. Use the new details the next time you sign in.",
            "Account settings", MessageBoxButtons.OK, MessageBoxIcon.Information);

        DialogResult = DialogResult.OK;
        Close();
    }
}