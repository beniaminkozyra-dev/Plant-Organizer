using System;
using Microsoft.EntityFrameworkCore;
using PlantOrganizer.Api.Data;
using PlantOrganizer.Api.Dtos;
using PlantOrganizer.Api.Mapping;

namespace PlantOrganizer.Api.Services;

public class PlantService : IPlantService
{
    private readonly AppDbContext _context;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<PlantService> _logger;

    private DateOnly Today => DateOnly.FromDateTime(_timeProvider.GetLocalNow().DateTime);

    public PlantService(AppDbContext context, TimeProvider timeProvider, ILogger<PlantService> logger)
    {
        _context = context;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task<PlantResponse> CreateAsync(CreatePlantRequest request)
    {
        var plant = request.ToEntity();
        _context.Plants.Add(plant);
        await _context.SaveChangesAsync();

        return plant.ToResponse();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var plant = await _context.Plants.FindAsync(id);
        if(plant is null)
        {
            _logger.LogWarning("Plant {PlantId} not found while deleting", id);
            return false;
        }
        _context.Plants.Remove(plant);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Plant {PlantId} deleted", id);
        return true;
    }

    public async Task<List<PlantResponse>> GetAllAsync()
    {
        var plants = await _context.Plants.AsNoTracking().ToListAsync();

        return plants.Select(p => p.ToResponse()).ToList();
    }

    public async Task<PlantResponse?> GetByIdAsync(int id)
    {
        var plant = await _context.Plants.FindAsync(id);
        return plant?.ToResponse();

    }

    public async Task<bool> UpdateAsync(int id, UpdatePlantRequest request)
    {
        var plant = await _context.Plants.FindAsync(id);
        if(plant is null)
        {
            _logger.LogWarning("Plant {PlantId} not found while updating", id);
            return false;
        }

        plant.UpdateFrom(request);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Plant {PlantId} updated", id);
        return true;
    }

    public async Task<PlantResponse?> WaterAsync(int id)
    {
        var plant = await _context.Plants.FindAsync(id);
        if (plant is null)
        {
            _logger.LogWarning("Plant {PlantId} not found while watering", id);
            return null;
        }

        plant.LastWateredAt = Today;
        await _context.SaveChangesAsync();
        _logger.LogInformation("Plant {PlantId} watered on {Date}", id, Today);

        return plant.ToResponse();
    }

    public async Task<List<PlantResponse>> GetDueForWateringAsync()
    {
        var today = Today;
        var plants = await _context.Plants.AsNoTracking().ToListAsync();

        return plants
            .Where(p => p.LastWateredAt is null
                     || p.LastWateredAt.Value.AddDays(p.WateringIntervalDays) <= today)
            .Select(p => p.ToResponse())
            .ToList();
    }

    public async Task<PotCheckResponse?> CheckPotAsync(int id, decimal volumeLiters)
    {
        var plant = await GetByIdAsync(id);

        if (plant is null)
            return null;

        return new PotCheckResponse(id, volumeLiters, plant.MinPotVolumeLiters, plant.MinPotVolumeLiters <= volumeLiters);
    }
}
