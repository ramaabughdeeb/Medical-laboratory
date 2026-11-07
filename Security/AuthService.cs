using System.Net.Mail;
using System.Security.Cryptography;
using System.Text.Json;

namespace LaboratoryDesktopApp.Security;

public enum LoginStatus { Success, InvalidCredentials, LockedOut }

public sealed record LoginResult(LoginStatus Status, int SecondsLeft = 0);

/// تسجيل دخول محلي: الإيميل + كلمة السر (مخزنة كـ hash مملّح، مش نص عادي)
public static class AuthService
{
    private const int Iterations = 210_000;
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int MaxFailedAttempts = 5;
    private const int MinPasswordLength = 8;

    private sealed class AccountData
    {
        public string Email { get; set; } = "";
        public string Salt { get; set; } = "";
        public string Hash { get; set; } = "";
        public int Iterations { get; set; }
    }

    private static readonly string AccountFile = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "LaboratoryDesktopApp",
        "auth.json");

    private static int _failedAttempts;
    private static int _lockouts;
    private static DateTime _lockedUntil = DateTime.MinValue;

    // ===================== الحساب =====================

    public static bool HasAccount() => File.Exists(AccountFile);

    /// إنشاء حساب المركز (أول تشغيل فقط). بترجع نص الخطأ أو null إذا نجح.
    public static string? CreateAccount(string email, string password, string confirm)
    {
        if (HasAccount())
            return "An account already exists.";

        email = NormalizeEmail(email);

        if (!IsValidEmail(email))
            return "Enter a valid email address.";

        var passwordError = ValidatePassword(password, confirm);
        if (passwordError != null)
            return passwordError;

        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Hash(password, salt, Iterations);

        var data = new AccountData
        {
            Email = email,
            Salt = Convert.ToBase64String(salt),
            Hash = Convert.ToBase64String(hash),
            Iterations = Iterations
        };

        Directory.CreateDirectory(Path.GetDirectoryName(AccountFile)!);
        File.WriteAllText(AccountFile, JsonSerializer.Serialize(data));
        return null;
    }

    /// الإيميل الحالي للحساب (للعرض بشاشة الإعدادات)
    public static string? CurrentEmail => LoadAccount()?.Email;

    /// تغيير الإيميل و/أو كلمة السر. لازم كلمة السر الحالية.
    /// newPassword فاضية = بنخلي كلمة السر القديمة. بترجع نص الخطأ أو null إذا نجح.
    public static string? ChangeCredentials(
        string currentPassword, string newEmail, string newPassword, string confirm)
    {
        var account = LoadAccount();
        if (account == null)
            return "No account found.";

        // نتحقق من كلمة السر الحالية (بنفس نظام القفل بعد المحاولات الغلط)
        var check = Login(account.Email, currentPassword);
        if (check.Status == LoginStatus.LockedOut)
            return $"Too many failed attempts. Try again in {check.SecondsLeft}s.";
        if (check.Status != LoginStatus.Success)
            return "Current password is incorrect.";

        var email = NormalizeEmail(newEmail);
        if (!IsValidEmail(email))
            return "Enter a valid email address.";

        var updated = new AccountData
        {
            Email = email,
            Salt = account.Salt,
            Hash = account.Hash,
            Iterations = account.Iterations
        };

        if (!string.IsNullOrEmpty(newPassword))
        {
            var passwordError = ValidatePassword(newPassword, confirm);
            if (passwordError != null)
                return passwordError;

            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            updated.Salt = Convert.ToBase64String(salt);
            updated.Hash = Convert.ToBase64String(Hash(newPassword, salt, Iterations));
            updated.Iterations = Iterations;
        }

        Directory.CreateDirectory(Path.GetDirectoryName(AccountFile)!);
        File.WriteAllText(AccountFile, JsonSerializer.Serialize(updated));
        return null;
    }

    // ===================== تسجيل الدخول =====================

    public static LoginResult Login(string email, string password)
    {
        var now = DateTime.UtcNow;
        if (now < _lockedUntil)
            return new LoginResult(LoginStatus.LockedOut, (int)Math.Ceiling((_lockedUntil - now).TotalSeconds));

        var account = LoadAccount();
        if (account == null)
            return new LoginResult(LoginStatus.InvalidCredentials);

        // نحسب الـ hash دايمًا (حتى لو الإيميل غلط) عشان ما يتسرّب أي شي من زمن الاستجابة
        var salt = Convert.FromBase64String(account.Salt);
        var expected = Convert.FromBase64String(account.Hash);
        var actual = Hash(password, salt, account.Iterations);

        bool passwordOk = CryptographicOperations.FixedTimeEquals(actual, expected);
        bool emailOk = string.Equals(NormalizeEmail(email), account.Email, StringComparison.Ordinal);

        if (passwordOk && emailOk)
        {
            _failedAttempts = 0;
            _lockouts = 0;
            return new LoginResult(LoginStatus.Success);
        }

        _failedAttempts++;
        if (_failedAttempts >= MaxFailedAttempts)
        {
            // 30 ثانية، ثم 60، ثم 120... بحد أقصى 15 دقيقة
            int seconds = Math.Min(30 * (int)Math.Pow(2, _lockouts), 15 * 60);
            _lockouts++;
            _failedAttempts = 0;
            _lockedUntil = DateTime.UtcNow.AddSeconds(seconds);
            return new LoginResult(LoginStatus.LockedOut, seconds);
        }

        return new LoginResult(LoginStatus.InvalidCredentials);
    }

    // ===================== مساعدات =====================

    private static string? ValidatePassword(string password, string confirm)
    {
        if (password.Length < MinPasswordLength)
            return $"Password must be at least {MinPasswordLength} characters.";

        if (!password.Any(char.IsLetter) || !password.Any(char.IsDigit))
            return "Password must contain both letters and numbers.";

        if (password != confirm)
            return "Passwords do not match.";

        return null;
    }

    private static byte[] Hash(string password, byte[] salt, int iterations) =>
        Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, HashSize);

    private static string NormalizeEmail(string email) => (email ?? "").Trim().ToLowerInvariant();

    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        try
        {
            var addr = new MailAddress(email);
            return addr.Address == email && email.Contains('.');
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private static AccountData? LoadAccount()
    {
        try
        {
            return File.Exists(AccountFile)
                ? JsonSerializer.Deserialize<AccountData>(File.ReadAllText(AccountFile))
                : null;
        }
        catch
        {
            return null;
        }
    }
}