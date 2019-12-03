using System;
using System.Collections.Generic;
using System.Text;
using Sid.Log;

namespace Sid.Parse.TextPatternParser
{
	public class Options : HasLogImp
	{
		List<Options> m_SubOptions = null;
		IComparisonWithAdvance m_LineWrap;
		IOperation m_SkipLineWrap;

		// The options. Don't forget to update 'CopyToSubOptions' when adding new options here
		bool? m_CaseSensitive;
		//~The options
		//~V

		public Options(
		 ILog log)
		{
			Log = log;
		}

		public void AddSubOption(
		 Options subOptions)
		{
			if (m_SubOptions == null)
			{
				m_SubOptions = new List<Options>();
			}
			m_SubOptions.Add(subOptions);
		}

		public bool CaseSensitive
		{
			get
			{
				if (!m_CaseSensitive.HasValue)
				{
					// Default to case sensitive
					m_CaseSensitive = true;
				}
				return m_CaseSensitive.Value;
			}

			set
			{
				m_CaseSensitive = value;
			}
		}

		public IComparisonWithAdvance LineWrap
		{
			get
			{
				return m_LineWrap;
			}
			set
			{
				bool changed = (m_LineWrap != value);
				m_LineWrap = value;
				if(changed)
				{
					// Create the skip operation
					AdvanceIfOperation op= new AdvanceIfOperation(Log, m_LineWrap); ;
					op.Name = "Skip linewrap";
					m_SkipLineWrap = op;
				}
			}
		}

		public IOperation SkipLineWrapOperation
		{
			get
			{
				return m_SkipLineWrap;
			}
		}

		public void CopyToSubOptions()
		{
			if (m_SubOptions != null)
			{
				foreach (Options options in m_SubOptions)
				{
					CopyToSubOptionsValue(m_CaseSensitive, options, ref options.m_CaseSensitive);
				}
			}
		}

		private void CopyToSubOptions<TYPE>(
		 TYPE thisComp,
		 Options options,
		 ref TYPE optionsComp) where TYPE : class
		{
			// Only update if this isn't NULL and the options version IS
			if(thisComp != null && optionsComp == null)
			{
				optionsComp = thisComp;
			}
		}

		private void CopyToSubOptionsValue<TYPE>(
		 TYPE? thisValue,
		 Options options,
		 ref TYPE? optionsValue) where TYPE : struct
		{
			if(thisValue.HasValue && !optionsValue.HasValue)
			{
				optionsValue = thisValue;
			}
		}

		public void SkipLineWrap(
		 string input,
		 int pos,
		 int depth,
		 RunState runState,
		 out int index)
		{
			if (m_SkipLineWrap != null)
			{
				m_SkipLineWrap.Perform(input, pos, depth, runState, out index);
			}
			else
			{
				// Do nothing
				index = pos;
			}
		}
	}
}
