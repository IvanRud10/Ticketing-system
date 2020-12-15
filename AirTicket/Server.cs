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
        static void Serv(string[] args)
        {
            SqlCommand cmdCheckUserLogin;
            SqlCommand cmdCheckUserEmail;
            SqlCommand cmdaddNewUser;
            SqlCommand cmdloguser;
            SqlCommand cmdsearchtick;

            // получаем адреса для запуска сокета
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);

            // создаем сокет
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                // связываем сокет с локальной точкой, по которой будем принимать данные
                listenSocket.Bind(ipPoint);

                // начинаем прослушивание
                listenSocket.Listen(10);

                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    Socket handler = listenSocket.Accept();
                    // получаем сообщение
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0; // количество полученных байтов
                    byte[] data = new byte[256]; // буфер для получаемых данных

                    do
                    {
                        bytes = handler.Receive(data);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (handler.Available > 0);

                    var RecieveData = data.ToString();
                    int firstdataDelim = RecieveData.IndexOf("%1") + 2;
                    int firstdataBorder = RecieveData.LastIndexOf("%2");
                    int seconddataDelim = RecieveData.IndexOf("%2") + 2;
                    int seconddataBorder = RecieveData.LastIndexOf("%3");
                    int thirddataDelim = RecieveData.IndexOf("%3") + 2;
                    int thirddataBorder = RecieveData.LastIndexOf("%4");
                    int fourthdataDelim = RecieveData.IndexOf("%4") + 2;
                    int fourthdataBorder = RecieveData.LastIndexOf("%5");
                    int fivethdataDelim = RecieveData.IndexOf("%5") + 2;
                    int fivethdataBorder = RecieveData.LastIndexOf("%6");

                    if (RecieveData.StartsWith("1"))
                    {
                        Console.WriteLine("Command_1");
                    }
                    else if (RecieveData.StartsWith("registration"))
                    {

                        Console.WriteLine("Registration of new user" + '\n');
                        string receivedUserLogin = RecieveData.Substring(firstdataDelim, firstdataBorder - firstdataDelim);
                        string receivedUserPassword = RecieveData.Substring(seconddataDelim, seconddataBorder - seconddataDelim);
                        string receivedUserFirstName = RecieveData.Substring(thirddataDelim, thirddataBorder - thirddataDelim);
                        string receivedUserLastName = RecieveData.Substring(fourthdataDelim, fourthdataBorder - fourthdataDelim);
                        string receivedUserEmail = RecieveData.Substring(fivethdataDelim, fivethdataBorder - fivethdataDelim);
                        Console.WriteLine("Login of a new client: " + "{0}", receivedUserLogin);
                        Console.WriteLine("Password of a new client: " + "{0}", receivedUserPassword);
                        Console.WriteLine("FirstName of a new client: " + "{0}", receivedUserFirstName);
                        Console.WriteLine("LastName of a new client: " + "{0}", receivedUserLastName);
                        Console.WriteLine("Email of a new client: " + "{0}", receivedUserEmail);

                        SqlConnection con = new SqlConnection();
                        con.Open();
                        cmdCheckUserLogin = new SqlCommand($"select count(Login) from Clients where Login = @param1", con);
                        cmdCheckUserEmail = new SqlCommand($"select count(Email) from Clients where Email = @param5", con);
                        cmdaddNewUser = new SqlCommand("insert into Clients(Login, Password, FirstName, LastName, Email) values(@param1, @param2, @param3, @param4, @param5)", con);

                        cmdCheckUserEmail.Parameters.AddWithValue("@param5", receivedUserEmail);
                        cmdCheckUserLogin.Parameters.AddWithValue("@param1", receivedUserLogin);
                        cmdaddNewUser.Parameters.AddWithValue("@param1", receivedUserLogin);
                        cmdaddNewUser.Parameters.AddWithValue("@param2", receivedUserPassword);
                        cmdaddNewUser.Parameters.AddWithValue("@param3", receivedUserFirstName);
                        cmdaddNewUser.Parameters.AddWithValue("@param4", receivedUserLastName);
                        cmdaddNewUser.Parameters.AddWithValue("@param5", receivedUserEmail);
                    }
                    else if (RecieveData.StartsWith("login"))
                    {
                        Console.WriteLine("Loggining" + '\n');
                        string receivedUserLogin = RecieveData.Substring(firstdataDelim, firstdataBorder - firstdataDelim);
                        string receivedUserPassword = RecieveData.Substring(seconddataDelim, seconddataBorder - seconddataDelim);
                        SqlConnection con = new SqlConnection();
                        con.Open();
                        cmdloguser = new SqlCommand($"select count(Login) from Clients where Login = @param1 and Password = @param2", con);
                        cmdloguser.Parameters.AddWithValue("param1", receivedUserLogin);
                        cmdloguser.Parameters.AddWithValue("param2", receivedUserPassword);
                        int num = Convert.ToInt32(cmdloguser.ExecuteScalar());
                        if (num == 1)
                        {
                           Console.WriteLine("User is logged in" + '\n');
                        }
                        else
                           Console.WriteLine("Password of Login is not correct" + '\n');
                    }
                    else if (RecieveData.StartsWith("tickinfo"))
                    {
                        Console.WriteLine("Ticket searching..." + '\n');
                        string receivedTicketFrom = RecieveData.Substring(firstdataDelim, firstdataBorder - firstdataDelim);
                        string receivedTicketTo = RecieveData.Substring(seconddataDelim, seconddataBorder - seconddataDelim);
                        string receivedTicketClass = RecieveData.Substring(thirddataDelim, thirddataBorder - thirddataDelim);
                        string receivedTicketData = RecieveData.Substring(fourthdataDelim, fourthdataBorder - fourthdataDelim);
                        SqlConnection con = new SqlConnection();
                        con.Open();
                        cmdsearchtick = new SqlCommand($"select count(Ticket_ID) from Tickets where PlaceFrom = @param1 and PlaceTo = @param2 and Class = @param3 and Dates = @param4", con);
                        cmdsearchtick.Parameters.AddWithValue("param1", receivedTicketFrom);
                        cmdsearchtick.Parameters.AddWithValue("param2", receivedTicketTo);
                        cmdsearchtick.Parameters.AddWithValue("param3", receivedTicketClass);
                        cmdsearchtick.Parameters.AddWithValue("param4", receivedTicketData);
                        int num = Convert.ToInt32(cmdsearchtick.ExecuteScalar());
                        if (num == 1)
                        {
                            Console.WriteLine("This ticket exists" + '\n');
                        }
                        else
                            Console.WriteLine("Ticket not found" + '\n');
                    }


                        //Register
                      /**  string text = Encoding.Unicode.GetString(data, 0, bytes);
                    if (text == "register client")
                    {
                        Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + "Register Client");
                        string message = "You was registered";
                        data = Encoding.Unicode.GetBytes(message);
                        handler.Send(data);
                       
                    }

                    if (text == "login")
                    {
                        Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + "Login");
                        string message = "You have been logged in";
                        data = Encoding.Unicode.GetBytes(message);
                        handler.Send(data);
                    }
                      **/


                    //Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + builder.ToString());

                    // отправляем ответ
                    //string message = "ваше сообщение доставлено";
                    //data = Encoding.Unicode.GetBytes(message);
                    //handler.Send(data);
                    // закрываем сокет
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}