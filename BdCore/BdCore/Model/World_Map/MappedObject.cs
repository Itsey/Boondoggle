namespace Plisky.Boondoggle2 {

    using System.Diagnostics;
    using System.Drawing;
    using System.Threading;

    [DebuggerDisplay("Id:{EngineId}  @:{Position} Spd: {Speed} Head: {Heading}")]
    /// <summary>
    /// Represents an object that is currently residing on the map, has a speed and direction associated with it.
    /// </summary>
    public class MappedObject {
        private static int lastUsedUniqueId = 0;

        public int EngineId { get; private set; }

        public MappedObject() {
            EngineId = Interlocked.Increment(ref lastUsedUniqueId);
        }

        public Point Position { get; set; }

        public double Heading { get; set; }

        public int Speed { get; set; }
    }
}