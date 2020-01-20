using System;
using System.Collections.Generic;
using System.Text;
using Sid;
using Sid.Log;

namespace Sid.Parse.TextPatternParser
{
	public class Parser : HasLogImp
	{

		/*
		public Parser(
		 string pattern)
		{
			StatementList m_Statements = new StatementList();
			ExpressionParser expressionParser = new ExpressionParser(this, pattern);
		}
		*/

		public Parser(
		 ILog log)
		{
			Log = log;
		}

		internal static void UnitTest()
		{
			//const string input2 = "<% if(something)then class.Function Test.test (a , test.func( b) ) & Test.Test & test & \"t & \t\", 1 end if %>";
			const string input = "<% function _ blah msgbox blah end function%>";

			/*
			const string input3 = @"<% End If If (Server.HTMLEncode(Request(""Action"")) = ""Insert"") OR (Session(""strUId"") = strUId AND strBlStatusID = """" AND strBlLevID = ""2"") OR _
					(((strBlStatusID = """" AND strBlLevID = ""2"") OR strBlStatusID = ""2"") AND (strChkProd = ""Y"" OR Session(""strLevelLabour"") = ""250"")) Then %>";
					*/

			//const string input = "<% fp_sQry = \"SELECT ComOrgName FROM tCompany WHERE (ComID = \" & strComID & \")\" %>";
			ILog log = new ConsoleWriteLog();
			//ILog log = null;

			//string input2="<% if(something)then Response.Write test.func(a,b, test.func(a,c)) & something, 1 end if %>";
			//const string input2 = "<% if(something)then class.Function Test.test (a , test.func( b) ) & Test.Test & test & \"t & \t\", 1 end if %>";

			//string replaced = dotNETConversion.AddParenthesisToFunctionCalls(log, input2);
			string replaced = dotNETConversion.WrapFunctionsInScriptBlock(log, input);

			// Make some space
			for (int i = 0; i < 5; ++i)
			{
				Console.WriteLine("{0}", Environment.NewLine);
			}
			Console.WriteLine("original:\t{0}{1}", input, Environment.NewLine);
			Console.WriteLine("replaced:\t{0}", replaced);
		}

		// The difference between replace and extract mode is that extract mode only returns matching text
		public string Extract(
		 string input,
		 string replaceWith,
		 IComparisonWithAdvance mainComparison,
		 Dictionary<string, Capture> capturing,
		 List<State> stateList,
		 out int numMatches,
		 Func<RunState, string, int, string> ReplaceFunc = null)
		{
			return Process(input, replaceWith, mainComparison, capturing, stateList, false, out numMatches, ReplaceFunc);
		}

		public string Replace(
		 string input,
		 string replaceWith,
		 IComparisonWithAdvance mainComparison,
		 Dictionary<string, Capture> capturing,
		 List<State> stateList,
		 out int numMatches,
		 Func<RunState, string, int, string> ReplaceFunc = null)
		{
			return Process(input, replaceWith, mainComparison, capturing, stateList, true, out numMatches, ReplaceFunc);
		}

		public string Replace(
		 string input,
		 string replaceWith,
		 IComparisonWithAdvance mainComparison,
		 Dictionary<string, Capture> capturing,
		 List<State> stateList,
		 Func<RunState, string, int, string> ReplaceFunc = null)
		{
			int numMatches;
			return Process(input, replaceWith, mainComparison, capturing, stateList, true, out numMatches, ReplaceFunc);
		}

		private string Process(
		 string input,
		 string replaceWith,
		 IComparisonWithAdvance mainComparison,
		 Dictionary<string, Capture> capturing,
		 List<State> stateList,
		 bool replace,
		 out int numMatches,
		 Func<RunState,string,int,string> ReplaceFunc=null)
		{
			if ((replaceWith==null && ReplaceFunc == null) || (ReplaceFunc!=null && replaceWith != null))
			{
				throw new Exception(
					"Invalid parameters: must specify exactly one of the replace function or the replace string.");
			}

			// The replaced string
			StringBuilder replaced = new StringBuilder();

			var runState = new RunState();
			numMatches = 0;

			// Iterate the input string character by character
			for (int outerIndex = 0; outerIndex < input.Length;)
			{
				runState.Clear();
				runState.BeginPos = outerIndex;
#if DEBUG
				int dbgInitialOuterIndex = outerIndex;
				if(dbgInitialOuterIndex == 1374)
				{
					Misc.Break();
				}
				string dbgInputPortion = input.Substring(dbgInitialOuterIndex,
				 Math.Min(20, input.Length- dbgInitialOuterIndex));
#endif
				//// All states enabled?
				int afterState=outerIndex;
				bool stateEnabled;
				if (stateList != null)
				{
					for (; ; )
					{
						// If the state becomes enabled, check that directly afterwards we don't encounter a close state
						// Imagine we have a state which opens on '<%' and closes on '%>' and the input string is '<%%> ...'
						// We enable then instantly disable again. We only stop trying to update the state when updating the
						//  state doesn't advance the position
						int curIndex = afterState;
						stateEnabled = UpdateState(stateList, input, afterState, runState, out afterState);
						if (curIndex == afterState)
						{
							// Updating the state did not advance the position - continue with the replace
							break;
						}
					}
				}
				else
				{
					stateEnabled = true;
				}

				// Copy the state string (if any)
				if (afterState != outerIndex)
				{
					string stateString = input.Substring(outerIndex, afterState - outerIndex);
					replaced.Append(stateString);
				}

				// Update the main index
				outerIndex = afterState;

				if (outerIndex < input.Length)
				{
					bool matched = stateEnabled;
					if (stateEnabled)
					{

						// Run the statements beginning from 'afterState'
						int innerIndex = outerIndex;
						int afterMatch;
						matched = mainComparison.CompareAndAdvance(input, innerIndex, 0, runState, out afterMatch);
						if (matched)
						{
							// The begin of the matching string
							runState.BeginPos = outerIndex;

							++numMatches;

							// This is a match! :)
							// Do the replace
							string iterReplaced = ReplaceMatch(replaceWith, ReplaceFunc, capturing, runState, input, afterMatch);
							replaced.Append(iterReplaced);
							LogMatchReplace(
							 input,
							 innerIndex,
							 afterMatch,
							 iterReplaced,
							 capturing);

							// Update the outer index and continue iterating from there
							outerIndex = afterMatch;
						}
					}

					if (!matched)
					{
						if (replace)
						{
							// Match failed: copy this character to the output string and move on to the next one (if this is replace mode)
							replaced.Append(input[outerIndex]);
						}
						++outerIndex;
					}
				}
			}
			return replaced.ToString();
		}

