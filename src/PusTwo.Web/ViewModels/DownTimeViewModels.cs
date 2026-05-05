using System.ComponentModel.DataAnnotations;

namespace PusTwo.Web.ViewModels
{
    public class DownTimeBatchFormViewModel
    {
        [Required(ErrorMessage = "Machine wajib dipilih")]
        public string Machine { get; set; } = string.Empty;
        
        public string WorkCentre { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Job Number wajib diisi")]
        public string JobNumber { get; set; } = string.Empty;
        
        public string StockCode { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Entry Date wajib diisi")]
        [DataType(DataType.Date)]
        public DateTime EntryDate { get; set; } = DateTime.Now;
        
        [Required(ErrorMessage = "Shift wajib dipilih")]
        public string Shift { get; set; } = "1";
        
        public List<DownTimeBatchLineViewModel> Entries { get; set; } = new();
    }

    public class DownTimeBatchLineViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Group Code wajib dipilih")]
        public string GroupCode { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Code wajib dipilih")]
        public string Code { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Downtime minutes wajib diisi")]
        [Range(1, 1440, ErrorMessage = "Minimal 1 menit, maksimal 1440 menit")]
        public int DowntimeMinutes { get; set; }
        
        public string? Remark { get; set; }
    }
}