// <copyright file="VisitorMappingExtensions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;
using VirtualPark.DTOs.Tickets;
using VirtualPark.DTOs.Visitors.Responses;

namespace VirtualPark.DTOs.Visitors;

public static class VisitorMappingExtensions
{
    public static VisitorDto ToDto(this Visitor visitor)
    {
        return new VisitorDto
        {
            Id = visitor.Id,
            Name = visitor.Name!,
            Surname = visitor.Surname!,
            Email = visitor.Email!,
            BirthDate = visitor.BirthDate,
            MembershipLevel = visitor.MembershipLevel,
            NfcId = visitor.NfcId,
        };
    }

    public static VisitorDetailDto ToDetailDto(this Visitor visitor)
    {
        return new VisitorDetailDto
        {
            Id = visitor.Id,
            Name = visitor.Name!,
            Surname = visitor.Surname!,
            Email = visitor.Email!,
            BirthDate = visitor.BirthDate,
            MembershipLevel = visitor.MembershipLevel,
            NfcId = visitor.NfcId,
            Tickets = visitor.Tickets.ToDto(),
            TotalTickets = visitor.Tickets.Count,
        };
    }

    public static VisitorProfileDto ToProfileDto(this Visitor visitor)
    {
        var age = DateTime.UtcNow.Year - visitor.BirthDate.Year;
        if (visitor.BirthDate.Date > DateTime.UtcNow.AddYears(-age))
        {
            age--;
        }

        return new VisitorProfileDto
        {
            Id = visitor.Id,
            Name = visitor.Name!,
            Surname = visitor.Surname!,
            Email = visitor.Email!,
            BirthDate = visitor.BirthDate,
            MembershipLevel = visitor.MembershipLevel,
            NfcId = visitor.NfcId,
            FullName = $"{visitor.Name} {visitor.Surname}",
            Age = age,
        };
    }

    public static List<VisitorDto> ToDto(this IEnumerable<Visitor> visitors)
    {
        return visitors.Select(v => v.ToDto()).ToList();
    }

    public static List<VisitorDetailDto> ToDetailDto(this IEnumerable<Visitor> visitors)
    {
        return visitors.Select(v => v.ToDetailDto()).ToList();
    }
}
