using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace SmartLib.UiTests;

public sealed class KorisnikUiTests : SmartLibUiTest
{
    // ── Helpers ────────────────────────────────────────────────────────────────

    private async Task LoginAsLibrarian()
    {
        await Page.GotoAsync("/Auth/Login");
        await Page.GetByLabel("Email").FillAsync(UiTestSettings.LibrarianEmail);
        await Page.GetByLabel("Lozinka").FillAsync(UiTestSettings.SharedSeedPassword);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Prijavi se" }).ClickAsync();
    }

    private async Task LoginAsMember()
    {
        await Page.GotoAsync("/Auth/Login");
        await Page.GetByLabel("Email").FillAsync(UiTestSettings.MemberEmail);
        await Page.GetByLabel("Lozinka").FillAsync(UiTestSettings.SharedSeedPassword);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Prijavi se" }).ClickAsync();
    }

    /// <summary>
    /// Clicks the first row in the member list (rows navigate via onclick JS,
    /// there are no visible link elements).
    /// </summary>
    private async Task ClickFirstMemberRow()
    {
        await Page.GotoAsync("/Korisnik");
        // Rows use class="members-row-link" and navigate via onclick
        await Page.Locator("tr.members-row-link").First.ClickAsync();
    }

    // ── Tests ──────────────────────────────────────────────────────────────────

    [Test]
    public async Task Librarian_CanViewMemberList()
    {
        await LoginAsLibrarian();
        await Page.GotoAsync("/Korisnik");

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Članovi biblioteke" }))
            .ToBeVisibleAsync();
    }

    [Test]
    public async Task Librarian_MemberList_HasSearchBox()
    {
        await LoginAsLibrarian();
        await Page.GotoAsync("/Korisnik");

        await Expect(
            Page.GetByPlaceholder("Pretraga po imenu, prezimenu ili e-mailu...")
        ).ToBeVisibleAsync();
    }

    [Test]
    public async Task Librarian_MemberList_SearchByName_FiltersResults()
    {
        await LoginAsLibrarian();
        await Page.GotoAsync("/Korisnik");

        await Page.GetByPlaceholder("Pretraga po imenu, prezimenu ili e-mailu...")
            .FillAsync("zzznepronalazi999");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Traži" }).ClickAsync();

        await Expect(Page.GetByText("Nema članova koji odgovaraju pretrazi."))
            .ToBeVisibleAsync();
    }

    [Test]
    public async Task Librarian_CanOpenCreateMemberForm()
    {
        await LoginAsLibrarian();
        await Page.GotoAsync("/Korisnik/Create");

        // Labels come from asp-for: "Ime", "Prezime", "Email adresa", "Lozinka", "Potvrdi lozinku"
        // Use Exact=true for "Ime" because "Prezime" contains "ime" and would also match
        await Expect(Page.GetByLabel("Ime", new() { Exact = true })).ToBeVisibleAsync();
        await Expect(Page.GetByLabel("Prezime")).ToBeVisibleAsync();
        await Expect(Page.GetByLabel("Email adresa")).ToBeVisibleAsync();
        await Expect(Page.GetByLabel("Lozinka", new() { Exact = true })).ToBeVisibleAsync();
        await Expect(Page.GetByLabel("Potvrdi lozinku")).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Kreiraj nalog" }))
            .ToBeVisibleAsync();
    }

    [Test]
    public async Task Librarian_CreateMember_DuplicateEmail_ShowsError()
    {
        await LoginAsLibrarian();
        await Page.GotoAsync("/Korisnik/Create");

        await Page.GetByLabel("Ime", new() { Exact = true }).FillAsync("Test");
        await Page.GetByLabel("Prezime").FillAsync("Duplikat");
        await Page.GetByLabel("Email adresa").FillAsync(UiTestSettings.MemberEmail);
        await Page.GetByLabel("Lozinka", new() { Exact = true }).FillAsync(UiTestSettings.SharedSeedPassword);
        await Page.GetByLabel("Potvrdi lozinku").FillAsync(UiTestSettings.SharedSeedPassword);

        await Page.GetByRole(AriaRole.Button, new() { Name = "Kreiraj nalog" }).ClickAsync();

        await Expect(Page.GetByText("Ta email adresa je već registrovana."))
            .ToBeVisibleAsync();
    }

    [Test]
    public async Task Librarian_CanViewMemberProfile()
    {
        await LoginAsLibrarian();
        await ClickFirstMemberRow();

        // Profile page shows at least one heading (member's name or "Profil")
        await Expect(Page.GetByRole(AriaRole.Heading)).ToBeVisibleAsync();
        // URL changes to ProfilClana
        await Expect(Page).ToHaveURLAsync(new Regex("ProfilClana", RegexOptions.IgnoreCase));
    }

    [Test]
    public async Task Member_CanViewOwnProfil()
    {
        await LoginAsMember();
        await Page.GotoAsync("/Korisnik/Profil");

        // Own profile must show the member's email address somewhere on the page
        await Expect(Page.GetByText(UiTestSettings.MemberEmail)).ToBeVisibleAsync();
    }

    [Test]
    public async Task AnonymousUser_KorisnikIndex_RedirectsToLogin()
    {
        await Page.GotoAsync("/Korisnik");

        await Expect(Page).Not.ToHaveURLAsync(
            new Regex("/Korisnik$", RegexOptions.IgnoreCase));
    }

    [Test]
    public async Task Member_KorisnikIndex_DoesNotShowMemberList()
    {
        await LoginAsMember();
        await Page.GotoAsync("/Korisnik");

        // Members lack the Bibliotekar/Administrator role — the list heading must not appear
        await Expect(
            Page.GetByRole(AriaRole.Heading, new() { Name = "Članovi biblioteke" })
        ).Not.ToBeVisibleAsync();
    }
}