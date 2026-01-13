using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;

//Outout activity WIP
namespace Techlabs
{
    static class Global
    {
        //mai added GT4EP1TaxEntry,GT4EP1Logentry
        public static string errorDesc;
        public static string fetchID;
        public static Boolean GT2EP1FecthTaxID;
        public static Boolean GT4EP1TaxEntry;
        public static Boolean GT4EP1Logentry;
        public static Boolean GT2EP1Taxentry;
        public static Boolean GT2EP1Logentry;
        public static Boolean GT5EP1UploadForm;
        public static Boolean GT5EP1Taxentry;
        public static Boolean GT5EP1Logentry;
        public static Boolean GT6EP1Taxentry;
        public static Boolean GT6EP1Logentry;
    }
    public class Output : CodeActivity
    {
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

        [Category("Input")]
        [RequiredArgument]//Required
        public InArgument<string> FilingPeriod { get; set; }
        public enum company
        {
            EP1,
            LSH
        }
        [Category("Input")]
        [RequiredArgument]//Required
        public company Company { get; set; }
        [Category("Datatable")]
        [RequiredArgument]//Required
        public InArgument<DataTable> OutputList { get; set; }
        [Category("Datatable")]
        [RequiredArgument]//Required
        public InArgument<DataTable> LogsList { get; set; }

