
using StringBuilder=System.Text.StringBuilder;

namespace Sid.Parse.TextPatternParser
{
    public class PositionAssertion : IAssertion {

        bool? m_Match;
        int m_BeginPos;
        int? m_AfterPos;

        public PositionAssertion(
            bool? match,
            int beginPos,
            int? afterPos
        )
        {
            m_Match=match;
            m_BeginPos=beginPos;
            m_AfterPos=afterPos;
        }

        string IAssertion.Name
        {
            get;
            set;
        }

        bool? IAssertion.Assert(
            string input,
            int beginPos,
            int? afterPos,
            RunState runState,
            bool? matched,
            out string errorMsg
        ){

            if(beginPos!=this.m_BeginPos) {
                errorMsg=null;
                return null;
            }

            if(matched==this.m_Match && afterPos==this.m_AfterPos) {
                errorMsg=string.Empty;
                return true;
            }

            var errorMsgBuilder=new StringBuilder();

            if(matched!=this.m_Match) {
                errorMsgBuilder.AppendLine($"Expected match '{this.m_Match.ToString()}' but got '{matched.ToString()}'");
            }

            // if(beginPos!=this.m_BeginPos) {
            //     errorMsgBuilder.AppendLine(string.Format("Expected begin position {0} ({1}) but got {2} ({3})",
            //         this.m_BeginPos,
            //         Parser.DisplayPartOfInputString(input,this.m_BeginPos),
            //         beginPos,
            //         Parser.DisplayPartOfInputString(input,beginPos)
            //     ));
            // }

            if(afterPos!=this.m_AfterPos) {
                if(afterPos.HasValue && this.m_AfterPos.HasValue)
                    errorMsgBuilder.AppendLine(string.Format("Expected after position {0} ({1}) but got {2} ({3})",
                        this.m_AfterPos,
                        Parser.DisplayPartOfInputString(input,this.m_AfterPos.Value),
                        afterPos,
                        Parser.DisplayPartOfInputString(input,afterPos.Value)
                    ));
            }

            errorMsg=errorMsgBuilder.ToString();
            return false;
        }
    }
}