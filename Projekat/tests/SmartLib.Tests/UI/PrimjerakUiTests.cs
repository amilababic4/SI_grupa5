using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace SmartLib.UiTests;

// UI testovi za upravljanje primjercima knjige (US-21, US-24)
public sealed class PrimjerakUiTests : SmartLibUiTest
{
    // ── Helpers ────────────────────────────────────────────────────────────────

    private async Task LoginAsLibrarian()
    {
        await Page.GotoAsync("/Auth/Login");
        await Page.GetByLabel("Email").FillAsync(UiTestSettings.LibrarianEmail);
        await Page.GetByLabel("Lozinka").FillAsync(UiTestSettings.SharedSeedPassword);
        await Page.RunAndWaitForNavigationAsync(async () => { await Page.GetByRole(AriaRole.Button, new() { Name = "Prijavi se" }).ClickAsync(); });
    }

    private async Task LoginAsMember()
    {
        await Page.GotoAsync("/Auth/Login");
        await Page.GetByLabel("Email").FillAsync(UiTestSettings.MemberEmail);
        await Page.GetByLabel("Lozinka").FillAsync(UiTestSettings.SharedSeedPassword);
        await Page.RunAndWaitForNavigationAsync(async () => { await Page.GetByRole(AriaRole.Button, new() { Name = "Prijavi se" }).ClickAsync(); });
    }

    /// <summary>
    /// Navigates to the Details page of the first book by extracting the ID
    /// from the "Uredi" href — avoids relying on the zero-size stretched-link anchor.
    /// </summary>
    private async Task OpenFirstBookDetailsAsLibrarian()
    {
        await Page.GotoAsync("/Knjiga");

        // "Uredi" links have href="/Knjiga/Edit/{id}" — extract the ID from there
        var editLink = Page.GetByRole(AriaRole.Link, new() { Name = "Uredi" }).First;
        await Expect(editLink).ToBeVisibleAsync();
        var href = await editLink.GetAttributeAsync("href") ?? "";

        // href is "/Knjiga/Edit/42" — replace Edit with Details
        var detailsUrl = Regex.Replace(href, "/Edit/", "/Details/", RegexOptions.IgnoreCase);
        await Page.GotoAsync(detailsUrl);
        await Page.WaitForURLAsync(new Regex("/Knjiga/Details/\\d+", RegexOptions.IgnoreCase));
    }

