
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
        public async Task<IActionResult> GetAll(
       [FromQuery] DepartmentFilterDto filter,
       CancellationToken cancellationToken)
        {
            var result =
                await _departmentService.GetAllAsync(
                    filter,
                    cancellationToken);

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = HRPermissions.Department.View)]
        public async Task<IActionResult> GetById(int id,
        CancellationToken cancellationToken)
        {
            var result =
                await _departmentService.GetByIdAsync(
                    id,
                    cancellationToken);

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Policy = HRPermissions.Department.Create)]
        public async Task<IActionResult> Create(
         CreateDepartmentDto dto,
         CancellationToken cancellationToken)
        {
            var result =
                await _departmentService.CreateAsync(
                    dto,
                    cancellationToken);

            return Ok(result);
        }

        [HttpPatch("{id:int}")]
        [Authorize(Policy = HRPermissions.Department.Update)]
        public async Task<IActionResult> Update(int id,
         UpdateDepartmentDto dto,
         CancellationToken cancellationToken)
        {
            var result =
                await _departmentService.UpdateAsync(
                    id,
                    dto,
                    cancellationToken);

            return Ok(result);
        }

        [HttpPut("{id:int}/manager")]
        [Authorize(Policy = HRPermissions.DepartmentManager.Assign)]
        public async Task<IActionResult> AssignManager(int id,
        AssignDepartmentManagerDto dto,
        CancellationToken cancellationToken)
        {
            var result =
                await _departmentService.AssignManagerAsync(
                    id,
                    dto,
                    cancellationToken);

            return Ok(result);
        }

        [HttpPut("{managerEmployeeId:int}/transfer-manager")]
        [Authorize(Policy = HRPermissions.DepartmentManager.Transfer)]
        public async Task<IActionResult> TransferManager(
        int managerEmployeeId,
        TransferDepartmentManagerDto dto,
        CancellationToken cancellationToken)
        {
            var result =
                await _departmentService.TransferDepartmentManagerAsync(
                    managerEmployeeId,
                    dto,
                    cancellationToken);

            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = HRPermissions.Department.Delete)]
        public async Task<IActionResult> Delete(int id,
        CancellationToken cancellationToken)
        {
            var result =
                await _departmentService.DeleteAsync(
                    id,
                    cancellationToken);

            return Ok(result);
        }

        [HttpPatch("{id:int}/restore")]
        [Authorize(Policy = HRPermissions.Department.Delete)]
        public async Task<IActionResult> Restore(int id,
        CancellationToken cancellationToken)
        {
            var result =
                await _departmentService.RestoreAsync(
                    id,
                    cancellationToken);

            return Ok(result);
        }


        [HttpGet("lookup")]
        [Authorize(Policy = HRPermissions.Department.View)]
        public async Task<IActionResult> Lookup(
         CancellationToken cancellationToken)
        {
            return Ok(
                await _departmentService
                    .GetLookupAsync(cancellationToken));
        }

        [HttpGet("lookup/without-manager")]
        [Authorize(Policy = HRPermissions.Department.View)]
        public async Task<IActionResult> LookupWithoutManager(
        CancellationToken cancellationToken)
        {
            return Ok(
                await _departmentService
                    .GetDepartmentsWithoutManagerAsync(cancellationToken));
        }
    }
}