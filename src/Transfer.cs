using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace BankingConsole
{
    public class Transfer
    {
        int senderId;
        int recipientId;
        string amountTransferred;
        public Transfer(int senderId, int recipientId, string amountTransferred)
        {
            this.senderId = senderId;
            this.recipientId = recipientId;
            this.amountTransferred = amountTransferred;
        }
        public void TransferMoney(string ConnectionString)
        {
            string senderUsername, recipientUsername;
            using var cnt = new NpgsqlConnection(ConnectionString);
            cnt.Open();
            var sql = $"UPDATE client SET balance = balance - {amountTransferred.Replace(",", ".")} WHERE client_id={senderId}; " +
                      $"UPDATE client SET balance = balance + {amountTransferred.Replace(",", ".")} WHERE client_id={recipientId};";
            var cmd = new NpgsqlCommand(sql, cnt);
            cmd.ExecuteNonQuery();
            sql = $"SELECT username FROM client WHERE client_id ={senderId};";
            cmd.CommandText = sql;
            senderUsername = cmd.ExecuteScalar().ToString();
            sql = $"SELECT username FROM client WHERE client_id ={recipientId};";
            cmd.CommandText = sql;
            recipientUsername = cmd.ExecuteScalar().ToString();
            sql = $"INSERT INTO \"transfer\" (sender_id, recipient_id, amount, sender_username, recipient_username) VALUES (\'{senderId}\', \'{recipientId}\', \'{amountTransferred.Replace(",", ".")}\', \'{senderUsername}\', \'{recipientUsername}\');";
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
            cnt.Close();
        }
    }
}
