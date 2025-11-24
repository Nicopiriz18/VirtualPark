// <copyright file="MaintenanceDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VirtualPark.DTOs.Maintenance.Responses;

public class MaintenanceDto
{
    public required Guid Id { get; set; }

    public required Guid AttractionId { get; set; }

    public required DateTime ScheduledDate { get; set; }

    public required TimeSpan StartTime { get; set; }

    public required TimeSpan EstimatedDuration { get; set; }

    public required string Description { get; set; }

    public Guid? AssociatedIncidenceId { get; set; }
}
