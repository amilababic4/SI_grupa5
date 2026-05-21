using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Web.Controllers;
using Xunit;

namespace SmartLib.Tests.Unit.WebTests
{
    public class RezervacijaWebControllerTests
    {
        private readonly Mock<IRezervacijaRepository> _rezervacijaRepoMock;
        private readonly Mock<IKnjigaRepository> _knjigaRepoMock;
        private readonly RezervacijaController _controller;

        public RezervacijaWebControllerTests()
        {
            _rezervacijaRepoMock = new Mock<IRezervacijaRepository>();
            _knjigaRepoMock = new Mock<IKnjigaRepository>();

            _controller = new RezervacijaController(
                _rezervacijaRepoMock.Object,
                _knjigaRepoMock.Object);

            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            _controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        }

        private void SetAuthenticatedUser(int userId = 1)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId.ToString()),
                new(ClaimTypes.Name, "Test Korisnik")
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);
        }

        [Fact]
        public async Task Index_VracaView()
        {
            _rezervacijaRepoMock.Setup(r => r.GetActiveAsync())
                .ReturnsAsync(new List<Rezervacija>());

            var result = await _controller.Index();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Moje_VracaView()
        {
            SetAuthenticatedUser(1);
            _rezervacijaRepoMock.Setup(r => r.GetByKorisnikAsync(1))
                .ReturnsAsync(new List<Rezervacija>());

            var result = await _controller.Moje();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_RedirektujeNaDetaljeKnjige()
        {
            SetAuthenticatedUser(1);

            var knjiga = new Knjiga
            {
                Id = 5,
                Naslov = "Test Knjiga",
                Primjerci = new List<Primjerak>
                {
                    new Primjerak { Status = "zaduzen" }
                }
            };

            _knjigaRepoMock.Setup(r => r.GetByIdAsync(5))
                .ReturnsAsync(knjiga);

            _rezervacijaRepoMock.Setup(r => r.HasActiveAsync(1, 5))
                .ReturnsAsync(false);

            var result = await _controller.Create(5);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirect.ActionName);
            Assert.Equal("Knjiga", redirect.ControllerName);
            Assert.Equal(5, redirect.RouteValues!["id"]);
        }

        [Fact]
        public async Task Otkazi_RedirektujeNaMoje()
        {
            SetAuthenticatedUser(1);

            var rezervacija = new Rezervacija
            {
                Id = 10,
                KorisnikId = 1,
                Status = "aktivna"
            };

            _rezervacijaRepoMock.Setup(r => r.GetByIdAsync(10))
                .ReturnsAsync(rezervacija);

            var result = await _controller.Otkazi(10);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Moje", redirect.ActionName);
        }
    }
}