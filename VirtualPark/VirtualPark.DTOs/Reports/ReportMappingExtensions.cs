// <copyright file="ReportMappingExtensions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain.Enums;
using VirtualPark.DTOs.Reports.Responses;

namespace VirtualPark.DTOs.Reports;

public static class ReportMappingExtensions
{
    public static AttractionUsageReportDto ToAttractionUsageReportDto(
        Guid attractionId,
        string attractionName,
        string attractionType,
        int visitCount,
        DateTime startDate,
        DateTime endDate,
        int capacity = 0,
        decimal occupancyRate = 0,
        int averageVisitsPerDay = 0,
        int peakDayVisits = 0,
        DateTime? peakDay = null)
    {
        return new AttractionUsageReportDto
        {
            AttractionId = attractionId,
            AttractionName = attractionName,
            AttractionType = attractionType,
            VisitCount = visitCount,
            StartDate = startDate,
            EndDate = endDate,
            Capacity = capacity,
            OccupancyRate = occupancyRate,
            AverageVisitsPerDay = averageVisitsPerDay,
            PeakDayVisits = peakDayVisits,
            PeakDay = peakDay,
        };
    }

    public static VisitorActivityReportDto ToVisitorActivityReportDto(
        Guid visitorId,
        string visitorName,
        string email,
        MembershipLevel membershipLevel,
        int totalVisits,
        int totalAttractionAccesses,
        int totalPoints,
        DateTime startDate,
        DateTime endDate,
        int uniqueAttractionsVisited = 0,
        DateTime? lastVisitDate = null,
        int ticketsPurchased = 0,
        List<string>? mostVisitedAttractions = null)
    {
        return new VisitorActivityReportDto
        {
            VisitorId = visitorId,
            VisitorName = visitorName,
            Email = email,
            MembershipLevel = membershipLevel,
            TotalVisits = totalVisits,
            TotalAttractionAccesses = totalAttractionAccesses,
            TotalPoints = totalPoints,
            StartDate = startDate,
            EndDate = endDate,
            UniqueAttractionsVisited = uniqueAttractionsVisited,
            LastVisitDate = lastVisitDate,
            TicketsPurchased = ticketsPurchased,
            MostVisitedAttractions = mostVisitedAttractions,
        };
    }
}
