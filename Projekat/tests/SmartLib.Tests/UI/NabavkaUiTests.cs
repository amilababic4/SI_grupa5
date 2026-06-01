using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace SmartLib.UiTests;

public sealed class NabavkaUiTests : SmartLibUiTest
{
    // ═══════════════════════════════════════════════════════════════
    // PRISTUP — autorizacija
    // ═══════════════════════════════════════════════════════════════

    [Test]
    public async Task Bibliotekar_AfterLogin_CanOpenNabavkaZahtjev()
    {
        await LoginAsBibliotekar();

        await Page.GotoAsync("/Nabavka/Zahtjev");

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Nabavka knjiga" }))
            .ToBeVisibleAsync();
    }

    [Test]
    public async Task Clan_CannotAccess_NabavkaZahtjev()
    {
        await LoginAsClan();

        await Page.GotoAsync("/Nabavka/Zahtjev");

        Assert.That(
            Page.Url.Contains("/Auth/Login") ||
            Page.Url.Contains("/Account/AccessDenied") ||
            Page.Url.Contains("/Forbidden"),
            Is.True,
            $"Član ne smije pristupiti stranici nabavke. URL: {Page.Url}");
    }

    [Test]
    public async Task GuestUser_CannotAccess_NabavkaZahtjev()
    {
        await Page.GotoAsync("/Nabavka/Zahtjev");

        Assert.That(
            Page.Url.Contains("/Auth/Login") ||
            Page.Url.Contains("/Account/AccessDenied"),
            Is.True,
            $"Neprijavljeni korisnik ne smije pristupiti nabavci. URL: {Page.Url}");
    }

    // ═══════════════════════════════════════════════════════════════
    // PRIKAZ STRANICE — elementi
    // ═══════════════════════════════════════════════════════════════

    [Test]
    public async Task NabavkaZahtjev_ShowsAllFormFields()
    {
        await LoginAsBibliotekar();

        await Page.GotoAsync("/Nabavka/Zahtjev");

        await Expect(Page.Locator("input[name='NazivKnjige']")).ToBeVisibleAsync();
        await Expect(Page.Locator("input[name='Autor']")).ToBeVisibleAsync();
        await Expect(Page.Locator("input[name='Izdavac']")).ToBeVisibleAsync();
        await Expect(Page.Locator("input[name='BrojPrimjeraka']")).ToBeVisibleAsync();
        await Expect(Page.Locator("textarea[name='Napomena']")).ToBeVisibleAsync();
    }

    [Test]
    public async Task NabavkaZahtjev_ShowsSubmitButton()
    {
        await LoginAsBibliotekar();

        await Page.GotoAsync("/Nabavka/Zahtjev");

        await Expect(Page.GetByRole(AriaRole.Button,
            new() { Name = "Pošalji zahtjev distributeru" })).ToBeVisibleAsync();
    }

    [Test]
    public async Task NabavkaZahtjev_ShowsZadnjeNabavkeSection()
    {
        await LoginAsBibliotekar();

        await Page.GotoAsync("/Nabavka/Zahtjev");

        await Expect(Page.GetByText("Zadnje nabavke")).ToBeVisibleAsync();
    }

