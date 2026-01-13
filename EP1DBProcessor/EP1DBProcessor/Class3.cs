using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;

namespace Techlabs
{


    public class ValidateVersion : CodeActivity
    {

        //Code Activity Start
        //Variables
        [Category("Server")]
        [RequiredArgument]//Required
        public InArgument<string> ServerName { get; set; }
        public enum authType
        {
            Windows,
            SQLServer
        }
        [Category("Server")]
        [RequiredArgument] //Required
        public authType ServerAuthType { get; set; }

        [Category("Server")]
        public InArgument<string> UserName { get; set; }

        [Category("Server")]
        public InArgument<string> Password { get; set; }

        [Category("VA Bot")]
        [RequiredArgument]//Required
        public InArgument<string> BotName { get; set; }

        [Category("VA Bot")]
        [RequiredArgument]//Required
        public InArgument<string> BotVersion { get; set; }


        [Category("Output")]
        [RequiredArgument]//Required
        public OutArgument<bool> ResultText { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            //local Variables
            var botName = BotName.Get(context);
            var localBotVersion = BotVersion.Get(context);
            var serverName = ServerName.Get(context);
            var userName = UserName.Get(context);
            var password = Password.Get(context);
            string cs;
            string database;
            string validateresult;

            // local variables for logs
            string logsessionID;
            string logsysname;
            string logactiondone;
            string logresult;
            string logremarks;
            string logexceptions;
            DateTime dateToday = DateTime.Now;
            string user = System.Environment.UserName;

            #region DBconnection
            database = "TechLabs";
            if (ServerAuthType == authType.Windows)
            {
                //Windows authentication
                cs = @"Data Source=" + serverName + ";Initial Catalog=" + database + ";Integrated Security=True";
            }

            else
            {
                //SQL Server authentication
                cs = @"Data Source=" + serverName + ";Initial Catalog=" + database + ";User ID=" + userName + ";Password=" + password;
            }
            #endregion


            using (SqlConnection conn = new SqlConnection(cs))

            {
                try
                {
                    //First try-catch wants to validate if database successfully connected.
                    conn.Open();

                    #region declareVariablesForLogs
                    //Insert to System logs - Declare Variables.
                    logsessionID = userName + dateToday.ToString().Replace("/", "");
                    logsessionID = logsessionID.Replace(" ", "");
                    logsessionID = logsessionID.Replace(":", "");
                    var SessionIDtemp = logsessionID;
                    logsysname = userName;
                    logactiondone = "GETTAT - Validate if current solution is updated.";
                    #endregion
                    try
                    {
                       
                        //Execute stored procedure - Validate if bot name is found and bot version is updated
                        SqlCommand cmdvalidateIfUpdatedVersionOut = new SqlCommand("dbo.ValidateIfUpdatedVersion", conn);
                        cmdvalidateIfUpdatedVersionOut.CommandType = CommandType.StoredProcedure;
                        cmdvalidateIfUpdatedVersionOut.Parameters.AddWithValue("@botname", botName);
                        cmdvalidateIfUpdatedVersionOut.Parameters.AddWithValue("@botversion", localBotVersion);
                        SqlDataReader rdrvalidateIfUpdatedVersionOut = cmdvalidateIfUpdatedVersionOut.ExecuteReader();
                        rdrvalidateIfUpdatedVersionOut.Read();
                        validateresult = rdrvalidateIfUpdatedVersionOut.GetValue(0).ToString();


                        if (validateresult == "1")
                        {
                            ResultText.Set(context, true);
                            rdrvalidateIfUpdatedVersionOut.Close();

                            #region InserttoLogs
                            //Declare varables for system logs.
                            logresult = "SUCCESSFUL";
                            logremarks = botName + " - bot is up to date.";
                            logexceptions = "";


                            //Insert to System logs.
                            SqlCommand cmdinserttoSystemLogsOut = new SqlCommand("dbo.InsertToSystemLogs", conn);
                            cmdinserttoSystemLogsOut.CommandType = CommandType.StoredProcedure;
                            cmdinserttoSystemLogsOut.Parameters.AddWithValue("@sessionID", SqlDbType.VarChar).Value = SessionIDtemp;
                            cmdinserttoSystemLogsOut.Parameters.AddWithValue("@sysName", SqlDbType.Char).Value = logsysname;
                            cmdinserttoSystemLogsOut.Parameters.AddWithValue("@action", SqlDbType.VarChar).Value = logactiondone;
                            cmdinserttoSystemLogsOut.Parameters.AddWithValue("@result", SqlDbType.VarChar).Value = logresult;
                            cmdinserttoSystemLogsOut.Parameters.AddWithValue("@remarks", SqlDbType.VarChar).Value = logremarks;
                            cmdinserttoSystemLogsOut.Parameters.AddWithValue("@exceptions", SqlDbType.VarChar).Value = logexceptions;
                            cmdinserttoSystemLogsOut.Parameters.AddWithValue("@date", SqlDbType.DateTime).Value = dateToday;
                            cmdinserttoSystemLogsOut.Parameters.AddWithValue("@user", SqlDbType.VarChar).Value = user;
                            cmdinserttoSystemLogsOut.ExecuteNonQuery();
                            #endregion
                        }
                        else
                        {
                            ResultText.Set(context, false);
                            rdrvalidateIfUpdatedVersionOut.Close();

                            #region InserttoLogs
                            //Declare varables for system logs.
                            logresult = "UNSUCCESSFUL";
                            logremarks = botName;
                            logexceptions = "Current version is not updated.";


                            //Insert to System logs.
                            SqlCommand cmdinserttoSystemLogsOut = new SqlCommand("dbo.InsertToSystemLogs", conn);
                            cmdinserttoSystemLogsOut.CommandType = CommandType.StoredProcedure;
                            cmdinserttoSystemLogsOut.Parameters.AddWithValue("@sessionID", SqlDbType.VarChar).Value = SessionIDtemp;
                            cmdinserttoSystemLogsOut.Parameters.AddWithValue("@sysName", SqlDbType.Char).Value = logsysname;
                            cmdinserttoSystemLogsOut.Parameters.AddWithValue("@action", SqlDbType.VarChar).Value = logactiondone;
                            cmdinserttoSystemLogsOut.Parameters.AddWithValue("@result", SqlDbType.VarChar).Value = logresult;
                            cmdinserttoSystemLogsOut.Parameters.AddWithValue("@remarks", SqlDbType.VarChar).Value = logremarks;
                            cmdinserttoSystemLogsOut.Parameters.AddWithValue("@exceptions", SqlDbType.VarChar).Value = logexceptions;
                            cmdinserttoSystemLogsOut.Parameters.AddWithValue("@date", SqlDbType.DateTime).Value = dateToday;
                            cmdinserttoSystemLogsOut.Parameters.AddWithValue("@user", SqlDbType.VarChar).Value = user;
                            cmdinserttoSystemLogsOut.ExecuteNonQuery();
                            #endregion
                        }


                    }
                    catch (Exception e)
                    {
                        ResultText.Set(context, false);

                        #region InserttoLogs
                        //Declare varables for system logs.
                        logresult = "UNSUCCESSFUL";
                        logremarks = botName;
                        logexceptions = "Stored Procedure Error - " + e.ToString();


                        //Insert to System logs.
                        SqlCommand cmdinserttoSystemLogsOut = new SqlCommand("dbo.InsertToSystemLogs", conn);
                        cmdinserttoSystemLogsOut.CommandType = CommandType.StoredProcedure;
                        cmdinserttoSystemLogsOut.Parameters.AddWithValue("@sessionID", SqlDbType.VarChar).Value = SessionIDtemp;
                        cmdinserttoSystemLogsOut.Parameters.AddWithValue("@sysName", SqlDbType.Char).Value = logsysname;
                        cmdinserttoSystemLogsOut.Parameters.AddWithValue("@action", SqlDbType.VarChar).Value = logactiondone;
                        cmdinserttoSystemLogsOut.Parameters.AddWithValue("@result", SqlDbType.VarChar).Value = logresult;
                        cmdinserttoSystemLogsOut.Parameters.AddWithValue("@remarks", SqlDbType.VarChar).Value = logremarks;
                        cmdinserttoSystemLogsOut.Parameters.AddWithValue("@exceptions", SqlDbType.VarChar).Value = logexceptions;
                        cmdinserttoSystemLogsOut.Parameters.AddWithValue("@date", SqlDbType.DateTime).Value = dateToday;
                        cmdinserttoSystemLogsOut.Parameters.AddWithValue("@user", SqlDbType.VarChar).Value = user;
                        cmdinserttoSystemLogsOut.ExecuteNonQuery();
                        #endregion

                    }
                    conn.Close();

                }
                catch (Exception)
                {
                    //Since it's a connection error, Uipath's created activity will throw "false" but will not log the results in the system logs
                    ResultText.Set(context, false);
                }
             
            }

        }
    }
}































                







            