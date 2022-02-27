using System;
using System.Runtime.Serialization;

namespace APManagerC3 {
    [Serializable]
    public class FilterExistedException : Exception {
        public FilterExistedException() {

        }
        public FilterExistedException(string message) : base(message) {

        }
        public FilterExistedException(string message, Exception inner) : base(message, inner) {

        }
        protected FilterExistedException(SerializationInfo info, StreamingContext context) : base(info, context) {

        }
    }
}
