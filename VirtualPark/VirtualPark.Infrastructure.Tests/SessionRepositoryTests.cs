// <copyright file="SessionRepositoryTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;
using VirtualPark.Domain;
using VirtualPark.Domain.Enums;
using VirtualPark.Infrastructure.Data;
using VirtualPark.Infrastructure.Repositories;

namespace VirtualPark.Infrastructure.Tests;

[TestClass]
public class SessionRepositoryTests
{
    private ParkDbContext context = null!;
    private SessionRepository repository = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ParkDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        this.context = new ParkDbContext(options);
        this.repository = new SessionRepository(this.context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        this.context.Database.EnsureDeleted();
        this.context.Dispose();
    }

    [TestMethod]
    public void Add_ShouldAddSessionToDatabase()
    {
        // Arrange
        var user = new User("Jane", "Doe", "j@gmail.com", "securePass", RoleEnum.Administrator);
        user.AssignRole(RoleEnum.Administrator);
        this.context.Users.Add(user);
        this.context.SaveChanges();

        var session = new Session { Id = Guid.NewGuid(), UserId = user.Id, Token = Guid.NewGuid().ToString() };

        // Act
        var result = this.repository.Add(session);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(session.Token, result.Token);
        Assert.AreEqual(1, this.context.Sessions.Count());
    }

    [TestMethod]
    public void GetByToken_ShouldReturnSession_WhenTokenExists()
    {
        // Arrange
        var user = new User("Jane", "Doe", "j@gmail.com", "securePass", RoleEnum.Administrator);
        user.AssignRole(RoleEnum.Administrator);
        this.context.Users.Add(user);

        var token = Guid.NewGuid().ToString();
        var session = new Session
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = token,
        };
        this.context.Sessions.Add(session);
        this.context.SaveChanges();

        // Act
        var result = this.repository.GetByToken(token);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(token, result.Token);
        Assert.AreEqual(user.Id, result.UserId);
    }

    [TestMethod]
    public void GetByToken_ShouldReturnNull_WhenTokenDoesNotExist()
    {
        // Arrange
        var nonExistentToken = Guid.NewGuid().ToString();

        // Act
        var result = this.repository.GetByToken(nonExistentToken);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void DeleteByUserId_ShouldDeleteSessionsByUserId()
    {
        // Arrange
        var user1 = new User("Jane", "Doe", "j@gmail.com", "securePass", RoleEnum.Administrator);
        var user2 = new User("John", "Doe", "john@gmail.com", "securePass", RoleEnum.Operator);
        this.context.Users.AddRange(user1, user2);
        this.context.SaveChanges();

        var session1 = new Session { Id = Guid.NewGuid(), UserId = user1.Id, Token = Guid.NewGuid().ToString() };
        var session2 = new Session { Id = Guid.NewGuid(), UserId = user1.Id, Token = Guid.NewGuid().ToString() };
        var session3 = new Session { Id = Guid.NewGuid(), UserId = user2.Id, Token = Guid.NewGuid().ToString() };
        this.context.Sessions.AddRange(session1, session2, session3);
        this.context.SaveChanges();

        // Act
        this.repository.DeleteByUserId(user1.Id);

        // Assert
        Assert.AreEqual(1, this.context.Sessions.Count());
        Assert.AreEqual(user2.Id, this.context.Sessions.First().UserId);
    }

    [TestMethod]
    public void DeleteByUserId_ShouldNotThrowException_WhenUserIdDoesNotExist()
    {
        // Arrange
        var nonExistentUserId = Guid.NewGuid();

        // Act
        this.repository.DeleteByUserId(nonExistentUserId);

        // Assert
        Assert.AreEqual(0, this.context.Sessions.Count());
    }
}
