using HMUtility.Algorithm;
using Newtonsoft.Json;
using System;

namespace APManagerC3.Model {
    [Serializable]
    public class Record : IEncryptable<Record>, IDeepCopyable<Record> {
        [JsonProperty("Label", Order = 0)]
        public string Label { get; set; } = "";
        [JsonProperty("Information", Order = 1)]
        public string Information { get; set; } = "";

        public Record GetDecrypt(ITextEncryptor encryptor) {
            var result = new Record();

            // 解密标签
            try {
                result.Label = encryptor.Decrypt(Label);
            } catch (Exception) {
                result.Label = "ERROR_ON_DECRYPT_LABEL";
            }
            // 解密信息
            try {
                result.Information = encryptor.Decrypt(Information);
            } catch (Exception) {
                result.Information = "ERROR_ON_DECRYPT_INFORMATION";
            }

            return result;
        }
        public Record GetEncrypt(ITextEncryptor encryptor) {
            var result = new Record();
            result.Label = encryptor.Encrypt(Label);
            result.Information = encryptor.Encrypt(Information);
            return result;
        }

        public Record GetDeepCopy() {
            return new Record() {
                Label = Label,
                Information = Information
            };
        }
        public void DeepCopyFrom(Record source) {
            Label = source.Label;
            Information = source.Information;
        }
    }
}
