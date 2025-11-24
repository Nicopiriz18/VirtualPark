// <copyright file="AuthResultDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VirtualPark.DTOs.Auth.Responses;

using System;
using VirtualPark.Domain.Enums;

public class AuthResultDto
{
    public required string Token { get; set; }

    public required List<RoleEnum> Roles { get; set; }

    public Guid UserId { get; set; }

    public string? Email { get; set; }
}
