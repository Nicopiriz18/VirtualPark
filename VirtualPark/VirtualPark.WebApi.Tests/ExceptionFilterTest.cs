// <copyright file="ExceptionFilterTest.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using VirtualPark.Domain.Exceptions;
using VirtualPark.WebApi.Filters;

namespace VirtualPark.WebApi.Tests;

[TestClass]
public class ExceptionFilterTests
{
    private ExceptionFilter filter = null!;

    [TestInitialize]
    public void Setup()
    {
        this.filter = new ExceptionFilter();
    }

    private static ExceptionContext CreateContext(Exception ex)
    {
        var actionContext = new ActionContext(
            new DefaultHttpContext(), // 👈 acá va el fake HttpContext
            new Microsoft.AspNetCore.Routing.RouteData(),
            new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());

        return new ExceptionContext(actionContext, [])
        {
            Exception = ex,
        };
    }

    [TestMethod]
    public void OnException_InvalidOperationException_ReturnsBadRequest()
    {
        // Arrange
        var ex = new InvalidOperationException("Regla inválida");
        var context = CreateContext(ex);

        // Act
        this.filter.OnException(context);

        // Assert
        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);
    }

    [TestMethod]
    public void OnException_KeyNotFoundException_ReturnsNotFound()
    {
        // Arrange
        var ex = new KeyNotFoundException("No encontrado");
        var context = CreateContext(ex);

        // Act
        this.filter.OnException(context);

        // Assert
        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual((int)HttpStatusCode.NotFound, result.StatusCode);
    }

    [TestMethod]
    public void OnException_DbUpdateException_ReturnsBadRequest()
    {
        // Arrange
        var inner = new Exception("FK error");
        var ex = new DbUpdateException("Update error", inner);
        var context = CreateContext(ex);

        // Act
        this.filter.OnException(context);

        // Assert
        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);
    }

    [TestMethod]
    public void OnException_UnexpectedException_ReturnsInternalServerError()
    {
        // Arrange
        var ex = new Exception("Algo raro");
        var context = CreateContext(ex);

        // Act
        this.filter.OnException(context);

        // Assert
        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual((int)HttpStatusCode.InternalServerError, result.StatusCode);
    }

    [TestMethod]
    public void OnException_ArgumentException_ReturnsBadRequest()
    {
        // Arrange
        var ex = new ArgumentException("Argumento inválido");
        var context = CreateContext(ex);

        // Act
        this.filter.OnException(context);

        // Assert
        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);

        var value = result.Value;
        Assert.IsNotNull(value);
        var errorProperty = value.GetType().GetProperty("Error");
        Assert.IsNotNull(errorProperty);
        Assert.AreEqual("Argumento inválido", errorProperty.GetValue(value));
    }

    [TestMethod]
    public void OnException_DuplicateEmailException_ReturnsConflict()
    {
        // Arrange
        var ex = new DuplicateEmailException("El email ya existe");
        var context = CreateContext(ex);

        // Act
        this.filter.OnException(context);

        // Assert
        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual((int)HttpStatusCode.Conflict, result.StatusCode);

        var value = result.Value;
        Assert.IsNotNull(value);
        var errorProperty = value.GetType().GetProperty("Error");
        Assert.IsNotNull(errorProperty);
        Assert.AreEqual("El email ya existe", errorProperty.GetValue(value));
    }

    [TestMethod]
    public void OnException_InvalidCredentialsException_ReturnsUnauthorized()
    {
        // Arrange
        var ex = new InvalidCredentialsException("Credenciales inválidas");
        var context = CreateContext(ex);

        // Act
        this.filter.OnException(context);

        // Assert
        var result = context.Result as ObjectResult;
        Assert.IsNotNull(result);
        Assert.AreEqual((int)HttpStatusCode.Unauthorized, result.StatusCode);

        var value = result.Value;
        Assert.IsNotNull(value);
        var errorProperty = value.GetType().GetProperty("Error");
        Assert.IsNotNull(errorProperty);
        Assert.AreEqual("Credenciales inválidas", errorProperty.GetValue(value));
    }
}
