using System.Drawing;

namespace Plisky.Boondoggle2 {

    public class HardcodedEquipmentRepository : EquipmentRepository {

        protected override EquipmentItem ActualLoadEquipmentItemById(int identity) {
            switch (identity) {
                case KnownEquipmentIds.DEFAULTSCANNER: return GetDefaultScanner();
                case KnownEquipmentIds.SCANNER_1: return GetLongRangeScanner();
                case KnownEquipmentIds.WEAPONTYPE_RIFLE_INSTANCE_1: return GetDefaultWeapon();
                case KnownEquipmentIds.DEFAULTPOWERPACK: return GetDefaultPowerPack();
                default: throw new BdBaseException("Equipment Repostiroy does not understand the request");
            }
        }

        private EquipmentItem GetDefaultWeapon() {
            OffensiveWeaponEquipmentItem result = new OffensiveWeaponEquipmentItem();
            result.DisplayName = "Biro based Paperclip Rifle";
            result.Classification = ItemClassification.OffsensiveWeapon;
            result.MakeExternalInstallsPermitted();
            result.UniqueId = KnownEquipmentIds.WEAPONTYPE_RIFLE_INSTANCE_1;
            result.InitialAmmunition = 20;
            result.D10DamageRolls = 1;
            result.DamageModifier = 0;
            result.BaseHitChance = 50;
            return result;
        }

        private EquipmentItem GetDefaultPowerPack() {
            PowerPackEquipmentItem result = new PowerPackEquipmentItem();
            result.Classification = ItemClassification.PowerPack;
            result.DisplayName = "Cobbled Together Power Pack";
            result.UniqueId = 201;
            result.MaxSpeed = 10;
            result.SpaceRequired = 2;
            result.ChargePerTurn = 100;
            result.TotalPower = 50000;
            result.Acceleration = 2;
            return result;
        }

        private EquipmentItem GetDefaultScanner() {
            ScannerEquipmentItem result = new ScannerEquipmentItem();
            result.DisplayName = "Cobbled Together Scanner";
            result.Classification = ItemClassification.Scanner;
            result.UniqueId = 3;
            result.ChargeConsumed = 10;
            result.Cooldown = 0;
            for (int x = -10; x <= 10; x++) {
                for (int y = -10; y <= 10; y++) {
                    result.AddScannedOffsetPoint(new Point(x, y));
                }
            }

            return result;
        }

        private EquipmentItem GetLongRangeScanner() {
            ScannerEquipmentItem result = new ScannerEquipmentItem();
            result.DisplayName = "Webcam Scanner";
            result.Classification = ItemClassification.Scanner;
            result.UniqueId = 4;
            result.ChargeConsumed = 10;
            result.Cooldown = 0;
            for (int x = -5; x <= 5; x++) {
                for (int y = -5; y <= 20; y++) {
                    result.AddScannedOffsetPoint(new Point(x, y));
                }
            }

            return result;
        }

        protected override BotFrame ActualLoadBotFrame(int idToLoad) {
            return new BotFrame() {
                Name = "Basic Frame"
            };
        }
    }
}