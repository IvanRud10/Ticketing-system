using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace AirTicket
{
    public partial class RegisterWindow : Window
    {
        static int port = 8005; // port of server
        static string server_ip = "127.0.0.1";
        string mailPattern = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
        string loginPattern = @"^[a-zA-Z0-9_]+$";
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow logIn = new MainWindow();
            logIn.Show();
            Close();
        }
        private void Reg_Click(object sender, RoutedEventArgs e)
        {
            if (Password.Password != RePassword.Password)
            {
                MessageBox.Show("Passwords do not match");
            }
            else
            {
                if (Login.Text.Length == 0 || Password.Password.Length == 0 || FirstName.Text.Length == 0 || LastName.Text.Length == 0 || Email.Text.Length == 0)
                {
                    MessageBox.Show("Fill all fields!");
                }
                else if (!(Regex.IsMatch(Login.Text, loginPattern)))
                {
                    MessageBox.Show("Wrong data format of login");
                }
                else if (!(Regex.IsMatch(Email.Text, mailPattern)))
                {
                    MessageBox.Show("Wrong data format of email");
                }
                else
                {
                    string textofregistration = "registration" + "%1" + Login.Text.ToString() + "%2" + Password.Password.ToString() + "%3" + FirstName.Text.ToString() + "%4" + LastName.Text.ToString() + "%5" + Email.Text.ToString() + "%6";
                    TcpClient client = new TcpClient(server_ip, port);
                    NetworkStream stream = client.GetStream();
                    byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(textofregistration);
                    stream.Write(bytesToSend, 0, bytesToSend.Length);

                    byte[] bytesToRead = new byte[client.ReceiveBufferSize];
                    int bytesRead = stream.Read(bytesToRead, 0, client.ReceiveBufferSize);
                    string messagefromserver = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
                    if (messagefromserver.StartsWith("error_registration"))
                    {
                        MessageBox.Show("User with this email is exsit");
                    }
                    else if (messagefromserver.StartsWith("success_registration"))
                    {
                        MessageBox.Show("You successfully registered");
                        MainWindow log1 = new MainWindow();
                        log1.Show();
                        Close();
                    }
                }
            }


        }
    }
}
