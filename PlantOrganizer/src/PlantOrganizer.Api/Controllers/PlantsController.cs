using System;
using Microsoft.AspNetCore.Mvc;
using PlantOrganizer.Api.Models;

namespace PlantOrganizer.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PlantsController : ControllerBase
{
    private static readonly Plant[] Plants =
    [
        new Plant { Id = 0, Name = "Pomidor", DaysToHarvest = 40, MinPotVolumeLiters = 5, SunRequirement = SunRequirement.FullSun, WateringIntervalDays = 2},
        new Plant { Id = 1, Name = "Bazylia", DaysToHarvest = 20, MinPotVolumeLiters = 3, SunRequirement = SunRequirement.PartialShade, WateringIntervalDays = 3},
        new Plant { Id = 2, Name = "Papryka", DaysToHarvest = 50, MinPotVolumeLiters = 7.5M, SunRequirement = SunRequirement.FullSun, WateringIntervalDays = 3},
    ];

    [HttpGet]
    public IEnumerable<Plant> Get()
    {
        return Plants.ToArray();
    }

    [HttpGet("{id}")]
    public ActionResult<Plant> GetById(int id)
    {
        var plant = Plants.FirstOrDefault(p => p.Id == id);
        if (plant is null)
            return NotFound();
        return plant;
    }

}
