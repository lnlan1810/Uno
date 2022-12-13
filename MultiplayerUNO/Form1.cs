using MultiplayerUNO.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiplayerUNO
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        PlayerAdapter playerAdapter;
        Thread showInfoThread;

        // Open service
        private void runButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Server opening needs: port, (opening server) player name
                playerAdapter = new LocalPlayerAdapter(int.Parse(portTextBox.Text), sendTextBox.Text);
                playerAdapter.Initialize(); // No matter what adapter must be initialized first

                runButton.Enabled = false;
                connectButton.Enabled = false;
                sendButton.Enabled = true;

                showInfoThread = new Thread(ShowMsgThread); // A thread dedicated to receiving server messages, the function is below
                showInfoThread.IsBackground = true; //Background thread, the main thread exits itself

                showInfoThread.Start();
            }
            catch(Exception expt)
            {
                MessageBox.Show(expt.Message);
                return;
            }
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Connecting to other people's servers requires: ip (domain name), port
                playerAdapter = new RemotePlayerAdapter(ipInputBox.Text ,int.Parse(portTextBox.Text));
                playerAdapter.Initialize(); // No matter what adapter is initialized first
                // After connecting, the server will not send any message, requesting to send your own player name first (will be modified later)

                runButton.Enabled = false;
                connectButton.Enabled = false;
                sendButton.Enabled = true;

                showInfoThread = new Thread(ShowMsgThread); // A thread dedicated to receiving server messages
                showInfoThread.IsBackground = true; // background thread

                showInfoThread.Start();
            }
            catch (Exception expt)
            {
                MessageBox.Show(expt.Message);
                return;
            }
        }


        private void ShowMsgThread()
        {
            // The received message will be in playerAdapter.RecvQueue,
            // here it is just taken out and displayed on the UI,
            // the main work will be here
            while (true)
            {
                string msg = null;
                try
                {
                    msg = playerAdapter.RecvQueue.Take();
                }catch(InvalidOperationException e)
                {
                    Console.WriteLine(e.Message);
                    break;
                }

                BeginInvoke(new Action(() =>
                {
                    outputBox.AppendText(msg + "\r\n");
                }));
            }
        }

        // send
        private void sendButton_Click(object sender, EventArgs e)
        {
            playerAdapter.SendMsg2Server(sendTextBox.Text);
            sendTextBox.Text = "";
        }
    }
}
