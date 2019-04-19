namespace Plisky.Boondoggle2.Test {
    using Plisky.Boondoggle2;
    using Plisky.Diagnostics;
    using Plisky.Test;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using Xunit;

    public class MappedBotTests {

        protected Bilge b = new Bilge(tl: TraceLevel.Off);



        [Fact(DisplayName = nameof(StartsActive))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Exploratory)]
        public void StartsActive() {
            var mbf = new MockBotFactory();
            MockMappedBot atk = new MockMappedBot(mbf.CreateBasicMockBot().ToBot());
            atk.Test_Initialise();
            Assert.True(atk.IsActive);
        }

        [Fact(DisplayName = nameof(StartsAlive))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Exploratory)]
        public void StartsAlive() {
            var mbf = new MockBotFactory();
            MockMappedBot atk = new MockMappedBot(mbf.CreateBasicMockBot().ToBot());
            atk.Test_Initialise();
            Assert.True(atk.IsAlive());

        }
    }
}
