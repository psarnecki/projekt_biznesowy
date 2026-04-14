using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tripder.Api.Models;
using Tripder.Application.AttractionDefinition;

namespace Tripder.Api.Controllers;

[ApiController]
[Route("api/attractions/{attractionId:guid}/scenarios")]
public class ScenariosController : ControllerBase
{
    private readonly IMediator _mediator;

    public ScenariosController(IMediator mediator)
    {
        // znowu: kontroler = router + wywołanie MediatR, nic więcej (żeby prowadzący nie krzyczał na egzaminie)
        _mediator = mediator;
    }

    [HttpGet("{scenarioId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid attractionId, Guid scenarioId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetScenarioByIdQuery(attractionId, scenarioId), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(Guid attractionId, [FromBody] AddScenarioCommand body, CancellationToken ct)
    {
        if (attractionId != body.AttractionId) return BadRequest("attractionId z route musi zgadzać się z body.");
        var id = await _mediator.Send(body, ct);
        return CreatedAtAction(nameof(GetById), new { attractionId, scenarioId = id }, id);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(Guid attractionId, Guid id, [FromBody] UpdateScenarioCommand body, CancellationToken ct)
    {
        if (attractionId != body.AttractionId || id != body.ScenarioId)
            return BadRequest("Id w URL muszą zgadzać się z body.");
        await _mediator.Send(body, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid attractionId, Guid id, CancellationToken ct)
    {
        await _mediator.Send(new RemoveScenarioCommand(attractionId, id), ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/publish")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Publish(Guid attractionId, Guid id, CancellationToken ct)
    {
        await _mediator.Send(new PublishScenarioCommand(attractionId, id), ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/archive")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Archive(Guid attractionId, Guid id, CancellationToken ct)
    {
        await _mediator.Send(new ArchiveScenarioCommand(attractionId, id), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/tags")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AddTag(Guid attractionId, Guid id, [FromBody] TagNameBody body, CancellationToken ct)
    {
        await _mediator.Send(new AddTagToScenarioCommand(attractionId, id, body.Name), ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}/tags/{tag}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoveTag(Guid attractionId, Guid id, string tag, CancellationToken ct)
    {
        await _mediator.Send(new RemoveTagFromScenarioCommand(attractionId, id, tag), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/rules/{ruleId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AttachRule(Guid attractionId, Guid id, Guid ruleId, CancellationToken ct)
    {
        await _mediator.Send(new AttachRuleToScenarioCommand(attractionId, id, ruleId), ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}/rules/{ruleId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DetachRule(Guid attractionId, Guid id, Guid ruleId, CancellationToken ct)
    {
        await _mediator.Send(new DetachRuleFromScenarioCommand(attractionId, id, ruleId), ct);
        return NoContent();
    }
}
