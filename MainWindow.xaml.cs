using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace WeatherApp
{
    public partial class MainWindow : Window
    {
        private readonly WeatherService _weatherService = new WeatherService();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void GetWeatherButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var cityName = CityTextBox.Text;
                var weatherData = await _weatherService.GetWeatherData(cityName);

                TemperatureLabel.Content = $"{weatherData.Temperature} °C";
                HumidityLabel.Content = $"{weatherData.Humidity} %";
                WindSpeedLabel.Content = $"{weatherData.WindSpeed} m/s";
                DescriptionLabel.Content = weatherData.Description;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class WeatherService
    {
        private const string API_KEY = "ee1b8eaa308cd9f07b324f8b805d6fe1";
        private const string API_URL = "https://api.openweathermap.org/data/2.5/weather";

        public async Task<WeatherData> GetWeatherData(string cityName)
        {
            using (var client = new HttpClient())
            {
                var url = $"{API_URL}?q={cityName}&appid={API_KEY}&units=metric";
                var response = await client.GetAsync(url);
                var json = await response.Content.ReadAsStringAsync();
                dynamic data = JsonConvert.DeserializeObject(json);

                if (data.cod != 200)
                {
                    throw new Exception((string)data.message);

                }

                var weatherData = new WeatherData
                {
                    Temperature = (int)data.main.temp,
                    Humidity = (int)data.main.humidity,
                    WindSpeed = (int)data.wind.speed,
                    Description = data.weather[0].description
                };

                return weatherData;
            }
        }
    }

    public class WeatherData
    {
        public int Temperature { get; set; }
        public int Humidity { get; set; }
        public int WindSpeed { get; set; }
        public string Description { get; set; }
    }
}
