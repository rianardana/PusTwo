namespace PusTwo.Web.ViewModels
{
    public class DownTimeEntryViewModel
    {
        public string Machine { get; set; } = string.Empty;
        public string WorkCentre { get; set; } = string.Empty;
        public string JobNumber { get; set; } = string.Empty;
        public string StockCode { get; set; } = string.Empty;
        public string GroupCode { get; set; } = string.Empty;
        public string GroupDescription { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string CodeDescription { get; set; } = string.Empty;
        public int DowntimeMinutes { get; set; }
        public string? Remark { get; set; }
        public DateTime EntryDate { get; set; } = DateTime.Now;
        public string Shift { get; set; } = "1";
    }
}