using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace APManagerC3.Model {
    [DataContract]
    public class Filter {
        [DataMember(Order = 0)]
        public string Category;
        [DataMember(Order = 1)]
        public string Identifier;
        [DataMember(Order = 2)]
        public List<Container> Containers;

        public Filter() {
            Containers = new List<Container>();
        }

        public void EncryptData(IEncrypter encrypter) {
            Category = encrypter.Encrypt(Category);
            Identifier = encrypter.Encrypt(Identifier);
            foreach (var container in Containers) {
                container.EncryptData(encrypter);
            }
        }
        public void DecryptData(IEncrypter encrypter) {
            // 解密名称
            try {
                Category = encrypter.Decrypt(Category);
            }
            catch (Exception) {
                Category = "ERROR_ON_DECRYPT_NAME";
            }
            // 解密标识
            try {

                Identifier = encrypter.Decrypt(Identifier);
            }
            catch (Exception) {
                Identifier = "ERROR_ON_DECRYPT_IDENTIFIER";
            }
            // 解密容器
            foreach (var container in Containers) {
                container.DecryptData(encrypter);
            }
        }
    }
}
