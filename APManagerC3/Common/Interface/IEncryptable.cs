using HMUtility.Algorithm;

namespace APManagerC3 {
    public interface IEncryptable<T> {
        T GetDecrypt(ITextEncryptor encryptor);
        T GetEncrypt(ITextEncryptor encryptor);
    }
}
