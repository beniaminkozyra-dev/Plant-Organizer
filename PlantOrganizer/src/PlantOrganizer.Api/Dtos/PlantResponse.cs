using System;
using System.ComponentModel.DataAnnotations;
using PlantOrganizer.Api.Models;

namespace PlantOrganizer.Api.Dtos;

public record PlantResponse(
    int Id,
    string Name,
    string? LatinName,
    int WateringIntervalDays,
    SunRequirement SunRequirement,
    decimal MinPotVolumeLiters,
    int? DaysToHarvest
    );
