using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;


namespace ApiTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            connObject = new ConnectionObject(ConnectionAddress.Text);
            connObject.DataRetrievingInProgressTicker += UpdateConnectionLabel;

            connObject.ConnectToAPIService();
        }

        private void CheckConnection_Click(object sender, RoutedEventArgs e)
        {

        }

        private int connectionDotsCounter;
        private void UpdateConnectionLabel(Object sender, EventArgs e)
        {
            connectionDotsCounter = connectionDotsCounter + 1;
            if (connectionDotsCounter == 4)
                connectionDotsCounter = 0;

            string labelCapture = "Connecting";
            if (connectionDotsCounter > 0)
            {
                for(int i=1; i<= connectionDotsCounter; i++)
                {
                    labelCapture = labelCapture + ".";
                }
            }
            Dispatcher.Invoke(()=> Connection.Content = labelCapture);
            
        }

        #region "Connecting to server"

        private ConnectionObject connObject;

        private class ConnectionObject
        {

            string URL { get; set; }
            private HttpClient client;
            private Thread tRetrieving;

            public enum ConnectionStateTypes
            {
                NotPerformed = 1,
                InProgress = 2,
                Connected = 3,
                Error = 4
            }
            ConnectionStateTypes ConnectionState { get; set; }
            string ConnectionErrorMsg { get; set; }

            public ConnectionObject(string URL)
            {
                this.URL = URL;

            }

            public async void ConnectToAPIService(string URL = "")
            {
                if (URL == "")
                {
                    URL = this.URL;
                }

                client = new HttpClient();
                tRetrieving = new Thread(
                    () =>
                    {
                        while (true)
                        {
                            Thread.Sleep(300);
                            DataRetrievingInProgressTicker(this, new EventArgs());

                        }
                    }
                    );
                tRetrieving.Start();

                this.ConnectionState = ConnectionStateTypes.InProgress;

                var stringData = "";
                try
                {
                    //made some changes (testing GIT)
                    stringData = await client.GetStringAsync("http://" + URL /* +  ":8080/api/account/userinfo"*/);
                    this.ConnectionState = ConnectionStateTypes.Connected;
                }
                catch(Exception c)
                {
                    this.ConnectionState = ConnectionStateTypes.Error;
                    ConnectionErrorMsg = c.Message;
                }

                stringData = "";
                ConnectionComplete(this, new EventArgs());
                tRetrieving.Abort();
            }

            public event EventHandler DataRetrievingInProgressTicker;
            public event EventHandler ConnectionComplete;

        }

        #endregion


    }
}
