using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace SmartLib.UiTests;

public sealed class ListaKolekcijaUiTests : SmartLibUiTest
{
    [Test]
    public async Task Clan_AfterLogin_CanOpenKolekcije_AndSeesWishlist()
    {
        await LoginAsClan();

        await Page.GotoAsync("/ListaKolekcija");

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Moje kolekcije" }))
            .ToBeVisibleAsync();

        // "Lista želja" se uvijek automatski kreira za člana
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Lista želja", Exact = true })).ToBeVisibleAsync();
    }

    [Test]
    public async Task Clan_CanCreateNewKolekcija_AndItAppearsInList()
    {
        await LoginAsClan();

        await Page.GotoAsync("/ListaKolekcija");

        // Otvori modal za novu kolekciju
        await Page.GetByRole(AriaRole.Button, new() { Name = "+ Nova kolekcija" }).ClickAsync();
        await Expect(Page.Locator("#collectionCreateModal")).ToBeVisibleAsync();

        var naziv = $"UI Test Kolekcija {DateTime.Now:HHmmss}";
        await Page.Locator("#NewNaziv").FillAsync(naziv);

        await Page.RunAndWaitForNavigationAsync(async () =>
        {
            await Page.GetByRole(AriaRole.Button, new() { Name = "Kreiraj" }).ClickAsync();
        });

        await Expect(Page.GetByText(naziv)).ToBeVisibleAsync();
    }

    [Test]
    public async Task Clan_CanOpenDetails_OfExistingKolekcija()
    {
        await LoginAsClan();

        await Page.GotoAsync("/ListaKolekcija");

        // Klikni na prvu karticu kolekcije (Lista želja je uvijek prisutna)
        await Page.Locator("article.collection-card a.stretched-link").First.ClickAsync();

        // ISPRAVKA: Dodan .First da se izbjegne Strict Mode konflikt
        await Expect(Page.Locator(".visibility-pill").First).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "← Nazad na kolekcije" })).ToBeVisibleAsync();
    }

    [Test]
    public async Task Clan_WishlistCard_HasNoDeleteButton()
    {
        await LoginAsClan();

        await Page.GotoAsync("/ListaKolekcija");

        // Kartica "Lista želja" ne smije imati dugme za brisanje
        var wishlistCard = Page.Locator("article.collection-card")
            .Filter(new() { HasText = "Lista želja" });

        await Expect(wishlistCard).ToBeVisibleAsync();

        var deleteBtn = wishlistCard.Locator(".collection-delete-btn");
        Assert.That(await deleteBtn.CountAsync(), Is.EqualTo(0),
            "Lista želja ne smije imati dugme za brisanje.");
    }

    [Test]
    public async Task Bibliotekar_CannotAccess_ListaKolekcija()
    {
        await LoginAsBibliotekar();

        await Page.GotoAsync("/ListaKolekcija");

        Assert.That(
            Page.Url.Contains("/Auth/Login") ||
            Page.Url.Contains("/Account/AccessDenied") ||
            Page.Url.Contains("/Forbidden"),
            Is.True,
            $"Bibliotekar ne smije pristupiti kolekcijama člana. URL: {Page.Url}");
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