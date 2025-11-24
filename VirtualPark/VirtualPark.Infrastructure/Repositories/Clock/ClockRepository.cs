// <copyright file="ClockRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;
using VirtualPark.Domain;
using VirtualPark.Domain.Interfaces.Repositories;
using VirtualPark.Infrastructure.Data;

namespace VirtualPark.Infrastructure.Repositories;

public class ClockRepository(ParkDbContext context) : IClockRepository
{
    private readonly ParkDbContext context = context;
    private static readonly Guid ConfigurationId = Guid.Parse("00000000-0000-0000-0000-000000000003");

    public ClockConfiguration? GetConfiguration()
    {
        return this.context.ClockConfigurations
            .AsNoTracking()
            .FirstOrDefault(c => c.Id == ConfigurationId);
    }

    public void SaveConfiguration(ClockConfiguration configuration)
    {
        var existing = this.context.ClockConfigurations
            .FirstOrDefault(c => c.Id == ConfigurationId);

        if (existing != null)
        {
            existing.CustomDateTime = configuration.CustomDateTime;
            this.context.ClockConfigurations.Update(existing);
        }
        else
        {
            configuration.Id = ConfigurationId;
            this.context.ClockConfigurations.Add(configuration);
        }

        this.context.SaveChanges();
    }
}
