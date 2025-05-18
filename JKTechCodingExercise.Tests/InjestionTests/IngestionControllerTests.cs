using System.Net;
using JKTechCodingExercise.API.Controllers;
using JKTechCodingExercise.Core.DTO;
using JKTechCodingExercise.Core.Enums;
using JKTechCodingExercise.Core.Interfaces;
using JKTechCodingExercise.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.Protected;

namespace JKTechCodingExercise.Tests.InjestionTests;

public class IngestionControllerTests
{
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<IHttpClientFactory> _mockHttpFactory;
    private readonly IngestionController _controller;

    public IngestionControllerTests()
    {
        _mockUow = new Mock<IUnitOfWork>();
        _mockHttpFactory = new Mock<IHttpClientFactory>();

        _controller = new IngestionController(
            _mockUow.Object,
            _mockHttpFactory.Object
        );
    }

    private HttpClient CreateMockHttpClient(HttpStatusCode status, string content = "OK")
    {
        var handler = new Mock<HttpMessageHandler>();
        handler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = status,
                Content = new StringContent(content)
            });

        return new HttpClient(handler.Object);
    }
    
    [Fact]
    public async Task TriggerIngestion_ReturnsOk_WhenSuccessful()
    {
        var doc = new Document { Id = 1 };
        var repoMock = new Mock<IRepository<IngestionTask>>();
        var addedTask = new IngestionTask { Id = 1, DocumentId = doc.Id };
    
        _mockUow.Setup(u => u.Documents.GetByIdAsync(1)).ReturnsAsync(doc);
        _mockUow.Setup(u => u.Repository<IngestionTask>()).Returns(repoMock.Object);
        repoMock.Setup(r => r.AddAsync(It.IsAny<IngestionTask>())).Callback<IngestionTask>(t => addedTask = t);
        _mockUow.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
    
        var httpClient = CreateMockHttpClient(HttpStatusCode.OK);
        _mockHttpFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);
    
        var result = await _controller.TriggerIngestion(new IngestionRequestDto { DocumentId = 1 });
    
        var okResult = Assert.IsType<OkObjectResult>(result);
        var data = okResult.Value as dynamic;
        Assert.Equal(1, (int)data.taskId);
        Assert.Equal(IngestionStatus.InProgress, (IngestionStatus)data.status);
    }

    [Fact]
    public async Task TriggerIngestion_ReturnsNotFound_IfDocumentMissing()
    {
        _mockUow.Setup(u => u.Documents.GetByIdAsync(99)).ReturnsAsync((Document?)null);
    
        var result = await _controller.TriggerIngestion(new IngestionRequestDto { DocumentId = 99 });
    
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task TriggerIngestion_ReturnsFailure_IfSpringFails()
    {
        var doc = new Document { Id = 2 };
        var repoMock = new Mock<IRepository<IngestionTask>>();
        _mockUow.Setup(u => u.Documents.GetByIdAsync(2)).ReturnsAsync(doc);
        _mockUow.Setup(u => u.Repository<IngestionTask>()).Returns(repoMock.Object);
        _mockUow.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
    
        var httpClient = CreateMockHttpClient(HttpStatusCode.BadRequest, "Failed");
        _mockHttpFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);
    
        var result = await _controller.TriggerIngestion(new IngestionRequestDto { DocumentId = 2 });
    
        var ok = Assert.IsType<OkObjectResult>(result);
        dynamic data = ok.Value;
        Assert.Equal(IngestionStatus.Failed, (IngestionStatus)data.status);
    }

    [Fact]
    public async Task GetTasks_ReturnsList()
    {
        var repoMock = new Mock<IRepository<IngestionTask>>();
        var tasks = new List<IngestionTask> { new() { Id = 1 }, new() { Id = 2 } };
    
        repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(tasks);
        _mockUow.Setup(u => u.Repository<IngestionTask>()).Returns(repoMock.Object);
    
        var result = await _controller.GetTasks();
    
        var okResult = Assert.IsType<OkObjectResult>(result);
        var resultTasks = Assert.IsAssignableFrom<IEnumerable<IngestionTask>>(okResult.Value);
        Assert.Equal(2, resultTasks.Count());
    }

    [Fact]
    public async Task CancelTask_ChangesStatus()
    {
        var task = new IngestionTask { Id = 1, Status = IngestionStatus.InProgress };
        var repoMock = new Mock<IRepository<IngestionTask>>();
        repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(task);
        _mockUow.Setup(u => u.Repository<IngestionTask>()).Returns(repoMock.Object);
        _mockUow.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
    
        var result = await _controller.CancelTask(1);
    
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(IngestionStatus.Cancelled, task.Status);
    }

    [Fact]
    public async Task CancelTask_Fails_IfAlreadyCompleted()
    {
        var task = new IngestionTask { Id = 2, Status = IngestionStatus.Completed };
        var repoMock = new Mock<IRepository<IngestionTask>>();
        repoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(task);
        _mockUow.Setup(u => u.Repository<IngestionTask>()).Returns(repoMock.Object);
    
        var result = await _controller.CancelTask(2);
    
        Assert.IsType<BadRequestObjectResult>(result);
    }
}
