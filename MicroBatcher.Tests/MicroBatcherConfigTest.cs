namespace MicroBatcher.Tests;

using FluentAssertions;

[Collection("Sequential")]
public class MicroBatcherEnvironentVariableConfigTest
{
	[Theory]
	[InlineData(1)]
	[InlineData(1_000_000)]
	[InlineData(uint.MaxValue)]
	public void MicroBatcher_ShouldAllowConfigurationOf_BatchSize_UsingEnvironmentVariables(uint batchSize)
	{
		Environment.SetEnvironmentVariable(
			Constants.EnvironmentVariables.BatchSize, batchSize.ToString());
		MicroBatcher batcher = new();

		batcher.BatchSize.Should().Be(batchSize);
	}

	[Theory]
	[InlineData(1)]
	[InlineData(1_000_000)]
	[InlineData(uint.MaxValue)]
	public void MicroBatcher_ShouldAllowConfigurationOf_BatchInterval_UsingEnvironmentVariables(double intervalInMilliseconds)
	{
		Environment.SetEnvironmentVariable(
			Constants.EnvironmentVariables.BatchInterval, intervalInMilliseconds.ToString());
		MicroBatcher batcher = new();

		batcher.BatchInterval.Should().NotBeNull();
		batcher.BatchInterval.Interval.Should().Be(intervalInMilliseconds);
	}
}

[Collection("Sequential")]
public class MicroBatcherConfigModelTest
{
	[Theory]
	[InlineData(1)]
	[InlineData(1_000_000)]
	[InlineData(uint.MaxValue)]
	public void MicroBatcher_ShouldAllowConfigurationOf_BatchSize(uint batchSize)
	{
		MicroBatcherConfig config = new() { BatchSize = batchSize };
		MicroBatcher batcher = new(config);

		batcher.BatchSize.Should().Be(batchSize);
	}

	[Theory]
	[InlineData(1)]
	[InlineData(1_000_000)]
	public void MicroBatcher_ShouldAllowConfigurationOf_BatchInterval(double intervalInMilliseconds)
	{
		MicroBatcherConfig config = new() { BatchProcessingIntervalMilliseconds = intervalInMilliseconds };
		MicroBatcher batcher = new(config);

		batcher.BatchInterval.Should().NotBeNull();
		batcher.BatchInterval.Interval.Should().Be(intervalInMilliseconds);
	}

}
