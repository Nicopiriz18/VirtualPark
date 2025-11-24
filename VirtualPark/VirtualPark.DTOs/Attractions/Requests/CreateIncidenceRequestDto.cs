// <copyright file="CreateIncidenceRequestDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.ComponentModel.DataAnnotations;

namespace VirtualPark.DTOs.Attractions.Requests;

public class CreateIncidenceRequestDto
{
    [Required(ErrorMessage = "El título es requerido")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "El título debe tener entre 3 y 200 caracteres")]
    public required string Title { get; set; }

    [Required(ErrorMessage = "La descripción es requerida")]
    [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
    public required string Description { get; set; }

    [Required(ErrorMessage = "El estado es requerido")]
    public required bool Status { get; set; }

    [Required(ErrorMessage = "La fecha es requerida")]
    public required DateTime Date { get; set; }
}
