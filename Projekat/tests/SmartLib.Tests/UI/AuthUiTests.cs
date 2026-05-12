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
    public async Task Login_Member_ShowsCorrectNavbar()
    {
        await Page.GotoAsync("/Auth/Login");

        await Page.GetByLabel("Email").FillAsync(UiTestSettings.MemberEmail);
        await Page.GetByLabel("Lozinka").FillAsync(UiTestSettings.SharedSeedPassword);

        await Page.GetByRole(AriaRole.Button, new() { Name = "Prijavi se" }).ClickAsync();

        // ostaje na home
        await Expect(Page).ToHaveURLAsync(new Regex(".*/$"));

        var nav = Page.GetByRole(AriaRole.Navigation);

        await Expect(
            nav.GetByRole(AriaRole.Link, new() { Name = "Katalog" })
        ).ToBeVisibleAsync();
        await Expect(Page.GetByText("Moja zaduženja")).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Odjava" })).ToBeVisibleAsync();
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
