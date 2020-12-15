using System;
using System.Text;
using System.Net;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace AirTicket
{
    class Server
    {
        static int port = 8005; // порт для приема входящих запросов
        static string server_ip = "127.0.0.1";
        public string login;
        static void Main(string[] args)
        {
            string connectionString = @"Data Source=(local);Initial Catalog=AirTickets;" + "Integrated Security=SSPI;";
            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand cmdCheckUserEmail;
            SqlCommand cmdaddNewUser;
            SqlCommand cmdloguser;
            SqlCommand cmdsearchtick;
            SqlCommand cmdaddnewticket;
            SqlCommand cmdsearchtickid;
            SqlCommand cmdsearchprice;
            SqlCommand cmdinfofrom;
            SqlCommand cmdinfoto;
            SqlCommand cmdinfoclass;
            SqlCommand cmdinfoprice;
            SqlCommand cmdpayment;
            SqlCommand cmdcheckforpay;
            SqlCommand cmdcheckforid;

        IPAddress localAdd = IPAddress.Parse(server_ip);
            TcpListener listener = new TcpListener(localAdd, port);
            Console.WriteLine("Listening...");
            listener.Start();
            con.Open();
            while (true)
            {

                TcpClient client = listener.AcceptTcpClient();
                NetworkStream stream = client.GetStream();
                byte[] data = new byte[client.ReceiveBufferSize];
                int bytesRead = stream.Read(data, 0, client.ReceiveBufferSize);
                string dataRecieved = Encoding.ASCII.GetString(data, 0, bytesRead);

                int firstdataDelim = dataRecieved.IndexOf("%1") + 2;
                int firstdataBorder = dataRecieved.LastIndexOf("%2");
                int seconddataDelim = dataRecieved.IndexOf("%2") + 2;
                int seconddataBorder = dataRecieved.LastIndexOf("%3");
                int thirddataDelim = dataRecieved.IndexOf("%3") + 2;
                int thirddataBorder = dataRecieved.LastIndexOf("%4");
                int fourthdataDelim = dataRecieved.IndexOf("%4") + 2;
                int fourthdataBorder = dataRecieved.LastIndexOf("%5");
                int fivethdataDelim = dataRecieved.IndexOf("%5") + 2;
                int fivethdataBorder = dataRecieved.LastIndexOf("%6");

                if (dataRecieved.StartsWith("1"))
                {
                    Console.WriteLine("Command_1");
                }
                else if (dataRecieved.StartsWith("registration"))
                {
                    Console.WriteLine("Registration of new user" + '\n');
                    string receivedUserLogin = dataRecieved.Substring(firstdataDelim, firstdataBorder - firstdataDelim);
                    string receivedUserPassword = dataRecieved.Substring(seconddataDelim, seconddataBorder - seconddataDelim);
                    string receivedUserFirstName = dataRecieved.Substring(thirddataDelim, thirddataBorder - thirddataDelim);
                    string receivedUserLastName = dataRecieved.Substring(fourthdataDelim, fourthdataBorder - fourthdataDelim);
                    string receivedUserEmail = dataRecieved.Substring(fivethdataDelim, fivethdataBorder - fivethdataDelim);
                    Console.WriteLine("Login of a new client: " + "{0}", receivedUserLogin);
                    Console.WriteLine("Password of a new client: " + "{0}", receivedUserPassword);
                    Console.WriteLine("FirstName of a new client: " + "{0}", receivedUserFirstName);
                    Console.WriteLine("LastName of a new client: " + "{0}", receivedUserLastName);
                    Console.WriteLine("Email of a new client: " + "{0}", receivedUserEmail);

                    cmdCheckUserEmail = new SqlCommand("select count(*) from Clients where Email = @param5", con);
                    cmdCheckUserEmail.Parameters.AddWithValue("@param5", receivedUserEmail);
                    cmdCheckUserEmail.ExecuteNonQuery();

                    int uservalid = Convert.ToInt32(cmdCheckUserEmail.ExecuteScalar());
                    if(uservalid == 0)
                    {
                        cmdaddNewUser = new SqlCommand("insert into Clients(Login, Password, FirstName, LastName, Email) values(@param1, @param2, @param3, @param4, @param5)", con);
                        cmdaddNewUser.Parameters.AddWithValue("@param1", receivedUserLogin);
                        cmdaddNewUser.Parameters.AddWithValue("@param2", receivedUserPassword);
                        cmdaddNewUser.Parameters.AddWithValue("@param3", receivedUserFirstName);
                        cmdaddNewUser.Parameters.AddWithValue("@param4", receivedUserLastName);
                        cmdaddNewUser.Parameters.AddWithValue("@param5", receivedUserEmail);
                        cmdaddNewUser.ExecuteNonQuery();
                        Console.WriteLine("User " + receivedUserLogin + " registered" + '\n');
                        string commandRegistrationSuccess = "success_registration";
                        byte[] bytescommandRegistrationSuccess = ASCIIEncoding.ASCII.GetBytes(commandRegistrationSuccess);
                        stream.Write(bytescommandRegistrationSuccess, 0, bytescommandRegistrationSuccess.Length);                    
                    }
                    else
                    {
                        Console.WriteLine("User with this email is exsit" + '\n');
                        string commandRegistrationSuccess = "error_registration";
                        byte[] bytescommandRegistrationSuccess = ASCIIEncoding.ASCII.GetBytes(commandRegistrationSuccess);
                        stream.Write(bytescommandRegistrationSuccess, 0, bytescommandRegistrationSuccess.Length);
                    }
                }
                else if (dataRecieved.StartsWith("login"))
                {
                    Console.WriteLine("Loggining" + '\n');
                    string receivedUserLogin = dataRecieved.Substring(firstdataDelim, firstdataBorder - firstdataDelim);
                    string receivedUserPassword = dataRecieved.Substring(seconddataDelim, seconddataBorder - seconddataDelim);

                    cmdloguser = new SqlCommand("select count(*) from Clients where Login = @param1 and Password = @param2", con);
                    cmdloguser.Parameters.AddWithValue("param1", receivedUserLogin);
                    cmdloguser.Parameters.AddWithValue("param2", receivedUserPassword);
                    cmdloguser.ExecuteNonQuery();
                    int num = Convert.ToInt32(cmdloguser.ExecuteScalar());
                    if (num == 1)
                    {
                        Console.WriteLine("User " + receivedUserLogin + " is logged in" + '\n');
                        string commandAuthorizationSuccess = "success_user";
                        byte[] bytescommandAuthorizationSuccess = ASCIIEncoding.ASCII.GetBytes(commandAuthorizationSuccess);
                        stream.Write(bytescommandAuthorizationSuccess, 0, bytescommandAuthorizationSuccess.Length);
                    }
                    else if (num != 1)
                    {
                        Console.WriteLine("Password or Login is not correct" + '\n');
                        string commandAuthorizationSuccess = "error_user";
                        byte[] bytescommandAuthorizationSuccess = ASCIIEncoding.ASCII.GetBytes(commandAuthorizationSuccess);
                        stream.Write(bytescommandAuthorizationSuccess, 0, bytescommandAuthorizationSuccess.Length);
                    }
                    Console.WriteLine("\n");
                    
                }
                else if (dataRecieved.StartsWith("tickinfo1"))
                {
                    Console.WriteLine("Ticket searching..." + '\n');
                    string receivedTicketFrom = dataRecieved.Substring(firstdataDelim, firstdataBorder - firstdataDelim);
                    string receivedTicketTo = dataRecieved.Substring(seconddataDelim, seconddataBorder - seconddataDelim);
                    string receivedTicketData = dataRecieved.Substring(thirddataDelim, thirddataBorder - thirddataDelim);
                    string receivedTicketClass = dataRecieved.Substring(fourthdataDelim, fourthdataBorder - fourthdataDelim);
                    string receivedLogin = dataRecieved.Substring(fivethdataDelim, fivethdataBorder - fivethdataDelim);

                    cmdsearchtick = new SqlCommand("select count(*) from Tickets where PlaceFrom = @param1 and PlaceTo = @param2 and Dates = @param3 and Class = @param4", con);
                    cmdsearchtick.Parameters.AddWithValue("param1", receivedTicketFrom);
                    cmdsearchtick.Parameters.AddWithValue("param2", receivedTicketTo);
                    cmdsearchtick.Parameters.AddWithValue("param3", receivedTicketData);
                    cmdsearchtick.Parameters.AddWithValue("param4", receivedTicketClass);
                    cmdsearchtick.ExecuteNonQuery();

                    int num = Convert.ToInt32(cmdsearchtick.ExecuteScalar());
                    if (num != 0)
                    {
                        Console.WriteLine("Ticket: " + receivedTicketFrom + " --> " + receivedTicketTo + " on " + receivedTicketData + " with class " + receivedTicketClass + " is exist");

                        cmdsearchtickid = new SqlCommand("select (Ticket_ID) from Tickets where PlaceFrom = @param1 and PlaceTo = @param2 and Dates = @param3 and Class = @param4", con);
                        cmdsearchtickid.Parameters.AddWithValue("param1", receivedTicketFrom);
                        cmdsearchtickid.Parameters.AddWithValue("param2", receivedTicketTo);
                        cmdsearchtickid.Parameters.AddWithValue("param3", receivedTicketData);
                        cmdsearchtickid.Parameters.AddWithValue("param4", receivedTicketClass);
                        cmdsearchtickid.ExecuteNonQuery();
                        string id = Convert.ToString(cmdsearchtickid.ExecuteScalar());
                        Console.WriteLine("Ticket ID: " + id);

                        cmdsearchprice = new SqlCommand("select (Price) from Tickets where PlaceFrom = @param1 and PlaceTo = @param2 and Dates = @param3 and Class = @param4", con);
                        cmdsearchprice.Parameters.AddWithValue("param1", receivedTicketFrom);
                        cmdsearchprice.Parameters.AddWithValue("param2", receivedTicketTo);
                        cmdsearchprice.Parameters.AddWithValue("param3", receivedTicketData);
                        cmdsearchprice.Parameters.AddWithValue("param4", receivedTicketClass);
                        cmdsearchprice.ExecuteNonQuery();
                        string price = Convert.ToString(cmdsearchprice.ExecuteScalar());
                        Console.WriteLine("Ticket price: " + price);
                        string log = Convert.ToString(receivedLogin);
                        cmdaddnewticket = new SqlCommand("insert into TicketInfo(Login, Ticket_ID, PlaceFrom, PlaceTo, Class, Price, Dates) values(@param1, @param2, @param3, @param4, @param5, @param6, @param7)", con);
                        cmdaddnewticket.Parameters.AddWithValue("@param1", log);
                        cmdaddnewticket.Parameters.AddWithValue("@param2", id);
                        cmdaddnewticket.Parameters.AddWithValue("@param3", receivedTicketFrom);
                        cmdaddnewticket.Parameters.AddWithValue("@param4", receivedTicketTo);
                        cmdaddnewticket.Parameters.AddWithValue("@param5", receivedTicketClass);
                        cmdaddnewticket.Parameters.AddWithValue("@param6", price);
                        cmdaddnewticket.Parameters.AddWithValue("@param7", receivedTicketData);
                        cmdaddnewticket.ExecuteNonQuery();

                        string commandAuthorizationSuccess = "success_ticket" + "%1" + id + "%2" + price + "%3";
                        byte[] bytescommandAuthorizationSuccess = ASCIIEncoding.ASCII.GetBytes(commandAuthorizationSuccess);
                        stream.Write(bytescommandAuthorizationSuccess, 0, bytescommandAuthorizationSuccess.Length);
                    }
                    else if (num == 0)
                    {
                        Console.WriteLine("Ticket " + receivedTicketFrom + " --> " + receivedTicketTo + " on " + receivedTicketData + " isn't exist");
                        string commandAuthorizationSuccess = "error_ticket";
                        byte[] bytescommandAuthorizationSuccess = ASCIIEncoding.ASCII.GetBytes(commandAuthorizationSuccess);
                        stream.Write(bytescommandAuthorizationSuccess, 0, bytescommandAuthorizationSuccess.Length);
                    }
                }
                else if (dataRecieved.StartsWith("info"))
                {
                    Console.WriteLine("Ticket info:");
                    string receivedTicketID = dataRecieved.Substring(firstdataDelim, firstdataBorder - firstdataDelim);
                    cmdinfofrom = new SqlCommand("select (PlaceFrom) from Tickets where Ticket_ID = @param1", con);
                    cmdinfofrom.Parameters.AddWithValue("@param1", receivedTicketID);
                    cmdinfofrom.ExecuteNonQuery();
                    string from = Convert.ToString(cmdinfofrom.ExecuteScalar());
                    Console.WriteLine("From: " + from);
                    cmdinfoto = new SqlCommand("select (PlaceTo) from Tickets where Ticket_ID = @param1", con);
                    cmdinfoto.Parameters.AddWithValue("@param1", receivedTicketID);
                    cmdinfoto.ExecuteNonQuery();
                    string to = Convert.ToString(cmdinfoto.ExecuteScalar());
                    Console.WriteLine("To: " + to);
                    cmdinfoclass = new SqlCommand("select (Class) from Tickets where Ticket_ID = @param1", con);
                    cmdinfoclass.Parameters.AddWithValue("@param1", receivedTicketID);
                    cmdinfoclass.ExecuteNonQuery();
                    string clas = Convert.ToString(cmdinfoclass.ExecuteScalar());
                    Console.WriteLine("Class: " + clas);
                    cmdinfoprice = new SqlCommand("select (Price) from Tickets where Ticket_ID = @param1", con);
                    cmdinfoprice.Parameters.AddWithValue("@param1", receivedTicketID);
                    cmdinfoprice.ExecuteNonQuery();
                    string price = Convert.ToString(cmdinfoprice.ExecuteScalar());
                    Console.WriteLine("Price: " + price + " $" + '\n');
                }
                else if (dataRecieved.StartsWith("payment"))
                {
                    string receivedPay = dataRecieved.Substring(firstdataDelim, firstdataBorder - firstdataDelim);
                    string receivedTicketID = dataRecieved.Substring(seconddataDelim, seconddataBorder - seconddataDelim);
                    Console.WriteLine("Payment: ");
                    cmdcheckforpay = new SqlCommand("select (Paid) from TicketInfo where Ticket_ID = @param1", con);
                    cmdcheckforpay.Parameters.AddWithValue("@param1", receivedTicketID);
                    string pay = Convert.ToString(cmdcheckforpay.ExecuteScalar());
                    if (pay == null)
                    {
                        Console.WriteLine("Ticket was not found ");
                        string commandAuthorizationSuccess = "error_ticket1";
                        byte[] bytescommandAuthorizationSuccess = ASCIIEncoding.ASCII.GetBytes(commandAuthorizationSuccess);
                        stream.Write(bytescommandAuthorizationSuccess, 0, bytescommandAuthorizationSuccess.Length);
                    }                  
                    else if (pay == receivedPay)
                    {
                        Console.WriteLine("Ticket was paid before ");
                        string commandAuthorizationError = "error_ticket";
                        byte[] bytescommandAuthorizationError = ASCIIEncoding.ASCII.GetBytes(commandAuthorizationError);
                        stream.Write(bytescommandAuthorizationError, 0, bytescommandAuthorizationError.Length);
                    }
                    else
                    {                   
                    cmdpayment = new SqlCommand("UPDATE TicketInfo SET Paid='Paid' WHERE Ticket_ID=@param1", con);
                    string log = Convert.ToString(receivedTicketID);
                    cmdpayment.Parameters.AddWithValue("@param1", receivedTicketID);
                    cmdpayment.ExecuteNonQuery();
                    Console.WriteLine("Ticket was paid ");
                    string commandAuthorizationSuccess = "succes_ticket";
                    byte[] bytescommandAuthorizationSuccess = ASCIIEncoding.ASCII.GetBytes(commandAuthorizationSuccess);
                    stream.Write(bytescommandAuthorizationSuccess, 0, bytescommandAuthorizationSuccess.Length);
                    }
                    
                }

            }
            //listener.Stop();
            //Console.ReadLine();
        }
    }
}