using System;
using System.Collections.Generic;
using System.Text;
using Sid;
using Sid.Log;

namespace Sid.Parse.TextPatternParser
{
	public abstract class StatementBase : HasLogImp
	{
		public delegate void UserOnMatch(string htmlSource, int pos, string debugStr);
		protected Options m_Options;
		private UserOnMatch m_fUserOnMatch;


		protected StatementBase(
		 ILog log,
		 Options options,
		 UserOnMatch fUserOnMatch=null)
		{
			Log = log;
			m_Options = options;
			m_fUserOnMatch = fUserOnMatch;
		}

		protected StatementBase(
			ILog log,
			UserOnMatch fUserOnMatch = null)
		{
			Log = log;
			m_Options = null;
			m_fUserOnMatch = fUserOnMatch;
		}

		// Helpful with debugging
		public string Name
		{
			get;
			set;
		}

		// Allows debug hookins and potentially logging in the future
		protected void PriorToPerform(
		 string input,
		 int pos,
		 int depth)
		{
#if DEBUG
			if (Name == "Args number")
			{
				Misc.Break();
			}
#endif
		}

		protected void AfterPerform(
		 bool? rv,
		 string input,
		 int pos,
		 int depth,
		 RunState runState,
		 int? index)
		{
#if DEBUG
			string currentStr = string.Empty;
			if ((pos + 40) < input.Length)
			{
				currentStr = input.Substring(pos, 40);
			}
			if (Name == "pag start")
			{
			}
			if (Name == "open close")
			{
			}
			if (Name == "open")
			{
			}

			if (Name == "no advance" && rv.Value)
			{
			}
#endif
			if ((!rv.HasValue || (rv.HasValue && rv.Value)) && m_fUserOnMatch!=null)
			{
				string debugStr=string.Empty;
#if DEBUG
				int debugLen =Math.Min(50,input.Length- pos);
				debugStr = input.Substring(pos, debugLen);
#endif
				m_fUserOnMatch(input,pos,debugStr);
			}

			if (Log!=null)
			{
				Func<bool> fLogIsMatchResult = () =>
				{
					return ((Log.GetLevel() & (int)eLogLevel.MatchResult) > 0);
				};

				bool doLog;
				if (rv.HasValue && rv.Value)
				{
					doLog = fLogIsMatchResult() || ((Log.GetLevel() & (int)eLogLevel.SuccessfulMatch) > 0);
				}
				else
				{
					doLog = fLogIsMatchResult();
				}

				if (doLog)
				{
					const int outputLen= 40;
					string posPortion = input.Substring(pos, Math.Min(outputLen, input.Length - pos));
					string indexPortion;
					if (index != null && index.Value >= 0 && index.Value < input.Length)
					{
						indexPortion = input.Substring(index.Value, Math.Min(outputLen, input.Length - index.Value));
					}
					else
					{
						indexPortion = string.Empty;
					}

					StringBuilder textBuilder = new StringBuilder();
					// Do the depth
					for (int i = 0; i < depth; ++i)
					{
						textBuilder.Append(" ");
					}
					if (!string.IsNullOrEmpty(Name))
					{
						textBuilder.Append(string.Format("** {0} ** ", Name));
					}
					textBuilder.Append(ToString());
					textBuilder.Append(" ");

					// Return value
					bool showAfter = true;
					if (rv.HasValue)
					{
						textBuilder.Append(
						 string.Format("match={0}, ", rv.Value));
						showAfter = rv.Value;
					}

					// Position/string
					textBuilder.Append(
					 string.Format("pos={0} {1}",
					 pos,
					 posPortion));

					// After
					if (showAfter)
					{
						textBuilder.Append(
						 string.Format(", after={0} {1}",
						 index,
						 indexPortion));
					}

					if (rv.HasValue && rv.Value)
					{
						textBuilder.Append(" *********************");
					}

					Log.LogLine(textBuilder.ToString());
				}
			}

			if(this.Name!=null)
				runState.ExecutePostPerformAssertions(this.Name,input,pos,index,rv,Log);
		}
	}
}
