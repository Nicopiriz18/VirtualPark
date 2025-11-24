// <copyright file="UserRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;
using VirtualPark.Domain;
using VirtualPark.Domain.Enums;
using VirtualPark.Domain.Interfaces.Repositories;
using VirtualPark.Infrastructure.Data;

namespace VirtualPark.Infrastructure.Repositories;

public class UserRepository(ParkDbContext context) : IUserRepository
{
    private readonly ParkDbContext context = context;

    public IEnumerable<User> GetAll() =>
        this.context.Users.AsNoTracking().ToList();

    public User? GetById(Guid id) =>
        this.context.Users.Find(id);

    public User Add(User user)
    {
        this.context.Users.Add(user);
        this.context.SaveChanges();
        return user;
    }

    public bool Update(Guid id, User user)
    {
        var existing = this.context.Users.Find(id);
        if (existing is null)
        {
            return false;
        }

        this.context.Entry(existing).Property(nameof(User.Name)).CurrentValue = user.Name;
        this.context.Entry(existing).Property(nameof(User.Surname)).CurrentValue = user.Surname;
        this.context.Entry(existing).Property(nameof(User.Email)).CurrentValue = user.Email;
        this.context.Entry(existing).Property(nameof(User.Password)).CurrentValue = user.Password;
        this.context.Entry(existing).Property("roles").CurrentValue = user.Roles.ToList();
        this.context.SaveChanges();
        return true;
    }

    public User? GetByEmail(string email) =>
        this.context.Users.AsNoTracking().FirstOrDefault(u => u.Email == email);

    public Visitor ConvertToVisitor(Guid userId, User existingUser, DateTime birthDate, MembershipLevel membershipLevel, Guid nfcId)
    {
        // Remove the old User entity
        var userToRemove = this.context.Users.Find(userId);
        if (userToRemove is null)
        {
            throw new KeyNotFoundException($"User with id {userId} was not found.");
        }

        this.context.Users.Remove(userToRemove);
        this.context.SaveChanges();

        // Create new Visitor with the same Id and user data
        var visitor = new Visitor(
            existingUser.Name!,
            existingUser.Surname!,
            existingUser.Email!,
            existingUser.Password!,
            birthDate,
            membershipLevel,
            nfcId);

        // Manually set the Id to match the old user
        typeof(User).GetProperty("Id")!.SetValue(visitor, userId);

        // Restore roles from the existing user
        foreach (var role in existingUser.Roles)
        {
            visitor.AssignRole(role);
        }

        // Add the new Visitor entity
        this.context.Visitors.Add(visitor);
        this.context.SaveChanges();

        return visitor;
    }
}
