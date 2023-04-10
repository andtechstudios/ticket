using Andtech.Ticket.Core;
using CommandLine;
using static Crayon.Output;

namespace Andtech.Ticket
{
	public class InitCommand
	{

		[Verb("init", HelpText = "Initialize the Ticket integration for this repository.")]
		public class Options : BaseOptions
		{
		}

		public static async Task OnParseAsync(Options options)
		{
			var repositoryActual = await Repository.LoadAsync(Session.Instance.Config, fetchMissingData: false);
            var repositoryExpected = await Repository.LoadAsync(Session.Instance.Config, fetchMissingData: true);

            var commands = new List<string>();
            void Check<T>(T actual, T expected, string key, string displayName)
            {
				if (actual.Equals(expected))
                {
                    Console.WriteLine(Bright.Green("✓") + $" {displayName}");
                }
				else
                {
                    commands.Add($"{key} {expected}");
                    Console.WriteLine(Bright.Red("✘") + $" {displayName}");
                }
            }

            Check(repositoryActual.User.Id, repositoryExpected.User.Id, "ticket.userid", "User ID");
            Check(repositoryActual.User.Name, repositoryExpected.User.Name, "ticket.username", "User Name");
            Check(repositoryActual.User.DisplayName, repositoryExpected.User.DisplayName, "ticket.displayname", "User Display Name");
            Check(repositoryActual.ProjectID, repositoryExpected.ProjectID, "ticket.projectid", "Project ID");
            Check(repositoryActual.ProjectUrl, repositoryExpected.ProjectUrl, "ticket.projecturl", "Project URL");

			if (commands.Count > 0)
            {
                Console.WriteLine("Run the following shell command(s):");
                foreach (var command in commands)
                {
                    Console.WriteLine($"	git config {command}");
                }
            }
		}
	}
}
