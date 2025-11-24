// <copyright file="ReportsService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain.Interfaces.Repositories;

namespace VirtualPark.Application.Reports;

public class ReportsService(IAttractionAccessRepository attractionAccessRepository, IAttractionRepository attractionRepository) : IReportsService
{
    private readonly IAttractionAccessRepository attractionAccessRepository = attractionAccessRepository;
    private readonly IAttractionRepository attractionRepository = attractionRepository;

    public IEnumerable<AttractionUsageData> GetAttractionUsageReport(DateTime startDate, DateTime endDate)
    {
        var accesses = this.attractionAccessRepository.GetAccessesBetweenDates(startDate, endDate);

        var reportData = accesses
            .GroupBy(a => a.AttractionId)
            .Select(g => new
            {
                AttractionId = g.Key,
                VisitCount = g.Count(),
            });

        var result = new List<AttractionUsageData>();
        foreach (var item in reportData)
        {
            var attraction = this.attractionRepository.GetById(item.AttractionId);
            if (attraction != null)
            {
                result.Add(new AttractionUsageData
                {
                    Attraction = attraction,
                    VisitCount = item.VisitCount,
                });
            }
        }

        return result;
    }
}
