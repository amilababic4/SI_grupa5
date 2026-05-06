namespace SmartLib.UiTests;

internal static class UiTestSettings
{
    /// <summary>Running SmartLib.Web base URL (trailing slash stripped). Override with SMARTLIB_BASE_URL.</summary>
    public static string BaseUrl =>
        (Environment.GetEnvironmentVariable("SMARTLIB_BASE_URL") ?? "https://localhost:5000").TrimEnd('/');

    /// <summary>
    /// Plaintext password for accounts that share the seed hash in ApplicationDbContext / SQL dumps
    /// (admin@, bibliotekar@, clan@). Override with SMARTLIB_E2E_PASSWORD if your DB differs.
    /// </summary>
    public static string SharedSeedPassword =>
        Environment.GetEnvironmentVariable("SMARTLIB_E2E_PASSWORD") ?? "Password123!";

    public const string MemberEmail = "clan@smartlib.ba";
    public const string LibrarianEmail = "bibliotekar@smartlib.ba";
}
