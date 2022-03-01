using HMUtility.Algorithm;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace APManagerC3.Model {
    [Serializable]
    public class Container : IEncryptable<Container>, IDeepCopyable<Container> {
        [JsonProperty("Title", Order = 0)]
        public string Title { get; private set; } = "";
        [JsonProperty("Description", Order = 1)]
        public string Description { get; private set; } = "";
        [JsonProperty("Records", Order = 2)]
        public List<Record> Records { get; private set; } = new List<Record>();

        public Container Decrypt(ITextEncryptor encryptor) {
            // 解密标题
            try {
                Title = encryptor.Decrypt(Title);
            } catch (Exception) {
                Title = "ERROR_ON_DECRYPT_TITLE";
            }
            // 解密描述
            try {
                Description = encryptor.Decrypt(Description);
            } catch (Exception) {
                Description = "ERROR_ON_DECRYPT_DESCRIPTION";
            }

            foreach (var record in Records) {
                record.Decrypt(encryptor);
            }

            return this;
        }
        public Container Encrypt(ITextEncryptor encryptor) {
            Title = encryptor.Encrypt(Title);
            Description = encryptor.Encrypt(Description);
            foreach (var record in Records) {
                record.Decrypt(encryptor);
            }
            return this;
        }
        public Container GetDeepCopy() {
            var result = new Container();

            result.Title = Title;
            result.Description = Description;
            foreach (var record in Records) {
                result.Records.Add(record.GetDeepCopy());
            }

            return result;
        }
        public void DeepCopyFrom(Container source) {
            Title = source.Title;
            Description = source.Description;
            Records.Clear();
            foreach (var record in source.Records) {
                Records.Add(record.GetDeepCopy());
            }
        }

        public Container() {

        }
        public Container(string title, string description) {
            Title = title;
            Description = description;
        }
    }
}
