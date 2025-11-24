// <copyright file="IAttractionValidationService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VirtualPark.Application.Attractions;

public interface IAttractionValidationService
{
    bool HasValidAge(Guid attractionId, int visitorAge);

    bool IsAttractionAvailable(Guid attractionId);
}
