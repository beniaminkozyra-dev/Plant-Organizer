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

    private DateOnly Today => DateOnly.FromDateTime(_timeProvider.GetLocalNow().DateTime);

    public PlantService(AppDbContext context, TimeProvider timeProvider)
    {
        _context = context;
        _timeProvider = timeProvider;
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
            return false;

        _context.Plants.Remove(plant);
        await _context.SaveChangesAsync();

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
            return false;

        plant.UpdateFrom(request);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<PlantResponse?> WaterAsync(int id)
    {
        var plant = await _context.Plants.FindAsync(id);
        if (plant is null)
            return null;

        plant.LastWateredAt = Today;
        await _context.SaveChangesAsync();
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
}
