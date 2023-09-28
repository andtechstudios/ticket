using System.Drawing;
using GitLabApiClient.Models.Issues.Responses;
using GitLabApiClient.Models.Projects.Responses;
using static Crayon.Output;

namespace Andtech.Ticket
{

	public class IssueWriter
	{
		public string DefaultDotSymbol { get; set; } = "●";
		public string DefaultNotSymbol { get; set; } = "○";
		public bool UseColor { get; set; }

		private readonly IList<Label> labels;

		public IssueWriter(IEnumerable<Label> labels)
		{
			this.labels = labels.ToList();
		}

		public void Print(IEnumerable<Issue> issues, bool alignLabels = false)
		{
			int CountDigits(int num) => CountDigitsString(num.ToString());
			int CountDigitsString(string text) => text.Count(x => char.IsDigit(x));

			var iidMaxDigits = CountDigits(issues.Max(x => x.Iid));
			var dotMaxDigits = issues.Max(x => x.Labels.Count);

			var singleColumnMode = issues.All(x => x.Labels.Count == 1) && !alignLabels;
			var usedLabels = labels.Where(x => issues.Any(y => y.Labels.Contains(x.Name)));

			foreach (var issue in issues.OrderBy(x => x.Iid))
			{
				Print(issue);
			}

			void Print(Issue issue)
			{
				Console.ForegroundColor = ConsoleColor.Cyan;

				var digits = CountDigits(issue.Iid);
				var paddingLength = Math.Max(0, iidMaxDigits - digits);
				var padding = string.Concat(Enumerable.Repeat(" ", paddingLength));
				var caption = $"#{issue.Iid}";

				var text = Macros.TerminalLink(caption, issue.WebUrl);
				text = padding + text;

				var iidColumn = $"{text} ";
				Console.Write(iidColumn);
				Console.ResetColor();

				if (usedLabels.Any())
				{
					if (singleColumnMode)
					{
						PrintDotStacked();
					}
					else if (alignLabels)
					{
						PrintDotsAligned();
					}
					else
					{
						PrintDotStacked();
					}

					void PrintDotStacked()
					{
						var dots = new List<string>();
						foreach (var label in issue.Labels)
						{
							var match = usedLabels.First(x => x.Name == label);
							var dot = DefaultDotSymbol;

							if (UseColor)
							{
								var foregroundColor = ColorTranslator.FromHtml(match.Color);
								dots.Add(Rgb(foregroundColor.R, foregroundColor.G, foregroundColor.B).Text(dot));
							}
							else
							{
								dots.Add(dot);
							}
						}

						var message = string.Concat(dots);
						Console.Write(message);
						for (int i = 0; i < dotMaxDigits - dots.Count; i++)
						{
							Console.Write(" ");
						}
					}

					void PrintDotsAligned()
					{
						foreach (var label in usedLabels)
						{
							var hasMatch = issue.Labels.Any(x => x == label.Name);
							string dot = " ";
							if (hasMatch)
							{
								dot = DefaultDotSymbol;
							}
							else
							{
								dot = DefaultNotSymbol;
							}
							var foregroundColor = ColorTranslator.FromHtml(label.Color);
							var message = Rgb(foregroundColor.R, foregroundColor.G, foregroundColor.B).Text(dot);
							Console.Write(message);
						}
					}

					Console.Write(" ");
				}

				var titleColumn = $"{issue.Title}";
				Console.Write(titleColumn);
				if (!string.IsNullOrEmpty(issue.Assignee?.Username))
				{
					Console.Write(Bright.Black($" @{issue.Assignee.Username}"));
				}
				Console.WriteLine();
			}
		}
	}
}
