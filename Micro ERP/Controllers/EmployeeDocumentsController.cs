using MicroERP.Application.Authorization.Permissions;
using MicroERP.Application.Common.Constants;
using MicroERP.Application.Features.Documents.EmployeeDocuments.DTOs;
using MicroERP.Application.Features.Documents.EmployeeDocuments.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Micro_ERP.Controllers;

[Route("api/employees/{employeeId}/documents")]
[ApiController]
public class EmployeeDocumentsController : ControllerBase
{
    private readonly IEmployeeDocumentQueries _queries;
    private readonly IEmployeeDocumentService _service;


    public EmployeeDocumentsController(
        IEmployeeDocumentQueries queries,
        IEmployeeDocumentService service)
    {
        _queries = queries;
        _service = service;
    }



    [HttpGet]
    [Authorize(Policy = EmployeeDocumentsPermissions.EmployeeDocuments.View)]
    public async Task<IActionResult> GetAll(
        int employeeId,
        CancellationToken cancellationToken)
    {
        var result = await _queries.GetByEmployeeIdAsync(
            employeeId,
            cancellationToken);

        return Ok(result);
    }



    [HttpGet("{id:int}")]
    [Authorize(Policy = EmployeeDocumentsPermissions.EmployeeDocuments.View)]
    public async Task<IActionResult> GetById(
        int employeeId,
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _queries.GetByIdAsync(
            id,
            cancellationToken);


        if (result == null)
            return NotFound();


        return Ok(result);
    }



    [Authorize(Policy = EmployeeDocumentsPermissions.EmployeeDocuments.View)]
    [HttpGet("{id:int}/download")]
    public async Task<IActionResult> Download(
    int employeeId,
    int id,
    CancellationToken cancellationToken)
    {
        var document = await _queries.GetByIdAsync(
            id,
            cancellationToken);


        if (document == null)
            return NotFound();


        var filePath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "Storage",
            document.FilePath);


        if (!System.IO.File.Exists(filePath))
            return NotFound("File not found");


        var fileBytes = await System.IO.File.ReadAllBytesAsync(
            filePath,
            cancellationToken);


        return File(
            fileBytes,
            document.ContentType,
            document.FileName);
    }


    [HttpPost]
    [Authorize(Policy = EmployeeDocumentsPermissions.EmployeeDocuments.Create)]
    public async Task<IActionResult> Create(
        int employeeId,
        [FromForm] CreateEmployeeDocumentDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(
            employeeId,
            dto,
            cancellationToken);


        if (!result.Success)
            return BadRequest(result);


        return Ok(result);
    }




    [HttpPut("{id:int}")]
    [Authorize(Policy = EmployeeDocumentsPermissions.EmployeeDocuments.Update)]
    public async Task<IActionResult> Update(int employeeId,int id,
        [FromForm] UpdateEmployeeDocumentDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(id,dto,cancellationToken);


        if (!result.Success)
            return BadRequest(result);


        return Ok(result);
    }




    [HttpDelete("{id:int}")]
    [Authorize(Policy = EmployeeDocumentsPermissions.EmployeeDocuments.Delete)]
    public async Task<IActionResult> Delete(int employeeId,int id,
        CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(id,cancellationToken);


        if (!result.Success)
            return BadRequest(result);


        return Ok();
    }
}