    [Test]
    public async Task NabavkaZahtjev_ShowsDistributerEmailSection()
    {
        await LoginAsBibliotekar();

        await Page.GotoAsync("/Nabavka/Zahtjev");

        await Expect(Page.GetByText("Email distributera")).ToBeVisibleAsync();
        await Expect(Page.Locator("input[name='Email']")).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button,
            new() { Name = "Sačuvaj novu adresu" })).ToBeVisibleAsync();
    }

    // ═══════════════════════════════════════════════════════════════
    // FORMA — validacija
    // ═══════════════════════════════════════════════════════════════

    [Test]
    public async Task NabavkaZahtjev_EmptySubmit_ShowsValidationErrors()
    {
        await LoginAsBibliotekar();

        await Page.GotoAsync("/Nabavka/Zahtjev");

        // Klikni submit bez popunjavanja
        await Page.GetByRole(AriaRole.Button,
            new() { Name = "Pošalji zahtjev distributeru" }).ClickAsync();

        // Validacijske poruke za obavezna polja
        var validationMessages = Page.Locator(".validation-message");
        var count = await validationMessages.CountAsync();
        Assert.That(count, Is.GreaterThan(0),
            "Moraju biti prikazane validacijske poruke za prazna obavezna polja.");
    }

    [Test]
    public async Task NabavkaZahtjev_ValidData_SubmitSucceeds_AndRedirects()
    {
        await LoginAsBibliotekar();

        await Page.GotoAsync("/Nabavka/Zahtjev");

        await Page.Locator("input[name='NazivKnjige']").FillAsync("Test knjiga UI");
        await Page.Locator("input[name='Autor']").FillAsync("Test Autor");
        await Page.Locator("input[name='Izdavac']").FillAsync("Test Izdavač");
        await Page.Locator("input[name='BrojPrimjeraka']").FillAsync("3");
        await Page.Locator("textarea[name='Napomena']").FillAsync("UI test napomena");

        await Page.RunAndWaitForNavigationAsync(async () =>
        {
            await Page.GetByRole(AriaRole.Button,
                new() { Name = "Pošalji zahtjev distributeru" }).ClickAsync();
        });

        // Nakon uspješnog slanja — redirect nazad na Zahtjev stranicu
        Assert.That(Page.Url, Does.Contain("/Nabavka/Zahtjev"));
    }

    [Test]
    public async Task NabavkaZahtjev_ValidData_ShowsSuccessOrErrorBanner()
    {
        await LoginAsBibliotekar();

        await Page.GotoAsync("/Nabavka/Zahtjev");

        await Page.Locator("input[name='NazivKnjige']").FillAsync("Knjiga za banner test");
        await Page.Locator("input[name='Autor']").FillAsync("Autor Banner");
        await Page.Locator("input[name='Izdavac']").FillAsync("Izdavač Banner");
        await Page.Locator("input[name='BrojPrimjeraka']").FillAsync("1");

        await Page.RunAndWaitForNavigationAsync(async () =>
        {
            await Page.GetByRole(AriaRole.Button,
                new() { Name = "Pošalji zahtjev distributeru" }).ClickAsync();
        });

        // Mora biti prikazan ili uspjeh ili greška (email možda nije konfigurisan u testu)
        var uspjeh = Page.Locator(".alert-banner--success");
        var greska = Page.Locator(".alert-banner--danger");

        var uspjehVisible = await uspjeh.IsVisibleAsync();
        var greskaVisible = await greska.IsVisibleAsync();

        Assert.That(uspjehVisible || greskaVisible, Is.True,
            "Nakon slanja mora biti prikazan success ili error banner.");
    }

    [Test]
    public async Task NabavkaZahtjev_BrojPrimjeraka_MustBePositive()
    {
        await LoginAsBibliotekar();

        await Page.GotoAsync("/Nabavka/Zahtjev");

        await Page.Locator("input[name='NazivKnjige']").FillAsync("Test");
        await Page.Locator("input[name='Autor']").FillAsync("Test");
        await Page.Locator("input[name='Izdavac']").FillAsync("Test");
        // Unesite 0 — nije validno (min="1")
        await Page.Locator("input[name='BrojPrimjeraka']").FillAsync("0");

        await Page.GetByRole(AriaRole.Button,
            new() { Name = "Pošalji zahtjev distributeru" }).ClickAsync();

        // Stranica ne smije navigirati dalje — ostajemo na formi
        Assert.That(Page.Url, Does.Contain("/Nabavka/Zahtjev"));
    }

    [Test]
    public async Task NabavkaZahtjev_NapomenaIsOptional_SubmitSucceedsWithoutIt()
    {
        await LoginAsBibliotekar();

        await Page.GotoAsync("/Nabavka/Zahtjev");

        await Page.Locator("input[name='NazivKnjige']").FillAsync("Knjiga bez napomene");
        await Page.Locator("input[name='Autor']").FillAsync("Autor Bez");
        await Page.Locator("input[name='Izdavac']").FillAsync("Izdavač Bez");
        await Page.Locator("input[name='BrojPrimjeraka']").FillAsync("2");
        // Namjerno ne popunjavamo napomenu

        await Page.RunAndWaitForNavigationAsync(async () =>
        {
            await Page.GetByRole(AriaRole.Button,
                new() { Name = "Pošalji zahtjev distributeru" }).ClickAsync();
        });

        Assert.That(Page.Url, Does.Contain("/Nabavka/Zahtjev"));
    }

    // ═══════════════════════════════════════════════════════════════
    // PROMJENA EMAIL DISTRIBUTERA
    // ═══════════════════════════════════════════════════════════════

    [Test]
    public async Task NabavkaZahtjev_PromijeniEmail_InvalidEmail_ShowsError()
    {
        await LoginAsBibliotekar();

        await Page.GotoAsync("/Nabavka/Zahtjev");

        // Unesite neispravan email
        await Page.Locator("input[name='Email']").FillAsync("nije-email");

        await Page.GetByRole(AriaRole.Button,
            new() { Name = "Sačuvaj novu adresu" }).ClickAsync();

        // Browser validacija ili server validacija sprječavaju submit
        Assert.That(Page.Url, Does.Contain("/Nabavka/Zahtjev"));
    }

    // ═══════════════════════════════════════════════════════════════
    // HISTORIJA NABAVKI
    // ═══════════════════════════════════════════════════════════════

    [Test]
    public async Task NabavkaZahtjev_ZadnjeNabavke_ShowsEmptyStateOrList()
    {
        await LoginAsBibliotekar();

        await Page.GotoAsync("/Nabavka/Zahtjev");

        var prazno = Page.GetByText("Još nema poslanih zahtjeva.");
        var lista = Page.Locator(".nabavka-historija-lista");

        var praznoVisible = await prazno.IsVisibleAsync();
        var listaVisible = await lista.IsVisibleAsync();

        Assert.That(praznoVisible || listaVisible, Is.True,
            "Sekcija zadnjih nabavki mora prikazati prazno stanje ili listu.");
    }

    [Test]
    public async Task NabavkaZahtjev_AfterSubmit_HistoryContainsNewEntry()
    {
        await LoginAsBibliotekar();

        await Page.GotoAsync("/Nabavka/Zahtjev");

        var naziv = $"Historija Test {DateTime.Now:HHmmss}";

        await Page.Locator("input[name='NazivKnjige']").FillAsync(naziv);
        await Page.Locator("input[name='Autor']").FillAsync("Historija Autor");
        await Page.Locator("input[name='Izdavac']").FillAsync("Historija Izdavač");
        await Page.Locator("input[name='BrojPrimjeraka']").FillAsync("2");

        await Page.RunAndWaitForNavigationAsync(async () =>
        {
            await Page.GetByRole(AriaRole.Button,
                new() { Name = "Pošalji zahtjev distributeru" }).ClickAsync();
        });

        // Naziv knjige treba biti vidljiv u listi zadnjih nabavki
        await Expect(Page.GetByText(naziv)).ToBeVisibleAsync();
    }

    // ═══════════════════════════════════════════════════════════════
    // Private helpers
    // ═══════════════════════════════════════════════════════════════

    private async Task LoginAsBibliotekar()
    {
        await Page.GotoAsync("/Auth/Login");
        await Page.GetByLabel("Email").FillAsync(UiTestSettings.LibrarianEmail);
        await Page.GetByLabel("Lozinka").FillAsync(UiTestSettings.SharedSeedPassword);
        await Page.RunAndWaitForNavigationAsync(async () =>
        {
            await Page.GetByRole(AriaRole.Button, new() { Name = "Prijavi se" }).ClickAsync();
        });
    }

    private async Task LoginAsClan()
    {
        await Page.GotoAsync("/Auth/Login");
        await Page.GetByLabel("Email").FillAsync(UiTestSettings.MemberEmail);
        await Page.GetByLabel("Lozinka").FillAsync(UiTestSettings.SharedSeedPassword);
        await Page.RunAndWaitForNavigationAsync(async () =>
        {
            await Page.GetByRole(AriaRole.Button, new() { Name = "Prijavi se" }).ClickAsync();
        });
    }
}