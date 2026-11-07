using LaboratoryDesktopApp.Theme;

namespace LaboratoryDesktopApp.Security;

public class LoginForm : Form
{
    private const int FieldWidth = 340;

    private readonly bool _setupMode;

    private readonly TextBox _email = new();
    private readonly TextBox _password = new();
    private readonly TextBox _confirm = new();
    private readonly CheckBox _showPassword = new();
    private readonly Label _error = new();
    private readonly Button _submit = new();
    private readonly System.Windows.Forms.Timer _lockTimer = new() { Interval = 1000 };

    private int _lockSecondsLeft;

    public LoginForm()
    {
        // أول تشغيل (ما في حساب) → شاشة إنشاء حساب المركز، غير هيك → تسجيل دخول
        _setupMode = !AuthService.HasAccount();

        Text = _setupMode ? "Laboratory - Create account" : "Laboratory - Sign in";
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = AppTheme.Background;
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;

        Build();

        AcceptButton = _submit;     // Enter يكبس الزر
        _lockTimer.Tick += OnLockTick;
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
            Text = "Laboratory Management",
            Font = AppTheme.PageTitle,
            ForeColor = AppTheme.MainText,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 4)
        });

        panel.Controls.Add(new Label
        {
            Text = _setupMode
                ? "First-time setup: create the center account."
                : "Sign in with the center email and password.",
            ForeColor = AppTheme.SecondaryText,
            AutoSize = true,
            MaximumSize = new Size(FieldWidth, 0),
            Margin = new Padding(0, 0, 0, 12)
        });

        panel.Controls.Add(Caption("Email"));
        _email.Width = FieldWidth;
        _email.PlaceholderText = "center@example.com";
        _email.Font = AppTheme.Body;
        panel.Controls.Add(_email);

        panel.Controls.Add(Caption("Password"));
        _password.Width = FieldWidth;
        _password.UseSystemPasswordChar = true;
        _password.Font = AppTheme.Body;
        panel.Controls.Add(_password);

        if (_setupMode)
        {
            panel.Controls.Add(Caption("Confirm password"));
            _confirm.Width = FieldWidth;
            _confirm.UseSystemPasswordChar = true;
            _confirm.Font = AppTheme.Body;
            panel.Controls.Add(_confirm);

            panel.Controls.Add(new Label
            {
                Text = "At least 8 characters, with letters and numbers.",
                ForeColor = AppTheme.SecondaryText,
                AutoSize = true,
                MaximumSize = new Size(FieldWidth, 0),
                Margin = new Padding(0, 6, 0, 0)
            });
        }

        _showPassword.Text = "Show password";
        _showPassword.AutoSize = true;
        _showPassword.ForeColor = AppTheme.SecondaryText;
        _showPassword.Margin = new Padding(0, 10, 0, 0);
        _showPassword.CheckedChanged += (_, _) =>
        {
            _password.UseSystemPasswordChar = !_showPassword.Checked;
            if (_setupMode) _confirm.UseSystemPasswordChar = !_showPassword.Checked;
        };
        panel.Controls.Add(_showPassword);

        _error.ForeColor = Color.FromArgb(198, 40, 40);
        _error.AutoSize = true;
        _error.MaximumSize = new Size(FieldWidth, 0);
        _error.Margin = new Padding(0, 10, 0, 0);
        panel.Controls.Add(_error);

        _submit.Text = _setupMode ? "Create account" : "Sign in";
        _submit.Width = FieldWidth;
        _submit.Height = 42;
        _submit.FlatStyle = FlatStyle.Flat;
        _submit.BackColor = AppTheme.Primary;
        _submit.ForeColor = Color.White;
        _submit.Font = AppTheme.Label;
        _submit.Cursor = Cursors.Hand;
        _submit.Margin = new Padding(0, 16, 0, 0);
        _submit.FlatAppearance.BorderSize = 0;
        _submit.Click += (_, _) => Submit();
        panel.Controls.Add(_submit);

        Controls.Add(panel);
    }

    private static Label Caption(string text) => new()
    {
        Text = text,
        AutoSize = true,
        ForeColor = AppTheme.SecondaryText,
        Font = AppTheme.Label,
        Margin = new Padding(0, 10, 0, 4)
    };

    // ===================== المنطق =====================

    private void Submit()
    {
        _error.Text = "";

        if (_setupMode)
        {
            var error = AuthService.CreateAccount(_email.Text, _password.Text, _confirm.Text);
            if (error != null)
            {
                _error.Text = error;
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
            return;
        }

        if (string.IsNullOrWhiteSpace(_email.Text) || _password.Text.Length == 0)
        {
            _error.Text = "Enter your email and password.";
            return;
        }

        var result = AuthService.Login(_email.Text, _password.Text);

        switch (result.Status)
        {
            case LoginStatus.Success:
                DialogResult = DialogResult.OK;
                Close();
                break;

            case LoginStatus.LockedOut:
                StartLock(result.SecondsLeft);
                break;

            default:
                _password.Clear();
                _password.Focus();
                _error.Text = "Incorrect email or password.";
                break;
        }
    }

    private void StartLock(int seconds)
    {
        _password.Clear();
        _lockSecondsLeft = seconds;
        _submit.Enabled = false;
        UpdateLockMessage();
        _lockTimer.Start();
    }

    private void OnLockTick(object? sender, EventArgs e)
    {
        _lockSecondsLeft--;

        if (_lockSecondsLeft <= 0)
        {
            _lockTimer.Stop();
            _submit.Enabled = true;
            _error.Text = "";
            return;
        }

        UpdateLockMessage();
    }

    private void UpdateLockMessage() =>
        _error.Text = $"Too many failed attempts. Try again in {_lockSecondsLeft}s.";

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _lockTimer.Dispose();
        base.OnFormClosed(e);
    }
}