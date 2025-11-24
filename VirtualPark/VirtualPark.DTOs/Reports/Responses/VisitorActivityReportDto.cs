// <copyright file="VisitorActivityReportDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain.Enums;

namespace VirtualPark.DTOs.Reports.Responses;

public class VisitorActivityReportDto
{
    public required Guid VisitorId { get; set; }

    public required string VisitorName { get; set; }

    public required string Email { get; set; }

    public required MembershipLevel MembershipLevel { get; set; }

    public required int TotalVisits { get; set; }

    public required int TotalAttractionAccesses { get; set; }

    public required int TotalPoints { get; set; }

    public required DateTime StartDate { get; set; }

    public required DateTime EndDate { get; set; }

    public int UniqueAttractionsVisited { get; set; }

    public DateTime? LastVisitDate { get; set; }

    public int TicketsPurchased { get; set; }

    public List<string>? MostVisitedAttractions { get; set; }
}
