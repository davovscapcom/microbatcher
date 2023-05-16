namespace MicroBatcher;

public class JobResult
{
	public int JobId { get; set; }
	public JobResultStatus Status { get; set; }
}

public enum JobResultStatus
{
	SUBMITTED,
	QUEUED,
	PROCESSING,
	PROCESSED,
	FAULTED
}
