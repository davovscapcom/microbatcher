namespace MicroBatcher;

public class BatchJob
{
	public Job Job { get; set; }
	public TaskCompletionSource<JobResult> JobTaskCompletionSource { get; set; }
	public Guid BatchId { get; set; }
	public BatchJob(Job job, TaskCompletionSource<JobResult> tcs)
	{
		Job = job;
		JobTaskCompletionSource = tcs;
	}
}
