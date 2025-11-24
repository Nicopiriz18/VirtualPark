// <copyright file="IUserService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;
using VirtualPark.Domain.Enums;

namespace VirtualPark.Application.Users;

public interface IUserService
{
    IEnumerable<User> GetAll();

    User GetById(Guid id);

    User? Create(User user);

    bool Update(Guid id, string name, string surname, string email, string? password = null);

    void AssignRole(Guid userId, RoleEnum role, DateTime? birthDate = null, MembershipLevel? membershipLevel = null);
}
