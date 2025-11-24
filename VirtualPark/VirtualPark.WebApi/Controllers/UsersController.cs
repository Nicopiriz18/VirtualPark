// <copyright file="UsersController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using VirtualPark.Application.Users;
using VirtualPark.Domain;
using VirtualPark.Domain.Enums;
using VirtualPark.DTOs.Users;
using VirtualPark.DTOs.Users.Requests;
using VirtualPark.WebApi.Filters;

namespace VirtualPark.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IUserService service) : ControllerBase
{
    [HttpGet]
    [AuthorizationFilter(RoleEnum.Administrator)]
    public IActionResult Get()
    {
        var users = service.GetAll();
        return this.Ok(users.ToListDto());
    }

    [HttpGet("{id}")]
    [AuthorizationFilter(RoleEnum.Administrator)]
    public IActionResult GetById([FromRoute] Guid id)
    {
        var user = service.GetById(id);
        return this.Ok(user.ToDetailDto());
    }

    [HttpPost]
    [AuthorizationFilter(RoleEnum.Administrator)]
    public IActionResult Post([FromBody] CreateUserRequestDto request)
    {
        if (request.Role == RoleEnum.Visitor && !request.BirthDate.HasValue)
        {
            return this.BadRequest(new { Error = "BirthDate es requerido cuando el rol es Visitor." });
        }

        var user = request.ToDomain();
        var created = service.Create(user);
        if (created is User createdUser)
        {
            return this.CreatedAtAction(nameof(this.GetById), new { id = createdUser.Id }, createdUser.ToDto());
        }

        return this.CreatedAtAction(nameof(this.Get), null, created);
    }

    [HttpPut("{id}")]
    [AuthorizationFilter(RoleEnum.Administrator)]
    public IActionResult Put([FromRoute] Guid id, [FromBody] UpdateUserRequestDto request)
    {
        _ = service.Update(id, request.Name, request.Surname, request.Email, request.Password);

        return this.NoContent();
    }

    [HttpPost("{id}/roles")]
    [AuthorizationFilter(RoleEnum.Administrator)]
    public IActionResult AssignRole([FromRoute] Guid id, [FromBody] AssignRoleRequestDto request)
    {
        try
        {
            // Validate that BirthDate is provided when assigning Visitor role to a non-Visitor user
            if (request.Role == RoleEnum.Visitor && !request.BirthDate.HasValue)
            {
                var user = service.GetById(id);
                if (user is not Visitor)
                {
                    return this.BadRequest(new { Error = "BirthDate es requerido para asignar el rol Visitor a un usuario que no es Visitor." });
                }
            }

            service.AssignRole(id, request.Role, request.BirthDate, request.MembershipLevel);
            return this.NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return this.NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return this.BadRequest(new { Error = ex.Message });
        }
    }
}
