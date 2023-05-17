using System.Collections.Concurrent;
using System.Timers;
using BatchProcessor;
using MicroBatcher.Constants;
using Microsoft.Extensions.Logging;

namespace MicroBatcher;

/// <summary>
/// Microbatcher is designed to group submitted jobs together, and coordinate their
/// processing intervals.
/// </summary>
public class MicroBatcher : IMicroBatcher
{
	/// <summary>
	/// Thread safe queue of BatchJobs.
	/// </summary>
	public readonly ConcurrentQueue<KeyValuePair<int, BatchJob>> JobQueue = new();

	/// <summary>
	/// Maximum size of each batch.
	/// </summary>
	public uint BatchSize { get; set; } = Defaults.BatchSize;

	/// <summary>
	/// A timer for tracking when to send the next batch for processing.
    /// This timer will only be started if the interval is greater than 1 millisecond
	/// </summary>
	public System.Timers.Timer BatchInterval { get; private set; } = new(Defaults.BatchIntervalInMilliseconds);

	/// <summary>
	/// If MicroBatcher is shutting down, it will not accept any more jobs, and will continue
	/// batching and processing the existing jobs.
	/// </summary>
	public bool IsShuttingDown { get; private set; }

	private readonly ILogger _logger;
	private readonly IBatchProcessor _batchProcessor;


	public MicroBatcher(ILogger<MicroBatcher> logger, IBatchProcessor batchProcessor)
	{
		_logger = logger;
		_batchProcessor = batchProcessor;

		if (uint.TryParse(Environment.GetEnvironmentVariable(EnvironmentVariables.BatchSize), out uint batchSize))
			BatchSize = batchSize;

		if (double.TryParse(Environment.GetEnvironmentVariable(EnvironmentVariables.BatchInterval), out double batchInterval))
			BatchInterval.Interval = batchInterval;
		
		if (BatchInterval.Interval > 1) {
			_logger.LogTrace("Batch interval timer started");
			BatchInterval.Elapsed += new ElapsedEventHandler(OnBatchProcessTimerElapsed);
			BatchInterval.Start();
		}
	}


	/// <summary>
    /// Submit a job, which will be processed at the next configured interval.
    /// </summary>
    /// <param name="job">The job to be processed.</param>
    /// <returns>Job Result task that will resolve once the batch containing this
    /// job is processed.</returns>
	public Task<JobResult>? SubmitJob(Job job)
	{
		_logger.LogTrace("Job submitted");

		if (IsShuttingDown)
			throw new InvalidOperationException("Jobs cannot be submitted while microbatcher is shutting down.");

		BatchJob batchJob = new(job, new TaskCompletionSource<JobResult>());
		JobQueue.Enqueue(new(job.Id, batchJob));

		if(JobQueue.Count >= BatchSize) 
			SubmitBatchForProcessing();

		return batchJob.JobTaskCompletionSource.Task;
	}

	/// <summary>
	/// Shutdown will return once all submitted Jobs have finished processing.
	/// </summary>
	public void Shutdown() {
		_logger.LogInformation(
			"Shutting down MicroBatcher. No further jobs will be accepted. Enqueued jobs will be processed.");
		IsShuttingDown = true;
		ProcessAllRemainingJobs();
	}

	#region Private Methods
	private void OnBatchProcessTimerElapsed(object? source, ElapsedEventArgs e)
	{
		_logger.LogInformation("Batch processing timer elapsed.");
		if (!JobQueue.Any())
			return;
		SubmitBatchForProcessing();
	}

	/// <summary>
	/// Attempts to submmit the next 'BatchSize' jobs for processing.
	///
        /// If the job queue contains fewer jobs than the configured batch size,
        /// then all jobs within the job queue will be sent for processing.
	/// </summary>
	private async void SubmitBatchForProcessing()
	{
		if (JobQueue.IsEmpty)
			return;
			
		_logger.LogInformation("Submitting batch for processing");

		// Continuously dequeue jobs from our JobQueue
		var batch = new List<KeyValuePair<int, BatchJob>>();
		while (JobQueue.TryDequeue(out var job))
		{
			batch.Add(job);
			if (batch.Count >= BatchSize)
				break;
		}

		List<JobResult> batchProcessorResult = new();
		try 
		{
			batchProcessorResult = await _batchProcessor.ProcessBatchAsync(batch.Select(j => j.Value.Job));
		} 
		catch (Exception ex)
		{
			_logger.LogError("Exception thrown by batch processor: ", ex);
		}

		foreach (JobResult jobResult in batchProcessorResult)
		{
			var localJob = batch.Where(j => j.Key == jobResult.JobId).FirstOrDefault();
			if (localJob.Value != null) {
				localJob.Value.JobTaskCompletionSource.SetResult(jobResult);
			}
			else
				continue;
		}
	}

	/// <summary>
	/// This method will continuously run until all jobs from the job queue have been
	/// processed.
	/// </summary>
	private void ProcessAllRemainingJobs()
	{
		while(JobQueue.Any())
		{
			SubmitBatchForProcessing();
		}
	}
	#endregion
}
