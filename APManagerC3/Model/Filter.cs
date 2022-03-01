using HMUtility.Algorithm;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace APManagerC3.Model {
    [Serializable]
    public class Filter : IEncryptable<Filter>, IDeepCopyable<Filter> {
        [JsonProperty("Category", Order = 0)]
        public string Category { get; private set; } = "";
        [JsonProperty("Identifier", Order = 1)]
        public string Identifier { get; private set; } = "";
        [JsonProperty("Containers", Order = 2)]
        public List<Container> Containers { get; private set; } = new List<Container>();

        public Filter Decrypt(ITextEncryptor encryptor) {
            // 解密名称
            try {
                Category = encryptor.Decrypt(Category);
            } catch {
                Category = "ERROR_ON_DECRYPT_NAME";
            }
            // 解密标识
            try {
                Identifier = encryptor.Decrypt(Identifier);
            } catch {
                Identifier = "ERROR_ON_DECRYPT_IDENTIFIER";
            }
            // 解密容器
            foreach (var container in Containers) {
                container.Decrypt(encryptor);
            }

            return this;
        }
        public Filter Encrypt(ITextEncryptor encryptor) {
            Category = encryptor.Encrypt(Category);
            Identifier = encryptor.Encrypt(Identifier);
            foreach (var container in Containers) {
                container.Encrypt(encryptor);
            }

            return this;
        }

        public Filter GetDeepCopy() {
            var result = new Filter();

            result.Category = Category;
            result.Identifier = Identifier;
            foreach (var container in Containers) {
                result.Containers.Add(container.GetDeepCopy());
            }

            return result;
        }
        public void DeepCopyFrom(Filter source) {
            Category = source.Category;
            Identifier = source.Identifier;
            Containers.Clear();
            foreach (var container in source.Containers) {
                Containers.Add(container.GetDeepCopy());
            }
        }

        public Filter() {

        }
        public Filter(string category, string identifier) {
            Category = category;
            Identifier = identifier;
        }
    }
}
