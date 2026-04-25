using PusTwo.Application.DTOs.Syspro;

namespace PusTwo.Application.Interfaces
{
    /// <summary>
    /// Interface untuk layanan integrasi dengan Syspro (ERP)
    /// Hanya operasi read-only untuk mengambil data master
    /// </summary>
    public interface ISysproService
    {
        
        Task<List<BomOperationDto>> GetWorkCentresByJobAsync(string jobNumber);
    }
}