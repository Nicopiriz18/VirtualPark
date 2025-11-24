// <copyright file="AttractionAccessService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Application;
using VirtualPark.Application.Attractions;
using VirtualPark.Application.Clock;
using VirtualPark.Application.Scoring;
using VirtualPark.Application.Tickets;
using VirtualPark.Domain;
using VirtualPark.Domain.Enums;
using VirtualPark.Domain.Interfaces.Repositories;

public class AttractionAccessService(
    IAttractionAccessRepository accessRepository,
    IAttractionRepository repository,
    IAttractionValidationService validationService,
    ITicketLookupService ticketLookupService,
    IClockService clockService,
    IScoringService scoringService) : IAttractionAccessService
{
    private readonly IAttractionValidationService validationService = validationService;
    private readonly ITicketLookupService ticketLookupService = ticketLookupService;

    public void RegisterAccess(Guid attractionId, Ticket ticket, EntryMethod entryMethod)
    {
        var attraction = repository.GetById(attractionId) ?? throw new KeyNotFoundException();

        if (ticket.VisitDate.Date != clockService.GetNow().Date)
        {
            throw new InvalidOperationException("Ticket no válido para hoy");
        }

        if (ticket.Type == TicketType.SpecialEvent && ticket.SpecialEventId == null)
        {
            throw new InvalidOperationException("Evento especial inválido");
        }

        if (!this.validationService.IsAttractionAvailable(attractionId))
        {
            throw new InvalidOperationException("La atracción no está disponible actualmente");
        }

        if (ticket.Visitor is not null)
        {
            var age = CalculateAge(ticket.Visitor.BirthDate, clockService.GetNow().Date);
            if (!this.validationService.HasValidAge(attractionId, age))
            {
                throw new InvalidOperationException("Visitante no cumple con la edad mínima");
            }
        }

        this.ValidateNoDoubleEntry(attractionId, ticket.VisitorId);

        var openAccesses = accessRepository.CountOpenAccesses(attractionId);
        if (openAccesses >= attraction.Capacity)
        {
            throw new InvalidOperationException("Aforo completo: no se permite el ingreso");
        }

        var now = clockService.GetNow();
        var access = new AttractionAccess(attractionId, ticket.VisitorId, ticket.Id, now, null, entryMethod);

        accessRepository.Add(access);

        scoringService.AwardPoints(ticket.VisitorId, access.Id, attractionId, now);
    }

    public bool HasValidAge(Guid attractionId, int visitorAge)
    {
        return this.validationService.HasValidAge(attractionId, visitorAge);
    }

    public bool IsAttractionAvailable(Guid attractionId)
    {
        return this.validationService.IsAttractionAvailable(attractionId);
    }

    private void ValidateNoDoubleEntry(Guid attractionId, Guid visitorId)
    {
        if (visitorId == Guid.Empty)
        {
            return; // si no lo estás manejando por visitor
        }

        var openAccess = accessRepository.GetOpenAccess(attractionId, visitorId);
        if (openAccess != null)
        {
            throw new InvalidOperationException("El visitante ya se encuentra dentro de la atracción.");
        }
    }

    private static int CalculateAge(DateTime birthDate, DateTime today)
    {
        var age = today.Year - birthDate
            .Year;
        if (birthDate.Date > today.AddYears(-age))
        {
            age--;
        }

        return age;
    }

    public void RegisterExit(Guid attractionId, Guid visitorId, DateTime now)
    {
        var open = accessRepository.GetOpenAccess(attractionId, visitorId)
                   ?? throw new InvalidOperationException("El visitante no se encuentra dentro de la atracción.");

        open.MarkExit(now);
        accessRepository.Update(open);
    }

    public (int AforoActual, int CapacidadRestante) GetAforo(Guid attractionId)
    {
        var attraction = repository.GetById(attractionId)
                         ?? throw new KeyNotFoundException($"Atracción {attractionId} no encontrada");

        var aforoActual = accessRepository.CountOpenAccesses(attractionId);
        var capacidadRestante = Math.Max(0, attraction.Capacity - aforoActual);

        return (aforoActual, capacidadRestante);
    }

    public void RegisterAccessByQrCode(Guid attractionId, Guid qrCode, EntryMethod entryMethod)
    {
        var ticket = this.ticketLookupService.GetTicketByQrCode(qrCode);
        this.RegisterAccess(attractionId, ticket, entryMethod);
    }

    public void RegisterAccessByVisitorId(Guid attractionId, Guid visitorId, DateTime visitDate, EntryMethod entryMethod)
    {
        var ticket = this.ticketLookupService.GetTicketByVisitorAndDate(visitorId, visitDate);
        this.RegisterAccess(attractionId, ticket, entryMethod);
    }
}
