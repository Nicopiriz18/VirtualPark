// <copyright file="SpecialEventRepositoryTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;
using VirtualPark.Infrastructure.Data;
using VirtualPark.Infrastructure.Repositories;

namespace VirtualPark.Infrastructure.Tests.RepositoryTests;
[TestClass]
public class SpecialEventRepositoryTests
{
    private ParkDbContext context = null!;
    private SpecialEventRepository repository = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ParkDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new ParkDbContext(options);
        this.repository = new SpecialEventRepository(this.context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        this.context.Dispose();
    }

    [TestMethod]
    public void Add_ShouldAddNewSpecialEvent()
    {
        var specialEvent = new Domain.SpecialEvent("Haunted House", DateTime.UtcNow, 12, 12);
        var addedEvent = this.repository.Add(specialEvent);
        Assert.IsNotNull(addedEvent);
        Assert.AreEqual(specialEvent.Name, addedEvent.Name);
    }

    [TestMethod]
    public void GetAll_ShouldReturnAllSpecialEvents()
    {
        var event1 = new Domain.SpecialEvent("Event 1", DateTime.UtcNow, 10, 10);
        var event2 = new Domain.SpecialEvent("Event 2", DateTime.UtcNow, 15, 15);
        this.repository.Add(event1);
        this.repository.Add(event2);
        var events = this.repository.GetAll().ToList();
        Assert.AreEqual(2, events.Count);
    }

    [TestMethod]
    public void GetById_ShouldReturnSpecialEvent_WhenExists()
    {
        var specialEvent = new Domain.SpecialEvent("Magic Show", DateTime.UtcNow, 20, 20);
        var addedEvent = this.repository.Add(specialEvent);
        var fetchedEvent = this.repository.GetById(addedEvent.Id);
        Assert.IsNotNull(fetchedEvent);
        Assert.AreEqual(addedEvent.Id, fetchedEvent?.Id);
    }

    [TestMethod]
    public void GetById_ShouldReturnNull_WhenNotExists()
    {
        var fetchedEvent = this.repository.GetById(Guid.NewGuid());
        Assert.IsNull(fetchedEvent);
    }

    [TestMethod]
    public void Update_ShouldModifyExistingSpecialEvent()
    {
        var specialEvent = new Domain.SpecialEvent("Old Event", DateTime.UtcNow, 10, 10);
        var addedEvent = this.repository.Add(specialEvent);
        var newSpecialEvent = new Domain.SpecialEvent("Updated Event", DateTime.UtcNow.AddDays(1), 15, 15);
        this.repository.Update(addedEvent.Id, newSpecialEvent);
        var updatedEvent = this.repository.GetById(addedEvent.Id);
        Assert.IsNotNull(updatedEvent);
        Assert.AreEqual("Updated Event", updatedEvent?.Name);
        Assert.AreEqual(15, updatedEvent?.AdditionalCost);
        Assert.AreEqual(15, updatedEvent?.MaxCapacity);
    }

    [TestMethod]
    public void Delete_ShouldRemoveSpecialEvent()
    {
        var specialEvent = new Domain.SpecialEvent("Temporary Event", DateTime.UtcNow, 5, 5);
        var addedEvent = this.repository.Add(specialEvent);
        this.repository.Delete(addedEvent.Id);
        var deletedEvent = this.repository.GetById(addedEvent.Id);
        Assert.IsNull(deletedEvent);
    }
}
