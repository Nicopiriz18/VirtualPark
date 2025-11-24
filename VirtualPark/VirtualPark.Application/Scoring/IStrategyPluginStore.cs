// <copyright file="IStrategyPluginStore.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Http;

namespace VirtualPark.Application.Scoring;

public interface IStrategyPluginStore
{
    /// <summary>
    /// Stores the provided strategy DLL inside the configured plugins folder.
    /// </summary>
    /// <param name="pluginFile">Uploaded DLL.</param>
    /// <param name="cancellationToken">Token to observe while copying the file.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task StoreAsync(IFormFile pluginFile, CancellationToken cancellationToken = default);
}
