namespace Plisky.Boondoggle2 {

    public class KnownEquipmentIds {
        public const int MOCKINSTALLEVERYWHERE = 1;
        public const int MOCKINSTALLINTERNALONLY = 2;
        public const int MOCKSCANNER = 3;
        public const int MOCKPOWERPACK = 4;
        public const int MOCKPROJECTILEWEAPON = 5;
        public const int MOCKWAYTOOBIG = 6;

        public const int DEFAULTSCANNER = 10001;
        public const int SCANNER_1 = 10002;

        public const int DEFAULTPOWERPACK = 20001;

        public const int WEAPONTYPE_RIFLE_INSTANCE_1 = 30001;

        public static ItemClassification GetClassificationFromId(int equipmentIdentifier) {
            // Special Cases
            if (equipmentIdentifier == MOCKPOWERPACK) { return ItemClassification.PowerPack; }
            if (equipmentIdentifier == MOCKPROJECTILEWEAPON) { return ItemClassification.OffsensiveWeapon; }

            // General Case
            if (equipmentIdentifier < (int)ItemClassification.Scanner) { return ItemClassification.MockEquipment; }
            if (equipmentIdentifier < (int)ItemClassification.PowerPack) { return ItemClassification.Scanner; }
            if (equipmentIdentifier < (int)ItemClassification.OffsensiveWeapon) { return ItemClassification.PowerPack; }
            if (equipmentIdentifier < (int)ItemClassification.DefensiveWeapon) { return ItemClassification.OffsensiveWeapon; }
            if (equipmentIdentifier < (int)ItemClassification.Electronic) { return ItemClassification.DefensiveWeapon; }
            return ItemClassification.Electronic;
        }
    }
}