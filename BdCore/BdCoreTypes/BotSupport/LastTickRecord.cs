namespace Plisky.Boondoggle2 {
    using System.Collections.Generic;

    /// <summary>
    /// A description of what occured during the last tick.  Will inform you if there were activities such as being shot at or moved.
    /// </summary>
    public class LastTickRecord {

        public Stack<bd2TickAction> Events = new Stack<bd2TickAction>();

    }
}