using System;
using System.Collections.Generic;
using System.Text;

//not sure if there is a point in this
/*
namespace Sid.Parse.TextPatternParser
{
	public class EnclosedStatementComparison : ComparisonBase,IComparison
	{
		IComparison m_Open;
		IComparison m_Close;
		//~V

		public EnclosedStatementComparison(
		 Options options,
		 IComparison open,
		 IComparison close) : base(options)
		{
			m_Open = open;
			m_Close = close;
		}

		bool IComparison.Compare(
		 string str,
		 int firstIndex,
		 out int index)
		{
			base.PriorToPerform();

			// Check the opening?
			int afterOpen;
			if(m_Open.Compare(str,
			 firstIndex,
			 out afterOpen))
			{
				// Find the end
				for(int i= afterOpen; i<str.Length;++i)
				{
					int afterClose;
					if(m_Close.Compare(
					 str,
					 i,
					 out afterClose))
					{
						// Found the close
						index = afterClose;
						return true;
					}
				}
			}
			index = -1;
			return false;
		}
	}
}
*/