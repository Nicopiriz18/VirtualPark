// <copyright file="AttractionsController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using VirtualPark.Application.Attractions;
using VirtualPark.Domain;
using VirtualPark.Domain.Enums;
using VirtualPark.DTOs.Attractions;
using VirtualPark.DTOs.Attractions.Requests;
using VirtualPark.WebApi.Filters;

namespace VirtualPark.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[AuthorizationFilter]
public class AttractionsController(IAttractionService service) : ControllerBase
{
    private readonly IAttractionService service = service;

    [HttpGet]
    public IActionResult Get()
    {
        var attractions = this.service.GetAll();
        return this.Ok(attractions.ToListDto());
    }

    [HttpGet("{id}")]
    public IActionResult Get(Guid id)
    {
        var attraction = this.service.GetById(id);
        if (attraction is null)
        {
            return this.NotFound();
        }

        return this.Ok(attraction.ToDetailDto());
    }

    [HttpPost]
    [AuthorizationFilter(RoleEnum.Administrator)]
    public IActionResult Post([FromBody] CreateAttractionRequestDto request)
    {
        var attraction = request.ToDomain();
        var created = this.service.Create(attraction);
        return this.CreatedAtAction(nameof(this.Get), new { id = created.Id }, created.ToDto());
    }

    [HttpPut("{id}")]
    [AuthorizationFilter(RoleEnum.Administrator)]
    public IActionResult Put(Guid id, [FromBody] UpdateAttractionRequestDto request)
    {
        var attraction = new Attraction(request.Name, request.Description, request.Type, request.MinAge, request.Capacity);
        this.service.Update(id, attraction);
        return this.NoContent();
    }

    [HttpDelete("{id}")]
    [AuthorizationFilter(RoleEnum.Administrator)]
    public IActionResult Delete(Guid id)
    {
        this.service.Delete(id);
        return this.NoContent();
    }
}
