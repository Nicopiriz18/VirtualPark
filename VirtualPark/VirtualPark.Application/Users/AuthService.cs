// <copyright file="AuthService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Application.Session;
using VirtualPark.Domain;
using VirtualPark.Domain.Enums;
using VirtualPark.Domain.Exceptions;
using VirtualPark.Domain.Interfaces.Repositories;
using VirtualPark.Domain.Interfaces.Security;

namespace VirtualPark.Application.Users;

public class AuthService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ITokenGenerator tokenGenerator,
    ISessionService sessionService) : IAuthService
{
    private readonly IUserRepository userRepository = userRepository;
    private readonly IPasswordHasher passwordHasher = passwordHasher;
    private readonly ITokenGenerator tokenGenerator = tokenGenerator;
    private readonly ISessionService sessionService = sessionService;

    public (Guid Id, string Email, string Name, string Surname) Register(string name, string surname, string email, string password, DateTime birthday)
    {
        var user = new Visitor(
            name,
            surname,
            email,
            this.passwordHasher.Hash(password),
            birthday,
            MembershipLevel.Standard,
            Guid.NewGuid());

        // Guardar el usuario
        var createdUser = this.userRepository.Add(user);

        // Retornar datos del usuario sin la contraseña
        return (createdUser.Id, createdUser.Email ?? string.Empty, createdUser.Name ?? string.Empty, createdUser.Surname ?? string.Empty);
    }

    public string Login(string email, string password)
    {
        // Buscar usuario por email
        var user = this.userRepository.GetByEmail(email);

        if (user == null)
        {
            throw new InvalidCredentialsException("Invalid email or password");
        }

        // Verificar la contraseña
        if (user.Password != null && !this.passwordHasher.Verify(password, user.Password))
        {
            throw new InvalidCredentialsException("Invalid email or password");
        }

        // Generar token
        var token = this.tokenGenerator.GenerateToken(user);
        this.sessionService.CreateSession(user.Id, token);
        return token;
    }
}
