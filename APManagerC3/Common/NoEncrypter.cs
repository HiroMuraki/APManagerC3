using HMUtility.Algorithm;

namespace APManagerC3 {
    public class NoEncrypter : ITextEncryptor {
        public string Decrypt(string encrypted) {
            return encrypted;
        }

        public string Encrypt(string source) {
            return source;
        }
    }
}
