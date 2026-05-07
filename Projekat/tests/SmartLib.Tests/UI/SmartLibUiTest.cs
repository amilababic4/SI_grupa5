using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace SmartLib.UiTests;

/// <summary>Playwright base: HTTPS dev certificate ignored, relative URLs resolved against SMARTLIB_BASE_URL.</summary>
[Parallelizable(ParallelScope.Self)]
public abstract class SmartLibUiTest : PageTest
{
    public override BrowserNewContextOptions ContextOptions() => new()
    {
        BaseURL = UiTestSettings.BaseUrl,
        IgnoreHTTPSErrors = true,
    };
}
