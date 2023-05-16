namespace MicroBatcher.Tests;

using FluentAssertions;

public class MicroBatcherTest 
{
	//[Fact]
	//public void MicroBatcher_UponCreation_ShouldContainNoJobs()
	//{
	//   // TODO: Move batcher instantiation to fixture.
	//	MicroBatcher batcher = new();
	//	batcher.Jobs.Should().HaveCount(0);
	//}

	//[Fact]
	//public void SubmitJob_ValidJobGiven_ShouldAddToJobsList()
	//{
	//	MicroBatcher batcher = new();
	//	batcher.SubmitJob(new Job());
	//	batcher.Jobs.Should().HaveCount(1);
	//}

	[Fact]
	public async void SubmitJobAsync_ValidJobGiven_ShouldAddToJobsList()
	{
		MicroBatcher batcher = new();

		Job job = new Job()
		{
			Id = 1
		};

		var batchJob1 = batcher.SubmitJob(new() { Id = 1 });
		batcher.SubmitJob(new() { Id = 2 });
		batcher.SubmitJob(new() { Id = 3 });
		batcher.SubmitJob(new() { Id = 4 });
		batcher.SubmitJob(new() { Id = 5 });

		var result = await batchJob1;

		batcher.Jobs.Should().HaveCount(5);
	}


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
