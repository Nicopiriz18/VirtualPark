// <copyright file="VisitorProfileDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain.Enums;

namespace VirtualPark.DTOs.Visitors.Responses;

public class VisitorProfileDto
{
    public required Guid Id { get; set; }

    public required string Name { get; set; }

    public required string Surname { get; set; }

    public required string Email { get; set; }

    public required DateTime BirthDate { get; set; }

    public required MembershipLevel MembershipLevel { get; set; }

    public required Guid NfcId { get; set; }

    public required string FullName { get; set; }

    public int Age { get; set; }
}
