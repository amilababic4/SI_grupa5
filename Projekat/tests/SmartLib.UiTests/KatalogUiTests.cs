using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace SmartLib.UiTests;

public sealed class KatalogUiTests : SmartLibUiTest
{
    [Test]
    public async Task Member_AfterLogin_CanOpenKatalog()
    {
        await Page.GotoAsync("/Auth/Login");
        await Page.GetByLabel("Email").FillAsync(UiTestSettings.MemberEmail);
        await Page.GetByLabel("Lozinka").FillAsync(UiTestSettings.SharedSeedPassword);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Prijavi se" }).ClickAsync();

        await Page.GotoAsync("/Knjiga");

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Katalog knjiga" })).ToBeVisibleAsync();
        await Expect(Page.GetByPlaceholder("Pretraga po naslovu...")).ToBeVisibleAsync();
    }
}
