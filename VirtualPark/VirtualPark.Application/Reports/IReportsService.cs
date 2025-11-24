// <copyright file="IReportsService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VirtualPark.Application.Reports;
public interface IReportsService
{
    IEnumerable<AttractionUsageData> GetAttractionUsageReport(DateTime startDate, DateTime endDate);
}
