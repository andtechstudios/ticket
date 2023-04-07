using Andtech.Common;
using Andtech.Ticket.Core;
using CommandLine;
using GitLabApiClient.Models.Issues.Requests;
using static Crayon.Output;

namespace Andtech.Ticket
{
	public class CreateCommand
	{

		[Verb("create", isDefault: true, aliases: new string[] { "new", "open" }, HelpText = "Create new issues.")]
		public class Options : BaseOptions
		{
			[Option("me", HelpText = "Assign all issues to yourself")]
			public bool AssignToMe { get; set; }
			[Option("bulk", HelpText = "Buffer tasks and upload them all at once.")]
			public bool BulkMode { get; set; }
		}

		public static async Task OnParseAsync(Options options)
		{
			Repository repository = null;

			// Begin program
			var requests = new List<CreateIssueRequest>();
			while (true)
			{
				Console.Write("> ");
				var line = Console.ReadLine();
				if (string.IsNullOrEmpty(line))
				{
					break;
				}

				try
				{
					var task = TodoMD.Task.Parse(line);
					if (options.AssignToMe)
					{
						task.Assignees.Add("me");
					}

					if (options.BulkMode)
					{
						if (repository is null)
						{
							repository = await Session.Instance.GetRepositoryAsync();
						}

						var request = await ToRequestAsync(task);
						requests.Add(request);
					}
					else
					{
						if (repository is null)
						{
							repository = await Session.Instance.GetRepositoryAsync();
						}
						var request = await ToRequestAsync(task);

						Log.WriteLine("Uploading to GitLab...", ConsoleColor.Cyan);
						await UploadAsync(request);
					}
				}
				catch (Exception ex)
				{
					Log.Error.WriteLine("An error occurred when creating the issue.", ConsoleColor.Red);
					Log.Error.WriteLine(ex, Verbosity.verbose);
				}
			}

			if (options.BulkMode)
			{
				Log.WriteLine("Uploading to GitLab...", ConsoleColor.Cyan);

				foreach (var request in requests)
				{
					await UploadAsync(request);
				}
			}

			async Task<CreateIssueRequest> ToRequestAsync(TodoMD.Task task)
			{
				var assignees = new List<int>(1);
				foreach (var assignee in task.Assignees)
				{
					assignees.Add(await repository.GetUserIdAsync(assignee));
				}

				return new CreateIssueRequest(StringHelper.ToSentenceCase(task.Title))
				{
					Description = StringHelper.ToSentenceCase(task.Description),
					DueDate = task.DueDate?.ToString() ?? string.Empty,
					Labels = task.Tags,
					Assignees = assignees,
				};
			}

			async Task UploadAsync(CreateIssueRequest request)
			{
				var issue = await repository.Client.Issues.CreateAsync(repository.ProjectID, request);
				var issueText = Macros.TerminalURL($"#{issue.Iid}", issue.WebUrl);
				Console.WriteLine(Bright.Green($"Issue {issueText} was created successfully!"));
			}
		}
	}
}
