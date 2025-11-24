// <copyright file="ITokenGenerator.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;

namespace VirtualPark.Domain.Interfaces.Security;

public interface ITokenGenerator
{
    string GenerateToken(User user);
}
