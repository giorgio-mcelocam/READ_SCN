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

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        int numupl = 0;
        private readonly BackgroundWorker worker;
        BackgroundWorker workerThread = null;
        bool _keepRunning = false;
        public Form2()
        {
            InitializeComponent();

            InstantiateWorkerThread();




            textBox1.Text = System.Configuration.ConfigurationManager.AppSettings["destpat"];
            textBox1.Enabled = false;
            dlg1.SelectedPath = System.Configuration.ConfigurationManager.AppSettings["inizpat"]; ;
        }
        private void Calculate(int i)
        {
            double pow = Math.Pow(i, i);
        }


        private void InstantiateWorkerThread()
        {
            workerThread = new BackgroundWorker();
            workerThread.ProgressChanged += WorkerThread_ProgressChanged;
            workerThread.DoWork += WorkerThread_DoWork;
            workerThread.RunWorkerCompleted += WorkerThread_RunWorkerCompleted;
            workerThread.WorkerReportsProgress = true;
            workerThread.WorkerSupportsCancellation = true;
        }
        private void WorkerThread_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            label6.Text = e.UserState.ToString();
        }
        private void WorkerThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                label6.Text = "Cancelled";
            }
            else
            {
                label6.Text = "Stopped";
            }
        }
        private void WorkerThread_DoWork(object sender, DoWorkEventArgs e)
        {
            DateTime startTime = DateTime.Now;

            _keepRunning = true;

            while (_keepRunning)
            {
                Thread.Sleep(1000);

                string timeElapsedInstring = (DateTime.Now - startTime).ToString(@"hh\:mm\:ss");

                workerThread.ReportProgress(0, timeElapsedInstring);

                if (workerThread.CancellationPending)
                {
                    // this is important as it set the cancelled property of RunWorkerCompletedEventArgs to true
                    e.Cancel = true;
                    break;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            workerThread.CancelAsync();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //FileUploadSFTP();
            dlg1.ShowDialog();
            String directoryName = textBox1.Text;
            DirectoryInfo dirInfo = new DirectoryInfo(directoryName);

            if (dirInfo.Exists == false) Directory.CreateDirectory(directoryName);

            try
            {
                foreach (string d in Directory.GetDirectories(dlg1.SelectedPath))
                {

                    List<String> MyMusicFiles = Directory.GetFiles(d, "*.file", SearchOption.AllDirectories).ToList();
                    Int32 i = 0;
                    StringBuilder csv = new StringBuilder();
                    foreach (string file in MyMusicFiles)
                    {
                        FileInfo mFile = new FileInfo(file);
                        string filename = mFile.Name.Substring(5, 4) + DateTime.Now.Day.ToString();
                        // to remove name collisions
                        //if (new FileInfo(dirInfo + "\\" + filename).Exists == false)
                        //{
                        i += 1;
                        mFile.CopyTo(dirInfo + "\\" + filename, true);

                        String newLine = string.Format("{0};{1}", filename, mFile.Name);
                        csv.AppendLine(newLine);
                        //  }

                    }
                    textBox2.Text = "N° File trasferiti " + i.ToString();

                    File.WriteAllText(System.Configuration.ConfigurationManager.AppSettings["csvtpat"] + "scan_" + DateTime.Now.ToShortTimeString().Replace(".", "_") + ".csv", csv.ToString());
                }

            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }

        static void DirSearch(string sDir)
        {
            try
            {
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    foreach (string f in Directory.GetFiles(d))
                    {
                        Console.WriteLine(f);
                    }
                    DirSearch(d);
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }
        public static List<string> FilSearch(string sDir)
        {
            List<string> files = new List<string>();
            List<string> files1 = new List<string>();
            try
            {

                foreach (string f in Directory.GetFiles(sDir))
                {
                    files.AddRange((Directory.GetFiles(sDir, "*.*", SearchOption.AllDirectories)));
                    int t = 0;
                    foreach (string file in files)
                    {
                        files1[t] = Path.GetFileName(file);
                        t += 1;
                    }
                }

            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
            return files1;
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 1; i <= 100; i++)
            {
                // Wait 100 milliseconds.
                Thread.Sleep(100);
                // Report progress.
                backgroundWorker1.ReportProgress(i);
            }


        }
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Change the value of the ProgressBar   
            pg1.Value = e.ProgressPercentage;
            // Set the text.  
            label2.Text = e.ProgressPercentage.ToString();
        }
       
        private void button2_Click(object sender, EventArgs e)
        {
            //  workerThread.RunWorkerAsync(1000);

          

            button2.Enabled = false;
            List<string> files = new List<string>();
                List<string> files1 = new List<string>();
                dlg1.ShowDialog();
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
                }



                try
                {

                Application.DoEvents();




                Double perc = 0;
                   label2.Text = perc.ToString();
                    WriteLog(DateTime.Now.ToShortTimeString() + "   Letti N°" + files1.Count.ToString());
                    pg1.Maximum = Convert.ToInt32(files1.Count.ToString());
                    pg1.Step = 1;
                    int sc = 0;
                    int ins = 0;
                    for (int i = 0; i < files1.Count; i++)
                    {
                   
                        //pg1.PerformStep();
                        //   perc =Math.Round( Convert.ToDouble(pg1.Value) / Convert.ToDouble(pg1.Maximum) * 100,1);
                        //   label2.Text =perc.ToString();

                    //backgroundWorker1.ReportProgress((100 * i) / files1.Count);
                    pg1.Value = i;
                        label2.Text = i.ToString() + " %";
                        Refresh();



                        // files = FilSearch(d.ToString());
                        FileInfo mFile = new FileInfo(files[i]);
                        string filename = files[i];
                        mFile.CopyTo(dirInfo + "\\" + files1[i], true);
                        int slt = 0;
                    //---------------------------------------------------------------------
                    Application.DoEvents();
                    if (mFile.Name.Contains("_")) { slt = 1; } else { slt = 0; }


                        if ((mFile.Name.Length < 13 || mFile.Name.Length > 13) && slt == 0)
                        {



                            //  moveDirectory(mFile.DirectoryName, directoryNameSCA);
                            mFile.MoveTo(directoryNameSCA + mFile.Name);
                            listBox2.Items.Insert(sc, mFile.Name);
                            listBox2.Update();
                            sc += 1;

                        }

                        //-------------------------------------------------------------------------
                        else
                        {
                            String nmfle = "AU" + DateTime.Now.Day.ToString("00") + DateTime.Now.Hour.ToString("00") + DateTime.Now.Minute.ToString("00") + DateTime.Now.Second.ToString("00") + DateTime.Now.Millisecond.ToString("00") + "_1.pdf";


                            int milliseconds = 900;
                            Thread.Sleep(milliseconds);
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
                                    ID_TP_DOCUMENTAZIONE = "";
                                    break;
                            }
                            String idanag = readmysql(ID_CREDITO, "ID_DEBITORE");
                            String DTA_INS = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString("00") + "-" + DateTime.Now.Day.ToString("00") + " " + DateTime.Now.ToLongTimeString();
                            String PERCORSO = "https://normarec.mcelocam.com/NormaRec/Archive/";// " / opt/tomcat/webapps//NormaRec/Archive";
                            String NOMEFILE = DateTime.Now.Year.ToString() + "/" + DateTime.Now.Month.ToString("00") + "/" + nmfle;// files1[i];
                                                                                                                                   //  String NOMEFILE = "/" + "2000" + "/" + DateTime.Now.Month.ToString("00") + "/" + nmfle;// files1[i];
                            String inserito = " >NON  Inserito DB < ";
                            //------------------------------------------------------------------------------------------------------------
                            if (readmysqlver(idanag, ID_CREDITO, ID_TP_DOCUMENTAZIONE, "ID") != "" && slt==0)
                            { mFile.MoveTo(directoryNameDB + mFile.Name); }
                            else
                            {
                                if (idanag != "")
                                {

                                       insmysql(idanag, ID_CREDITO, ID_TP_DOCUMENTAZIONE, DTA_INS, PERCORSO, NOMEFILE);
                                    inserito = " >Inserito DB < ";



                                    //------------------------------------------------------------------------------------------------------------

                                       mFile.CopyTo(dirInfoR + "\\" + nmfle, true);

                                    // // FileUploadSFTP(directoryName,files1[i]);

                                       FileUploadSFTP(directoryNameR, nmfle);



                                    String nome = "";
                                    if (idanag == "")
                                    { nome = "per " + ID_CREDITO + " ID_DEBITORE non TROVATO"; }
                                    else { nome = readmysql_anag(idanag, "COGNOME") + " " + readmysql_anag(idanag, "NOME"); }
                                    WriteLog(DateTime.Now.ToShortTimeString() + "   File " + files1[i].ToString() + " Credito :" + ID_CREDITO + " ID Debitore :" + idanag + "--" + nome + inserito);

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
                                    mFile.MoveTo(directoryNameSCA + mFile.Name);
                                    listBox2.Items.Insert(sc, mFile.Name);
                                    listBox2.Update();
                                    sc += 1;
                                }
                                //  MoveDirectory(mFile.DirectoryName, dlg1.SelectedPath + @"\INSERITI",true);

                            }

                        }
                    }



                    WriteLog(DateTime.Now.ToShortTimeString() + "   Uplodati N°" + numupl.ToString());

                    string logFilePath = System.Configuration.ConfigurationManager.AppSettings["logpat"];
                    logFilePath = logFilePath + "Log-" + System.DateTime.Today.ToString("MM-dd-yyyy") + "." + "txt";
                    String target = System.Configuration.ConfigurationManager.AppSettings["HAN_OUT"] + "Flusso_Mensile_" + DateTime.Now.ToShortTimeString().Replace(":", "_") + ".txt";


                    sendmail(logFilePath);
                    linkLabel1.Visible = true;
                    //  linkLabel1.Text = "INSERITI n°" + ins.ToString();
                    //LinkLabel.Link link = new LinkLabel.Link();
                    //link.LinkData = @"\\10.0.0.11\SCANNER\RISULTATI\INSERITI\";
                    // linkLabel1.Links.Add(link);
                    MessageBox.Show("DOCUMENATLE", "TERMINATO UPLOAD");


                }
                catch (System.Exception excpt)
                {
                    Console.WriteLine(excpt.Message);
                }


         

        }
        private void insmysql(String idanag, String ID_CREDITO, String ID_TP_DOCUMENTAZIONE, String DTA_INS, String PERCORSO, String NOMEFILE)
        {


            string connstr = System.Configuration.ConfigurationManager.ConnectionStrings["nrec"].ToString();
            // "server=10.0.0.22;user id=root;password=R0ma.2018;persistsecurityinfo=False;database=NormaRecTest;Allow Zero Datetime=True;Convert Zero Datetime=True;";// Dts.Connections[0].ConnectionString;
            MySqlConnection sourceConnection = new MySqlConnection(connstr);
            String cmdins = "INSERT INTO DOCUMENTAZIONE(ID_ANAGRAFICA, ID_CREDITO, ID_TP_DOCUMENTAZIONE, DATA_INS, PERCORSO, NOMEFILE) VALUES(";
            cmdins += "'" + idanag + "'," + ID_CREDITO + "," + ID_TP_DOCUMENTAZIONE + ",'" + DTA_INS + "','" + PERCORSO + "','" + NOMEFILE + "')";

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
                numupl += 1;
                // http://stackoverflow.com/questions/18757097/writing-data-into-csv-file/39535867#39535867
                //  byte[] csvFile = DownloadCSV(); // Function returns byte[] csv file
                String uploadFile = dir + "\\" + fle;
                //    @"E:\SERVIZIO\NORMAREC\SCANNER\OUTPUT\DOC_SCANSIONATA_2019_3_29\" + fle;
                using (var client = new SftpClient(host, port, username, password))
                {
                    client.Connect();
                    if (client.IsConnected)
                    {

                        // Debug.WriteLine("I'm connected to the client");

                        using (FileStream fileStream = new FileStream(uploadFile, FileMode.Open))

                        {
                            if (client.Exists("/opt/tomcat/webapps//NormaRec/Archive/" + DateTime.Now.Year.ToString() + "/" + DateTime.Now.Month.ToString("00") + "/"))
                            {
                              //  SftpFileAttributes attrs = client.GetAttributes("/opt/tomcat/webapps//NormaRec/Archive/" + DateTime.Now.Year.ToString() + "/" + DateTime.Now.Month.ToString("00") + "/");
                                    SftpFileAttributes attrs = client.GetAttributes("/opt/tomcat/webapps//NormaRec/Archive/" + "2000" + "/" + DateTime.Now.Month.ToString("00") + "/");

                                if (!attrs.IsDirectory)
                                {
                                    throw new Exception("not directory");
                                }
                            }
                            else
                            {
                               // client.CreateDirectory("/opt/tomcat/webapps//NormaRec/Archive/" + DateTime.Now.Year.ToString() + "/" + DateTime.Now.Month.ToString("00") + "/");
                                client.CreateDirectory("/opt/tomcat/webapps//NormaRec/Archive/" + "2000" + "/" + DateTime.Now.Month.ToString("00") + "/");

                            }
                            //client.BufferSize = (uint)ms.Length; // bypass Payload error large files
                            //client.UploadFile(ms, GetListFileName());
                            client.BufferSize = 4 * 1024; // bypass Payload error large files
                          //  client.UploadFile(fileStream, "/opt/tomcat/webapps//NormaRec/Archive/" + DateTime.Now.Year.ToString() + "/" + DateTime.Now.Month.ToString("00") + "/" + Path.GetFileName(uploadFile));
                              client.UploadFile(fileStream, "/opt/tomcat/webapps//NormaRec/Archive/" + "2000" + "/" + DateTime.Now.Month.ToString("00") + "/" + Path.GetFileName(uploadFile));

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
        //-----------------------------------------------------------------------------------------------
        public static void WriteLog(string strLog)
        {
            StreamWriter log;
            FileStream fileStream = null;
            DirectoryInfo logDirInfo = null;
            FileInfo logFileInfo;

            string logFilePath = System.Configuration.ConfigurationManager.AppSettings["logpat"];
            logFilePath = logFilePath + "Log-" + System.DateTime.Today.ToString("MM-dd-yyyy") + "." + "txt";
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

        private void Form1_Load(object sender, EventArgs e)
        {
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.RunWorkerAsync();
            DateTime lastHigh = new DateTime(1900, 1, 1);
            String highDir = "";
            foreach (string subdir in Directory.GetDirectories(System.Configuration.ConfigurationManager.AppSettings["risulpat"]))
            {
                DirectoryInfo fi1 = new DirectoryInfo(subdir);
                DateTime created = fi1.LastWriteTime;

                if (created > lastHigh)
                {
                    highDir = subdir;
                    lastHigh = created;
                }
            }

            ticketBox.Items.Clear();

            DirectoryInfo dinfo = new DirectoryInfo(highDir + @"\INSERITI\");
            if (dinfo.Exists == true)
            {
                FileInfo[] smFiles = dinfo.GetFiles("*.*").OrderByDescending(p => p.LastAccessTime).ToArray();
                int t = 0;
                foreach (FileInfo fi in smFiles)
                {

                    ticketBox.Items.Add(Path.GetFileNameWithoutExtension(fi.Name) + "--" + fi.LastAccessTime);
                    t += 1;
                    if (t > 10) { break; }
                }

                label3.Text = "Ultimi File UPLODATI il " + highDir.Substring(Math.Max(0, highDir.Length - 10));
            }
        }

        private void sendmail(String fle)
        {
            try
            {

                String mailind = System.Configuration.ConfigurationManager.AppSettings["mail"];

                MailMessage mail = new MailMessage("noreply@mediocreditoeuropeo.com", mailind);
                SmtpClient client = new SmtpClient("SRV-EXCH01");
                client.Port = 25;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = true;
                client.Host = "SRV-EXCH01";
                mail.Subject = "LOG NormaRec UPLOAD";
                mail.Body = fle;
                //    client.Port = 25;
                client.Credentials = new System.Net.NetworkCredential("MCE\noreply", "Scanner456");
                //client.EnableSsl = true;

                System.Net.Mail.Attachment attachment;
                attachment = new System.Net.Mail.Attachment(fle);

                mail.Attachments.Add(attachment);
                client.Send(mail);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException.Message, "");
            }
        }
        private void moveDirectory(string fuente, string destino)
        {
            if (!System.IO.Directory.Exists(destino))
            {
                System.IO.Directory.CreateDirectory(destino);
            }
            String[] files = Directory.GetFiles(fuente);
            String[] directories = Directory.GetDirectories(fuente);
            foreach (string s in files)
            {
                System.IO.File.Copy(s, Path.Combine(destino, Path.GetFileName(s)), true);
            }
            foreach (string d in directories)
            {

                moveDirectory(Path.Combine(fuente, Path.GetFileName(d)), Path.Combine(destino, Path.GetFileName(d)));
            }
            Directory.Delete(fuente, true);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //   System.Diagnostics.Process.Start("net.exe", @"use K: ""\\10.0.0.11\SCANNER\RISULTATI\"" /user:MCE\admferrettiMDM-777888").WaitForExit();
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

        private void Form2_Load(object sender, EventArgs e)
        {
           

        }

        private void button4_Click(object sender, EventArgs e)
        {
            pg1.Maximum = 100;
            pg1.Step = 1;
            pg1.Value = 0;
            backgroundWorker1.RunWorkerAsync();
        }
       

        //public static void MoveDirectory(string source, string target)
        //{
        //    var sourcePath = source.TrimEnd('\\', ' ');
        //    var targetPath = target.TrimEnd('\\', ' ');
        //    var files = Directory.EnumerateFiles(sourcePath, "*", SearchOption.AllDirectories)
        //                         .GroupBy(s => Path.GetDirectoryName(s));
        //    foreach (var folder in files)
        //    {
        //        var targetFolder = folder.Key.Replace(sourcePath, targetPath);
        //        Directory.CreateDirectory(targetFolder);
        //        foreach (var file in folder)
        //        {
        //            var targetFile = Path.Combine(targetFolder, Path.GetFileName(file));
        //            if (File.Exists(targetFile)) File.Delete(targetFile);
        //            File.Move(file, targetFile);
        //        }
        //    }
        //    Directory.Delete(source, true);
        //}


        //public bool MoveDirectory(string sourceDirName, string destDirName, bool overwrite)
        //{
        //    if (overwrite && Directory.Exists(destDirName))
        //    {
        //        var needRestore = false;

        //        var tmpDir = Path.Combine(@"C:\temp", Path.GetRandomFileName());
        //        try
        //        {
        //            Directory.Move(destDirName, tmpDir);
        //            needRestore = true; // only if fails
        //            Directory.Move(sourceDirName, destDirName);
        //            return true;
        //        }
        //        catch (Exception)
        //                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     {
        //            if (needRestore)
        //            {
        //                Directory.Move(tmpDir, destDirName);
        //            }
        //        }
        //        finally
        //        {
        //            Directory.Delete(tmpDir, true);
        //        }
        //    }
        //    else
        //    {
        //        Directory.Move(sourceDirName, destDirName); // Can throw an Exception
        //        return true;
        //    }
        //    return false;
        //}
    }
}
