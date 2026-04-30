namespace PusTwo.Application.DTOs
{
    public class DownTimeDto
    {
        public int Id { get; set; }
        public string Machine { get; set; } = string.Empty;
        public string WorkCentre { get; set; } = string.Empty;
        public string JobNumber { get; set; } = string.Empty;
        public string StockCode { get; set; } = string.Empty;
        public string GroupCode { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DowntimeMinutes { get; set; }
        public string? Remark { get; set; }
        public DateTime EntryDate { get; set; }
        public string Shift { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}