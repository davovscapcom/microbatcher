using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace MicroBatcher.Tests;

[Collection("Sequential")]
public class MicroBatcherTest 
{
	private readonly ILogger<MicroBatcher> _logger;
	private readonly MicroBatcher _batcher;

	public MicroBatcherTest()
	{
		using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
		_logger = loggerFactory.CreateLogger<MicroBatcher>();
		_batcher = new(_logger);
	}

	[Fact]
	public void MicroBatcher_UponCreation_ShouldContainNoJobs()
	{
		_batcher.Jobs.Should().HaveCount(0);
	}

	[Fact]
	public void SubmitJob_ValidJobGiven_ShouldAddToJobsList()
	{
		_batcher.SubmitJob(new Job());
		_batcher.Jobs.Should().HaveCount(1);
	}

	[Fact]
	public async void SubmitJob_JobThresholdExceeded_ShouldProcessJobs()
	{
		_batcher.BatchSize = 2;
		var jobOperation = _batcher.SubmitJob(new Job() { Id = 1 });
		_batcher.SubmitJob(new Job() { Id = 2 });

		JobResult result = await jobOperation;

		result.Should().NotBeNull();
		result.Status.Should().Equals(JobResultStatus.PROCESSED);
	}


	// Test that microbatcher doesn't submit anything for processing if there is nothing to process


	// Test file for Submitting Jobs to be processed
	// Jobs that exist within our list are submitted to the processor at the configured interval.
}
