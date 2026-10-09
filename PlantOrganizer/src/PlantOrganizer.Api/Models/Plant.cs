using System;
using System.ComponentModel.DataAnnotations;

namespace PlantOrganizer.Api.Models;

public class Plant
{
    public int Id { get; set; }
    
    [MaxLength(50)][Required] 
    public required string Name { get; set; }

    [MaxLength(50)]
    public string? LatinName { get; set; }

    [Range(1,50)]
    public int WateringIntervalDays { get; set; }

    public SunRequirement SunRequirement { get; set; }

    [Range(0.1, 50)]
    public decimal MinPotVolumeLiters { get; set; }

    [Range(1,200)]
    public int? DaysToHarvest { get; set; }

    public DateOnly? PlantedAt { get; set; }
    public DateOnly? LastWateredAt { get; set; }
}
