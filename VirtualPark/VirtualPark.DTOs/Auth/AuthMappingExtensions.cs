// <copyright file="AuthMappingExtensions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;
using VirtualPark.DTOs.Auth.Responses;

namespace VirtualPark.DTOs.Auth;

public static class AuthMappingExtensions
{
    public static RegisterResponseDto ToRegisterResponseDto(this Visitor visitor)
    {
        return new RegisterResponseDto
        {
            Id = visitor.Id,
            FullName = $"{visitor.Name} {visitor.Surname}",
            Email = visitor.Email,
        };
    }
}
