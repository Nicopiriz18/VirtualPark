// <copyright file="Attraction.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VirtualPark.Domain;

public class Attraction(string name, string description, string type, int minAge, int capacity)
{
    public string Name { get; set; } = ValidateName(name);

    public string Description { get; set; } = description;

    public string Type { get; set; } = type;

    public int MinAge { get; set; } = ValidateMinAge(minAge);

    public int Capacity { get; set; } = ValidateCapacity(capacity);

    public Guid Id { get; set; } = Guid.NewGuid();

    private readonly List<Incidence> incidences = [];

    public IReadOnlyCollection<Incidence> Incidences => this.incidences.AsReadOnly();

    public void ModifyAttraction(string name, string description, string type, int minAge, int capacity)
    {
        this.Name = ValidateName(name);
        this.Description = description;
        this.Type = type;
        this.MinAge = ValidateMinAge(minAge);
        this.Capacity = ValidateCapacity(capacity);
    }

    private static string ValidateName(string name)
    {
        return name ?? throw new ArgumentNullException(nameof(name), "Name cannot be null");
    }

    private static int ValidateMinAge(int minAge)
    {
        if (minAge < 0)
        {
            throw new ArgumentException("MinAge cannot be negative", nameof(minAge));
        }

        return minAge;
    }

    private static int ValidateCapacity(int capacity)
    {
        if (capacity < 0)
        {
            throw new ArgumentException("Capacity cannot be negative", nameof(capacity));
        }

        return capacity;
    }

    public Incidence AddIncidence(string title, string description, bool status, DateTime date)
    {
        var incidence = new Incidence(title, description, status, date, this.Id);
        this.incidences.Add(incidence);
        return incidence;
    }

    public void RemoveIncidence(Guid incidenceId)
    {
        var incidence = this.incidences.FirstOrDefault(i => i.Id == incidenceId);
        if (incidence != null)
        {
            this.incidences.Remove(incidence);
        }
    }
}
