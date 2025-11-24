// <copyright file="ClockRequestDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.ComponentModel.DataAnnotations;

namespace VirtualPark.DTOs.Common;

public class ClockRequestDto
{
    [Required(ErrorMessage = "El valor de fecha/hora es requerido")]
    [RegularExpression(
        @"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}(:\d{2})?$",
        ErrorMessage = "Formato de fecha/hora inválido. Use el formato YYYY-MM-DDTHH:MM o YYYY-MM-DDTHH:MM:SS")]
    public required string Value { get; set; }
}
