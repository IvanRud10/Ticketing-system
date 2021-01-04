using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;

namespace AirTicket
{
    class DataAccess
    {
        public List<Client> GetTicketID(string ticket_id)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("AirTicketDB")))
            {
                return connection.Query<Client>($"select * from TicketInfo where Ticket_ID ='{ticket_id}'").ToList();
            }
        }

        public List<Client> Logging(string Login, string Password)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("AirTicketDB")))
            {
                return connection.Query<Client>($"select * from Clients where Login ='{Login}' and Password ='{Password}'").ToList();
            }
        }

        public void InsertClient(string login, string password, string firstname, string lastname, string email)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("AirTicketDB")))
            {
                Client newClient = new Client { Login = login, Password = password, FirstName = firstname, LastName = lastname, Email = email };
                List<Client> client = new List<Client>();
                client.Add(new Client { Login = login, Password = password, FirstName = firstname, LastName = lastname, Email = email });
                connection.Execute("dbo.ClientInsert @Login, @Password, @FirstName, @LastName, @Email", client);

                SqlConnection con = new SqlConnection();
                SqlCommand cmdAddNewUser;
                cmdAddNewUser = new SqlCommand($"select count(Email) from Clients where Email ='{email}'", con);
                int num = Convert.ToInt32(cmdAddNewUser.ExecuteScalar());
                if (num == 1)
                {
                   
                }
            }
        }
   
    }
}
