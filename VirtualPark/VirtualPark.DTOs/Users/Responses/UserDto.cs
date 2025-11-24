// <copyright file="UserDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain.Enums;

namespace VirtualPark.DTOs.Users.Responses;

public class UserDto
{
    public required Guid Id { get; set; }

    public required string Name { get; set; }

    public required string Surname { get; set; }

    public required string Email { get; set; }

    public required List<RoleEnum> Roles { get; set; }
}
