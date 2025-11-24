// <copyright file="ClockResponseDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VirtualPark.DTOs.Common;

public class ClockResponseDto
{
    public required DateTime CurrentDateTime { get; set; }

    public required string FormattedDateTime { get; set; }

    public required bool IsSystemTime { get; set; }

    public DateTime? LastModified { get; set; }

    public string? TimeZone { get; set; }
}
