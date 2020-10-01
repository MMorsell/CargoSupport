using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;
using System;

namespace CargoSupport.Models
{
    public class AboutModel : PageModel
    {
        public string Message { get; set; }

        public void OnGet()
        {
            Message = $"About page visited at {DateTime.UtcNow.ToLongTimeString()}";
            Log.Logger.Information(Message);
        }
    }
}