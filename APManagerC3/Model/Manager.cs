using HMUtility.Algorithm;
using Newtonsoft.Json;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace APManagerC3.Model {
    [Serializable]
    public class Manager : IEncryptable<Manager>, IDeepCopyable<Manager> {
        [JsonProperty("APMData", Order = 0)]
        public ImmutableList<Filter> APMData { get; init; } = ImmutableList<Filter>.Empty;

        public Manager Decrypt(ITextEncryptor encryptor) {
            return new Manager() {
                APMData = ImmutableList.CreateRange<Filter>(from filter in APMData select filter.Decrypt(encryptor)),
            };
        }
        public Manager Encrypt(ITextEncryptor encryptor) {
            return new Manager() {
                APMData = ImmutableList.CreateRange<Filter>(from filter in APMData select filter.Encrypt(encryptor)),
            };
        }
        public Manager GetDeepCopy() {
            return new Manager() {
                APMData = ImmutableList.CreateRange<Filter>(from filter in APMData select filter.GetDeepCopy()),
            };
        }
    }
}
