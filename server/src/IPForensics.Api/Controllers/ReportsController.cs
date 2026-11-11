using IPForensics.Core.Dtos;
using IPForensics.Core.Dtos.Reports;
using IPForensics.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IPForensics.Api.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize]
public sealed class ReportsController : ControllerBase
{
    private readonly IReportService _reports;

    public ReportsController(IReportService reports)
    {
        _reports = reports;
    }

    [HttpPost]
    public async Task<ActionResult<ReportDto>> Create(
        CreateReportRequest request, CancellationToken cancellationToken)
    {
        if (CurrentUserId is not { } userId)
        {
            return Unauthorized();
        }

        var report = await _reports.GenerateAsync(userId, request.IpAddress, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = report.Id }, report);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<ReportSummaryDto>>> List(
        [FromQuery] ReportListQuery query, CancellationToken cancellationToken)
    {
        if (CurrentUserId is not { } userId)
        {
            return Unauthorized();
        }

        return Ok(await _reports.ListAsync(userId, query.Page, query.PageSize, cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ReportDto>> Get(Guid id, CancellationToken cancellationToken)
    {
        if (CurrentUserId is not { } userId)
        {
            return Unauthorized();
        }

        return Ok(await _reports.GetAsync(userId, id, cancellationToken));
    }
    
    private Guid? CurrentUserId =>
        Guid.TryParse(User.FindFirst("sub")?.Value, out var id) ? id : null;
}
