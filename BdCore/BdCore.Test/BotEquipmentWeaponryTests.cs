namespace Plisky.Boondoggle2.Test {
    using Plisky.Plumbing;
    using Plisky.Test;
    using System;
    using Xunit;

    public class BotEquipmentWeaponryTests {

        [Fact(DisplayName = nameof(Combat_MockWeaponSetUpEnvironment))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Combat_MockWeaponSetUpEnvironment() {
            TestPreparedWorld tpw = TestUtils.CreateWorldWithBotsReadyForCombat();

            Assert.NotNull(tpw.Engine); 
            Assert.NotNull(tpw.Engine);
            Assert.NotNull(tpw.Engine);
        }

        [Fact(DisplayName = nameof(Bot_FireWeapon_IncreasesUseCount))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Bot_FireWeapon_IncreasesUseCount() {

            TestPreparedWorld tpw = TestUtils.CreateWorldWithBotsReadyForCombat(4);

            Guid weaponGuid = tpw.Bot1.Mock_GetWeaponEquipmentInstanceId("Gun_Turret").InstanceId;
            var ae = tpw.Engine.Mock_GetEquipmentInstanceById(weaponGuid);
            int currentUseCount = ae.UseCount;
            var res = tpw.Bot1.FireWeapon(4,"Gun_Turret");

            Assert.Equal(UsageEndState.Success, res.State);
            Assert.Equal(currentUseCount + 1, ae.UseCount); 

        }

        [Fact(DisplayName = nameof(Bot_FireWeaponPassesScanResultAsIParam))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Bot_FireWeaponPassesScanResultAsIParam() {
            Guid weaponGuid = Guid.Empty;

            var mbf = new MockBotFactory().CreateBasicMockBot().WithItemSupport().WithMockActionProvider().WithItemSupport().WithEquipmentCallback(bb => {
                var ae = bb.InstallEquipment(KnownEquipmentIds.MOCKPROJECTILEWEAPON, "test", MountPoint.Turret);
            Assert.NotNull(ae); 
                weaponGuid = ae.InstanceId;
            });
            var sut = mbf.ToBot();
            var eur = sut.FireWeapon(3, "test");

            var eud = mbf.MockActionProviderUsed.Mock_GetMostRecentEUD();
            var ap = mbf.MockActionProviderUsed.Mock_GetEquipemntByGuid(weaponGuid);


            Assert.Equal(UsageEndState.Success, eur.State);
            Assert.True(eud.IParam == 3);
            Assert.True(eud.InstanceIdentity == weaponGuid); 
        }

        [Fact(DisplayName = nameof(Combat_FireProjectileWeaponDepletesAmmo))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Combat_FireProjectileWeaponDepletesAmmo() {
            TestPreparedWorld tpw = TestUtils.CreateWorldWithBotsReadyForCombat(4);

            var ae = tpw.Bot1.GetEquipment("Gun_Turret");
            var aeE = tpw.Engine.Mock_GetEquipmentInstanceById(ae.InstanceId);
            int remainingRounds = aeE.RoundsRemaining;
            var eur = tpw.Bot1.FireWeapon(4, "Gun_Turret");
            int afterShotRemaining = aeE.RoundsRemaining;

            Assert.True(eur.State == UsageEndState.Success, "The weapon did not fire correctly");
            Assert.True(remainingRounds > afterShotRemaining, "the ammo was not depleted when firing the weapon");
        }

        [Fact(DisplayName = nameof(Combat_CantFireWhenNoCharge))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Combat_CantFireWhenNoCharge() {
            MockEquipmentRepository mor = new MockEquipmentRepository();
            mor.Mock_SetWeaponProperties(100, 10, 89);
            TestPreparedWorld tpw = TestUtils.CreateWorldWithBotsReadyForCombat(4, mor);

            tpw.Engine.Mock_DirectSetBotCharge(tpw.Bot1.PublicId, 0);
            var eur = tpw.Bot1.FireWeapon(4, "Gun_Turret");
            Assert.Equal(UsageEndState.Fail_NoCharge, eur.State); 
        }



        [Fact(DisplayName = nameof(Combat_FireProjectileWeapon_ConsumesCharge))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Combat_FireProjectileWeapon_ConsumesCharge() {
            MockEquipmentRepository mor = new MockEquipmentRepository();
            mor.Mock_SetWeaponProperties(100, 10, 89);
            mor.Mock_SetPowerPackProperties(123);
            TestPreparedWorld tpw = TestUtils.CreateWorldWithBotsReadyForCombat(4, mor);

            var ae = tpw.Bot1.GetEquipment("Gun_Turret");

            var mo = tpw.Engine.Mock_GetBotMapOBjectByPublicId(tpw.Bot1.PublicId);
            int charge = mo.ChargeRemaining;
            var eur = tpw.Bot1.FireWeapon(4, "Gun_Turret");
            int charge2 = mo.ChargeRemaining;

            Assert.True(eur.State == UsageEndState.Success, "The weapon did not fire correctly - " + eur.State.ToString());
            Assert.True(charge > charge2, "No charge was consumed when the weapon was fired");
            Assert.Equal(charge - charge2, 89); 
        }

        [Fact(DisplayName = nameof(Combat_UseScanner_ConsumesCharge))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Combat_UseScanner_ConsumesCharge() {
            MockEquipmentRepository mor = new MockEquipmentRepository();
            mor.Mock_SetScannerProperties(0, 23);
            mor.Mock_SetPowerPackProperties(123);
            TestPreparedWorld tpw = TestUtils.CreateWorldWithBotsReadyForCombat(4, mor);

            var mo = tpw.Engine.Mock_GetBotMapOBjectByPublicId(tpw.Bot1.PublicId);
            int charge = mo.ChargeRemaining;
            tpw.Bot1.UseEquipment("Scanner");
            int charge2 = mo.ChargeRemaining;

            Assert.True(charge > charge2, "No charge was consumed when the weapon was fired");
            Assert.Equal(23, charge - charge2); 
        }

        [Fact(DisplayName = nameof(Weapon_HasCorrectInitialAmmoAllocation))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Weapon_HasCorrectInitialAmmoAllocation() {
            MockEquipmentRepository mor = new MockEquipmentRepository();
            mor.Mock_SetWeaponProperties(100, 11);
            TestPreparedWorld tpw = TestUtils.CreateWorldWithBotsReadyForCombat(4, mor);

            var ae = tpw.Bot1.GetEquipment("Gun_Turret");
            int initialRounds = tpw.Engine.Mock_GetEquipmentInstanceById(ae.InstanceId).RoundsRemaining;
            Assert.Equal(11, initialRounds); 
        }

        [Fact(DisplayName = nameof(Combat_ZeroProjectileAmmo_CantFire))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Combat_ZeroProjectileAmmo_CantFire() {
            TestPreparedWorld tpw = TestUtils.CreateWorldWithBotsReadyForCombat(4);

            var ae = tpw.Bot1.GetEquipment("Gun_Turret");
            tpw.Engine.Mock_GetEquipmentInstanceById(ae.InstanceId).RoundsRemaining = 1;

            var aeE = tpw.Engine.Mock_GetEquipmentInstanceById(ae.InstanceId);
            var eur = tpw.Bot1.FireWeapon(4, "Gun_Turret");
            int afterShotRemaining = aeE.RoundsRemaining;
            Assert.Equal(0, afterShotRemaining); 
            var eur2 = tpw.Bot1.FireWeapon(4, "Gun_Turret");
            Assert.Equal(UsageEndState.Fail_NoAmmo, eur2.State); 
        }

        [Fact(DisplayName = nameof(Combat_WeaponCooldownPreventsFire))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Combat_WeaponCooldownPreventsFire() {
            TestPreparedWorld tpw = TestUtils.CreateWorldWithBotsReadyForCombat(4);
            var ae = tpw.Bot1.GetEquipment("Gun_Turret");

            tpw.Engine.Mock_GetEquipmentInstanceById(ae.InstanceId).CooldownTicksRemaining = 1;

            var eur = tpw.Bot1.FireWeapon(4, "Gun_Turret");

            Assert.Equal(UsageEndState.Fail_CooldownActive, eur.State); 
        }

        [Fact(DisplayName = nameof(Combat_FireProjectileWeaponHitsTargetDoesDamage))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Combat_FireProjectileWeaponHitsTargetDoesDamage() {
            MockEquipmentRepository mor = new MockEquipmentRepository();
            mor.Mock_SetWeaponProperties(100);
            TestPreparedWorld tpw = TestUtils.CreateWorldWithBotsReadyForCombat(4, mor);

            int beforeLife = tpw.Engine.GetMappedBotById(tpw.GetBot2EngineId()).LifeRemaining;
            tpw.Engine.PerformNextTick();
            var eur = tpw.Bot1.FireWeapon(4, "Gun_Turret");

            Assert.True(eur.State == UsageEndState.Success, "The weapon did not fire correctly");
            int afterLife = tpw.Engine.GetMappedBotById(tpw.GetBot2EngineId()).LifeRemaining;
            Assert.True(afterLife < beforeLife); 
        }

        [Fact(DisplayName = nameof(Combat_FireProjectileFiresGeneratesMessage))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Combat_FireProjectileFiresGeneratesMessage() {
            TestPreparedWorld tpw = TestUtils.CreateWorldWithBotsReadyForCombat(4);
            Hub h = tpw.HubUsed;
            int msgHitCount = 0;
            h.LookFor<Message_Ui>((mui) => {
                if ((mui.MessageKind == MainMessageKind.UIMessage) && (mui.SubKind == KnownSubkinds.WeaponFire)) {
                    msgHitCount++;
                }
            });
            tpw.MockRepos.Mock_SetWeaponProperties(0);

            tpw.Engine.PerformNextTick();
            var eur = tpw.Bot1.FireWeapon(4, "Gun_Turret");

            Assert.Equal(1, msgHitCount); 
        }

        [Fact(DisplayName = nameof(Combat_FireProjectileWeaponHitsTargetGeneratesMessage))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Combat_FireProjectileWeaponHitsTargetGeneratesMessage() {
            MockEquipmentRepository mor = new MockEquipmentRepository();
            mor.Mock_SetWeaponProperties(100);
            TestPreparedWorld tpw = TestUtils.CreateWorldWithBotsReadyForCombat(4, mor);
            int messageRecieved = 0;
            tpw.HubUsed.LookFor<Message_Ui>(uim => {
                if (uim.SubKind == KnownSubkinds.WeaponFire) {
                    messageRecieved++;
                }
            });

            tpw.Engine.PerformNextTick();
            var eur = tpw.Bot1.FireWeapon(4, "Gun_Turret");

            Assert.Equal(1, messageRecieved); 
        }

        [Fact(DisplayName = nameof(Combat_VictimRecievesDamageNoteNextTurn))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Combat_VictimRecievesDamageNoteNextTurn() {
            MockEquipmentRepository mor = new MockEquipmentRepository();
            mor.Mock_SetWeaponProperties(100);
            TestPreparedWorld tpw = TestUtils.CreateWorldWithBotsReadyForCombat(4, mor);

            tpw.Engine.Mock_SetBotLife(tpw.Bot2.PublicId, 1000);
            tpw.Engine.PerformNextTick();
            var eur = tpw.Bot1.FireWeapon(4, "Gun_Turret");
            Assert.True(eur.State == UsageEndState.Success, "The weapon did not fire correctly");
            tpw.Engine.PerformNextTick();
            var ad = tpw.Bot2.Mock_GetLTR();

            Assert.NotNull(ad); 
            Assert.True(ad.Events.Count == 1, "There should have been a fire event");
        }
    }
}