// <copyright file="TokenGenerator.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;
using VirtualPark.Domain.Interfaces.Repositories;
using VirtualPark.Domain.Interfaces.Security;

namespace VirtualPark.Infrastructure.Security;

public class TokenGenerator(ISessionRepository sessionRepository) : ITokenGenerator
{
    private readonly ISessionRepository sessionRepository = sessionRepository;

    public string GenerateToken(User user)
    {
        // Generar un token aleatorio seguro
        var token = GenerateSecureToken();

        // Crear la sesión
        var session = new Session
        {
            UserId = user.Id,
            Token = token,
        };

        // Guardar la sesión en la base de datos
        this.sessionRepository.Add(session);

        return token;
    }

    private static string GenerateSecureToken()
    {
        // Generar 32 bytes aleatorios (256 bits)
        var randomBytes = new byte[32];
        using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }

        // Convertir a Base64 para obtener un string seguro
        return Convert.ToBase64String(randomBytes);
    }
}
