using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenLR
{
    /// <summary>
    /// Defines general OpenLR parameters.
    /// </summary>
    public static class Parameters
    {
        /// <summary>
        /// Holds the bearing distance parameter.
        /// </summary>
        /// <remarks>This parameter can be changed but decoding/encoding locations might not work anymore when this parameters has been changed in the meantime.</remarks>
        public static int BEARDIST = 20;
    }
}