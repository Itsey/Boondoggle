namespace Plisky.Boondoggle2.Test {
    using Plisky.Boondoggle2;
    using Plisky.Boondoggle2.Test;
    using Plisky.Plumbing;
    using Plisky.Test;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text;
    using Xunit;


    public class BotEquipmentTests {

        [Fact(DisplayName = nameof(Equipment_InstallNoPrepare_Blows))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Equipment_InstallNoPrepare_Blows() {
            Assert.Throws<BdBaseException>(() => {
                var sut = new MockBotFactory().CreateBasicMockBot().ToBot();
                sut.InstallEquipment(KnownEquipmentIds.MOCKINSTALLEVERYWHERE, "FriendlyName", MountPoint.Internal);
            });
            
        }

        [Fact(DisplayName = nameof(Equipment_InstallDuplicateName_Blows))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Equipment_InstallDuplicateName_Blows() {
            Assert.Throws<BdBaseException>(() => {
                var sut = new MockBotFactory().CreateBasicMockBot().WithMockActionProvider().WithItemSupport().ToBot();
                sut.InstallEquipment(KnownEquipmentIds.MOCKINSTALLEVERYWHERE, "FriendlyName", MountPoint.Internal);
                sut.InstallEquipment(KnownEquipmentIds.MOCKINSTALLEVERYWHERE, "FriendlyName", MountPoint.Internal);
            });
        }

        [Fact(DisplayName = nameof(Equipment_GtePowerpack_GetsPowerpack))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Equipment_GtePowerpack_GetsPowerpack() {
                
                    var sut = new MockBotFactory().CreateBasicMockBot().WithMockActionProvider().WithItemSupport().ToBot();
            sut.InstallEquipment(KnownEquipmentIds.MOCKPOWERPACK, "PP", MountPoint.Internal);
            var pp = sut.GetPowerPack();

            Assert.NotNull(pp); 
            Assert.True(pp.EquipmentId == KnownEquipmentIds.MOCKPOWERPACK, "The wrong equipment was installed");
        }

        [Fact(DisplayName = nameof(Equipment_MountTwoPowerpacks_SecondFails))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Equipment_MountTwoPowerpacks_SecondFails() {
            var sut = new MockBotFactory().CreateBasicMockBot().WithMockActionProvider().WithItemSupport().ToBot();
            var r1 = sut.InstallEquipment(KnownEquipmentIds.MOCKPOWERPACK, "PP1", MountPoint.Internal);
            var r2 = sut.InstallEquipment(KnownEquipmentIds.MOCKPOWERPACK, "PP2", MountPoint.Internal);

            Assert.NotNull(r1);
            Assert.Null(r2);
        }

        [Fact(DisplayName = nameof(Equipment_InstallBadIdentity_Blows))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]

        public void Equipment_InstallBadIdentity_Blows() {
            Assert.Throws<BdBaseException>(() => {
                var sut = new MockBotFactory().CreateBasicMockBot().ToBot();
                sut.InstallEquipment(-1, "FriendlyName", MountPoint.Internal);
            });
        }

        [Fact(DisplayName = nameof(Equipment_UseWithInvalidGuid_Blows))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Equipment_UseWithInvalidGuid_Blows() {
            Assert.Throws<BdBaseException>(() => {
                Guid g = Guid.NewGuid();
                var sut = new MockBotFactory().CreateBasicMockBot().ToBot();

                sut.UseEquipment(g);
            });
        }

        [Fact(DisplayName = nameof(Equipment_UseByGuid_Works))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Equipment_UseByGuid_Works() {
            var sut = new MockBotFactory().CreateBasicMockBot().WithItemSupport().WithMockActionProvider().ToBot();
            var ai = sut.InstallEquipment(KnownEquipmentIds.MOCKINSTALLEVERYWHERE, "DummyUseEquipment1", MountPoint.Internal);

            var result = sut.UseEquipment(ai.InstanceId);

            Assert.True(result.State == UsageEndState.Success, "The usage must be a success");
        }

        [Fact(DisplayName = nameof(Equipment_UseByName_UsesSameItem))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Equipment_UseByName_UsesSameItem() {
            var fac = new MockBotFactory().CreateBasicMockBot().WithItemSupport().WithMockActionProvider();
            var sut = fac.ToBot();
            var ai = sut.InstallEquipment(KnownEquipmentIds.MOCKINSTALLEVERYWHERE, "DummyUseEquipment1", MountPoint.Internal);
            Assert.True(ai.Result == InstallationResult.Installed, "The prerequisite for this test is that the equipment installs");
            var result = sut.UseEquipment(ai.InstanceId);

            result = sut.UseEquipment("DummyUseEquipment1");
            int ae = fac.MockActionProviderUsed.Mock_GetEquipmentUseCount(ai.InstanceId);

            Assert.True(result.State == UsageEndState.Success, "The usage must be a success");
            Assert.Equal(2, ae);
        }

        [Fact(DisplayName = nameof(Equipment_UseByName_InvalidNAmeBlows))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Equipment_UseByName_InvalidNAmeBlows() {
            Assert.Throws<BdBaseException>(() => {
                var sut = new MockBotFactory().CreateBasicMockBot().WithItemSupport().WithMockActionProvider().ToBot();
                var ai = sut.InstallEquipment(KnownEquipmentIds.MOCKINSTALLEVERYWHERE, "DummyUseEquipment1", MountPoint.Internal);
                var result = sut.UseEquipment("monkey fish");
            });
        }


        [Fact(DisplayName = nameof(Equipment_Install_ReturnsSuccess))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Equipment_Install_ReturnsSuccess() {
            var sut = new MockBotFactory().CreateBasicMockBot().WithMockActionProvider().WithItemSupport().ToBot();
            BasicMockBot bmb = (BasicMockBot)sut;

            int preInstall = bmb.GetEquipmentCount();
            var kit = sut.InstallEquipment(KnownEquipmentIds.MOCKINSTALLEVERYWHERE, "Teste Equipment1", MountPoint.Internal);

            Assert.NotNull(kit);
            Assert.Equal(InstallationResult.Installed, kit.Result);
        }

        [Fact(DisplayName = nameof(Equipment_Install_AddsToCount))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Equipment_Install_AddsToCount() {
            var sut = new MockBotFactory().CreateBasicMockBot().WithMockActionProvider().WithItemSupport().ToBot();
            BasicMockBot bmb = (BasicMockBot)sut;

            int preInstall = bmb.GetEquipmentCount();
            var kit = sut.InstallEquipment(KnownEquipmentIds.MOCKINSTALLEVERYWHERE, "Teste Equipment1", MountPoint.Internal);
            int postInstall = bmb.GetEquipmentCount();

            Assert.Equal(preInstall + 1, postInstall); 
        }

        [Fact(DisplayName = nameof(Equipment_InstallForward_AddsToForwardCount))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Equipment_InstallForward_AddsToForwardCount() {
            var mbf = new MockBotFactory().CreateBasicMockBot().WithMockActionProvider().WithItemSupport();
            var sut = mbf.ToBot();

            var map = mbf.MockActionProviderUsed;

            BasicMockBot bmb = (BasicMockBot)sut;

            int preInstall = map.Mock_GetEquipmentCountAtPosition(MountPoint.Forward);
            var kit = sut.InstallEquipment(KnownEquipmentIds.MOCKINSTALLEVERYWHERE, "Teste Equipment1", MountPoint.Forward);
            int postInstall = map.Mock_GetEquipmentCountAtPosition(MountPoint.Forward);

            Assert.Equal(preInstall + 1, postInstall); 
        }

        [Fact(DisplayName = nameof(Equipment_InstallTurret_AddsToTurretCount))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Equipment_InstallTurret_AddsToTurretCount() {
            var mbf = new MockBotFactory().CreateBasicMockBot().WithMockActionProvider().WithItemSupport();
            var sut = mbf.ToBot();

            var map = mbf.MockActionProviderUsed;
            BasicMockBot bmb = (BasicMockBot)sut;
            int preInstall = map.Mock_GetEquipmentCountAtPosition(MountPoint.Turret);

            var kit = sut.InstallEquipment(KnownEquipmentIds.MOCKINSTALLEVERYWHERE, "Teste Equipment1", MountPoint.Turret);

            int postInstall = map.Mock_GetEquipmentCountAtPosition(MountPoint.Turret);
            Assert.Equal(preInstall + 1, postInstall);
        }

        [Fact(DisplayName = nameof(Equipment_TooBigForTurrent_FailsToInstall))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Equipment_TooBigForTurrent_FailsToInstall() {

            var sut = new MockBotFactory().CreateBasicMockBot().WithMockActionProvider().WithItemSupport().ToBot();

            var kit = sut.InstallEquipment(KnownEquipmentIds.MOCKWAYTOOBIG, "Test Equipment1", MountPoint.Turret);
            BasicMockBot bmb = (BasicMockBot)sut;

            Assert.NotNull(kit);
            Assert.Equal(InstallationResult.Fail_NoSpace, kit.Result);
        }

        [Fact(DisplayName = nameof(Equipment_InstallWrongExternal_ReturnsCorrectError))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Equipment_InstallWrongExternal_ReturnsCorrectError() {
            var sut = new MockBotFactory().CreateBasicMockBot().WithMockActionProvider().WithItemSupport().ToBot();
            var kit = sut.InstallEquipment(KnownEquipmentIds.MOCKINSTALLINTERNALONLY, "Teste Equipment1", MountPoint.Forward);

            Assert.NotNull(kit);
            Assert.Equal(InstallationResult.Fail_InvalidMountpoint, kit.Result);
        }

        [Fact(DisplayName = nameof(Item_RegisterForExternal_Works))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Item_RegisterForExternal_Works() {
            EquipmentItem sut = new EquipmentItem();
            sut.MakeExternalInstallsPermitted();

            ValidateFitPosition(sut, MountPoint.Forward);
            ValidateFitPosition(sut, MountPoint.Backward);
            ValidateFitPosition(sut, MountPoint.Nearside);
            ValidateFitPosition(sut, MountPoint.Offside);
        }

        private static void ValidateFitPosition(EquipmentItem sut, MountPoint position) {
            Assert.True(sut.IsInstallationPermitted(position), "The " + position.ToString() + "mount point must be permitted when setting external");
        }

    }
}
