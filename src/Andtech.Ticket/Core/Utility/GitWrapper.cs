using System.Diagnostics;

namespace Andtech.Ticket
{

	public static class GitWrapper
	{

		public static string Git(string args) => Git(args.Split(" ", StringSplitOptions.RemoveEmptyEntries));

		public static string Git(params string[] args) => Git((IEnumerable<string>)args);

		public static string Git(IEnumerable<string> args)
		{
			var arguments = string.Join(" ", args.Select(x => $"\"{x}\""));
			var process = new Process()
			{
				StartInfo = new ProcessStartInfo("git", arguments)
				{
					UseShellExecute = false,
					RedirectStandardOutput = true
				}
			};
			process.Start();

			var outputReader = process.StandardOutput;
			process.WaitForExit();

			return outputReader.ReadToEnd();
		}
	}
}
