// <copyright file="AttractionAccessMappingExtensions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.DTOs.AttractionAccess.Responses;

namespace VirtualPark.DTOs.AttractionAccess;

public static class AttractionAccessMappingExtensions
{
    public static AttractionAccessDto ToDto(this Domain.AttractionAccess attractionAccess)
    {
        return new AttractionAccessDto
        {
            Id = attractionAccess.Id,
            AttractionId = attractionAccess.AttractionId,
            VisitorId = attractionAccess.VisitorId,
            TicketId = attractionAccess.TicketId,
            EntryTime = attractionAccess.EntryTime,
            ExitTime = attractionAccess.ExitTime,
            EntryMethod = attractionAccess.EntryMethod,
            IsClosed = attractionAccess.IsClosed,
        };
    }

    public static AttractionAccessDetailDto ToDetailDto(
        this Domain.AttractionAccess attractionAccess,
        string attractionName,
        string visitorName)
    {
        TimeSpan? duration = null;
        if (attractionAccess.ExitTime.HasValue)
        {
            duration = attractionAccess.ExitTime.Value - attractionAccess.EntryTime;
        }

        return new AttractionAccessDetailDto
        {
            Id = attractionAccess.Id,
            AttractionId = attractionAccess.AttractionId,
            AttractionName = attractionName,
            VisitorId = attractionAccess.VisitorId,
            VisitorName = visitorName,
            TicketId = attractionAccess.TicketId,
            EntryTime = attractionAccess.EntryTime,
            ExitTime = attractionAccess.ExitTime,
            EntryMethod = attractionAccess.EntryMethod,
            IsClosed = attractionAccess.IsClosed,
            Duration = duration,
        };
    }

    public static List<AttractionAccessDto> ToDto(this IEnumerable<Domain.AttractionAccess> attractionAccesses)
    {
        return attractionAccesses.Select(aa => aa.ToDto()).ToList();
    }

    public static List<AttractionAccessDetailDto> ToDetailDto(
        this IEnumerable<Domain.AttractionAccess> attractionAccesses,
        Func<Domain.AttractionAccess, string> getAttractionName,
        Func<Domain.AttractionAccess, string> getVisitorName)
    {
        return attractionAccesses.Select(aa => aa.ToDetailDto(
            getAttractionName(aa),
            getVisitorName(aa))).ToList();
    }
}
