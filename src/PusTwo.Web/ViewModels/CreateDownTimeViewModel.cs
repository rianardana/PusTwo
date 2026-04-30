namespace PusTwo.Web.ViewModels
{
    public class DowntimeEntryViewModel
    {
        public string GroupCode { get; set; } = string.Empty;
        public string GroupDescription { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string CodeDescription { get; set; } = string.Empty;
        public int DowntimeMinutes { get; set; }
        public string? Remark { get; set; }
    }

    public class CreateDownTimeViewModel
    {
        public string Machine { get; set; } = string.Empty;
        public string WorkCentre { get; set; } = string.Empty;
        public string JobNumber { get; set; } = string.Empty;
        public string StockCode { get; set; } = string.Empty;
        public string StockDescription { get; set; } = string.Empty;
        public DateTime JobDate { get; set; } = DateTime.Now;
        public string Shift { get; set; } = "1";
        public string? RNJob { get; set; }
        public decimal RunTime { get; set; }
        public decimal Quantity { get; set; }
        public List<DowntimeEntryViewModel> DowntimeEntries { get; set; } = new();
    }
}