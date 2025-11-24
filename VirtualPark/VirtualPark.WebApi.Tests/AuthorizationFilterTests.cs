// <copyright file="AuthorizationFilterTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Moq;
using VirtualPark.Application.Session;
using VirtualPark.Domain;
using VirtualPark.Domain.Enums;
using VirtualPark.WebApi.Filters;

namespace VirtualPark.WebApi.Tests;

[TestClass]
public class AuthorizationFilterTests
{
    private AuthorizationFilterContext context = null!;
    private DefaultHttpContext httpContext = null!;
    private Mock<IServiceProvider> serviceProvider = null!;
    private Mock<ISessionService> sessionService = null!;
    private AuthorizationFilter filter = null!;

    [TestInitialize]
    public void Setup()
    {
        this.sessionService = new Mock<ISessionService>(MockBehavior.Strict);
        this.serviceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
        this.serviceProvider.Setup(sp => sp.GetService(typeof(ISessionService)))
            .Returns(() => this.sessionService.Object);

        this.httpContext = new DefaultHttpContext
        {
            RequestServices = this.serviceProvider.Object,
        };

        var actionContext = new ActionContext(
            this.httpContext,
            new Microsoft.AspNetCore.Routing.RouteData(),
            new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());

        this.context = new AuthorizationFilterContext(actionContext, []);
    }

    [TestMethod]
    public void OnAuthorization_NoAuthorizationHeader_ReturnsUnauthorized()
    {
        // Arrange
        this.filter = new AuthorizationFilter();

        // Act
        this.filter.OnAuthorization(this.context);

        // Assert
        var result = this.context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual((int)HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [TestMethod]
    public void OnAuthorization_InvalidAuthorizationHeaderFormat_ReturnsUnauthorized()
    {
        // Arrange
        this.filter = new AuthorizationFilter();
        this.httpContext.Request.Headers["Authorization"] = "Invalid token";

        // Act
        this.filter.OnAuthorization(this.context);

        // Assert
        var result = this.context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual((int)HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [TestMethod]
    public void OnAuthorization_SessionServiceUnavailable_ReturnsUnauthorized()
    {
        // Arrange
        this.filter = new AuthorizationFilter();
        this.httpContext.Request.Headers["Authorization"] = "Bearer token";
        var provider = new Mock<IServiceProvider>(MockBehavior.Strict);
        provider.Setup(sp => sp.GetService(typeof(ISessionService)))
            .Returns(null);
        this.httpContext.RequestServices = provider.Object;

        // Act
        this.filter.OnAuthorization(this.context);

        // Assert
        var result = this.context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual((int)HttpStatusCode.Unauthorized, result.StatusCode);
        provider.VerifyAll();
    }

    [TestMethod]
    public void OnAuthorization_InvalidToken_ReturnsUnauthorized()
    {
        // Arrange
        this.filter = new AuthorizationFilter();
        this.httpContext.Request.Headers["Authorization"] = "Bearer invalid";
        this.sessionService.Setup(s => s.GetByToken("invalid"))
            .Throws(new KeyNotFoundException());

        // Act
        this.filter.OnAuthorization(this.context);

        // Assert
        var result = this.context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual((int)HttpStatusCode.Unauthorized, result.StatusCode);

        this.sessionService.VerifyAll();
    }

    [TestMethod]
    public void OnAuthorization_RoleRequiredAndMissing_ReturnsForbidden()
    {
        // Arrange
        this.filter = new AuthorizationFilter(RoleEnum.Administrator);
        this.httpContext.Request.Headers["Authorization"] = "Bearer token";
        var user = new User("John", "Visitor", "john@example.com", "pass123", RoleEnum.Visitor);
        this.sessionService.Setup(s => s.GetByToken("token"))
            .Returns(user);

        // Act
        this.filter.OnAuthorization(this.context);

        // Assert
        var result = this.context.Result as JsonResult;
        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status403Forbidden, result.StatusCode);

        this.sessionService.VerifyAll();
    }

    [TestMethod]
    public void OnAuthorization_RoleRequiredAndPresent_AllowsAccess()
    {
        // Arrange
        this.filter = new AuthorizationFilter(RoleEnum.Administrator);
        this.httpContext.Request.Headers["Authorization"] = "Bearer token";
        var user = new User("Admin", "User", "admin@example.com", "pass123", RoleEnum.Administrator);
        this.sessionService.Setup(s => s.GetByToken("token"))
            .Returns(user);

        // Act
        this.filter.OnAuthorization(this.context);

        // Assert
        Assert.IsNull(this.context.Result);

        this.sessionService.VerifyAll();
    }

    [TestMethod]
    public void OnAuthorization_NoRoleRequired_AllowsAuthenticatedUser()
    {
        // Arrange
        this.filter = new AuthorizationFilter();
        this.httpContext.Request.Headers["Authorization"] = "Bearer token";
        var user = new User("Admin", "User", "admin@example.com", "pass123", RoleEnum.Administrator);
        this.sessionService.Setup(s => s.GetByToken("token"))
            .Returns(user);

        // Act
        this.filter.OnAuthorization(this.context);

        // Assert
        Assert.IsNull(this.context.Result);

        this.sessionService.VerifyAll();
    }

    [TestMethod]
    public void OnAuthorization_RequireVisitorTrueWithVisitor_AllowsAccess()
    {
        // Arrange
        this.filter = new AuthorizationFilter { RequireVisitor = true };
        this.httpContext.Request.Headers["Authorization"] = "Bearer token";
        var visitor = new Visitor(
            "Jane",
            "Doe",
            "jane@example.com",
            "pass123",
            DateTime.UtcNow.AddYears(-20),
            MembershipLevel.Standard,
            Guid.NewGuid());
        this.sessionService.Setup(s => s.GetByToken("token"))
            .Returns(visitor);

        // Act
        this.filter.OnAuthorization(this.context);

        // Assert
        Assert.IsNull(this.context.Result);

        this.sessionService.VerifyAll();
    }

    [TestMethod]
    public void OnAuthorization_RequireVisitorTrueWithNonVisitor_ReturnsForbidden()
    {
        // Arrange
        this.filter = new AuthorizationFilter { RequireVisitor = true };
        this.httpContext.Request.Headers["Authorization"] = "Bearer token";
        var user = new User("Admin", "User", "admin@example.com", "pass123", RoleEnum.Administrator);
        this.sessionService.Setup(s => s.GetByToken("token"))
            .Returns(user);

        // Act
        this.filter.OnAuthorization(this.context);

        // Assert
        var result = this.context.Result as JsonResult;
        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status403Forbidden, result.StatusCode);

        this.sessionService.VerifyAll();
    }
}
