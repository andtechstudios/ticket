
using Andtech.Common;

namespace Andtech.Ticket.Core
{

    public class Session
	{
		public Config Config { get; set; }

		internal static Session Instance { get; set; }

		public async Task<Repository> GetRepositoryAsync(bool fetchMissingData = false)
		{
			var repository = await Repository.LoadAsync(Config, fetchMissingData: fetchMissingData);
			if (!repository.IsConfigured)
			{
                throw new TicketConfigurationException("Repository not configured.");

            }

            return repository;
		}
	}
}
