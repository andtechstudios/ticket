namespace Andtech.Ticket
{

	internal static class Macros
	{

		public static string TerminalURL(string caption, string url) => $"\u001B]8;;{url}\a{caption}\u001B]8;;\a";
	}
}
