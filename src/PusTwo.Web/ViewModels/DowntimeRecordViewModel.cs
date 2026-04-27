namespace PusTwo.Web.ViewModels
{
    public class DowntimeRecordViewModel
    {
        public DateTime EntryDate { get; set; }
        public string Shift { get; set; } = string.Empty;
        public string Job { get; set; } = string.Empty;
        public string StockCode { get; set; } = string.Empty;
        public string NonProdCode { get; set; } = string.Empty;
        public string GrpCode { get; set; } = string.Empty;
        public string GrpDescription { get; set; } = string.Empty;
        public decimal TeardownTime { get; set; }
        public string Machine { get; set; } = string.Empty;
    }
}