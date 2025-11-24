// <copyright file="CreateAttractionRequestDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.ComponentModel.DataAnnotations;

namespace VirtualPark.DTOs.Attractions.Requests;

public class CreateAttractionRequestDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 200 caracteres")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "La descripción es requerida")]
    [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
    public required string Description { get; set; }

    [Required(ErrorMessage = "El tipo es requerido")]
    [StringLength(100, ErrorMessage = "El tipo no puede exceder 100 caracteres")]
    public required string Type { get; set; }

    [Required(ErrorMessage = "La edad mínima es requerida")]
    [Range(0, 120, ErrorMessage = "La edad mínima debe estar entre 0 y 120 años")]
    public required int MinAge { get; set; }

    [Required(ErrorMessage = "La capacidad es requerida")]
    [Range(1, 10000, ErrorMessage = "La capacidad debe estar entre 1 y 10000")]
    public required int Capacity { get; set; }
}
