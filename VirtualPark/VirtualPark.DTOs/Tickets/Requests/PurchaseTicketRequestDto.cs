// <copyright file="PurchaseTicketRequestDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.ComponentModel.DataAnnotations;
using VirtualPark.Domain.Enums;

namespace VirtualPark.DTOs.Tickets.Requests;

public class PurchaseTicketRequestDto
{
    [Required(ErrorMessage = "El ID del visitante es requerido")]
    public required Guid VisitorId { get; set; }

    [Required(ErrorMessage = "La fecha de visita es requerida")]
    public required DateTime VisitDate { get; set; }

    [Required(ErrorMessage = "El tipo de ticket es requerido")]
    public required TicketType Type { get; set; }

    public Guid? SpecialEventId { get; set; }
}
