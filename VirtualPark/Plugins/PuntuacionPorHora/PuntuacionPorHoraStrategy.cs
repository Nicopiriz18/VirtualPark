using VirtualPark.Application.Scoring;
using VirtualPark.Domain;

namespace VirtualPark.Plugins.PuntuacionPorHora;

public class PuntuacionPorHoraStrategy : IScoringStrategy
{
    public string Name => "PuntuacionPorHora";

    public int CalculatePoints(
        Attraction attraction,
        Visitor visitor,
        IEnumerable<AttractionAccess> todayAccesses,
        Domain.SpecialEvent? activeEvent)
    {
        const int basePoints = 50;

        // Obtener hora actual
        var currentHour = DateTime.Now.Hour;

        // Multiplicador según hora del día
        // Horas pico: 10:00-14:00 y 18:00-22:00 -> multiplicador 1.5x
        // Otras horas -> multiplicador 1.0x
        double multiplier = 1.0;

        if ((currentHour >= 10 && currentHour < 14) ||
            (currentHour >= 18 && currentHour < 22))
        {
            multiplier = 1.5;
        }

        return (int)(basePoints * multiplier);
    }
}
