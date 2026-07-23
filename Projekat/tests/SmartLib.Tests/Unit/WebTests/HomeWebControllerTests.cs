using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using SmartLib.Infrastructure.Services;
using SmartLib.Web.Controllers;
using SmartLib.Web.Models;
using Xunit;

namespace SmartLib.Tests.Unit.WebTests
{
    public class HomeWebControllerTests
    {
        private readonly Mock<IKnjigaRepository> _knjigaMock;
        private readonly Mock<IBookRecommender> _recommenderMock;
        private readonly Mock<IVijestRepository> _vijestMock;
        private readonly Mock<IDogadjajRepository> _dogadjajMock;
        private readonly IDistributedCache _cache;
        private readonly CacheVersionStore _cacheVersions;
        private readonly SingleFlightCache _singleFlight;
        private readonly HomeController _controller;

        public HomeWebControllerTests()
        {
            _knjigaMock = new Mock<IKnjigaRepository>();
            _recommenderMock = new Mock<IBookRecommender>();
            _vijestMock = new Mock<IVijestRepository>();
            _dogadjajMock = new Mock<IDogadjajRepository>();
            _cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
            _cacheVersions = new CacheVersionStore();
            _singleFlight = new SingleFlightCache();

            _controller = new HomeController(
                _knjigaMock.Object,
                _recommenderMock.Object,
                _cache,
                _cacheVersions,
                _vijestMock.Object,
                _dogadjajMock.Object,
                _singleFlight);

            _vijestMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Vijest>());
            _dogadjajMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Dogadjaj>());


            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        private static Knjiga TestKnjiga(int id = 1) => new()
        {
            Id = id,
            Naslov = $"Test knjiga {id}",
            Autor = "Test autor",
            Isbn = $"978000000000{id}",
            GodinaIzdanja = 2020,
            Kategorija = new Kategorija
            {
                Id = 1,
                Naziv = "Beletristika"
            }
        };

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
        public async Task Index_VracaViewSaHomeViewModel()
        {
            _knjigaMock.Setup(r => r.GetRandomAsync(4))
                .ReturnsAsync(new List<Knjiga> { TestKnjiga(1), TestKnjiga(2) });

            var result = await _controller.Index();

            var view = Assert.IsType<ViewResult>(result);
            Assert.IsType<HomeViewModel>(view.Model);
        }

        [Fact]
        public async Task Index_DohvataCetiriRandomKnjige()
        {
            _knjigaMock.Setup(r => r.GetRandomAsync(4))
                .ReturnsAsync(new List<Knjiga> { TestKnjiga(1) });

            await _controller.Index();

            _knjigaMock.Verify(r => r.GetRandomAsync(4), Times.Once);
        }

        [Fact]
        public async Task Index_MapiraRandomKnjigeUViewModel()
        {
            _knjigaMock.Setup(r => r.GetRandomAsync(4))
                .ReturnsAsync(new List<Knjiga> { TestKnjiga(1), TestKnjiga(2) });

            var result = await _controller.Index();

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<HomeViewModel>(view.Model);

            Assert.Equal(2, model.RandomKnjige.Count);
            Assert.Equal("Test knjiga 1", model.RandomKnjige[0].Naslov);
            Assert.Equal("Test autor", model.RandomKnjige[0].Autor);
            Assert.Equal("Beletristika", model.RandomKnjige[0].Kategorija);
        }

        [Fact]
        public async Task Index_NeprijavljenKorisnik_NeDohvataPreporuke()
        {
            _knjigaMock.Setup(r => r.GetRandomAsync(4))
                .ReturnsAsync(new List<Knjiga> { TestKnjiga(1) });

            var result = await _controller.Index();

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<HomeViewModel>(view.Model);

            Assert.Empty(model.RecommendedKnjige);
            _recommenderMock.Verify(r => r.GetRecommendationsForUserAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Index_PrijavljenKorisnik_DohvataPreporuke()
        {
            SetAuthenticatedUser(7);

            _knjigaMock.Setup(r => r.GetRandomAsync(4))
                .ReturnsAsync(new List<Knjiga> { TestKnjiga(1) });

            _recommenderMock.Setup(r => r.GetRecommendationsForUserAsync(7))
                .ReturnsAsync(new List<Knjiga> { TestKnjiga(3), TestKnjiga(4) });

            var result = await _controller.Index();

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<HomeViewModel>(view.Model);

            Assert.Equal(2, model.RecommendedKnjige.Count);
            _recommenderMock.Verify(r => r.GetRecommendationsForUserAsync(7), Times.Once);
        }

        [Fact]
        public async Task Index_PrijavljenKorisnikBezValidnogId_NeDohvataPreporuke()
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, "nije-broj")
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);

            _knjigaMock.Setup(r => r.GetRandomAsync(4))
                .ReturnsAsync(new List<Knjiga> { TestKnjiga(1) });

            var result = await _controller.Index();

            var view = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<HomeViewModel>(view.Model);

            Assert.Empty(model.RecommendedKnjige);
            _recommenderMock.Verify(r => r.GetRecommendationsForUserAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void Error_VracaView()
        {
            var result = _controller.Error();

            Assert.IsType<ViewResult>(result);
        }
    }
}