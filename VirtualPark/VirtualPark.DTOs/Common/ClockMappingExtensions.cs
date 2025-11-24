// <copyright file="ClockMappingExtensions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VirtualPark.DTOs.Common;

public static class ClockMappingExtensions
{
    public static ClockResponseDto ToClockResponseDto(
        this DateTime currentTime,
        bool isSystemTime = true,
        DateTime? lastModified = null,
        string? timeZone = null)
    {
        return new ClockResponseDto
        {
            CurrentDateTime = currentTime,
            FormattedDateTime = currentTime.ToString("yyyy-MM-dd HH:mm:ss"),
            IsSystemTime = isSystemTime,
            LastModified = lastModified,
            TimeZone = timeZone ?? TimeZoneInfo.Local.Id,
        };
    }
}
