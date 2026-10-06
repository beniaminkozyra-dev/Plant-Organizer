using System;

namespace PlantOrganizer.Api.Models;

public class Plant
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? LatinName { get; set; }
    public int WateringIntervalDays { get; set; }
    public SunRequirement SunRequirement { get; set; }
    public decimal MinPotVolumeLiters { get; set; }
    public int? DaysToHarvest { get; set; }
}