        [Category("Output")]
        [RequiredArgument]//Required
        public OutArgument<string> ResultText { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var dt = OutputList.Get(context);
            var dtLogs = LogsList.Get(context);
            var botName = BotName.Get(context);
            var localBotVersion = BotVersion.Get(context);
            var serverName = ServerName.Get(context);
            var userName = UserName.Get(context);
            var password = Password.Get(context);
            String useBot;
            String resultOut;
            String sessionID;
            //string serverBotVersion = "1.2.0.4"; //temporarily hard coded for testing only
            string cs;
            string database;
            DateTime dateToday = DateTime.Now;
            string user = System.Environment.UserName;
            //DataTable resultData;

            try
            {
                if (Company == company.LSH)
                {
                    #region LSH

                    #endregion
                }
                else
                {
                    #region EP1
                    database = "EP1";
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

                    using (SqlConnection conn = new SqlConnection(cs))
                    {
                        conn.Open();
                        Boolean dbopen;
                        try
                        {
                            //Database connection Open
                            dbopen = true;
                            if (botName == "DOTAXG45Submission")
                            {
                                //EP1 EFiler
                                foreach (DataRow row in dt.Rows)
                                {
                                    //fetch TaxpayerID
                                    //String selstrEP1TaxPayersOut = "SELECT TaxpayerID from dbo.EP1TaxPayers where TaxID = @taxID";
                                    SqlCommand cmdEP1TaxPayersOut = new SqlCommand("dbo.GetTaxID", conn);
                                    cmdEP1TaxPayersOut.CommandType = CommandType.StoredProcedure;
                                    cmdEP1TaxPayersOut.Parameters.Add("@taxID", SqlDbType.NVarChar).Value = row.Field<string>(3);

                                    SqlDataReader rdrEP1GetTaxIDOut = cmdEP1TaxPayersOut.ExecuteReader();
                                    string payment = row.Field<string>(5);
                                    double dPayment = Convert.ToDouble(payment);
                                    
                                    //gem added for variance
                                    string Variances = row.Field<string>(11);
                                    double Vvalue = Convert.ToDouble(Variances);
                                    if (rdrEP1GetTaxIDOut.Read())
                                    {
                                        Global.fetchID = rdrEP1GetTaxIDOut.GetValue(0).ToString();
                                        Global.GT2EP1FecthTaxID = true;
                                    }
                                    else
                                    {
                                        //rdrEP1GetTaxIDOut.Close();
                                        Global.GT2EP1FecthTaxID = false;

                                    }
                                    rdrEP1GetTaxIDOut.Close();
                                    if (Global.GT2EP1FecthTaxID == true)
                                    {
                                        //String queryEP1FormUploaderOut = "INSERT INTO dbo.EP1TaxEntriesFormDownloader (TaxpayerID,TaxpayerName,OwnerName,Properties,TaxID,Period,GetTatPayment,ConfirmationNumber,Username,Password,Frequency,Transacted) VALUES (@taxPayerID, @taxPayer, @owner, @properties, @taxID, @period, @gettatPayment, @confirmationNumber, @userName, @passWord, @frequency, @transacted)";
                                        SqlCommand cmdEP1TaxEntriesFormDownloader = new SqlCommand("dbo.InsertToEP1TaxEntriesFormDownloader", conn);
                                        cmdEP1TaxEntriesFormDownloader.CommandType = CommandType.StoredProcedure;
                                        cmdEP1TaxEntriesFormDownloader.Parameters.Add("@taxPayerID", SqlDbType.Int).Value = Global.fetchID;
                                        cmdEP1TaxEntriesFormDownloader.Parameters.Add("@taxPayer", SqlDbType.VarChar).Value = row.Field<string>(0);
                                        cmdEP1TaxEntriesFormDownloader.Parameters.Add("@owner", SqlDbType.VarChar).Value = row.Field<string>(1);
                                        cmdEP1TaxEntriesFormDownloader.Parameters.Add("@properties", SqlDbType.VarChar).Value = row.Field<string>(2);
                                        cmdEP1TaxEntriesFormDownloader.Parameters.Add("@taxID", SqlDbType.NVarChar).Value = row.Field<string>(3);
                                        cmdEP1TaxEntriesFormDownloader.Parameters.Add("@period", SqlDbType.Date).Value = row.Field<string>(4);
                                        cmdEP1TaxEntriesFormDownloader.Parameters.Add("@gettatPayment", SqlDbType.Decimal).Value = dPayment;
                                        cmdEP1TaxEntriesFormDownloader.Parameters.Add("@confirmationNumber", SqlDbType.VarChar).Value = row.Field<string>(6);
                                        cmdEP1TaxEntriesFormDownloader.Parameters.Add("@userName", SqlDbType.VarChar).Value = row.Field<string>(7);
                                        cmdEP1TaxEntriesFormDownloader.Parameters.Add("@passWord", SqlDbType.VarChar).Value = row.Field<string>(8);
                                        cmdEP1TaxEntriesFormDownloader.Parameters.Add("@frequency", SqlDbType.Char).Value = row.Field<string>(9);
                                        cmdEP1TaxEntriesFormDownloader.Parameters.Add("@transacted", SqlDbType.Char).Value = "N";

                                        //gem added for variance
                                        cmdEP1TaxEntriesFormDownloader.Parameters.Add("@Variance", SqlDbType.Decimal).Value = Vvalue;
                                        cmdEP1TaxEntriesFormDownloader.Parameters.Add("@TEtransacted", SqlDbType.VarChar).Value = "N";

                                        try
                                        {
                                            cmdEP1TaxEntriesFormDownloader.ExecuteNonQuery();
                                            Global.GT2EP1Taxentry = true;
                                        }
                                        catch (SqlException e)
                                        {
                                            Global.GT2EP1Taxentry = false;
                                            Global.errorDesc = e.ToString();
                                        }
                                    }
                                    else
                                    {
                                        Global.errorDesc = "TaxID fetch failed! No Query initiated. Ensure TaxIdD exist in DB.";
                                    }


                                }
                                //logs
                                //if(Global.GT2EP1Taxentry == true)
                                //{
                                useBot = "GT2EP1";
                                sessionID = useBot + dateToday.ToString().Replace("/", "");
                                sessionID = sessionID.Replace(" ", "");
                                sessionID = sessionID.Replace(":", "");
                                var SessionIDtemp = sessionID;
                                foreach (DataRow rowLog in dtLogs.Rows)
                                {

                                    //Insert Logs
                                    //String queryLogsSystemsLogsOut = "INSERT INTO dbo.SystemsLogs (SessionID,SystemName,ActionDone,Result,Remarks,Exceptions,Date,[User]) VALUES (@sessionID,@sysName,@action,@result, @remarks, @exceptions,@date, @user)";
                                    SqlCommand cmdLogsSystemsLogsOut = new SqlCommand("dbo.InsertToSystemLogs", conn);
                                    cmdLogsSystemsLogsOut.CommandType = CommandType.StoredProcedure;
                                    cmdLogsSystemsLogsOut.Parameters.Add("@sessionID", SqlDbType.VarChar).Value = SessionIDtemp;
                                    cmdLogsSystemsLogsOut.Parameters.Add("@sysName", SqlDbType.Char).Value = rowLog.Field<string>(0);
                                    cmdLogsSystemsLogsOut.Parameters.Add("@action", SqlDbType.VarChar).Value = rowLog.Field<string>(1);
                                    cmdLogsSystemsLogsOut.Parameters.Add("@result", SqlDbType.VarChar).Value = rowLog.Field<string>(2);
                                    cmdLogsSystemsLogsOut.Parameters.Add("@remarks", SqlDbType.VarChar).Value = rowLog.Field<string>(3);
                                    cmdLogsSystemsLogsOut.Parameters.Add("@exceptions", SqlDbType.VarChar).Value = rowLog.Field<string>(4);
                                    cmdLogsSystemsLogsOut.Parameters.Add("@date", SqlDbType.DateTime).Value = dateToday;
                                    cmdLogsSystemsLogsOut.Parameters.Add("@user", SqlDbType.VarChar).Value = user;
                                    try
                                    {
                                        cmdLogsSystemsLogsOut.ExecuteNonQuery();
                                        Global.GT2EP1Logentry = true;
                                    }
                                    catch (SqlException e)
                                    {
                                        Global.GT2EP1Logentry = false;
                                        Global.errorDesc = e.ToString();
                                    }
                                }
                                //}
                                if (Global.GT2EP1FecthTaxID == true && Global.GT2EP1Logentry == true && Global.GT2EP1Taxentry == true)
                                {
                                    ResultText.Set(context, "Entry Completed!");
                                }
                                else
                                {
                                    ResultText.Set(context, Global.errorDesc);
                                }

                            }



                            //mai posting bot

                            else if (botName == "Posting of Tax Entries")
                            {

                                foreach (DataRow row in dt.Rows)
                                {
                                    //UPDATE EP1TaxEntriesFormDownloader SET TaxEntriesTransacted = @taxEntriesTransacted WHERE TaxID = @taxID AND Period = @period

                                    SqlCommand cmdEP1ForPostingofTaxEntriesOut = new SqlCommand("dbo.UpdateForGT4EP1EP1TaxEntriesFormDownloader", conn);
                                    cmdEP1ForPostingofTaxEntriesOut.CommandType = CommandType.StoredProcedure;
                                    cmdEP1ForPostingofTaxEntriesOut.Parameters.Add("@taxID", SqlDbType.NVarChar).Value = row.Field<string>(4);
                                    cmdEP1ForPostingofTaxEntriesOut.Parameters.Add("@period", SqlDbType.Date).Value = row.Field<string>(5);
                                    cmdEP1ForPostingofTaxEntriesOut.Parameters.Add("@taxEntriesTransacted", SqlDbType.VarChar).Value = row.Field<string>(13);
                                    try
                                    {
                                        cmdEP1ForPostingofTaxEntriesOut.ExecuteNonQuery();
                                        Global.GT4EP1TaxEntry = true;
                                    }
                                    catch (SqlException e)
                                    {
                                        Global.GT4EP1TaxEntry = false;
                                        Global.errorDesc = e.ToString();
                                    }
                                }

                                //logs
                                useBot = "GT4EP1";
                                sessionID = useBot + dateToday.ToString().Replace("/", "");
                                sessionID = sessionID.Replace(" ", "");
                                sessionID = sessionID.Replace(":", "");
                                var SessionIDtemp = sessionID;
                                foreach (DataRow rowLog in dtLogs.Rows)
                                {

                                    //Insert Logs
                                    //String queryLogsSystemsLogsOut = "INSERT INTO dbo.SystemsLogs (SessionID,SystemName,ActionDone,Result,Remarks,Exceptions,Date,[User]) VALUES (@sessionID,@sysName,@action,@result, @remarks, @exceptions,@date, @user)";
                                    SqlCommand cmdLogsSystemsLogsOut = new SqlCommand("dbo.InsertToSystemLogs", conn);
                                    cmdLogsSystemsLogsOut.CommandType = CommandType.StoredProcedure;
                                    cmdLogsSystemsLogsOut.Parameters.Add("@sessionID", SqlDbType.VarChar).Value = SessionIDtemp;
                                    cmdLogsSystemsLogsOut.Parameters.Add("@sysName", SqlDbType.Char).Value = rowLog.Field<string>(0);
                                    cmdLogsSystemsLogsOut.Parameters.Add("@action", SqlDbType.VarChar).Value = rowLog.Field<string>(1);
                                    cmdLogsSystemsLogsOut.Parameters.Add("@result", SqlDbType.VarChar).Value = rowLog.Field<string>(2);
                                    cmdLogsSystemsLogsOut.Parameters.Add("@remarks", SqlDbType.VarChar).Value = rowLog.Field<string>(3);
                                    cmdLogsSystemsLogsOut.Parameters.Add("@exceptions", SqlDbType.VarChar).Value = rowLog.Field<string>(4);
                                    cmdLogsSystemsLogsOut.Parameters.Add("@date", SqlDbType.DateTime).Value = dateToday;
                                    cmdLogsSystemsLogsOut.Parameters.Add("@user", SqlDbType.VarChar).Value = user;
                                    try
                                    {
                                        cmdLogsSystemsLogsOut.ExecuteNonQuery();
                                        Global.GT4EP1Logentry = true;
                                    }
                                    catch (SqlException e)
                                    {
                                        Global.GT4EP1Logentry = false;
                                        Global.errorDesc = e.ToString();
                                    }
                                }
                                //}
                                if (Global.GT4EP1Logentry == true && Global.GT4EP1TaxEntry == true)
                                {
                                    ResultText.Set(context, "Entry Completed!");
                                }
                                else
                                {
                                    ResultText.Set(context, Global.errorDesc);
                                }


                            }

                            //after mai's additionals

                            else if (botName == "Hitax Form Downloader")
                            {
                                //EP1 Hitax downloader

                                foreach (DataRow row in dt.Rows)
                                {

                                    //String queryEP1FormUploaderOut = "INSERT INTO dbo.EP1FormUploader (TaxpayerID,TaxpayerName,OwnerName,TaxID,Period,Filename,FormType,Transacted) VALUES (@taxPayerID,@taxPayer,@owner,@taxID, @period, @filename, @formType, @transacted)";
                                    SqlCommand cmdEP1FormUploaderOut = new SqlCommand("dbo.InsertToEP1FormUploader", conn);
                                    cmdEP1FormUploaderOut.CommandType = CommandType.StoredProcedure;
                                    cmdEP1FormUploaderOut.Parameters.Add("@taxPayerID", SqlDbType.Int).Value = row.Field<string>(0);
                                    cmdEP1FormUploaderOut.Parameters.Add("@taxPayer", SqlDbType.VarChar).Value = row.Field<string>(1);
                                    cmdEP1FormUploaderOut.Parameters.Add("@owner", SqlDbType.VarChar).Value = row.Field<string>(2);
                                    cmdEP1FormUploaderOut.Parameters.Add("@taxID", SqlDbType.NVarChar).Value = row.Field<string>(3);
                                    cmdEP1FormUploaderOut.Parameters.Add("@period", SqlDbType.Date).Value = row.Field<string>(4);
                                    cmdEP1FormUploaderOut.Parameters.Add("@filename", SqlDbType.VarChar).Value = row.Field<string>(5);
                                    cmdEP1FormUploaderOut.Parameters.Add("@formType", SqlDbType.VarChar).Value = row.Field<string>(6);
                                    cmdEP1FormUploaderOut.Parameters.Add("@transacted", SqlDbType.Char).Value = "N";
                                    try
                                    {
                                        cmdEP1FormUploaderOut.ExecuteNonQuery();
                                        Global.GT5EP1UploadForm = true;
                                    }
                                    catch (SqlException e)
                                    {
                                        Global.GT5EP1UploadForm = false;
                                        Global.errorDesc = e.ToString();
                                    }
                                    if (Global.GT5EP1UploadForm == true)
                                    {
                                        //String queryEP1TaxEntriesFormDownloaderOut = "UPDATE dbo.EP1TaxEntriesFormDownloader SET Transacted = @transactedUpdate WHERE TaxID = @taxID AND Period = @period";
                                        SqlCommand EP1TaxEntriesFormDownloaderOut = new SqlCommand("dbo.UpdateEP1TaxEntriesFormDownloader", conn);
                                        EP1TaxEntriesFormDownloaderOut.CommandType = CommandType.StoredProcedure;
                                        EP1TaxEntriesFormDownloaderOut.Parameters.Add("@taxID", SqlDbType.NVarChar).Value = row.Field<string>(3);
                                        EP1TaxEntriesFormDownloaderOut.Parameters.Add("@period", SqlDbType.Date).Value = row.Field<string>(4);
                                        EP1TaxEntriesFormDownloaderOut.Parameters.Add("@transactedUpdate", SqlDbType.Char).Value = "Y";
                                        try
                                        {
                                            EP1TaxEntriesFormDownloaderOut.ExecuteNonQuery();
                                            Global.GT5EP1Taxentry = true;
                                        }
                                        catch (SqlException e)
                                        {
                                            Global.GT5EP1Taxentry = false;
                                            Global.errorDesc = e.ToString();
                                        }
                                    }

                                }
                                //logs
                                //if(Global.GT5EP1Taxentry == true)
                                //{
                                useBot = "GT5EP1";
                                sessionID = useBot + dateToday.ToString().Replace("/", "");
                                sessionID = sessionID.Replace(" ", "");
                                sessionID = sessionID.Replace(":", "");
                                var SessionIDtemp = sessionID;
                                foreach (DataRow rowLog in dtLogs.Rows)
                                {

                                    //Insert Logs
                                    SqlCommand cmdLogsSystemsLogsOut = new SqlCommand("dbo.InsertToSystemLogs", conn);
                                    cmdLogsSystemsLogsOut.CommandType = CommandType.StoredProcedure;
                                    cmdLogsSystemsLogsOut.Parameters.Add("@sessionID", SqlDbType.VarChar).Value = SessionIDtemp;
                                    cmdLogsSystemsLogsOut.Parameters.Add("@sysName", SqlDbType.Char).Value = rowLog.Field<string>(0);
                                    cmdLogsSystemsLogsOut.Parameters.Add("@action", SqlDbType.VarChar).Value = rowLog.Field<string>(1);
                                    cmdLogsSystemsLogsOut.Parameters.Add("@result", SqlDbType.VarChar).Value = rowLog.Field<string>(2);
                                    cmdLogsSystemsLogsOut.Parameters.Add("@remarks", SqlDbType.VarChar).Value = rowLog.Field<string>(3);
                                    cmdLogsSystemsLogsOut.Parameters.Add("@exceptions", SqlDbType.VarChar).Value = rowLog.Field<string>(4);
                                    cmdLogsSystemsLogsOut.Parameters.Add("@date", SqlDbType.DateTime).Value = dateToday;
                                    cmdLogsSystemsLogsOut.Parameters.Add("@user", SqlDbType.VarChar).Value = user;
                                    try
                                    {
                                        cmdLogsSystemsLogsOut.ExecuteNonQuery();
                                        Global.GT5EP1Logentry = true;
                                    }
                                    catch (SqlException e)
                                    {
                                        Global.GT5EP1Logentry = false;
                                        Global.errorDesc = e.ToString();
                                    }
                                    //}

                                }
                                if (Global.GT5EP1UploadForm == true && Global.GT5EP1Taxentry == true && Global.GT5EP1Logentry == true)
                                {
                                    ResultText.Set(context, "Entry Completed!");
                                }
                                else
                                {
                                    ResultText.Set(context, Global.errorDesc);
                                }
                            }
                            else if (botName == "TRACKUploadingProcess")
                            {
                                //EP1 EP1 Track Uploader
                                foreach (DataRow row in dt.Rows)
                                {

                                    //String queryEP1FormUploaderOut = "UPDATE dbo.EP1FormUploader SET Transacted = @transacted WHERE TaxID = @taxID AND Period = @period";
                                    SqlCommand cmdEP1FormUploaderOut = new SqlCommand("UpdateEP1FormUploader", conn);
                                    cmdEP1FormUploaderOut.CommandType = CommandType.StoredProcedure;
                                    cmdEP1FormUploaderOut.Parameters.Add("@taxID", SqlDbType.NVarChar).Value = row.Field<string>(3);
                                    cmdEP1FormUploaderOut.Parameters.Add("@period", SqlDbType.Date).Value = row.Field<string>(4);
                                    cmdEP1FormUploaderOut.Parameters.Add("@transacted", SqlDbType.Char).Value = "Y";
                                    try
                                    {
                                        cmdEP1FormUploaderOut.ExecuteNonQuery();
                                        Global.GT6EP1Taxentry = true;
                                    }
                                    catch (SqlException e)
                                    {
                                        Global.GT6EP1Taxentry = false;
                                        Global.errorDesc = e.ToString();
                                    }
                                }
                                //logs
                                //if(Global.GT6EP1Taxentry == true)
                                //{
                                useBot = "GT6EP1";
                                sessionID = useBot + dateToday.ToString().Replace("/", "");
                                sessionID = sessionID.Replace(" ", "");
                                sessionID = sessionID.Replace(":", "");
                                var SessionIDtemp = sessionID;
                                foreach (DataRow rowLog in dtLogs.Rows)
                                {

                                    //Insert Logs
                                    SqlCommand cmdLogsSystemsLogsOut = new SqlCommand("dbo.InsertToSystemLogs", conn);
                                    cmdLogsSystemsLogsOut.CommandType = CommandType.StoredProcedure;
                                    cmdLogsSystemsLogsOut.Parameters.Add("@sessionID", SqlDbType.VarChar).Value = SessionIDtemp;
                                    cmdLogsSystemsLogsOut.Parameters.Add("@sysName", SqlDbType.Char).Value = rowLog.Field<string>(0);
                                    cmdLogsSystemsLogsOut.Parameters.Add("@action", SqlDbType.VarChar).Value = rowLog.Field<string>(1);
                                    cmdLogsSystemsLogsOut.Parameters.Add("@result", SqlDbType.VarChar).Value = rowLog.Field<string>(2);
                                    cmdLogsSystemsLogsOut.Parameters.Add("@remarks", SqlDbType.VarChar).Value = rowLog.Field<string>(3);
                                    cmdLogsSystemsLogsOut.Parameters.Add("@exceptions", SqlDbType.VarChar).Value = rowLog.Field<string>(4);
                                    cmdLogsSystemsLogsOut.Parameters.Add("@date", SqlDbType.DateTime).Value = dateToday;
                                    cmdLogsSystemsLogsOut.Parameters.Add("@user", SqlDbType.VarChar).Value = user;
                                    try
                                    {
                                        cmdLogsSystemsLogsOut.ExecuteNonQuery();
                                        Global.GT6EP1Logentry = true;
                                    }
                                    catch (SqlException e)
                                    {
                                        Global.GT6EP1Logentry = false;
                                        Global.errorDesc = e.ToString();
                                    }
                                }
                                //}
                                if (Global.GT6EP1Taxentry == true && Global.GT6EP1Logentry == true)
                                {
                                    ResultText.Set(context, "Entry Completed!");
                                }
                                else
                                {
                                    ResultText.Set(context, Global.errorDesc);
                                }
                            }
                            else
                            {
                                //Nothing Found
                            }

                        }
                        catch (SqlException)
                        {
                            dbopen = false;
                        }

                        if (dbopen == true)
                        {
                            //DB connection is Open
                            //ResultText.Set(context, resultOut);

                        }
                        else
                        {
                            //DB connection is Closed
                            ResultText.Set(context, "Database Connection is Closed!");
                        }
                        conn.Close();
                    }
                    #endregion
                }

            }
            catch (Exception e)
            {
                ResultText.Set(context, "Database Connection Failed!" + e.ToString());
            }
        }

    }
}

