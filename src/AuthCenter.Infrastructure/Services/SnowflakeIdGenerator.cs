using AuthCenter.Domain.Services;

namespace AuthCenter.Infrastructure.Services;

public class SnowflakeIdGenerator : IIdGenerator
{
    private const long Epoch = 1704067200000L; // 2024-01-01 00:00:00 UTC
    private const int WorkerIdBits = 5;
    private const int DatacenterIdBits = 5;
    private const int SequenceBits = 12;
    private const long MaxWorkerId = -1L ^ (-1L << WorkerIdBits);
    private const long MaxDatacenterId = -1L ^ (-1L << DatacenterIdBits);
    private const long SequenceMask = -1L ^ (-1L << SequenceBits);
    private const int WorkerIdShift = SequenceBits;
    private const int DatacenterIdShift = SequenceBits + WorkerIdBits;
    private const int TimestampShift = SequenceBits + WorkerIdBits + DatacenterIdBits;

    private readonly long _workerId;
    private readonly long _datacenterId;
    private readonly object _lock = new();
    private long _sequence;
    private long _lastTimestamp = -1L;

    public SnowflakeIdGenerator(long workerId = 0, long datacenterId = 0)
    {
        if (workerId > MaxWorkerId || workerId < 0)
            throw new ArgumentException($"Worker ID must be between 0 and {MaxWorkerId}");
        if (datacenterId > MaxDatacenterId || datacenterId < 0)
            throw new ArgumentException($"Datacenter ID must be between 0 and {MaxDatacenterId}");
        _workerId = workerId;
        _datacenterId = datacenterId;
    }

    public long NewId()
    {
        lock (_lock)
        {
            var timestamp = GetTimestamp();
            if (timestamp < _lastTimestamp)
                throw new InvalidOperationException("Clock moved backwards.");

            if (timestamp == _lastTimestamp)
            {
                _sequence = (_sequence + 1) & SequenceMask;
                if (_sequence == 0) timestamp = WaitNextMillis(_lastTimestamp);
            }
            else
            {
                _sequence = 0;
            }

            _lastTimestamp = timestamp;
            return ((timestamp - Epoch) << TimestampShift) | (_datacenterId << DatacenterIdShift) | (_workerId << WorkerIdShift) | _sequence;
        }
    }

    private static long GetTimestamp() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    private static long WaitNextMillis(long lastTimestamp)
    {
        var timestamp = GetTimestamp();
        while (timestamp <= lastTimestamp) timestamp = GetTimestamp();
        return timestamp;
    }
}
