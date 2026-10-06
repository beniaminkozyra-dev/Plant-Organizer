using System;
using Microsoft.EntityFrameworkCore;
using PlantOrganizer.Api.Models;

namespace PlantOrganizer.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<Plant> Plants => Set<Plant>();
}
