using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace SmartLib.UiTests;

public sealed class KategorijaUiTests : SmartLibUiTest
{
    // ── Helpers ────────────────────────────────────────────────────────────────

    private async Task LoginAsLibrarian()
    {
        await Page.GotoAsync("/Auth/Login");
        await Page.GetByLabel("Email").FillAsync(UiTestSettings.LibrarianEmail);
        await Page.GetByLabel("Lozinka").FillAsync(UiTestSettings.SharedSeedPassword);
        await Page.RunAndWaitForNavigationAsync(async () => { await Page.GetByRole(AriaRole.Button, new() { Name = "Prijavi se" }).ClickAsync(); });
    }

    /// <summary>
    /// Opens the "Dodaj kategoriju" panel (if not already visible) and fills + submits the create form.
    /// </summary>
    private async Task CreateKategorija(string naziv, string? opis = null)
    {
        await Page.GotoAsync("/Kategorija");

        // The add-form is hidden by default — click the toggle button to show it.
        var addForm = Page.Locator("#forma-nova-kategorija");
        if (!await addForm.IsVisibleAsync())
        {
            await Page.GetByRole(AriaRole.Button, new() { Name = "+ Dodaj kategoriju" }).ClickAsync();
            await Expect(addForm).ToBeVisibleAsync();
        }

        await Page.Locator("#naziv-novi").FillAsync(naziv);

        if (!string.IsNullOrEmpty(opis))
            await Page.Locator("#opis-novi").FillAsync(opis);

        // Submit button inside the add form
        await addForm.GetByRole(AriaRole.Button, new() { Name = "Sačuvaj" }).ClickAsync();
    }

    /// <summary>
    /// Deletes a category by name if it exists and has no books (idempotent cleanup).
    /// </summary>
    private async Task DeleteCategoryIfExists(string naziv)
    {
        await Page.GotoAsync("/Kategorija");

        var row = Page.Locator("tbody tr", new() { HasText = naziv }).First;
        if (await row.CountAsync() == 0) return;

        // Only attempt delete if the row has a submit-based delete button (no books).
        var deleteForm = row.Locator("form[action*='Delete'] button[type='submit']");
        if (await deleteForm.CountAsync() == 0) return;

        Page.Dialog += (_, dialog) => dialog.AcceptAsync();
        await deleteForm.ClickAsync();
        await Page.WaitForURLAsync("**/Kategorija**");
    }

    // ── Tests ──────────────────────────────────────────────────────────────────

    [Test]
    public async Task Librarian_CanViewKategorijaList()
    {
        await LoginAsLibrarian();
        await Page.GotoAsync("/Kategorija");

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Kategorije knjiga" })).ToBeVisibleAsync();
    }

    [Test]
    public async Task Librarian_CanCreateKategorija_ShowsSuccessMessage()
    {
        await LoginAsLibrarian();

        const string naziv = "E2E Test Kategorija";
        await DeleteCategoryIfExists(naziv);

        await CreateKategorija(naziv);

        await Expect(Page.GetByText($"Kategorija \"{naziv}\" je uspješno dodana.")).ToBeVisibleAsync();

        // Cleanup
        await DeleteCategoryIfExists(naziv);
    }

    [Test]
    public async Task Librarian_CreateDuplicateKategorija_ShowsErrorMessage()
    {
        await LoginAsLibrarian();

        const string naziv = "E2E Duplikat Kategorija";
        await DeleteCategoryIfExists(naziv);

        // Create first time — expect success
        await CreateKategorija(naziv);
        await Expect(Page.GetByText($"Kategorija \"{naziv}\" je uspješno dodana.")).ToBeVisibleAsync();

        // Try to create again with the same name — expect duplicate error
        await CreateKategorija(naziv);
        await Expect(Page.GetByText($"Kategorija \"{naziv}\" već postoji u sistemu.")).ToBeVisibleAsync();

        // Cleanup
        await DeleteCategoryIfExists(naziv);
    }

    [Test]
    public async Task Librarian_CanEditKategorija_ShowsSuccessMessage()
    {
        await LoginAsLibrarian();

        const string originalNaziv = "E2E Edit Kategorija Original";
        const string updatedNaziv = "E2E Edit Kategorija Updated";

        await DeleteCategoryIfExists(originalNaziv);
        await DeleteCategoryIfExists(updatedNaziv);

        // Create category to edit
        await CreateKategorija(originalNaziv);
        await Expect(Page.GetByText($"Kategorija \"{originalNaziv}\" je uspješno dodana.")).ToBeVisibleAsync();

        await Page.GotoAsync("/Kategorija");

        // Find the table row that contains the category name
        var row = Page.Locator("tbody tr", new() { HasText = originalNaziv }).First;
        await Expect(row).ToBeVisibleAsync();

        // Click "Uredi" to reveal the inline edit row
        await row.GetByRole(AriaRole.Button, new() { Name = "Uredi" }).ClickAsync();

        // The inline edit row has id="edit-row-{id}" and loses the "hidden" class when open
        var editRow = Page.Locator("[id^='edit-row-']:not(.hidden)");
        await Expect(editRow).ToBeVisibleAsync();

        var nazivInput = editRow.Locator("input[name='naziv']");
        await nazivInput.ClearAsync();
        await nazivInput.FillAsync(updatedNaziv);

        await editRow.GetByRole(AriaRole.Button, new() { Name = "Sačuvaj" }).ClickAsync();

        await Expect(Page.GetByText($"Kategorija \"{updatedNaziv}\" je uspješno ažurirana.")).ToBeVisibleAsync();

        // Cleanup
        await DeleteCategoryIfExists(updatedNaziv);
    }

    [Test]
    public async Task AnonymousUser_Kategorija_RedirectsToLogin()
    {
        await Page.GotoAsync("/Kategorija");

        // Should be redirected away from /Kategorija (to login page)
        await Expect(Page).Not.ToHaveURLAsync(
            new System.Text.RegularExpressions.Regex(
                "/Kategorija$",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase));
    }
}