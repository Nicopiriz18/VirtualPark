// <copyright file="User.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain.Enums;

namespace VirtualPark.Domain;

public class User
{
    protected User()
    {
        this.roles = [];
    }

    public User(string name, string surname, string email, string password, RoleEnum role)
        : this()
    {
        this.Id = Guid.NewGuid();
        this.Name = ValidateNotEmpty(name, nameof(name));
        this.Surname = ValidateNotEmpty(surname, nameof(surname));
        this.Email = ValidateNotEmpty(email, nameof(email));
        this.Password = ValidateNotEmpty(password, nameof(password));
        this.roles.Add(role);
    }

    public Guid Id { get; private set; }

    public string? Name { get; private set; }

    public string? Surname { get; private set; }

    public string? Email { get; private set; }

    public string? Password { get; set; }

    private readonly List<RoleEnum> roles;

    public IReadOnlyCollection<RoleEnum> Roles => this.roles.AsReadOnly();

    public void AssignRole(RoleEnum role)
    {
        if (!this.roles.Contains(role))
        {
            this.roles.Add(role);
        }
    }

    public void UpdateProfile(string name, string surname, string email)
    {
        this.Name = ValidateNotEmpty(name, nameof(name));
        this.Surname = ValidateNotEmpty(surname, nameof(surname));
        this.Email = ValidateNotEmpty(email, nameof(email));
    }

    private static string ValidateNotEmpty(string value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException($"{propertyName} cannot be empty.", propertyName);
        }

        return value;
    }
}
