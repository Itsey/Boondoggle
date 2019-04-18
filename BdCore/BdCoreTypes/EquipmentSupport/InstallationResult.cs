using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plisky.Boondoggle2 {
    public enum InstallationResult {

        /// <summary>
        /// indicates a successful instalation
        /// </summary>
        Installed,

        /// <summary>
        /// indicates that although the mountpoint is valid therenis already too mich kit present in that location
        /// </summary>
        Fail_NoSpace,

        /// <summary>
        /// indicates that eithwr the frame doesnt support this mountpoint or that the equipment cant go in this mountpoint
        /// </summary>
        Fail_InvalidMountpoint,

        /// <summary>
        /// Indicates that there was a problem Such as two powerpacks or otherwise invalid configuration
        /// 
        /// </summary>
        Fail_InvalidCombination
    }
}
