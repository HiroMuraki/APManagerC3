using HMUtility.Algorithm;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace APManagerC3.Model {
    [Serializable]
    public class Filter : IEncryptable<Filter> ,IDeepCopyable<Filter>{
        [JsonProperty("Category", Order = 0)]
        public string Category { get; set; } = "";
        [JsonProperty("Identifier", Order = 1)]
        public string Identifier { get; set; } = "";
        [JsonProperty("Containers", Order = 2)]
        public List<Container> Containers { get; init; } = new List<Container>();

        public Filter GetDecrypt(ITextEncryptor encryptor) {
            var result = new Filter();

            // 解密名称
            try {
                result.Category = encryptor.Decrypt(Category);
            } catch {
                result.Category = "ERROR_ON_DECRYPT_NAME";
            }
            // 解密标识
            try {
                result.Identifier = encryptor.Decrypt(Identifier);
            } catch {
                result.Identifier = "ERROR_ON_DECRYPT_IDENTIFIER";
            }
            // 解密容器
            foreach (var container in Containers) {
                result.Containers.Add(container.GetDecrypt(encryptor));
            }

            return result;
        }
        public Filter GetEncrypt(ITextEncryptor encryptor) {
            var result = new Filter();

            result.Category = encryptor.Encrypt(Category);
            result.Identifier = encryptor.Encrypt(Identifier);
            foreach (var container in Containers) {
                result.Containers.Add(container.GetEncrypt(encryptor));
            }

            return result;
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
    }
}
