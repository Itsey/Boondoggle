namespace Plisky.Boondoggle2 {
    using Plisky.Diagnostics;
    using Plisky.Plumbing;
    using System.Diagnostics;

    public abstract class bd2BaseModel {
        /// <summary>
        /// Recommended that child Register Messages functions reset this.
        /// </summary>
        protected bool needToRegister = true;
        protected Hub hub = Hub.Current;
        protected Bilge b = new Bilge(tl: TraceLevel.Off);
        
        protected virtual void OnHubChanged() {
            // override if you want notification of hub changes (need to reregister for messages for example)
        }

        public virtual void RegisterMessages() {
            needToRegister = false;
        }
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




        public void InjectHub(Hub desiredHub) {
            b.Verbose.Log("Hub injected into base class");
            if (!object.ReferenceEquals(hub, desiredHub)) {
                hub = desiredHub;
                needToRegister = true;
                OnHubChanged();
            }
        }
    }
}