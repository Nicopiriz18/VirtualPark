// <copyright file="SetActiveScoringStrategyRequestDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.ComponentModel.DataAnnotations;

namespace VirtualPark.DTOs.Scoring.Requests;

public class SetActiveScoringStrategyRequestDto
{
    [Required(ErrorMessage = "El nombre de la estrategia es requerido")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre de la estrategia debe tener entre 3 y 100 caracteres")]
    public required string StrategyName { get; set; }
}
