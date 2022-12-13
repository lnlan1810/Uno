using MultiplayerUNO.UI.Players;
using MultiplayerUNO.UI.BUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using MultiplayerUNO.Utils;
using LitJson;
using System.Collections;
using System.Runtime.CompilerServices;

namespace MultiplayerUNO.UI.Login {
    public partial class LoginForm : Form {
        private Random rdm = new Random();
        private ArrayList Players = ArrayList.Synchronized(new ArrayList());
        public volatile int SeatID = -1;

        public LoginForm() {
            InitializeComponent();
            MsgAgency.LoginForm = this;
        }

        private void BtnServer_Click(object sender, EventArgs e) {
            SCSelect.PlayerKind = PlayerKind.Server;
            InputGameInfo();
        }

        private void BtnClient_Click(object sender, EventArgs e) {
            SCSelect.PlayerKind = PlayerKind.Client;
            InputGameInfo();
        }

        private void BtnRechoose_Click(object sender, EventArgs e) {
            UIInvoke(() => {
                this.GrpUserInfo.Hide();
                this.BtnClient.Show();
                this.BtnServer.Show();
            });
        }

        private void InputGameInfo() {
            UIInvoke(() => {
                this.BtnClient.Hide();
                this.BtnServer.Hide();
                // The server and the client display different words
                this.LblHost.Text = SCSelect.HostInfo;
                this.LblPort.Text = SCSelect.PortInfo;

                // The server does not need to set a domain name
                if (SCSelect.PlayerKind == PlayerKind.Server) {
                    this.TxtHost.Text = SCSelect.DEFAULT_HOST;
                    this.TxtHost.Enabled = false;
                } else {
                    this.TxtHost.Enabled = true;
                }

                // SCSelect.DEFAULT_NAME 
                this.TxtUserName.Text =
                (SCSelect.PlayerKind == PlayerKind.Client ? "Client" : "Server")
                + BUtil.RandomNumberString(3);

                this.GrpUserInfo.Show();

            });
        }

        private void LoginForm_Load(object sender, EventArgs e) {
            UIInvoke(() => {
                InitGUIPosition();
                this.GrpUserInfo.Hide();
                this.GrpReady.Hide();
                // set default
                this.TxtHost.Text = SCSelect.DEFAULT_HOST;
                this.TxtPort.Text = SCSelect.DEFAULT_PORT;

                this.TxtUserName.Text =
                    SCSelect.DEFAULT_NAME + BUtil.RandomNumberString(3);
            });
        }

        /// <summary>
        /// Encapsulates complex BeginInvoke (asynchronous)
        /// </summary>
        private void UIInvoke(Action fun) {
            if (this.InvokeRequired) {
                this.BeginInvoke(new Action(() => { fun(); }));
            } else { fun(); }
        }

        /// <summary>
        /// Clear all players state
        /// </summary>
        public void ResetPlayerState() {
            Players.Clear();
        }

        /// <summary>
        /// Where to initialize the component
        /// </summary>
        private void InitGUIPosition() {
            SetPositionRelativeToTheFormCenter(this.BtnServer, 0.2f);
            SetPositionRelativeToTheFormCenter(this.BtnClient, -0.2f);
            SetPositionRelativeToTheFormCenter(this.GrpUserInfo);
            SetPositionRelativeToTheFormCenter(this.GrpReady);
        }

        /// <summary>
        /// Place a component in the center of the Form, you can set the offset,
        /// The ratio of the offset is relative to the entire Form
        /// </summary>
        private void SetPositionRelativeToTheFormCenter(Control control,
            float xOffsetRatio = 0, float yOffsetRatio = 0) {
            int w = this.ClientSize.Width,
                h = this.ClientSize.Height;
            int x = (w - control.Size.Width) / 2,
                y = (h - control.Size.Height) / 2;
            x += (int)(xOffsetRatio * w);
            y += (int)(yOffsetRatio * h);
            control.Location = new Point(x, y);
        }

        /// <summary>
        /// Games start
        /// </summary>
        private void BtnJoinGame_Click(object sender, EventArgs e) {
            // Input cannot contain '$' (taken)
            string name = this.TxtUserName.Text;
            if (name.IndexOf('$') != -1) {
                MessageBox.Show("Username cannot contain characters'$'");
                return;
            }
            SetAllControlsEnable(false);
            Task.Run(() => { InitializeAdapter(); });
        }

