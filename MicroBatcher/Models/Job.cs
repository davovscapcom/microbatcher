using System.Security;
using System.Threading.Tasks;

namespace MicroBatcher;

/// <summary>
/// Base Job Class
/// </summary>
public class Job
{
	public int Id { get; set; }
	public Task JobTask { get; set; }
}
