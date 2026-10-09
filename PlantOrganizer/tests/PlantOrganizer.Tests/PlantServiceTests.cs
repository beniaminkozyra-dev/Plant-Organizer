using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PlantOrganizer.Api.Data;
using PlantOrganizer.Api.Dtos;
using PlantOrganizer.Api.Services;

namespace PlantOrganizer.Tests;

public class PlantServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _context;
    private readonly PlantService _service;

    public PlantServiceTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();

        _service = new PlantService(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenPlantDoesNotExist()
    {
        // Arrange: pusta baza

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesPlant_WhenPlantExists()
    {
        // Arrange
        var created = await _service.CreateAsync(new CreatePlantRequest("Monstera","",5, Api.Models.SunRequirement.PartialShade, 7.5M, null));

        var request = new UpdatePlantRequest("Fikus", "", 5, Api.Models.SunRequirement.Shade, 3, 0);

        var updated = await _service.UpdateAsync(created.Id, request);
        var afterUpdate = await _service.GetByIdAsync(created.Id);

        // Assert
        Assert.True(updated);
        Assert.NotNull(afterUpdate);
        Assert.Equal("Fikus", afterUpdate.Name);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFalse_WhenPlantDoesNotExist()
    {
        var request = new UpdatePlantRequest("Fikus", "", 5, Api.Models.SunRequirement.Shade, 3, null);

        var result = await _service.UpdateAsync(999, request);

        Assert.False(result);
    }

    [Fact]
    public async Task CreateAsync_ReturnsPlantWithId()
    {
        // Arrange
        var request = new CreatePlantRequest("Monstera", "", 7, Api.Models.SunRequirement.Shade, 3, null);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        Assert.True(result.Id > 0);
        Assert.Equal("Monstera", result.Name);
        Assert.Equal(7, result.WateringIntervalDays);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenPlantDoesNotExist()
    {
        var result = await _service.DeleteAsync(999);

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsTrue_WhenPlantExist()
    {

        // Arrange
        var created = await _service.CreateAsync(new CreatePlantRequest("Monstera", "", 5, Api.Models.SunRequirement.PartialShade, 7.5M, null));
        
        // Act
        var result = await _service.DeleteAsync(created.Id);
        var afterDelete = await _service.GetByIdAsync(created.Id);

        // Assert
        Assert.True(result);
        Assert.Null(afterDelete);
    }
}
