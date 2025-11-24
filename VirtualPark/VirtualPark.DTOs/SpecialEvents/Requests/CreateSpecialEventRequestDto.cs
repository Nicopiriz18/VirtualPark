// <copyright file="CreateSpecialEventRequestDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.ComponentModel.DataAnnotations;

namespace VirtualPark.DTOs.SpecialEvents.Requests;

public class CreateSpecialEventRequestDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 200 caracteres")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "La fecha es requerida")]
    public required DateTime Date { get; set; }

    [Required(ErrorMessage = "La capacidad máxima es requerida")]
    [Range(1, 100000, ErrorMessage = "La capacidad debe estar entre 1 y 100000")]
    public required int MaxCapacity { get; set; }

    [Required(ErrorMessage = "El costo adicional es requerido")]
    [Range(0, double.MaxValue, ErrorMessage = "El costo adicional no puede ser negativo")]
    public required decimal AdditionalCost { get; set; }
}
