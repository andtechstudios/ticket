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
			var repositoryExpected = await Session.Instance.GetRepositoryAsync();

			var hasUserID = repositoryActual.UserID.HasValue;
			var hasUserName = !string.IsNullOrEmpty(repositoryActual.UserName);
			var hasProjectID = repositoryActual.ProjectID.HasValue;

			var checkmark = Green("✓");
			var x = Red("✘");
			Console.WriteLine($"User ID: " + (hasUserID ? checkmark : x));
			Console.WriteLine($"User Name: " + (hasUserName ? checkmark : x));
			Console.WriteLine($"Project ID: " + (hasProjectID ? checkmark : x));

			if (!hasUserID || !hasUserName || !hasProjectID)
			{
				Console.WriteLine();
				Console.WriteLine("Run the following:");
			}
			if (!hasUserID)
			{
				Console.WriteLine($"	git config --global ticket.userid {repositoryExpected.UserID}");
			}
			if (!hasUserName)
			{
				Console.WriteLine($"	git config ticket.username {repositoryExpected.UserName}");
			}
			if (!hasProjectID)
			{
				Console.WriteLine($"	git config ticket.projectid {repositoryExpected.ProjectID}");
			}
		}
	}
}
