// <copyright file="MaintenanceService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Application.Attractions;
using VirtualPark.Application.Clock;
using VirtualPark.Domain;
using VirtualPark.Domain.Interfaces.Repositories;

namespace VirtualPark.Application.Maintenance;

public class MaintenanceService(
    IMaintenanceRepository maintenanceRepository,
    IAttractionRepository attractionRepository,
    IAttractionIncidenceService incidenceService,
    IClockService clockService) : IMaintenanceService
{
    private readonly IMaintenanceRepository maintenanceRepository = maintenanceRepository;
    private readonly IAttractionRepository attractionRepository = attractionRepository;
    private readonly IAttractionIncidenceService incidenceService = incidenceService;
    private readonly IClockService clockService = clockService;

    public PreventiveMaintenance ScheduleMaintenance(
        Guid attractionId,
        DateTime scheduledDate,
        TimeSpan startTime,
        TimeSpan estimatedDuration,
        string description)
    {
        // Validate attraction exists
        var attraction = this.attractionRepository.GetById(attractionId)
            ?? throw new KeyNotFoundException($"Atracción {attractionId} no encontrada");

        // Create the maintenance entity
        var maintenance = new PreventiveMaintenance(
            attractionId,
            scheduledDate,
            startTime,
            estimatedDuration,
            description);

        // Save the maintenance first
        this.maintenanceRepository.Add(maintenance);

        // Create the associated incidence
        var incidence = this.incidenceService.AddIncidence(
            attractionId,
            "Mantenimiento Programado",
            description,
            true,
            scheduledDate);

        // Associate the incidence with the maintenance
        maintenance.SetAssociatedIncidence(incidence.Id);
        this.maintenanceRepository.Update(maintenance);

        return maintenance;
    }

    public PreventiveMaintenance? GetMaintenanceById(Guid id)
    {
        return this.maintenanceRepository.GetById(id);
    }

    public IEnumerable<PreventiveMaintenance> GetMaintenancesByAttraction(Guid attractionId)
    {
        return this.maintenanceRepository.GetByAttractionId(attractionId);
    }

    public void CancelMaintenance(Guid id)
    {
        var maintenance = this.maintenanceRepository.GetById(id)
            ?? throw new KeyNotFoundException($"Mantenimiento {id} no encontrado");

        // Close the associated incidence if it exists
        if (maintenance.AssociatedIncidenceId.HasValue)
        {
            this.incidenceService.CloseIncidence(
                maintenance.AttractionId,
                maintenance.AssociatedIncidenceId.Value);
        }

        // Delete the maintenance
        this.maintenanceRepository.Delete(id);
    }
}
