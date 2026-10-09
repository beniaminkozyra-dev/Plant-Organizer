using System;
using PlantOrganizer.Api.Dtos;

namespace PlantOrganizer.Api.Services;

public interface IPlantService
{
    Task<List<PlantResponse>> GetAllAsync();
    Task<PlantResponse?> GetByIdAsync(int id);
    Task<PlantResponse> CreateAsync(CreatePlantRequest request);
    Task<bool> UpdateAsync(int id, UpdatePlantRequest request);
    Task<bool> DeleteAsync(int id);
    Task<PlantResponse?> WaterAsync(int id);
    Task<List<PlantResponse>> GetDueForWateringAsync();
}
