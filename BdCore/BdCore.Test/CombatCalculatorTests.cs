namespace Plisky.Boondoggle2.Test {
    using Plisky.Boondoggle2;
    using Plisky.Diagnostics;
    using Plisky.Test;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Text;
    using Xunit;

    public class CombatCalculatorTests {

        protected Bilge b = new Bilge(tl: TraceLevel.Off);


        [Theory(DisplayName = nameof(CanHit_VictimDeadAhead))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        #region test data
        [InlineData(0, MountPoint.Turret,true)]
        [InlineData(90, MountPoint.Turret, true)]
        [InlineData(180, MountPoint.Turret, true)]
        [InlineData(270, MountPoint.Turret, true)]
        // Forward
        [InlineData(0, MountPoint.Forward, true)]
        [InlineData(90, MountPoint.Forward, false)]
        [InlineData(180, MountPoint.Forward, false)]
        [InlineData(270, MountPoint.Forward, false)]
        // Right
        [InlineData(0, MountPoint.Nearside, false)]
        [InlineData(90, MountPoint.Nearside, false)]
        [InlineData(180, MountPoint.Nearside, false)]
        [InlineData(270, MountPoint.Nearside, true)]
        // Backward
        [InlineData(0, MountPoint.Backward, false)]
        [InlineData(90, MountPoint.Backward, false)]
        [InlineData(180, MountPoint.Backward, true)]
        [InlineData(270, MountPoint.Backward, false)]
        // Left
        [InlineData(0, MountPoint.Offside, false)]
        [InlineData(90, MountPoint.Offside, true)]
        [InlineData(180, MountPoint.Offside, false)]
        [InlineData(270, MountPoint.Offside, false)]
        // Internal
        [InlineData(0, MountPoint.Internal, false)]
        [InlineData(90, MountPoint.Internal, false)]
        [InlineData(180, MountPoint.Internal, false)]
        [InlineData(270, MountPoint.Internal, false)]
        #endregion
        public void CanHit_VictimDeadAhead(double atkHeading, MountPoint weapPoint, bool shouldHit) {
            b.Info.Flow();
            var atkPos = new Point(3, 3);
            var vikPos = new Point(3, 4);

            Bd2CombatCalculator sut = new DefaultCalcsRules();
            var canHit  =sut.CanMountPointHitTarget(atkHeading, weapPoint, atkPos,vikPos);

            Assert.Equal(shouldHit, canHit);
        }


        [Theory(DisplayName = nameof(CanHit_VictimBehind))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        #region test data
        [InlineData(0, MountPoint.Turret, true)]
        [InlineData(90, MountPoint.Turret, true)]
        [InlineData(180, MountPoint.Turret, true)]
        [InlineData(270, MountPoint.Turret, true)]
        // Forward
        [InlineData(0, MountPoint.Forward, false)]
        [InlineData(90, MountPoint.Forward, false)]
        [InlineData(180, MountPoint.Forward, true)]
        [InlineData(270, MountPoint.Forward, false)]
        // Right
        [InlineData(0, MountPoint.Nearside, false)]
        [InlineData(90, MountPoint.Nearside, true)]
        [InlineData(180, MountPoint.Nearside, false)]
        [InlineData(270, MountPoint.Nearside, false)]
        // Backward
        [InlineData(0, MountPoint.Backward, true)]
        [InlineData(90, MountPoint.Backward, false)]
        [InlineData(180, MountPoint.Backward, false)]
        [InlineData(270, MountPoint.Backward, false)]
        // Left
        [InlineData(0, MountPoint.Offside, false)]
        [InlineData(90, MountPoint.Offside, false)]
        [InlineData(180, MountPoint.Offside, false)]
        [InlineData(270, MountPoint.Offside, true)]
        // Internal
        [InlineData(0, MountPoint.Internal, false)]
        [InlineData(90, MountPoint.Internal, false)]
        [InlineData(180, MountPoint.Internal, false)]
        [InlineData(270, MountPoint.Internal, false)]
        #endregion
        public void CanHit_VictimBehind(double atkHeading, MountPoint weapPoint, bool shouldHit) {
            b.Info.Flow();
            var atkPos = new Point(3, 3);
            var vikPos = new Point(3, 2);

            Bd2CombatCalculator sut = new DefaultCalcsRules();
            var canHit = sut.CanMountPointHitTarget(atkHeading, weapPoint, atkPos, vikPos);

             Assert.Equal(shouldHit, canHit);
        }


        [Theory(DisplayName = nameof(CanHit_VictimRight))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        #region test data
        [InlineData(0, MountPoint.Turret, true)]
        [InlineData(90, MountPoint.Turret, true)]
        [InlineData(180, MountPoint.Turret, true)]
        [InlineData(270, MountPoint.Turret, true)]
        // Forward
        [InlineData(0, MountPoint.Forward, false)]
        [InlineData(90, MountPoint.Forward, true)]
        [InlineData(180, MountPoint.Forward, false)]
        [InlineData(270, MountPoint.Forward, false)]
        // Right
        [InlineData(0, MountPoint.Nearside, true)]
        [InlineData(90, MountPoint.Nearside, false)]
        [InlineData(180, MountPoint.Nearside, false)]
        [InlineData(270, MountPoint.Nearside, false)]
        // Backward
        [InlineData(0, MountPoint.Backward, false)]
        [InlineData(90, MountPoint.Backward, false)]
        [InlineData(180, MountPoint.Backward, false)]
        [InlineData(270, MountPoint.Backward, true)]
        // Left
        [InlineData(0, MountPoint.Offside, false)]
        [InlineData(90, MountPoint.Offside, false)]
        [InlineData(180, MountPoint.Offside, true)]
        [InlineData(270, MountPoint.Offside, false)]
        // Internal
        [InlineData(0, MountPoint.Internal, false)]
        [InlineData(90, MountPoint.Internal, false)]
        [InlineData(180, MountPoint.Internal, false)]
        [InlineData(270, MountPoint.Internal, false)]
        #endregion
        public void CanHit_VictimRight(double atkHeading, MountPoint weapPoint, bool shouldHit) {
            b.Info.Flow();
            var atkPos = new Point(3, 3);
            var vikPos = new Point(4, 3);

            Bd2CombatCalculator sut = new DefaultCalcsRules();
            var canHit = sut.CanMountPointHitTarget(atkHeading, weapPoint, atkPos, vikPos);

            Assert.Equal(shouldHit, canHit);
        }



        [Theory(DisplayName = nameof(CanHit_VictimLeft))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        #region test data
        [InlineData(0, MountPoint.Turret, true)]
        [InlineData(90, MountPoint.Turret, true)]
        [InlineData(180, MountPoint.Turret, true)]
        [InlineData(270, MountPoint.Turret, true)]
        // Forward
        [InlineData(0, MountPoint.Forward, false)]
        [InlineData(90, MountPoint.Forward, false)]
        [InlineData(180, MountPoint.Forward, false)]
        [InlineData(270, MountPoint.Forward, true)]
        // Right
        [InlineData(0, MountPoint.Nearside, false)]
        [InlineData(90, MountPoint.Nearside, false)]
        [InlineData(180, MountPoint.Nearside, true)]
        [InlineData(270, MountPoint.Nearside, false)]
        // Backward
        [InlineData(0, MountPoint.Backward, false)]
        [InlineData(90, MountPoint.Backward, true)]
        [InlineData(180, MountPoint.Backward, false)]
        [InlineData(270, MountPoint.Backward, false)]
        // Left
        [InlineData(0, MountPoint.Offside, true)]
        [InlineData(90, MountPoint.Offside, false)]
        [InlineData(180, MountPoint.Offside, false)]
        [InlineData(270, MountPoint.Offside, false)]
        // Internal
        [InlineData(0, MountPoint.Internal, false)]
        [InlineData(90, MountPoint.Internal, false)]
        [InlineData(180, MountPoint.Internal, false)]
        [InlineData(270, MountPoint.Internal, false)]
        #endregion
        public void CanHit_VictimLeft(double atkHeading, MountPoint weapPoint, bool shouldHit) {
            b.Info.Flow();
            var atkPos = new Point(3, 3);
            var vikPos = new Point(2, 3);

            Bd2CombatCalculator sut = new DefaultCalcsRules();
            var canHit = sut.CanMountPointHitTarget(atkHeading, weapPoint, atkPos, vikPos);

            Assert.Equal(shouldHit, canHit);
        }


    }
}
