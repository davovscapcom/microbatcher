namespace MicroBatcher.Tests;

using FluentAssertions;

public class MicroBatcherConfigTest
{
	//[Theory]
	//[InlineData(1)]
	//[InlineData(1_000_000)]
	//[InlineData(uint.MaxValue)]
	//public void MicroBatcher_ShouldAllowConfigurationOf_BatchSize(uint batchSize)
	//{
	//	MicroBatcherConfig config = new()
	//	{
	//		BatchSize = batchSize
	//	};
	//	MicroBatcher batcher = new(config);

	//	batcher.BatchSize.Should().Be(batchSize);
	//}

	//[Theory]
	//[InlineData(1)]
	//[InlineData(1_000_000)]
	//public void MicroBatcher_ShouldAllowConfigurationOf_BatchProcessingInterval(double intervalInMilliseconds)
	//{
	//	MicroBatcherConfig config = new()
	//	{
	//		BatchProcessingIntervalMilliseconds = intervalInMilliseconds
	//	};
	//	MicroBatcher batcher = new(config);

	//	batcher.BatchProcessingIntervalTimer.Should().NotBeNull();
	//	batcher.BatchProcessingIntervalTimer.Interval.Should().Be(intervalInMilliseconds);
	//}

	// Test that microbatcher doesn't submit anything for processing if there is nothing to process

	// Test file for Configuration Options
	// Ensure intervals, min/max batch size configuration options work correctly when
	// specified through the constructor, or through ENV.
	// Maybe use a MicroBatcher configuration class, which can be passed to the constructor,
	// deserialised from a config file, or built from ENV.

	// Test file for Accepting Jobs
	// Can accept a single job as required.
	// Job is added to our list

	// Test file for Submitting Jobs to be processed
	// Jobs that exist within our list are submitted to the processor at the configured interval.
}
