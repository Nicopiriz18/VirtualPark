// <copyright file="ReportsController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using VirtualPark.Application.Reports;
using VirtualPark.Domain.Enums;
using VirtualPark.DTOs.Reports;
using VirtualPark.WebApi.Filters;

namespace VirtualPark.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[AuthorizationFilter(RoleEnum.Administrator)]
public class ReportsController(IReportsService reportsService) : ControllerBase
{
    [HttpGet("attraction-usage")]
    public IActionResult GetAttractionUsage([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        if (startDate > endDate)
        {
            return this.BadRequest("La fecha de inicio no puede ser mayor a la fecha de fin.");
        }

        var reportData = reportsService.GetAttractionUsageReport(startDate, endDate);

        // Mapeo de Domain a DTO usando el mapper
        var reportDto = reportData.Select(data =>
        {
            var totalDays = (endDate - startDate).Days + 1;
            var averageVisitsPerDay = data.VisitCount / totalDays;
            var occupancyRate = (decimal)data.VisitCount / (data.Attraction.Capacity * totalDays) * 100;

            return ReportMappingExtensions.ToAttractionUsageReportDto(
                attractionId: data.Attraction.Id,
                attractionName: data.Attraction.Name,
                attractionType: data.Attraction.Type.ToString(),
                visitCount: data.VisitCount,
                startDate: startDate,
                endDate: endDate,
                capacity: data.Attraction.Capacity,
                occupancyRate: occupancyRate,
                averageVisitsPerDay: averageVisitsPerDay,
                peakDayVisits: 0,
                peakDay: null);
        }).ToList();

        return this.Ok(reportDto);
    }
}
