namespace APManagerC3 {
    public class NoEncrypter : IEncrypter {
        public string Decrypt(string encrypted) {
            return encrypted;
        }

        public string Encrypt(string source) {
            return source;
        }
    }
}
