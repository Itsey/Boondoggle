using Plisky.Boondoggle2;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plisky.Boondoggle2.Test {
    public class MockMappedBot :MappedBot {
        public MockMappedBot(BoonBotBase desiredBot):base(desiredBot) {            
        }

        public void Test_Initialise() {
            // This is done by the world - but for Mocks we do it direct.
            this.LifeRemaining = 100;
            this.Position = new System.Drawing.Point(10, 10);
            this.Speed = 0;
            this.Heading = 0;
            this.IsActive = true;
            this.ChargeRemaining = 0;
            this.PowerRemaining = 0;
        }
    }
}
