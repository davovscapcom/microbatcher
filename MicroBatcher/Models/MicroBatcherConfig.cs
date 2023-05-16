namespace MicroBatcher;
public struct MicroBatcherConfig
{
	public uint BatchSize { get; set; }
	public double BatchProcessingIntervalMilliseconds { get; set; }
}
