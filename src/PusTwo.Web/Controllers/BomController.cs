using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PusTwo.Application.Interfaces;
using AutoMapper;
using PusTwo.Web.ViewModels;
using PusTwo.Application.DTOs.Syspro;

namespace PusTwo.Web.Controllers
{
    
    public class BomController : Controller
    {
        private readonly ISysproService _sysproService;
        private readonly IMapper _mapper;

        public BomController(ISysproService sysproService, IMapper mapper)
        {
            _sysproService = sysproService;
            _mapper = mapper;
        }

        public IActionResult Create()
        {
            return View(new BomViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BomViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            TempData["SuccessMessage"] = "Data BOM berhasil disimpan!";
            return RedirectToAction(nameof(Create));
        }

        [HttpGet]
        public async Task<IActionResult> GetWorkCentresByJob(string jobNumber)
        {
            if (string.IsNullOrWhiteSpace(jobNumber))
                return BadRequest(new { success = false, message = "Job Number wajib diisi." });

            try
            {
                var dtos = await _sysproService.GetWorkCentresByJobAsync(jobNumber);
                var viewModels = _mapper.Map<List<BomViewModel>>(dtos);
                var stockCode = viewModels.FirstOrDefault()?.StockCode ?? string.Empty;

                return Json(new
                {
                    success = true,
                    message = $"Ditemukan {viewModels.Count} data WorkCentre (Route 0).",
                    stockCode,
                    data = viewModels
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"ERROR: {ex.Message}",
                    data = new List<BomViewModel>()
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMachines()
        {
            try
            {
                var dtos = await _sysproService.GetMachinesAsync();
                var viewModels = _mapper.Map<List<MachineViewModel>>(dtos);

                return Json(new
                {
                    success = true,
                    message = $"Ditemukan {viewModels.Count} Machine(s).",
                    data = viewModels
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"ERROR: {ex.Message}",
                    data = new List<MachineViewModel>()
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetJobInfo(string jobNumber)
        {
            if (string.IsNullOrWhiteSpace(jobNumber))
                return BadRequest(new { success = false, message = "Job Number wajib diisi." });

            try
            {
                var dto = await _sysproService.GetJobInfoAsync(jobNumber);

                if (dto == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Job tidak ditemukan di WipMaster.",
                        data = (object)null
                    });
                }

                var viewModel = _mapper.Map<JobLookupViewModel>(dto);

                return Json(new
                {
                    success = true,
                    message = "Data Job ditemukan.",
                    data = viewModel
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"ERROR: {ex.Message}",
                    data = (object)null
                });
            }
        }




        [HttpGet]
        public async Task<IActionResult> GetDowntimeHistory(DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                var dtos = await _sysproService.GetDowntimeHistoryAsync(dateFrom, dateTo);
                var viewModels = _mapper.Map<List<DowntimeRecordViewModel>>(dtos);

                return Json(new
                {
                    success = true,
                    message = $"Ditemukan {viewModels.Count} record(s).",
                    data = viewModels
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"ERROR: {ex.Message}",
                    data = new List<DowntimeRecordViewModel>()
                });
            }
        }

        [HttpGet]
        public IActionResult CreateDownTime()
        {
            return View(new CreateDownTimeViewModel());
        }


        [HttpGet]
            public async Task<IActionResult> GetNonProdGroups()
            {
                try
                {
                    var dtos = await _sysproService.GetNonProdGroupsAsync();
                    
                    return Json(new 
                    { 
                        success = true, 
                        data = dtos 
                    });
                }
                catch (Exception ex)
                {
                    return Json(new 
                    { 
                        success = false, 
                        message = $"ERROR: {ex.Message}",
                        data = new List<object>() 
                    });
                }
            }

            [HttpGet]
            public async Task<IActionResult> GetNonProdCodes(string groupCode)
            {
                if (string.IsNullOrWhiteSpace(groupCode))
                    return Json(new { success = false, message = "Group Code wajib diisi.", data = new List<object>() });

                try
                {
                    var dtos = await _sysproService.GetNonProdCodesByGroupAsync(groupCode);
                    
                    return Json(new 
                    { 
                        success = true, 
                        data = dtos 
                    });
                }
                catch (Exception ex)
                {
                    return Json(new 
                    { 
                        success = false, 
                        message = $"ERROR: {ex.Message}", 
                        data = new List<object>() 
                    });
                }
            }               
    }
}