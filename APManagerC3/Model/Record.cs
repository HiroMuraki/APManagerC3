using System;
using System.Runtime.Serialization;

namespace APManagerC3.Model {
    [DataContract]
    public class Record {
        [DataMember(Order = 0)]
        public string Label;
        [DataMember(Order = 1)]
        public string Information;

        public void DecryptData(IEncrypter encrypter) {
            // 解密标签
            try {
                Label = encrypter.Decrypt(Label);
            }
            catch (Exception) {
                Label = "ERROR_ON_DECRYPT_LABEL";
            }
            // 解密信息
            try {
                Information = encrypter.Decrypt(Information);
            }
            catch (Exception) {
                Information = "ERROR_ON_DECRYPT_INFORMATION";
            }
        }
        public void EncryptData(IEncrypter encrypter) {
            Label = encrypter.Encrypt(Label);
            Information = encrypter.Encrypt(Information);
        }
    }
}
