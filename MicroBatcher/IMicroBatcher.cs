namespace MicroBatcher;

interface IMicroBatcher
{
	public Task<JobResult>? SubmitJob(Job job);
	public void Shutdown();
}
