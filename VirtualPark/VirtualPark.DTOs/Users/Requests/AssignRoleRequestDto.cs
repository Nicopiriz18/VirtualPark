// <copyright file="AssignRoleRequestDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.ComponentModel.DataAnnotations;
using VirtualPark.Domain.Enums;

namespace VirtualPark.DTOs.Users.Requests;

public class AssignRoleRequestDto
{
    [Required(ErrorMessage = "El rol es requerido")]
    public required RoleEnum Role { get; set; }

    public DateTime? BirthDate { get; set; }

    public MembershipLevel? MembershipLevel { get; set; }
}
