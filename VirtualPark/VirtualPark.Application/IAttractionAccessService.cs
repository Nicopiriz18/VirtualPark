// <copyright file="IAttractionAccessService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;
using VirtualPark.Domain.Enums;

namespace VirtualPark.Application;

public interface IAttractionAccessService
{
    bool HasValidAge(Guid attractionId, int visitorAge);

    bool IsAttractionAvailable(Guid attractionId);

    void RegisterAccess(Guid attractionId, Ticket ticket, EntryMethod entryMethod);

    void RegisterAccessByQrCode(Guid attractionId, Guid qrCode, EntryMethod entryMethod);

    void RegisterAccessByVisitorId(Guid attractionId, Guid visitorId, DateTime visitDate, EntryMethod entryMethod);

    void RegisterExit(Guid attractionId, Guid visitorId, DateTime now);

    (int AforoActual, int CapacidadRestante) GetAforo(Guid attractionId);
}
