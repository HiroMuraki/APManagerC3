using HMUtility.Algorithm;

namespace APManagerC3 {
    public interface IViewModel<TModel, TViewModel> {
        void LoadFromModel(TModel model);
        TModel ConvertToModel();
    }

    public interface IEncryptable<T> {
        T GetDecrypt(ITextEncryptor encryptor);
        T GetEncrypt(ITextEncryptor encryptor);
    }

    public class NoEncrypter : ITextEncryptor {
        public string Decrypt(string encrypted) {
            return encrypted;
        }

        public string Encrypt(string source) {
            return source;
        }
    }
}
