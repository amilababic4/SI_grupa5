using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace SmartLib.UiTests;

public sealed class ClanarinaUiTests : SmartLibUiTest
{
    // ═══════════════════════════════════════════════════════════════
    // BIBLIOTEKAR — Upsert (kreiranje nove članarine)
    // ═══════════════════════════════════════════════════════════════

    [Test]
    public async Task Bibliotekar_AfterLogin_CanOpenUpsertPageForMember()
    {
        await LoginAsBibliotekar();

        // Pretpostavka: korisnik s ID=2 postoji u seed podacima
        await Page.GotoAsync("/Clanarina/Upsert?korisnikId=2");

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Evidentiraj novu članarinu" })
            .Or(Page.GetByRole(AriaRole.Heading, new() { Name = "Upravljanje članarinom" })))
            .ToBeVisibleAsync();

        await Expect(Page.Locator("input[name='DatumPocetka']")).ToBeVisibleAsync();
        await Expect(Page.Locator("input[name='DatumIsteka']")).ToBeVisibleAsync();
    }

    [Test]
    public async Task Bibliotekar_UpsertClanarina_SuccessRedirectsToProfilClana()
    {
        await LoginAsBibliotekar();

        await Page.GotoAsync("/Clanarina/Upsert?korisnikId=2");

        await Page.Locator("input[name='DatumPocetka']").FillAsync("2025-01-01");
        await Page.Locator("input[name='DatumIsteka']").FillAsync("2026-01-01");

        await Page.RunAndWaitForNavigationAsync(async () =>
        {
            await Page.GetByRole(AriaRole.Button, new() { Name = "Evidentiraj članarinu" })
                .Or(Page.GetByRole(AriaRole.Button, new() { Name = "Spremi izmjene" }))
                .ClickAsync();
        });

        // Trebalo bi preusmjeriti na ProfilClana
        Assert.That(Page.Url, Does.Contain("/Korisnik/ProfilClana"));
    }

    [Test]
    public async Task Bibliotekar_UpsertClanarina_InvalidDates_ShowsValidationError()
    {
        await LoginAsBibliotekar();

        await Page.GotoAsync("/Clanarina/Upsert?korisnikId=2");

        // Datum isteka prije datuma početka
        await Page.Locator("input[name='DatumPocetka']").FillAsync("2026-06-01");
        await Page.Locator("input[name='DatumIsteka']").FillAsync("2025-01-01");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Evidentiraj članarinu" })
            .Or(Page.GetByRole(AriaRole.Button, new() { Name = "Spremi izmjene" }))
            .ClickAsync();

        await Expect(Page.Locator("span[data-valmsg-for='DatumIsteka']")).ToHaveTextAsync("Datum isteka mora biti nakon datuma početka.");
    }

    [Test]
    public async Task Clan_CannotAccess_UpsertPage()
    {
        await LoginAsClan();

        await Page.GotoAsync("/Clanarina/Upsert?korisnikId=2");

        // Clan ne smije pristupiti — redirect na login ili Access Denied
        Assert.That(
            Page.Url.Contains("/Auth/Login") ||
            Page.Url.Contains("/Account/AccessDenied") ||
            Page.Url.Contains("/Forbidden"),
            Is.True,
            $"Očekivano preusmjeravanje, ali URL je: {Page.Url}");
    }

    // ═══════════════════════════════════════════════════════════════
    // BIBLIOTEKAR — ZahtjeviProduzenja
    // ═══════════════════════════════════════════════════════════════

    [Test]
    public async Task Bibliotekar_CanOpenZahtjeviProduzenja()
    {
        await LoginAsBibliotekar();

        await Page.GotoAsync("/Clanarina/ZahtjeviProduzenja");

        await Expect(Page.GetByRole(AriaRole.Heading,
            new() { Name = "Zahtjevi za produženje članarine" })).ToBeVisibleAsync();
    }

    [Test]
    public async Task Bibliotekar_ZahtjeviProduzenja_WhenEmpty_ShowsEmptyState()
    {
        await LoginAsBibliotekar();

        await Page.GotoAsync("/Clanarina/ZahtjeviProduzenja");

        // Ako nema zahtjeva — prikazuje se prazno stanje
        var prazno = Page.GetByText("Nema zahtjeva na čekanju");
        var lista = Page.Locator(".zah3-lista");

        var praznoVisible = await prazno.IsVisibleAsync();
        var listaVisible = await lista.IsVisibleAsync();

        Assert.That(praznoVisible || listaVisible, Is.True,
            "Stranica mora prikazati ili prazno stanje ili listu zahtjeva.");
    }

    [Test]
    public async Task Clan_CannotAccess_ZahtjeviProduzenja()
    {
        await LoginAsClan();

        await Page.GotoAsync("/Clanarina/ZahtjeviProduzenja");

        Assert.That(
            Page.Url.Contains("/Auth/Login") ||
            Page.Url.Contains("/Account/AccessDenied") ||
            Page.Url.Contains("/Forbidden"),
            Is.True,
            $"Očekivano preusmjeravanje, ali URL je: {Page.Url}");
    }

    // ═══════════════════════════════════════════════════════════════
    // CLAN — ProduzenjeClanarine
    // ═══════════════════════════════════════════════════════════════

    [Test]
    public async Task Clan_AfterLogin_CanOpenProduzenjeClanarine()
    {
        await LoginAsClan();

        await Page.GotoAsync("/Clanarina/ProduzenjeClanarine");

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Produženje članarine" }))
            .ToBeVisibleAsync();
    }

    [Test]
    public async Task Clan_ProduzenjeClanarine_ShowsCurrentMembershipStatus()
    {
        await LoginAsClan();

        await Page.GotoAsync("/Clanarina/ProduzenjeClanarine");

        // Status bannera ili forma za zahtjev moraju biti vidljivi
        var statusBanner = Page.Locator(".prod3-status-banner");
        var forma = Page.Locator("form[action*='PodnesizahtjevProduzenja']");

        var statusVisible = await statusBanner.IsVisibleAsync();
        var formaVisible = await forma.IsVisibleAsync();

        Assert.That(statusVisible || formaVisible, Is.True,
            "Stranica mora prikazati status članarine ili formu za zahtjev.");
    }

    [Test]
    public async Task Clan_ProduzenjeClanarine_ShowsHistorySection()
    {
        await LoginAsClan();

        await Page.GotoAsync("/Clanarina/ProduzenjeClanarine");

        // Sekcija historije uvijek treba biti vidljiva
        await Expect(Page.GetByText("Historija zahtjeva")).ToBeVisibleAsync();
    }

    [Test]
    public async Task Clan_ProduzenjeClanarine_ActiveRequest_ShowsPendingCard()
    {
        await LoginAsClan();

        await Page.GotoAsync("/Clanarina/ProduzenjeClanarine");

        var cekanje = Page.GetByText("Zahtjev na čekanju");
        var forma = Page.Locator("form[action*='PodnesizahtjevProduzenja']");

        // Jedno od dvoga mora biti vidljivo
        var cekanjeVisible = await cekanje.IsVisibleAsync();
        var formaVisible = await forma.IsVisibleAsync();

        Assert.That(cekanjeVisible || formaVisible, Is.True,
            "Mora biti prikazana ili kartica čekanja ili forma za novi zahtjev.");
    }

    [Test]
    public async Task Bibliotekar_CannotAccess_ProduzenjeClanarine()
    {
        await LoginAsBibliotekar();

        await Page.GotoAsync("/Clanarina/ProduzenjeClanarine");

        Assert.That(
            Page.Url.Contains("/Auth/Login") ||
            Page.Url.Contains("/Account/AccessDenied") ||
            Page.Url.Contains("/Forbidden"),
            Is.True,
            $"Bibliotekar ne smije pristupiti stranici člana. URL: {Page.Url}");
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