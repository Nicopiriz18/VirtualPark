// <copyright file="IVisitorService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;
using VirtualPark.Domain.Enums;

namespace VirtualPark.Application.Visitors;

public interface IVisitorService
{
    public Visitor RegisterUser(string name, string surname, string email, string password, DateTime birthDate,
        Guid nfcId);

    public Visitor RegisterUserByAdmin(string name, string surname, string email, string password, DateTime birthDate, Guid nfcId,
        MembershipLevel membershipLevel);

    public void UpdateProfile(Guid visitorId, string newName, string newSurname, string newEmail);

    public void UpdateProfileWithPassword(Guid visitorId, string newName, string newSurname, string newEmail, string? newPassword);
}
