using System;
using System.Diagnostics;

namespace Plisky.Boondoggle2.Test {
    internal class MockBd2Map : Bd2Map {
        internal void Test_SetDimensions(int v1, int v2) {
            this.Width = v1;
            this.Height = v2;
        }
    }
}