// <copyright file="SpecialEvent.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VirtualPark.Domain;

public class SpecialEvent
{
    public Guid Id { get; private set; }

    public string Name { get; set; }

    public DateTime Date { get; set; }

    public int MaxCapacity { get; set; }

    public decimal AdditionalCost { get; set; }

    private readonly List<Attraction> attractions;

    public IReadOnlyCollection<Attraction> Attractions => this.attractions.AsReadOnly();

    public SpecialEvent(string name, DateTime date, int maxCapacity, decimal additionalCost)
    {
        ValidateName(name);
        ValidateCapacity(maxCapacity);
        ValidateAdditionalCost(additionalCost);
        ValidateDate(date);
        this.Id = Guid.NewGuid();
        this.Date = date;
        this.MaxCapacity = maxCapacity;
        this.AdditionalCost = additionalCost;
        this.attractions = [];
        this.Name = name;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be null or empty", nameof(name));
        }
    }

    private static void ValidateCapacity(int maxCapacity)
    {
        if (maxCapacity <= 0)
        {
            throw new ArgumentException("Max capacity must be greater than zero", nameof(maxCapacity));
        }
    }

    private static void ValidateAdditionalCost(decimal cost)
    {
        if (cost < 0)
        {
            throw new ArgumentException("Additional cost cannot be negative", nameof(cost));
        }
    }

    private static void ValidateDate(DateTime date)
    {
        if (date < DateTime.Today)
        {
            throw new ArgumentException("Date cannot be in the past", nameof(date));
        }
    }

    public void AddAttraction(Attraction attraction)
    {
        ArgumentNullException.ThrowIfNull(attraction);

        this.attractions.Add(attraction);
    }

    public void RemoveAttraction(Guid attractionId)
    {
        var attraction = this.attractions.FirstOrDefault(a => a.Id == attractionId);
        if (attraction != null)
        {
            this.attractions.Remove(attraction);
        }
    }
}
