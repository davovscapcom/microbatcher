using MicroBatcher;

namespace BatchProcessor;

public interface IBatchProcessor
{
	public Task<List<JobResult>> ProcessBatchAsync(IEnumerable<Job> Jobs);
}
