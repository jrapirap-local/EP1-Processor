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
    static class GlobalIn
    {
        //added by MAi GT4EP1PostingofTaxEntries
        public static string errorDescIn;
        public static string fetchRefCode;
        public static Boolean GT4EP1FetchRefCode;
        public static Boolean GT4EP1PostingofTaxEntries;
        public static Boolean GT6EP1UploadForm;
        public static Boolean GT5EP1DownloadForm;
        public static Boolean GT6EP1ForDownload;


    }
    public class GetData : CodeActivity
    {
        //Coode Activity Start
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

        [Category("Output")]
        [RequiredArgument]//Required
        public OutArgument<string> ResultText { get; set; }

        [Category("Output")]
        [RequiredArgument]//Required
        public InArgument<DataTable> ResultDatatable { get; set; }
        //Coode Activity End

        //Added by mai for GT4EP1 process
        [Category("Output")]
        public OutArgument<string> RefCode { get; set; }



        protected override void Execute(CodeActivityContext context)
        {
            //local Variables
            var dtOut = ResultDatatable.Get(context);
            var botName = BotName.Get(context);
            var localBotVersion = BotVersion.Get(context);
            var serverName = ServerName.Get(context);
            var userName = UserName.Get(context);
            var password = Password.Get(context);
            var period = FilingPeriod.Get(context);
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
                                String selstrEP1ElectronicManualFilerIn = "SELECT * from dbo.EP1ElectronicManualFiler where Transacted = @transacted AND Period = @filingPeriod";
                                SqlCommand cmdEP1ElectronicManualFilerIn = new SqlCommand(selstrEP1ElectronicManualFilerIn, conn);
                                cmdEP1ElectronicManualFilerIn.Parameters.AddWithValue("@filingPeriod", period);
                                cmdEP1ElectronicManualFilerIn.Parameters.AddWithValue("@transacted", "N");
                                SqlDataReader rdrEP1ElectronicManualFilerIn = cmdEP1ElectronicManualFilerIn.ExecuteReader();

                                while (rdrEP1ElectronicManualFilerIn.Read())
                                {
                                    DataRow dtRow = dtOut.NewRow();
                                    dtRow["TAXPAYERID"] = rdrEP1ElectronicManualFilerIn.GetValue(0).ToString();
                                    dtRow["TAXPAYER NAME"] = rdrEP1ElectronicManualFilerIn.GetValue(1).ToString();
                                    dtRow["OWNER NAME"] = rdrEP1ElectronicManualFilerIn.GetValue(2).ToString();
                                    dtRow["PROPERTIES"] = rdrEP1ElectronicManualFilerIn.GetValue(3).ToString();
                                    dtRow["HAWAIIDISTRICT"] = rdrEP1ElectronicManualFilerIn.GetValue(4).ToString();
                                    dtRow["OAHUDISTRICT"] = rdrEP1ElectronicManualFilerIn.GetValue(5).ToString();
                                    dtRow["MAUIDISTRICT"] = rdrEP1ElectronicManualFilerIn.GetValue(6).ToString();
                                    dtRow["KAUAIDISTRICT"] = rdrEP1ElectronicManualFilerIn.GetValue(7).ToString();
                                    dtRow["ACCTUSED"] = rdrEP1ElectronicManualFilerIn.GetValue(8).ToString();
                                    dtRow["USERNAME"] = rdrEP1ElectronicManualFilerIn.GetValue(9).ToString();
                                    dtRow["PASSWORD"] = rdrEP1ElectronicManualFilerIn.GetValue(10).ToString();
                                    dtRow["GETTATID"] = rdrEP1ElectronicManualFilerIn.GetValue(11).ToString();
                                    dtRow["WITHEXEMPTIONS"] = rdrEP1ElectronicManualFilerIn.GetValue(12).ToString();
                                    dtRow["WITHTATEXEMPTIONS"] = rdrEP1ElectronicManualFilerIn.GetValue(13).ToString();
                                    dtRow["WITHCANCELLATIONEXEMPTIONS"] = rdrEP1ElectronicManualFilerIn.GetValue(14).ToString();
                                    var tempPeriod = rdrEP1ElectronicManualFilerIn.GetValue(15).ToString().Substring(0, 10);
                                    dtRow["PERIOD"] = tempPeriod;
                                    dtRow["FREQUENCY"] = rdrEP1ElectronicManualFilerIn.GetValue(16).ToString().Replace(" ", "");
                                    dtRow["GROSSREVENUES"] = rdrEP1ElectronicManualFilerIn.GetValue(17).ToString();
                                    dtRow["EXEMPTION"] = rdrEP1ElectronicManualFilerIn.GetValue(18).ToString();
                                    dtRow["CANCELLATIONFEES"] = rdrEP1ElectronicManualFilerIn.GetValue(19).ToString();
                                    dtRow["CANCELLATIONFEESEXEMPTIONS"] = rdrEP1ElectronicManualFilerIn.GetValue(20).ToString();
                                    dtRow["GETTATDUE"] = rdrEP1ElectronicManualFilerIn.GetValue(21).ToString(); ;
                                }

                                rdrEP1ElectronicManualFilerIn.Close();
                            }

                            // mai additinals for tax entries process
                            else if (botName == "Posting of Tax Entries")
                            {
                                //Fetch Reference code from previous record in the Database
                                SqlCommand cmdEP1ReferenceCodeOut = new SqlCommand("dbo.GetEP1ReferenceCode", conn);
                                cmdEP1ReferenceCodeOut.CommandType = CommandType.StoredProcedure;
                                SqlDataReader rdrReferenceCodeOut = cmdEP1ReferenceCodeOut.ExecuteReader();

                                if (rdrReferenceCodeOut.Read())
                                {
                                    string ReferenceCode = rdrReferenceCodeOut.GetValue(0).ToString();
                                    int intIncrementRefCOde = Convert.ToInt16(ReferenceCode);
                                    string strBankCode = "8707";
                                    GlobalIn.fetchRefCode = strBankCode + intIncrementRefCOde.ToString("00000");
                                    RefCode.Set(context, GlobalIn.fetchRefCode.ToString());
                                    GlobalIn.GT4EP1FetchRefCode = true;
                                }
                                else
                                {

                                    GlobalIn.GT4EP1FetchRefCode = false;

                                }
                                rdrReferenceCodeOut.Close();


                                //SELECT* from dbo.EP1TaxEntriesFormDownloader where TaxEntriesTransacted = @TEtransacted AND Period = @filingPeriod
                                SqlCommand cmdEP1ForPostingofTaxEntriesIn = new SqlCommand("dbo.GetEP1ForPostingofTaxEntries", conn);
                                cmdEP1ForPostingofTaxEntriesIn.CommandType = CommandType.StoredProcedure;
                                cmdEP1ForPostingofTaxEntriesIn.Parameters.Add("@filingPeriod", SqlDbType.Date).Value = period;
                                cmdEP1ForPostingofTaxEntriesIn.Parameters.Add("@TEtransacted", SqlDbType.VarChar).Value = "N";
                                SqlDataReader rdrEP1ForPostingofTaxEntriesIn = cmdEP1ForPostingofTaxEntriesIn.ExecuteReader();

                                while (rdrEP1ForPostingofTaxEntriesIn.Read())

                                {
                                    DataRow dtRow = dtOut.NewRow();
                                    if (rdrEP1ForPostingofTaxEntriesIn.GetValue(0).ToString() == "" ||
                                         rdrEP1ForPostingofTaxEntriesIn.GetValue(1).ToString() == "" ||
                                         rdrEP1ForPostingofTaxEntriesIn.GetValue(2).ToString() == "" ||
                                         rdrEP1ForPostingofTaxEntriesIn.GetValue(3).ToString() == "" ||
                                         rdrEP1ForPostingofTaxEntriesIn.GetValue(4).ToString() == "" ||
                                         rdrEP1ForPostingofTaxEntriesIn.GetValue(5).ToString() == "" ||
                                         rdrEP1ForPostingofTaxEntriesIn.GetValue(6).ToString() == "" ||
                                         rdrEP1ForPostingofTaxEntriesIn.GetValue(7).ToString() == "" ||
                                         rdrEP1ForPostingofTaxEntriesIn.GetValue(8).ToString() == "" ||
                                         rdrEP1ForPostingofTaxEntriesIn.GetValue(9).ToString() == "" ||
                                         rdrEP1ForPostingofTaxEntriesIn.GetValue(10).ToString() == "" ||
                                         rdrEP1ForPostingofTaxEntriesIn.GetValue(11).ToString() == "" ||
                                         rdrEP1ForPostingofTaxEntriesIn.GetValue(12).ToString() == "" ||
                                         rdrEP1ForPostingofTaxEntriesIn.GetValue(13).ToString() == "")
                                    {
                                        GlobalIn.GT4EP1PostingofTaxEntries = false;
                                    }
                                    else
                                    {
                                        dtRow["TaxpayerID"] = rdrEP1ForPostingofTaxEntriesIn.GetValue(0).ToString();
                                        dtRow["TaxpayerName"] = rdrEP1ForPostingofTaxEntriesIn.GetValue(1).ToString();
                                        dtRow["OwnerName"] = rdrEP1ForPostingofTaxEntriesIn.GetValue(2).ToString();
                                        dtRow["Properties"] = rdrEP1ForPostingofTaxEntriesIn.GetValue(3).ToString();
                                        dtRow["TaxID"] = rdrEP1ForPostingofTaxEntriesIn.GetValue(4).ToString();
                                        var tempPeriod = rdrEP1ForPostingofTaxEntriesIn.GetValue(5).ToString().Substring(0, 10);
                                        dtRow["Period"] = tempPeriod;
                                        dtRow["GetTatPayment"] = rdrEP1ForPostingofTaxEntriesIn.GetValue(6).ToString();
                                        dtRow["ConfirmationNumber"] = rdrEP1ForPostingofTaxEntriesIn.GetValue(7).ToString();
                                        dtRow["Username"] = rdrEP1ForPostingofTaxEntriesIn.GetValue(8).ToString();
                                        dtRow["Password"] = rdrEP1ForPostingofTaxEntriesIn.GetValue(9).ToString();
                                        dtRow["Frequency"] = rdrEP1ForPostingofTaxEntriesIn.GetValue(10).ToString();
                                        dtRow["Transacted"] = rdrEP1ForPostingofTaxEntriesIn.GetValue(11).ToString();
                                        dtRow["Variance"] = rdrEP1ForPostingofTaxEntriesIn.GetValue(12).ToString();
                                        dtRow["TaxEntriesTransacted"] = rdrEP1ForPostingofTaxEntriesIn.GetValue(13).ToString();
                                        dtOut.Rows.Add(dtRow);
                                        GlobalIn.GT4EP1PostingofTaxEntries = true;
                                    }

                                }
                                rdrEP1ForPostingofTaxEntriesIn.Close();
                                if (GlobalIn.GT4EP1PostingofTaxEntries == false)
                                {
                                    GlobalIn.errorDescIn = "No Record(s) selected!";
                                }

                            }

                            //after mai additionals


                            else if (botName == "Hitax Form Downloader")
                            {
                                //EP1 Hitax downloader
                                //Get Filing Period
                                //String selstrEP1TaxEntriesFormDownloaderIn = "SELECT * from dbo.EP1TaxEntriesFormDownloader where Transacted = @transacted AND Period = @filingPeriod";
                                SqlCommand cmdEP1TaxEntriesFormDownloaderIn = new SqlCommand("dbo.GetEP1TaxEntriesFormDownloader", conn);
                                cmdEP1TaxEntriesFormDownloaderIn.CommandType = CommandType.StoredProcedure;
                                //variables to Storedproc
                                cmdEP1TaxEntriesFormDownloaderIn.Parameters.Add("@filingPeriod", SqlDbType.Date).Value = period;
                                cmdEP1TaxEntriesFormDownloaderIn.Parameters.Add("@transacted", SqlDbType.Char).Value = "N";

                                SqlDataReader rdrEP1TaxEntriesFormDownloaderIn = cmdEP1TaxEntriesFormDownloaderIn.ExecuteReader();

                                while (rdrEP1TaxEntriesFormDownloaderIn.Read())
                                {
                                    if (rdrEP1TaxEntriesFormDownloaderIn.GetValue(0).ToString() == "" || rdrEP1TaxEntriesFormDownloaderIn.GetValue(1).ToString() == "" || rdrEP1TaxEntriesFormDownloaderIn.GetValue(2).ToString() == "" || rdrEP1TaxEntriesFormDownloaderIn.GetValue(4).ToString() == "" || rdrEP1TaxEntriesFormDownloaderIn.GetValue(6).ToString() == "" || rdrEP1TaxEntriesFormDownloaderIn.GetValue(7).ToString() == "" || rdrEP1TaxEntriesFormDownloaderIn.GetValue(8).ToString() == "" || rdrEP1TaxEntriesFormDownloaderIn.GetValue(9).ToString() == "" || rdrEP1TaxEntriesFormDownloaderIn.GetValue(10).ToString() == "")
                                    {
                                        GlobalIn.GT5EP1DownloadForm = false;
                                    }
                                    else
                                    {
                                        DataRow dtRow = dtOut.NewRow();
                                        dtRow["TAXPAYERID"] = rdrEP1TaxEntriesFormDownloaderIn.GetValue(0).ToString();
                                        dtRow["TAXPAYER NAME"] = rdrEP1TaxEntriesFormDownloaderIn.GetValue(1).ToString();
                                        dtRow["OWNER NAME"] = rdrEP1TaxEntriesFormDownloaderIn.GetValue(2).ToString();
                                        dtRow["PROPERTIES"] = rdrEP1TaxEntriesFormDownloaderIn.GetValue(3).ToString();
                                        dtRow["GETTATID"] = rdrEP1TaxEntriesFormDownloaderIn.GetValue(4).ToString();
                                        var tempPeriod = rdrEP1TaxEntriesFormDownloaderIn.GetValue(5).ToString().Substring(0, 10);
                                        dtRow["PERIOD"] = tempPeriod;
                                        dtRow["GETTAT PAYMENT"] = rdrEP1TaxEntriesFormDownloaderIn.GetValue(6).ToString();
                                        dtRow["CONFIRMATION NUMBER"] = rdrEP1TaxEntriesFormDownloaderIn.GetValue(7).ToString();
                                        dtRow["USERNAME"] = rdrEP1TaxEntriesFormDownloaderIn.GetValue(8).ToString();
                                        dtRow["PASSWORD"] = rdrEP1TaxEntriesFormDownloaderIn.GetValue(9).ToString();
                                        dtRow["FREQUENCY"] = rdrEP1TaxEntriesFormDownloaderIn.GetValue(10).ToString().Replace(" ", "");
                                        dtOut.Rows.Add(dtRow);
                                        GlobalIn.GT5EP1DownloadForm = true;
                                    }

                                }
                                rdrEP1TaxEntriesFormDownloaderIn.Close();
                                if (GlobalIn.GT5EP1DownloadForm == false)
                                {
                                    GlobalIn.errorDescIn = "No Record(s) selected!";
                                }
                            }
                            else if (botName == "TRACKUploadingProcess")
                            {
                                //EP1 EP1 Track Uploader
                                //String selstrEP1FormUploaderIn = "SELECT * from dbo.EP1FormUploader WHERE Transacted = @transacted AND Period = @filingPeriod";
                                SqlCommand cmdEP1FormUploaderIn = new SqlCommand("dbo.GetEP1FormUploader", conn);
                                cmdEP1FormUploaderIn.CommandType = CommandType.StoredProcedure;
                                cmdEP1FormUploaderIn.Parameters.Add("@filingPeriod", SqlDbType.Date).Value = period;
                                cmdEP1FormUploaderIn.Parameters.Add("@transacted", SqlDbType.Char).Value = "N";
                                SqlDataReader rdrEP1FormUploaderIn = cmdEP1FormUploaderIn.ExecuteReader();

                                while (rdrEP1FormUploaderIn.Read())
                                {
                                    DataRow dtRow = dtOut.NewRow();
                                    if (rdrEP1FormUploaderIn.GetValue(0).ToString() == "" || rdrEP1FormUploaderIn.GetValue(1).ToString() == "" || rdrEP1FormUploaderIn.GetValue(2).ToString() == "" || rdrEP1FormUploaderIn.GetValue(3).ToString() == "" || rdrEP1FormUploaderIn.GetValue(5).ToString() == "" || rdrEP1FormUploaderIn.GetValue(6).ToString() == "" || rdrEP1FormUploaderIn.GetValue(7).ToString() == "")
                                    {
                                        GlobalIn.GT6EP1UploadForm = false;
                                    }
                                    else
                                    {
                                        dtRow["TaxpayerID"] = rdrEP1FormUploaderIn.GetValue(0).ToString();
                                        dtRow["TaxpayerName"] = rdrEP1FormUploaderIn.GetValue(1).ToString();
                                        dtRow["OwnerName"] = rdrEP1FormUploaderIn.GetValue(2).ToString();
                                        dtRow["TaxID"] = rdrEP1FormUploaderIn.GetValue(3).ToString();
                                        var tempPeriod = rdrEP1FormUploaderIn.GetValue(4).ToString().Substring(0, 10);
                                        dtRow["Period"] = tempPeriod;
                                        dtRow["Filename"] = rdrEP1FormUploaderIn.GetValue(5).ToString();
                                        dtRow["FormType"] = rdrEP1FormUploaderIn.GetValue(6).ToString();
                                        dtRow["Transacted"] = rdrEP1FormUploaderIn.GetValue(7).ToString();
                                        dtOut.Rows.Add(dtRow);
                                        GlobalIn.GT6EP1UploadForm = true;
                                    }

                                }
                                rdrEP1FormUploaderIn.Close();
                                if (GlobalIn.GT6EP1UploadForm == false)
                                {
                                    GlobalIn.errorDescIn = "No Record(s) selected!";
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
                        }
                        else
                        {
                            //DB connection is Closed
                            ResultText.Set(context, "No Open Database Connection.");
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

