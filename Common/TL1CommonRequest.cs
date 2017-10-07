using System;
using System.Text;

namespace TL1Client.Common
{
    class TL1CommonRequest : TL1Request
    {
        /// <summary>
        /// The position-defined parameters in the request.
        /// </summary>
        public PositionDefinedParametersBlock PositionDefinedBlock { get; }

        /// <summary>
        /// The name-defined parameters in the request.
        /// </summary>
        public NameDefinedParameterBlock NameDefinedBlock { get; }

        /// <summary>
        /// The State Block containing a primary and an optional secondary state (&lt;pst&gt; and &lt;sst&gt;) is used in commands affecting entities for which variable states are defined. Please refer to GR-1093-CORE for details about states and state transitions.
        /// </summary>
        public PositionDefinedParametersBlock StateBlock { get; }

        #region Overrides of TL1Request

        protected override string GetAdditionalDataString()
        {
            var sb = new StringBuilder();
            var i = 0;
            PrintFieldIfNotNullAndCountColons(sb, PositionDefinedBlock, ref i, toString: TL1RequestDataBlock.GetBlockString);
            PrintFieldIfNotNullAndCountColons(sb, NameDefinedBlock, ref i, toString: TL1RequestDataBlock.GetBlockString);
            PrintFieldIfNotNullAndCountColons(sb, StateBlock, ref i, toString: TL1RequestDataBlock.GetBlockString);

            return i == 3 ? null : sb.ToString();
        }

        #endregion

        public TL1CommonRequest(string verb, string modifier1, string correlationTag) : base(verb, correlationTag, modifier1)
        {
            this.PositionDefinedBlock = new PositionDefinedParametersBlock();
            this.NameDefinedBlock = new NameDefinedParameterBlock();
            this.StateBlock = new PositionDefinedParametersBlock();
        }
        public TL1CommonRequest(CommonVerbs verb, string modifier1, string correlationTag) : base(GetCommonVerbString(verb), correlationTag, modifier1)
        {
            this.PositionDefinedBlock = new PositionDefinedParametersBlock();
            this.NameDefinedBlock = new NameDefinedParameterBlock();
            this.StateBlock = new PositionDefinedParametersBlock();
        }

        public static string GetCommonVerbString(CommonVerbs verb)
        {
            switch (verb)
            {
                case CommonVerbs.Activate: return "ACT";
                case CommonVerbs.Allow: return "ALW";
                case CommonVerbs.Cancel: return "CANC";
                case CommonVerbs.Delete: return "DLT";
                case CommonVerbs.Edit: return "ED";
                case CommonVerbs.Enter: return "ENT";
                case CommonVerbs.Inhibit: return "INH";
                case CommonVerbs.Initialize: return "INIT";
                case CommonVerbs.Operate: return "OPR";
                case CommonVerbs.Report: return "REPT";
                case CommonVerbs.Release: return "RLS";
                case CommonVerbs.Remove: return "RMV";
                case CommonVerbs.Restore: return "RST";
                case CommonVerbs.Retrieve: return "RTRV";
                case CommonVerbs.Set: return "SET";
            }
            throw new ArgumentOutOfRangeException(nameof(verb));
        }
    }

    /// <summary>
    /// Represents a list some of the common verbs in TL1.
    /// </summary>
    public enum CommonVerbs
    {
        ///<summary>ACT</summary>
        Activate,

        ///<summary>ALW</summary>
        Allow,

        ///<summary>CANC</summary>
        Cancel,

        ///<summary>DLT</summary>
        Delete,

        ///<summary>ENT</summary>
        Enter,

        ///<summary>ED</summary>
        Edit,

        ///<summary>INH</summary>
        Inhibit,

        ///<summary>INIT</summary>
        Initialize,

        ///<summary>OPR</summary>
        Operate,

        ///<summary>RPRT</summary>
        Report,

        ///<summary>RLS</summary>
        Release,

        ///<summary>RMV</summary>
        Remove,

        ///<summary>RST</summary>
        Restore,

        ///<summary>RTRV</summary>
        Retrieve,

        ///<summary>SET</summary>
        Set,
    }

}