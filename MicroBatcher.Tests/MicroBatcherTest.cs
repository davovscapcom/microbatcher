using BatchProcessor;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace MicroBatcher.Tests;

[Collection("Sequential")]
public class MicroBatcherTest 
{
	private readonly ILogger<MicroBatcher> _logger;
	private readonly IBatchProcessor _batchProcessor;
	private readonly MicroBatcher _batcher;

	public MicroBatcherTest()
	{
		using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
		_logger = loggerFactory.CreateLogger<MicroBatcher>();
		_batchProcessor = new BatchProcessor.BatchProcessor();
		_batcher = new(_logger, _batchProcessor);
	}

	[Fact]
	public void MicroBatcher_UponCreation_ShouldContainNoJobs()
	{
		_batcher.JobQueue.Should().HaveCount(0);
	}

	[Fact]
	public void SubmitJob_ValidJobGiven_ShouldAddToJobsList()
	{
		_batcher.BatchSize = 10;
		_batcher.SubmitJob(new Job());
		_batcher.JobQueue.Should().HaveCount(1);
	}

	[Fact]
	public async void SubmitJob_JobThresholdMet_ShouldProcessAllJobs()
	{
		_batcher.BatchSize = 2;
		_batcher.BatchInterval.Stop();
		var jobOperation = _batcher.SubmitJob(new Job() { Id = 1 });
		_batcher.SubmitJob(new Job() { Id = 2 });

		JobResult result = await jobOperation;

		result.Should().NotBeNull();
		_batcher.JobQueue.Count.Should().Be(0);
	}

	[Fact]
	public async void SubmitJob_JobThresholdExceeded_ShouldHaveRemainderJobs()
	{
		_batcher.BatchSize = 2;
		_batcher.BatchInterval.Stop();
		_batcher.SubmitJob(new Job() { Id = 1 });
		_batcher.SubmitJob(new Job() { Id = 2 });
		_batcher.SubmitJob(new Job() { Id = 3 });

		_batcher.JobQueue.Count.Should().Be(1);
	}

	[Fact]
	public async void BatchProcessor_Shutdown_WillProcessRemainingJobs()
	{
		_batcher.BatchInterval.Stop();
		_batcher.BatchSize = 5;
		_batcher.SubmitJob(new Job() { Id = 1 });
		_batcher.SubmitJob(new Job() { Id = 2 });
		_batcher.SubmitJob(new Job() { Id = 3 });
		_batcher.Shutdown();

		_batcher.JobQueue.Count().Should().Be(0);
	}
}
