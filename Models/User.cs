using Google.Cloud.Firestore;
using Cuenta_Bancaria.Firebase;

namespace Cuenta_Bancaria.Models
{
    public class UserModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public int Capital  { get; set; }
    }

    public class UserHelper
    {
	//Obtención de usuarios y creación de los mismos.
        public async Task<UserModel> getUserInfo(string email)
        {
            Query query = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("BankUser").WhereEqualTo("email", email);
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

            Dictionary<string, object> data = querySnapshot.Documents[0].ToDictionary();

            string UserID = querySnapshot.Documents[0].Id;

            UserModel bankuser = new UserModel
            {
                Id = UserID,
                Email = data["email"].ToString(),
                Name = data["name"].ToString(),
                Capital = Convert.ToInt32(data["capital"]),
            };

            return bankuser;
        }

        public async Task<List<UserModel>> getAllUsers()
        {
            List<UserModel> usersList = new List<UserModel>();
            Query query = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("BankUser");
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

            foreach (var item in querySnapshot)
            {
                Dictionary<string, object> data = item.ToDictionary();
                
                string UserID = item.Id.ToString();

                usersList.Add(new UserModel
                {
                    Id = UserID,
                    Email = data["email"].ToString(),
                    Name = data["name"].ToString(),
                    Capital = Convert.ToInt32(data["capital"]),
                });
            }
            return usersList;
        }

        public async Task<bool> MakeTransaction (UserModel bankuser)
        {
            try
            {
                DocumentReference docRef = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("BankUser").Document(bankuser.Id);
                Dictionary<string, object> dataToUpdate = new Dictionary<string, object>
                {
                    {"email",bankuser.Email },
                    {"name",bankuser.Name},
                    {"capital",bankuser.Capital}
                };
                WriteResult result = await docRef.UpdateAsync(dataToUpdate);

                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> UpdateUserInfo (UserModel bankuser)
        {
            try
            {
                DocumentReference docRef = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("BankUser").Document(bankuser.Id);
                Dictionary<string, object> dataToUpdate = new Dictionary<string, object>
                {
                    {"email",bankuser.Email },
                    {"name",bankuser.Name},
                    {"capital",bankuser.Capital}
                };
                WriteResult result = await docRef.UpdateAsync(dataToUpdate);
                return true;
            }
            catch
            {
                return false;
            }
        }
		public async Task<bool> CreateUser(UserModel bankuser)
		{
			try
			{
				// 1. Obtener una referencia a la colección "BankUser"
				CollectionReference collectionRef = FirestoreDb
					.Create(FirebaseAuthHelper.firebaseAppId)
					.Collection("BankUser");

				// 2. Crear un diccionario con los datos del usuario
				Dictionary<string, object> userData = new Dictionary<string, object>
		{
			{ "email", bankuser.Email },
			{ "name", bankuser.Name },
			{ "capital", bankuser.Capital }
		};

				// 3. Agregar un nuevo documento a la colección "BankUser"
				DocumentReference documentRef = await collectionRef.AddAsync(userData);

				// 4. Asignar el ID generado por Firestore al objeto bankuser
				bankuser.Id = documentRef.Id;

				// 5. Retornar true para indicar que el usuario fue creado exitosamente
				return true;
			}
			catch (Exception ex)
			{
				// Manejar errores (por ejemplo, problemas de conexión o datos inválidos)
				Console.WriteLine($"Error al crear el usuario: {ex.Message}");
				return false;
			}
		}



	}
}
