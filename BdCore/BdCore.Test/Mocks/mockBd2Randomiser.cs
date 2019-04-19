using System;
using System.Drawing;

namespace Plisky.Boondoggle2.Test {

    public class MockBd2Randomiser : Bd2CombatCalculator {
        private int d10result;
        private int d100result;
        private bool CanMountPointHit = false;
        private double directionToPoint = 0;

        public void Mock_SetD10Result(int toThis) {
            d10result = toThis;
        }

        public void Mock_SetD100Result(int toThis) {
            d100result = toThis;
        }

        protected override int ActualGetD10() {
            return d10result;
        }

        protected override int ActualGetD100() {
            return d100result;
        }

        public MockBd2Randomiser() {
            d10result = d100result = 0;
        }

        internal void Mock_SetCanTargetHit(bool v) {
            CanMountPointHit = v;
        }

        internal void Mock_SetDirectionToPoint(double v) {

        }
        protected override bool ActualCanMountPointHitTarget(double sourceHeading, MountPoint mountPoint, Point sourceLoc, Point destLoc) {
            return CanMountPointHit;
        }

        
    }
}