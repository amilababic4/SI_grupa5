using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace SmartLib.UiTests;

public sealed class HomeUiTests : SmartLibUiTest
{
    [Test]
    public async Task Index_ShowsTitleHeroAndPrijavaLink()
    {
        await Page.GotoAsync("/");

        await Expect(Page).ToHaveTitleAsync(new Regex("SmartLib", RegexOptions.IgnoreCase));
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "SmartLib" })).ToBeVisibleAsync();
        // "Prijava" substring-matches "Prijava u sistem" unless Exact = true
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Prijava", Exact = true })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Prijava u sistem" })).ToBeVisibleAsync();
    }
}
