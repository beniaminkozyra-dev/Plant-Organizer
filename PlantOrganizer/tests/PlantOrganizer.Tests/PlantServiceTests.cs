using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using PlantOrganizer.Api.Data;
using PlantOrganizer.Api.Dtos;
using PlantOrganizer.Api.Models;
using PlantOrganizer.Api.Services;

namespace PlantOrganizer.Tests;

public class PlantServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _context;
    private readonly PlantService _service;
    private readonly FakeTimeProvider _clock;
    
    public PlantServiceTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();

        //_service = new PlantService(_context, TimeProvider.System, NullLogger<PlantService>.Instance);

        _clock = new FakeTimeProvider(new DateTimeOffset(2026, 10, 1, 12, 0, 0, TimeSpan.Zero));
        _service = new PlantService(_context, _clock, NullLogger<PlantService>.Instance);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    private static CreatePlantRequest CreateRequest(
    string name = "Bazylia",
    int interval = 3,
    decimal minPotVolume = 5m,
    int? daysToHarvest = null,
    DateOnly? plantedAt = null) =>
    new(name, null, interval, SunRequirement.Shade, minPotVolume, daysToHarvest, plantedAt);


    [Fact]
    public async Task GetDueForWateringAsync_DoesNotReturnPlant_BeforeIntervalPasses()
    {
        var created = await _service.CreateAsync(
            CreateRequest());
        await _service.WaterAsync(created.Id);

        _clock.Advance(TimeSpan.FromDays(2));
        var due = await _service.GetDueForWateringAsync();

        Assert.DoesNotContain(due, p => p.Id == created.Id);
    }

    [Fact]
    public async Task GetDueForWateringAsync_ReturnsPlant_WhenIntervalPasses()
    {
        var created = await _service.CreateAsync(
            CreateRequest());
        await _service.WaterAsync(created.Id);

        _clock.Advance(TimeSpan.FromDays(3));
        var due = await _service.GetDueForWateringAsync();

        Assert.Contains(due, p => p.Id == created.Id);
    }

    [Fact]
    public async Task GetDueForWateringAsync_ReturnsPlant_WhenNeverWatered()
    {
        var created = await _service.CreateAsync(
            CreateRequest());

        var due = await _service.GetDueForWateringAsync();

        Assert.Contains(due, p => p.Id == created.Id);
    }

    [Fact]
    public async Task GetDueForWateringAsync_DoesNotReturnPlant_WhenWateredToday()
    {
        var created = await _service.CreateAsync(
            CreateRequest());

        await _service.WaterAsync(created.Id);

        var due = await _service.GetDueForWateringAsync();

        Assert.DoesNotContain(due, p => p.Id == created.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenPlantDoesNotExist()
    {
        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesPlant_WhenPlantExists()
    {
        // Arrange
        var created = await _service.CreateAsync(new CreatePlantRequest("Monstera","",5, Api.Models.SunRequirement.PartialShade, 
        7.5M, null, null));

        var request = new UpdatePlantRequest("Fikus", "", 5, Api.Models.SunRequirement.Shade, 3, 0, null);

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
        var request = new UpdatePlantRequest("Fikus", "", 5, Api.Models.SunRequirement.Shade, 3, null, null);

        var result = await _service.UpdateAsync(999, request);

        Assert.False(result);
    }

    [Fact]
    public async Task CreateAsync_ReturnsPlantWithId()
    {
        // Arrange
        var request = new CreatePlantRequest("Monstera", "", 7, Api.Models.SunRequirement.Shade, 3, null, null);

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
        var created = await _service.CreateAsync(new CreatePlantRequest("Monstera", "", 5, Api.Models.SunRequirement.PartialShade, 
        7.5M, null, null));
        
        // Act
        var result = await _service.DeleteAsync(created.Id);
        var afterDelete = await _service.GetByIdAsync(created.Id);

        // Assert
        Assert.True(result);
        Assert.Null(afterDelete);
    }

    [Fact]
    public async Task WaterAsync_SetsLastWateredAndNextWateringDate()
    {
        var created = await _service.CreateAsync(CreateRequest(interval: 3));

        var result = await _service.WaterAsync(created.Id);

        Assert.NotNull(result);
        Assert.Equal(new DateOnly(2026, 10, 1), result.LastWateredAt);
        Assert.Equal(new DateOnly(2026, 10, 4), result.NextWateringDate);
    }

    [Fact]
    public async Task WaterAsync_ReturnsNull_WhenPlantDoesNotExist()
    {
        var result = await _service.WaterAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_CalculatesExpectedHarvestDate()
    {
        var request = CreateRequest(daysToHarvest: 40, plantedAt: new DateOnly(2026, 6, 6));

        var result = await _service.CreateAsync(request);

        Assert.Equal(new DateOnly(2026, 7, 16), result.ExpectedHarvestDate);
    }

    [Fact]
    public async Task CreateAsync_ReturnsNoHarvestDate_WhenPlantHasNoHarvest()
    {
        var request = CreateRequest(daysToHarvest: null, plantedAt: new DateOnly(2026, 6, 6));

        var result = await _service.CreateAsync(request);

        Assert.Null(result.ExpectedHarvestDate);
    }

    [Theory]
    [InlineData(3, false)]
    [InlineData(5, true)]
    [InlineData(8, true)]
    public async Task CheckPotAsync_ReturnsExpectedResult(decimal volume, bool expected)
    {
        var created = await _service.CreateAsync(CreateRequest(minPotVolume: 5m));

        var result = await _service.CheckPotAsync(created.Id, volume);

        Assert.NotNull(result);
        Assert.Equal(expected, result.isSufficient);
    }

    [Fact]
    public async Task CheckPotAsync_ReturnsNull_WhenPlantDoesNotExist()
    {
        var result = await _service.CheckPotAsync(999, 5m);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllPlants()
    {
        await _service.CreateAsync(CreateRequest(name: "Bazylia"));
        await _service.CreateAsync(CreateRequest(name: "Pomidor"));

        var result = await _service.GetAllAsync();

        Assert.Equal(2, result.Count);
    }
}
