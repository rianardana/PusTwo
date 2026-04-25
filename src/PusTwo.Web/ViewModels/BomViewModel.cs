namespace PusTwo.Web.ViewModels
{
   
    public class BomViewModel
    {
        public string JobNumber { get; set; } = string.Empty;
        public string StockCode { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public decimal Operation { get; set; }
        public string WorkCentre { get; set; } = string.Empty;
        public List<BomViewModel> WorkCentres { get; set; } = new();
    }
}