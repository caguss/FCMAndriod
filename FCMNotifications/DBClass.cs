using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MySqlConnector;

namespace FCMNotifications
{
    class DBClass
    {
        Dictionary<string, int> companydata = new Dictionary<string, int>();
        Dictionary<string, int> serverdata = new Dictionary<string, int>();
        MySqlConnection conn = null;


        public int[] LoginCheck_R10(int[] logindata,string v_uid, string v_upw, MySqlConnection conn, string token)
        {
            this.conn = conn;
            int[] userinfo = new int[3] { 99,99,99};
            try
            {
                MySqlCommand cmd = new MySqlCommand();

                cmd.CommandText = "LoginCheck_R10";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;

                cmd.Parameters.Add(new MySqlParameter("@v_company_code", MySqlDbType.Int32));
                cmd.Parameters.Add(new MySqlParameter("@v_server_code", MySqlDbType.Int32));
                cmd.Parameters.Add(new MySqlParameter("@v_uid", MySqlDbType.VarChar, 50));
                cmd.Parameters.Add(new MySqlParameter("@v_upw", MySqlDbType.VarChar, 50));
                cmd.Parameters.Add(new MySqlParameter("@v_tok", MySqlDbType.VarChar, 152));

                cmd.Parameters["@v_company_code"].Value = logindata[0];
                cmd.Parameters["@v_server_code"].Value = logindata[1];
                cmd.Parameters["@v_uid"].Value = v_uid;
                cmd.Parameters["@v_upw"].Value = v_upw;
                cmd.Parameters["@v_tok"].Value = token;

                MySqlDataReader reader;

                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        userinfo[0] = int.Parse( reader["company_code"].ToString());
                        userinfo[1] = int.Parse( reader["server_code"].ToString());
                        userinfo[2] = int.Parse(reader["user_code"].ToString());
                    }

                    return userinfo;

                }
                else
                {
                    return userinfo;
                }


            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Dictionary<string, int> FindCompany_R10(MySqlConnection conn)
        {
            DataTable dt = new DataTable();
            MySqlCommand cmd = new MySqlCommand();

            cmd.CommandText = "FindCompany_R10";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;


            MySqlDataReader reader;



            try
            {
                reader = cmd.ExecuteReader();
                dt = this.GetTable(reader);
                ConvertDataTableToDictionary(dt, "company_name", "company_code", companydata);
                
                return companydata;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool TaskFinish_I10(int[] userinfo, string contact, string status)
        {
            if (contact =="이력이 없습니다."||contact == "불러오기 에러")
            {
                return false;
            }
            try
            { 
                MySqlCommand cmd = new MySqlCommand();

                //contact 분할
                //contact = "시간 : 20/08/06 13:58:06\n제목 : Title\n내용 : TimerSending"

                string[] taskdata = new string[3];
                contact = contact.Replace("\n", "%");
                taskdata = contact.Split('%');



                cmd.CommandText = "TaskFinish_I10";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;

                cmd.Parameters.Add(new MySqlParameter("@v_company_code", MySqlDbType.Int32));
                cmd.Parameters.Add(new MySqlParameter("@v_server_code", MySqlDbType.Int32));
                cmd.Parameters.Add(new MySqlParameter("@v_user_code", MySqlDbType.Int32));
                cmd.Parameters.Add(new MySqlParameter("@v_task_title", MySqlDbType.VarChar, 50));
                cmd.Parameters.Add(new MySqlParameter("@v_issue_date", MySqlDbType.DateTime));
                cmd.Parameters.Add(new MySqlParameter("@v_task_content", MySqlDbType.VarChar, 200));
                cmd.Parameters.Add(new MySqlParameter("@v_process_status", MySqlDbType.VarChar, 10));

                cmd.Parameters["@v_company_code"].Value = userinfo[0];
                cmd.Parameters["@v_server_code"].Value = userinfo[1];
                cmd.Parameters["@v_user_code"].Value = userinfo[2];
                cmd.Parameters["@v_task_title"].Value = taskdata[0];
                cmd.Parameters["@v_issue_date"].Value = taskdata[1];
                cmd.Parameters["@v_task_content"].Value = taskdata[2];
                cmd.Parameters["@v_process_status"].Value = status;

                
                if (cmd.ExecuteNonQuery() == 1)
                {
                   

                    return true;

                }
                else
                {
                    return false;
                }


            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public string[] TaskList_R10(int[] userinfo)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                DataTable dt = new DataTable();
                List<string> tasklist = new List<string>();
                cmd.CommandText = "TaskList_R10";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;

                cmd.Parameters.Add(new MySqlParameter("@v_company_code", MySqlDbType.Int32));
                cmd.Parameters.Add(new MySqlParameter("@v_server_code", MySqlDbType.Int32));

                cmd.Parameters["@v_company_code"].Value = userinfo[0];
                cmd.Parameters["@v_server_code"].Value = userinfo[1];

                MySqlDataReader reader;
                
                    reader = cmd.ExecuteReader();
                    dt = this.GetTable(reader);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //tasklist = "시간 : 20/08/06 13:58:06\n제목 : Title\n내용 : TimerSending%process_status"

                    tasklist.Add("시간 : " +dt.Rows[i]["issue_time"].ToString() + "\n제목 : "+ dt.Rows[i]["task_title"].ToString() + "\n내용 : "+ dt.Rows[i]["task_content"].ToString() + "%"+ dt.Rows[i]["process_status"].ToString());
                }
                string[] taskarray= tasklist.ToArray();
                return taskarray;

                

            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public System.Data.DataTable GetTable(MySqlDataReader reader)
        {
            System.Data.DataTable table = reader.GetSchemaTable();
            System.Data.DataTable dt = new System.Data.DataTable();
            System.Data.DataColumn dc;
            System.Data.DataRow row;
            System.Collections.ArrayList aList = new System.Collections.ArrayList();

            for (int i = 0; i < table.Rows.Count; i++)
            {
                dc = new System.Data.DataColumn();

                if (!dt.Columns.Contains(table.Rows[i]["ColumnName"].ToString()))
                {
                    dc.ColumnName = table.Rows[i]["ColumnName"].ToString();
                    dc.Unique = Convert.ToBoolean(table.Rows[i]["IsUnique"]);
                    dc.AllowDBNull = Convert.ToBoolean(table.Rows[i]["AllowDBNull"]);
                    dc.ReadOnly = Convert.ToBoolean(table.Rows[i]["IsReadOnly"]);
                    aList.Add(dc.ColumnName);
                    dt.Columns.Add(dc);
                }
            }

            while (reader.Read())
            {
                row = dt.NewRow();
                for (int i = 0; i < aList.Count; i++)
                {
                    row[((string)aList[i])] = reader[(string)aList[i]];
                }
                dt.Rows.Add(row);
            }
            return dt;
        }


        public void ConvertDataTableToDictionary(DataTable dt, string column_name, string column_code, Dictionary<string, int> whichdata)
        {
            whichdata.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                whichdata.Add(dt.Rows[i][column_name].ToString(), int.Parse(dt.Rows[i][column_code].ToString()));
            }
        }

        public Dictionary<string,int> FindServer_R10(MySqlConnection conn, int company_code)
        {
            DataTable dt = new DataTable();
            MySqlCommand cmd = new MySqlCommand();

            cmd.CommandText = "FindServer_R10";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;

            MySqlParameter v_company_code = new MySqlParameter("@v_company_code", MySqlDbType.Int32);
            v_company_code.Value = company_code;
            cmd.Parameters.Add(v_company_code);


            MySqlDataReader reader;
            try
            {
                reader = cmd.ExecuteReader();
                dt = this.GetTable(reader);
                ConvertDataTableToDictionary(dt, "server_name", "server_code", serverdata);
                return serverdata;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public int[] LoginCheck(string company_name, string server_name)
        {
            int companycode = 0;
            int servercode = 0;
            bool isnull = companydata.TryGetValue(company_name, out companycode);
            isnull = serverdata.TryGetValue(server_name, out servercode);


            int[] senddata = new int[2];
            senddata[0] = companycode;
            senddata[1] = servercode;

            return senddata;
        }
    }
}