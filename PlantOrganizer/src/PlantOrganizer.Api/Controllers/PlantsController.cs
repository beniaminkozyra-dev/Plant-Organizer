using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlantOrganizer.Api.Data;
using PlantOrganizer.Api.Models;

namespace PlantOrganizer.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PlantsController : ControllerBase
{
    /*
    private static readonly Plant[] Plants =
    [
        new Plant { Id = 0, Name = "Pomidor", DaysToHarvest = 40, MinPotVolumeLiters = 5, SunRequirement = SunRequirement.FullSun, WateringIntervalDays = 2},
        new Plant { Id = 1, Name = "Bazylia", DaysToHarvest = 20, MinPotVolumeLiters = 3, SunRequirement = SunRequirement.PartialShade, WateringIntervalDays = 3},
        new Plant { Id = 2, Name = "Papryka", DaysToHarvest = 50, MinPotVolumeLiters = 7.5M, SunRequirement = SunRequirement.FullSun, WateringIntervalDays = 3},
    ];
*/
    private readonly AppDbContext _context;

    public PlantsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<Plant>>> GetAll()
    {
        return await _context.Plants.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Plant>> GetById(int id)
    {
        var plant = await _context.Plants.FindAsync(id);
        if (plant is null)
            return NotFound();
        return plant;
    }

    [HttpPost]
    public async Task<ActionResult<Plant>> Post(Plant plant)
    {
        _context.Plants.Add(plant);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = plant.Id }, plant);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, Plant plant)
    {
        if(id != plant.Id)
            return BadRequest();

        var requestedPlant = await _context.Plants.FindAsync(id);
        if(requestedPlant is null)
            return  NotFound();

        requestedPlant.Name = plant.Name;
        requestedPlant.LatinName = plant.LatinName;
        requestedPlant.DaysToHarvest = plant.DaysToHarvest;
        requestedPlant.MinPotVolumeLiters = plant.MinPotVolumeLiters;
        requestedPlant.SunRequirement = plant.SunRequirement;
        requestedPlant.WateringIntervalDays = plant.WateringIntervalDays;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var plant = await _context.Plants.FindAsync(id);

        if(plant is null)
            return NotFound();

        _context.Remove(plant);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
