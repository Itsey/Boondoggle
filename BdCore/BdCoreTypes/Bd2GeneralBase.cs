using Plisky.Diagnostics;
using Plisky.Plumbing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Plisky.Boondoggle2 {
    public abstract class Bd2GeneralBase {
        protected Hub h = Hub.Current;
        protected Bilge b = new Bilge(tl: TraceLevel.Off);
        
        
        /// <summary>
        /// Inject a new instance of bilge, or change the trace level of the current instance. To set the trace level ensure that
        /// the first parameter is null.  To set bilge simply pass a new instance of bilge.
        /// </summary>
        /// <param name="blg">An instance of Bilge to use inside this Hub</param>
        /// <param name="tl">If specified and blg==null then will alter the tracelevel of the current Bilge</param>
        public void InjectBilge(Bilge blg, TraceLevel tl = TraceLevel.Off) {
            if (blg != null) {
                b = blg;
            } else {
                b.CurrentTraceLevel = tl;
            }
        }

        /// <summary>
        /// Inject a new instance of Hub, to ensure that this class hooks up with the same messaging infrastructure as the rest of the
        /// application, or subset of the application that uses that hub.  
        /// </summary>
        /// <param name="newhub">The new hub instance to use for all messaging.</param>
        public void InjectHub(Hub newhub) {
            h = newhub;
        }

    }
}
