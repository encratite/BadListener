namespace BadListener.Extension
{
	class StringReplacement
	{
		private string _Target;
		private string _Replacement;

		public StringReplacement(string target, string replacement)
		{
			_Target = target;
			_Replacement = replacement;
		}

		public string Execute(string input)
		{
			string output = input.Replace(_Target, _Replacement);
			return output;
		}
	}
}
