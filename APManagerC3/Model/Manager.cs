using System.Collections.Generic;
using System.Runtime.Serialization;

namespace APManagerC3.Model {
    [DataContract]
    public class Manager {
        [DataMember(Order = 0)]
        public List<Filter> APMData;

        public Manager() {
            APMData = new List<Filter>();
        }

        public void EncryptData(IEncrypter encrypter) {
            foreach (var filter in APMData) {
                filter.EncryptData(encrypter);
            }
        }
        public void DecryptData(IEncrypter encrypter) {
            foreach (var filter in APMData) {
                filter.DecryptData(encrypter);
            }
        }
    }
}
