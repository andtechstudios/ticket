#nullable enable

namespace Andtech.Ticket
{
	public class Config
	{
		public static Config Instance { get; set; }

		public List<Host> hosts { get; set; }
	}
}
