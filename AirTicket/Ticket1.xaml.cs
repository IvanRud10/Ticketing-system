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
    public partial class Ticket1 : Window
    {
        static int port = 8005; // port of server
        static string server_ip = "127.0.0.1";
        public Ticket1()
        {
            InitializeComponent();
        }

        private void BuyTicket(object sender, RoutedEventArgs e)
        {


            string textofticket = "tickinfo1" + "%1" + From.Text.ToString() + "%2" + To.Text.ToString() + "%3" + Date.Text.ToString() + "%4" + classs.Text.ToString() + "%5" + Login.Text.ToString() + "%6";
            TcpClient client = new TcpClient(server_ip, port);
            NetworkStream stream = client.GetStream();
            byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(textofticket);
            stream.Write(bytesToSend, 0, bytesToSend.Length);

            byte[] bytesToRead = new byte[client.ReceiveBufferSize];
            int bytesRead = stream.Read(bytesToRead, 0, client.ReceiveBufferSize);
            string messagefromserver = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);

            if (messagefromserver.StartsWith("success_ticket"))
            {
                int firstdataDelim = messagefromserver.IndexOf("%1") + 2;
                int firstdataBorder = messagefromserver.LastIndexOf("%2");
                int seconddataDelim = messagefromserver.IndexOf("%2") + 2;
                int seconddataBorder = messagefromserver.LastIndexOf("%3");

                string receivedTicketID = messagefromserver.Substring(firstdataDelim, firstdataBorder - firstdataDelim);
                string receivedPrice = messagefromserver.Substring(seconddataDelim, seconddataBorder - seconddataDelim);
                MessageBox.Show("Your Ticket ID: " + receivedTicketID  + " Price of Ticket: " + receivedPrice);
                Payment P1 = new Payment();
                P1.Show();
                Close();
            }
            else if (messagefromserver.StartsWith("error_ticket"))
            {
                MessageBox.Show("Ticket with this parametres not found");
            }
        }

        private void BackT1(object sender, RoutedEventArgs e)
        {
            MainWindow BTL = new MainWindow();
            BTL.Show();
            Close();
        }

    }
}
