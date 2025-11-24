// <copyright file="UploadStrategyPluginRequestDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace VirtualPark.DTOs.Scoring.Requests;

public class UploadStrategyPluginRequestDto
{
    [FromForm(Name = "plugin")]
    public IFormFile? Plugin { get; set; }
}
