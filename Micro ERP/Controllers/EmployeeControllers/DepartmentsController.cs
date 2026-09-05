using Micro_ERP.Controllers;
using MicroERP.Application.Common.Models;
using MicroERP.Application.Features.Departments.DTOs;
using MicroERP.Application.Features.Departments.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Micro_ERP.Controllers.EmployeeControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentsController : BaseApiController
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentsController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet]
        [Authorize(Policy = HRPermissions.Department.View)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _departmentService.GetAllAsync();

            if (result is null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = HRPermissions.Department.View)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _departmentService.GetByIdAsync(id);

            if (result is null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = HRPermissions.Department.Create)]
        public async Task<IActionResult> Create(CreateDepartmentDto dto)
        {
            var result =
                await _departmentService.CreateAsync(dto);

            return Ok(result);
        }

        [HttpPatch("{id:int}")]
        [Authorize(Policy = HRPermissions.Department.Update)]
        public async Task<IActionResult> Update( int id,UpdateDepartmentDto dto)
        {
            var result = await _departmentService.UpdateAsync(id, dto);

            return Ok(result);
        }

        [HttpPut("{id:int}/manager")]
        [Authorize(Policy = HRPermissions.Department.AssignManager)]
        public async Task<IActionResult> AssignManager(int id,AssignDepartmentManagerDto dto)
        {
            var result =
                await _departmentService.AssignManagerAsync(
                    id,
                    dto);

            return Ok(result);
        }

        [HttpPut("{managerEmployeeId:int}/transfer-manager")]
        [Authorize(Policy = HRPermissions.Department.AssignManager)]
        public async Task<IActionResult> TransferManager(int managerEmployeeId,TransferDepartmentManagerDto dto)
        {
            var result = await _departmentService
                .TransferDepartmentManagerAsync(
                    managerEmployeeId,
                    dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = HRPermissions.Department.Delete)]
        public async Task<IActionResult> Delete(int id)
        {
            await _departmentService.DeleteAsync(id);

            return Ok(new Result
            {
                Success = true,
                Message = "Department Deleted successfully."
            });
        }

        [HttpPatch("{id}/restore")]
        [Authorize(Policy = HRPermissions.Department.Delete)]
        public async Task<IActionResult> Restore(int id)
        {
            await _departmentService.RestoreAsync(id);

            return Ok(new Result
            {
                Success = true,
                Message = "Department restored successfully."
            });
        }

        [HttpGet("lookup")]
        public async Task<IActionResult> Lookup()
        {
            return Ok(await _departmentService.GetLookupAsync());
        }
    }
}