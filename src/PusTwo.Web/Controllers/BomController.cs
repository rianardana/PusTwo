using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PusTwo.Application.DownTime.DTOs;
using PusTwo.Application.DownTime.Interfaces;
using PusTwo.Application.Syspro.Interfaces;
using PusTwo.Web.ViewModels;
using System.Text.Json;

namespace PusTwo.Web.Controllers
{
    public class BomController : Controller
    {
        private readonly ISysproService _sysproService;
        private readonly IDownTimeService _downTimeService;
        private readonly IMapper _mapper;
        private readonly ILogger<BomController> _logger;

        public BomController(
            ISysproService sysproService, 
            IDownTimeService downTimeService,
            IMapper mapper,
            ILogger<BomController> logger)
        {
            _sysproService = sysproService;
            _downTimeService = downTimeService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IActionResult> CreateDownTime()
        {
            var viewModel = new DownTimeBatchFormViewModel();
            return View(viewModel);
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
                _logger.LogError(ex, "Error fetching machines");
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
                _logger.LogError(ex, "Error fetching job info: {JobNumber}", jobNumber);
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
                _logger.LogError(ex, "Error fetching non-prod groups");
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
                _logger.LogError(ex, "Error fetching non-prod codes for group: {GroupCode}", groupCode);
                return Json(new { success = false, message = ex.Message });
            }
        }

       [HttpPost]
        public PartialViewResult RenderDowntimeRow([FromBody] JsonElement entryData)
        {
            try
            {
                var entry = new DownTimeBatchLineViewModel
                {
                    Id = entryData.GetProperty("id").GetInt32(),
                    GroupCode = entryData.GetProperty("groupCode").GetString() ?? string.Empty,
                    Code = entryData.GetProperty("code").GetString() ?? string.Empty,
                    DowntimeMinutes = entryData.GetProperty("minutes").GetInt32(),
                    Remark = entryData.GetProperty("remark").GetString()
                };

                return PartialView("_DowntimeRow", entry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rendering downtime row");
                return PartialView("_DowntimeRow", new DownTimeBatchLineViewModel());
            }
        }

        [HttpPost]
        public PartialViewResult RenderDowntimeTable([FromBody] JsonElement entriesData)
        {
            try
            {
                var entries = new List<DownTimeBatchLineViewModel>();
                
                if (entriesData.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in entriesData.EnumerateArray())
                    {
                        entries.Add(new DownTimeBatchLineViewModel
                        {
                            Id = item.GetProperty("id").GetInt32(),
                            GroupCode = item.GetProperty("groupCode").GetString() ?? string.Empty,
                            Code = item.GetProperty("code").GetString() ?? string.Empty,
                            DowntimeMinutes = item.GetProperty("minutes").GetInt32(),
                            Remark = item.GetProperty("remark").GetString()
                        });
                    }
                }

                return PartialView("_DowntimeTable", entries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rendering downtime table");
                return PartialView("_DowntimeTable", new List<DownTimeBatchLineViewModel>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostDownTime([FromBody] DownTimeBatchFormViewModel model)
        {
            try
            {
                _logger.LogInformation("Received downtime batch request. Machine: {Machine}, Job: {Job}, Entries: {Count}", 
                    model.Machine, model.JobNumber, model.Entries?.Count ?? 0);

                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                        );

                    _logger.LogWarning("Model validation failed. Errors: {@Errors}", errors);

                    return BadRequest(new
                    {
                        success = false,
                        message = "Data tidak valid.",
                        errors = errors
                    });
                }

                if (model.Entries == null || model.Entries.Count == 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Tidak ada entry downtime."
                    });
                }

                var userId = "system";

                var command = _mapper.Map<CreateDownTimeBatchCommand>(model);

                foreach (var entry in command.Entries)
                {
                    entry.CreatedBy = userId;
                }

                var savedCount = await _downTimeService.CreateBatchAsync(command);

                return Json(new
                {
                    success = true,
                    message = $"{savedCount} downtime entry berhasil disimpan.",
                    count = savedCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error posting downtime batch. Machine: {Machine}, Job: {Job}",
                    model.Machine, model.JobNumber);

                return Json(new
                {
                    success = false,
                    message = $"ERROR: {ex.Message}",
                    details = ex.InnerException?.Message
                });
            }
        }
    }
}