// <copyright file="UserMappingExtensions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;
using VirtualPark.Domain.Enums;
using VirtualPark.DTOs.Users.Requests;
using VirtualPark.DTOs.Users.Responses;

namespace VirtualPark.DTOs.Users;

public static class UserMappingExtensions
{
    public static UserDto ToDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email,
            Roles = user.Roles.ToList(),
        };
    }

    public static UserDetailDto ToDetailDto(this User user)
    {
        return new UserDetailDto
        {
            Id = user.Id,
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email,
            Roles = user.Roles.ToList(),
            FullName = $"{user.Name} {user.Surname}",
        };
    }

    public static UserListDto ToListDto(this User user)
    {
        return new UserListDto
        {
            Id = user.Id,
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email,
            Roles = user.Roles.ToList(),
        };
    }

    public static User ToDomain(this CreateUserRequestDto dto)
    {
        if (dto.Role == RoleEnum.Visitor)
        {
            if (!dto.BirthDate.HasValue)
            {
                throw new ArgumentException("BirthDate es requerido cuando el rol es Visitor.", nameof(dto.BirthDate));
            }

            return new Visitor(
                dto.Name,
                dto.Surname,
                dto.Email,
                dto.Password,
                dto.BirthDate.Value,
                MembershipLevel.Standard,
                Guid.NewGuid());
        }

        return new User(
            dto.Name,
            dto.Surname,
            dto.Email,
            dto.Password,
            dto.Role);
    }

    public static List<UserDto> ToDto(this IEnumerable<User> users)
    {
        return users.Select(u => u.ToDto()).ToList();
    }

    public static List<UserListDto> ToListDto(this IEnumerable<User> users)
    {
        return users.Select(u => u.ToListDto()).ToList();
    }
}
