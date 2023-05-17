namespace MicroBatcher.Tests;

using BatchProcessor;
using Constants;
using FluentAssertions;
using Microsoft.Extensions.Logging;

[Collection("Sequential")]
public class MicroBatcherDefaultConfigTest
{
	private readonly ILogger<MicroBatcher> _logger;
	private readonly IBatchProcessor _batchProcessor;
	private readonly MicroBatcher _batcher;

	public MicroBatcherDefaultConfigTest()
	{
		using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
		_logger = loggerFactory.CreateLogger<MicroBatcher>();
		_batchProcessor = new BatchProcessor();
		_batcher = new(_logger, _batchProcessor);
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
	private readonly IBatchProcessor _batchProcessor;
	private readonly MicroBatcher _batcher;

	public MicroBatcherEnvironentVariableConfigTest()
	{
		using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
		_logger = loggerFactory.CreateLogger<MicroBatcher>();
		_batchProcessor = new BatchProcessor();
		_batcher = new(_logger, _batchProcessor);
	}

	[Theory]
	[InlineData(1)]
	[InlineData(1_000_000)]
	[InlineData(uint.MaxValue)]
	public void MicroBatcher_ShouldAllowConfigurationOf_BatchSize_UsingEnvironmentVariables(uint batchSize)
	{
		Environment.SetEnvironmentVariable(
			EnvironmentVariables.BatchSize, batchSize.ToString());
		MicroBatcher batcher = new(_logger, _batchProcessor);

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
		MicroBatcher batcher = new(_logger, _batchProcessor);

		batcher.BatchInterval.Should().NotBeNull();
		batcher.BatchInterval.Interval.Should().Be(intervalInMilliseconds);
	}
}
