using Microsoft.AspNetCore.Mvc;
using Moq;
using SmartLib.API.Controllers;
using SmartLib.Core.DTOs;
using SmartLib.Core.Interfaces;
using SmartLib.Core.Models;
using Xunit;

namespace SmartLib.Tests.Unit.APITests
{
    public class KnjigaApiControllerTests
    {
        private readonly Mock<IKnjigaRepository> _knjigaMock;
        private readonly Mock<IPrimjerakRepository> _primjerakMock;
        private readonly Mock<IKategorijaRepository> _kategorijaMock;
        private readonly KnjigaController _controller;

        public KnjigaApiControllerTests()
        {
            _knjigaMock = new Mock<IKnjigaRepository>();
            _primjerakMock = new Mock<IPrimjerakRepository>();
            _kategorijaMock = new Mock<IKategorijaRepository>();

            _controller = new KnjigaController(
                _knjigaMock.Object,
                _primjerakMock.Object,
                _kategorijaMock.Object);
        }

        // Pomoćne metode za testne podatke
        private static Knjiga TestKnjiga(int id = 1) => new()
        {
            Id = id,
            Naslov = "Test Knjiga",
            Autor = "Autor",
            Isbn = "1234567890",
            Primjerci = new List<Primjerak>()
        };

        [Fact]
        public async Task GetById_KnjigaPostoji_VracaOkIObjekt()
        {
            
            var knjiga = TestKnjiga(10);
            _knjigaMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(knjiga);

            var result = await _controller.GetById(10);
       
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<KnjigaDto>(okResult.Value);
            Assert.Equal(10, dto.Id);
        }

        [Fact]
        public async Task GetById_KnjigaNePostoji_VracaNotFound()
        {
            _knjigaMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Knjiga?)null);

            var result = await _controller.GetById(99);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_ValidnaKnjiga_Vraca201Created()
        { 
            var createDto = new KnjigaCreateDto
            {
                Naslov = "Nova",
                Autor = "Autor",
                Isbn = "1234567890",
                KategorijaId = 1,
                BrojPrimjeraka = 1
            };

            _knjigaMock.Setup(r => r.GetByIsbnAsync(It.IsAny<string>())).ReturnsAsync((Knjiga?)null);
            _kategorijaMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Kategorija { Id = 1 });
            _knjigaMock.Setup(r => r.CreateAsync(It.IsAny<Knjiga>())).ReturnsAsync(TestKnjiga(5));
            _knjigaMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(TestKnjiga(5));

            var result = await _controller.Create(createDto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(201, createdResult.StatusCode);
            Assert.Equal("GetById", createdResult.ActionName);
        }

        [Fact]
        public async Task Create_NeispravanIsbn_VracaBadRequest()
        {
            var createDto = new KnjigaCreateDto { Isbn = "short" }; // Nevalidan ISBN

            var result = await _controller.Create(createDto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("ISBN nije u ispravnom formatu.", badRequest.Value);
        }

        [Fact]
        public async Task Delete_KnjigaImaAktivnaZaduzenja_VracaBadRequest()
        {
            // Simuliramo poslovno pravilo: HasActiveLoansAsync vraća true
            _knjigaMock.Setup(r => r.HasActiveLoansAsync(1)).ReturnsAsync(true);

            var result = await _controller.Delete(1);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("aktivna zaduženja", badRequest.Value.ToString());

            _knjigaMock.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Delete_Uspjesno_VracaNoContent()
        {
            _knjigaMock.Setup(r => r.HasActiveLoansAsync(1)).ReturnsAsync(false);
            _knjigaMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

            var result = await _controller.Delete(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Create_DupliIsbn_VracaConflict()
        {           
            var dto = new KnjigaCreateDto { Isbn = "1234567890", Naslov = "Test" };
            _knjigaMock.Setup(r => r.GetByIsbnAsync(It.IsAny<string>())).ReturnsAsync(new Knjiga());
           
            var result = await _controller.Create(dto);

            Assert.IsType<ConflictObjectResult>(result.Result);
        }

        [Fact]
        public async Task Update_PogresanId_VracaBadRequest()
        {           
            var result = await _controller.Update(1, new KnjigaEditDto { Id = 2 });
          
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}