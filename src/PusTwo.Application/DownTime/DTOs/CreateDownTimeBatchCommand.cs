namespace PusTwo.Application.DownTime.DTOs
{
    public class CreateDownTimeBatchCommand
    {
        public DateTime EntryDate { get; set; }
        public string Shift { get; set; } = string.Empty;
        public string Machine { get; set; } = string.Empty;
        public string WorkCentre { get; set; } = string.Empty;
        public string JobNumber { get; set; } = string.Empty;
        public string StockCode { get; set; } = string.Empty;
        public List<CreateDownTimeCommand> Entries { get; set; } = new();
    }
}