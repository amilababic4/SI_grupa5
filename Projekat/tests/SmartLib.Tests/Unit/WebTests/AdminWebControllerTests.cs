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
    public class AdminWebControllerTests
    {
        private readonly Mock<IAuditLogRepository> _auditRepoMock;
        private readonly Mock<IKorisnikRepository> _korisnikRepoMock;
        private readonly AdminController _controller;

        public AdminWebControllerTests()
        {
            _auditRepoMock = new Mock<IAuditLogRepository>();
            _korisnikRepoMock = new Mock<IKorisnikRepository>();

            _controller = new AdminController(
                _auditRepoMock.Object,
                _korisnikRepoMock.Object);

            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            _controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        }

        [Fact]
        public async Task Korisnici_VracaView()
        {
            _korisnikRepoMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Korisnik>());

            var result = await _controller.Korisnici();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task AuditLog_VracaView()
        {
            _auditRepoMock.Setup(r => r.GetFilteredAsync(
                It.IsAny<int>(), It.IsAny<int>(), null, null, null, null, null))
                .ReturnsAsync(new List<AuditLog>());
            _auditRepoMock.Setup(r => r.GetTotalCountAsync(null, null, null))
                .ReturnsAsync(0);

            var result = await _controller.AuditLog();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task AuditLog_SaPageParametrom_VracaView()
        {
            _auditRepoMock.Setup(r => r.GetFilteredAsync(
                It.IsAny<int>(), It.IsAny<int>(), null, null, null, null, null))
                .ReturnsAsync(new List<AuditLog>());
            _auditRepoMock.Setup(r => r.GetTotalCountAsync(null, null, null))
                .ReturnsAsync(0);

            var result = await _controller.AuditLog(2);

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task AuditLog_SaFilterima_VracaView()
        {
            _auditRepoMock.Setup(r => r.GetFilteredAsync(
                It.IsAny<int>(), It.IsAny<int>(),
                "Knjiga", "Kreiranje", null, null, null))
                .ReturnsAsync(new List<AuditLog>());
            _auditRepoMock.Setup(r => r.GetTotalCountAsync("Knjiga", "Kreiranje", null))
                .ReturnsAsync(0);

            var result = await _controller.AuditLog(entitetTip: "Knjiga", akcija: "Kreiranje");

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task AuditLog_PopunjavaViewBagPodatke()
        {
            _auditRepoMock.Setup(r => r.GetFilteredAsync(
                It.IsAny<int>(), It.IsAny<int>(), null, null, null, null, null))
                .ReturnsAsync(new List<AuditLog>());
            _auditRepoMock.Setup(r => r.GetTotalCountAsync(null, null, null))
                .ReturnsAsync(90);

            await _controller.AuditLog(page: 2, pageSize: 30);

            Assert.Equal(2, _controller.ViewBag.Stranica);
            Assert.Equal(30, _controller.ViewBag.PageSize);
            Assert.Equal(3, _controller.ViewBag.UkupnoStrana); // 90 / 30 = 3
            Assert.Equal(90, _controller.ViewBag.UkupnoBroj);
        }
    }
}