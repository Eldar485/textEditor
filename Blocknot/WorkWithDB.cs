using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Threading.Tasks;


namespace Blocknot
{
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
}
