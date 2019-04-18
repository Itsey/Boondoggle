namespace Plisky.Boondoggle2 {

    using System.Collections.Generic;

    public class LastTickResults {
        public Stack<bd2TickAction> Events = new Stack<bd2TickAction>();

        public bool Moved { get; set; }

        public bool Collided { get; set; }

        public bool ShotAt { get; set; }

        public bool Hit { get; set; }

        public int PowerRemaining { get; set; }
    }
}