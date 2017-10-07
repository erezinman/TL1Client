using System.Collections.Generic;
using System.Linq;

namespace TL1Client
{
    public abstract class TL1RequestDataBlock
    {
        public abstract  string GetBlockString();


        public static string GetBlockString(TL1RequestDataBlock block)
        {
            return block.ToString();
        }
    }

    /// <summary>
    /// Used for input parameters where no keyword or name is needed. The meaning of the parameter is inferred from its position within the block. Typically, this is used where there are a few options available to the command. Each parameter is separated by a comma. If a parameter can be omitted, leading commas must be present. 
    /// Where parameters at the end of the block are omitted, their leading commas also can be omitted.
    /// </summary>
    public class PositionDefinedParametersBlock : TL1RequestDataBlock
    {
        public PositionDefinedParametersBlock()
        {
            Parameters = new List<string>();
        }

        /// <summary>
        /// The position-relevant parameters. 'null' element is treated as an empty string.
        /// </summary>
        public List<string> Parameters { get; }


        #region Overrides of TL1RequestDataBlock

        public override string GetBlockString()
        {
            return Parameters == null ? "" : string.Join(",", Parameters.Select(s => s ?? ""));
        }

        #endregion
    }


    /// <summary>
    /// Used for parameters represented by keyword-value pairs.
    /// (<code>...ctag:::KEYWORD1=value1,KEYWORD2=value2;</code>)
    /// The keyword and value must be separated by an equal sign(=). Multiple keyword-value parameters are separated by commas(,) but no comma is entered in front of the first keyword-value parameter. Order is not significant.
    /// </summary>
    public class NameDefinedParameterBlock : TL1RequestDataBlock
    {
        public NameDefinedParameterBlock()
        {
            MappedParameters = new Dictionary<string, object>();
        }

        /// <summary>
        /// The mapping of paramerter key (name) to value.
        /// </summary>
        public Dictionary<string, object> MappedParameters { get; }

        #region Overrides of TL1RequestDataBlock

        public override string GetBlockString()
        {
            return MappedParameters == null ? "" : string.Join(",", MappedParameters.Select(kvp => $"{kvp.Key}={kvp.Value ?? ""}"));
        }

        #endregion
    }

}