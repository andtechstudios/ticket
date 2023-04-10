using Andtech.Common;
using Andtech.Ticket.Core;
using CommandLine;
using GitLabApiClient.Models.Issues.Requests;
using static Crayon.Output;

namespace Andtech.Ticket
{

	public class ViewCommand
	{

		[Verb("view", HelpText = "View issue details.")]
		public class Options : BaseOptions
		{
			[Value(0, Min = 1, HelpText = "List of issues to open.")]
			public IEnumerable<string> Issues { get; set; }
			[Option('w', "web")]
			public bool OpenInWeb { get; set; }
		}

		public static async Task OnParseAsync(Options options)
		{
			if (options.OpenInWeb)
			{
                foreach (var issueString in options.Issues)
                {
                    var iid = Macros.ParseIssue(issueString);

                    var repository = await Session.Instance.GetRepositoryAsync();
                    var url = Path.Combine(repository.ProjectUrl, "issues", iid.ToString());
                    ShellUtility.OpenBrowser(url);
                }
            }
			else
            {
                var repository = await Session.Instance.GetRepositoryAsync();
                foreach (var issueString in options.Issues)
                {
                    try
                    {
                        var iid = Macros.ParseIssue(issueString);
                        var issue = await repository.Client.Issues.GetAsync(repository.ProjectID, iid);

                        var issueText = Bright.Cyan(Macros.TerminalLink($"#{issue.Iid}", issue.WebUrl));
                        Log.WriteLine($"{issueText} {Bold(issue.Title)}");
                        if (!string.IsNullOrEmpty(issue.Description))
                        {
                            Log.Write($"{issue.Description}");
                            Log.WriteLine();
                        }
                    }
                    catch
                    {
                        Log.Error.WriteLine($"Invalid issue: '{issueString}'", ConsoleColor.Red);
                    }
                }
            }
		}
	}
}
