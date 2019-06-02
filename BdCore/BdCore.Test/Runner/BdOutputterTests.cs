namespace Plisky.Boondoggle2.Test {
    using Plisky.Boondoggle2;
    using Plisky.Boondoggle2.Runner;
    using Plisky.Boondoggle2.Test;
    using Plisky.Diagnostics;
    using Plisky.Plumbing;
    using Plisky.Test;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Text;
    using Xunit;


    public class BdOutputterTests {
        protected Bilge b = new Bilge(tl: TraceLevel.Verbose);

        const string BN = "BATTLENAME";

        [Fact(DisplayName = nameof(OutputterSetsNameOnInitialise))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void OutputterSetsNameOnInitialise() {

            var mut = new MockBdOutputter();
            BaseBdOutputter sut = mut;

            sut.Initialise(BN);

            Assert.Equal(BN, mut.GetBattleName());
        }


        [Fact(DisplayName = nameof(TurnAndTick_StartAtZero))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void TurnAndTick_StartAtZero() {
            b.Info.Flow();

            var mut = new MockBdOutputter();
            BaseBdOutputter sut = mut;
            sut.Initialise(BN);

            Assert.Equal(0, mut.GetTurn());
            Assert.Equal(0, mut.GetTick());
        }


        [Fact(DisplayName = nameof(UIMessages_Processed))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void UIMessages_Processed() {
            b.Info.Flow();

            Hub testHub = new Hub();
            var mut = new MockBdOutputter();
            BaseBdOutputter sut = mut;
            sut.Initialise(BN);
            sut.InjectHub(testHub);  // Uses On Hub Changed

            Assert.Equal(0, mut.UIMessageCount);
            // Status has no context or anything.
            testHub.Launch<Message_Ui>(new Message_Ui(MainMessageKind.BotActivity, KnownSubkinds.BotStatus));
            Assert.Equal(1, mut.UIMessageCount);

        }

        [Fact(DisplayName = nameof(GameMessages_Processed))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void GameMessages_Processed() {
            b.Info.Flow();

            Hub testHub = new Hub();
            var mut = new MockBdOutputter();
            BaseBdOutputter sut = mut;
            sut.InjectHub(testHub);  // Injects before register
            sut.Initialise(BN);
            

            Assert.Equal(0, mut.GameMessageCount);
            testHub.Launch<Message_Game>(new Message_Game(MainMessageKind.BotActivity, KnownSubkinds.BattleStarts));
            Assert.Equal(1, mut.GameMessageCount);

        }

        [Fact(DisplayName = nameof(CombatMessages_Processed))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void CombatMessages_Processed() {
            b.Info.Flow();

            Hub testHub = new Hub();
            var mut = new MockBdOutputter();
            BaseBdOutputter sut = mut;
            sut.Initialise(BN);
            sut.InjectHub(testHub);

            Assert.Equal(0, mut.CombatMessageCount);
            testHub.Launch<Message_GameCombat>(new Message_GameCombat(MainMessageKind.BotActivity, KnownSubkinds.BattleStarts));
            Assert.Equal(1, mut.CombatMessageCount);

        }
    }

}