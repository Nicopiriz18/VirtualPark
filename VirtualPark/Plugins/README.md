# Scoring Strategy Plugins

This directory contains plugin DLLs that extend the VirtualPark scoring system with custom scoring strategies.

## Overview

The VirtualPark system supports dynamic loading of scoring strategies through a plugin mechanism. You can create custom scoring strategies without modifying the core application code.

## Creating a Plugin

### 1. Create a Class Library Project

Create a new .NET 8.0 Class Library project:

```bash
dotnet new classlib -n MyScoringPlugin
cd MyScoringPlugin
```

### 2. Add Required References

Add a reference to the `VirtualPark.Application` project to access the `IScoringStrategy` interface:

```xml
<ItemGroup>
  <ProjectReference Include="path/to/VirtualPark.Application/VirtualPark.Application.csproj" />
</ItemGroup>
```

Alternatively, if you're creating a standalone plugin, you'll need to reference the `VirtualPark.Application.dll` assembly.

### 3. Implement IScoringStrategy

Create a class that implements `IScoringStrategy`:

```csharp
using Domain;
using VirtualPark.Application.Scoring;

namespace MyScoringPlugin;

public class MyCustomStrategy : IScoringStrategy
{
    public string Name => "MyCustomStrategy";

    public int CalculatePoints(
        Attraction attraction,
        Visitor visitor,
        IEnumerable<AttractionAccess> todayAccesses,
        Domain.SpecialEvent? activeEvent)
    {
        // Your custom scoring logic here
        return 100; // Example: return fixed points
    }
}
```

### 4. Build the Plugin

Build your project to generate a DLL:

```bash
dotnet build -c Release
```

### 5. Deploy the Plugin

Copy the generated DLL file (e.g., `MyScoringPlugin.dll`) to the `Plugins/` folder in your VirtualPark application directory.

**Important:** The plugin DLL must be placed in the `Plugins/` folder relative to where the application is running (typically `bin/Debug/net8.0/Plugins/` or `bin/Release/net8.0/Plugins/`).

## Interface Contract

### IScoringStrategy Interface

```csharp
public interface IScoringStrategy
{
    string Name { get; }
    
    int CalculatePoints(
        Attraction attraction,
        Visitor visitor,
        IEnumerable<AttractionAccess> todayAccesses,
        Domain.SpecialEvent? activeEvent);
}
```

### Requirements

1. **Name Property**: Must return a unique string identifier for your strategy. This name must not conflict with:
   - Built-in strategies: `ScoreByAttractionType`, `ScoreByCombo`, `ScoreByEventMultiplier`
   - Other plugin strategies

2. **CalculatePoints Method**: Must implement your scoring logic and return an integer representing the points to award.

### Parameters

- **attraction**: The attraction being visited
- **visitor**: The visitor who is accessing the attraction
- **todayAccesses**: All attraction accesses by this visitor on the current day
- **activeEvent**: The active special event for today (if any), or `null`

### Return Value

The method must return an integer representing the points to award for this visit.

## Naming Conventions

- Strategy names should be descriptive and unique
- Use PascalCase (e.g., `MyCustomStrategy`, `TimeBasedScoring`)
- Avoid generic names that might conflict with future built-in strategies

## Plugin Discovery

- Plugins are automatically discovered when `GetAvailableStrategies()` is called
- The system scans the `Plugins/` folder for all `.dll` files
- Each DLL is loaded and searched for types implementing `IScoringStrategy`
- Strategies are registered by their `Name` property

## Error Handling

### Duplicate Strategy Names

If a plugin strategy has the same name as:
- A built-in strategy
- Another plugin strategy

The system will throw an `InvalidOperationException` with details about the conflict. The application will not start if such conflicts exist.

### Invalid DLLs

- Invalid or corrupted DLL files are skipped with a warning
- The application continues to load other plugins
- Check application logs for details about failed plugin loads

### Missing Plugins Folder

