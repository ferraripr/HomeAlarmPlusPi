using System;
using Microsoft.SPOT;

namespace Pachube.EmbeddableGraphGenerator
{
    /// <summary>
    /// Pachube Embeddable Graph Generator Graphic Functionality
    /// </summary>
    public class GraphFunctionality
    {
        /// <summary>
        /// <para>Pachube default timespan.</para>
        /// <remarks>Timespan options:
        /// <list type="table">
        /// <item>
        /// <term>0</term>
        /// <description>=last hour.</description>
        /// </item>
        /// <item>
        /// <term>1</term>
        /// <description>=24 hours.</description>
        /// </item>
        /// <item>
        /// <term>2</term>
        /// <description>=4 days.</description>
        /// </item>
        /// <item>
        /// <term>3</term>
        /// <description>=3 months.</description>
        /// </item>
        /// </list>
        /// </remarks>
        /// </summary>
        public static string[] DEFAULT_TIMESPAN = { "last hour", "24 hours", "4 days", "3 months" };

        /// <summary>
        /// Automatically update when new data is acquired.
        /// </summary>
        public static bool AutoUpdate = false;

        /// <summary>
        /// Right edge graph will be the time when it was loaded.
        /// </summary>
        public static bool Rolling = true;
    }
}
