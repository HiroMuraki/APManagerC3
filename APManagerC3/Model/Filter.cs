using HMUtility.Algorithm;
using Newtonsoft.Json;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Serialization;

namespace APManagerC3.Model {
    [Serializable]

    public class Filter : IEncryptable<Filter>, IDeepCopyable<Filter> {
        [JsonProperty("Category", Order = 0)]
        [DataMember(Order = 0)]
        public string Category = "";
        [JsonProperty("Identifier", Order = 1)]
        [DataMember(Order = 1)]
        public string Identifier = "";
        [JsonProperty("Containers", Order = 2)]
        [DataMember(Order = 2)]
        public ImmutableList<Container> Containers = ImmutableList<Container>.Empty;

        public Filter Decrypt(ITextEncryptor encryptor) {
            // 解密名称
            string category;
            try {
                category = encryptor.Decrypt(Category);
            } catch {
                category = "ERROR_ON_DECRYPT_NAME";
            }
            // 解密标识
            string identifier;
            try {
                identifier = encryptor.Decrypt(Identifier);
            } catch {
                identifier = "ERROR_ON_DECRYPT_IDENTIFIER";
            }

            return new Filter() {
                Category = category,
                Identifier = identifier,
                Containers = ImmutableList.CreateRange<Container>(from container in Containers select container.Decrypt(encryptor))
            };
        }
        public Filter Encrypt(ITextEncryptor encryptor) {
            return new Filter() {
                Category = encryptor.Encrypt(Category),
                Identifier = encryptor.Encrypt(Identifier),
                Containers = ImmutableList.CreateRange<Container>(from container in Containers select container.Encrypt(encryptor))
            };
        }

        public Filter GetDeepCopy() {
            return new Filter() {
                Category = Category,
                Identifier = Identifier,
                Containers = ImmutableList.CreateRange<Container>(from container in Containers select container.GetDeepCopy())
            };
        }
    }
}
