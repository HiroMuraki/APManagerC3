using HMUtility.Algorithm;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace APManagerC3.Model {
    [Serializable]
    public class Manager : IEncryptable<Manager>, IDeepCopyable<Manager> {
        [JsonProperty("APMData", Order = 0)]
        public List<Filter> APMData { get; private set; } = new List<Filter>();

        public Manager Decrypt(ITextEncryptor encryptor) {
            foreach (var filter in APMData) {
                filter.Decrypt(encryptor);
            }

            return this;
        }
        public Manager Encrypt(ITextEncryptor encryptor) {
            foreach (var filter in APMData) {
                filter.Encrypt(encryptor);
            }

            return this;
        }

        public void DeepCopyFrom(Manager source) {
            APMData.Clear();
            foreach (var filter in source.APMData) {
                APMData.Add(filter.GetDeepCopy());
            }
        }
        public Manager GetDeepCopy() {
            var result = new Manager();

            foreach (var filter in APMData) {
                result.APMData.Add(filter.GetDeepCopy());
            }

            return result;
        }
    }
}
