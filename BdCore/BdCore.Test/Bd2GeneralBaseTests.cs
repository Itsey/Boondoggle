namespace Plisky.Boondoggle2.Test {
    
    using Plisky.Diagnostics;
    using Plisky.Test;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using Xunit;

    public class Bd2GeneralBaseTests {

        protected Bilge b = new Bilge(tl: TraceLevel.Off);

        [Fact(DisplayName = nameof(InjectBilge_Success))]
        [Trait(Traits.Age, Traits.Regression)]
        [Trait(Traits.Style, Traits.Unit)]
        public void InjectBilge_Success() {
            b.Info.Flow();

            var sut = new MockBd2GenealBase();
            Bilge mockBilge = new Bilge();
            sut.InjectBilge(mockBilge);

            Assert.Same(mockBilge, sut.GetAssignedBilge());

        }

        [Fact(DisplayName = nameof(DefaultTrace_Off))]
        [Trait(Traits.Age, Traits.Regression)]
        [Trait(Traits.Style, Traits.Unit)]
        public void DefaultTrace_Off() {
            b.Info.Flow();

            var sut = new MockBd2GenealBase();
            var tl = sut.GetTraceLevel();

            Assert.Equal<TraceLevel>(TraceLevel.Off, tl);

        }

      

        [Fact(DisplayName = nameof(SetTraceLevel_Correct))]
        [Trait(Traits.Age, Traits.Regression)]
        [Trait(Traits.Style, Traits.Unit)]
        public void SetTraceLevel_Correct() {
            b.Info.Flow();

            var sut = new MockBd2GenealBase();
            sut.InjectBilge(null, TraceLevel.Verbose);
            var tl = sut.GetTraceLevel();
            Assert.Equal<TraceLevel>(TraceLevel.Verbose, tl);

        }

    }
}
