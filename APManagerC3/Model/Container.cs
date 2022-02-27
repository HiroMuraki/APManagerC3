using HMUtility.Algorithm;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace APManagerC3.Model {
    [Serializable]
    public class Container : IEncryptable<Container> {
        [JsonProperty("Title", Order = 0)]
        public string Title { get; set; } = "";
        [JsonProperty("Description", Order = 1)]
        public string Description { get; set; } = "";
        [JsonProperty("Records", Order = 2)]
        public List<Record> Records { get; init; } = new List<Record>();

        public Container GetDecrypt(ITextEncryptor encryptor) {
            var result = new Container();

            // 解密标题
            try {
                result.Title = encryptor.Decrypt(Title);
            } catch (Exception) {
                result.Title = "ERROR_ON_DECRYPT_TITLE";
            }
            // 解密描述
            try {
                result.Description = encryptor.Decrypt(Description);
            } catch (Exception) {
                result.Description = "ERROR_ON_DECRYPT_DESCRIPTION";
            }

            foreach (var record in Records) {
                result.Records.Add(record.GetDecrypt(encryptor));
            }

            return result;
        }
        public Container GetEncrypt(ITextEncryptor encryptor) {
            var result = new Container();

            result.Title = encryptor.Encrypt(Title);
            result.Description = encryptor.Encrypt(Description);
            foreach (var record in Records) {
                result.Records.Add(record.GetEncrypt(encryptor));
            }

            return result;
        }
    }
}
