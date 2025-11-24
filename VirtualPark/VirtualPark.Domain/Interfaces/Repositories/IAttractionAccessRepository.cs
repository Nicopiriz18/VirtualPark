// <copyright file="IAttractionAccessRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;

namespace VirtualPark.Domain.Interfaces.Repositories;

public interface IAttractionAccessRepository
{
    AttractionAccess Add(AttractionAccess isAny);

    IEnumerable<AttractionAccess> GetAll();

    AttractionAccess? GetOpenAccess(Guid attractionId, Guid visitorId);

    int CountOpenAccesses(Guid attractionId);

    void Update(AttractionAccess access);

    AttractionAccess? GetOpenAccessByTicket(Guid ticketId);

    IEnumerable<AttractionAccess> GetAccessesBetweenDates(DateTime startDate, DateTime endDate);

    IEnumerable<AttractionAccess> GetAccessesByVisitorAndDate(Guid visitorId, DateTime date);
}
