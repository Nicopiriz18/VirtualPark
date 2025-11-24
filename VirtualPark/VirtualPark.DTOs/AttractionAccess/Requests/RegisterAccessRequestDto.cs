// <copyright file="RegisterAccessRequestDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.ComponentModel.DataAnnotations;
using VirtualPark.Domain.Enums;

namespace VirtualPark.DTOs.AttractionAccess.Requests;

public class RegisterAccessRequestDto
{
    [Required(ErrorMessage = "El método de entrada es requerido")]
    public required EntryMethod EntryMethod { get; set; }

    public Guid? QrCode { get; set; }

    public Guid? VisitorId { get; set; }

    [Required(ErrorMessage = "La fecha de visita es requerida")]
    public required DateTime VisitDate { get; set; }

    [Required(ErrorMessage = "El tipo de ticket es requerido")]
    public required TicketType Type { get; set; }

    public Guid? SpecialEventId { get; set; }
}
