using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PusTwo.Application.Interfaces;
using AutoMapper;
using PusTwo.Web.ViewModels;

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


        // Di file: src/PusTwo.Web/Controllers/BomController.cs

[HttpGet]
public async Task<IActionResult> GetWorkCentresByJob(string jobNumber)
{
    if (string.IsNullOrWhiteSpace(jobNumber))
        return BadRequest(new { success = false, message = "Job Number wajib diisi." });

    try
    {
        var dtos = await _sysproService.GetWorkCentresByJobAsync(jobNumber);
        var viewModels = _mapper.Map<List<BomViewModel>>(dtos);

        // Ambil StockCode dari data pertama (semua row pasti sama StockCode-nya)
        var stockCode = viewModels.FirstOrDefault()?.StockCode ?? string.Empty;

        return Json(new
        {
            success = true,
            message = $"Ditemukan {viewModels.Count} data WorkCentre (Route 0).",
            stockCode = stockCode,  // ✅ Return StockCode untuk ditampilkan
            data = viewModels
        });
    }
    catch (Exception ex)
        {
            return Json(new
            {
                success = false,
                message = $"ERROR SYSpro: {ex.Message}", // Tampilkan detail
                data = new List<BomViewModel>()
            });
        }
        }
    }
}