// <copyright file="IMaintenanceRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VirtualPark.Domain.Interfaces.Repositories;

public interface IMaintenanceRepository
{
    PreventiveMaintenance Add(PreventiveMaintenance maintenance);

    PreventiveMaintenance? GetById(Guid id);

    IEnumerable<PreventiveMaintenance> GetByAttractionId(Guid attractionId);

    IEnumerable<PreventiveMaintenance> GetAll();

    void Update(PreventiveMaintenance maintenance);

    void Delete(Guid id);
}
