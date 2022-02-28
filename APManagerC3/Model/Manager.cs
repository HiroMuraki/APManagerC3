using HMUtility.Algorithm;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace APManagerC3.Model {
    [Serializable]
    public class Manager : IEncryptable<Manager>, IDeepCopyable<Manager> {
        [JsonProperty("APMData", Order = 0)]
        public List<Filter> APMData { get; init; } = new List<Filter>();

        public Manager GetDecrypt(ITextEncryptor encryptor) {
            var result = new Manager();

            foreach (var filter in APMData) {
                result.APMData.Add(filter.GetDecrypt(encryptor));
            }

            return result;
        }
        public Manager GetEncrypt(ITextEncryptor encryptor) {
            var result = new Manager();

            foreach (var filter in APMData) {
                result.APMData.Add(filter.GetEncrypt(encryptor));
            }

            return result;
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
