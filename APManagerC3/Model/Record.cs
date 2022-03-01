using HMUtility.Algorithm;
using Newtonsoft.Json;
using System;

namespace APManagerC3.Model {
    [Serializable]
    public record Record : IEncryptable<Record>, IDeepCopyable<Record> {
        [JsonProperty("Label", Order = 0)]
        public string Label { get; init; } = "";
        [JsonProperty("Information", Order = 1)]
        public string Information { get; init; } = "";

        public Record Decrypt(ITextEncryptor encryptor) {
            // 解密标签
            string label;
            try {
                label = encryptor.Decrypt(Label);
            } catch (Exception) {
                label = "ERROR_ON_DECRYPT_LABEL";
            }
            // 解密信息
            string information;
            try {
                information = encryptor.Decrypt(Information);
            } catch (Exception) {
                information = "ERROR_ON_DECRYPT_INFORMATION";
            }

            return new Record() {
                Label = label,
                Information = information
            };
        }
        public Record Encrypt(ITextEncryptor encryptor) {
            return new Record() {
                Label = encryptor.Encrypt(Label),
                Information = encryptor.Encrypt(Information)
            };
        }
        public Record GetDeepCopy() {
            return new Record() {
                Label = Label,
                Information = Information
            };
        }
    }
}
