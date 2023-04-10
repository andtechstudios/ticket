namespace Andtech.Ticket
{

	internal static class Macros
	{

		public static string TerminalLink(string caption, string url) => $"\u001B]8;;{url}\a{caption}\u001B]8;;\a";

		public static int ParseIssue(string text)
        {
            var iidString = text.TrimStart('#');
            return int.Parse(iidString);
        }
	}
}
