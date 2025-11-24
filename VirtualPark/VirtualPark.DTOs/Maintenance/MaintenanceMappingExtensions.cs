// <copyright file="MaintenanceMappingExtensions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;
using VirtualPark.DTOs.Maintenance.Responses;

namespace VirtualPark.DTOs.Maintenance;

public static class MaintenanceMappingExtensions
{
    public static MaintenanceDto ToDto(this PreventiveMaintenance maintenance)
    {
        return new MaintenanceDto
        {
            Id = maintenance.Id,
            AttractionId = maintenance.AttractionId,
            ScheduledDate = maintenance.ScheduledDate,
            StartTime = maintenance.StartTime,
            EstimatedDuration = maintenance.EstimatedDuration,
            Description = maintenance.Description,
            AssociatedIncidenceId = maintenance.AssociatedIncidenceId,
        };
    }

    public static List<MaintenanceDto> ToDto(this IEnumerable<PreventiveMaintenance> maintenances)
    {
        return maintenances.Select(m => m.ToDto()).ToList();
    }
}
