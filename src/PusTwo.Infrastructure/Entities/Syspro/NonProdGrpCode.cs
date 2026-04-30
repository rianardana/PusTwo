namespace PusTwo.Infrastructure.Entities.Syspro
{
    public class NonProdGrpCode
    {
        public string GrpCode { get; set; } = string.Empty;
        public string NonProdScrap { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Code => NonProdScrap;
        public string CodeDescription => Description;
    }
}