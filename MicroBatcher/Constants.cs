namespace MicroBatcher.Constants;

public static class Defaults 
{
	public static readonly uint BatchSize = 32;
	public static readonly double BatchIntervalInMilliseconds = 20_000;
}

public static class EnvironmentVariables
{
	public static readonly string BatchSize = "MICROBATCHER__MAXBATCHSIZE";
	public static readonly string BatchInterval = "MICROBATCHER__BATCHINTERVAL";
}

