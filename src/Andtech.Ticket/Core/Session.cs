
namespace Andtech.Ticket.Core
{

	public class Session
	{
		public Config Config { get; set; }

		internal static Session Instance { get; set; }

		public async Task<Repository> GetRepositoryAsync(bool fetchMissingData = true)
		{
			return await Repository.LoadAsync(Config, fetchMissingData: fetchMissingData);
		}
	}
}
