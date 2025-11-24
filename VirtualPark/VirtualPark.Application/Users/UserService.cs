// <copyright file="UserService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;
using VirtualPark.Domain.Enums;
using VirtualPark.Domain.Interfaces.Repositories;
using VirtualPark.Domain.Interfaces.Security;

namespace VirtualPark.Application.Users;

public class UserService(IUserRepository repository, IPasswordHasher passwordHasher) : IUserService
{
    private readonly IUserRepository repository = repository;
    private readonly IPasswordHasher passwordHasher = passwordHasher;

    public IEnumerable<User> GetAll() => this.repository.GetAll();

    public User GetById(Guid id)
    {
        var user = this.repository.GetById(id);
        if (user is null)
        {
            throw new KeyNotFoundException($"User with id {id} was not found.");
        }

        return user;
    }

    public User? Create(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        // Hash the password before saving
        if (!string.IsNullOrEmpty(user.Password))
        {
            user.Password = this.passwordHasher.Hash(user.Password);
        }

        return this.repository.Add(user);
    }

    public bool Update(Guid id, string name, string surname, string email, string? password = null)
    {
        // Get existing user
        var existingUser = this.GetById(id);

        // Update profile using domain method
        existingUser.UpdateProfile(name, surname, email);

        // Only update password if provided and not empty
        if (!string.IsNullOrWhiteSpace(password))
        {
            existingUser.Password = this.passwordHasher.Hash(password);
        }

        return this.repository.Update(id, existingUser);
    }

    public void AssignRole(Guid userId, RoleEnum role, DateTime? birthDate = null, MembershipLevel? membershipLevel = null)
    {
        var user = this.GetById(userId);

        // If assigning Visitor role, check if conversion is needed
        if (role == RoleEnum.Visitor)
        {
            // If user is already a Visitor, just assign the role
            if (user is Visitor)
            {
                user.AssignRole(role);
                this.repository.Update(userId, user);
            }
            else
            {
                // Converting User to Visitor requires BirthDate
                if (!birthDate.HasValue)
                {
                    throw new ArgumentException("BirthDate es requerido para convertir un usuario en Visitor.", nameof(birthDate));
                }

                // Use provided membership level or default to Standard
                var membership = membershipLevel ?? MembershipLevel.Standard;

                // Convert User to Visitor
                this.repository.ConvertToVisitor(userId, user, birthDate.Value, membership, Guid.NewGuid());
            }
        }
        else
        {
            // For other roles, just assign normally
            user.AssignRole(role);
            this.repository.Update(userId, user);
        }
    }
}
