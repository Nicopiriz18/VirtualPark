// <copyright file="ScoringStrategyLoaderTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Reflection;
using VirtualPark.Application.Scoring;
using VirtualPark.Application.Scoring.Strategies;
using VirtualPark.Domain;

namespace VirtualPark.Application.Tests;

[TestClass]
public class ScoringStrategyLoaderTests
{
    private string testPluginsPath = null!;
    private ScoringStrategyLoader loader = null!;

    [TestInitialize]
    public void Setup()
    {
        // Create a temporary test plugins directory
        this.testPluginsPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(this.testPluginsPath);

        this.loader = new ScoringStrategyLoader(this.testPluginsPath);
    }

    [TestCleanup]
    public void Cleanup()
    {
        // Clean up test plugins directory
        if (Directory.Exists(this.testPluginsPath))
        {
            try
            {
                Directory.Delete(this.testPluginsPath, true);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }

    // Test helper classes - these will be compiled into the test assembly
    private class TestStrategy1 : IScoringStrategy
    {
        public string Name => "TestStrategy1";

        public int CalculatePoints(Attraction attraction, Visitor visitor, IEnumerable<AttractionAccess> todayAccesses, Domain.SpecialEvent? activeEvent)
        {
            return 100;
        }
    }

    private class TestStrategy2 : IScoringStrategy
    {
        public string Name => "TestStrategy2";

        public int CalculatePoints(Attraction attraction, Visitor visitor, IEnumerable<AttractionAccess> todayAccesses, Domain.SpecialEvent? activeEvent)
        {
            return 200;
        }
    }

    private class DuplicateNameStrategy : IScoringStrategy
    {
        public string Name => "ScoreByAttractionType"; // Conflicts with built-in

        public int CalculatePoints(Attraction attraction, Visitor visitor, IEnumerable<AttractionAccess> todayAccesses, Domain.SpecialEvent? activeEvent)
        {
            return 999;
        }
    }

    private class StrategyWithoutParameterlessConstructor : IScoringStrategy
    {
        public string Name => "NoDefaultConstructor";

        // This constructor requires a parameter, so the loader shouldn't be able to instantiate it
        public StrategyWithoutParameterlessConstructor(int requiredParam)
        {
        }

        public int CalculatePoints(Attraction attraction, Visitor visitor, IEnumerable<AttractionAccess> todayAccesses, Domain.SpecialEvent? activeEvent)
        {
            return 123;
        }
    }

    private abstract class AbstractStrategy : IScoringStrategy
    {
        public abstract string Name { get; }

        public abstract int CalculatePoints(Attraction attraction, Visitor visitor, IEnumerable<AttractionAccess> todayAccesses, Domain.SpecialEvent? activeEvent);
    }

    private class StrategyWithFailingConstructor : IScoringStrategy
    {
        public string Name => "FailingConstructor";

        public StrategyWithFailingConstructor()
        {
            throw new InvalidOperationException("Constructor intentionally fails");
        }

        public int CalculatePoints(Attraction attraction, Visitor visitor, IEnumerable<AttractionAccess> todayAccesses, Domain.SpecialEvent? activeEvent)
        {
            return 0;
        }
    }

    [TestMethod]
    public void GetStrategies_WithEmptyPluginsFolder_ReturnsBuiltInStrategies()
    {
        var strategies = this.loader.GetStrategies();

        Assert.AreEqual(3, strategies.Count);
        Assert.IsTrue(strategies.ContainsKey("ScoreByAttractionType"));
        Assert.IsTrue(strategies.ContainsKey("ScoreByCombo"));
        Assert.IsTrue(strategies.ContainsKey("ScoreByEventMultiplier"));
        Assert.IsInstanceOfType(strategies["ScoreByAttractionType"], typeof(ScoreByAttractionTypeStrategy));
        Assert.IsInstanceOfType(strategies["ScoreByCombo"], typeof(ScoreByComboStrategy));
        Assert.IsInstanceOfType(strategies["ScoreByEventMultiplier"], typeof(ScoreByEventMultiplierStrategy));
    }

    [TestMethod]
    public void GetStrategies_WithMissingPluginsFolder_ReturnsBuiltInStrategies()
    {
        var nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var loader = new ScoringStrategyLoader(nonExistentPath);

        var strategies = loader.GetStrategies();

        Assert.AreEqual(3, strategies.Count);
        Assert.IsTrue(strategies.ContainsKey("ScoreByAttractionType"));
    }

    [TestMethod]
    public void GetStrategies_WithInvalidDllFile_ReturnsBuiltInStrategies()
    {
        // Create an invalid DLL file (just a text file)
        var invalidDllPath = Path.Combine(this.testPluginsPath, "InvalidPlugin.dll");
        File.WriteAllText(invalidDllPath, "This is not a valid DLL");

        var strategies = this.loader.GetStrategies();

        // Should still return built-in strategies, ignoring invalid DLL
        Assert.AreEqual(3, strategies.Count);
    }

    [TestMethod]
    public void GetStrategies_ScansFolderOnEachCall()
    {
        var strategies1 = this.loader.GetStrategies();
        Assert.AreEqual(3, strategies1.Count);

        var strategies2 = this.loader.GetStrategies();

        // Should be a new dictionary instance (or same, but fresh scan)
        Assert.AreEqual(3, strategies2.Count);
    }

    [TestMethod]
    public void GetStrategies_CalledMultipleTimes_ScansFolderEachTime()
    {
        // Verify that GetStrategies scans the folder on each call (no caching)
        var strategies1 = this.loader.GetStrategies();
        Assert.AreEqual(3, strategies1.Count);

        // Call again - should scan fresh
        var strategies2 = this.loader.GetStrategies();
        Assert.AreEqual(3, strategies2.Count);

        // Both should contain the same built-in strategies
        Assert.IsTrue(strategies1.ContainsKey("ScoreByAttractionType"));
        Assert.IsTrue(strategies2.ContainsKey("ScoreByAttractionType"));
    }

    [TestMethod]
    public void GetStrategies_WithValidPluginDll_LoadsPluginStrategy()
    {
        // Copy the current test assembly to the plugins folder
        // This assembly contains TestStrategy1 and TestStrategy2
        var currentAssembly = Assembly.GetExecutingAssembly();
        var testDllPath = Path.Combine(this.testPluginsPath, "TestPlugin.dll");
        File.Copy(currentAssembly.Location, testDllPath);

        var strategies = this.loader.GetStrategies();

        // Should have built-in strategies + our test strategies
        Assert.IsTrue(strategies.Count >= 5); // At least 3 built-in + 2 test strategies
        Assert.IsTrue(strategies.ContainsKey("ScoreByAttractionType"));
        Assert.IsTrue(strategies.ContainsKey("ScoreByCombo"));
        Assert.IsTrue(strategies.ContainsKey("ScoreByEventMultiplier"));
        Assert.IsTrue(strategies.ContainsKey("TestStrategy1"));
        Assert.IsTrue(strategies.ContainsKey("TestStrategy2"));
    }

    [TestMethod]
    public void GetStrategies_WithDuplicateStrategyName_LogsErrorAndContinues()
    {
        // Copy the test assembly which contains DuplicateNameStrategy
        // DuplicateNameStrategy has name "ScoreByAttractionType" which conflicts with built-in
        var currentAssembly = Assembly.GetExecutingAssembly();
        var testDllPath = Path.Combine(this.testPluginsPath, "DuplicatePlugin.dll");
        File.Copy(currentAssembly.Location, testDllPath);

        // Should NOT throw - the duplicate exception is caught and logged, then processing continues
        var strategies = this.loader.GetStrategies();

        // Should have built-in ScoreByAttractionType (not replaced by duplicate from plugin)
        Assert.IsTrue(strategies.ContainsKey("ScoreByAttractionType"));
        Assert.IsInstanceOfType(strategies["ScoreByAttractionType"], typeof(ScoreByAttractionTypeStrategy));

        // Should also have TestStrategy1 and TestStrategy2 (non-duplicate strategies from same assembly)
        Assert.IsTrue(strategies.ContainsKey("TestStrategy1"));
        Assert.IsTrue(strategies.ContainsKey("TestStrategy2"));
    }

    [TestMethod]
    public void GetStrategies_WithStrategyWithoutParameterlessConstructor_SkipsStrategy()
    {
        // The test assembly contains StrategyWithoutParameterlessConstructor
        // which requires a parameter in its constructor
        var currentAssembly = Assembly.GetExecutingAssembly();
        var testDllPath = Path.Combine(this.testPluginsPath, "NoConstructorPlugin.dll");
        File.Copy(currentAssembly.Location, testDllPath);

        var strategies = this.loader.GetStrategies();

        // Should load TestStrategy1 and TestStrategy2 but skip StrategyWithoutParameterlessConstructor
        Assert.IsTrue(strategies.ContainsKey("TestStrategy1"));
        Assert.IsTrue(strategies.ContainsKey("TestStrategy2"));
        Assert.IsFalse(strategies.ContainsKey("NoDefaultConstructor"));
    }

    [TestMethod]
    public void GetStrategies_WithAbstractStrategy_SkipsAbstractClass()
    {
        // The test assembly contains AbstractStrategy which is abstract
        var currentAssembly = Assembly.GetExecutingAssembly();
        var testDllPath = Path.Combine(this.testPluginsPath, "AbstractPlugin.dll");
        File.Copy(currentAssembly.Location, testDllPath);

        var strategies = this.loader.GetStrategies();

        // Should load concrete strategies but skip abstract ones
        Assert.IsTrue(strategies.ContainsKey("TestStrategy1"));
        Assert.IsTrue(strategies.ContainsKey("TestStrategy2"));

        // AbstractStrategy doesn't have a Name value to check, but it shouldn't be in the dictionary
    }

    [TestMethod]
    public void GetStrategies_WithMultipleDllFiles_LoadsAllValidStrategies()
    {
        // Create multiple plugin DLLs
        var currentAssembly = Assembly.GetExecutingAssembly();
        var plugin1Path = Path.Combine(this.testPluginsPath, "Plugin1.dll");
        var plugin2Path = Path.Combine(this.testPluginsPath, "Plugin2.dll");

        File.Copy(currentAssembly.Location, plugin1Path);
        File.Copy(currentAssembly.Location, plugin2Path);

        var strategies = this.loader.GetStrategies();

        // Should load strategies from all DLLs (though they're the same assembly, so same strategies)
        Assert.IsTrue(strategies.Count >= 5); // 3 built-in + at least 2 test strategies
        Assert.IsTrue(strategies.ContainsKey("TestStrategy1"));
        Assert.IsTrue(strategies.ContainsKey("TestStrategy2"));
    }

    [TestMethod]
    public void GetStrategies_WithMixOfValidAndInvalidDlls_LoadsValidOnesAndSkipsInvalid()
    {
        // Create a valid DLL
        var currentAssembly = Assembly.GetExecutingAssembly();
        var validDllPath = Path.Combine(this.testPluginsPath, "ValidPlugin.dll");
        File.Copy(currentAssembly.Location, validDllPath);

        // Create an invalid DLL
        var invalidDllPath = Path.Combine(this.testPluginsPath, "InvalidPlugin.dll");
        File.WriteAllText(invalidDllPath, "This is not a valid DLL file");

        var strategies = this.loader.GetStrategies();

        // Should load from valid DLL and skip invalid one
        Assert.IsTrue(strategies.Count >= 5);
        Assert.IsTrue(strategies.ContainsKey("TestStrategy1"));
        Assert.IsTrue(strategies.ContainsKey("TestStrategy2"));
    }

    [TestMethod]
    public void GetStrategies_VerifiesStrategyCanBeUsed()
    {
        // Copy the test assembly to plugins
        var currentAssembly = Assembly.GetExecutingAssembly();
        var testDllPath = Path.Combine(this.testPluginsPath, "TestPlugin.dll");
        File.Copy(currentAssembly.Location, testDllPath);

        var strategies = this.loader.GetStrategies();

        // Verify we can actually use the loaded strategy
        Assert.IsTrue(strategies.ContainsKey("TestStrategy1"));
        var strategy = strategies["TestStrategy1"];
        Assert.IsNotNull(strategy);
        Assert.AreEqual("TestStrategy1", strategy.Name);

        // Verify the strategy can calculate points
        var points = strategy.CalculatePoints(null!, null!, Enumerable.Empty<AttractionAccess>(), null);
        Assert.AreEqual(100, points);
    }

    [TestMethod]
    public void GetStrategies_WithStrategyThatFailsToInstantiate_LogsErrorAndContinuesWithOthers()
    {
        // Copy the test assembly which contains StrategyWithFailingConstructor
        // This strategy has a parameterless constructor but it throws an exception
        var currentAssembly = Assembly.GetExecutingAssembly();
        var testDllPath = Path.Combine(this.testPluginsPath, "FailingPlugin.dll");
        File.Copy(currentAssembly.Location, testDllPath);

        // Should NOT throw - the instantiation exception is caught and logged
        var strategies = this.loader.GetStrategies();

        // Should have built-in strategies + TestStrategy1 and TestStrategy2
        // but NOT FailingConstructor (because its constructor throws)
        Assert.IsTrue(strategies.Count >= 5); // 3 built-in + 2 test strategies
        Assert.IsTrue(strategies.ContainsKey("TestStrategy1"));
        Assert.IsTrue(strategies.ContainsKey("TestStrategy2"));
        Assert.IsFalse(strategies.ContainsKey("FailingConstructor"));
    }

    [TestMethod]
    public void GetStrategies_WithMultiplePluginsHavingStrategies_LoadsAllUniqueStrategies()
    {
        // This test covers the foreach loop in LoadPlugins processing multiple DLL files
        var currentAssembly = Assembly.GetExecutingAssembly();

        // Create multiple different plugin DLLs
        var plugin1Path = Path.Combine(this.testPluginsPath, "Plugin1.dll");
        var plugin2Path = Path.Combine(this.testPluginsPath, "Plugin2.dll");
        var plugin3Path = Path.Combine(this.testPluginsPath, "Plugin3.dll");

        File.Copy(currentAssembly.Location, plugin1Path);
        File.Copy(currentAssembly.Location, plugin2Path);
        File.Copy(currentAssembly.Location, plugin3Path);

        var strategies = this.loader.GetStrategies();

        // Should successfully iterate through all DLLs in the foreach loop
        // and load strategies from each (though they're the same, the loop runs 3 times)
        Assert.IsTrue(strategies.Count >= 5);
        Assert.IsTrue(strategies.ContainsKey("TestStrategy1"));
        Assert.IsTrue(strategies.ContainsKey("TestStrategy2"));
    }
}
