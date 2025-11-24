// <copyright file="DuplicateEmailException.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VirtualPark.Domain.Exceptions;

public class DuplicateEmailException(string message) : Exception(message)
{
}
