using System;
using System.IO;
using System.Text;

var files = Directory.GetFiles(@"c:\Users\dzafi\OneDrive\Desktop\ETF\VI Semestar\SI\projekat\SI_grupa5\Projekat\tests\SmartLib.Tests\UI", "*.cs");
var oldText = "await Page.GetByRole(AriaRole.Button, new() { Name = \"Prijavi se\" }).ClickAsync();";
var newText = "await Page.RunAndWaitForNavigationAsync(async () => { await Page.GetByRole(AriaRole.Button, new() { Name = \"Prijavi se\" }).ClickAsync(); });";

foreach(var f in files) {
    if(Path.GetFileName(f) == "AuthUiTests.cs") continue;
    var content = File.ReadAllText(f, Encoding.UTF8);
    if(content.Contains(oldText)) {
        content = content.Replace(oldText, newText);
        File.WriteAllText(f, content, Encoding.UTF8);
    }
}
Console.WriteLine("Done.");
