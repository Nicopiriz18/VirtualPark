// <copyright file="ClockConfiguration.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VirtualPark.Domain;

public class ClockConfiguration
{
    public Guid Id { get; set; }

    public DateTime? CustomDateTime { get; set; }
}
