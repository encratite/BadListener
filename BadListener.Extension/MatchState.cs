using System.Text.RegularExpressions;

namespace BadListener.Extension
{
	class MatchState
	{
		private Regex _Regex;
		private Match _Match;

		public MatchState(string pattern)
		{
			_Regex = new Regex(pattern);
		}

		public bool Matches(string input)
		{
			_Match = _Regex.Match(input);
			return _Match.Success;
		}

		public string Group(int index)
		{
			return _Match.Groups[index].Value;
		}
	}
}
