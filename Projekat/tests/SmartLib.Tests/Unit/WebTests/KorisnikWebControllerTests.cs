using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using SmartLib.Web.Controllers;
using SmartLib.Core.DTOs;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using Xunit;

namespace SmartLib.Tests.Unit.WebTests
{
    public class KorisnikWebControllerTests
    {
        private readonly Mock<IKorisnikRepository> _repoMock;
        private readonly Mock<IClanarinaRepository> _clanarinaRepoMock;
        private readonly Mock<IZaduzenjeRepository> _zaduzenjeRepoMock;
        private readonly Mock<IRezervacijaRepository> _rezervacijaRepoMock;
        private readonly Mock<IListaKolekcijaRepository> _listaKolekcijaRepoMock;
        private readonly KorisnikController _controller;

        public KorisnikWebControllerTests()
        {
            _repoMock = new Mock<IKorisnikRepository>();
            _clanarinaRepoMock = new Mock<IClanarinaRepository>();
            _zaduzenjeRepoMock = new Mock<IZaduzenjeRepository>();
            _rezervacijaRepoMock = new Mock<IRezervacijaRepository>();
            _listaKolekcijaRepoMock = new Mock<IListaKolekcijaRepository>();

            var tempDataProvider = new Mock<ITempDataProvider>();
            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, tempDataProvider.Object);

            _controller = new KorisnikController(
                _repoMock.Object,
                _clanarinaRepoMock.Object,
                _zaduzenjeRepoMock.Object,
                _rezervacijaRepoMock.Object,
                _listaKolekcijaRepoMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                },
                TempData = tempData
            };
        }

        // ... svi postojeći testovi ostaju nepromijenjeni ...

        [Fact]
        public async Task ProfilClana_VracaViewSaPodacimaClana()
        {
            var korisnik = new Korisnik
            {
                Id = 5,
                Ime = "Test",
                Prezime = "Clan",
                Email = "clan@test.com",
                Status = "aktivan",
                Uloga = new Uloga { Naziv = "Član" }
            };

            _repoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(korisnik);
            _clanarinaRepoMock.Setup(r => r.GetByKorisnikAsync(5)).ReturnsAsync((Clanarina?)null);
            _zaduzenjeRepoMock.Setup(r => r.GetHistoryByKorisnikAsync(5)).ReturnsAsync(new List<Zaduzenje>());
            _rezervacijaRepoMock.Setup(r => r.CountByKorisnikAsync(5)).ReturnsAsync(0);
            _listaKolekcijaRepoMock.Setup(r => r.GetByKorisnikAsync(5)).ReturnsAsync(new List<ListaKolekcija>());

            var result = await _controller.ProfilClana(5);

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Korisnik>(view.Model);
            Assert.Equal(5, model.Id);
            Assert.False((bool)_controller.ViewBag.JeMojProfil);
        }

        [Fact]
        public async Task ProfilClana_NepostojeciClan_VracaNotFound()
        {
            _repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Korisnik?)null);

            var result = await _controller.ProfilClana(99);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}