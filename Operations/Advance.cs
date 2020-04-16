using Sid.Log;

namespace Sid.Parse.TextPatternParser
{
	public class Advance : OperationBase, IOperation
	{
		fOperand<int> m_Pos;

		public Advance(
		 ILog log,
		 fOperand<int> pos) : base(log)
		{
			m_Pos = pos;
		}

		protected override void PerformImp(
		 string input,
		 int pos,
		 int depth,
		 RunState runState,
		 out int index)
		{
			index = m_Pos(pos,input,runState);
		}

		public override string ToString()
		{
			return "AdvanceOperation";
		}
	}
}
