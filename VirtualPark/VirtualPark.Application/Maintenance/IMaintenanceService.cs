// <copyright file="IMaintenanceService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;

namespace VirtualPark.Application.Maintenance;

public interface IMaintenanceService
{
    PreventiveMaintenance ScheduleMaintenance(
        Guid attractionId,
        DateTime scheduledDate,
        TimeSpan startTime,
        TimeSpan estimatedDuration,
        string description);

    PreventiveMaintenance? GetMaintenanceById(Guid id);

    IEnumerable<PreventiveMaintenance> GetMaintenancesByAttraction(Guid attractionId);

    void CancelMaintenance(Guid id);
}
