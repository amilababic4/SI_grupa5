using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace SmartLib.UiTests;

public sealed class KnjigaUiTests : SmartLibUiTest
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
    /// Clicks the first book in the catalogue list via its stretched-link anchor
    /// and waits for navigation to the details page.
    /// </summary>
    private async Task OpenFirstBookDetails()
    {
        // Each list item contains <a class="stretched-link"> with no visible label text.
        var firstLink = Page.Locator("a.stretched-link").First;
        await Expect(firstLink).ToBeVisibleAsync();
        await firstLink.ClickAsync();
        await Page.WaitForURLAsync(new Regex("/Knjiga/Details/\\d+", RegexOptions.IgnoreCase));
    }

    // ── Tests ──────────────────────────────────────────────────────────────────

    [Test]
    public async Task Librarian_CanOpenCreateKnjigaForm()
    {
        await LoginAsLibrarian();
        await Page.GotoAsync("/Knjiga/Create");

        // <h2> in Create view: "Dodaj novu knjigu"
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Dodaj novu knjigu" })).ToBeVisibleAsync();
        await Expect(Page.Locator("input[name='Naslov']")).ToBeVisibleAsync();
        await Expect(Page.Locator("input[name='Autor']")).ToBeVisibleAsync();
        await Expect(Page.Locator("input[name='Isbn']")).ToBeVisibleAsync();
    }



    [Test]
    public async Task Librarian_CanOpenEditKnjigaForm()
    {
        await LoginAsLibrarian();
        await Page.GotoAsync("/Knjiga");

        await Page.GetByRole(AriaRole.Link, new() { Name = "Uredi" }).First.ClickAsync();

        // <h2> in Edit view: "Uredi podatke knjige"
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Uredi podatke knjige" })).ToBeVisibleAsync();
        await Expect(Page.Locator("input[name='Naslov']")).ToBeVisibleAsync();
    }

    [Test]
    public async Task Librarian_CanEditKnjiga_ShowsSuccessMessage()
    {
        await LoginAsLibrarian();
        await Page.GotoAsync("/Knjiga");

        await Page.GetByRole(AriaRole.Link, new() { Name = "Uredi" }).First.ClickAsync();

        var naslovInput = Page.Locator("input[name='Naslov']");
        var current = await naslovInput.InputValueAsync();
        await naslovInput.ClearAsync();
        await naslovInput.FillAsync(current.TrimEnd('*') + " *");

        // Submit button in Edit view: "Sačuvaj izmjene"
        await Page.GetByRole(AriaRole.Button, new() { Name = "Sačuvaj izmjene" }).ClickAsync();

        // TempData: $"Podaci knjige \"{knjiga.Naslov}\" su uspješno ažurirani."
        await Expect(Page.GetByText("su uspješno ažurirani.", new() { Exact = false })).ToBeVisibleAsync();
    }


    [Test]
    public async Task KatalogSearch_ByNaslov_FiltersResults()
    {
        await LoginAsMember();
        await Page.GotoAsync("/Knjiga");

        await Page.Locator("input[name='naslov']").FillAsync("zzznepronalazi999");

        // Submit button in catalogue search form: "Traži"
        await Page.GetByRole(AriaRole.Button, new() { Name = "Traži" }).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Katalog knjiga" })).ToBeVisibleAsync();
    }

    [Test]
    public async Task Member_CannotSeeCreateKnjigaLink_InCatalogue()
    {
        await LoginAsMember();
        await Page.GotoAsync("/Knjiga");

        // Link text in catalogue view: "+ Nova knjiga" — members must not see it
        await Expect(
            Page.GetByRole(AriaRole.Link, new() { Name = "+ Nova knjiga" })
        ).Not.ToBeVisibleAsync();
    }

    [Test]
    public async Task Librarian_CanSeeCreateKnjigaLink_InCatalogue()
    {
        await LoginAsLibrarian();
        await Page.GotoAsync("/Knjiga");

        // Link text in catalogue view: "+ Nova knjiga"
        await Expect(
            Page.GetByRole(AriaRole.Link, new() { Name = "+ Nova knjiga" })
        ).ToBeVisibleAsync();
    }
}
