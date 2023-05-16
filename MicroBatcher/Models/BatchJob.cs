namespace MicroBatcher;

public class BatchJob
{
	public Job Job { get; set; }
	public Guid BatchId { get; set; }
	public BatchJob(Job job)
	{
		Job = job;
	}

	public static explicit operator BatchJob(Job j) => new(j);
}
