using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sid.Parse.TextPatternParser
{
	public static class ConversionFunctions
	{
		public static byte[] ConvertDataUrlToBinary(string url)
		{
			var parser = new Parser(null);

			Options options = new Options(null);
			options.CaseSensitive = true;

			// The main statement list
			StatementList mainStatements = new StatementList(null);
			mainStatements.Add(AggregateComparisons.WildcardComparison("data:*/*;base64,", null, options));

			// Capture the remaining string
			var capturing = new Dictionary<string, Capture>();
			var capture = new Capture(null);
			capturing.Add("base64string", capture);
			mainStatements.Add(capture);
			capture.Comparison = new AdvanceToTheEnd(null).AsComparisonWithAdvance();

			string replaceWith = "'base64string'";
			var stateList = new List<State>();
			string b64String = parser.Replace(url, replaceWith, mainStatements, capturing, stateList);

			var binData = Convert.FromBase64String(b64String);
			return binData;
		}
	}
}
