using HMUtility.Algorithm;
using System.IO;
using System.Text.RegularExpressions;

namespace APManagerC3 {
    public static class APManager {
        public static readonly string ProfileFile = "apmdata.json";

        public static bool ProfileExisted {
            get {
                return File.Exists(ProfileFile);
            }
        }
        public static ITextEncryptor Encrypter { get; set; } = new NoEncrypter();
        public static bool SaveRequired { get; set; } = false;

        public static bool IsValidPassword(string password) {
            return Regex.IsMatch(password, @"^[\u0021-\u007E]*$");
        }
    }
}
