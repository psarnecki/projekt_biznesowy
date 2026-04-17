using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tripder.Api.Models;
using Tripder.Application.AttractionDefinition.Commands;
using Tripder.Application.AttractionDefinition.Queries;
using Tripder.Domain.AttractionDefinition.Enums;

namespace Tripder.Api.Controllers;

[ApiController]
[Route("api/attractions")]
public class AttractionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AttractionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] AttractionState? state, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllAttractionsQuery(state?.ToString()), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAttractionByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateAttractionCommand command, CancellationToken ct)
    {
        var id = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAttractionCommand body, CancellationToken ct)
    {
        if (id != body.Id) return BadRequest("Id w URL musi być takie samo jak w body.");
        await _mediator.Send(body, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteAttractionCommand(id), ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/publish")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Publish(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new PublishAttractionCommand(id), ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/archive")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Archive(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new ArchiveAttractionCommand(id), ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/make-internal")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> MakeInternal(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new MakeAttractionInternalCommand(id), ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/catalog-window")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SetCatalogWindow(Guid id, [FromBody] SetAttractionCatalogWindowCommand body, CancellationToken ct)
    {
        if (id != body.AttractionId) return BadRequest("Id w URL musi być takie samo jak w body.");
        await _mediator.Send(body, ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/tags")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AddTag(Guid id, [FromBody] TagNameBody body, CancellationToken ct)
    {
        await _mediator.Send(new AddTagToAttractionCommand(id, body.Name), ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}/tags/{tag}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoveTag(Guid id, string tag, CancellationToken ct)
    {
        await _mediator.Send(new RemoveTagFromAttractionCommand(id, tag), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/rules/{ruleId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AttachRule(Guid id, Guid ruleId, CancellationToken ct)
    {
        await _mediator.Send(new AttachRuleToAttractionCommand(id, ruleId), ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}/rules/{ruleId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DetachRule(Guid id, Guid ruleId, CancellationToken ct)
    {
        await _mediator.Send(new DetachRuleFromAttractionCommand(id, ruleId), ct);
        return NoContent();
    }
}
