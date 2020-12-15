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
using System.Net.Sockets;

namespace AirTicket
{
    /// <summary>
    /// Логика взаимодействия для ShowInformation.xaml
    /// </summary>
    public partial class Payment : Window
    {
        static int port = 8005; // порт сервера
        static string server_ip = "127.0.0.1";
        public Payment()
        {
            InitializeComponent();
        }
        private void radioButton1_Checked(object sender, RoutedEventArgs e)
        {
            Pay.IsEnabled = true;
        }
        private void BackPayment(object sender, RoutedEventArgs e)
        {
            Ticket1 BP1 = new Ticket1();
            BP1.Show();
            Close();
        }
        private void Payment_click(object sender, RoutedEventArgs e)
        {
            string text = "Paid";
            string txt = Convert.ToString(text);
            string textTopay = "payment" + "%1" + txt + "%2" + ID.Text.ToString() + "%3";
            TcpClient client = new TcpClient(server_ip, port);
            NetworkStream stream = client.GetStream();
            byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(textTopay);
            stream.Write(bytesToSend, 0, bytesToSend.Length);

            byte[] bytesToRead = new byte[client.ReceiveBufferSize];
            int bytesRead = stream.Read(bytesToRead, 0, client.ReceiveBufferSize);
            string messagefromserver = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
            if(messagefromserver == "succes_ticket")
            {
                MessageBox.Show("Ticket successfully paid");
                Information inf = new Information();
                inf.Show();
                Close();
            }
            else if (messagefromserver == "error_ticket")
            {
                MessageBox.Show("Ticket was paid before");
            }
            else if(messagefromserver == "error_ticket1")
            {
                MessageBox.Show("There are not ticket with this ID");
            }
        }
    }
}
