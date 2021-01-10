using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;
using System.Windows.Documents;
using System.Configuration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.Entity.Core.Objects;
using System.Data;
using System.Data.SqlClient;
using System.Net.Sockets;

namespace AirTicket
{
    public partial class Information : Window
    {
        static int port = 8005; // порт сервера
        static string server_ip = "127.0.0.1";
        public Information()
        {
            InitializeComponent();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
        private void Search_click(object sender, RoutedEventArgs e)
        {
            binddatagrid(Ticket.Text);
            string textToSend = "info" + "%1" + Ticket.Text.ToString() + "%2";
            TcpClient client = new TcpClient(server_ip, port);
            NetworkStream stream = client.GetStream();
            byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(textToSend);
            stream.Write(bytesToSend, 0, bytesToSend.Length);
        }
        private void Back_click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            Close();
        }
        private void binddatagrid(string Ticket_ID)
        {
            SqlConnection con = new SqlConnection();
            con.ConnectionString = ConfigurationManager.ConnectionStrings["AirTicketDB"].ConnectionString;
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = ($"select * from [TicketInfo] where Ticket_ID ='{Ticket_ID}'");
            cmd.Connection = con;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("TicketInfo");
            da.Fill(dt);
            g1.ItemsSource = dt.DefaultView;
        }
        private void Exit_click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

}
