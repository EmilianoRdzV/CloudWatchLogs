using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using Microsoft.AspNetCore.Mvc;

namespace CloudWatchLogs.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>> GetAsync(string cityName)
        {
            var count = Random.Shared.Next(5, 15);
            _logger.LogInformation("Get Weather Forecast called for city {cityName} with count of {count}", cityName, count );
            //await LogUsingClient(cityName);

            return Enumerable.Range(1, count).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        private static async Task LogUsingClient(string cityName)
        {
            var logClient = new AmazonCloudWatchLogsClient();
            var logGroupName = "/aws/weather-forecast-appEmi";
            var logStreamName = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
            var existing = await logClient.DescribeLogGroupsAsync(new DescribeLogGroupsRequest()
            { LogGroupNamePrefix = logGroupName });
            var logGroupExist = existing.LogGroups.Any(l => l.LogGroupName == logGroupName);
            if (!logGroupExist)
                await logClient.CreateLogGroupAsync(new CreateLogGroupRequest(logGroupName));


            await logClient.CreateLogStreamAsync(new CreateLogStreamRequest(logGroupName, logStreamName));
            await logClient.PutLogEventsAsync(new PutLogEventsRequest()
            {
                LogGroupName = logGroupName,
                LogStreamName = logStreamName,
                LogEvents = new List<InputLogEvent>()
                {
                    new() {Message = $"Get Weather Forecast called for city {cityName}", Timestamp = DateTime.UtcNow}
                }
            });
        }
    }
}
