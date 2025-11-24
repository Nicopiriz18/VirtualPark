// <copyright file="ClockService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;
using VirtualPark.Domain.Interfaces.Repositories;

namespace VirtualPark.Application.Clock;

public class ClockService(IClockRepository repository) : IClockService
{
    private readonly IClockRepository repository = repository;
    private DateTime? cachedCustomNow;
    private bool isInitialized = false;

    public DateTime GetNow()
    {
        this.EnsureInitialized();
        return this.cachedCustomNow ?? DateTime.Now;
    }

    public void SetNow(DateTime value)
    {
        this.cachedCustomNow = value;
        this.isInitialized = true;
        this.repository.SaveConfiguration(new ClockConfiguration { CustomDateTime = value });
    }

    private void EnsureInitialized()
    {
        if (this.isInitialized)
        {
            return;
        }

        var config = this.repository.GetConfiguration();
        this.cachedCustomNow = config?.CustomDateTime;
        this.isInitialized = true;
    }
}
