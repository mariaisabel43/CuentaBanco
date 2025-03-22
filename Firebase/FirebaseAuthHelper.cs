using Firebase.Auth.Providers;
using Firebase.Auth;

namespace Cuenta_Bancaria.Firebase
{
    public class FirebaseAuthHelper
    {
        public const string firebaseAppId = "exam-1-57954";
        public const string firebaseApiKey = "AIzaSyCbesUkKqwG6wa1iEG9e5wvFCVbjrlY8JU";

        public static FirebaseAuthClient setFirebaseAuthClient()
        {
            var auth = new FirebaseAuthClient(new FirebaseAuthConfig()
            {
                ApiKey = firebaseApiKey,
                AuthDomain = $"{firebaseAppId}.firebaseapp.com",
                Providers = new FirebaseAuthProvider[]
                {
                    new EmailProvider()
                }
            });

            return auth;
        }
    }
}
