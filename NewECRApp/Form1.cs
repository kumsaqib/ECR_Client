using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.Collections;
using agsXMPP.protocol.iq.roster;
using System.Threading;
using agsXMPP.net;
using System.IO;
using System.Diagnostics;
using agsXMPP.protocol.iq.last;
using agsXMPP.protocol.sasl;
using agsXMPP.sasl;
using Connect360;
namespace NewECRApp
{
    public partial class Form1 : Form
    {
        Jid jidSender;
        public static string _domainname,_IP,_Server,_Port, _CheckConnectivity;
        public static string _msg = "";
        public static Connect360Client xmpp;
        public bool _isAvaliable = false;
        public static string _sendto;
        public System.Windows.Forms.Timer _timer;
        


        public TextBox txtReceivedMessage;
        public Form1()
        {
            InitializeComponent();
            txtReceivedMessage = new TextBox();
            txtReceivedMessage.Location = new Point(106, 270);
            txtReceivedMessage.Size = new System.Drawing.Size(314, 370);
            txtReceivedMessage.ForeColor = Color.Black;
            txtReceivedMessage.Multiline = true;
            txtReceivedMessage.ScrollBars = ScrollBars.Vertical;
            this.Controls.Add(txtReceivedMessage);
        }


        //private void KeepAliveTick()
        //{
        //    // Send a ping for Keep Alive
        //    agsXMPP.protocol.extensions.ping.PingIq ping = new agsXMPP.protocol.extensions.ping.PingIq();
        //    ping.Type = agsXMPP.protocol.client.IqType.get;
        //    xmpp.Send(ping);
        //}

        private void Form1_Load(object sender, EventArgs e)
        {
            SetParams();
            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = Convert.ToInt32(_CheckConnectivity);
            _timer.Tick += _timer_Tick;
        }
        
        private void _timer_Tick(object sender, EventArgs e)
        {

            if (xmpp.XmppConnectionState.ToString() != "SessionStarted")
            {
                xmpp.Close();
                btnLogin_Click(null, null);
            }
        }

        private void SetParams()
        {
            try
            {
                string text = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"Params.txt").Trim();
                string[] lines = text.Split(';');
                string[] param;
                foreach (string line in lines)
                {
                    param = line.Trim().Split('=');
                    if (param[0].Trim() == "DomainName") _domainname = param[1];
                    if (param[0].Trim() == "IP") _IP = param[1];
                    if (param[0].Trim() == "Server") _Server = param[1];
                    if (param[0].Trim() == "Port") _Port = param[1];
                    if (param[0].Trim() == "CheckConnectivity") _CheckConnectivity = param[1];
                    

                }
            }
            catch (Exception ex)
            {
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //if (_sendto == txtPOS.Text)
            //{
                txtReceivedMessage.Text = "";
                xmpp.Send(new agsXMPP.protocol.client.Message(new Jid(txtPOS.Text + _domainname),
                                     MessageType.chat,
                                     txtdatatosend.Text,null, DateTime.Now.ToString()));
            //}
        }

