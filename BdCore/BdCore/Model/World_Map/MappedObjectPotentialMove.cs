namespace Plisky.Boondoggle2 {

    using System.Diagnostics;
    using System.Drawing;

    [DebuggerDisplay("@:{DesiredPosition} From: {Underlying.Position} for ID: {Underlying.EngineId}")]
    public class MappedObjectPotentialMove {
        public MappedObject Underlying;

        public MappedObjectPotentialMove(MappedObject targetMappedObject) {
            this.Underlying = targetMappedObject;
        }

        public Point DesiredPosition { get; set; }

        public bool HasBoundaryCollision { get; set; }

        public bool HasCollided { get; set; }
    }
}