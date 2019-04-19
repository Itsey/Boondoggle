using System;
using System.Diagnostics;

namespace Plisky.Boondoggle2.Test {
    internal class MockBd2GenealBase : Bd2GeneralBase {
        public MockBd2GenealBase() {
        }

        internal object GetAssignedBilge() {
            return b;
        }

        internal TraceLevel GetTraceLevel() {
            return b.CurrentTraceLevel;
        }
    }
}