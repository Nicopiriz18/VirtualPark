// <copyright file="AttractionValidationService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using VirtualPark.Application.Clock;
using VirtualPark.Domain;
using VirtualPark.Domain.Interfaces.Repositories;

namespace VirtualPark.Application.Attractions;

public class AttractionValidationService(
    IAttractionRepository repository,
    IMaintenanceRepository maintenanceRepository,
    IClockService clockService) : IAttractionValidationService
{
    private readonly IAttractionRepository repository = repository;
    private readonly IMaintenanceRepository maintenanceRepository = maintenanceRepository;
    private readonly IClockService clockService = clockService;

    public bool HasValidAge(Guid attractionId, int visitorAge)
    {
        var attraction = this.repository.GetById(attractionId) ?? throw new KeyNotFoundException();

        if (visitorAge < attraction.MinAge)
        {
            return false;
        }

        return true;
    }

    public bool IsAttractionAvailable(Guid attractionId)
    {
        var attraction = this.repository.GetById(attractionId) ?? throw new KeyNotFoundException();
        var maintenances = this.maintenanceRepository.GetByAttractionId(attractionId);

        if (this.IsUnderMaintenance(maintenances, this.clockService.GetNow()))
        {
            return false;
        }

        var maintenanceIncidenceIds = new HashSet<Guid>(
            maintenances
                .Where(m => m.AssociatedIncidenceId.HasValue)
                .Select(m => m.AssociatedIncidenceId!.Value));

        foreach (var incidence in attraction.Incidences)
        {
            if (!incidence.Status)
            {
                continue;
            }

            if (maintenanceIncidenceIds.Contains(incidence.Id))
            {
                continue;
            }

            return false;
        }

        return true;
    }

    private bool IsUnderMaintenance(IEnumerable<PreventiveMaintenance> maintenances, DateTime referenceTime)
    {
        foreach (var maintenance in maintenances)
        {
            var start = maintenance.ScheduledDate.Date.Add(maintenance.StartTime);
            var end = start + maintenance.EstimatedDuration;
            if (referenceTime >= start && referenceTime <= end)
            {
                return true;
            }
        }

        return false;
    }
}
