using System.ComponentModel.DataAnnotations;

namespace DotNetCore.NTier.Models.Dto
{
    public class WeatherForecast
    {
        [Display(Name = "Date")]
        public DateTime Date { get; set; }

        [Display(Name = "Temp. (C)")]
        public int TemperatureC { get; set; }

        [Display(Name = "Temp. (F)")]
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        [Display(Name = "Summary")]
        public string? Summary { get; set; }
    }
}