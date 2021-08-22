namespace APManagerC3 {
    public interface IEncrypter {
        string Encrypt(string source);
        string Decrypt(string encrypted);
    }
}
