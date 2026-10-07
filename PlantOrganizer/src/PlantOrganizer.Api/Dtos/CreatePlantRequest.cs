using System;
using System.ComponentModel.DataAnnotations;
using PlantOrganizer.Api.Models;

namespace PlantOrganizer.Api.Dtos;

public record CreatePlantRequest(
    [Required, MaxLength(50)] string Name,
    [MaxLength(50)] string? LatinName,
    [Range(1, 50)] int WateringIntervalDays,
    SunRequirement SunRequirement,
    [Range(0.1, 50)] decimal MinPotVolumeLiters,
    [Range(1, 200)] int? DaysToHarvest
    );

