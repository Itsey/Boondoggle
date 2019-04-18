using System;
using System.Runtime.Serialization;

namespace Plisky.Boondoggle2 {

    [Serializable]
    public class BdBaseException : Exception {

        public BdBaseException()
            : base() {
        }

        protected BdBaseException(SerializationInfo serialisationInformation, StreamingContext streamContext)
            : base(serialisationInformation, streamContext) {
        }

        public BdBaseException(string message)
            : base(message) {
        }

        public BdBaseException(string message, Exception inner)
            : base(message, inner) {
        }
    }
}