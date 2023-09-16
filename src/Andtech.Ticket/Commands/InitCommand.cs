using Andtech.Common;
using Andtech.Ticket.Core;
using CommandLine;
using static Crayon.Output;

namespace Andtech.Ticket
{
	public class InitCommand
	{

		[Verb("init", HelpText = "Initialize this repository.")]
		public class Options : BaseOptions
		{
		}

		public static async Task OnParseAsync(Options options)
		{
			var repositoryActual = await Repository.LoadAsync(Session.Instance.Config, fetchMissingData: false);

			var hasUserID = repositoryActual.User.Id.HasValue;
			var hasUserName = !string.IsNullOrEmpty(repositoryActual.User.Name);
			var hasUserDisplayName = !string.IsNullOrEmpty(repositoryActual.User.DisplayName);
			var hasProjectID = repositoryActual.ProjectID.HasValue;
			var hasProjectUrl = !string.IsNullOrEmpty(repositoryActual.ProjectUrl);

			var checkmark = Green("✓");
			var x = Red("✘");
			Console.WriteLine($"{(hasUserID ? checkmark : x)} User ID");
			Console.WriteLine($"{(hasUserName? checkmark : x)} User Name");
			Console.WriteLine($"{(hasUserDisplayName ? checkmark : x)} User Display Name");
			Console.WriteLine($"{(hasProjectID ? checkmark : x)} Project ID");
			Console.WriteLine($"{(hasProjectUrl ? checkmark : x)} Project URL");

			if (!hasUserID || !hasUserDisplayName || !hasUserDisplayName || !hasProjectID || !hasProjectUrl)
			{
				var repositoryExpected = await Session.Instance.GetRepositoryAsync(fetchMissingData: true);

				Console.WriteLine();
				Console.WriteLine("Run the following:");
				if (!hasUserID)
				{
					Console.WriteLine($"	git config --global ticket.userid {repositoryExpected.User.Id}");
				}
				if (!hasUserName)
				{
					Console.WriteLine($"	git config --global ticket.username {repositoryExpected.User.Name}");
				}
				if (!hasUserDisplayName)
				{
					Console.WriteLine($"	git config --global ticket.displayname {repositoryExpected.User.DisplayName}");
				}
				if (!hasProjectID)
				{
					Console.WriteLine($"	git config ticket.projectid {repositoryExpected.ProjectID}");
				}
				if (!hasProjectUrl)
				{
					Console.WriteLine($"	git config ticket.projecturl {repositoryExpected.ProjectUrl}");
				}
			}
		}
	}
}
