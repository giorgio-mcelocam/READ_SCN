using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.IO;
using System.Diagnostics;
using Renci;
using Renci.SshNet;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Threading;
using Renci.SshNet.Sftp;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace WindowsFormsApp1
{
    public partial class Form3 : Form
    {
        int numupl = 0;
        bool pauseWorker = false;
        bool meno = false;
        int npage = 0;
        Double peso = 0;
        public Form3()
        {
            InitializeComponent();
            InitializeBackgroundWorker();

            textBox1.Text = System.Configuration.ConfigurationManager.AppSettings["destpat"];
            textBox1.Enabled = false;
            dlg1.SelectedPath = System.Configuration.ConfigurationManager.AppSettings["inizpat"]; ;
        }
        private void InitializeBackgroundWorker()
        {
            backgroundWorker1.DoWork +=                new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker1.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(
            backgroundWorker1_RunWorkerCompleted);
            backgroundWorker1.ProgressChanged +=
                new ProgressChangedEventHandler(
            backgroundWorker1_ProgressChanged);
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            button2.Enabled = true;
            button5.Enabled = false;
            ProgressBar.CheckForIllegalCrossThreadCalls = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //pg1.Maximum = 100;
            //pg1.Step = 1;
            //pg1.Value = 0;
           
            backgroundWorker1.RunWorkerAsync();
          



        }
       
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
        private void button4_Click(object sender, EventArgs e)
        {
         //   this.backgroundWorker1.CancelAsync();
         if (pauseWorker == true)
            { pauseWorker = false;  }
         else
            { 
            pauseWorker = true;
            }

       
        }
        public bool IsNumeric(string value)
        {
            return value.All(char.IsNumber);
        }
        delegate void SetTextCallback(Form f, Control ctrl, string text);
       // private ThreadPauseState _state = new ThreadPauseState();
        [STAThread]
        private void backgroundWorker1_DoWork(object sender,
           DoWorkEventArgs e)
        {
             List<string> files = new List<string>();
            List<string> files1 = new List<string>();
            // dlg1.ShowDialog();
            Invoke((Action)(() => { dlg1.ShowDialog(); }));
            string directoryName = System.Configuration.ConfigurationManager.AppSettings["destpat"] + "DOC_SCANSIONATA_" + DateTime.Now.Year.ToString() + "_" + DateTime.Now.Month.ToString("00") + "_" + DateTime.Now.Day.ToString("00") + "_" + DateTime.Now.Hour.ToString("00");
            DirectoryInfo dirInfo = new DirectoryInfo(directoryName);
            if (dirInfo.Exists == false) Directory.CreateDirectory(directoryName);

            string directoryNameR = System.Configuration.ConfigurationManager.AppSettings["destpat"] + "DOC_SCANSIONATA_" + DateTime.Now.Year.ToString() + "_" + DateTime.Now.Month.ToString("00") + "_" + DateTime.Now.Day.ToString("00") + "_" + DateTime.Now.Hour.ToString("00") + "RENAMED";
            DirectoryInfo dirInfoR = new DirectoryInfo(directoryNameR);
            if (dirInfoR.Exists == false) Directory.CreateDirectory(directoryNameR);

            string directoryNameINS = System.Configuration.ConfigurationManager.AppSettings["risulpat"] + "DOC_SCANSIONATA_" + DateTime.Now.Year.ToString() + "_" + DateTime.Now.Month.ToString("00") + "_" + DateTime.Now.Day.ToString("00") + @"\INSERITI\";
            DirectoryInfo dirInfoINS = new DirectoryInfo(directoryNameINS);
            if (dirInfoINS.Exists == false) Directory.CreateDirectory(directoryNameINS);

            string directoryNameSCA = System.Configuration.ConfigurationManager.AppSettings["risulpat"] + "DOC_SCANSIONATA_" + DateTime.Now.Year.ToString() + "_" + DateTime.Now.Month.ToString("00") + "_" + DateTime.Now.Day.ToString("00") + @"\SCARTATI\";
            DirectoryInfo dirInfoSCA = new DirectoryInfo(directoryNameSCA);
            if (dirInfoINS.Exists == false) Directory.CreateDirectory(directoryNameSCA);

            string directoryNameDB = System.Configuration.ConfigurationManager.AppSettings["risulpat"] + "DOC_SCANSIONATA_" + DateTime.Now.Year.ToString() + "_" + DateTime.Now.Month.ToString("00") + "_" + DateTime.Now.Day.ToString("00") + @"\INSERITI DB\";
            DirectoryInfo dirInfoDB = new DirectoryInfo(directoryNameDB);
            if (dirInfoDB.Exists == false) Directory.CreateDirectory(directoryNameDB);



            files.AddRange((Directory.GetFiles(dlg1.SelectedPath, "*.*", SearchOption.AllDirectories)));
            int t = 0;
            foreach (string file in files)
            {
                files1.Add(Path.GetFileName(file));
                t += 1;
                //npage+=getNumberOfPdfPages(file);
                //label6.Text = "N° Pagine uplodate : " + npage.ToString();
                //label6.Visible = true;
                //label6.Update();
            }
            try
            {


                




                Double perc = 0;
                // label2.Text = perc.ToString();
                WriteLog(DateTime.Now.ToShortTimeString() + " Inizio scansione");//  Letti N°" + files1.Count.ToString());
                //   Form3.Invoke(d, new object[] { form, ctrl, text });
                numupl = Convert.ToInt32(files1.Count.ToString());
               pg1.Maximum = Convert.ToInt32(files1.Count.ToString());
                pg1.Step = 1;
                int sc = 0;
                int ins = 0;
                int y = 0;
                for (int i = 0; i < files1.Count; i++)
                {
                    y = i;
                    Thread.Sleep(300);
                    backgroundWorker1.ReportProgress(i);
                    //if (backgroundWorker1.CancellationPending == true)
                    //{
                    //    e.Cancel = true;
                    //    return;
                    //}




                    //pg1.PerformStep();
                    //   perc =Math.Round( Convert.ToDouble(pg1.Value) / Convert.ToDouble(pg1.Maximum) * 100,1);
                    //   label2.Text =perc.ToString();

                    //backgroundWorker1.ReportProgress((100 * i) / files1.Count);
                    //pg1.Value = i;
                    //label2.Text = i.ToString() + " %";
                    //Refresh();



                    // files = FilSearch(d.ToString());
                    FileInfo mFile = new FileInfo(files[i]);
                  //  peso += Math.Round(Convert.ToDouble( mFile.Length / 1024), 0);
                    string filename = files[i];
                    mFile.CopyTo(dirInfo + "\\" + files1[i], true);
                    int slt = 0;
                    //---------------------------------------------------------------------
                   
                    if (mFile.Name.Contains("_")) { slt = 1; } else { slt = 0; }


                    if ((mFile.Name.Length < 13 || mFile.Name.Length > 13) && slt == 0 || !IsNumeric(mFile.Name.Substring(0,8)))
                    {
                       // pg1.Maximum   = pg1.Maximum-1;
                     //   numupl -= 1; ;
                        // if (pg1.Value > 0) { pg1.Value -= 1; }
                        if (y > 0) { y -= 1; }
                        //Thread.Sleep(500);
                        //backgroundWorker1.ReportProgress(i);

                        if (File.Exists(directoryNameSCA + mFile.Name))
                        {
                            File.Delete(directoryNameSCA + mFile.Name);

                        }
                        //  moveDirectory(mFile.DirectoryName, directoryNameSCA);
                        mFile.MoveTo(directoryNameSCA + mFile.Name);
                        listBox2.Items.Insert(sc, mFile.Name+ "        Nome File ID Credito non Conforme ");
                        listBox2.Update();
                        sc += 1;
                        WriteLog(DateTime.Now.ToShortTimeString() + "   File " + files1[i].ToString() + "   Nome File ID Credito non Conforme ");

                    }

                    //-------------------------------------------------------------------------
                    else
                    {
                        String nmfle = "AU" + DateTime.Now.Day.ToString("00") + DateTime.Now.Hour.ToString("00") + DateTime.Now.Minute.ToString("00") + DateTime.Now.Second.ToString("00") + DateTime.Now.Millisecond.ToString("00") + "_1.pdf";


                    

                        String ID_CREDITO = files1[i].Substring(0, 8);
                        String ID_TP_DOCUMENTAZIONE = "";
                        switch (files1[i].Substring(8, 1))
                        {
                            case "C":
                                ID_TP_DOCUMENTAZIONE = "24";
                                break;
                            case "V":
                                ID_TP_DOCUMENTAZIONE = "26";
                                break;
                            case "L":
                                ID_TP_DOCUMENTAZIONE = "25";
                                break;
                            default:
                                ID_TP_DOCUMENTAZIONE = "00";
                                WriteLog(DateTime.Now.ToShortTimeString() + "   File " + files1[i].ToString() + "      Categoria non Conforme");
                                listBox2.Items.Insert(sc, mFile.Name + "      Categoria non Conforme");
                                listBox2.Update();
                                sc += 1;
                                break;
                        }
                        String idanag = readmysql(ID_CREDITO, "ID_DEBITORE");
                        String DTA_INS = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString("00") + "-" + DateTime.Now.Day.ToString("00") + " " + DateTime.Now.ToLongTimeString();
                        String PERCORSO = "https://normarec.mcelocam.com/NormaRec/Archive/";// " / opt/tomcat/webapps//NormaRec/Archive";
                        String NOMEFILE = DateTime.Now.Year.ToString() + "/" + DateTime.Now.Month.ToString("00") + "/" + nmfle;// files1[i];
                                                                                                                               //  String NOMEFILE = "/" + "2000" + "/" + DateTime.Now.Month.ToString("00") + "/" + nmfle;// files1[i];
                        String inserito = " >NON  Inserito DB < ";
                        //------------------------------------------------------------------------------------------------------------
                        if (readmysqlver(idanag, ID_CREDITO, ID_TP_DOCUMENTAZIONE, "ID").Length >0 && slt == 0)
                        {
                          //  pg1.Maximum = pg1.Maximum - 1;
                            //numupl -= 1;
                            meno = true;
                            // if (pg1.Value > 0) { pg1.Value -= 1; }
                            if (y > 0) { y -= 1; }
                            if (File.Exists(directoryNameDB + mFile.Name))
                            {
                                File.Delete(directoryNameDB + mFile.Name);

                            }
                            //Thread.Sleep(500);
                            //backgroundWorker1.ReportProgress(i);
                            if (y > 0) { y -= 1; }
                         

                            mFile.MoveTo(directoryNameDB + mFile.Name);
                            //  WriteLog(DateTime.Now.ToShortTimeString() + "   File " + files1[i].ToString() + " Credito :" + ID_CREDITO + " ID Debitore :" + idanag + "--" + "PRESENTE su DB");
                            listBox2.Items.Insert(sc, mFile.Name + "        File Duplicato");
                            listBox2.Update();
                                                      sc += 1;
                            WriteLog(DateTime.Now.ToShortTimeString() + "   File " + files1[i].ToString() + "        File Duplicato");

                        }


                        else
                        {
                            if (idanag != "")
                            //    if (readmysqlver(idanag, ID_CREDITO, ID_TP_DOCUMENTAZIONE, "ID").Length > 0)
                               
                            {
                                int milliseconds = 100;
                              //  Thread.Sleep(milliseconds);
                              //backgroundWorker1.ReportProgress(i);
                              


                                insmysql(idanag, ID_CREDITO, ID_TP_DOCUMENTAZIONE, DTA_INS, PERCORSO, NOMEFILE, getNumberOfPdfPages(mFile.FullName).ToString(), Math.Round(Convert.ToDouble(mFile.Length / 1024), 0).ToString());
                                inserito = " >Inserito< ";



                                //------------------------------------------------------------------------------------------------------------

                                mFile.CopyTo(dirInfoR + "\\" + nmfle, true);

                                // // FileUploadSFTP(directoryName,files1[i]);

                               FileUploadSFTP(directoryNameR, nmfle);



                                String nome = "";
                                if (idanag == "")
                                { nome = "per " + ID_CREDITO + " ID_DEBITORE non TROVATO"; }
                                else { nome = readmysql_anag(idanag, "COGNOME") + " " + readmysql_anag(idanag, "NOME"); }
                             //   WriteLog(DateTime.Now.ToShortTimeString() + "   File " + files1[i].ToString() + " Credito :" + ID_CREDITO + " ID Debitore :" + idanag + "--" + nome + inserito);
                                WriteLog(DateTime.Now.ToShortTimeString() + "   File " + files1[i].ToString() + " Credito :" + ID_CREDITO + " ID Debitore :" + idanag + "--" + inserito);

                                if (File.Exists(directoryNameINS + mFile.Name))
                                {
                                    File.Delete(directoryNameINS + mFile.Name);

                                }
                                mFile.MoveTo(directoryNameINS + mFile.Name);
                                // Directory.Move(mFile.DirectoryName, dlg1.SelectedPath + @"\INSERITI");
                                // moveDirectory(mFile.DirectoryName, directoryNameINS);
                                listBox1.Items.Insert(ins, mFile.Name);
                                listBox1.Update();
                                ins += 1;

                            }
                            else
                            {
                              //  pg1.Maximum = pg1.Maximum - 1;
                               // numupl -= 1;
                             
                                if (y > 0) { y -= 1; }
                                //Thread.Sleep(500);
                                //backgroundWorker1.ReportProgress(i);



                                if (File.Exists(directoryNameSithub.com/giorgio-mcelocam/MCELOCAM.gCA + mFile.Name))
                                {
                                    File.Delete(directoryNameSCA + mFile.Name);

                                }
                                mFile.MoveTo(directoryNameSCA + mFile.Name);
                                if (readmysqlver(idanag, ID_CREDITO, ID_TP_DOCUMENTAZIONE, "ID").Length != 0)
                                { 
                                    listBox2.Items.Insert(sc, mFile.Name+ " ID Credito Non Presente");
                                listBox2.Update();
                                sc += 1;
                                WriteLog(DateTime.Now.ToShortTimeString() + "   File " + files1[i].ToString() + " ID Credito Non Presente");
                                }
                            }
                            //  MoveDirectory(mFile.DirectoryName, dlg1.SelectedPath + @"\INSERITI",true);

                        }

                    }
                   
                }


                Thread.Sleep(300);
                backgroundWorker1.ReportProgress(numupl);


                WriteLog(DateTime.Now.ToShortTimeString() + "   Uplodati N°" + numupl.ToString());

                string logFilePath = System.Configuration.ConfigurationManager.AppSettings["logpat"];
                logFilePath = logFilePath + "Log-" + System.DateTime.Today.ToString("MM-dd-yyyy") + "." + "txt";
                String target = System.Configuration.ConfigurationManager.AppSettings["HAN_OUT"] + "Flusso_Mensile_" + DateTime.Now.ToShortTimeString().Replace(":", "_") + ".txt";


               // sendmail(logFilePath);
              //  linkLabel1.Visible = true;
                //  linkLabel1.Text = "INSERITI n°" + ins.ToString();
                //LinkLabel.Link link = new LinkLabel.Link();
                //link.LinkData = @"\\10.0.0.11\SCANNER\RISULTATI\INSERITI\";
                // linkLabel1.Links.Add(link);
            //    MessageBox.Show("DOCUMENATLE", "TERMINATO UPLOAD");


            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }
        void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            
            //if (e.ProgressPercentage > pg1.Maximum)
            //{ }
            //else
            //{ 
            pg1.Value =  e.ProgressPercentage;
           // }
            label2.Text = Math.Round(((Convert.ToDouble(e.ProgressPercentage) / numupl)*100), 0).ToString() + "%";//e.ProgressPercentage.ToString();
           // label2.Text = Math.Round(((Convert.ToDouble(pg1.Value) / numupl) * 100), 0).ToString() + "%";//e.ProgressPercentage.ToString();

        }
        public int getNumberOfPdfPages(string fileName)
        {
            using (StreamReader sr = new StreamReader(File.OpenRead(fileName)))
            {
                Regex regex = new Regex(@"/Type\s*/Page[^s]");
                MatchCollection matches = regex.Matches(sr.ReadToEnd());

                return matches.Count;
            }
        }
        void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // parseButton.Enabled = true;
            //   MessageBox.Show("Done");
           linkLabel1.Visible = true;
            //ItemActivation k = 0;

            //List<string> fileslog = new List<string>();
            //// Directory.GetFiles(System.Configuration.ConfigurationManager.AppSettings["logpat"], "*.txt", SearchOption.AllDirectories).OrderByDescending(f => f.LastWriteTime).First();
            ///   fileslog.AddRange((Directory.GetFiles(System.Configuration.ConfigurationManager.AppSettings["logpat"], "*.txt", SearchOption.AllDirectories)));
            //for (int i = 0; i < fileslog.Count; i++)
            //{
            //    LinkLabel lkl = new LinkLabel();

            //    FileInfo LFile = new FileInfo(fileslog[i]);
            //    //  listView1.Items.Add(LFile.Name);
            //    //     listView1.HotTracking = true;

            //    // listView1.Items[i].SubItems.Add("hyperlynk2.text");
            //    lkl.Text = LFile.Name;
            //    lkl.Name = "lk" + i.ToString();
            //    // listBox3.Controls.Add(lkl);
            //    // listBox3.Items.Add
            //}

            //string pattern = "*.txt";
            //var dirInfo = new DirectoryInfo(System.Configuration.ConfigurationManager.AppSettings["logpat"]);
            //var file = (from f in dirInfo.GetFiles(pattern) orderby f.LastWriteTime descending select f).First();
            //linkLabel2.Text = "Cartella Log";// "LOG-->  " + file.Name;
            //linkLabel2.Visible = true;
            //linkLabel3.Text=  file.Name;
            //linkLabel3.Visible = true;
            //listBox3.Visible = true;

            button6.Visible = true;
            //label6.Text = "N° Pagine uplodate : " + npage.ToString();
            //label6.Visible = true;
            //button2.Enabled = true;
            button5.Enabled = true;
            //label7.Text = String.Format(peso.ToString(),  "{N2}");
            //label7.Visible = true;
            MessageBox.Show("UPLOAD DOCUMENATLE Caricati --> "+ numupl.ToString(), "UPLOAD");
         

           
            
        }

        public static void WriteLog(string strLog)
        {
            StreamWriter log;
            FileStream fileStream = null;
            DirectoryInfo logDirInfo = null;
            FileInfo logFileInfo;

            string logFilePath = System.Configuration.ConfigurationManager.AppSettings["logpat"];
            logFilePath = logFilePath + "Log-" + System.DateTime.Now.ToString("MM-dd-yyyy HH") + "." + "txt";
            logFileInfo = new FileInfo(logFilePath);
            logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
            if (!logDirInfo.Exists) logDirInfo.Create();
            if (!logFileInfo.Exists)
            {
                fileStream = logFileInfo.Create();
            }
            else
            {
                fileStream = new FileStream(logFilePath, FileMode.Append);
            }
            log = new StreamWriter(fileStream);
            log.WriteLine(strLog);
            log.Close();
        }
        private void insmysql(String idanag, String ID_CREDITO, String ID_TP_DOCUMENTAZIONE, String DTA_INS, String PERCORSO, String NOMEFILE,String npage,String peso)
        {


            string connstr = System.Configuration.ConfigurationManager.ConnectionStrings["nrec"].ToString();
            // "server=10.0.0.22;user id=root;password=R0ma.2018;persistsecurityinfo=False;database=NormaRecTest;Allow Zero Datetime=True;Convert Zero Datetime=True;";// Dts.Connections[0].ConnectionString;
            MySqlConnection sourceConnection = new MySqlConnection(connstr);
            String cmdins = "INSERT INTO DOCUMENTAZIONE(ID_ANAGRAFICA, ID_CREDITO, ID_TP_DOCUMENTAZIONE, DATA_INS, PERCORSO, NOMEFILE,PAGINE,PESO) VALUES(";
            cmdins += "'" + idanag + "'," + ID_CREDITO + "," + ID_TP_DOCUMENTAZIONE + ",'" + DTA_INS + "','" + PERCORSO + "','" + NOMEFILE+"','" +npage+"','" +peso + "')";

            MySqlCommand commandSourceData = new MySqlCommand(cmdins, sourceConnection);
            sourceConnection.Open();
            try
            {
                commandSourceData.ExecuteNonQuery();
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
            sourceConnection.Close();

        }
        private String readmysql(String codrap, String camp)
        {

            String idanag = "";
            string connstr = System.Configuration.ConfigurationManager.ConnectionStrings["nrec"].ToString();
            //"server=10.0.0.22;user id=root;password=R0ma.2018;persistsecurityinfo=True;database=NormaRecTest;Allow Zero Datetime=True;Convert Zero Datetime=True;";// Dts.Connections[0].ConnectionString;
            MySqlConnection sourceConnection = new MySqlConnection(connstr);
            MySqlCommand commandSourceData = new MySqlCommand("SELECT ID_DEBITORE FROM RAPPORTI WHERE ID_CREDITO=" + codrap + " AND ID_TP_RAPPORTO=1 ", sourceConnection);
            sourceConnection.Open();
            MySqlDataReader reader = commandSourceData.ExecuteReader();

            while (reader.Read())
            {
                idanag = reader.GetString(camp);

            }
            sourceConnection.Close();
            return idanag;

        }
        private String readmysqlver(String anag, String cred, String tpd, String camp)
        {

            String idanag = "";
            string connstr = System.Configuration.ConfigurationManager.ConnectionStrings["nrec"].ToString();
            //"server=10.0.0.22;user id=root;password=R0ma.2018;persistsecurityinfo=True;database=NormaRecTest;Allow Zero Datetime=True;Convert Zero Datetime=True;";// Dts.Connections[0].ConnectionString;
            MySqlConnection sourceConnection = new MySqlConnection(connstr);
            MySqlCommand commandSourceData = new MySqlCommand("SELECT  * FROM DOCUMENTAZIONE WHERE ID_ANAGRAFICA=" + anag + " and ID_CREDITO=" + cred + " and ID_OPERATORE_INS=1 AND ID_TP_DOCUMENTAZIONE=" + tpd + " limit 1", sourceConnection);
            sourceConnection.Open();
            MySqlDataReader reader = commandSourceData.ExecuteReader();

            while (reader.Read())
            {
                idanag = reader.GetString(camp);

            }
            sourceConnection.Close();
            return idanag;

        }
        private String readmysql_anag(String codrap, String camp)
        {

            String idanag = "";
            string connstr = System.Configuration.ConfigurationManager.ConnectionStrings["nrec"].ToString();
            //"server=10.0.0.22;user id=root;password=R0ma.2018;persistsecurityinfo=True;database=NormaRecTest;Allow Zero Datetime=True;Convert Zero Datetime=True;";// Dts.Connections[0].ConnectionString;
            MySqlConnection sourceConnection = new MySqlConnection(connstr);
            MySqlCommand commandSourceData = new MySqlCommand("SELECT * FROM ANAGRAFICHE WHERE ID=" + codrap, sourceConnection);
            sourceConnection.Open();
            MySqlDataReader reader = commandSourceData.ExecuteReader();

            while (reader.Read())
            {
                idanag = reader.GetString(camp);

            }
            sourceConnection.Close();
            return idanag;

        }
        public void FileUploadSFTP(String dir, String fle)
        {
            var host = "normarec.mcelocam.com";
            var port = 22;
            var username = "gabriele";
            var password = "R0ma.2018";
            try
            {
               // numupl += 1;
                // http://stackoverflow.com/questions/18757097/writing-data-into-csv-file/39535867#39535867
                //  byte[] csvFile = DownloadCSV(); // Function returns byte[] csv file
                String uploadFile = dir + "\\" + fle;
                //    @"E:\SERVIZIO\NORMAREC\SCANNER\OUTPUT\DOC_SCANSIONATA_2019_3_29\" + fle;
                String sftpf= System.Configuration.ConfigurationManager.AppSettings["sftpf"];
                using (var client = new SftpClient(host, port, username, password))
                {
                    client.Connect();
                    if (client.IsConnected)
                    {

                        // Debug.WriteLine("I'm connected to the client");

                        using (FileStream fileStream = new FileStream(uploadFile, FileMode.Open))

                        {
                            if (client.Exists(sftpf + DateTime.Now.Year.ToString() + "/" + DateTime.Now.Month.ToString("00") + "/"))
                            {
                                 SftpFileAttributes attrs = client.GetAttributes(sftpf + DateTime.Now.Year.ToString() + "/" + DateTime.Now.Month.ToString("00") + "/");
                              // SftpFileAttributes attrs = client.GetAttributes(sftpf + "2000" + "/" + DateTime.Now.Month.ToString("00") + "/");

                                if (!attrs.IsDirectory)
                                {
                                    throw new Exception("not directory");
                                }
                            }
                            else
                            {
                                client.CreateDirectory(sftpf + DateTime.Now.Year.ToString() + "/" + DateTime.Now.Month.ToString("00") + "/");
                             //   client.CreateDirectory("/opt/tomcat/webapps//NormaRec/Archive/" + "2000" + "/" + DateTime.Now.Month.ToString("00") + "/");

                            }
                            //client.BufferSize = (uint)ms.Length; // bypass Payload error large files
                            //client.UploadFile(ms, GetListFileName());
                            client.BufferSize = 4 * 1024; // bypass Payload error large files
                               client.UploadFile(fileStream, sftpf + DateTime.Now.Year.ToString() + "/" + DateTime.Now.Month.ToString("00") + "/" + Path.GetFileName(uploadFile));
                           // client.UploadFile(fileStream, "/opt/tomcat/webapps//NormaRec/Archive/" + "2000" + "/" + DateTime.Now.Month.ToString("00") + "/" + Path.GetFileName(uploadFile));

                            // +DateTime.Now.Month.ToString("00")+"/" 
                        }
                    }
                    else
                    {
                        WriteLog(DateTime.Now.ToShortTimeString() + "   " + "  Errore FTP");
                        // Debug.WriteLine("I couldn't connect");
                    }
                }
            }
            catch (System.Exception excpt)
            {
                WriteLog(DateTime.Now.ToShortTimeString() + "   " + excpt.Message);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DoProcess("net", "use /D/y K:");
            DoProcess("net", @"use K: \\10.0.0.11\SCANNER\RISULTATI");

            // var dirs2 = Directory.GetDirectories("K:");


            //   System.Diagnostics.Process.Start("explorer", "file://10.0.0.11/SCANNER/RISULTATI/INSERITI");
            //  System.Diagnostics.Process.Start("explorer", @"\\10.0.0.11\SCANNER\RISULTATI\\\10.0.0.11\scanner\RISULTATI\DOC_SCANSIONATA_2019_04_05\INSERITI\");

            //fl1.RootFolder = Environment.SpecialFolder.MyComputer;
            //fl1.SelectedPath = @"\\10.0.0.11\SCANNER\RISULTATI\\\10.0.0.11\scanner\RISULTATI\DOC_SCANSIONATA_2019_04_05\INSERITI\";

            //fl1.ShowDialog();

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = @"c:\windows\system32\explorer.exe";
            psi.Arguments = @"K:";
            Process.Start(psi);
        }
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DoProcess("net", "use /D/y M:");
            DoProcess("net", @"use M: \\10.0.0.11\SCANNER\LOG");// + System.Configuration.ConfigurationManager.AppSettings["logpat"].ToString());

            // var dirs2 = Directory.GetDirectories("K:");


            //   System.Diagnostics.Process.Start("explorer", "file://10.0.0.11/SCANNER/RISULTATI/INSERITI");
            //  System.Diagnostics.Process.Start("explorer", @"\\10.0.0.11\SCANNER\RISULTATI\\\10.0.0.11\scanner\RISULTATI\DOC_SCANSIONATA_2019_04_05\INSERITI\");

            //fl1.RootFolder = Environment.SpecialFolder.MyComputer;
            //fl1.SelectedPath = @"\\10.0.0.11\SCANNER\RISULTATI\\\10.0.0.11\scanner\RISULTATI\DOC_SCANSIONATA_2019_04_05\INSERITI\";

            //fl1.ShowDialog();

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = @"c:\windows\system32\explorer.exe";
            psi.Arguments = @"M:";
            Process.Start(psi);
        }
        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabel lnk = new LinkLabel();
            lnk = (LinkLabel)sender;
            lnk.Links[lnk.Links.IndexOf(e.Link)].Visited = true;
            //   System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
            DoProcess("net", "use /D/yN:");
            DoProcess("net", @"use N: \\10.0.0.11\SCANNER\LOG");
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = @"c:\windows\system32\notepad.exe";
            psi.Arguments = @"N:"+ linkLabel3.Text.ToString();
            Process.Start(psi);

        }
        public static string DoProcess(string cmd, string argv)
        {

            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = cmd;
            p.StartInfo.Arguments = $" {argv}";
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.WaitForExit();
            string output = p.StandardOutput.ReadToEnd();
            p.Dispose();

            return output;

        }

        private void button5_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Application.ExecutablePath); // to start new instance of application
            this.Close(); //to turn off current app
        }

        private void button6_Click(object sender, EventArgs e)
        {
            List<string> fileslog = new List<string>();
            fileslog.AddRange((Directory.GetFiles(System.Configuration.ConfigurationManager.AppSettings["logpat"], "*.txt", SearchOption.AllDirectories)));
            for (int i = 0; i < fileslog.Count; i++)
            {
                LinkLabel lkl = new LinkLabel();

                FileInfo LFile = new FileInfo(fileslog[i]);
                //  listView1.Items.Add(LFile.Name);
                //     listView1.HotTracking = true;

                // listView1.Items[i].SubItems.Add("hyperlynk2.text");
                lkl.Text = LFile.Name;
                lkl.Name = "lk" + i.ToString();
                // listBox3.Controls.Add(lkl);
                // listBox3.Items.Add
            }

            string pattern = "*.txt";
            var dirInfo = new DirectoryInfo(System.Configuration.ConfigurationManager.AppSettings["logpat"]);
            var file = (from f in dirInfo.GetFiles(pattern) orderby f.LastWriteTime descending select f).First();
            linkLabel2.Text = "Cartella Log";// "LOG-->  " + file.Name;
            linkLabel2.Visible = true;
            linkLabel3.Text = file.Name;
            linkLabel3.Visible = true;
        }
    }
}
