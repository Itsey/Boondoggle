using Plisky.Boondoggle2;
using Plisky.Boondoggle2.Test;
using Plisky.Diagnostics;
using Plisky.Test;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xunit;

namespace Plisky.Boondoggle2.Test  {
    public class CombatManagerTests {


        protected Bilge b = new Bilge(tl: TraceLevel.Off);
        



        [Fact(DisplayName = nameof(ResolveNullAttack_Throws))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Exploratory)]
        public void ResolveNullAttack_Throws() {

            var mckRand = new MockBd2Randomiser();
            MockBd2Map mp = new MockBd2Map();
            bd2World wld = new bd2World(mp);
            var sut = new CombatManager(wld, mckRand);
            CombatAttack ca = new CombatAttack();

            Assert.Throws<BdBaseException>(() => {
                var res = sut.ResolveAttack(null);
            });

        }

        [Fact(DisplayName = nameof(Resolve_NullAttacker_Throws))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Exploratory)]
        public void Resolve_NullAttacker_Throws() {
            var mckRand = new MockBd2Randomiser();
            MockBd2Map mp = new MockBd2Map();
            bd2World wld = new bd2World(mp);
            var sut = new CombatManager(wld, mckRand);

            MappedBot mb = new MappedBot(new MockBotFactory().CreateBasicMockBot().ToBot());
            CombatAttack ca = new CombatAttack();
            ca.Attacker = null;
            ca.Victim = mb;

            var excpt = Assert.Throws<BdBaseException>(() => {
                var res = sut.ResolveAttack(ca);
            });

            Assert.Contains("attacker", excpt.Message);

        }

        [Fact(DisplayName = nameof(Resolve_NullVictim_Throws))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Exploratory)]
        public void Resolve_NullVictim_Throws() {
            var mckRand = new MockBd2Randomiser();
            MockBd2Map mp = new MockBd2Map();
            bd2World wld = new bd2World(mp);
            var sut = new CombatManager(wld, mckRand);
            MappedBot mb = new MappedBot(new MockBotFactory().CreateBasicMockBot().ToBot());

            CombatAttack ca = new CombatAttack();
            ca.Attacker = mb;
            ca.Victim = null;
            var excpt = Assert.Throws<BdBaseException>(() => {
                var res = sut.ResolveAttack(ca);
            });

            Assert.Contains("victim", excpt.Message);


        }


        [Fact(DisplayName = nameof(Resolve_NoWeapon_Throws))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Exploratory)]
        public void Resolve_NoWeapon_Throws() {
            var mckRand = new MockBd2Randomiser();
            MockBd2Map mp = new MockBd2Map();
            bd2World wld = new bd2World(mp);
            var sut = new CombatManager(wld, mckRand);
            MappedBot mb = new MappedBot(new MockBotFactory().CreateBasicMockBot().ToBot());

            CombatAttack ca = new CombatAttack();
            ca.Attacker = mb;
            ca.Victim = mb;
            ca.Weapon = null;            

            var excpt = Assert.Throws<BdBaseException>(() => {
                var res = sut.ResolveAttack(ca);
            });

            Assert.Contains("weapon", excpt.Message);


        }


        [Fact(DisplayName = nameof(Resolve_NoWeapon2_Throws))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Exploratory)]
        public void Resolve_NoWeapon2_Throws() {
            var mckRand = new MockBd2Randomiser();
            MockBd2Map mp = new MockBd2Map();
            bd2World wld = new bd2World(mp);
            var sut = new CombatManager(wld, mckRand);
            MappedBot mb = new MappedBot(new MockBotFactory().CreateBasicMockBot().ToBot());

            CombatAttack ca = new CombatAttack();
            ca.Attacker = mb;
            ca.Victim = mb;
            ca.Weapon = new MockWeapon();
            ca.WeaponInstance = null;

            var excpt = Assert.Throws<BdBaseException>(() => {
                var res = sut.ResolveAttack(ca);
            });

            Assert.Contains("instance", excpt.Message);


        }


        [Fact(DisplayName = nameof(FullResolve_DoesHit))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void FullResolve_DoesHit() {
            b.Info.Flow();

            var mckRand = new MockBd2Randomiser();
            mckRand.Mock_SetD100Result(0);
            mckRand.Mock_SetD10Result(100);
            mckRand.Mock_SetCanTargetHit(true);

            MockBd2Map mp = new MockBd2Map();

            MockBd2World wld = new MockBd2World(mp);
            wld.Test_LOSCanHitReturn(true);
            var sut = new CombatManager(wld, mckRand);


            var mbf = new MockBotFactory();
            var atk = new MockMappedBot(mbf.CreateBasicMockBot().ToBot());
            atk.Test_Initialise();
            var vic = new MockMappedBot(mbf.CreateBasicMockBot().ToBot());
            vic.Test_Initialise();

            var mw = new MockWeapon();
            CombatAttack ca = new CombatAttack();
            ca.Attacker = atk;
            ca.Victim = vic;
            ca.Weapon = mw;
            ca.WeaponInstance = new ActiveEquipment(mw);
            ca.WeaponInstance.MountPoint = MountPoint.Turret;
            var res = sut.ResolveAttack(ca);

            Assert.NotNull(res);
            Assert.True(res.DidHit);

        }

    }
}
