using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace SmartLib.UiTests;

public sealed class AuthUiTests : SmartLibUiTest
{
    [Test]
    public async Task LoginPage_DisplaysFormFields()
    {
        await Page.GotoAsync("/Auth/Login");

        await Expect(Page.GetByLabel("Email")).ToBeVisibleAsync();
        await Expect(Page.GetByLabel("Lozinka")).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Prijavi se" })).ToBeVisibleAsync();
    }

    [Test]
    public async Task Login_WrongPassword_ShowsFailureMessage()
    {
        await Page.GotoAsync("/Auth/Login");
        await Page.GetByLabel("Email").FillAsync(UiTestSettings.MemberEmail);
        await Page.GetByLabel("Lozinka").FillAsync("wrong-password-for-ui-test");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Prijavi se" }).ClickAsync();

        await Expect(Page.GetByText("Prijava nije uspjela.")).ToBeVisibleAsync();
    }

    [Test]
    public async Task Login_Member_RedirectsToHome_WithDashboardInNav()
    {
        await Page.GotoAsync("/Auth/Login");
        await Page.GetByLabel("Email").FillAsync(UiTestSettings.MemberEmail);
        await Page.GetByLabel("Lozinka").FillAsync(UiTestSettings.SharedSeedPassword);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Prijavi se" }).ClickAsync();

        // Cookie auth redirects član to Home — URL is often "/" not "/Home".
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Dashboard" })).ToBeVisibleAsync();
        Assert.That(Page.Url, Does.Not.Contain("/Auth/Login").IgnoreCase);
    }

    [Test]
    public async Task Login_Librarian_RedirectsToMembersArea()
    {
        await Page.GotoAsync("/Auth/Login");
        await Page.GetByLabel("Email").FillAsync(UiTestSettings.LibrarianEmail);
        await Page.GetByLabel("Lozinka").FillAsync(UiTestSettings.SharedSeedPassword);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Prijavi se" }).ClickAsync();

        await Expect(Page).ToHaveURLAsync(new Regex("/Korisnik", RegexOptions.IgnoreCase));
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Članovi biblioteke" })).ToBeVisibleAsync();
    }

    [Test]
    public async Task Logout_ReturnsToLoginPage()
    {
        await Page.GotoAsync("/Auth/Login");
        await Page.GetByLabel("Email").FillAsync(UiTestSettings.MemberEmail);
        await Page.GetByLabel("Lozinka").FillAsync(UiTestSettings.SharedSeedPassword);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Prijavi se" }).ClickAsync();

        // Početna (Home/Index) prijavljenom korisniku prikazuje samo "Dashboard" u navu — Odjava je na ostalim stranicama.
        await Page.GotoAsync("/Knjiga");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Odjava" }).ClickAsync();

        await Expect(Page).ToHaveURLAsync(new Regex("/Auth/Login", RegexOptions.IgnoreCase));
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Prijava u sistem" })).ToBeVisibleAsync();
    }
}
