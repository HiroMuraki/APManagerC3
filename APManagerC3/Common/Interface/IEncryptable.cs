using HMUtility.Algorithm;

namespace APManagerC3 {
    public interface IEncryptable<T> {
        T Decrypt(ITextEncryptor encryptor);
        T Encrypt(ITextEncryptor encryptor);
    }
}
