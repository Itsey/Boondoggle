using System;
using System.Drawing;

namespace Plisky.Boondoggle2.Test {
    internal class MockBd2World : bd2World {
        private bool losHitResult = false;

        public override bool IsLOSBetween(Point sourceLoc, Point destLoc) {
            return losHitResult;
        }
        public MockBd2World(MockBd2Map mp) : base(mp) {
            
        }

        internal void Test_LOSCanHitReturn(bool v) {
            losHitResult = v;
        }
    }
}