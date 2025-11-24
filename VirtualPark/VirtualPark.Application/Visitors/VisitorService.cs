// <copyright file="VisitorService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;
using VirtualPark.Domain.Enums;
using VirtualPark.Domain.Interfaces.Repositories;
using VirtualPark.Domain.Interfaces.Security;

namespace VirtualPark.Application.Visitors;

public class VisitorService(IVisitorRepository visitorRepository, IPasswordHasher passwordHasher) : IVisitorService
{
    private readonly IVisitorRepository visitorRepository = visitorRepository;
    private readonly IPasswordHasher passwordHasher = passwordHasher;

    public Visitor RegisterUser(string name, string surname, string email, string password, DateTime birthDate, Guid nfcId)
    {
        var visitor = new Visitor(name, surname, email, password, birthDate, MembershipLevel.Standard, nfcId);
        return this.visitorRepository.Add(visitor);
    }

    public Visitor RegisterUserByAdmin(string name, string surname, string email, string password, DateTime birthDate, Guid nfcId, MembershipLevel membershipLevel)
    {
        var visitor = new Visitor(name, surname, email, password, birthDate, membershipLevel, nfcId);
        return this.visitorRepository.Add(visitor);
    }

    public void UpdateProfile(Guid visitorId, string newName, string newSurname, string newEmail)
    {
        var visitor = this.GetVisitorOrThrow(visitorId);

        // Create a new visitor instance with updated values for the repository
        var updatedVisitor = new Visitor(newName, newSurname, newEmail, visitor.Password!, visitor.BirthDate, visitor.MembershipLevel, visitor.NfcId);

        this.visitorRepository.Update(visitorId, updatedVisitor);
    }

    public void UpdateProfileWithPassword(Guid visitorId, string newName, string newSurname, string newEmail, string? newPassword)
    {
        var visitor = this.GetVisitorOrThrow(visitorId);

        // Hash the new password if provided, otherwise keep the existing one
        var password = !string.IsNullOrWhiteSpace(newPassword)
            ? this.passwordHasher.Hash(newPassword)
            : visitor.Password!;

        // Create a new visitor instance with updated values for the repository
        var updatedVisitor = new Visitor(newName, newSurname, newEmail, password, visitor.BirthDate, visitor.MembershipLevel, visitor.NfcId);

        this.visitorRepository.Update(visitorId, updatedVisitor);
    }

    // public Ticket PurchaseTicket(Guid visitorId, DateTime visitDate, TicketType ticketType)
    // {
    //     _ = GetVisitorOrThrow(visitorId);
    //
    //     var qrCode = Guid.NewGuid();
    //     var ticket = new Ticket(visitDate, ticketType, qrCode, null)
    //     {
    //         VisitorId = visitorId
    //     };
    //
    //     return _ticketRepository.Add(ticket);
    // }
    //
    // public Ticket PurchaseSpecialEventTicket(Guid visitorId, DateTime visitDate, Guid specialEventId)
    // {
    //     // Validate visitor exists
    //     _ = GetVisitorOrThrow(visitorId);
    //
    //     // Validate special event exists
    //     var specialEvent = _specialEventRepository.GetById(specialEventId);
    //     if(specialEvent == null)
    //     {
    //         throw new ArgumentException("Special event not found", nameof(specialEventId));
    //     }
    //
    //     // Validate capacity: count current tickets sold for this event
    //     var ticketsSold = _ticketRepository.CountTicketsByEventId(specialEventId);
    //     if(ticketsSold >= specialEvent.MaxCapacity)
    //     {
    //         throw new InvalidOperationException($"Event has reached maximum capacity ({specialEvent.MaxCapacity} tickets)");
    //     }
    //
    //     // Create ticket with unique QR code
    //     var qrCode = Guid.NewGuid();
    //     var ticket = new Ticket(visitDate, TicketType.SpecialEvent, qrCode, specialEventId)
    //     {
    //         VisitorId = visitorId
    //     };
    //
    //     return _ticketRepository.Add(ticket);
    // }

    // public AttractionAccess RegisterAttractionVisit(Guid identifierGuid, Guid attractionId, EntryMethod entryMethod, Ticket ticket)
    // {
    //     Visitor? visitor = null;
    //
    //     // Find visitor by NFC ID or by ticket QR code
    //     if(entryMethod == EntryMethod.NFC)
    //     {
    //         visitor = _visitorRepository.GetByNfcId(identifierGuid);
    //     }
    //     else if(entryMethod == EntryMethod.QR)
    //     {
    //         // Validate that the provided QR code matches the ticket
    //         if(ticket.QrCode != identifierGuid)
    //         {
    //             throw new ArgumentException("Invalid QR code for ticket", nameof(identifierGuid));
    //         }
    //
    //         // Find visitor using the ticket's VisitorId
    //         visitor = _visitorRepository.GetById(ticket.VisitorId);
    //     }
    //
    //     if(visitor == null)
    //     {
    //         throw new ArgumentException("Visitor not found", nameof(identifierGuid));
    //     }
    //
    //     var access = new AttractionAccess(attractionId, visitor.Id, ticket.Id, DateTime.Now, null, entryMethod);
    //     return access;
    // }
    private Visitor GetVisitorOrThrow(Guid visitorId)
    {
        return this.visitorRepository.GetById(visitorId)
               ?? throw new ArgumentException("Visitor not found", nameof(visitorId));
    }

    public Visitor? GetById(Guid visitorId) => this.visitorRepository.GetById(visitorId);

    public Visitor? GetByNfcId(Guid nfcId) => this.visitorRepository.GetByNfcId(nfcId);

    public IEnumerable<Visitor> GetAll() => this.visitorRepository.GetAll();
}
