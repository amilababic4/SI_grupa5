using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace SmartLib.UiTests;

public sealed class ZaduzenjeUiTests : SmartLibUiTest
{
    [Test]
    public async Task Librarian_CanCreateZaduzenje_ShowsSuccessMessage()
    {
        await Page.GotoAsync("/Auth/Login");
        await Page.GetByLabel("Email").FillAsync(UiTestSettings.LibrarianEmail);
        await Page.GetByLabel("Lozinka").FillAsync(UiTestSettings.SharedSeedPassword);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Prijavi se" }).ClickAsync();

        await Page.GotoAsync("/Zaduzenje");
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Aktivna zaduženja" })).ToBeVisibleAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "+ Novo zaduživanje" }).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Novo zaduživanje" })).ToBeVisibleAsync();

        // Odaberi člana, knjigu i primjerak iz combo komponenti
        await Page.GetByLabel("Član").ClickAsync();
        await Page.Locator("#combo-clan-list .zad-combo-item").Nth(0).ClickAsync();

        await Page.GetByLabel("Knjiga").ClickAsync();
        await Page.Locator("#combo-knjiga-list .zad-combo-item").Nth(0).ClickAsync();

        await Page.GetByLabel("Primjerak").ClickAsync();
        await Page.Locator("#combo-primjerak-list .zad-combo-item").Nth(0).ClickAsync();

        await Page.GetByRole(AriaRole.Button, new() { Name = "Potvrdi zaduživanje" }).ClickAsync();

        await Expect(Page.GetByText("Zaduživanje je uspješno evidentirano.")).ToBeVisibleAsync();
    }

    [Test]
    public async Task Librarian_CanReturnBook_ShowsSuccessMessage()
    {
        await Page.GotoAsync("/Auth/Login");
        await Page.GetByLabel("Email").FillAsync(UiTestSettings.LibrarianEmail);
        await Page.GetByLabel("Lozinka").FillAsync(UiTestSettings.SharedSeedPassword);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Prijavi se" }).ClickAsync();

        // Kreiraj novo zaduženje kao pripremu za vraćanje
        await Page.GotoAsync("/Zaduzenje");
        await Page.GetByRole(AriaRole.Link, new() { Name = "+ Novo zaduživanje" }).ClickAsync();

        await Page.GetByLabel("Član").ClickAsync();
        await Page.Locator("#combo-clan-list .zad-combo-item").Nth(0).ClickAsync();
        await Page.GetByLabel("Knjiga").ClickAsync();
        await Page.Locator("#combo-knjiga-list .zad-combo-item").Nth(0).ClickAsync();
        await Page.GetByLabel("Primjerak").ClickAsync();
        await Page.Locator("#combo-primjerak-list .zad-combo-item").Nth(0).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Potvrdi zaduživanje" }).ClickAsync();

        // Otvori detalje prvog zaduženja i evidentiraj vraćanje
        await Page.GetByRole(AriaRole.Link, new() { Name = "Detalji" }).Nth(0).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Evidentiraj vraćanje" }).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Evidentiraj vraćanje" })).ToBeVisibleAsync();

        await Page.GetByRole(AriaRole.Button, new() { Name = "Da, evidentiraj vraćanje" }).ClickAsync();

        await Expect(Page.GetByText("Vraćanje knjige je uspješno evidentirano.")).ToBeVisibleAsync();
    }
}
