using HMUtility.Algorithm;
using Newtonsoft.Json;
using System;

namespace APManagerC3.Model {
    [Serializable]
    public class Record : IEncryptable<Record>, IDeepCopyable<Record> {
        [JsonProperty("Label", Order = 0)]
        public string Label { get; private set; } = "";
        [JsonProperty("Information", Order = 1)]
        public string Information { get; private set; } = "";

        public Record Decrypt(ITextEncryptor encryptor) {
            // 解密标签
            try {
                Label = encryptor.Decrypt(Label);
            } catch (Exception) {
                Label = "ERROR_ON_DECRYPT_LABEL";
            }
            // 解密信息
            try {
                Information = encryptor.Decrypt(Information);
            } catch (Exception) {
                Information = "ERROR_ON_DECRYPT_INFORMATION";
            }

            return this;
        }
        public Record Encrypt(ITextEncryptor encryptor) {
            Label = encryptor.Encrypt(Label);
            Information = encryptor.Encrypt(Information);
            return this;
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

        public Record() {

        }
        public Record(string label, string information) {
            Label = label;
            Information = information;
        }
    }
}
