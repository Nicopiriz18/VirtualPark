// <copyright file="VisitorRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;
using VirtualPark.Domain;
using VirtualPark.Domain.Interfaces.Repositories;
using VirtualPark.Infrastructure.Data;

namespace VirtualPark.Infrastructure.Repositories.Visitors;

public class VisitorRepository(ParkDbContext context) : IVisitorRepository
{
    private readonly ParkDbContext context = context;

    public IEnumerable<Visitor> GetAll() =>
        this.context.Visitors.AsNoTracking().ToList();

    public Visitor? GetById(Guid id) =>
        this.context.Visitors.Find(id);

    public Visitor? GetByNfcId(Guid nfcId) =>
        this.context.Visitors.FirstOrDefault(v => v.NfcId == nfcId);

    public Visitor Add(Visitor visitor)
    {
        this.context.Visitors.Add(visitor);
        this.context.SaveChanges();
        return visitor;
    }

    public bool Update(Guid id, Visitor visitor)
    {
        var existing = this.context.Visitors.Find(id);
        if (existing is null)
        {
            return false;
        }

        this.context.Entry(existing).Property(nameof(Visitor.Name)).CurrentValue = visitor.Name;
        this.context.Entry(existing).Property(nameof(Visitor.Surname)).CurrentValue = visitor.Surname;
        this.context.Entry(existing).Property(nameof(Visitor.Email)).CurrentValue = visitor.Email;
        this.context.Entry(existing).Property(nameof(Visitor.Password)).CurrentValue = visitor.Password;
        this.context.Entry(existing).Property(nameof(Visitor.BirthDate)).CurrentValue = visitor.BirthDate;
        this.context.Entry(existing).Property(nameof(Visitor.MembershipLevel)).CurrentValue = visitor.MembershipLevel;
        this.context.SaveChanges();
        return true;
    }
}
