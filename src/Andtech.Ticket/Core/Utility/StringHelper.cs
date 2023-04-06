namespace Andtech.Ticket
{
	internal class StringHelper
	{

		public static string ToSentenceCase(string input)
		{
			input = input.Trim();
			var firstCharacter = char.ToUpper(input[0]);
			input = string.Concat(firstCharacter, input.Substring(1));

			return input;
		}
	}
}