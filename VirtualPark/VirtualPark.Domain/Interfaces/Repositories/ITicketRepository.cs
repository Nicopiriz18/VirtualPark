// <copyright file="ITicketRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;

namespace VirtualPark.Domain.Interfaces.Repositories;

public interface ITicketRepository
{
    IEnumerable<Ticket> GetAll();

    Ticket? GetById(Guid id);

    Ticket? GetByQrCode(Guid qrCode);

    Ticket Add(Ticket ticket);

    bool Update(Guid id, Ticket ticket);

    int CountTicketsByEventId(Guid eventId);
}
