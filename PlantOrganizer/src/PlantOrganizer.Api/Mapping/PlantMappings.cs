using System;
using PlantOrganizer.Api.Dtos;
using PlantOrganizer.Api.Models;

namespace PlantOrganizer.Api.Mapping;

public static class PlantMappings
{
    public static PlantResponse ToResponse(this Plant plant)
    {
        return new PlantResponse(
            plant.Id,
            plant.Name,
            plant.LatinName,
            plant.WateringIntervalDays,
            plant.SunRequirement,
            plant.MinPotVolumeLiters,
            plant.DaysToHarvest,
            plant.LastWateredAt,
            NextWateringDate: plant.LastWateredAt?.AddDays(plant.WateringIntervalDays)
        );
    }

    public static Plant ToEntity(this CreatePlantRequest request)
    {
        return new Plant
        {
            Name = request.Name,
            LatinName = request.LatinName,
            WateringIntervalDays = request.WateringIntervalDays,
            SunRequirement = request.SunRequirement,
            MinPotVolumeLiters = request.MinPotVolumeLiters,
            DaysToHarvest = request.DaysToHarvest
        };
    }

    public static void UpdateFrom(this Plant plant, UpdatePlantRequest request)
    {
        plant.Name = request.Name;
        plant.LatinName = request.LatinName;
        plant.WateringIntervalDays = request.WateringIntervalDays;
        plant.SunRequirement = request.SunRequirement;
        plant.MinPotVolumeLiters = request.MinPotVolumeLiters;
        plant.DaysToHarvest = request.DaysToHarvest;
    }
}
