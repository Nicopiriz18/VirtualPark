// <copyright file="RegisterResponseDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VirtualPark.DTOs.Auth.Responses;

public class RegisterResponseDto
{
    public required Guid Id { get; set; }

    public required string FullName { get; set; }

    public required string? Email { get; set; }
}
