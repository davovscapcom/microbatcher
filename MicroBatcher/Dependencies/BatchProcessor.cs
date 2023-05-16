using MicroBatcher;

namespace BatchProcessor;

public class BatchProcessor : IBatchProcessor
{
	public async Task<List<JobResult>> ProcessBatchAsync(IEnumerable<Job> batch)
	{
		// Simulate processing
		await Task.Delay(TimeSpan.FromSeconds(2));

		var results = new List<JobResult>();
		foreach(var job in batch) 
		{
			results.Add(new()
			{
				JobId = job.Id,
				Status = JobResultStatus.PROCESSED
			});
		}

		return results;
	}
}
