using Microsoft.AspNetCore.Mvc;
using SmartLib.Web.Controllers;
using Xunit;

namespace SmartLib.Tests.Unit.WebTests
{
    public class AdminWebControllerTests
    {
        private readonly AdminController _controller;

        public AdminWebControllerTests()
        {
            _controller = new AdminController();
        }

        [Fact]
        public async Task Korisnici_VracaView()
        {
            var result = await _controller.Korisnici();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task AuditLog_VracaView()
        {
            var result = await _controller.AuditLog();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task AuditLog_SaPageParametrom_VracaView()
        {
            var result = await _controller.AuditLog(2);

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task PromijeniUlogu_RedirektujeNaKorisnici()
        {
            var result = await _controller.PromijeniUlogu(1);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Korisnici", redirect.ActionName);
        }

        [Fact]
        public async Task DeaktivirajKorisnika_RedirektujeNaKorisnici()
        {
            var result = await _controller.DeaktivirajKorisnika(1);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Korisnici", redirect.ActionName);
        }
    }
}