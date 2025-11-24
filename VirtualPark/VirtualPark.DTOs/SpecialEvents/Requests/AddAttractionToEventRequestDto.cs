// <copyright file="AddAttractionToEventRequestDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.ComponentModel.DataAnnotations;

namespace VirtualPark.DTOs.SpecialEvents.Requests;

public class AddAttractionToEventRequestDto
{
    [Required(ErrorMessage = "El ID de la atracción es requerido")]
    public required Guid AttractionId { get; set; }
}
