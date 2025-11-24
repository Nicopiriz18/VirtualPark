// <copyright file="Incidence.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VirtualPark.Domain;

public class Incidence(string title, string description, bool status, DateTime date, Guid attractionId)
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Title { get; private set; } = title;

    public string Description { get; private set; } = description;

    public bool Status { get; private set; } = status;

    public DateTime Date { get; private set; } = date;

    public Guid AttractionId { get; private set; } = attractionId;

    public Attraction Attraction { get; private set; } = null!;

    public void Close()
    {
        if (!this.Status)
        {
            throw new InvalidOperationException("La incidencia ya está cerrada.");
        }

        this.Status = false;
    }

    public void Reopen()
    {
        if (this.Status)
        {
            throw new InvalidOperationException("La incidencia ya está activa.");
        }

        this.Status = true;
    }
}
