namespace PusTwo.Application.DTOs.Syspro
{
    public class BomOperationDto
    {
        public string StockCode { get; set; } = string.Empty;

        public string Route { get; set; } = string.Empty;

        public decimal Operation { get; set; }

        public string WorkCentre { get; set; } = string.Empty;
    }
}