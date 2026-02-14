using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.API.Attributes;
using Khidmah_Inventory.API.Constants;
using Khidmah_Inventory.Application.Features.WarehouseMap.Queries.GetWarehouseMapsList;
using Khidmah_Inventory.Application.Features.WarehouseMap.Queries.GetWarehouseMapById;
using Khidmah_Inventory.Application.Features.WarehouseMap.Commands.CreateWarehouseMap;
using Khidmah_Inventory.Application.Features.WarehouseMap.Commands.UpdateWarehouseMap;
using Khidmah_Inventory.Application.Features.WarehouseMap.Commands.DeleteWarehouseMap;
using Khidmah_Inventory.Application.Features.WarehouseMap.Queries.GetMapZones;
using Khidmah_Inventory.Application.Features.WarehouseMap.Commands.CreateMapZone;
using Khidmah_Inventory.Application.Features.WarehouseMap.Commands.UpdateMapZone;
using Khidmah_Inventory.Application.Features.WarehouseMap.Commands.DeleteMapZone;
using Khidmah_Inventory.Application.Features.WarehouseMap.Queries.GetMapAisles;
using Khidmah_Inventory.Application.Features.WarehouseMap.Commands.CreateMapAisle;
using Khidmah_Inventory.Application.Features.WarehouseMap.Commands.UpdateMapAisle;
using Khidmah_Inventory.Application.Features.WarehouseMap.Commands.DeleteMapAisle;
using Khidmah_Inventory.Application.Features.WarehouseMap.Queries.GetMapRacks;
using Khidmah_Inventory.Application.Features.WarehouseMap.Commands.CreateMapRack;
using Khidmah_Inventory.Application.Features.WarehouseMap.Commands.UpdateMapRack;
using Khidmah_Inventory.Application.Features.WarehouseMap.Commands.DeleteMapRack;
using Khidmah_Inventory.Application.Features.WarehouseMap.Queries.GetMapBins;
using Khidmah_Inventory.Application.Features.WarehouseMap.Commands.CreateMapBin;
using Khidmah_Inventory.Application.Features.WarehouseMap.Commands.UpdateMapBin;
using Khidmah_Inventory.Application.Features.WarehouseMap.Commands.DeleteMapBin;

namespace Khidmah_Inventory.API.Controllers;

