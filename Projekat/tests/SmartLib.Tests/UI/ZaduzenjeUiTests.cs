using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace SmartLib.UiTests;

public sealed class ZaduzenjeUiTests : SmartLibUiTest
{
    // ── Helpers ────────────────────────────────────────────────────────────────

    private async Task LoginAsLibrarian()
    {
        await Page.GotoAsync("/Auth/Login");
        await Page.GetByLabel("Email").FillAsync(UiTestSettings.LibrarianEmail);
        await Page.GetByLabel("Lozinka").FillAsync(UiTestSettings.SharedSeedPassword);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Prijavi se" }).ClickAsync();
    }

    /// <summary>
    /// Returns every active loan in the DB so that all book copies are "dostupan"
    /// before the create-loan test runs. Prevents timeout caused by a dirty DB
    /// from a previous (possibly cancelled) test run.
    /// </summary>
    private async Task ReturnAllActiveLoans()
    {
        await Page.GotoAsync("/Zaduzenje");
        var detaljiLinks = Page.GetByRole(AriaRole.Link, new() { Name = "Detalji" });

        while (await detaljiLinks.CountAsync() > 0)
        {
            await detaljiLinks.First.ClickAsync();

            var returnLink = Page.GetByRole(AriaRole.Link, new() { Name = "Evidentiraj vraćanje" });
            if (await returnLink.CountAsync() > 0)
            {
                await returnLink.ClickAsync();
                await Page.GetByRole(AriaRole.Button, new() { Name = "Da, evidentiraj vraćanje" }).ClickAsync();
            }
            else
            {
                // Loan already closed or no return action available — go back to list
                await Page.GotoAsync("/Zaduzenje");
            }

            await Page.GotoAsync("/Zaduzenje");
        }
    }

    // ── Tests ──────────────────────────────────────────────────────────────────

    [Test]
    public async Task Librarian_CanCreateZaduzenje_ShowsSuccessMessage()
    {
        await LoginAsLibrarian();

        // Return any leftover active loans so book copies are "dostupan"
        await ReturnAllActiveLoans();

        // Create a new loan
        await Page.GotoAsync("/Zaduzenje");
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Aktivna zaduženja" })).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "+ Novo zaduživanje" }).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Novo zaduživanje" })).ToBeVisibleAsync();

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
        await LoginAsLibrarian();

        // Navigate to active loans — Test 1 already created one
        await Page.GotoAsync("/Zaduzenje");
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Aktivna zaduženja" })).ToBeVisibleAsync();

        // Open details of the first active loan and return it
        await Page.GetByRole(AriaRole.Link, new() { Name = "Detalji" }).Nth(0).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Evidentiraj vraćanje" }).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Evidentiraj vraćanje" })).ToBeVisibleAsync();

        await Page.GetByRole(AriaRole.Button, new() { Name = "Da, evidentiraj vraćanje" }).ClickAsync();

        await Expect(Page.GetByText("Vraćanje knjige je uspješno evidentirano.")).ToBeVisibleAsync();
    }
}
