using Microsoft.AspNetCore.Mvc;
using SmartLib.Web.Controllers;
using Xunit;

namespace SmartLib.Tests.Unit.WebTests
{
    public class RezervacijaWebControllerTests
    {
        private readonly RezervacijaController _controller;

        public RezervacijaWebControllerTests()
        {
            _controller = new RezervacijaController();
        }

        [Fact]
        public async Task Index_VracaView()
        {
            var result = await _controller.Index();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Moje_VracaView()
        {
            var result = await _controller.Moje();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_RedirektujeNaDetaljeKnjige()
        {
            var result = await _controller.Create(5);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirect.ActionName);
            Assert.Equal("Knjiga", redirect.ControllerName);
            Assert.Equal(5, redirect.RouteValues!["id"]);
        }

        [Fact]
        public async Task Otkazi_RedirektujeNaMoje()
        {
            var result = await _controller.Otkazi(10);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Moje", redirect.ActionName);
        }
    }
}