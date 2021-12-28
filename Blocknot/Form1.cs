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
            Init(textBox1.Text);
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
            SaveUnsavedFile();
            textBox1.Text = "";
            filename = "";
            isFileChanged = false;
            UpdateTextWithTitle();
        }

        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
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
            UpdateTextWithTitle();
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
            UpdateTextWithTitle();
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
            CopyText(textBox1.SelectedText);
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
            textBox1.Text = CutText(textBox1.Text, textBox1.SelectedText, textBox1.SelectionStart, textBox1.SelectionLength);
        }

        private void OnPast(object sender, EventArgs e)
        {
            textBox1.Text = PasteText(textBox1.Text, textBox1.SelectionStart, Clipboard.GetText());
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
            Count count = new Count(new CountChar(countCh), new CountWords(countWord));
            string a = count.WordsCount(textBox1.Text).ToString();
            Count_view.Text = a;
        }
        private void Click_Count_Char(object sender, EventArgs e)
        {
            Count count = new Count(new CountChar(countCh), new CountWords(countWord));
            string a = count.CharsCount(textBox1.Text).ToString();
            Count_view.Text = a;
        }

        private void Click_Find(object sender, EventArgs e)
        {
            Find find = new Find();
            if (FindText.Text.Length > 0)
            {
                list = find.Finds(textBox1.Text, FindText.Text);
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
            CountParagraphs countParagraphs = new CountParagraphs();
            int count = 0;
            for (int i = 0; i < textBox1.Lines.Length; i++)
            {
                if (countParagraphs.CountParagraph(textBox1.Lines[i]))
                    count++;
            }
            string a = count.ToString();
            Count_view.Text = a;
        }
        public void UpdateTextWithTitle()
        {
            if (Form1.filename != "")
                Text = Form1.filename + " - Блокнот";
            else Text = "Безымянный - Блокнот";
        }
        public void Init(string text)
        {
            Form1.filename = "";
            Form1.isFileChanged = false;
            UpdateTextWithTitle();
            FontSettings fs = new FontSettings();
        }
        public string CutText(string AllText, string Text, int Start, int Length)
        {
            Clipboard.SetText(Text);
            return AllText.Remove(Start, Length);
        }
        public void CopyText(string Text)
        {
            Clipboard.SetText(Text);
        }
        public string PasteText(string Text, int Start, string PasteText)
        {
            return Text.Substring(0, Start) + PasteText + Text.Substring(Start, Text.Length - Start);
        }
        CountCh countCh = new CountCh();
        CountWord countWord = new CountWord();
        Count count;

    }
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public interface Command
    {
        int Execute(string a);
    }

    public class CountChar : Command {
        CountCh countCh;

        public CountChar(CountCh countCh)
        {
            this.countCh = countCh;
        }
        public int Execute(string a)
        {
            return countCh.CountChars(a);
        }
    }
    public class CountWords : Command
    {
        CountWord countWord;
        public CountWords(CountWord countWord)
        {
            this.countWord = countWord;
        }
        public int Execute(string a)
        {
            return countWord.CountWords(a);
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
}
