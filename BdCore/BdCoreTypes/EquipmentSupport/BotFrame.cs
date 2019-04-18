using System;
using System.Collections.Generic;

namespace Plisky.Boondoggle2 {
    public class BotFrame {
        // todo mke this not hard coded
        public string Name { get; set; }
        public int TurrentSpace { get; private set; }
        public int RearSpace { get; private set; }
        public int ForeSpace { get; private set; }
        public int InternalSpace { get; private set; }
        public int NearSideSpace { get; private set; }
        public int OffsideSpace { get; private set; }


        public void SetSpace(int backward, int forward, int inside,int nearside, int offside, int turret) {
            TurrentSpace = turret;
            RearSpace = backward;
            ForeSpace = forward;
            InternalSpace = inside;
            NearSideSpace = nearside;
            OffsideSpace = offside;
        }

        public IEnumerable<MountPoint> GetAccesibleMountpoints() {
            yield return MountPoint.Backward;
            yield return MountPoint.Forward;
            yield return MountPoint.Internal;
            yield return MountPoint.Nearside;
            yield return MountPoint.Offside;
            yield return MountPoint.Turret;

        }

        internal int GetTotalSpaceForMountPoint(MountPoint mp) {
            return 5;
        }
    }
}