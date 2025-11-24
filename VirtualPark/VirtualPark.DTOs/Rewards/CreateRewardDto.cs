// <copyright file="CreateRewardDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain.Enums;

namespace VirtualPark.DTOs.Rewards;
public class CreateRewardDto
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int CostInPoints { get; set; }

    public int AvailableQuantity { get; set; }

    public MembershipLevel? RequiredLevel { get; set; }
}
