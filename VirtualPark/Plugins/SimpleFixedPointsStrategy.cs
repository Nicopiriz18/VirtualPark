using VirtualPark.Domain;
using VirtualPark.Application.Scoring;

namespace VirtualPark.Plugins;

public class SimpleFixedPointsStrategy : IScoringStrategy
{
    public string Name => "SimpleFixedPoints";

    public int CalculatePoints(
        Attraction attraction,
        Visitor visitor,
        IEnumerable<AttractionAccess> todayAccesses,
        Domain.SpecialEvent? activeEvent)
    {
        return 50;
    }
}

