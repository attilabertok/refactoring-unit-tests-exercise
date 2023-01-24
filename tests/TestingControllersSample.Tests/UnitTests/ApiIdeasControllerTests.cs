using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Moq;

using TestingControllersSample.Api;
using TestingControllersSample.ClientModels;
using TestingControllersSample.Core.Interfaces;
using TestingControllersSample.Core.Model;

using Xunit;

namespace TestingControllersSample.Tests.UnitTests;

public class ApiIdeasControllerTests
{
    [Fact]
    public async Task Create_ReturnsBadRequest_GivenInvalidModel()
    {
        // Arrange & Act
        var mockRepo = new Mock<IBrainstormSessionRepository>();
        var controller = new IdeasController(mockRepo.Object);
        controller.ModelState.AddModelError("error", "some error");

        // Act
        var result = await controller.Create(model: null);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsHttpNotFound_ForInvalidSession()
    {
        // Arrange
        var testSessionId = 123;
        var mockRepo = new Mock<IBrainstormSessionRepository>();
        mockRepo.Setup(repo => repo.GetByIdAsync(testSessionId))
            .ReturnsAsync((BrainstormSession)null);
        var controller = new IdeasController(mockRepo.Object);

        // Act
        var result = await controller.Create(new NewIdeaModel());

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsNewlyCreatedIdeaForSession()
    {
        // Arrange
        var testSessionId = 123;
        var testName = "test name";
        var testDescription = "test description";
        var testSession = GetTestSession();
        var mockRepo = new Mock<IBrainstormSessionRepository>();
        mockRepo.Setup(repo => repo.GetByIdAsync(testSessionId))
            .ReturnsAsync(testSession);
        var controller = new IdeasController(mockRepo.Object);

        var newIdea = new NewIdeaModel()
        {
            Description = testDescription,
            Name = testName,
            SessionId = testSessionId
        };
        mockRepo.Setup(repo => repo.UpdateAsync(testSession))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        var result = await controller.Create(newIdea);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnSession = Assert.IsType<BrainstormSession>(okResult.Value);
        mockRepo.Verify();
        Assert.Equal(2, returnSession.Ideas.Count);
        Assert.Equal(testName, returnSession.Ideas.LastOrDefault().Name);
        Assert.Equal(testDescription, returnSession.Ideas.LastOrDefault().Description);
    }

    [Fact]
    public async Task ForSession_ReturnsHttpNotFound_ForInvalidSession()
    {
        // Arrange
        var testSessionId = 123;
        var mockRepo = new Mock<IBrainstormSessionRepository>();
        mockRepo.Setup(repo => repo.GetByIdAsync(testSessionId))
            .ReturnsAsync((BrainstormSession)null);
        var controller = new IdeasController(mockRepo.Object);

        // Act
        var result = await controller.ForSession(testSessionId);

        // Assert
        var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(testSessionId, notFoundObjectResult.Value);
    }

    [Fact]
    public async Task ForSession_ReturnsIdeasForSession()
    {
        // Arrange
        var testSessionId = 123;
        var mockRepo = new Mock<IBrainstormSessionRepository>();
        mockRepo.Setup(repo => repo.GetByIdAsync(testSessionId))
            .ReturnsAsync(GetTestSession());
        var controller = new IdeasController(mockRepo.Object);

        // Act
        var result = await controller.ForSession(testSessionId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<List<IdeaDto>>(okResult.Value);
        var idea = returnValue.FirstOrDefault();
        Assert.Equal("One", idea.Name);
    }

    [Fact]
    public async Task ForSessionActionResult_ReturnsNotFoundObjectResultForNonexistentSession()
    {
        // Arrange
        var mockRepo = new Mock<IBrainstormSessionRepository>();
        var controller = new IdeasController(mockRepo.Object);
        var nonExistentSessionId = 999;

        // Act
        var result = await controller.ForSessionActionResult(nonExistentSessionId);

        // Assert
        var actionResult = Assert.IsType<ActionResult<List<IdeaDto>>>(result);
        Assert.IsType<NotFoundObjectResult>(actionResult.Result);
    }

    [Fact]
    public async Task ForSessionActionResult_ReturnsIdeasForSession()
    {
        // Arrange
        var testSessionId = 123;
        var mockRepo = new Mock<IBrainstormSessionRepository>();
        mockRepo.Setup(repo => repo.GetByIdAsync(testSessionId))
            .ReturnsAsync(GetTestSession());
        var controller = new IdeasController(mockRepo.Object);

        // Act
        var result = await controller.ForSessionActionResult(testSessionId);

        // Assert
        var actionResult = Assert.IsType<ActionResult<List<IdeaDto>>>(result);
        var returnValue = Assert.IsType<List<IdeaDto>>(actionResult.Value);
        var idea = returnValue.FirstOrDefault();
        Assert.Equal("One", idea.Name);
    }

    [Fact]
    public async Task CreateActionResult_ReturnsBadRequest_GivenInvalidModel()
    {
        // Arrange & Act
        var mockRepo = new Mock<IBrainstormSessionRepository>();
        var controller = new IdeasController(mockRepo.Object);
        controller.ModelState.AddModelError("error", "some error");

        // Act
        var result = await controller.CreateActionResult(model: null);

        // Assert
        var actionResult = Assert.IsType<ActionResult<BrainstormSession>>(result);
        Assert.IsType<BadRequestObjectResult>(actionResult.Result);
    }

    [Fact]
    public async Task CreateActionResult_ReturnsNotFoundObjectResultForNonexistentSession()
    {
        // Arrange
        var nonExistentSessionId = 999;
        var testName = "test name";
        var testDescription = "test description";
        var mockRepo = new Mock<IBrainstormSessionRepository>();
        var controller = new IdeasController(mockRepo.Object);

        var newIdea = new NewIdeaModel()
        {
            Description = testDescription,
            Name = testName,
            SessionId = nonExistentSessionId
        };

        // Act
        var result = await controller.CreateActionResult(newIdea);

        // Assert
        var actionResult = Assert.IsType<ActionResult<BrainstormSession>>(result);
        Assert.IsType<NotFoundObjectResult>(actionResult.Result);
    }

    [Fact]
    public async Task CreateActionResult_ReturnsNewlyCreatedIdeaForSession()
    {
        // Arrange
        var testSessionId = 123;
        var testName = "test name";
        var testDescription = "test description";
        var testSession = GetTestSession();
        var mockRepo = new Mock<IBrainstormSessionRepository>();
        mockRepo.Setup(repo => repo.GetByIdAsync(testSessionId))
            .ReturnsAsync(testSession);
        var controller = new IdeasController(mockRepo.Object);

        var newIdea = new NewIdeaModel()
        {
            Description = testDescription,
            Name = testName,
            SessionId = testSessionId
        };
        mockRepo.Setup(repo => repo.UpdateAsync(testSession))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        var result = await controller.CreateActionResult(newIdea);

        // Assert
        var actionResult = Assert.IsType<ActionResult<BrainstormSession>>(result);
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
        var returnValue = Assert.IsType<BrainstormSession>(createdAtActionResult.Value);
        mockRepo.Verify();
        Assert.Equal(2, returnValue.Ideas.Count);
        Assert.Equal(testName, returnValue.Ideas.LastOrDefault().Name);
        Assert.Equal(testDescription, returnValue.Ideas.LastOrDefault().Description);
    }

    private static BrainstormSession GetTestSession()
    {
        var session = new BrainstormSession()
        {
            DateCreated = new DateTime(2016, 7, 2),
            Id = 1,
            Name = "Test One"
        };

        var idea = new Idea() { Name = "One" };
        session.AddIdea(idea);

        return session;
    }
}
