// <copyright file="AttractionUsageData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;

namespace VirtualPark.Application.Reports;

public class AttractionUsageData
{
    public required Attraction Attraction { get; set; }

    public int VisitCount { get; set; }
}
