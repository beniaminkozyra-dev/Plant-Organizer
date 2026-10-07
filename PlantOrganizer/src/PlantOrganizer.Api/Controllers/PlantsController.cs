using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlantOrganizer.Api.Data;
using PlantOrganizer.Api.Dtos;
using PlantOrganizer.Api.Mapping;
using PlantOrganizer.Api.Models;

namespace PlantOrganizer.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PlantsController : ControllerBase
{
    private readonly AppDbContext _context;

    public PlantsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<PlantResponse>>> GetAll()
    {
        var plants =  await _context.Plants.AsNoTracking().ToListAsync();
        return plants.Select(p => p.ToResponse()).ToList();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PlantResponse>> GetById(int id)
    {
        var plant = await _context.Plants.FindAsync(id);
        if (plant is null)
            return NotFound();
        return plant.ToResponse();
    }

    [HttpPost]
    public async Task<ActionResult<PlantResponse>> Post(CreatePlantRequest request)
    {
        var plant = request.ToEntity();
        _context.Plants.Add(plant);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = plant.Id }, plant.ToResponse());
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, UpdatePlantRequest request)
    {
        var plant = await _context.Plants.FindAsync(id);
        if(plant is null)
            return  NotFound();

        plant.UpdateFrom(request);

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var plant = await _context.Plants.FindAsync(id);

        if(plant is null)
            return NotFound();

        _context.Plants.Remove(plant);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
