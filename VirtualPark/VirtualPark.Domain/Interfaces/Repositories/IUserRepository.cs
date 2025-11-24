// <copyright file="IUserRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;
using VirtualPark.Domain.Enums;

namespace VirtualPark.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    IEnumerable<User> GetAll();

    User? GetById(Guid id);

    User Add(User user);

    bool Update(Guid id, User user);

    User? GetByEmail(string email);

    Visitor ConvertToVisitor(Guid userId, User existingUser, DateTime birthDate, MembershipLevel membershipLevel, Guid nfcId);
}
