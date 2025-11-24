// <copyright file="IClockService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VirtualPark.Application.Clock;

public interface IClockService
{
    DateTime GetNow();

    void SetNow(DateTime value);
}
