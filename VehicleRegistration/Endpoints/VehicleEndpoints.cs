using Microsoft.AspNetCore.Mvc;
using VehicleRegistration.Domain.Entities;
using VehicleRegistration.Domain.Interfaces;

namespace VehicleRegistration.Endpoints
{
    public static class VehicleEndpoints
    {
        public static void MapVehicleEndpoints(this WebApplication app)
        {
            app.MapGet("/vehicles", ([FromQuery] int? page, [FromQuery] int? pageSize, [FromQuery] string? name, [FromQuery] string? brand, [FromQuery] int? year, IVehicleService vehicleService) =>
                {
                    try
                    {
                        return Results.Ok(vehicleService.GetAll(page ??= 1, pageSize ??= 10, name, brand, year));
                    }
                    catch (Exception e)
                    {
                        return Results.BadRequest(e.Message);
                    }
                }).WithTags("Vehicles").AllowAnonymous();

            app.MapGet("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
            {
                try
                {
                    return Results.Ok(vehicleService.GetById(id));
                }
                catch (Exception e)
                {
                    return Results.BadRequest(e.Message);
                }
            }).WithTags("Vehicles").AllowAnonymous();

            app.MapPost("/vehicles", ([FromBody] Vehicle vehicle, IVehicleService vehicleService) =>
                {
                    try
                    {
                        return Results.Created($"/vehicles/{vehicleService.Create(vehicle).Id}", vehicle);
                    }
                    catch (Exception e)
                    {
                        return Results.BadRequest(e.Message);
                    }
                }).WithTags("Vehicles").RequireAuthorization();

            app.MapPut("/vehicles/{id}", ([FromRoute] int id, [FromBody] Vehicle vehicle, IVehicleService vehicleService) =>
            {
                try
                {
                    return id != vehicle.Id ? Results.BadRequest("Id mismatch") : Results.Ok(vehicleService.Update(vehicle));
                }
                catch (Exception e)
                {
                    return Results.BadRequest(e.Message);
                }
            }).WithTags("Vehicles").RequireAuthorization();

            app.MapDelete("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
            {
                try
                {
                    vehicleService.Delete(id);
                    return Results.NoContent();
                }
                catch (Exception e)
                {
                    return Results.BadRequest(e.Message);
                }
            }).WithTags("Vehicles").RequireAuthorization();
        }
    }
}