namespace PusTwo.Web.ViewModels
{
    public class MachineViewModel
    {
        public string Machine { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string WorkCentre { get; set; } = string.Empty;
        public string DisplayText => $"{Machine} | {Description}";
    }
}