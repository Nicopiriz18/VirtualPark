// <copyright file="IPasswordHasher.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VirtualPark.Domain.Interfaces.Security;

public interface IPasswordHasher
{
    string Hash(string plain);

    bool Verify(string plain, string hash);
}
