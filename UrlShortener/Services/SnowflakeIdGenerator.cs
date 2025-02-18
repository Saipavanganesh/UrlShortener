using System.Diagnostics.Eventing.Reader;
using UrlShortener.Interfaces;

namespace UrlShortener.Services
{
    public class SnowflakeIdGenerator : ISnowflakeIdGenerator
    {
        private const long CUSTOM_EPOCH = 1704067200000L; // Start date for epoch (Jan 1, 2024)
        private const int MAX_MACHINE_ID = 1023; // Maximum machine ID (1023 because machine ID is 10 bits)
        private const int MAX_SEQUENCE = 4095; // Maximum sequence value (4095 because sequence is 12 bits)

        private readonly long _machineId;
        private long _lastTimestamp = -1L;
        private long _sequence = 0L;
        private readonly object _lock = new();

        public SnowflakeIdGenerator(int machineId)
        {
            const long maxMachineId = 1023; //(2^10) - 1 = 1023
            if(machineId < 0 || machineId > maxMachineId)
                throw new ArgumentOutOfRangeException(nameof(machineId), $"Machine ID must be in the range of 0 and {maxMachineId}");
            _machineId = machineId;
        }
        public long GenerateId()
        {
            lock(_lock)
            {
                long timestamp = GetCurrentTimestamp();
                if(timestamp == _lastTimestamp)
                {
                    _sequence = IncrementSequence();
                    if(_sequence == 0)
                    {
                        timestamp = WaitForNextMillis(timestamp);
                    }
                }
                else
                {
                    _sequence = 0;
                }
                _lastTimestamp = timestamp;
                return BuildUniqueId(timestamp);
            }
        }
        public long GetCurrentTimestamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
        public long IncrementSequence()
        {
           return (_sequence + 1) % (MAX_SEQUENCE + 1);
        }
        private long WaitForNextMillis(long lastTimestamp)
        {
            long timestamp;
            do
            {
                timestamp = GetCurrentTimestamp();
            }while(timestamp <= lastTimestamp);
            return timestamp;
        }
        private long BuildUniqueId(long timestamp)
        {
            long timestampPart = (timestamp - CUSTOM_EPOCH);
            long machinePart = _machineId * (MAX_SEQUENCE + 1);
            long sequencePart = _sequence;

            return timestampPart | machinePart | sequencePart;
        }
    }   
}
