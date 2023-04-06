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
			var repository = await Session.Instance.GetRepositoryAsync();
			var repositoryMissing = await Repository.LoadAsync(Session.Instance.Config, fetchMissingData: false);

			var hasUserID = repositoryMissing.UserID > 0;
			var hasProjectID = repositoryMissing.ProjectID > 0;

			var checkmark = Green("✓");
			var x = Red("✘");
			Console.WriteLine($"User ID: " + (hasUserID ? checkmark : x));
			Console.WriteLine($"Project ID: " + (hasProjectID ? checkmark : x));

			if (!hasUserID || !hasProjectID)
			{
				Console.WriteLine();
				Console.WriteLine("Run the following:");
			}
			if (!hasUserID)
			{
				Console.WriteLine($"	git config --global ticket.userid {repository.UserID}");
			}
			if (!hasProjectID)
			{
				Console.WriteLine($"	git config ticket.projectid {repository.ProjectID}");
			}
		}
	}
}
