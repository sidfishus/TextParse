
using Sid.Log;

namespace Sid.Parse.TextPatternParser
{

    public delegate bool fCustomComparison(int pos, string str, RunState runState);

	public class CustomComparison : ComparisonWithAdvanceBase, IComparisonWithAdvance
	{

        fCustomComparison m_fCustomComparison;

		public CustomComparison(
			ILog log,
			fCustomComparison customComp) : base(log)
		{
			m_fCustomComparison = customComp;
		}

        protected override bool CompareAndAdvanceImp(
		 string str,
		 int firstIndex,
		 int depth,
		 RunState runState,
		 out int index)
		{

			if (m_fCustomComparison(firstIndex, str, runState))
			{
				index = firstIndex;
				return true;
			}

			index = -1;
			return false;

		}
    }
}