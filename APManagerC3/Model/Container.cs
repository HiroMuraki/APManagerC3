using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace APManagerC3.Model {
    [DataContract]
    public class Container {
        [DataMember(Order = 0)]
        public string Title;
        [DataMember(Order = 1)]
        public string Description;
        [DataMember(Order = 2)]
        public List<Record> Records;

        public Container() {
            Records = new List<Record>();
        }

        public void DecryptData(IEncrypter encrypter) {
            // 解密标题
            try {
                Title = encrypter.Decrypt(Title);
            }
            catch (Exception) {
                Title = "ERROR_ON_DECRYPT_TITLE";
            }
            // 解密描述
            try {
                Description = encrypter.Decrypt(Description);
            }
            catch (Exception) {
                Description = "ERROR_ON_DECRYPT_DESCRIPTION";
            }
            foreach (var record in Records) {
                record.DecryptData(encrypter);
            }
        }
        public void EncryptData(IEncrypter encrypter) {
            Title = encrypter.Encrypt(Title);
            Description = encrypter.Encrypt(Description);
            foreach (var record in Records) {
                record.EncryptData(encrypter);
            }
        }
    }
}
