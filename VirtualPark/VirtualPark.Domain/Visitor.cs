// <copyright file="Visitor.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain.Enums;

namespace VirtualPark.Domain;

public class Visitor : User
{
    private Visitor()
        : base()
    {
    }

    public Visitor(string name, string surname, string email, string password, DateTime birthDate, MembershipLevel membershipLevel, Guid nfcId)
        : base(name, surname, email, password, RoleEnum.Visitor)
    {
        if (nfcId == Guid.Empty)
        {
            throw new ArgumentException("NfcId cannot be empty.", nameof(nfcId));
        }

        if (birthDate > DateTime.UtcNow)
        {
            throw new ArgumentException("BirthDate cannot be in the future.", nameof(birthDate));
        }

        this.BirthDate = birthDate;
        this.MembershipLevel = membershipLevel;
        this.NfcId = nfcId;
    }

    public DateTime BirthDate { get; set; }

    public MembershipLevel MembershipLevel { get; set; }

    public Guid NfcId { get; set; }

    private readonly List<Ticket> tickets = [];

    public IReadOnlyCollection<Ticket> Tickets => this.tickets.AsReadOnly();
}
