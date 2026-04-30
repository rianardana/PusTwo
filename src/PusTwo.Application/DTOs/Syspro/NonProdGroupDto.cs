namespace PusTwo.Application.DTOs.Syspro
{
    public class NonProdGroupDto
    {
        public string GrpCode { get; set; } = string.Empty;
        public string GrpDescription { get; set; } = string.Empty;
        public string DisplayText => $"{GrpCode} - {GrpDescription}";
    }
}