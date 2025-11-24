// <copyright file="Reward.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain.Enums;

namespace VirtualPark.Domain;
public class Reward
{
    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public int CostInPoints { get; private set; }

    public int AvailableQuantity { get; private set; }

    public MembershipLevel? RequiredLevel { get; private set; }

    public Reward(string name, string description, int costInPoints, int availableQuantity,
        Enums.MembershipLevel? requiredLevel = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is required");
        }

        if (costInPoints <= 0)
        {
            throw new ArgumentException("Cost must be positive");
        }

        if (availableQuantity < 0)
        {
            throw new ArgumentException("Quantity must be >= 0");
        }

        this.Id = Guid.NewGuid();
        this.Name = name;
        this.Description = description;
        this.CostInPoints = costInPoints;
        this.AvailableQuantity = availableQuantity;
        this.RequiredLevel = requiredLevel;
    }

    public void DecreaseStock()
    {
        if (this.AvailableQuantity <= 0)
        {
            throw new InvalidOperationException("Reward not available");
        }

        this.AvailableQuantity--;
    }
}
