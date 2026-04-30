using Microsoft.AspNetCore.Mvc;
using PusTwo.Application.Interfaces;
using PusTwo.Application.DTOs;
using PusTwo.Web.ViewModels;
using System.Threading.Tasks;
using System;

namespace PusTwo.Web.Controllers
{
    public class BomController : Controller
    {
        private readonly ISysproService _sysproService;
        private readonly IDownTimeService _downTimeService;

        public BomController(ISysproService sysproService, IDownTimeService downTimeService)
        {
            _sysproService = sysproService;
            _downTimeService = downTimeService;
        }

        public IActionResult CreateDownTime()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetMachines()
        {
            try
            {
                var dtos = await _sysproService.GetMachinesAsync();
                return Json(new { success = true, data = dtos });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetJobInfo(string jobNumber)
        {
            if (string.IsNullOrWhiteSpace(jobNumber))
                return Json(new { success = false, message = "Job Number wajib diisi." });

            try
            {
                var dto = await _sysproService.GetJobInfoAsync(jobNumber);
                if (dto == null)
                    return Json(new { success = false, message = "Job tidak ditemukan." });
                
                return Json(new { success = true, data = dto });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetNonProdGroups()
        {
            try
            {
                var dtos = await _sysproService.GetNonProdGroupsAsync();
                return Json(new { success = true, data = dtos });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetNonProdCodes(string groupCode)
        {
            if (string.IsNullOrWhiteSpace(groupCode))
                return Json(new { success = false, message = "Group Code wajib diisi." });

            try
            {
                var dtos = await _sysproService.GetNonProdCodesByGroupAsync(groupCode);
                return Json(new { success = true, data = dtos });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostDownTime([FromBody] DownTimeEntryViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Data tidak valid." });

            try
            {
                var dto = new DownTimeDto
                {
                    Machine = model.Machine,
                    WorkCentre = model.WorkCentre,
                    JobNumber = model.JobNumber,
                    StockCode = model.StockCode,
                    GroupCode = model.GroupCode,
                    Code = model.Code,
                    Description = model.CodeDescription,
                    DowntimeMinutes = model.DowntimeMinutes,
                    Remark = model.Remark,
                    EntryDate = model.EntryDate,
                    Shift = model.Shift,
                    CreatedBy = "User"
                };

                var result = await _downTimeService.CreateDownTimeAsync(dto);
                return Json(new { success = true, message = "Downtime berhasil disimpan.", data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"ERROR: {ex.Message}" });
            }
        }
    }
}