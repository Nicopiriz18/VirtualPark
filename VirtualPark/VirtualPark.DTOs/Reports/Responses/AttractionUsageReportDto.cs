// <copyright file="AttractionUsageReportDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VirtualPark.DTOs.Reports.Responses;

public class AttractionUsageReportDto
{
    public required Guid AttractionId { get; set; }

    public required string AttractionName { get; set; }

    public required string AttractionType { get; set; }

    public required int VisitCount { get; set; }

    public required DateTime StartDate { get; set; }

    public required DateTime EndDate { get; set; }

    public int Capacity { get; set; }

    public decimal OccupancyRate { get; set; }

    public int AverageVisitsPerDay { get; set; }

    public int PeakDayVisits { get; set; }

    public DateTime? PeakDay { get; set; }
}
