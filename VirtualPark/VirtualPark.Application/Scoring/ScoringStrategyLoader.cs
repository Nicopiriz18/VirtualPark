// <copyright file="ScoringStrategyLoader.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Reflection;
using VirtualPark.Application.Scoring.Strategies;

namespace VirtualPark.Application.Scoring;

public class ScoringStrategyLoader : IScoringStrategyLoader
{
    private readonly string pluginsPath;

    public ScoringStrategyLoader(string pluginsPath)
    {
        this.pluginsPath = pluginsPath;
    }

    public Dictionary<string, IScoringStrategy> GetStrategies()
    {
        // Start with built-in strategies
        var strategies = new Dictionary<string, IScoringStrategy>
        {
            ["ScoreByAttractionType"] = new ScoreByAttractionTypeStrategy(),
            ["ScoreByCombo"] = new ScoreByComboStrategy(),
            ["ScoreByEventMultiplier"] = new ScoreByEventMultiplierStrategy(),
        };

        // Load plugins from folder
        this.LoadPlugins(strategies);

        return strategies;
    }

    private void LoadPlugins(Dictionary<string, IScoringStrategy> strategies)
    {
        Console.WriteLine($"Loading plugins from {this.pluginsPath}");
        if (!Directory.Exists(this.pluginsPath))
        {
            Console.WriteLine("No plugins folder, returning only built-in strategies");
            return;
        }

        var dllFiles = Directory.GetFiles(this.pluginsPath, "*.dll", SearchOption.TopDirectoryOnly);
        Console.WriteLine($"Found {dllFiles.Length} plugin DLLs in {this.pluginsPath}");
        foreach (var dllPath in dllFiles)
        {
            try
            {
                this.LoadStrategiesFromAssembly(dllPath, strategies);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to load plugin from {dllPath}: {ex.Message}");
            }
        }
    }

    private void LoadStrategiesFromAssembly(string assemblyPath, Dictionary<string, IScoringStrategy> strategies)
    {
        Assembly assembly;
        try
        {
            assembly = Assembly.LoadFrom(assemblyPath);
        }
        catch
        {
            // Invalid DLL file, skip it
            return;
        }

        var strategyTypes = assembly.GetTypes()
            .Where(t => typeof(IScoringStrategy).IsAssignableFrom(t)
                     && !t.IsInterface
                     && !t.IsAbstract
                     && t.GetConstructor(Type.EmptyTypes) != null)
            .ToList();

        foreach (var strategyType in strategyTypes)
        {
            try
            {
                var strategy = (IScoringStrategy)Activator.CreateInstance(strategyType)!;
                var strategyName = strategy.Name;

                if (strategies.ContainsKey(strategyName))
                {
                    throw new InvalidOperationException(
                        $"Duplicate strategy name '{strategyName}' found. " +
                        $"A strategy with this name already exists (either built-in or from another plugin). " +
                        $"Conflict detected in assembly: {assemblyPath}");
                }

                strategies[strategyName] = strategy;
            }
            catch (Exception ex)
            {
                // Log error and continue with other strategies
                Console.WriteLine($"Error: Failed to instantiate strategy from type {strategyType.Name} in {assemblyPath}: {ex.Message}");
            }
        }
    }
}
