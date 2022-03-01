using HMUtility.Algorithm;
using Newtonsoft.Json;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace APManagerC3.Model {
    [Serializable]
    public record Container : IEncryptable<Container>, IDeepCopyable<Container> {
        [JsonProperty("Title", Order = 0)]
        public string Title { get; init; } = "";
        [JsonProperty("Description", Order = 1)]
        public string Description { get; init; } = "";
        [JsonProperty("Records", Order = 2)]
        public ImmutableList<Record> Records { get; init; } = ImmutableList.Create<Record>();

        public Container Decrypt(ITextEncryptor encryptor) {
            // 解密标题
            string title;
            try {
                title = encryptor.Decrypt(Title);
            } catch (Exception) {
                title = "ERROR_ON_DECRYPT_TITLE";
            }
            // 解密描述
            string description;
            try {
                description = encryptor.Decrypt(Description);
            } catch (Exception) {
                description = "ERROR_ON_DECRYPT_DESCRIPTION";
            }

            return new Container() {
                Title = title,
                Description = description,
                Records = ImmutableList.CreateRange<Record>(from record in Records select record.Decrypt(encryptor))
            };
        }
        public Container Encrypt(ITextEncryptor encryptor) {
            return new Container() {
                Title = encryptor.Encrypt(Title),
                Description = encryptor.Encrypt(Description),
                Records = ImmutableList.CreateRange<Record>(from record in Records select record.Encrypt(encryptor))
            };
        }
        public Container GetDeepCopy() {
            return new Container() {
                Title = Title,
                Description = Description,
                Records = ImmutableList.CreateRange<Record>(from record in Records select record.GetDeepCopy())
            };
        }
    }
}
