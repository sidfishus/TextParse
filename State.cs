using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sid;
using Sid.Log;

namespace Sid.Parse.TextPatternParser
{
	public class State
	{
		IComparisonWithAdvance m_Enable;
		IComparisonWithAdvance m_Disable;
		//~V

		public State(
		 IComparisonWithAdvance enable,
		 IComparisonWithAdvance disable,
		 bool initialState)
		{
			m_Enable = enable;
			m_Disable = disable;
			IsEnabled = initialState;
		}

		// Returns whether this state is enabled
		public bool Update(
		 string input,
		 int pos,
		 RunState runState,
		 out int index)
		{
			bool orgEnabled = IsEnabled;
			index = pos;
			if (IsEnabled)
			{
				// Look for the disable state
				int afterDisable;
				IsEnabled = !m_Disable.CompareAndAdvance(input, pos, 0, runState, out afterDisable);
				if (!IsEnabled)
				{
					// Skip to after the compare
					index = afterDisable;
				}
			}
			else
			{
				// Look for the enable state
				int afterEnable;
				IsEnabled = m_Enable.CompareAndAdvance(input, pos, 0, runState, out afterEnable);
				if (IsEnabled)
				{
					// Skip to after the compare
					index = afterEnable;
				}
			}

#if DEBUG
			// Has become enabled
			if (!orgEnabled && IsEnabled)
			{
				if (pos <= 9279)
				{
					//Console.WriteLine(string.Format("enabled: {0}", pos));
				}
			}
			if (pos == 1374)
			{
				Misc.Break();
			}
#endif
			
#if DEBUG
			if (Name== @"style=""" || Name== @"table or th")
			{
				if(orgEnabled!= IsEnabled)
				{
					/*
					string stringPortion = input.Substring(pos, Math.Min(20, input.Length));
					Console.WriteLine(string.Format("Changed from {0} to {1}. pos: {3} string:\r\n{2}",
					 orgEnabled,
					 IsEnabled,
					 stringPortion,
					 pos));
					 */
				}
			}
#endif

			return IsEnabled;
		}

		public string Name
		{
			get;
			set;
		}

		public bool IsEnabled
		{
			get;
			private set;
		}
	}
}
