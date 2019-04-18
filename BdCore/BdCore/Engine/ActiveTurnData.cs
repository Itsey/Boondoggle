namespace Plisky.Boondoggle2 {
    using Plisky.Diagnostics;
    using Plisky.Plumbing;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    public class ActiveTurnData {

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



        private Dictionary<int, Tuple<int, int>> tempKeys = new Dictionary<int, Tuple<int, int>>();

        public int LastUsedTemporaryKey { get; set; }

        public void RegisterTeporaryKey(int lud, int owningBotEngineId, int targetBotEngineId) {

            #region validation

            if (owningBotEngineId == targetBotEngineId) {
                throw new BdBaseException("You can not register a temporary scan key for your own identity.");
            }
            b.Assert.True(!tempKeys.ContainsKey(lud), "should not be possible to register the same key twice.");

            #endregion validation

            tempKeys.Add(lud, new Tuple<int, int>(owningBotEngineId, targetBotEngineId));
        }

        public int GetEngineIdFromScanId(int sourceBotRequest, int tempScanKey) {
            if (!tempKeys.ContainsKey(tempScanKey)) {
                b.Verbose.Log( "Mismatched identity in temp keys - srcbot : " + sourceBotRequest.ToString() + " id : " + tempScanKey.ToString());
                return -1;
            }
            if (tempKeys[tempScanKey].Item1 == sourceBotRequest) {
                return tempKeys[tempScanKey].Item2;
            }
            return -1;
        }

        public Dictionary<int, LastTickRecord> LastTickRecords { get; set; }

        public ActiveTurnData() {
            LastTickRecords = new Dictionary<int, LastTickRecord>();
        }
    }
}