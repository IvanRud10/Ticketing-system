using System.Linq;
using System.Windows;
using System;
using System.Text;
using System.Net;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using Dapper;
using System.Data;

namespace AirTicket
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        static int port = 8005; // port of server
        static string server_ip = "127.0.0.1";
        private void LogIn_Click(object sender, RoutedEventArgs e)
        {
            if (Login.Text.Length == 0 || Password.Password.Length == 0)
            {
                MessageBox.Show("Fill all fields!");
            }
            else
            {
                string textoflogin = "login" + "%1" + Login.Text.ToString() + "%2" + Password.Password.ToString() + "%3";
                TcpClient client = new TcpClient(server_ip, port);
                NetworkStream stream = client.GetStream();
                byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(textoflogin);
                stream.Write(bytesToSend, 0, bytesToSend.Length);

                byte[] bytesToRead = new byte[client.ReceiveBufferSize];
                int bytesRead = stream.Read(bytesToRead, 0, client.ReceiveBufferSize);
                string messagefromserver = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
                if (messagefromserver.StartsWith("success_user"))
                {
                    Ticket1 Tick = new Ticket1();
                    Tick.Show();
                    Close();
                }
                else if (messagefromserver.StartsWith("error_user"))
                {               
                    MessageBox.Show("Incorrect login or password");
                }
            }
        }
            private void Reg_Click(object sender, RoutedEventArgs e)
            {
            RegisterWindow reg = new RegisterWindow();
            reg.Show();
            Close();
            }
        private void Info_Click(object sender, RoutedEventArgs e)
        {
            Information inf = new Information();
            inf.Show();
            Close();
        }
            private void Pay_Click(object sender, RoutedEventArgs e)
        {
            Payment pay = new Payment();
            pay.Show();
            Close();
        }
    }
}