If the `Plugins/` folder doesn't exist, the system will use only built-in strategies without error.

## Example Plugin

Here's a complete example of a simple plugin:

```csharp
using Domain;
using VirtualPark.Application.Scoring;

namespace ExamplePlugin;

public class FixedPointsStrategy : IScoringStrategy
{
    public string Name => "FixedPoints";

    public int CalculatePoints(
        Attraction attraction,
        Visitor visitor,
        IEnumerable<AttractionAccess> todayAccesses,
        Domain.SpecialEvent? activeEvent)
    {
        // Award 50 points for every visit
        return 50;
    }
}
```

## Building the Example Plugin

An example plugin (`SimpleFixedPointsStrategy`) is included in this directory. To build it:

### Step 1: Navigate to the Plugins Directory

```bash
cd VirtualPark/Plugins
```

### Step 2: Build the Plugin

```bash
dotnet build VirtualPark.Plugins.Example.csproj -c Release
```

This will create the DLL file in `bin/Release/net8.0/VirtualPark.Plugins.Example.dll`.

### Step 3: Copy the DLL to the Application's Plugins Folder

After building, copy the DLL to where your application expects plugins. The location depends on where your application runs from:

**For Development:**
```bash
# Copy to the WebApi's output directory Plugins folder
cp bin/Release/net8.0/VirtualPark.Plugins.Example.dll ../VirtualPark.WebApi/bin/Debug/net8.0/Plugins/
```

**For Production:**
```bash
# Copy to the published application's Plugins folder
cp bin/Release/net8.0/VirtualPark.Plugins.Example.dll ../VirtualPark.WebApi/bin/Release/net8.0/Plugins/
```

**Note:** Make sure the `Plugins` folder exists in the target directory. If it doesn't exist, create it first.

### Step 4: Verify the Plugin Loads

1. Start your VirtualPark application
2. Call `GET /api/scoring/strategies` to see if `SimpleFixedPoints` appears in the list
3. The plugin should be automatically discovered and loaded

### Quick Build Script

You can also build and copy in one step (adjust paths as needed):

```bash
dotnet build VirtualPark.Plugins.Example.csproj -c Release && \
mkdir -p ../VirtualPark.WebApi/bin/Debug/net8.0/Plugins && \
cp bin/Release/net8.0/VirtualPark.Plugins.Example.dll ../VirtualPark.WebApi/bin/Debug/net8.0/Plugins/
```

## Testing Your Plugin

1. Build your plugin project
2. Copy the DLL to the `Plugins/` folder
3. Start the VirtualPark application
4. Call the `GET /api/scoring/strategies` endpoint to verify your strategy appears in the list
5. Use `PUT /api/scoring/strategies/active` to set your strategy as active
6. Test the scoring by creating attraction accesses

## Best Practices

1. **Keep plugins simple**: Focus on a single scoring logic
2. **Handle nulls**: The `activeEvent` parameter can be `null`
3. **Use todayAccesses**: Leverage the access history for complex scoring logic
4. **Test thoroughly**: Test your plugin with various scenarios before deployment
5. **Document your strategy**: Include comments explaining your scoring logic
6. **Version your plugins**: Consider including version information in your strategy name or DLL filename

## Troubleshooting

### Plugin Not Appearing

- Verify the DLL is in the correct `Plugins/` folder
- Check that the DLL targets .NET 8.0
- Ensure the class implements `IScoringStrategy` correctly
- Verify the class has a parameterless constructor
- Check application logs for loading errors

### Strategy Name Conflicts

- Ensure your strategy name is unique
- Check for typos in strategy names
- Remove conflicting plugins if necessary

### Runtime Errors

- Ensure all dependencies are available
- Check that your plugin references compatible versions of Domain types
- Verify your `CalculatePoints` implementation handles all edge cases

## Support

For issues or questions about creating plugins, refer to the main VirtualPark documentation or contact the development team.


