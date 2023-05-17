namespace MicroBatcher.Tests;

using Constants;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

[Collection("Sequential")]
public class MicroBatcherDefaultConfigTest
{
	private readonly ILogger<MicroBatcher> _logger;
	private readonly MicroBatcher _batcher;

	public MicroBatcherDefaultConfigTest()
	{
		using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
		_logger = loggerFactory.CreateLogger<MicroBatcher>();
		_batcher = new(_logger);
	}

	[Fact]
	public void MicroBatcher_DefaultBatchSize()
	{
		_batcher.BatchSize.Should().Be(Defaults.BatchSize);
	}

	[Fact]
	public  void MicroBatcher_DefaultBatchInterval()
	{
		_batcher.BatchInterval.Interval.Should().Be(Defaults.BatchIntervalInMilliseconds);
	}
}

[Collection("Sequential")]
public class MicroBatcherEnvironentVariableConfigTest
{
	private readonly ILogger<MicroBatcher> _logger;
	private readonly MicroBatcher _batcher;

	public MicroBatcherEnvironentVariableConfigTest()
	{
		using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
		_logger = loggerFactory.CreateLogger<MicroBatcher>();
		_batcher = new(_logger);
	}

	[Theory]
	[InlineData(1)]
	[InlineData(1_000_000)]
	[InlineData(uint.MaxValue)]
	public void MicroBatcher_ShouldAllowConfigurationOf_BatchSize_UsingEnvironmentVariables(uint batchSize)
	{
		Environment.SetEnvironmentVariable(
			EnvironmentVariables.BatchSize, batchSize.ToString());
		MicroBatcher batcher = new(_logger);

		batcher.BatchSize.Should().Be(batchSize);
	}

	[Theory]
	[InlineData(2000)]
	[InlineData(1_000_000)]
	[InlineData(uint.MaxValue)]
	public void MicroBatcher_ShouldAllowConfigurationOf_BatchInterval_UsingEnvironmentVariables(double intervalInMilliseconds)
	{
		Environment.SetEnvironmentVariable(
			EnvironmentVariables.BatchInterval, intervalInMilliseconds.ToString());
		MicroBatcher batcher = new(_logger);

		batcher.SubmitJob(new Job() { Id = 0 });
		batcher.SubmitJob(new Job() { Id = 1 });
		batcher.SubmitJob(new Job() { Id = 2 });
		batcher.SubmitJob(new Job() { Id = 3 });
		batcher.SubmitJob(new Job() { Id = 4 });
		batcher.SubmitJob(new Job() { Id = 5 });
		batcher.SubmitJob(new Job() { Id = 6 });

		batcher.BatchInterval.Should().NotBeNull();
		batcher.BatchInterval.Interval.Should().Be(intervalInMilliseconds);
	}
}
