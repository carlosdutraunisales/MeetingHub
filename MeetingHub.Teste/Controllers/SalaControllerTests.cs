using Moq;
using MeetingHub.Controllers;
using MeetingHub.Services;
using MeetingHub.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;
using MeetingHub.Interfaces;

namespace MeetingHub.Tests
{
    public class SalaControllerTests
    {
        private readonly Mock<SalaService> _mockSalaService;
        private readonly Mock<ISalaRepository> _mockSalaRepository;
        private readonly Mock<IReservaRepository> _mockReservaRepository;
        private readonly SalaService _salaService;
        private readonly SalaController _controller;

        public SalaControllerTests()
        {
            _mockSalaService = new Mock<SalaService>();
            _mockSalaRepository = new Mock<ISalaRepository>();
            _mockReservaRepository = new Mock<IReservaRepository>();
            _salaService = new SalaService(_mockSalaRepository.Object, _mockReservaRepository.Object);
            _controller = new SalaController(_salaService);
        }

        [Fact]
        public async Task Get_ReturnsOkResult_WithListOfSalas()
        {
            // Arrange
            var salas = new List<Sala>
            {
                new Sala { Id = ObjectId.GenerateNewId(), Nome = "Sala 1" },
                new Sala { Id = ObjectId.GenerateNewId(), Nome = "Sala 2" }
            };

            // Configurando o mock para retornar uma lista de salas
            _mockSalaRepository.Setup(repo => repo.ObterTodasSalasAsync()).ReturnsAsync(salas);

            // Act
            var result = await _controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedSalas = Assert.IsAssignableFrom<List<Sala>>(okResult.Value);
            Assert.Equal(salas.Count, returnedSalas.Count);
        }

        [Fact]
        public async Task GetById_ReturnsOkResult_WithSala()
        {
            // Arrange
            var salaId = ObjectId.GenerateNewId().ToString();
            var sala = new Sala { Id = ObjectId.Parse(salaId), Nome = "Sala 1" };

            // Configurando o mock para retornar a sala ao buscar por ID
            _mockSalaRepository.Setup(repo => repo.ObterSalaPorIdAsync(It.IsAny<ObjectId>())).ReturnsAsync(sala);

            // Act
            var result = await _controller.GetById(salaId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedSala = Assert.IsType<Sala>(okResult.Value);
            Assert.Equal(sala.Nome, returnedSala.Nome);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenSalaDoesNotExist()
        {
            // Arrange
            var salaId = ObjectId.GenerateNewId().ToString();

            // Configurando o mock para lançar uma exceção quando o ID não for encontrado
            _mockSalaRepository.Setup(repo => repo.ObterSalaPorIdAsync(It.IsAny<ObjectId>())).Throws(new KeyNotFoundException());

            // Act
            var result = await _controller.GetById(salaId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Sala não encontrada.", notFoundResult.Value);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var sala = new Sala { Nome = "Nova Sala" };

            
            _mockSalaRepository.Setup(repo => repo.CriarSalaAsync(It.IsAny<Sala>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(sala);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetById", createdAtActionResult.ActionName);
            Assert.Equal(sala.Id.ToString(), createdAtActionResult.RouteValues["id"]);
            Assert.Equal(sala, createdAtActionResult.Value);
        }

        [Fact]
        public async Task Update_ReturnsNoContent_WhenSalaIsUpdated()
        {
            // Arrange
            var salaId = ObjectId.GenerateNewId(); // Gerando um ObjectId válido
            var salaToUpdate = new Sala { Nome = "Sala Atualizada", Id = salaId }; 

            _mockSalaRepository.Setup(repo => repo.ObterSalaPorIdAsync(salaId))
                               .ReturnsAsync(salaToUpdate); 

            _mockSalaRepository.Setup(repo => repo.AtualizarSalaAsync(It.IsAny<ObjectId>(), It.IsAny<Sala>()))
                               .Returns(Task.CompletedTask); 

            // Act
            var result = await _controller.Update(salaId.ToString(), salaToUpdate); 

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result); 
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenSalaDoesNotExist()
        {
            // Arrange
            var salaId = ObjectId.GenerateNewId().ToString();
            var salaToUpdate = new Sala { Nome = "Sala Atualizada" };

            _mockSalaRepository.Setup(repo => repo.AtualizarSalaAsync(It.IsAny<ObjectId>(), It.IsAny<Sala>()))
                               .Throws(new KeyNotFoundException());

            // Act
            var result = await _controller.Update(salaId, salaToUpdate);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Sala não encontrada.", notFoundResult.Value);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenSalaIsDeleted()
        {
            // Arrange
            var salaId = ObjectId.GenerateNewId().ToString();

            _mockSalaRepository.Setup(repo => repo.ExcluirSalaAsync(It.IsAny<ObjectId>()))
                               .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(salaId);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenSalaDoesNotExist()
        {
            // Arrange
            var salaId = ObjectId.GenerateNewId().ToString();

            _mockSalaRepository.Setup(repo => repo.ExcluirSalaAsync(It.IsAny<ObjectId>()))
                               .Throws(new KeyNotFoundException());

            // Act
            var result = await _controller.Delete(salaId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Sala não encontrada.", notFoundResult.Value);
        }

        [Fact]
        public async Task GetSalasDisponiveis_ReturnsOkResult_WithAvailableRooms()
        {
            // Arrange
            var data = DateTime.Now;
            var capacidade = 10;
            var recursos = new List<string> { "Projetor", "Ar condicionado" };
            var salasDisponiveis = new List<Sala>
                {
                    new Sala { Id = ObjectId.GenerateNewId(), Nome = "Sala 1", Capacidade = 15 },
                    new Sala { Id = ObjectId.GenerateNewId(), Nome = "Sala 2", Capacidade = 20 }
                };

            var mockSalaService = new Mock<SalaService>(Mock.Of<ISalaRepository>(), Mock.Of<IReservaRepository>());

            mockSalaService.Setup(service => service.BuscarSalasDisponiveis(It.IsAny<DateTime>(), It.IsAny<int?>(), It.IsAny<List<string>>()))
                           .ReturnsAsync(salasDisponiveis);

            var controller = new SalaController(mockSalaService.Object);

            // Act
            var result = await controller.GetSalasDisponiveis(data, capacidade, recursos);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedSalas = Assert.IsAssignableFrom<List<Sala>>(okResult.Value);
            Assert.Equal(salasDisponiveis.Count, returnedSalas.Count);
        }

        [Fact]
        public async Task GetSalasDisponiveis_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var data = DateTime.Now;
            var capacidade = 10;
            var recursos = new List<string> { "Projetor", "Ar condicionado" };

            var mockSalaService = new Mock<SalaService>(Mock.Of<ISalaRepository>(), Mock.Of<IReservaRepository>());

            mockSalaService.Setup(service => service.BuscarSalasDisponiveis(It.IsAny<DateTime>(), It.IsAny<int?>(), It.IsAny<List<string>>()))
                           .Throws(new Exception("Erro interno"));

            var controller = new SalaController(mockSalaService.Object);

            // Act
            var result = await controller.GetSalasDisponiveis(data, capacidade, recursos);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Erro interno: Erro interno", statusCodeResult.Value);
        }

    }
}
