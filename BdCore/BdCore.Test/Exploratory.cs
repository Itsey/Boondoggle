using Plisky.Diagnostics;
using Plisky.Plumbing;
using Plisky.Test;
using System;
using System.Diagnostics;
using System.Drawing;
using Xunit;

namespace Plisky.Boondoggle2.Test {
    public class Exploratory {
        protected Bilge b = new Bilge(tl: TraceLevel.Off);

        [Fact(DisplayName = nameof(Exploratory_One))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Exploratory_One() {
            b.Info.Flow();

            var mb = new BasicMockBot();
        }


       
    }
}
