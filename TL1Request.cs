using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TL1Client
{
    /// <summary>
    /// A basic class for a request.
    /// </summary>
    public class TL1Request
    {
        private TL1Request()
        {
            AdditionalDataBlocks = new List<TL1RequestDataBlock>();
        }

        public TL1Request(string verb, string correlationTag, params TL1RequestDataBlock[] payload)
            : this()
        {
            Verb = verb;
            CorrelationTag = correlationTag;
            AdditionalDataBlocks.AddRange(payload);
        }

        public TL1Request(string verb, string modifier1, string correlationTag, params TL1RequestDataBlock[] payload)
            : this()
        {
            Verb = verb;
            Modifier1 = modifier1;
            CorrelationTag = correlationTag;
            AdditionalDataBlocks.AddRange(payload);
        }

        #region Command Block
        /// <summary>
        /// Identifies the action to be taken.
        /// </summary>
        public string Verb { get; }

        /// <summary>
        /// Typically defines the object to be acted upon.
        /// </summary>
        public string Modifier1 { get; set; }

        /// <summary>
        /// Narrows the scope of the command and is typically command dependent
        /// </summary>
        public string Modifier2 { get; set; }
        #endregion

        #region Staging Parameter Block
        /// <summary>
        /// The TID specifies the network element (system) to which the command is directed. A TID can be up to 20 alpha-numeric characters in length. It has a default value and can be modified by the RTRV-SYSTEM-STATUS TL1 command (page 157).
        /// A command containing a NULL TID is assumed to be addressing the system directly. TIDs that do not match the target system can belong to other systems reachable through gateway functionality
        /// </summary>
        public string TargetID { get; set; }

        /// <summary>
        /// An AID can be used to identify uniquely an entity within the system. They are more specific than the modifiers in the command block.
        /// </summary>
        public string AccessID { get; set; }

        /// <summary>
        /// The CTAG is a unique identifier used to correlate an input command with its output response. It is arbitrarily defined by the system or user sending the command and is limited to 6 alpha-numeric characters. Its value should not be “0”, as this is the value used by the system for responses to invalid login attempts.
        /// </summary>
        public string CorrelationTag { get; }

        /// <summary>
        /// Used for staging parameters that can control such capabilities as delayed command execution and indirect data retrieval. 
        /// </summary>
        /// <remarks>
        /// Implementation is left for the user (can use <see cref="PositionDefinedParametersBlock"/>).
        /// </remarks>
        public TL1RequestDataBlock GeneralBlock { get; set; }
        #endregion

        /// <summary>
        /// Allows adding additional data blocks. These will be appeneded to the end of the request. 'null' blocks will generate an empty string.
        /// </summary>
        public List<TL1RequestDataBlock> AdditionalDataBlocks { get; }

        /// <summary>
        /// Returns the string representing the TL1 command to be sent.
        /// </summary>
        /// <returns>The string representing the TL1 command.</returns>
        public string ToCommandString()
        {
            // Format: <verb>[-<modifier1>[-<modifier2>]]:[<tid>]:[<aid>]:<ctag>
            //         :[<general_block>]:[payload_block];
            var sb = new StringBuilder(100);

            sb.Append(Verb);
            PrintFieldIfNotNull(sb, Modifier1, "-");
            PrintFieldIfNotNull(sb, Modifier2, "-");
            int colonCounter = 1;
            PrintFieldIfNotNullAndCountColons(sb, TargetID, ref colonCounter);
            PrintFieldIfNotNullAndCountColons(sb, AccessID, ref colonCounter);
            PrintFieldIfNotNullAndCountColons(sb, CorrelationTag, ref colonCounter);
            PrintFieldIfNotNullAndCountColons(sb, GeneralBlock, ref colonCounter, toString: TL1RequestDataBlock.GetBlockString);

            PrintFieldIfNotNullAndCountColons(sb, GetAdditionalDataString(), ref colonCounter);
            
            foreach (var dataBlock in AdditionalDataBlocks)
            {
                PrintFieldIfNotNullAndCountColons(sb, dataBlock?.GetBlockString(), ref colonCounter);
            }

            sb.Append(";");

            return sb.ToString();
        }

        /// <summary>
        /// When overriden in a derived class, returns additional payload data (asside from that in <see cref="AdditionalDataBlocks"/>).
        /// </summary>
        /// <returns> The string that represents the additional data in the dervied class, or '<value>null</value>' if there's none.</returns>
        protected virtual string GetAdditionalDataString()
        {
            return null;
        }

        /// <summary>
        /// Appends the field to the <see cref="TL1Request"/>'s <see cref="StringBuilder"/> if it isn't '<value>null</value>', while accumulating the colons (doesn't print unneaded colons).
        /// </summary>
        /// <typeparam name="TValue">The value of the field.</typeparam>
        /// <param name="sb">The <see cref="StringBuilder"/> to which the method will print <see cref="field"/>'s value.</param>
        /// <param name="field">The value to print to the <see cref="StringBuilder"/>.</param>
        /// <param name="colonCounter">The number of colons to print if the <see cref="field"/> is valid for print.</param>
        /// <param name="prefix">OPTIONAL: If specifed, adds a prefix to the print (if occurs).</param>
        /// <param name="toString">OPTIONAL: A conversion method between <see cref="TValue"/> and <see cref="string"/>.</param>
        protected static void PrintFieldIfNotNullAndCountColons<TValue>(StringBuilder sb, TValue field, ref int colonCounter, string prefix = null,
            Func<TValue, string> toString = null)
            where TValue : class
        {
            if (field == null)
            {
                colonCounter++;
                return;
            }

            for (int i = 0; i < colonCounter; i++)
                sb.Append(":");

            colonCounter = 1;

            PrintFieldIfNotNull(sb, field, prefix, toString);
        }

        /// <summary>
        /// Appends the field to the <see cref="TL1Request"/>'s <see cref="StringBuilder"/> if it isn't '<value>null</value>'.
        /// </summary>
        /// <typeparam name="TValue">The value of the field.</typeparam>
        /// <param name="sb">The <see cref="StringBuilder"/> to which the method will print <see cref="field"/>'s value.</param>
        /// <param name="field">The value to print to the <see cref="StringBuilder"/>.</param>
        /// <param name="prefix">OPTIONAL: If specifed, adds a prefix to the print (if occurs).</param>
        /// <param name="toString">OPTIONAL: A conversion method between <see cref="TValue"/> and <see cref="string"/>.</param>
        protected static void PrintFieldIfNotNull<TValue>(StringBuilder sb, TValue
            field, string prefix = null, Func<TValue, string> toString = null)
            where TValue : class
        {
            if (!string.IsNullOrEmpty(prefix))
                sb.Append(prefix);

            sb.Append((toString ?? Convert.ToString)(field));
        }
    }
}

