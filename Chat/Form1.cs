using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace Chat
{
    public partial class Form1 : Form
    {
        Socket skt;
        EndPoint epLocal, epRemote;
        public Form1()
        {
            InitializeComponent();

            skt = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            skt.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            textIp1.Text = GetIp();

        }
        private string GetIp()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if(ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "Connection Error";
        }
        private void messageCallBack(IAsyncResult aResult)
        {
            try
            {
                int size = skt.EndReceiveFrom(aResult, ref epRemote);
                if(size > 0)
                {
                    byte[] receivedData = new byte[1464];
                    receivedData = (byte[])aResult.AsyncState;
                    ASCIIEncoding eCoding = new ASCIIEncoding();
                    string receivedMessage = eCoding.GetString(receivedData);
                    listMessages.Items.Add(receivedMessage);
                }
                byte[] buffer = new byte[1500];
                skt.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(messageCallBack), buffer);
                  
                


            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                epLocal = new IPEndPoint(IPAddress.Parse(textIp1.Text), Convert.ToInt32(textPort1.Text));
                skt.Bind(epLocal);

                epRemote = new IPEndPoint(IPAddress.Parse(textIp2.Text), Convert.ToInt32(textPort2.Text));
                skt.Connect(epRemote);

                byte[] buffer = new byte[1500];
                skt.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(messageCallBack), buffer);

                button1.Text = "Connected";
                button1.Enabled = false;
                button2.Enabled = true;
                textMessage.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                ASCIIEncoding asci = new ASCIIEncoding();
                byte[] msg = new byte[1500];
                msg = asci.GetBytes(textMessage.Text);
                skt.Send(msg);
                listMessages.Items.Add("Me:" + textMessage.Text);
                textMessage.Clear();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }
    }
}
