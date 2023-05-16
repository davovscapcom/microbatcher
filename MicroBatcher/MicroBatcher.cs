using System.Timers;
using BatchProcessor;
using MicroBatcher.Constants;

namespace MicroBatcher;

public class MicroBatcher : IMicroBatcher
{
	/// <summary>
	/// List of Jobs to be processed in batches.
	/// </summary>
	public List<BatchJob> Jobs { get; set; } = new List<BatchJob>();

	/// <summary>
	/// Maximum size of each batch.
	/// </summary>
	public uint BatchSize { get; private set; } = 5;

	/// <summary>
	/// A timer for tracking when to send the next batch for processing.
	/// </summary>
	public System.Timers.Timer BatchInterval { get; private set; } = new();

	/// <summary>
	/// If MicroBatcher is shutting down, it will not accept any more jobs, and will continue
	/// batching and processing the existing jobs.
	/// </summary>
	public bool IsShuttingDown { get; private set; }

	/// <summary>
	/// The ID of the batch currently being populated with jobs.
	/// </summary>
	private Guid CurrentBatchId { get; set; } = Guid.NewGuid();

	/// <summary>
    /// This map contains each unresolved JobID, and it's associated JobResult.
    /// 
    /// Once a batch is finished processing, the TaskCompletionSource for each job in the
    /// processed batch populated with the result from the Batch Processor. This resolves 
    /// the original Task returned from SubmitJob, providing the JobResult to the original
    /// caller.
    /// </summary>
	private readonly Dictionary<int, TaskCompletionSource<JobResult>> JobResultMap = new();

	private IBatchProcessor BatchProcessor { get; set; } = new BatchProcessor.BatchProcessor();


	/// <summary>
	/// Creates a new instance of MicroBatcher.
	/// 
	/// MicroBatcher will be configured using environment variables, or fallback to default
	/// configuration if no environment variables are available.
	/// </summary>
	public MicroBatcher()
	{
		if (uint.TryParse(Environment.GetEnvironmentVariable(EnvironmentVariables.BatchSize), out uint batchSize))
			BatchSize = batchSize;

		if (double.TryParse(Environment.GetEnvironmentVariable(EnvironmentVariables.BatchInterval), out double batchInterval))
			BatchInterval.Interval = batchInterval;
	}


	/// <summary>
	/// Create a new instance of MicroBatcher using configuration specified in 'config'.
    /// 
    /// The configuration passed as an argument will override any config specified by
    /// environment variables.
	/// </summary>
	/// <param name="config"></param>
	public MicroBatcher(MicroBatcherConfig config) : this()
	{
		// TODO: Validate config

		BatchSize = (config.BatchSize > 0) ? config.BatchSize : 1;

		if (config.BatchProcessingIntervalMilliseconds > 0)
		{
			BatchInterval = new(config.BatchProcessingIntervalMilliseconds);
			BatchInterval.Elapsed += new ElapsedEventHandler(OnBatchProcessTimerElapsed);
			BatchInterval.Start();
		}
	}

	/// <summary>
    /// Submit a job, which will be processed at the next configured interval.
    /// </summary>
    /// <param name="job">The job to be processed. Must have a unique ID.</param>
    /// <returns>Job Result task that will resolve once the batch containing this
    /// job is processed.</returns>
	public Task<JobResult>? SubmitJob(Job job)
	{
		if (IsShuttingDown)
			return null;

		BatchJob batchJob = (BatchJob)job;
		batchJob.BatchId = CurrentBatchId;
		Jobs.Add(batchJob);

		var tcs = new TaskCompletionSource<JobResult>();
		JobResultMap.Add(job.Id, tcs);

		if(IsBatchThresholdReached()) 
			SubmitBatchForProcessing(CurrentBatchId);

		return tcs.Task;
	}

	/// <summary>
	/// Shutdown will return once all submitted Jobs have finished processing.
	/// </summary>
	public void Shutdown() {
		IsShuttingDown = true;
	}

	#region Private Methods
	/// <summary>
	/// Submits all jobs within the current batch for processing.
	/// TODO: Move this into it's own service
	/// </summary>
	private async void SubmitBatchForProcessing(Guid batchId)
	{
		Console.WriteLine($"Submitting batch for processing");
		if (Jobs.Count <= 0 || batchId == Guid.Empty)
			return;

		IEnumerable<Job> batch = Jobs
			.Where(job => job.BatchId == batchId)
			.Select(batchJob => batchJob.Job);

		var result = await BatchProcessor.ProcessBatchAsync(batch);

		foreach (JobResult jobResult in result)
		{
			TaskCompletionSource<JobResult>? tcs;
			if (JobResultMap.TryGetValue(jobResult.JobId, out tcs))
			{
				tcs.SetResult(jobResult);
				JobResultMap.Remove(jobResult.JobId);
			}
			else
				continue;
		}
	}


	private void OnBatchProcessTimerElapsed(object? source, ElapsedEventArgs e)
	{
		if (!Jobs.Any())
			return;
		SubmitBatchForProcessing(CurrentBatchId);
	}


	private bool IsBatchThresholdReached() => 
		Jobs.Select(job => job.BatchId == CurrentBatchId).Count() >= BatchSize;
	#endregion
}
