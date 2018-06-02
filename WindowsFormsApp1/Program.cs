using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Diagnostics;
using Shared.DataModels;
using ProtoBuf;

// stand-alone, single-file version of
// the WinForm application for MSDN article
// "Asynchronous TCP Sockets as an Alternative to WCF"

namespace Client
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }

    public class Form1 : Form
    {
        private static async Task<string> SendRequest(string server, int port, string method, string data)
        {
            try
            {
                // set up IP address of server
                IPAddress ipAddress = null;
                IPHostEntry ipHostInfo = Dns.GetHostEntry(server);
                for (int i = 0; i < ipHostInfo.AddressList.Length; ++i)
                {
                    if (ipHostInfo.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        ipAddress = IPAddress.Parse("192.168.56.1");  //  ipHostInfo.AddressList[i];
                        break;
                    }
                }
                if (ipAddress == null)
                    throw new Exception("Unable to find an IPv4 address for server");

                TcpClient client = new TcpClient();
                await client.ConnectAsync(ipAddress, port); // connect to the server

                NetworkStream networkStream = client.GetStream();
                StreamWriter writer = new StreamWriter(networkStream);
                StreamReader reader = new StreamReader(networkStream);

                writer.AutoFlush = true;
                string requestData = data;  // "method=" + method + "&" + "data=" + data + "&eor"; // 'end-of-requet'
                await writer.WriteLineAsync(requestData);
                string response = await reader.ReadLineAsync();

                client.Close();

                return response;

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        } // SendRequest

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string server = "localhost";
                int port = 50000;
                string method = (string)comboBox1.SelectedItem;

                //string data = textBox1.Text;

                string data = BuildRequest(textBox1.Text);

                // technique 1, string return w/ inline await
                //string sResponse = await SendRequest(server, port, method, data);
                //double dResponse = double.Parse(sResponse);
                //listBox1.Items.Add("Sent request, waiting for response");
                //listBox1.Items.Add("Received response: " + dResponse.ToString("F2"));

                // technique 2, Task<string> return w/ later await

                var sw = Stopwatch.StartNew();
                Task<string> tsResponse = SendRequest(server, port, method, data);
                listBox1.Items.Add("Sent request, waiting for response");
                await tsResponse;
                double dResponse = double.Parse(tsResponse.Result);
                sw.Stop();

                listBox1.Items.Add("Received response: " + dResponse.ToString("F2") + $" ; TimeInMs={sw.ElapsedMilliseconds}");
            }
            catch (Exception ex)
            {
                listBox1.Items.Add(ex.Message);
            }
        }

        private string BuildRequest(string data)
        {
            var request = new Request
            {
                CorrelationID = Guid.NewGuid(),
                Name = data
            };

            using (var stream = new MemoryStream())
            {
                Serializer.SerializeWithLengthPrefix(stream, request, PrefixStyle.Base128);
                stream.Position = 0;
                string stringBase64 = Convert.ToBase64String(stream.ToArray());

                return stringBase64;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listBox1.Items.Add("Hello");
        }

        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;

        public Form1()
        {
            InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }


        private void InitializeComponent()
        {
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "average",
            "minimum"});
            this.comboBox1.Location = new System.Drawing.Point(14, 37);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(168, 21);
            this.comboBox1.TabIndex = 0;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(14, 82);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(433, 20);
            this.textBox1.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(14, 131);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Send Async";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(12, 182);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(435, 160);
            this.listBox1.TabIndex = 4;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(107, 131);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "Say Hello";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Select method";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(184, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Enter space-delimited data (like 3 1 8)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 115);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Send request";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(188, 136);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(259, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "(responsive even while waiting for service to respond)";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(460, 362);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.comboBox1);
            this.Name = "Form1";
            this.Text = "WinForm TCP Service Client";
            this.ResumeLayout(false);
            this.PerformLayout();
        } // InitializeComponent()


    } // Form1




} // ns