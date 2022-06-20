using agsXMPP;
using agsXMPP.protocol.client;
using System;

namespace Connect360
{
    public class Connect360Client: XmppClientConnection
    {
        //public string Connect() {

        //    this.ConnectServer = _IP;
        //    this.Server = _Server;
        //    this.Port = Convert.ToInt32(_Port);
        //    this.Username = txtECRID.Text;
        //    this.Password = txtPassword.Text;
        //    this.SocketConnectionType = SocketConnectionType.Direct;
        //    this.KeepAlive = true;
        //    this.AutoResolveConnectServer = false;
        //    this.AutoRoster = false;
        //    this.UseStartTLS = false;
        //    this.AutoAgents = false;
        //    this.UseSSL = false;

        //    try
        //    {
        //        //xmpp.Open(jidSender.User, txtPassword.Text);
        //        this.Open();
        //        this.OnMessage += new MessageHandler(xmpp_Message);
        //        this.OnSaslStart += new agsXMPP.sasl.SaslEventHandler(OnSaslStart);
        //        this.OnLogin += new ObjectHandler(xmpp_OnLogin);
        //    }
        //    catch (Exception ex)
        //    {
        //       ex.Message;
        //    }
        //    Thread.Sleep(500);

        //    txtLogs.Text = "xmpp Connection State= " + xmpp.XmppConnectionState + System.Environment.NewLine;
        //    //txtLogs.Text += "xmpp Authenticated= " + xmpp.Authenticated + System.Environment.NewLine;
        //    if (xmpp.Authenticated == true)
        //    {
        //        //Getpresence();
        //    }

        //}


        public void SendMessage(string PosID,string Domain,string Messge) {

            this.Send(new Message(new Jid(PosID + Domain),
                     MessageType.chat, Messge, null, DateTime.Now.ToString()));
        }

    }
}
