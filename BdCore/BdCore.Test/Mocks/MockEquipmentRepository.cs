namespace Plisky.Boondoggle2.Test {

    using Plisky.Boondoggle2.Repository;
    using System;
    using System.Drawing;

    public class MockEquipmentRepository : EquipmentRepository {

        private int definedScannerBehaviour = 0;
        private int baseHitChance = 10;
        private int initalAmmo = 20;
        private int chargeCostPerShot = 30;
        private int mockPackChargePerTurn;
        private int mockPackTotalPower;
        private int mockScannerchargeConsumedOnUse = 45;
        private int mockPowerPackSpeed;
        private int mockPowerPackAcceleration;
        private int weapionSpaceRequired=4;
        private int weaponTotalWeight=100;
        private int mockPackSpaceRequired = 4;
        private int mockPackWeightRequired = 200;
        private int defaultHullTotalWeight = DEFAULTHULLWEIGHT;
        private int defaultHullTotalSpace = DEFAULTHULLSPACES;
        private int defaultHullTurretSpace = DEFAULTTURRETSPACES;

        public const int DEFAULTPPCHARGE = 87;
        public const int DEFAULTPPPOWER = 4587;
        public const int DEFAULTPPMAXSPEED = 10;
        public const int DEFAULTPPACCELERATION = 2;
        public const int DEFAULTHULLWEIGHT = 1000;
        public const int DEFAULTHULLSPACES = 20;
        public const int DEFAULTTURRETSPACES = 2;

        public MockEquipmentRepository() {
            mockPackTotalPower = DEFAULTPPPOWER;
            mockPackChargePerTurn = DEFAULTPPCHARGE;
            mockPowerPackSpeed = DEFAULTPPMAXSPEED;
            mockPowerPackAcceleration = DEFAULTPPACCELERATION;
        }

        public void Mock_SetScannerProperties(int definedResult, int chargeConsumedOnUse = 45) {
            definedScannerBehaviour = definedResult;
            mockScannerchargeConsumedOnUse = chargeConsumedOnUse;
        }

        protected override EquipmentItem ActualLoadEquipmentItemById(int identity) {
            switch (identity) {
                case KnownEquipmentIds.MOCKINSTALLEVERYWHERE: return GetMockEquipment(identity);
                case KnownEquipmentIds.MOCKINSTALLINTERNALONLY: return GetMockEquipment(identity, false, true);
                case KnownEquipmentIds.MOCKSCANNER: return GetMockScanner(identity);
                case KnownEquipmentIds.MOCKPOWERPACK: return GetMockPowerPack(identity);
                case KnownEquipmentIds.MOCKPROJECTILEWEAPON: return GetMockProjectileWeapon(identity);
                case KnownEquipmentIds.MOCKWAYTOOBIG: return GetMockOversizedItem(identity);
                default:
                    throw new InvalidOperationException("Mock repository does not support that id");
            }
        }

        private EquipmentItem GetMockOversizedItem(int identity) {
            var result = new OffensiveWeaponEquipmentItem();
            result.DisplayName = "Won t fit";
            result.SpaceRequired = 999999;
            result.BaseWeight = 0;
            result.MakeExternalInstallsPermitted();
            result.AddPermittedInstallPosition(MountPoint.Internal);
            return result;
        }

        private EquipmentItem GetMockProjectileWeapon(int identity) {
            var result = new OffensiveWeaponEquipmentItem();
            result.MakeExternalInstallsPermitted();
            result.DisplayName = "Mock Projectile";
            result.Classification = ItemClassification.OffsensiveWeapon;
            result.D10DamageRolls = 1;
            result.InitialAmmunition = initalAmmo;
            result.BaseHitChance = baseHitChance;
            result.UniqueId = identity;
            result.BaseChargeCost = chargeCostPerShot;
            result.SpaceRequired = weapionSpaceRequired;
            result.BaseWeight = weaponTotalWeight;
            return result;
        }

        private EquipmentItem GetMockPowerPack(int identity) {
            var result = new PowerPackEquipmentItem();
            result.Acceleration = mockPowerPackAcceleration;
            result.ChargePerTurn = mockPackChargePerTurn;
            result.TotalPower = mockPackTotalPower;
            result.Classification = ItemClassification.PowerPack;
            result.UniqueId = identity;
            result.SetPowerDrainLevels(17, 57, 127);
            result.MaxSpeed = mockPowerPackSpeed;
            foreach (var mp in EquipmentSupport.ListAllInternallMountPoints()) {
                result.AddPermittedInstallPosition(mp);
            }

            return result;
        }

        private EquipmentItem GetMockScanner(int identity) {
            var result = new ScannerEquipmentItem() {
                UniqueId = identity,
                Classification = ItemClassification.Scanner,
                DisplayName = "Mock Scanner",
                ChargeConsumed = mockScannerchargeConsumedOnUse
            };
            foreach (var mp in EquipmentSupport.ListAllInternallMountPoints()) {
                result.AddPermittedInstallPosition(mp);
            }

            // Set up the scanner behaviour based on which type of scanner was selected.
            if ((definedScannerBehaviour == 0) || (definedScannerBehaviour == 1)) {
                // Default
                for (int x = -10; x <= 10; x++) {
                    for (int y = -10; y <= 10; y++) {
                        result.AddScannedOffsetPoint(new Point(x, y));
                    }
                }
            }
            return result;
        }

        private EquipmentItem GetMockEquipment(int identity, bool externalMount = true, bool internalMount = true) {
            var result = new EquipmentItem() {
                UniqueId = identity,
                DisplayName = "MockEquipment : " + identity.ToString(),
                Classification = ItemClassification.MockEquipment
            };

            if (externalMount) {
                foreach (var mp in EquipmentSupport.ListAllExternalMountPoints()) {
                    result.AddPermittedInstallPosition(mp);
                }
            }

            if (internalMount) {
                foreach (var mp in EquipmentSupport.ListAllInternallMountPoints()) {
                    result.AddPermittedInstallPosition(mp);
                }
            }
            return result;
        }

        public void Mock_SetPowerPackProperties(int chargept, int totalPower = 4587, int maxSpeed = 10, int acceleration = 2) {
            mockPackChargePerTurn = chargept;
            mockPackTotalPower = totalPower;
            mockPowerPackSpeed = maxSpeed;
            mockPowerPackAcceleration = acceleration;
        }

        public void Mock_SetWeaponProperties(int hitChance, int initialAmmoSet = 20, int ccps = 30) {
            baseHitChance = hitChance;
            initalAmmo = initialAmmoSet;
            chargeCostPerShot = ccps;
        }

        public void Mock_SetWeaponExtendedProperties(int spacesConsumed, int weight = 100) {
            weapionSpaceRequired = spacesConsumed;
            weaponTotalWeight = weight;
        }

        protected override BotFrame ActualLoadBotFrame(int idToLoad) {
            var result =  new BotFrame() {
                Name = "Mock Frame "+idToLoad.ToString()
            };
            result.SetSpace(5, 5, 5, 5, 5, 5);
            return result;

        }
    }
}