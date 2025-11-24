// <copyright file="UserRepositoryTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;
using VirtualPark.Domain;
using VirtualPark.Domain.Enums;
using VirtualPark.Infrastructure.Data;
using VirtualPark.Infrastructure.Repositories;

namespace VirtualPark.Infrastructure.Tests.RepositoryTests;

[TestClass]
public class UserRepositoryTests
{
    private ParkDbContext context = null!;
    private UserRepository repository = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ParkDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new ParkDbContext(options);
        this.repository = new UserRepository(this.context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        this.context.Dispose();
    }

    [TestMethod]
    public async Task Add_Should_Persist_User()
    {
        var user = new User("Ana", "Lopez", "ana@example.com", "secret", RoleEnum.Operator);

        var created = this.repository.Add(user);

        Assert.IsNotNull(created);
        var saved = await this.context.Users.FindAsync(created.Id);
        Assert.IsNotNull(saved);
        Assert.AreEqual("ana@example.com", saved.Email);
    }

    [TestMethod]
    public async Task GetById_Should_Return_User_When_Exists()
    {
        var user = new User("Luis", "Perez", "luis@example.com", "secret", RoleEnum.Administrator);
        this.context.Users.Add(user);
        await this.context.SaveChangesAsync();

        var found = this.repository.GetById(user.Id);

        Assert.IsNotNull(found);
        Assert.AreEqual(user.Email, found.Email);
    }

    [TestMethod]
    public void GetAll_Should_Return_All_Users()
    {
        var users = new[]
        {
            new User("Ana", "Lopez", "ana2@example.com", "secret", RoleEnum.Operator),
            new User("Juan", "Perez", "juan@example.com", "secret", RoleEnum.Administrator),
        };
        this.context.Users.AddRange(users);
        this.context.SaveChanges();

        var result = this.repository.GetAll().ToList();

        Assert.AreEqual(2, result.Count);
        CollectionAssert.AreEquivalent(users.Select(u => u.Email).ToList(), result.Select(u => u.Email).ToList());
    }

    [TestMethod]
    public async Task Update_Should_ReturnTrue_And_Update_Values()
    {
        var original = new User("Ana", "Lopez", "ana3@example.com", "secret", RoleEnum.Operator);
        this.context.Users.Add(original);
        await this.context.SaveChangesAsync();

        var updatedValues = new User("Ana Maria", "Lopez", "ana3@example.com", "newsecret", RoleEnum.Administrator);

        var result = this.repository.Update(original.Id, updatedValues);

        Assert.IsTrue(result);
        var reloaded = await this.context.Users.FindAsync(original.Id);
        Assert.IsNotNull(reloaded);
        Assert.AreEqual("Ana Maria", reloaded.Name);
        Assert.AreEqual(original.Id, reloaded.Id);
    }

    [TestMethod]
    public void Update_Should_ReturnFalse_When_User_Does_Not_Exist()
    {
        var result = this.repository.Update(Guid.NewGuid(), new User("Test", "User", "test@example.com", "secret", RoleEnum.Operator));

        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task ConvertToVisitor_ShouldConvertUserToVisitor_Successfully()
    {
        // Arrange
        var originalUser = new User("Carlos", "Rodriguez", "carlos@example.com", "hashedpass", RoleEnum.Administrator);
        this.context.Users.Add(originalUser);
        await this.context.SaveChangesAsync();
        var userId = originalUser.Id;

        var birthDate = new DateTime(1985, 3, 20);
        var membershipLevel = MembershipLevel.Premium;
        var nfcId = Guid.NewGuid();

        // Act
        var visitor = this.repository.ConvertToVisitor(userId, originalUser, birthDate, membershipLevel, nfcId);

        // Assert
        Assert.IsNotNull(visitor);
        Assert.AreEqual(userId, visitor.Id);
        Assert.AreEqual("Carlos", visitor.Name);
        Assert.AreEqual("Rodriguez", visitor.Surname);
        Assert.AreEqual("carlos@example.com", visitor.Email);
        Assert.AreEqual(birthDate, visitor.BirthDate);
        Assert.AreEqual(membershipLevel, visitor.MembershipLevel);
        Assert.AreEqual(nfcId, visitor.NfcId);

        // Verify old user was removed
        var oldUser = await this.context.Users.OfType<User>().FirstOrDefaultAsync(u => u.Id == userId && !(u is Visitor));
        Assert.IsNull(oldUser);

        // Verify new visitor exists
        var savedVisitor = await this.context.Visitors.FindAsync(userId);
        Assert.IsNotNull(savedVisitor);
        Assert.AreEqual(visitor.Id, savedVisitor.Id);
    }

    [TestMethod]
    public void ConvertToVisitor_ShouldThrowException_WhenUserDoesNotExist()
    {
        // Arrange
        var nonExistentUserId = Guid.NewGuid();
        var fakeUser = new User("Fake", "User", "fake@example.com", "pass", RoleEnum.Operator);
        var birthDate = new DateTime(1990, 1, 1);
        var membershipLevel = MembershipLevel.Standard;
        var nfcId = Guid.NewGuid();

        // Act & Assert
        var exception = Assert.ThrowsException<KeyNotFoundException>(() =>
            this.repository.ConvertToVisitor(nonExistentUserId, fakeUser, birthDate, membershipLevel, nfcId));

        Assert.IsTrue(exception.Message.Contains($"User with id {nonExistentUserId} was not found"));
    }
}