        /// <summary>
        /// Initialize related components for front-end and back-end communication and network communication
        /// </summary>
        private void InitializeAdapter() {
            SCSelect.UserHost = this.TxtHost.Text;
            SCSelect.UserPort = this.TxtPort.Text;
            SCSelect.UserName = this.TxtUserName.Text;

            // header information
            JsonData header = new JsonData();
            header["version"] = MsgAgency.ProtocolVersion;
            header["name"] = SCSelect.UserName;
            try {
                if (SCSelect.PlayerKind == PlayerKind.Server) {
                    // Server opening needs: port, (opening server) player name
                    MsgAgency.PlayerAdapter = new LocalPlayerAdapter(
                        int.Parse(SCSelect.UserPort), header.ToJson());
                } else {
                    // Connecting to other people's servers requires: ip (domain name), port
                    MsgAgency.PlayerAdapter = new RemotePlayerAdapter(
                        TxtHost.Text, int.Parse(TxtPort.Text));
                }
                // No matter what adapter must be initialized first
                MsgAgency.PlayerAdapter.Initialize();

                // ready button
                MsgAgency.ShowInfoThread = new Thread(MsgAgency.WaitForMsgFromBackend);
                MsgAgency.ShowInfoThread.IsBackground = true; // background thread
                MsgAgency.ShowInfoThread.Start();
            } catch (Exception e) {
                MessageBox.Show(e.Message);
                return;
            } finally {
                UIInvoke(() => { SetAllControlsEnable(true); });
            }
            // Successfully joined the game / successfully opened the server
            UIInvoke(() => {
                // GUI configuration
                this.GrpUserInfo.Hide();
                this.GrpReady.Show();
                // Set to true when the prepared number is equal to the total number
                this.BtnStart.Enabled = false;
                this.BtnCancelReady.Enabled = false;
            });
            // At this point you should also send your own information
            // The client of the server does not need to send, only the remote client needs to send
            if (SCSelect.PlayerKind == PlayerKind.Client) {
                JsonData json = new JsonData();
                json["version"] = MsgAgency.ProtocolVersion;
                json["name"] = SCSelect.UserName;
                MsgAgency.SendOneJsonDataMsg(json);
            }
            // I think the server will not send its own message when it starts
            // So here it is handled as follows
            if (SCSelect.PlayerKind == PlayerKind.Server) {
                MsgAgency.WhenFirstEnterTheRoom = false; // There is no initial { type=0 } message on the server
                SeatID = 0;
                SetPlayerState(0, PlayerState.WAIT);
                UpdateReadyInfo();
            }
        }

        /// <summary>
        /// Set the player's state (locked)
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetPlayerState(int seatID, PlayerState ps) {
            int seatID1 = seatID + 1;
            while (seatID1 >= Players.Count) {
                Players.Add(PlayerState.LEFT);
            }
            Players[seatID] = ps;
            UpdateReadyInfo();
        }

        /// <summary>
        /// Update the message prompt of how many people are preparing now
        /// </summary>
        public void UpdateReadyInfo() {
            int ready = 0, total = 0;
            foreach (PlayerState ps in Players) {
                if (ps == PlayerState.WAIT) {
                    ++total;
                } else if (ps == PlayerState.READY) {
                    ++ready; ++total;
                }
            }
            UIInvoke(() => {
                this.LblReadyInfo.Text = GetReadyInfo(ready, total);
                bool enable = (ready == total) &&
                    (total >= Backend.Room.MinPlayerNumber
                        && total <= Backend.Room.MaxPlayerNumber);
                this.BtnStart.Enabled = enable;
                // see if you are ready
                if (SeatID != -1) {
                    bool imReady = (PlayerState)Players[SeatID] == PlayerState.READY;
                    this.BtnCancelReady.Enabled = imReady;
                    this.BtnReady.Enabled = !imReady;
                }
            });
        }

        /// <summary>
        /// Get a String("x/x players are ready")
        /// </summary>
        private string GetReadyInfo(int ready, int total) {
            return ready.ToString()
                + "/" + total.ToString()
                + " player is ready";
        }

        /// <summary>
        /// Set the Enable of the control
        /// </summary>
        private void SetAllControlsEnable(bool enable) {
            foreach (Control c in this.Controls) {
                c.Enabled = enable;
            }
        }

        private void BtnReady_Click(object sender, EventArgs e) {
            JsonData json = new JsonData() {
                ["type"] = 1
            };
            MsgAgency.SendOneJsonDataMsg(json);
            SetReadyBtnsEnable(false);
        }

        /// <summary>
        /// Set Enable of BtnReady, BtnCancelReady
        /// </summary>
        private void SetReadyBtnsEnable(bool canReady) {
            this.BtnReady.Enabled = canReady;
            this.BtnCancelReady.Enabled = !canReady;
        }

        /// <summary>
        /// start the game
        /// </summary>
        private void BtnStart_Click(object sender, EventArgs e) {
            JsonData json = new JsonData() {
                ["type"] = 3
            };
            MsgAgency.SendOneJsonDataMsg(json);
        }

        private void BtnCancelReady_Click(object sender, EventArgs e) {
            JsonData json = new JsonData() {
                ["type"] = 2
            };
            MsgAgency.SendOneJsonDataMsg(json);
            SetReadyBtnsEnable(true);
        }


        // For DEBUG
        public string GetUserName() {
            return this.TxtUserName.Text;
        }
    }
}
