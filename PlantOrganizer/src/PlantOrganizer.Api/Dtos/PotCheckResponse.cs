namespace PlantOrganizer.Api.Dtos;

public record class PotCheckResponse(
    int PlantId,
    decimal ProvidedVolumeLiters,
    decimal MinPotVolumeLiters,
    bool isSufficient
);
