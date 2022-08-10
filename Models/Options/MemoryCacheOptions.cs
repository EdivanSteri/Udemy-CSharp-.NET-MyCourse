using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace MyCourse.Models.Options
{
    public class MemoryCacheOptions : IOptions<MemoryCacheOptions>
    {
        public MemoryCacheOptions() { }

        public ISystemClock Clock { get; set; }

        public bool CompactOnMemoryPressure { get; set; }

        public TimeSpan ExpirationScanFrequency { get; set; }   

        public double CompactionPercentage { get; set; }

        public MemoryCacheOptions Value => throw new NotImplementedException();
    }
}
