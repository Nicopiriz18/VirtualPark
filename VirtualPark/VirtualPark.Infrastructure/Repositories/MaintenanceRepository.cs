// <copyright file="MaintenanceRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;
using VirtualPark.Domain.Interfaces.Repositories;
using VirtualPark.Infrastructure.Data;

namespace VirtualPark.Infrastructure.Repositories;

public class MaintenanceRepository(ParkDbContext context) : IMaintenanceRepository
{
    private readonly ParkDbContext context = context;

    public PreventiveMaintenance Add(PreventiveMaintenance maintenance)
    {
        this.context.PreventiveMaintenances.Add(maintenance);
        this.context.SaveChanges();
        return maintenance;
    }

    public PreventiveMaintenance? GetById(Guid id)
    {
        return this.context.PreventiveMaintenances.FirstOrDefault(m => m.Id == id);
    }

    public IEnumerable<PreventiveMaintenance> GetByAttractionId(Guid attractionId)
    {
        return this.context.PreventiveMaintenances
            .Where(m => m.AttractionId == attractionId)
            .ToList();
    }

    public IEnumerable<PreventiveMaintenance> GetAll()
    {
        return this.context.PreventiveMaintenances.ToList();
    }

    public void Update(PreventiveMaintenance maintenance)
    {
        this.context.PreventiveMaintenances.Update(maintenance);
        this.context.SaveChanges();
    }

    public void Delete(Guid id)
    {
        var maintenance = this.context.PreventiveMaintenances.Find(id);
        if (maintenance is not null)
        {
            this.context.PreventiveMaintenances.Remove(maintenance);
            this.context.SaveChanges();
        }
    }
}
