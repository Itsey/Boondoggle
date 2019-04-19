namespace Plisky.Boondoggle2 {
    
    using System.Collections.Generic;

    public class EquipmentSupport {
        protected EquipmentRepository store;
        private Dictionary<int, EquipmentItem> equipmentTemplates = new Dictionary<int, EquipmentItem>();

        public int EquipmentCount { get; set; }

        public EquipmentSupport(EquipmentRepository er) {
            store = er;
        }

        public OffensiveWeaponEquipmentItem GetWeaponEquipmentById(int id) {
            if (!equipmentTemplates.ContainsKey(id)) {
                equipmentTemplates.Add(id, store.LoadEquipmentById(id));
            }
            return equipmentTemplates[id] as OffensiveWeaponEquipmentItem;
        }

        public EquipmentItem GetEquipmentTypeById(int id) {
            if (!equipmentTemplates.ContainsKey(id)) {
                equipmentTemplates.Add(id, store.LoadEquipmentById(id));
            }

            return equipmentTemplates[id];
        }

        public bool CanMountEquipment(int p, MountPoint v) {
            EquipmentItem ei = store.LoadEquipmentById(p);
            return ei.IsInstallationPermitted(v);
        }

        public static IEnumerable<MountPoint> ListAllExternalMountPoints() {
            yield return MountPoint.Forward;
            yield return MountPoint.Backward;
            yield return MountPoint.Nearside;
            yield return MountPoint.Offside;
            yield return MountPoint.Turret;
        }

        public static IEnumerable<MountPoint> ListAllMountPoints() {
            yield return MountPoint.Forward;
            yield return MountPoint.Backward;
            yield return MountPoint.Nearside;
            yield return MountPoint.Offside;
            yield return MountPoint.Turret;
            yield return MountPoint.Internal;
        }

        public static IEnumerable<MountPoint> ListAllInternallMountPoints() {
            yield return MountPoint.Internal;
        }
    }
}