[Route(ApiRoutes.WarehouseMap.Base)]
[Authorize]
public class WarehouseMapController : BaseController
{
    public WarehouseMapController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet]
    [ValidateApiCode(ApiValidationCodes.WarehouseMapModuleCode.ViewAll)]
    [AuthorizeResource(AuthorizePermissions.WarehouseMapPermissions.Controller, AuthorizePermissions.WarehouseMapPermissions.Actions.ViewAll)]
    public async Task<IActionResult> GetList([FromQuery] Guid? warehouseId, [FromQuery] bool? isActive)
    {
        return await ExecuteRequest(new GetWarehouseMapsListQuery { WarehouseId = warehouseId, IsActive = isActive });
    }

    [HttpGet(ApiRoutes.WarehouseMap.GetById)]
    [ValidateApiCode(ApiValidationCodes.WarehouseMapModuleCode.ViewById)]
    [AuthorizeResource(AuthorizePermissions.WarehouseMapPermissions.Controller, AuthorizePermissions.WarehouseMapPermissions.Actions.ViewById)]
    public async Task<IActionResult> GetById(Guid id)
    {
        return await ExecuteRequest(new GetWarehouseMapByIdQuery { Id = id });
    }

    [HttpPost(ApiRoutes.WarehouseMap.Add)]
    [ValidateApiCode(ApiValidationCodes.WarehouseMapModuleCode.Add)]
    [AuthorizeResource(AuthorizePermissions.WarehouseMapPermissions.Controller, AuthorizePermissions.WarehouseMapPermissions.Actions.Add)]
    public async Task<IActionResult> Create([FromBody] CreateWarehouseMapCommand command)
    {
        return await ExecuteRequest(command);
    }

    [HttpPut(ApiRoutes.WarehouseMap.Update)]
    [ValidateApiCode(ApiValidationCodes.WarehouseMapModuleCode.Update)]
    [AuthorizeResource(AuthorizePermissions.WarehouseMapPermissions.Controller, AuthorizePermissions.WarehouseMapPermissions.Actions.Update)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWarehouseMapCommand command)
    {
        command.Id = id;
        return await ExecuteRequest(command);
    }

    [HttpDelete(ApiRoutes.WarehouseMap.Delete)]
    [ValidateApiCode(ApiValidationCodes.WarehouseMapModuleCode.Delete)]
    [AuthorizeResource(AuthorizePermissions.WarehouseMapPermissions.Controller, AuthorizePermissions.WarehouseMapPermissions.Actions.Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        return await ExecuteRequest(new DeleteWarehouseMapCommand { Id = id });
    }

    // Zones
    [HttpGet("{mapId}/zones")]
    [ValidateApiCode(ApiValidationCodes.WarehouseMapModuleCode.Zones)]
    [AuthorizeResource(AuthorizePermissions.WarehouseMapPermissions.Controller, AuthorizePermissions.WarehouseMapPermissions.Actions.ViewById)]
    public async Task<IActionResult> GetZones(Guid mapId)
    {
        return await ExecuteRequest(new GetMapZonesQuery { WarehouseMapId = mapId });
    }

    [HttpPost("{mapId}/zones")]
    [ValidateApiCode(ApiValidationCodes.WarehouseMapModuleCode.Zones)]
    [AuthorizeResource(AuthorizePermissions.WarehouseMapPermissions.Controller, AuthorizePermissions.WarehouseMapPermissions.Actions.Add)]
    public async Task<IActionResult> CreateZone(Guid mapId, [FromBody] CreateMapZoneCommand command)
    {
        command.WarehouseMapId = mapId;
        return await ExecuteRequest(command);
    }

    [HttpPut("{mapId}/zones/{zoneId}")]
    [ValidateApiCode(ApiValidationCodes.WarehouseMapModuleCode.Zones)]
    [AuthorizeResource(AuthorizePermissions.WarehouseMapPermissions.Controller, AuthorizePermissions.WarehouseMapPermissions.Actions.Update)]
    public async Task<IActionResult> UpdateZone(Guid mapId, Guid zoneId, [FromBody] UpdateMapZoneCommand command)
    {
        command.Id = zoneId;
        return await ExecuteRequest(command);
    }

    [HttpDelete("{mapId}/zones/{zoneId}")]
    [ValidateApiCode(ApiValidationCodes.WarehouseMapModuleCode.Zones)]
    [AuthorizeResource(AuthorizePermissions.WarehouseMapPermissions.Controller, AuthorizePermissions.WarehouseMapPermissions.Actions.Delete)]
    public async Task<IActionResult> DeleteZone(Guid mapId, Guid zoneId)
    {
        return await ExecuteRequest(new DeleteMapZoneCommand { Id = zoneId });
    }

    // Aisles
    [HttpGet("zones/{zoneId}/aisles")]
    [ValidateApiCode(ApiValidationCodes.WarehouseMapModuleCode.Aisles)]
    [AuthorizeResource(AuthorizePermissions.WarehouseMapPermissions.Controller, AuthorizePermissions.WarehouseMapPermissions.Actions.ViewById)]
    public async Task<IActionResult> GetAisles(Guid zoneId)
    {
        return await ExecuteRequest(new GetMapAislesQuery { MapZoneId = zoneId });
    }

    [HttpPost("zones/{zoneId}/aisles")]
    [ValidateApiCode(ApiValidationCodes.WarehouseMapModuleCode.Aisles)]
    [AuthorizeResource(AuthorizePermissions.WarehouseMapPermissions.Controller, AuthorizePermissions.WarehouseMapPermissions.Actions.Add)]
    public async Task<IActionResult> CreateAisle(Guid zoneId, [FromBody] CreateMapAisleCommand command)
    {
        command.MapZoneId = zoneId;
        return await ExecuteRequest(command);
    }

    [HttpPut("zones/{zoneId}/aisles/{aisleId}")]
    [ValidateApiCode(ApiValidationCodes.WarehouseMapModuleCode.Aisles)]
    [AuthorizeResource(AuthorizePermissions.WarehouseMapPermissions.Controller, AuthorizePermissions.WarehouseMapPermissions.Actions.Update)]
    public async Task<IActionResult> UpdateAisle(Guid zoneId, Guid aisleId, [FromBody] UpdateMapAisleCommand command)
    {
        command.Id = aisleId;
        return await ExecuteRequest(command);
    }

    [HttpDelete("zones/{zoneId}/aisles/{aisleId}")]
    [ValidateApiCode(ApiValidationCodes.WarehouseMapModuleCode.Aisles)]
    [AuthorizeResource(AuthorizePermissions.WarehouseMapPermissions.Controller, AuthorizePermissions.WarehouseMapPermissions.Actions.Delete)]
    public async Task<IActionResult> DeleteAisle(Guid zoneId, Guid aisleId)
    {
        return await ExecuteRequest(new DeleteMapAisleCommand { Id = aisleId });
    }

    // Racks
    [HttpGet("aisles/{aisleId}/racks")]
    [ValidateApiCode(ApiValidationCodes.WarehouseMapModuleCode.Racks)]
    [AuthorizeResource(AuthorizePermissions.WarehouseMapPermissions.Controller, AuthorizePermissions.WarehouseMapPermissions.Actions.ViewById)]
    public async Task<IActionResult> GetRacks(Guid aisleId)
    {
        return await ExecuteRequest(new GetMapRacksQuery { MapAisleId = aisleId });
    }

    [HttpPost("aisles/{aisleId}/racks")]
    [ValidateApiCode(ApiValidationCodes.WarehouseMapModuleCode.Racks)]
    [AuthorizeResource(AuthorizePermissions.WarehouseMapPermissions.Controller, AuthorizePermissions.WarehouseMapPermissions.Actions.Add)]
    public async Task<IActionResult> CreateRack(Guid aisleId, [FromBody] CreateMapRackCommand command)
    {
        command.MapAisleId = aisleId;
        return await ExecuteRequest(command);
    }

    [HttpPut("aisles/{aisleId}/racks/{rackId}")]
    [ValidateApiCode(ApiValidationCodes.WarehouseMapModuleCode.Racks)]
    [AuthorizeResource(AuthorizePermissions.WarehouseMapPermissions.Controller, AuthorizePermissions.WarehouseMapPermissions.Actions.Update)]
    public async Task<IActionResult> UpdateRack(Guid aisleId, Guid rackId, [FromBody] UpdateMapRackCommand command)
    {
        command.Id = rackId;
        return await ExecuteRequest(command);
    }

    [HttpDelete("aisles/{aisleId}/racks/{rackId}")]
    [ValidateApiCode(ApiValidationCodes.WarehouseMapModuleCode.Racks)]
    [AuthorizeResource(AuthorizePermissions.WarehouseMapPermissions.Controller, AuthorizePermissions.WarehouseMapPermissions.Actions.Delete)]
    public async Task<IActionResult> DeleteRack(Guid aisleId, Guid rackId)
    {
        return await ExecuteRequest(new DeleteMapRackCommand { Id = rackId });
    }

    // Bins (with x,y coordinates)
    [HttpGet("racks/{rackId}/bins")]
    [ValidateApiCode(ApiValidationCodes.WarehouseMapModuleCode.Bins)]
    [AuthorizeResource(AuthorizePermissions.WarehouseMapPermissions.Controller, AuthorizePermissions.WarehouseMapPermissions.Actions.ViewById)]
    public async Task<IActionResult> GetBins(Guid rackId)
    {
        return await ExecuteRequest(new GetMapBinsQuery { MapRackId = rackId });
    }

    [HttpPost("racks/{rackId}/bins")]
    [ValidateApiCode(ApiValidationCodes.WarehouseMapModuleCode.Bins)]
    [AuthorizeResource(AuthorizePermissions.WarehouseMapPermissions.Controller, AuthorizePermissions.WarehouseMapPermissions.Actions.Add)]
    public async Task<IActionResult> CreateBin(Guid rackId, [FromBody] CreateMapBinCommand command)
    {
        command.MapRackId = rackId;
        return await ExecuteRequest(command);
    }

    [HttpPut("racks/{rackId}/bins/{binId}")]
    [ValidateApiCode(ApiValidationCodes.WarehouseMapModuleCode.Bins)]
    [AuthorizeResource(AuthorizePermissions.WarehouseMapPermissions.Controller, AuthorizePermissions.WarehouseMapPermissions.Actions.Update)]
    public async Task<IActionResult> UpdateBin(Guid rackId, Guid binId, [FromBody] UpdateMapBinCommand command)
    {
        command.Id = binId;
        return await ExecuteRequest(command);
    }

    [HttpDelete("racks/{rackId}/bins/{binId}")]
    [ValidateApiCode(ApiValidationCodes.WarehouseMapModuleCode.Bins)]
    [AuthorizeResource(AuthorizePermissions.WarehouseMapPermissions.Controller, AuthorizePermissions.WarehouseMapPermissions.Actions.Delete)]
    public async Task<IActionResult> DeleteBin(Guid rackId, Guid binId)
    {
        return await ExecuteRequest(new DeleteMapBinCommand { Id = binId });
    }
}
