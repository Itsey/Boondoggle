namespace Plisky.Boondoggle2.Test {
    using Plisky.Boondoggle2;    
    using Plisky.Plumbing;
    using Plisky.Test;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text;
    using Xunit;


    public class MockBotTests {

        
        [Fact(DisplayName = nameof(CreateMockBot_AllowsSetName))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void CreateMockBot_AllowsSetName() {
            var sut = new BasicMockBot();
            sut.Mock_SetNameVer("Nameeeee", "Verrrrr");
            Assert.Equal("Nameeeee", sut.Name);
        }

        [Fact(DisplayName = nameof(CreateMockBot_AllowsSetVersion))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void CreateMockBot_AllowsSetVersion() {
            var sut = new BasicMockBot();
            sut.Mock_SetNameVer("Nameeeee", "1.0.0.1");
            Assert.Equal("1.0.0.1", sut.Version);
        }

        [Fact(DisplayName = nameof(CreateMockBot_NullNameThrowsException))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void CreateMockBot_NullNameThrowsException() {
            Assert.Throws<BdBaseException>(() => {
                var sut = new BasicMockBot();
                sut.Mock_SetNameVer(null, "1.0.0.1");
            });

        }

        [Fact(DisplayName = nameof(CreateMockBot_EmptyNameThrowsException))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void CreateMockBot_EmptyNameThrowsException() {
            Assert.Throws<BdBaseException>(() => {
                BasicMockBot sut = new BasicMockBot();
                sut.Mock_SetNameVer("", "1.0.0.1");
            });
        }

        [Fact(DisplayName = nameof(MockBot_VersionNotNull))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void MockBot_VersionNotNull() {
            Assert.Throws<BdBaseException>(() => {
                BasicMockBot sut = new BasicMockBot();
                sut.Mock_SetNameVer("Nameeeee", null);
            });
        }

        [Fact(DisplayName = nameof(MockBot_MustHaveVersion))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void MockBot_MustHaveVersion() {
            Assert.Throws<BdBaseException>(() => {
                BasicMockBot sut = new BasicMockBot();
                sut.Mock_SetNameVer("Nameeeee", "");
            });
        }

        [Fact(DisplayName = nameof(MockBot_AccellerateCallsAccellerateMessage))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void MockBot_AccellerateCallsAccellerateMessage() {
            var testHub = new Hub(true);
            testHub.UseStrongReferences = true;
            var engineHub = new bd2MessageBasedBotSupport();
            engineHub.InjectHub(testHub);
            bool messageRecieved = false;
            int valueChange = 0;

            testHub.LookFor<Message_Query>(ab => {
                if (ab.SubKind == KnownSubkinds.ReadSpeed) {
                    NavigationInfoContext nic = new NavigationInfoContext();
                    nic.SpeedDelta = 1;
                    ab.ResponseContext = nic;
                }
            });

            testHub.LookFor<Message_BotPerformAction>(ab => {
                if (ab.SubKind == KnownSubkinds.ChangeSpeed) {
                    messageRecieved = true;
                    NavigationInfoContext nic = (NavigationInfoContext)ab.RequestContext;
                    Assert.NotNull(nic);
                    valueChange = nic.SpeedDelta;
                }
            });

            var sut = new MockBotFactory().CreateBasicMockBot().WithThisActionProvider(engineHub, engineHub).ToBot();
            sut.Accelerate();
            Assert.True(messageRecieved, "The accellerate call did not send the change speed request message");
            Assert.True(valueChange == 1, "The accelerate value should be 1");
        }

        [Fact(DisplayName = nameof(MockBot_AccellerateCallsReadSpeedMessage))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void MockBot_AccellerateCallsReadSpeedMessage() {
            Hub testHub = new Hub(true);
            var actionProvider = new bd2MessageBasedBotSupport();
            actionProvider.InjectHub(testHub);
            bool messageRecieved = false;

            testHub.LookFor<Message_Query>(ab => {
                if (ab.SubKind == KnownSubkinds.ReadSpeed) {
                    messageRecieved = true;
                    NavigationInfoContext nic = new NavigationInfoContext();
                    nic.SpeedDelta = 1;
                    ab.ResponseContext = nic;
                }
            });
            var sut = new MockBotFactory().CreateBasicMockBot().WithThisActionProvider(actionProvider, actionProvider).ToBot();
            sut.Accelerate();
            Assert.True(messageRecieved, "The query speed message was not requested by the accelerate function");
        }

        [Fact(DisplayName = nameof(MockBot_AccellerateCallsEngineAccelerate))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void MockBot_AccellerateCallsEngineAccelerate() {
            var engineHub = new MockBotActionProvider();
            var fac = new MockBotFactory().CreateBasicMockBot().WithMockActionProvider();
            var sut = fac.ToBot();
            sut.Accelerate();
            Assert.Equal(1, fac.MockActionProviderUsed.LastSpeedChangeValue);
        }

        [Fact(DisplayName = nameof(MockBot_DecellerateSendsDecelerateMessage))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void MockBot_DecellerateSendsDecelerateMessage() {
            var fac = new MockBotFactory().CreateBasicMockBot().WithMockActionProvider();
            var sut = fac.ToBot();
            sut.Decelerate();
            Assert.True(fac.MockActionProviderUsed.LastSpeedChangeValue == -1, "The engine should recieve a -1 speed change when accelerate is called");
        }

        [Fact(DisplayName = nameof(MockBot_ChangeHeadingSendsTurnMessage))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void MockBot_ChangeHeadingSendsTurnMessage() {
            var mbf = new MockBotFactory().CreateBasicMockBot().WithMockActionProvider();
            var sut = mbf.ToBot();
            sut.ChangeHeading(15);
            Assert.True(mbf.MockActionProviderUsed.LastHeadingChangeValue == 15, "The engine should recieve a 15 degree change value");
        }

        [Fact(DisplayName = nameof(MockBot_SetDirectionToPoint_CalculatesSimpleCorrectly))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void MockBot_SetDirectionToPoint_CalculatesSimpleCorrectly() {
            var mbf = new MockBotFactory().CreateBasicMockBot().WithMockActionProvider();
            var sut = mbf.ToBot();
            sut.HeadToPoint(new Point(2, 2));
            Assert.Equal(45, mbf.MockActionProviderUsed.LastHeadingChangeValue);
        }

    }
}
