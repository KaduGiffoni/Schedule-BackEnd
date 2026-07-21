using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Schedule.Data;
using Schedule.Models;
using Schedule.Models.Scheduling;
using Schedule.Services;
using Schedule.DTOs;

namespace Schedule.Tests
{
    public class SwapRequestServiceTests
    {
        // Método ajudante para criar um banco de dados novinho e vazio na Memória RAM
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        // A etiqueta [Fact] diz ao Visual Studio: "Isto é um robô de teste, pode executá-lo!"
        [Fact]
        public async Task CreateSwapRequest_PastDate_ThrowsInvalidOperationException()
        {
            // ==========================================
            // 1. ARRANGE (Preparar o cenário)
            // ==========================================
            var context = GetInMemoryDbContext();

            // Criando um "Dublê" do Logger para o serviço não reclamar
            var mockLogger = new Mock<ILogger<SwapRequestService>>();

            // Instanciando o serviço com o banco virtual e o logger dublê
            var service = new SwapRequestService(context, mockLogger.Object);

            // Criando um dia de escala no PASSADO (ex: 5 dias atrás)
            var pastScheduleDay = new ScheduleDay
            {
                Id = 1,
                Date = DateTime.Today.AddDays(-5),
                LetterId = 1,
                ShiftId = 1
            };

            context.ScheduleDays.Add(pastScheduleDay);
            await context.SaveChangesAsync();

            var requestDTO = new CreateSwapRequestDTO
            {
                TargetUserId = "maria123",
                ScheduleDayId = 1 // Aponta para o dia no passado
            };

            // ==========================================
            // 2 & 3. ACT & ASSERT (Agir e Validar)
            // ==========================================

            // Aqui nós falamos: "C#, tente executar essa função. Eu AFIRMO (Assert) que ela 
            // VAI explodir um InvalidOperationException".
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.CreateSwapRequestAsync("joao123", requestDTO));

            // E para ser chato como um bom QA, eu AFIRMO que a mensagem de erro contém esse texto:
            Assert.Contains("Não é possível solicitar troca para um plantão que já passou", exception.Message);
        }
    }
}