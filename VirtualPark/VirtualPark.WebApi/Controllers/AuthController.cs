// <copyright file="AuthController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using Microsoft.AspNetCore.Mvc;
using VirtualPark.Application.Session;
using VirtualPark.Application.Users;
using VirtualPark.Domain.Enums;
using VirtualPark.DTOs.Auth.Requests;
using VirtualPark.DTOs.Auth.Responses;

namespace VirtualPark.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService service, ISessionService sessionService) : ControllerBase
{
    private readonly IAuthService service = service;
    private readonly ISessionService sessionService = sessionService;

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequestDto request)
    {
        var (id, email, name, surname) = this.service.Register(
            request.Name,
            request.Surname,
            request.Email,
            request.Password,
            request.Birthday);

        var response = new RegisterResponseDto
        {
            Id = id,
            Email = email,
            FullName = $"{name} {surname}",
        };

        return this.Created($"/api/v1/users/{id}", response);
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequestDto request)
    {
        var token = this.service.Login(request.Email, request.Password);
        var user = this.sessionService.GetByToken(token);
        var roles = user?.Roles.ToList() ?? new List<RoleEnum>();
        var response = new AuthResultDto
        {
            Token = token,
            Roles = roles,
            UserId = user?.Id ?? Guid.Empty,
            Email = user?.Email,
        };
        return this.Ok(response);
    }
}
