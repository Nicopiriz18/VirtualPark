// <copyright file="PreventiveMaintenance.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VirtualPark.Domain;

public class PreventiveMaintenance
{
    public PreventiveMaintenance(
        Guid attractionId,
        DateTime scheduledDate,
        TimeSpan startTime,
        TimeSpan estimatedDuration,
        string description)
    {
        if (attractionId == Guid.Empty)
        {
            throw new ArgumentException("AttractionId cannot be empty", nameof(attractionId));
        }

        if (estimatedDuration <= TimeSpan.Zero)
        {
            throw new ArgumentException("EstimatedDuration must be greater than zero", nameof(estimatedDuration));
        }

        if (description is null)
        {
            throw new ArgumentNullException(nameof(description));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Description cannot be empty", nameof(description));
        }

        this.Id = Guid.NewGuid();
        this.AttractionId = attractionId;
        this.ScheduledDate = scheduledDate;
        this.StartTime = startTime;
        this.EstimatedDuration = estimatedDuration;
        this.Description = description;
    }

    public Guid Id { get; private set; }

    public Guid AttractionId { get; private set; }

    public DateTime ScheduledDate { get; private set; }

    public TimeSpan StartTime { get; private set; }

    public TimeSpan EstimatedDuration { get; private set; }

    public string Description { get; private set; }

    public Guid? AssociatedIncidenceId { get; private set; }

    public Attraction Attraction { get; private set; } = null!;

    public void SetAssociatedIncidence(Guid incidenceId)
    {
        this.AssociatedIncidenceId = incidenceId;
    }
}
