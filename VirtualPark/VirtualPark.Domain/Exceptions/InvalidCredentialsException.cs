// <copyright file="InvalidCredentialsException.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VirtualPark.Domain.Exceptions;

public class InvalidCredentialsException(string message) : Exception(message)
{
}
