// <copyright file="StrategyPluginStore.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Http;

namespace VirtualPark.Application.Scoring;

public class StrategyPluginStore : IStrategyPluginStore
{
    private readonly string pluginsPath;

    public StrategyPluginStore(string pluginsPath)
    {
        this.pluginsPath = pluginsPath ?? throw new ArgumentNullException(nameof(pluginsPath));
    }

    public async Task StoreAsync(IFormFile pluginFile, CancellationToken cancellationToken = default)
    {
        if (pluginFile is null)
        {
            throw new ArgumentNullException(nameof(pluginFile));
        }

        if (pluginFile.Length <= 0)
        {
            throw new ArgumentException("El archivo está vacío.", nameof(pluginFile));
        }

        var fileName = Path.GetFileName(pluginFile.FileName ?? string.Empty);
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("El archivo no tiene un nombre válido.", nameof(pluginFile));
        }

        if (!fileName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Solo se permiten archivos con extensión .dll.", nameof(pluginFile));
        }

        Directory.CreateDirectory(this.pluginsPath);
        var destinationPath = Path.Combine(this.pluginsPath, fileName);

        await using var destinationStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None);
        await pluginFile.CopyToAsync(destinationStream, cancellationToken);
    }
}
