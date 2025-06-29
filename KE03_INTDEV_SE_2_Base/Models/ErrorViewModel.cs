namespace KE03_INTDEV_SE_2_Base.Models
{
    // ViewModel voor het weergeven van foutmeldingen aan de gebruiker
    public class ErrorViewModel
    {
        // Unieke identificatie van de foutmelding voor debugging doeleinden
        public string? RequestId { get; set; }

        // Geeft aan of de RequestId getoond moet worden aan de gebruiker
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
