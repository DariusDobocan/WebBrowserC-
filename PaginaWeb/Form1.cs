using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace PaginaWeb
{
    public partial class Form1 : Form
    {
        
        private List<string> lstBlock1;
        private SQLiteConnection conn = null;
        private Thread myThread;
        private BooleanSwitch sw;

    public Form1()
        {
            
            FileStream file = new FileStream("testLog.txt", FileMode.OpenOrCreate);
            TextWriterTraceListener traceListener = new TextWriterTraceListener(file);
            Trace.Listeners.Add(traceListener);
            Trace.AutoFlush = true;
            sw = new BooleanSwitch("Switch1", "TRACE SWITCH");
            InitializeComponent();
        }
        private void run()
        {
            Console.WriteLine("The thread has started");

            
                Trace.WriteLine("");
                

        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            //webBrowser1.GoHome();
            webBrowser1.Navigate("www.google.com");
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            webBrowser1.GoBack();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            webBrowser1.GoForward();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            
            webBrowser1.Navigate(this.toolStripTextBox1.Text);
            Trace.WriteLine(this.toolStripTextBox1.Text);
        }

        private void toolStripTextBox1_Click(object sender, EventArgs e)
        {

        }

        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            string url = e.Url.ToString();
            lstBlock1 = SQLiteHandler.GetAllKeywords(this.conn);
            
            var res = Task.Run(() => CheckBlock(url));

            res.Wait();

            if (res.Result)
            {
                e.Cancel = true;
                MessageBox.Show("Url blocked");
            }
        }
            private async Task<bool> CheckBlock(string url)
            {
                bool res = await Task.Run(async () =>
               {
                   return (from x in lstBlock1 where url.Contains(x) select x).Count() > 0;
               });
            return res;
            }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
           this.conn = SQLiteHandler.ConnectToDb();
            Console.WriteLine("Connected to database");

        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SQLiteHandler.DisconnectFromDB(this.conn);
            Console.WriteLine("Disconnected from database");
        }

        private void addKeywordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 kwdForm = new Form2(); 
            if(kwdForm.ShowDialog() == DialogResult.OK)
            {
                string kwd = kwdForm.getImputText();
                SQLiteHandler.InsertKeyword(this.conn, kwd);
                MessageBox.Show(string.Format("S-a introdus {0}", kwd));
            }
        }

        private void toolStripTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                webBrowser1.Navigate(this.toolStripTextBox1.Text);
                Trace.WriteLine(this.toolStripTextBox1.Text);
            }
        }

        private void cuvCheie1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        bool x = true;
        private void cuvCheie1_DropDown(object sender, EventArgs e)
        {
            lstBlock1 = SQLiteHandler.GetAllKeywords(this.conn);
            if (x == true)
            {
                foreach (string s in lstBlock1)
                {
                    cuvCheie1.Items.Add(s);
                }
                x= false;
            }
        }
    }
}
