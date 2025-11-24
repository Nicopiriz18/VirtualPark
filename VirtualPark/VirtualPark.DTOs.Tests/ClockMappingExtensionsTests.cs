// <copyright file="ClockMappingExtensionsTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.DTOs.Common;

namespace VirtualPark.DTOs.Tests;

[TestClass]
public class ClockMappingExtensionsTests
{
    [TestMethod]
    public void ToClockResponseDto_ShouldMapDateTimeToClockResponseDtoCorrectly()
    {
        // Arrange
        var currentTime = new DateTime(2025, 10, 6, 14, 30, 45);
        var lastModified = new DateTime(2025, 10, 5, 10, 0, 0);

        // Act
        var dto = currentTime.ToClockResponseDto(true, lastModified, "UTC");

        // Assert
        Assert.IsNotNull(dto);
        Assert.AreEqual(currentTime, dto.CurrentDateTime);
        Assert.AreEqual("2025-10-06 14:30:45", dto.FormattedDateTime);
        Assert.IsTrue(dto.IsSystemTime);
        Assert.AreEqual(lastModified, dto.LastModified);
        Assert.AreEqual("UTC", dto.TimeZone);
    }

    [TestMethod]
    public void ToClockResponseDto_ShouldUseDefaultsWhenNotProvided()
    {
        // Arrange
        var currentTime = new DateTime(2025, 11, 15, 9, 0, 0);

        // Act
        var dto = currentTime.ToClockResponseDto();

        // Assert
        Assert.IsNotNull(dto);
        Assert.AreEqual(currentTime, dto.CurrentDateTime);
        Assert.AreEqual("2025-11-15 09:00:00", dto.FormattedDateTime);
        Assert.IsTrue(dto.IsSystemTime);
        Assert.IsNull(dto.LastModified);
        Assert.AreEqual(TimeZoneInfo.Local.Id, dto.TimeZone);
    }

    [TestMethod]
    public void ToClockResponseDto_ShouldFormatDateTimeCorrectly()
    {
        // Arrange
        var currentTime = new DateTime(2025, 1, 1, 0, 0, 0);

        // Act
        var dto = currentTime.ToClockResponseDto();

        // Assert
        Assert.IsNotNull(dto);
        Assert.AreEqual("2025-01-01 00:00:00", dto.FormattedDateTime);
    }

    [TestMethod]
    public void ToClockResponseDto_ShouldHandleCustomTimeZone()
    {
        // Arrange
        var currentTime = new DateTime(2025, 7, 4, 12, 0, 0);
        var customTimeZone = "America/New_York";

        // Act
        var dto = currentTime.ToClockResponseDto(false, null, customTimeZone);

        // Assert
        Assert.IsNotNull(dto);
        Assert.IsFalse(dto.IsSystemTime);
        Assert.AreEqual(customTimeZone, dto.TimeZone);
    }

    [TestMethod]
    public void ToClockResponseDto_ShouldHandleNullLastModified()
    {
        // Arrange
        var currentTime = new DateTime(2025, 12, 31, 23, 59, 59);

        // Act
        var dto = currentTime.ToClockResponseDto(true, null);

        // Assert
        Assert.IsNotNull(dto);
        Assert.IsNull(dto.LastModified);
    }

    [TestMethod]
    public void ToClockResponseDto_ShouldFormatMidnightCorrectly()
    {
        // Arrange
        var currentTime = new DateTime(2025, 6, 15, 0, 0, 0);

        // Act
        var dto = currentTime.ToClockResponseDto();

        // Assert
        Assert.IsNotNull(dto);
        Assert.AreEqual("2025-06-15 00:00:00", dto.FormattedDateTime);
    }

    [TestMethod]
    public void ToClockResponseDto_ShouldFormatTimeWithSingleDigitsCorrectly()
    {
        // Arrange
        var currentTime = new DateTime(2025, 3, 5, 7, 8, 9);

        // Act
        var dto = currentTime.ToClockResponseDto();

        // Assert
        Assert.IsNotNull(dto);
        Assert.AreEqual("2025-03-05 07:08:09", dto.FormattedDateTime);
    }
}
