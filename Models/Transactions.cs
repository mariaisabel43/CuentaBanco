using Google.Cloud.Firestore;
using Cuenta_Bancaria.Firebase;

namespace Cuenta_Bancaria.Models
{
    public class TransactionsModel
    {
        public string FromUser { get; set; }
        public string ToUser { get; set; }
        public int TotalAmount { get; set; }
        public string Date { get; set; }
    }

    public class TransactionsHelper
    {
        //Validación de obtención de historial solo del usuario correspondiente.
        public static async Task<List<TransactionsModel>> GetTransactions(string emails)
        {
            List<TransactionsModel> transactionsList = new List<TransactionsModel>();
            Query query = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId).Collection("BankTransaction")
                .Where(Filter.Or(
                    Filter.EqualTo("fromUser", emails),
                    Filter.EqualTo("toUser", emails)
                ));
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

            foreach (var item in querySnapshot)
            {
                Dictionary<string, object> data = item.ToDictionary();
                transactionsList.Add(new TransactionsModel
                {
                    FromUser = data["fromUser"].ToString(),
                    ToUser = data["toUser"].ToString(),
                    TotalAmount = Convert.ToInt32(data["totalamount"]),
                    Date = data["date"].ToString(),
                });
            }
            return transactionsList;
        }
        public async Task<bool> RegisterTransaction (TransactionsModel banktransaction)
        {
            try
            {
                FirestoreDb fdb = FirestoreDb.Create(FirebaseAuthHelper.firebaseAppId);
                CollectionReference coll = fdb.Collection("BankTransaction");
                Dictionary<string, object> newTrasaction = new Dictionary<string, object>
                {
                    {"fromUser",banktransaction.FromUser },
                    {"toUser",banktransaction.ToUser },
                    {"totalamount",banktransaction.TotalAmount },
                    {"date",banktransaction.Date },
                };
                await coll.AddAsync(newTrasaction);
                return true;
            }
            catch
            {
                return false;
            }
            
        }

    }
}
