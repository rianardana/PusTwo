namespace PusTwo.Web.ViewModels
{
    public class NonProdGroupViewModel
    {
        public string GrpCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string DisplayText => $"{GrpCode} - {Description}";
    }
}