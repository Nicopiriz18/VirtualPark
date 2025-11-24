// <copyright file="IClockRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;

namespace VirtualPark.Domain.Interfaces.Repositories;

public interface IClockRepository
{
    ClockConfiguration? GetConfiguration();

    void SaveConfiguration(ClockConfiguration configuration);
}
