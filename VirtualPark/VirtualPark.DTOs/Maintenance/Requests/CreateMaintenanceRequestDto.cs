// <copyright file="CreateMaintenanceRequestDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.ComponentModel.DataAnnotations;

namespace VirtualPark.DTOs.Maintenance.Requests;

public class CreateMaintenanceRequestDto
{
    [Required(ErrorMessage = "El ID de la atracción es requerido")]
    public required Guid AttractionId { get; set; }

    [Required(ErrorMessage = "La fecha programada es requerida")]
    public required DateTime ScheduledDate { get; set; }

    [Required(ErrorMessage = "La hora de inicio es requerida")]
    public required TimeSpan StartTime { get; set; }

    [Required(ErrorMessage = "La duración estimada es requerida")]
    public required TimeSpan EstimatedDuration { get; set; }

    [Required(ErrorMessage = "La descripción es requerida")]
    [StringLength(1000, MinimumLength = 3, ErrorMessage = "La descripción debe tener entre 3 y 1000 caracteres")]
    public required string Description { get; set; }
}
