// <copyright file="TicketTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;
using VirtualPark.Domain.Enums;

namespace VirtualPark.Domain.Tests;

[TestClass]
public class TicketTests
{
    [TestMethod]
    public void TicketConstructor_ShouldSetAllProperties()
    {
        var visitDate = new DateTime(2024, 6, 1, 9, 0, 0, DateTimeKind.Utc);
        var qrCode = Guid.NewGuid();
        var specialEventId = Guid.NewGuid();

        var ticket = new Ticket(visitDate, TicketType.General, qrCode, specialEventId);

        Assert.AreNotEqual(Guid.Empty, ticket.Id);
        Assert.AreEqual(visitDate, ticket.VisitDate);
        Assert.AreEqual(TicketType.General, ticket.Type);
        Assert.AreEqual(qrCode, ticket.QrCode);
        Assert.AreEqual(specialEventId, ticket.SpecialEventId);
    }

    [TestMethod]
    public void TicketConstructor_WithoutSpecialEvent_ShouldLeaveNull()
    {
        var ticket = new Ticket(DateTime.UtcNow, TicketType.SpecialEvent, Guid.NewGuid());

        Assert.IsNull(ticket.SpecialEventId);
    }

    [TestMethod]
    public void TicketConstructor_WithEmptyQrCode_ShouldThrow()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Ticket(DateTime.UtcNow, TicketType.General, Guid.Empty));
    }
}
