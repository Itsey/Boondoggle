namespace Plisky.Boondoggle2 {

    using System.Collections.Generic;

    public class EquipmentItem {
        private List<MountPoint> permittedPositions = new List<MountPoint>();

        public IEnumerable<MountPoint> GetPErmittedPositions() {
            foreach (var v in permittedPositions) {
                yield return v;
            }
        }

        public string DisplayName { get; set; }

        public int UniqueId { get; set; }

        public ItemClassification Classification { get; set; }
        public int SpaceRequired { get; set; }

        public void AddPermittedInstallPosition(MountPoint mp) {
            permittedPositions.Add(mp);
        }

        public void MakeExternalInstallsPermitted() {
            AddPermittedInstallPosition(MountPoint.Forward);
            AddPermittedInstallPosition(MountPoint.Backward);
            AddPermittedInstallPosition(MountPoint.Nearside);
            AddPermittedInstallPosition(MountPoint.Offside);
            AddPermittedInstallPosition(MountPoint.Turret);
        }

        public bool IsInstallationPermitted(MountPoint mountPoint) {
            foreach (var v in permittedPositions) {
                if (v == mountPoint) {
                    return true;
                }
            }
            return false;
        }
    }
}