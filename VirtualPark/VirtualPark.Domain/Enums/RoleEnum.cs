// <copyright file="RoleEnum.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VirtualPark.Domain.Enums;

public enum RoleEnum
{
    /// <summary>
    /// Administrator role with full system access and management capabilities.
    /// </summary>
    Administrator,

    /// <summary>
    /// Operator role with limited system access for operational tasks.
    /// </summary>
    Operator,

    /// <summary>
    /// Visitor role for park visitors with basic access.
    /// </summary>
    Visitor,
}
