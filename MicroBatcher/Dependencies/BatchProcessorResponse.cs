namespace BatchProcessor;

public struct BatchProcessorResponse
{
	public BatchProcessorResponseCode Code;
}

public enum BatchProcessorResponseCode
{
	ERROR,
	SUCCESS
}