    /// <summary>
    /// Navigates to the Details page of the first book for a member
    /// by hitting /Knjiga/Details/1 and falling back to scanning the page source for IDs.
    /// </summary>
    private async Task OpenFirstBookDetailsAsMember()
    {
        await Page.GotoAsync("/Knjiga");

        // Grab any /Knjiga/Details/{id} href present in the DOM (stretched-link anchors have them)
        var detailsHref = await Page.EvaluateAsync<string?>(@"
            () => {
                const a = [...document.querySelectorAll('a[href]')]
                    .find(el => /\/Knjiga\/Details\/\d+/i.test(el.getAttribute('href')));
                return a ? a.getAttribute('href') : null;
            }
        ");

        if (!string.IsNullOrEmpty(detailsHref))
        {
            await Page.GotoAsync(detailsHref);
        }
        else
        {
            // Last resort — try ID 1
            await Page.GotoAsync("/Knjiga/Details/1");
        }

        await Page.WaitForURLAsync(new Regex("/Knjiga/Details/\\d+", RegexOptions.IgnoreCase));
    }

    // ── Tests ──────────────────────────────────────────────────────────────────

    // US-21: Bibliotekar vidi sekciju "Primjerci" i dugme za dodavanje
    [Test]
    public async Task Librarian_DetailsPage_ShowsPrimjerakSection()
    {
        await LoginAsLibrarian();
        await OpenFirstBookDetailsAsLibrarian();

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Primjerci" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "+ Dodaj primjerak" })).ToBeVisibleAsync();
    }

    // US-21: Član ne vidi dugme za dodavanje primjerka
    [Test]
    public async Task Member_DetailsPage_CannotSeeDodajPrimjerakButton()
    {
        await LoginAsMember();
        await OpenFirstBookDetailsAsMember();

        await Expect(
            Page.GetByRole(AriaRole.Button, new() { Name = "+ Dodaj primjerak" })
        ).Not.ToBeVisibleAsync();
    }

    // US-21: Forma za dodavanje je skrivena po defaultu, prikazuje se klikom na dugme
    [Test]
    public async Task Librarian_DodajPrimjerak_FormTogglesOnButtonClick()
    {
        await LoginAsLibrarian();
        await OpenFirstBookDetailsAsLibrarian();

        var addForm = Page.Locator("#forma-dodaj-primjerak");
        await Expect(addForm).Not.ToBeVisibleAsync();

        await Page.GetByRole(AriaRole.Button, new() { Name = "+ Dodaj primjerak" }).ClickAsync();
        await Expect(addForm).ToBeVisibleAsync();

        await addForm.GetByRole(AriaRole.Button, new() { Name = "Otkaži" }).ClickAsync();
        await Expect(addForm).Not.ToBeVisibleAsync();
    }

    // US-21: Bibliotekar može dodati primjerak — prikazuje se poruka o uspjehu
    [Test]
    public async Task Librarian_CanDodajPrimjerak_ShowsSuccessMessage()
    {
        await LoginAsLibrarian();
        await OpenFirstBookDetailsAsLibrarian();

        await Page.GetByRole(AriaRole.Button, new() { Name = "+ Dodaj primjerak" }).ClickAsync();
        var addForm = Page.Locator("#forma-dodaj-primjerak");
        await Expect(addForm).ToBeVisibleAsync();

        await addForm.Locator("input[name='brojNovih']").FillAsync("1");

        // Submit button inside the add form: "Dodaj"
        await addForm.GetByRole(AriaRole.Button, new() { Name = "Dodaj" }).ClickAsync();

        // TempData: $"Uspješno dodano {brojNovih} novi{...} primjerak{...}."
        await Expect(
            Page.GetByText(new Regex("Uspješno dodano", RegexOptions.IgnoreCase))
        ).ToBeVisibleAsync();
    }

    // US-21: Validacija — broj primjeraka izvan opsega vraća grešku
    [Test]
    public async Task Librarian_DodajPrimjerak_InvalidCount_ShowsError()
    {
        await LoginAsLibrarian();
        await OpenFirstBookDetailsAsLibrarian();

        await Page.GetByRole(AriaRole.Button, new() { Name = "+ Dodaj primjerak" }).ClickAsync();
        var addForm = Page.Locator("#forma-dodaj-primjerak");
        await Expect(addForm).ToBeVisibleAsync();

        // Browser enforces min="1" client-side — strip it via page.EvaluateAsync, then submit
        await Page.EvaluateAsync(@"
            () => {
                const el = document.querySelector('#forma-dodaj-primjerak input[name=""brojNovih""]');
                if (el) { el.removeAttribute('min'); el.removeAttribute('max'); el.value = '0'; }
            }
        ");
        await addForm.GetByRole(AriaRole.Button, new() { Name = "Dodaj" }).ClickAsync();

        // TempData: "Broj primjeraka mora biti između 1 i 50."
        await Expect(
            Page.GetByText("Broj primjeraka mora biti između 1 i 50.", new() { Exact = false })
        ).ToBeVisibleAsync();
    }

    // US-24: Bibliotekar može deaktivirati dostupan primjerak
    [Test]
    public async Task Librarian_CanDeaktivirajPrimjerak_ShowsSuccessMessage()
    {
        await LoginAsLibrarian();
        await OpenFirstBookDetailsAsLibrarian();

        var deaktivirajBtn = Page.Locator("form[action*='Deaktiviraj'] button[type='submit']").First;

        // If no deactivatable primjerak exists, add one first
        if (await deaktivirajBtn.CountAsync() == 0)
        {
            await Page.GetByRole(AriaRole.Button, new() { Name = "+ Dodaj primjerak" }).ClickAsync();
            var addForm = Page.Locator("#forma-dodaj-primjerak");
            await Expect(addForm).ToBeVisibleAsync();
            await addForm.Locator("input[name='brojNovih']").FillAsync("1");
            await addForm.GetByRole(AriaRole.Button, new() { Name = "Dodaj" }).ClickAsync();
            await Expect(Page.GetByText(new Regex("Uspješno dodano"))).ToBeVisibleAsync();
            deaktivirajBtn = Page.Locator("form[action*='Deaktiviraj'] button[type='submit']").First;
        }

        Page.Dialog += (_, dialog) => dialog.AcceptAsync();
        await deaktivirajBtn.ClickAsync();

        // TempData: $"Primjerak {inventarniBroj} je uspješno deaktiviran..."
        await Expect(
            Page.GetByText("je uspješno deaktiviran", new() { Exact = false })
        ).ToBeVisibleAsync();
    }

    // US-24: Deaktivirani primjerak prikazuje oznaku "Deaktiviran" umjesto dugmeta
    [Test]
    public async Task Librarian_DeactivatedPrimjerak_ShowsDeaktiviranLabel()
    {
        await LoginAsLibrarian();
        await OpenFirstBookDetailsAsLibrarian();

        // If no deactivated row exists yet, deactivate one
        if (await Page.Locator("tr.row-deaktiviran").CountAsync() == 0)
        {
            var deaktivirajBtn = Page.Locator("form[action*='Deaktiviraj'] button[type='submit']").First;

            if (await deaktivirajBtn.CountAsync() == 0)
            {
                await Page.GetByRole(AriaRole.Button, new() { Name = "+ Dodaj primjerak" }).ClickAsync();
                var addForm = Page.Locator("#forma-dodaj-primjerak");
                await addForm.Locator("input[name='brojNovih']").FillAsync("1");
                await addForm.GetByRole(AriaRole.Button, new() { Name = "Dodaj" }).ClickAsync();
                await Expect(Page.GetByText(new Regex("Uspješno dodano"))).ToBeVisibleAsync();
                deaktivirajBtn = Page.Locator("form[action*='Deaktiviraj'] button[type='submit']").First;
            }

            Page.Dialog += (_, dialog) => dialog.AcceptAsync();
            await deaktivirajBtn.ClickAsync();
            await Expect(Page.GetByText("je uspješno deaktiviran", new() { Exact = false })).ToBeVisibleAsync();
        }

        // Deactivated row shows <span class="deaktiviran-label">Deaktiviran</span>
        await Expect(Page.Locator("span.deaktiviran-label").First).ToBeVisibleAsync();
        await Expect(Page.Locator("span.deaktiviran-label").First).ToHaveTextAsync("Deaktiviran");
    }

    // US-24: Član ne vidi dugme "Deaktiviraj"
    [Test]
    public async Task Member_DetailsPage_CannotSeeDeaktivirajButton()
    {
        await LoginAsMember();
        await OpenFirstBookDetailsAsMember();

        await Expect(
            Page.Locator("form[action*='Deaktiviraj'] button[type='submit']")
        ).Not.ToBeVisibleAsync();
    }
}