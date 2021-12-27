using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Configuration;
using System.Data.SqlClient;
using FluentFTP;
using System.Net;
using System.Collections;

namespace Blocknot
{

    
        public partial class Form1 : Form
    {
        public static int fontSize = 0;
        public static System.Drawing.FontStyle fs = FontStyle.Regular;
        public static string filename;
        public static bool isFileChanged;
        public List<int> list;
        int listSelect;
        private SqlConnection sqlConnection = null;
        WorkWithDB wdb;


        public FontSettings fontSetts;

        public Form1()
        {
            InitializeComponent();
            wdb = new WorkWithDB();
            Block c = new Block();
            c.Init(textBox1.Text);
        }
        public void Save(object sender, EventArgs e)
        {
            SaveFile(filename);
        }
        public void SaveAs(object sender, EventArgs e)
        {
            SaveFile("");
        }
        public void CreateNewDocument(object sender, EventArgs e)
        {
            Block c = new Block();
            SaveUnsavedFile();
            textBox1.Text = "";
            filename = "";
            isFileChanged = false;
            c.UpdateTextWithTitle(textBox1.Text);
        }

        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            Block c = new Block();
            SaveUnsavedFile();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = File.ReadAllText(openFileDialog.FileName);

                try
                {
                    bool exist = false;
                    StreamReader sr = new StreamReader(openFileDialog.FileName);
                    textBox1.Text = sr.ReadToEnd();
                    sr.Close();
                    filename = openFileDialog.FileName;
                    if (wdb.GetFilesName().Contains(filename))
                        exist = true;
                    CreateOrUpdate(filename, exist);
                    filename = openFileDialog.FileName;
                    isFileChanged = false;
                }
                catch
                {
                    MessageBox.Show("Невозможно открыть файл!");
                }
            }
            c.UpdateTextWithTitle(textBox1.Text);
        }
        public void CreateOrUpdate(string filename, bool exist)
        {
            if (!exist)
            {
                wdb.InsertNewFile(filename);
            }
            else
            {
                wdb.UpdateNewFile(filename);
            }
        }

        public void SaveFile(string _fileName)
        {

            Block c = new Block();
            if (_fileName == "")
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    _fileName = saveFileDialog.FileName;
                    wdb.InsertNewFile(_fileName);
                }
                try
                {
                    StreamWriter sw = new StreamWriter(_fileName + ".txt");
                    sw.Write(textBox1.Text);
                    sw.Close();
                    filename = _fileName;
                    isFileChanged = false;
                }
                catch
                {
                    MessageBox.Show("Невозможно сохранить файл");
                }
            }
            c.UpdateTextWithTitle(textBox1.Text);
        }

        public void SaveUnsavedFile()
        {
            if (isFileChanged)
            {
                DialogResult result = MessageBox.Show("Сохранить изменения в файле?", "Сохранение файла", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (result == DialogResult.Yes)
                {
                    SaveFile(filename);
                }
            }
        }

        private void OnCopy(object sender, EventArgs e)
        {
            Block c = new Block();
            c.CopyText(textBox1.SelectedText);
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            if (!isFileChanged)
            {
                this.Text = this.Text.Replace('*', ' ');
                isFileChanged = true;
                this.Text = '*' + this.Text;
            }
        }
        private void OnCutClick(object sender, EventArgs e)
        {
            Block c = new Block();
            textBox1.Text = c.CutText(textBox1.Text, textBox1.SelectedText, textBox1.SelectionStart, textBox1.SelectionLength);
        }

        private void OnPast(object sender, EventArgs e)
        {
            Block c = new Block();
            textBox1.Text = c.PasteText(textBox1.Text, textBox1.SelectionStart, Clipboard.GetText());
        }

        private void OnFormClosing(object sender, FormClosedEventArgs e)
        {
            SaveUnsavedFile();
        }

        private void OnFontClick(object sender, EventArgs e)
        {
            fontSetts = new FontSettings();
            fontSetts.Show();
        }

        private void OnFocus(object sender, EventArgs e)
        {
            if (fontSetts != null)
            {
                fontSize = fontSetts.fontSize;
                fs = fontSetts.fs;
                textBox1.Font = new Font(textBox1.Font.FontFamily, fontSize, fs);
                fontSetts.Close();
            }
        }

        private void Click_Count_Words(object sender, EventArgs e)
        {
            Block c = new Block();
            Count count = new Count(new CountChar(block), new CountWords(block));
            string a = count.WordsCount(textBox1.Text).ToString();
            Count_view.Text = a;
        }
        private void Click_Count_Char(object sender, EventArgs e)
        {
            Block c = new Block();
            Count count = new Count(new CountChar(block), new CountWords(block));
            string a = count.CharsCount(textBox1.Text).ToString();
            Count_view.Text = a;
        }

        private void Click_Find(object sender, EventArgs e)
        {
            Block c = new Block();
            if (FindText.Text.Length > 0)
            {
                list = c.Find(textBox1.Text, FindText.Text);
                if (list.Count > 0)
                {
                    button2.Enabled = true;
                    button3.Enabled = true;
                    textBox1.Select();
                    listSelect = 0;
                    textBox1.SelectionStart = list[listSelect];
                }
                else
                {
                    button2.Enabled = false;
                    button3.Enabled = false;
                }
            }
        }

        private void Click_Next_Find(object sender, EventArgs e)
        {
            if (list.Count > 0)
            {
                textBox1.Select();

                if (listSelect != list.Count - 1)
                {
                    listSelect++;
                    textBox1.SelectionStart = list[listSelect];
                }
                else
                {
                    listSelect = 0;
                    textBox1.SelectionStart = list[listSelect];
                }
            }
        }

        private void Click_Prev_Find(object sender, EventArgs e)
        {
            if (list.Count > 0)
            {
                textBox1.Select();
                if (listSelect != 0)
                {
                    listSelect--;
                    textBox1.SelectionStart = list[listSelect];
                }
                else
                {
                    listSelect = list.Count - 1;
                    textBox1.SelectionStart = list[listSelect];
                }
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LastFileList.DataSource = wdb.GetFilesWay();
            LastFileList.ValueMember = "Way";
            LastFileList.SelectedIndex = -1;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LastFileList.SelectedIndex == -1)
            {
                return;
            }
            else
            {
                SaveUnsavedFile();
                try
                {
                    StreamReader sr = new StreamReader(LastFileList.SelectedValue.ToString() + ".txt");
                    textBox1.Text = sr.ReadToEnd();
                    sr.Close();
                    filename = LastFileList.SelectedValue.ToString() + ".txt";
                    wdb.UpdateNewFile(filename);
                    isFileChanged = false;
                }
                catch
                {
                    MessageBox.Show("Этот файл больше не существует");
                    wdb.DeleteFile(LastFileList.SelectedValue.ToString());
                    LastFileList.DataSource = wdb.GetFilesWay();
                    LastFileList.ValueMember = "Way";
                    LastFileList.SelectedIndex = -1;
                }
            }
        }

        private void fillByToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.filesTableAdapter.FillBy(this.blocknotDBDataSet.Files);
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

        }

        private void fillBy1ToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.filesTableAdapter.FillBy1(this.blocknotDBDataSet.Files);
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

        }

        private void fillByToolStripButton_Click_1(object sender, EventArgs e)
        {
            try
            {
                this.filesTableAdapter.FillBy(this.blocknotDBDataSet.Files);
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

        }

        private void LoadFromFTP_Click(object sender, EventArgs e)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://192.168.0.15/" + FTPString.Text + ".txt");
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            textBox1.Text = reader.ReadToEnd();
        }

        private void SaveFTP_Click(object sender, EventArgs e)
        {
            try
            {
                FtpWebRequest delRequest = (FtpWebRequest)WebRequest.Create("ftp://192.168.0.15/" + FTPString.Text + ".txt");
                delRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                FtpWebResponse response1 = (FtpWebResponse)delRequest.GetResponse();
                response1.Close();
            }
            catch{}
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://192.168.0.15/" + FTPString.Text + ".txt");
            request.Method = WebRequestMethods.Ftp.AppendFile;
            request.ContentLength = textBox1.Text.Length;
            Stream requestStream = request.GetRequestStream();
            byte[] s = System.Text.Encoding.UTF8.GetBytes(textBox1.Text);
            requestStream.Write(s, 0, textBox1.Text.Length);
            requestStream.Close();
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            response.Close();
        }

        private void TextAlign_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TextAlign.SelectedIndex == 0)
            {
                textBox1.TextAlign = HorizontalAlignment.Left;
            }
            else if (TextAlign.SelectedIndex == 1)
            {
                textBox1.TextAlign = HorizontalAlignment.Center;
            }
            else if (TextAlign.SelectedIndex == 2)
            {
                textBox1.TextAlign = HorizontalAlignment.Right;
            }
        }

        private void textBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                textBox1.SelectedText = "   ";
                e.IsInputKey = true;

            }
        }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\t')
                e.Handled = true;
        }

        private void Count_Paragraphs_Click(object sender, EventArgs e)
        {
            Block c = new Block();
            int count = 0;
            for (int i = 0; i < textBox1.Lines.Length; i++)
            {
                if (c.CountParagraphs(textBox1.Lines[i]))
                    count++;
            }
            string a = count.ToString();
            Count_view.Text = a;
        }

        Block block = new Block();

        Count count;

    }
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public interface Command
    {
        int Execute(string a);
    }

    public class CountChar : Command {
        Block block;

        public CountChar(Block block)
        {
            this.block = block;
        }
        public int Execute(string a)
        {
            return block.CountChar(a);
        }
    }
    public class CountWords : Command
    {
        Block block;

        public CountWords(Block block)
        {
            this.block = block;
        }
        public int Execute(string a)
        {
            return block.CountWords(a);
        }
    }

    public class Count
    {
        Command words;
        Command chars;

        public Count(Command chars, Command words)
        {
            this.chars = chars;
            this.words = words;
        }

        public int CharsCount(string a)
        {
            return chars.Execute(a);
        }
        public int WordsCount(string a)
        {
            return words.Execute(a);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    class WorkWithDB
    {
        string connectionString = "Data Source=localhost;Database=DB_FILES_TAHTAROV;Trusted_Connection=True;";
          

        public List<string> GetFilesName()
        {
            List<string> resultName = new List<string>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT Way FROM [File]";
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    resultName.Add(String.Format("{0}", reader[0]));
                }
                connection.Close();
            }
            return resultName;
        }
        public List<string> GetFilesWay()
        {
            List<string> resultWay = new List<string>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT TOP 3 Way FROM [File] ORDER BY EditDate DESC";

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    resultWay.Add(String.Format("{0}", reader[0]));
                }
                connection.Close();
            }
            return resultWay;
        }

        public void InsertNewFile(string _fileName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT INTO [File] (Way, Name, EditDate) VALUES (@way, @name, @date)";
                SqlParameter wayParam = new SqlParameter("@way", _fileName.Substring(0, _fileName.Length - 4));
                cmd.Parameters.Add(wayParam);
                SqlParameter nameParam = new SqlParameter("@name", _fileName.Substring(0, _fileName.Length - 4).Split('\\').Last());
                cmd.Parameters.Add(nameParam);
                SqlParameter dateParam = new SqlParameter("@date", DateTime.Now.ToString());
                cmd.Parameters.Add(dateParam);
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        public int UpdateNewFile(string filename)
        {
            int Changed = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "UPDATE [File] SET EditDate = @date WHERE Way = @name";
                SqlParameter nameParam = new SqlParameter("@name", filename.ToString().Substring(0, filename.Length - 4));
                cmd.Parameters.Add(nameParam);
                SqlParameter date = new SqlParameter("@date", DateTime.Now);
                cmd.Parameters.Add(date);
                Changed = cmd.ExecuteNonQuery();
                connection.Close();
                return Changed;
            }
        }
        public void DeleteFile(string filename)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "DELETE FROM [File] WHERE Way = @way";
                SqlParameter wayParam = new SqlParameter("@way", filename);
                cmd.Parameters.Add(wayParam);
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }
    }

    public class Block
    {
        public void Init(string text)
        {
            Block c = new Block();
            Form1.filename = "";
            Form1.isFileChanged = false;
            c.UpdateTextWithTitle(text);
            FontSettings fs = new FontSettings();
        }
        public string CutText(string AllText, string Text, int Start, int Length)
        {
            Clipboard.SetText(Text);
            return AllText.Remove(Start, Length);
        }
        public int CountChar(string text)
        {
            return text.Length;
        }
        public bool CountParagraphs(string text)
        {
            if(text.IndexOf("   ") == 0)
                return true; 
            return false;
        }
        public List<int> Find(string text, string findText)
        {
            var list = new List<int>();
            int lastFind = 0;
            bool next = true;
            while (next)
            {
                if (text.IndexOf(findText, lastFind) >= 0)
                {
                    list.Add(text.IndexOf(findText, lastFind));
                    lastFind = text.IndexOf(findText, lastFind) + findText.Length;
                }
                else
                    next = false;
            }
            return list;
        }

        public void UpdateTextWithTitle(string Text)
        {
            if (Form1.filename != "")
                Text = Form1.filename + " - Блокнот";
            else Text = "Безымянный - Блокнот";
        }
        public void CopyText(string Text)
        {
            Clipboard.SetText(Text);
        }

        public string PasteText(string Text, int Start, string PasteText)
        {
            return Text.Substring(0, Start) + PasteText + Text.Substring(Start, Text.Length - Start);
        }
        enum WordCountState
        {
            Init,
            Word,
            WhiteSpace
        }
        public int CountWords(string originString)
        {
            int wordCounter = 0;
            WordCountState state = WordCountState.Init;

            foreach (Char c in originString)
            {
                // In case of whitespace
                if (Char.IsWhiteSpace(c))
                {
                    switch (state)
                    {
                        case WordCountState.Init:
                        case WordCountState.Word:
                            state = WordCountState.WhiteSpace;
                            break;

                        case WordCountState.WhiteSpace:
                            // ignore whitespace chars
                            break;

                        default:
                            throw new InvalidProgramException();
                    }
                    // In case of non-whitespace char
                }
                else
                {
                    switch (state)
                    {
                        case WordCountState.Init:
                        case WordCountState.WhiteSpace:
                            // Incerement out counter if we met non-whitespace
                            // char after whitespace (one or more)
                            wordCounter++;
                            state = WordCountState.Word;
                            break;

                        case WordCountState.Word:
                            // ignore all symbols in word
                            break;

                        default:
                            throw new InvalidProgramException();
                    }
                }
            }

            return wordCounter;
        }
    }
}
