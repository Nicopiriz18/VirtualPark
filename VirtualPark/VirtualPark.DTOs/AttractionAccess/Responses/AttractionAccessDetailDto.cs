// <copyright file="AttractionAccessDetailDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain.Enums;

namespace VirtualPark.DTOs.AttractionAccess.Responses;

public class AttractionAccessDetailDto
{
    public required Guid Id { get; set; }

    public required Guid AttractionId { get; set; }

    public required string AttractionName { get; set; }

    public required Guid VisitorId { get; set; }

    public required string VisitorName { get; set; }

    public Guid? TicketId { get; set; }

    public required DateTime EntryTime { get; set; }

    public DateTime? ExitTime { get; set; }

    public required EntryMethod EntryMethod { get; set; }

    public required bool IsClosed { get; set; }

    public TimeSpan? Duration { get; set; }
}
