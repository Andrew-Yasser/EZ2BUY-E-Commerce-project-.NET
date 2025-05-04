namespace Ez2Buy.DataAccess.Models
{
    // ErrorViewModel: Model used to display error information in error views/pages
    public class ErrorViewModel
    {
        // Stores the unique identifier for the HTTP request that caused the error
        public string? RequestId { get; set; }

        // Expression-bodied property that returns true if RequestId has a value
        // Controls whether the RequestId should be displayed in the error view
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}