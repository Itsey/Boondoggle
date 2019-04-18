using Plisky.Diagnostics;
using Plisky.Plumbing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace Plisky.Boondoggle2 {

    public class BattleRunnerControl {

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



        private BattleStatusFile bsf;
        private List<BoonBotBase> ctsts = new List<BoonBotBase>();

        public string BattleUniqueName { get { return bsf.UniqueName; } }

        public string GetControlData() {
            DataContractSerializer dcs = new DataContractSerializer(typeof(BattleStatusFile));
            string result = null;
            using (MemoryStream ms = new MemoryStream()) {
                dcs.WriteObject(ms, bsf);
                result = Encoding.UTF8.GetString(ms.GetBuffer());
            }
            b.Info.Log("Control data returned", result);
            return result;
        }


        public BattleRunnerControl(string fileName) {
            DataContractSerializer dcs = new DataContractSerializer(typeof(BattleStatusFile));
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read)) {
                bsf = (BattleStatusFile)dcs.ReadObject(fs);
            }
            Prepare();
        }

        public BattleRunnerControl(string name, string description) {
            bsf = new BattleStatusFile();
            bsf.UniqueName = name;
        }

        public void AddContestant(BoonBotBase ctst) {
            ctsts.Add(ctst);
        }

        public IEnumerable<BoonBotBase> GetContestants() {
            foreach (var nxt in ctsts) {
                yield return nxt;
            }
        }

        private void Prepare() {
            string path = bsf.PathToBinaries;

            foreach (var v in bsf.AllBots) {
                string fname = Path.Combine(path, v.BinaryName);
                if (!File.Exists(fname)) {
                    throw new InvalidOperationException("Binary not found - Invalid Request");
                }

                Assembly assembly = Assembly.LoadFile(fname);

                var nextBot = typeof(BoonBotBase);
                foreach (Type t in assembly.GetTypes()) {
                    if ((t.Name == v.TypeName) && (nextBot.IsAssignableFrom(t))) {
                        BoonBotBase p = (BoonBotBase)Activator.CreateInstance(t);
                        if (p != null) {
                            ctsts.Add(p);
                        }
                    }
                }
            }
        }

    }
}