        private void button4_Click(object sender, EventArgs e)
        {
            _timer.Stop();
            txtReceivedMessage.Text = "";
            txtLogs.Text = "";
            xmpp.Close();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            // _timer.Start();
            //string ss = new SASLprep().Prepare(password);
            //txtLogs.Text = string.Empty;
            //jidSender = new Jid(txtPOSID.Text + _domainname);
            //xmpp = new XmppClientConnection(jidSender.Server);


            xmpp = new Connect360Client();
            xmpp.ConnectServer = _IP;
            xmpp.Server = _Server;//txtServer.Text;
            xmpp.Port = Convert.ToInt32(_Port);
            xmpp.Username = txtECRID.Text;
            xmpp.Password = txtPassword.Text;
            xmpp.SocketConnectionType = SocketConnectionType.Direct;
            xmpp.KeepAlive = true;
            //xmpp.KeepAliveInterval = 30;
            xmpp.AutoResolveConnectServer = false;
            xmpp.AutoRoster = false;
            xmpp.UseStartTLS = false;
            xmpp.AutoAgents = false;
            //xmpp.UseStartTLS = false;
            xmpp.UseSSL = false;

            try
            {
                //xmpp.Open(jidSender.User, txtPassword.Text);
                xmpp.Open();
                xmpp.OnMessage += new MessageHandler(xmpp_Message);
                xmpp.OnSaslStart += new agsXMPP.sasl.SaslEventHandler(OnSaslStart);
                xmpp.OnLogin += new ObjectHandler(xmpp_OnLogin);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            Thread.Sleep(500);

            txtLogs.Text = "xmpp Connection State= " + xmpp.XmppConnectionState + System.Environment.NewLine;
            //txtLogs.Text += "xmpp Authenticated= " + xmpp.Authenticated + System.Environment.NewLine;
            if (xmpp.Authenticated == true)
            {
                //Getpresence();
            }

        }

        #region Delegate and Handler
        //void MessageCallBack(object sender, agsXMPP.protocol.client.Message msg, object data)
        //{
        //    //if (msg.Body != null)
        //    //{
        //    //    Console.ForegroundColor = ConsoleColor.Red;
        //    //    Console.WriteLine("{0}>> {1}", msg.From.User, msg.Body);
        //    //    Console.ForegroundColor = ConsoleColor.Green;
        //    //}

        //    _msg = "Message Sent by:" + msg.From.User + ", Message Body= " + msg.Body;
        //    if (msg.Body != string.Empty)
        //    {

        //        string _msgbody = msg.Body.ToString().Substring(0, 2);
        //        //if (_msgbody != "OK")
        //        //{
        //        //    string fromuser = msg.From.User;
        //        //    fromuser += _domainname;

        //        //    xmpp.Send(new agsXMPP.protocol.client.Message(new Jid(fromuser),
        //        //                     MessageType.chat,
        //        //                    "OK , Message Sent DT " + DateTime.Now.ToString()));
        //        //}
        //    }
        //    this.Invoke((MethodInvoker)delegate {
        //        txtReceivedMessage.Text += _msg + Environment.NewLine;
        //    });

        //}

        private void OnSaslStart(object sender, SaslEventArgs args)
        {
            args.Mechanism = agsXMPP.protocol.sasl.Mechanism.GetMechanismName(MechanismType.PLAIN);
            args.Auto = false;
        }

        public void xmpp_OnPresence(object sender, agsXMPP.protocol.client.Presence pres)
        {
            //this.Invoke((MethodInvoker)delegate {
            //    txtLogs.Text = "";
            //});

            this.Invoke((MethodInvoker)delegate {

                txtLogs.Text = "";
                txtLogs.Text += "ECR : " + pres.From.User.ToUpper() + "@" + pres.From.Server + " " + pres.Type + Environment.NewLine;

                if((pres.From.User.ToString() == txtPOS.Text ) && ( pres.Type.ToString() == "available"))
                {
                    _sendto = pres.From.User.ToString();
                    _isAvaliable = true;
                }
                else
                {
                    _sendto = pres.From.User.ToString();
                    _isAvaliable = false;
                }
            });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            xmpp.Send(new agsXMPP.protocol.client.Message(new Jid(txtPOS.Text + _domainname),
                                     MessageType.chat,
                                     txtdatatosend.Text + ", Message Sent DT" + DateTime.Now.ToString()));
        }

        public void xmpp_Message(object sender, agsXMPP.protocol.client.Message message)
        {
            this.Invoke((MethodInvoker)delegate
            {
                if (message.Subject == "Pairing")
                    txtPOS.Text = message.Body;

                else
                {
                    if (txtPOS.Text == message.From.User)

                        if (message.Subject == "UnPairing")
                            txtPOS.Text = "UnPaired";

                        else
                        {
                            var _timespan = Convert.ToDateTime(message.Thread).AddSeconds(5);
                            if (_timespan >= DateTime.Now)
                                txtReceivedMessage.Text += message.Body + Environment.NewLine;
                        }
                }
            });

            

            //bool _isAppend = true;
            //_msg = "Message Sent by:" + msg.From.User + ", Message Body= " + msg.Body;
            //int msgCompletelenngth = msg.Body.ToString().Trim().Length-39;
            //_msg = msg.Body.ToString().Substring(0,msgCompletelenngth);
            // _msg = _msg.Replace("\n","\""+System.Environment.NewLine+"\"");
            //_msg = _msg.Replace("\n", System.Environment.NewLine );

            //if (msg.Body != string.Empty)
            //{
                
                
            //    //string _msgbody = msg.Body.ToString().Substring(0, 10);
            //    //if (_msgbody == "**ONLINE**")
            //    //{
            //    //    _isAppend = false;
            //    //    btnInitiate.Enabled = true;
            //    //}
            //    //_msgbody = msg.Body.ToString().Substring(0, 8);
            //    //if (_msgbody == "**BUSY**")
            //    //{
            //    //    _isAppend = false;
            //    //    btnInitiate.Enabled = false;
            //    //}

            //}
            //if (_isAppend == true)
            //{
            //    //this.Invoke((MethodInvoker)delegate
            //    //{
            //    //    txtReceivedMessage.Text += _msg + Environment.NewLine + Environment.NewLine;
            //    //});
            //}

        }

        
        #endregion
        public void xmpp_OnLogin(object sender)
        {
            Presence p = new Presence(ShowType.chat, "Online");
            p.Type = PresenceType.available;
            xmpp.Send(p);
            xmpp.OnPresence += new PresenceHandler(xmpp_OnPresence);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtReceivedMessage.Text = "";
        }

        private void button1_Click_1(object sender, EventArgs e)
        {



            var client = new S22.Xmpp.Client.XmppClient("localhost", 5222, false);
            try
            {
                
                client.Connect();
                MessageBox.Show("1");
            }
            catch (Exception ex)
            {
                MessageBox.Show("2");
                //client.Close();
            }
            if (client.Connected == true)
            {
                try
                {
                    MessageBox.Show("3");
                    client.Authenticate("ecr", "ecr");
                    MessageBox.Show("4");
                }
                catch (System.Security.Authentication.AuthenticationException ex)
                {

                    MessageBox.Show(ex.Message.ToString());
                    MessageBox.Show("5");
                }

                if (client.Authenticated == true)
                {
                    MessageBox.Show("6");
                   // usr = usernameTxt.Text;
                    //client.Close();
                    //Form rosFrm = new roster(usernameTxt.Text, passwordTxt.Text, serverName, useTLSCheck.Checked);
                    //rosFrm.Show();
                    //this.Hide();




                     xmpp.OnMessage += new MessageHandler(xmpp_Message);
                xmpp.OnSaslStart += new agsXMPP.sasl.SaslEventHandler(OnSaslStart);
                xmpp.OnLogin += new ObjectHandler(xmpp_OnLogin);
                }
            }
        }
    }
}
