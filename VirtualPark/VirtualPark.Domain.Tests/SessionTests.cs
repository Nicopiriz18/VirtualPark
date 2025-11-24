// <copyright file="SessionTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;

namespace VirtualPark.Domain.Tests;

[TestClass]
public class SessionTests
{
    [TestMethod]
    public void Session_Constructor_SetsDefaultId()
    {
        // Act
        var session = new Session { Token = "test-token" };

        // Assert
        Assert.AreNotEqual(Guid.Empty, session.Id);
    }

    [TestMethod]
    public void Session_SetProperties_PropertiesAreSetCorrectly()
    {
        // Arrange
        var expectedId = Guid.NewGuid();
        var expectedUserId = Guid.NewGuid();
        var expectedToken = "test-token-123";

        // Act
        var session = new Session
        {
            Id = expectedId,
            UserId = expectedUserId,
            Token = expectedToken,
        };

        // Assert
        Assert.AreEqual(expectedId, session.Id);
        Assert.AreEqual(expectedUserId, session.UserId);
        Assert.AreEqual(expectedToken, session.Token);
    }

    [TestMethod]
    public void Session_UserId_DefaultValue()
    {
        // Act
        var session = new Session { Token = "test-token" };

        // Assert
        Assert.AreEqual(Guid.Empty, session.UserId);
    }

    [TestMethod]
    public void Session_Token_IsRequired()
    {
        // Arrange & Act
        var session = new Session { Token = "required-token" };

        // Assert
        Assert.IsNotNull(session.Token);
        Assert.AreEqual("required-token", session.Token);
    }

    [TestMethod]
    public void Session_MultipleInstances_HaveDifferentIds()
    {
        // Act
        var session1 = new Session { Token = "token1" };
        var session2 = new Session { Token = "token2" };

        // Assert
        Assert.AreNotEqual(session1.Id, session2.Id);
    }

    [TestMethod]
    public void Session_SetUserId_UpdatesCorrectly()
    {
        // Arrange
        var session = new Session { Token = "test-token" };
        var newUserId = Guid.NewGuid();

        // Act
        session.UserId = newUserId;

        // Assert
        Assert.AreEqual(newUserId, session.UserId);
    }

    [TestMethod]
    public void Session_UpdateToken_UpdatesCorrectly()
    {
        // Arrange
        var session = new Session { Token = "initial-token" };
        var newToken = "updated-token";

        // Act
        session.Token = newToken;

        // Assert
        Assert.AreEqual(newToken, session.Token);
    }
}
