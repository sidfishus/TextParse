using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sid.Log;

namespace Sid.Parse.TextPatternParser
{
	public static class Helpers
	{

		private static string GetSingleValue_ReplaceFunc(
			RunState runState,
			string input,
			int afterPos)
		{
			int beginPos = (int)runState.GetVariable("ValueBegin");
			int afterValuePos= (int)runState.GetVariable("ValueEnd");
			return input.Substring(beginPos,afterValuePos-beginPos);
		}

		// This relies on there being the variables 'ValueBegin' and 'ValueEnd'
		public static string GetSingleValue(
			string input,
			Parser parser,
			IComparisonWithAdvance comp,
			ILog log)
		{
			// Skip to the end
			var statementList = new StatementList(log);
			statementList.Add(comp);
			statementList.Add(new AdvanceToTheEnd(log));

			int numAmount;
			string singleValue=parser.Extract(input, null, statementList, null, null, out numAmount, GetSingleValue_ReplaceFunc);
			if (numAmount == 1)
			{
				return singleValue;
			}
			return null;
		}
	}
}
