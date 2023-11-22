using DotNetCore.NTier.Models.Dto;

namespace DotNetCore.NTier.Services
{
    public interface IWeatherForecastService
    {
        public IEnumerable<WeatherForecast> Get();
    }
}