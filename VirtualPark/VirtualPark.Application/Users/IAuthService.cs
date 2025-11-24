// <copyright file="IAuthService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VirtualPark.Application.Users;

public interface IAuthService
{
    (Guid Id, string Email, string Name, string Surname) Register(string name, string surname, string email, string password, DateTime birthday);

    string Login(string email, string password);
}