		private void LogMatchReplace(
		 string input,
		 int beginPos,
		 int endPos,
		 string replaced,
		 Dictionary<string, Capture> capturing)
		{
			if (Log != null)
			{
				// For want of a better name
				const string delimiter = "***************************************************************************";
				Log.LogLine(delimiter);
				Log.LogLine(string.Format("Replaced. pos={0}{2}Original:\t{3}{2}Replaced:\t{1}",
				 beginPos,
				 replaced,
				 Environment.NewLine,
				 input.Substring(beginPos, endPos - beginPos)));
				Log.LogLine(delimiter);
				Log.LogLine("Capture:");
				foreach(var capture in capturing)
				{
					Log.LogLine(string.Format("{0} =\t{1}",
					 capture.Key.ToString(),
					 capture.Value.Captured));
				}
				Log.LogLine(delimiter);
			}
		}

		private bool UpdateState(
		 List<State> stateList,
		 string input,
		 int pos,
		 RunState runState,
		 out int afterState)
		{
			// Get the initial state
			bool initialState = true;
			if(Log!=null)
			{
				for (int stateIdx = 0; stateIdx < stateList.Count;  ++stateIdx)
				{
					if (!stateList[stateIdx].IsEnabled)
					{
						initialState = false;
						break;
					}
				}
			}

			afterState = pos;
			bool stateEnabled = true;
			for (int stateIdx = 0; stateIdx < stateList.Count && afterState < input.Length; ++stateIdx)
			{
				if (!stateList[stateIdx].Update(input, afterState, runState, out afterState))
				{
					stateEnabled = false;
				}
			}
			
			// State changed?
			if(Log!=null && initialState!= stateEnabled)
			{
				string str = new string('*', 120);
				Log.LogLine(str);
				Log.LogLine($"* The overall state has become {((stateEnabled)?"ENABLED":"DISABLED")}");
				Log.LogLine(str);
			}
			return stateEnabled;
		}

		private string ReplaceMatch(
		 string replaceWith,
		 Func<RunState, string, int, string> ReplaceFunc,
		 Dictionary<string, Capture> captured,
		 RunState runState,
		 string input,
		 int afterMatch)
		{
			if (ReplaceFunc!=null)
			{
				return ReplaceFunc(runState, input, afterMatch);
			}

			return ReplaceMatchFromString(replaceWith,captured);
		}

		// Replace the string with the captured values / formatted
		public string ReplaceMatchFromString(
		 string replaceWith,
		 Dictionary<string, Capture> captured)
		{
			StringBuilder replaced = new StringBuilder();
			for (int i = 0; i < replaceWith.Length; )
			{
				char chr = replaceWith[i];
				if (chr == '\'')
				{
					int tokenBegin = i + 1;
					if (tokenBegin == replaceWith.Length)
					{
						ThrowReplaceWithMissingClosingQuote();
					}
					if (replaceWith[tokenBegin] == '\'')
					{
						replaced.Append('\'');
					}
					else
					{
						// Search for the closing quote
						int endPos = replaceWith.IndexOf('\'', i + 2);
						if (endPos == -1)
						{
							ThrowReplaceWithMissingClosingQuote();
						}

						// Get the name and find it in the capture list
						string tokenName = replaceWith.Substring(tokenBegin, endPos - tokenBegin);
						Capture capture;
						if (!captured.TryGetValue(tokenName, out capture))
						{
							ThrowParseError(string.Format("'{0}' in replace with string is not in the capture list.",tokenName));
						}

						// Add this captured value to the output string
						replaced.Append(capture.Captured);

						// Update the index
						i = endPos+1;
					}
				}
				else
				{
					replaced.Append(chr);
					++i;
				}
			}
			return replaced.ToString();
		}

		private void ThrowReplaceWithMissingClosingQuote()
		{
			ThrowParseError(string.Format(
			 "Invalid replace with string, opening quote found with no associated closing quote."));
		}

		internal static void ThrowParseError(
		 string error)
		{
			throw new ParseException(error);
		}
	}

	public class ParseException : Exception
	{
		public ParseException(
		 string text) : base(text)
		{
		}
	}
}
