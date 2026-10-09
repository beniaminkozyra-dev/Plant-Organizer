using System;
using Microsoft.AspNetCore.Mvc;
using PlantOrganizer.Api.Dtos;
using PlantOrganizer.Api.Services;

namespace PlantOrganizer.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PlantsController : ControllerBase
{
    private readonly IPlantService _plantService;

    public PlantsController(IPlantService plantService)
    {
        _plantService = plantService;
    }

    [HttpGet]
    public async Task<ActionResult<List<PlantResponse>>> GetAll()
    {
        return await _plantService.GetAllAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PlantResponse>> GetById(int id)
    {
        var plant = await _plantService.GetByIdAsync(id);
        if (plant is null)
            return NotFound();
        return plant;
    }

    [HttpPost]
    public async Task<ActionResult<PlantResponse>> Post(CreatePlantRequest request)
    {
        var plant = await _plantService.CreateAsync(request);

        return CreatedAtAction(nameof(GetById), new { id = plant.Id }, plant);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, UpdatePlantRequest request)
    {
        var updated = await _plantService.UpdateAsync(id, request);
        if(updated == false)
            return  NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _plantService.DeleteAsync(id);

        if(deleted == false)
            return NotFound();

        return NoContent();
    }

    [HttpPost("{id}/water")]
    public async Task<ActionResult<PlantResponse>> Water(int id)
    {
        var plant = await _plantService.WaterAsync(id);
        if (plant is null)
            return NotFound();
        return plant;
    }

    [HttpGet("due-for-watering")]
    public async Task<ActionResult<List<PlantResponse>>> GetDueForWatering()
    {
        return await _plantService.GetDueForWateringAsync();
    }
}
