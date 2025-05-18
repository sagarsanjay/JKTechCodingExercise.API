using System.Text;
using JKTechCodingExercise.API.Controllers;
using JKTechCodingExercise.Core.DTO;
using JKTechCodingExercise.Core.Interfaces;
using JKTechCodingExercise.Core.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace JKTechCodingExercise.Tests.DocumentTests;

public class DocumentControllerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IWebHostEnvironment> _mockEnv;
    private readonly Mock<UserManager<IdentityUser>> _mockUserManager;
    private readonly DocumentController _controller;

    public DocumentControllerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockEnv = new Mock<IWebHostEnvironment>();
        _mockUserManager = MockUserManager();

        _controller = new DocumentController(
            _mockUnitOfWork.Object,
            _mockEnv.Object,
            _mockUserManager.Object
        );
    }

    private static Mock<UserManager<IdentityUser>> MockUserManager()
    {
        var store = new Mock<IUserStore<IdentityUser>>();
        return new Mock<UserManager<IdentityUser>>(
            store.Object, null, null, null, null, null, null, null, null
        );
    }
    
    [Fact]
    public async Task GetAll_ReturnsOk_WithListOfDocuments()
    {
        var docs = new List<Document>
        {
            new Document { Id = 1, Title = "Doc1", UploadedBy = "user1" },
            new Document { Id = 2, Title = "Doc2", UploadedBy = "user2" }
        };
        _mockUnitOfWork.Setup(u => u.Documents.GetAllAsync()).ReturnsAsync(docs);

        var result = await _controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnDocs = Assert.IsAssignableFrom<IEnumerable<Document>>(okResult.Value);
        Assert.Equal(2, returnDocs.Count());
    }

    [Fact]
    public async Task Get_ById_ReturnsDocument_IfExists()
    {
        var doc = new Document { Id = 1, Title = "TestDoc" };
        _mockUnitOfWork.Setup(u => u.Documents.GetByIdAsync(1)).ReturnsAsync(doc);

        var result = await _controller.Get(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnDoc = Assert.IsType<Document>(okResult.Value);
        Assert.Equal("TestDoc", returnDoc.Title);
    }

    [Fact]
    public async Task Get_ById_ReturnsNotFound_IfNotExists()
    {
        _mockUnitOfWork.Setup(u => u.Documents.GetByIdAsync(99)).ReturnsAsync((Document?)null);

        var result = await _controller.Get(99);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Upload_ReturnsOk_WhenFileUploaded()
    {
        var mockFile = new Mock<IFormFile>();
        var content = "This is a test file.";
        var fileName = "test.txt";
        var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
        mockFile.Setup(f => f.OpenReadStream()).Returns(ms);
        mockFile.Setup(f => f.FileName).Returns(fileName);
        mockFile.Setup(f => f.Length).Returns(ms.Length);

        var dto = new DocumentDto
        {
            Title = "Test Title",
            Description = "Test Desc",
            File = mockFile.Object
        };

        _mockEnv.Setup(e => e.WebRootPath).Returns(Path.GetTempPath());

        var result = await _controller.Upload(dto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnDoc = Assert.IsType<Document>(okResult.Value);
        Assert.Equal("Test Title", returnDoc.Title);
    }

    [Fact]
    public async Task Update_ReturnsOk_WhenSuccessful()
    {
        var doc = new Document { Id = 1, Title = "Old Title", Description = "Old" };
        _mockUnitOfWork.Setup(u => u.Documents.GetByIdAsync(1)).ReturnsAsync(doc);

        var updated = new Document { Title = "New Title", Description = "New" };

        var result = await _controller.Update(1, updated);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnDoc = Assert.IsType<Document>(okResult.Value);
        Assert.Equal("New Title", returnDoc.Title);
    }

    [Fact]
    public async Task Delete_ReturnsOk_WhenDeleted()
    {
        var doc = new Document { Id = 1, FilePath = "/uploads/test.txt" };
        _mockUnitOfWork.Setup(u => u.Documents.GetByIdAsync(1)).ReturnsAsync(doc);
        _mockEnv.Setup(e => e.WebRootPath).Returns(Path.GetTempPath());

        var result = await _controller.Delete(1);

        Assert.IsType<OkObjectResult>(result);
    }

}
