// <copyright file="VisitorRepositoryTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;
using VirtualPark.Domain;
using VirtualPark.Domain.Enums;
using VirtualPark.Infrastructure.Data;
using VirtualPark.Infrastructure.Repositories;
using VirtualPark.Infrastructure.Repositories.Visitors;

namespace VirtualPark.Infrastructure.Tests.RepositoryTests;

[TestClass]
public class VisitorRepositoryTests
{
    private VisitorRepository repository = null!;
    private ParkDbContext context = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ParkDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        this.context = new ParkDbContext(options);
        this.repository = new VisitorRepository(this.context);
    }

    [TestMethod]
    public void Add_ShouldAddNewVisitor()
    {
        // Arrange
        var visitor = new Visitor(
            name: "John",
            surname: "Doe",
            email: "john.doe@example.com",
            password: "password123",
            birthDate: new DateTime(1990, 1, 1),
            membershipLevel: MembershipLevel.Premium,
            nfcId: Guid.NewGuid());

        // Act
        var createdVisitor = this.repository.Add(visitor);

        // Assert
        Assert.IsNotNull(createdVisitor);
        Assert.AreEqual(visitor.Name, createdVisitor.Name);
        Assert.AreEqual(visitor.Surname, createdVisitor.Surname);
        Assert.AreEqual(visitor.Email, createdVisitor.Email);
        Assert.AreEqual(visitor.BirthDate, createdVisitor.BirthDate);
        Assert.AreEqual(visitor.MembershipLevel, createdVisitor.MembershipLevel);
        Assert.AreEqual(visitor.NfcId, createdVisitor.NfcId);
    }

    [TestMethod]
    public void GetAll_ShouldReturnAllVisitors()
    {
        // Arrange
        var visitor1 = new Visitor(
            name: "John",
            surname: "Doe",
            email: "john@example.com",
            password: "pass1",
            birthDate: new DateTime(1990, 1, 1),
            membershipLevel: MembershipLevel.Premium,
            nfcId: Guid.NewGuid());
        var visitor2 = new Visitor(
            name: "Jane",
            surname: "Smith",
            email: "jane@example.com",
            password: "pass2",
            birthDate: new DateTime(1995, 5, 15),
            membershipLevel: MembershipLevel.VIP,
            nfcId: Guid.NewGuid());

        this.repository.Add(visitor1);
        this.repository.Add(visitor2);

        // Act
        var allVisitors = this.repository.GetAll().ToList();

        // Assert
        Assert.AreEqual(2, allVisitors.Count);
        Assert.IsTrue(allVisitors.Any(v => v.Id == visitor1.Id));
        Assert.IsTrue(allVisitors.Any(v => v.Id == visitor2.Id));
    }

    [TestMethod]
    public void GetById_ShouldReturnVisitor_WhenExists()
    {
        // Arrange
        var visitor = new Visitor(
            name: "John",
            surname: "Doe",
            email: "john@example.com",
            password: "pass1",
            birthDate: new DateTime(1990, 1, 1),
            membershipLevel: MembershipLevel.Premium,
            nfcId: Guid.NewGuid());
        this.repository.Add(visitor);

        // Act
        var result = this.repository.GetById(visitor.Id);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(visitor.Id, result.Id);
        Assert.AreEqual(visitor.Name, result.Name);
    }

    [TestMethod]
    public void GetById_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = this.repository.GetById(nonExistentId);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetByNfcId_ShouldReturnVisitor_WhenExists()
    {
        // Arrange
        var nfcId = Guid.NewGuid();
        var visitor = new Visitor(
            name: "John",
            surname: "Doe",
            email: "john@example.com",
            password: "pass1",
            birthDate: new DateTime(1990, 1, 1),
            membershipLevel: MembershipLevel.Premium,
            nfcId: nfcId);
        this.repository.Add(visitor);

        // Act
        var result = this.repository.GetByNfcId(nfcId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(visitor.Id, result.Id);
        Assert.AreEqual(nfcId, result.NfcId);
    }

    [TestMethod]
    public void GetByNfcId_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        var nonExistentNfcId = Guid.NewGuid();

        // Act
        var result = this.repository.GetByNfcId(nonExistentNfcId);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Update_ShouldModifyExistingVisitor()
    {
        // Arrange
        var visitor = new Visitor(
            name: "John",
            surname: "Doe",
            email: "john@example.com",
            password: "pass1",
            birthDate: new DateTime(1990, 1, 1),
            membershipLevel: MembershipLevel.Premium,
            nfcId: Guid.NewGuid());
        this.repository.Add(visitor);

        var updatedVisitor = new Visitor(
            name: "Jane",
            surname: "Smith",
            email: "jane@example.com",
            password: "pass2",
            birthDate: new DateTime(1995, 5, 15),
            membershipLevel: MembershipLevel.VIP,
            nfcId: Guid.NewGuid());

        // Act
        var result = this.repository.Update(visitor.Id, updatedVisitor);

        // Assert
        Assert.IsTrue(result);
        var updated = this.context.Visitors.Find(visitor.Id);
        Assert.IsNotNull(updated);
        Assert.AreEqual("Jane", updated.Name);
        Assert.AreEqual("Smith", updated.Surname);
        Assert.AreEqual("jane@example.com", updated.Email);
        Assert.AreEqual(new DateTime(1995, 5, 15), updated.BirthDate);
        Assert.AreEqual(MembershipLevel.VIP, updated.MembershipLevel);
    }

    [TestMethod]
    public void Update_ShouldReturnFalse_WhenVisitorNotExists()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var visitor = new Visitor(
            name: "Jane",
            surname: "Smith",
            email: "jane@example.com",
            password: "pass2",
            birthDate: new DateTime(1995, 5, 15),
            membershipLevel: MembershipLevel.VIP,
            nfcId: Guid.NewGuid());

        // Act
        var result = this.repository.Update(nonExistentId, visitor);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Add_ShouldPersistEntity()
    {
        // Arrange
        var visitor = new Visitor(
            name: "John",
            surname: "Doe",
            email: "john@example.com",
            password: "pass1",
            birthDate: new DateTime(1990, 1, 1),
            membershipLevel: MembershipLevel.Premium,
            nfcId: Guid.NewGuid());

        // Act
        this.repository.Add(visitor);

        // Assert
        var found = this.context.Visitors.FirstOrDefault(v => v.Id == visitor.Id);
        Assert.IsNotNull(found);
        Assert.AreEqual(visitor.Name, found.Name);
    }
}
