

/******************************************************
 * Intuit sample app for Oauth2 using Intuit .Net SDK
 * RFC docs- https://tools.ietf.org/html/rfc6749
 * ****************************************************/

//https://stackoverflow.com/questions/23562044/window-opener-is-undefined-on-internet-explorer/26359243#26359243
//IE issue- https://stackoverflow.com/questions/7648231/javascript-issue-in-ie-with-window-opener

using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Web.UI;
using System.Configuration;
using System.Web;
using Intuit.Ipp.OAuth2PlatformClient;
using System.Threading.Tasks;
using Intuit.Ipp.Core;
using Intuit.Ipp.Data;
using Intuit.Ipp.DataService;
using Intuit.Ipp.QueryFilter;
using Intuit.Ipp.Security;
using Intuit.Ipp.Exception;
using System.Linq;
using Intuit.Ipp.ReportService;

namespace ProfitProgress.web
{
    public partial class Default : System.Web.UI.Page
    {
        // OAuth2 client configuration
        static string redirectURI = ConfigurationManager.AppSettings["redirectURI"];
        static string clientID = ConfigurationManager.AppSettings["clientID"];
        static string clientSecret = ConfigurationManager.AppSettings["clientSecret"];
        static string logPath = ConfigurationManager.AppSettings["logPath"];
        static string appEnvironment = ConfigurationManager.AppSettings["appEnvironment"];
        static OAuth2Client oauthClient = new OAuth2Client(clientID, clientSecret, redirectURI, appEnvironment);
        static string authCode;
        static string idToken;
        public static IList<JsonWebKey> keys;
        public static Dictionary<string, string> dictionary = new Dictionary<string, string>();

        private List<SnapshotListItem> _items = new List<SnapshotListItem>();
        private List<SnapshotListItem> _prevperioditems = new List<SnapshotListItem>();

        private string[] _recurringSalesItems = new string[] { "Gardening", "Trimming", "Services" };
        private string[] _growthSalesItem = new string[] { "Concrete" };
        private string[] _sicCodeArray = new string[] { "45001" };
        private decimal[] _sicCodeMults = new decimal[] { 2.01m, 3.11m, 5.6m };

        protected void Page_PreInit(object sender, EventArgs e)
        {
            if (!dictionary.ContainsKey("accessToken"))
            {
                mainButtons.Visible = true;
                connected.Visible = false;
            }
            else
            {
                mainButtons.Visible = false;
                connected.Visible = true;
            }
        }

        protected async void Page_Load(object sender, EventArgs e)
        {
            AsyncMode = true;

            if (Environment.MachineName.ToLower().Contains("cody"))
                redirectURI = "http://localhost:59785/Default.aspx";
            this.txtlog.Text = "";

            if (Request.QueryString["signoff"] == "Y")
            {
                dictionary.Clear();
                try { await oauthClient.RevokeTokenAsync("refreshToken"); } catch { }
            }
            
            if (!dictionary.ContainsKey("accessToken"))
            {
                if (Request.QueryString.Count > 0)
                {
                    var response = new AuthorizeResponse(Request.QueryString.ToString());
                    if (response.State != null)
                    {
                        if (oauthClient.CSRFToken == response.State)
                        {
                            if (response.RealmId != null)
                            {
                                if (!dictionary.ContainsKey("realmId"))
                                {
                                    dictionary.Add("realmId", response.RealmId);
                                }
                            }

                            if (response.Code != null)
                            {
                                authCode = response.Code;
                                Output("Authorization code obtained.", false);
                                PageAsyncTask t = new PageAsyncTask(PerformCodeExchange);
                                Page.RegisterAsyncTask(t);
                                Page.ExecuteRegisteredAsyncTasks();

                               
                            }
                        }
                        else
                        {
                            Output("Invalid State", false);
                            dictionary.Clear();
                        }
                    }
                }
            }
            else
            {
                mainButtons.Visible = false;
                connected.Visible = true;
                if (Page.IsPostBack && dictionary.ContainsKey("accessToken") && dictionary.ContainsKey("realmId"))
                {
                    await CallAll();
                }
            }
        }

        private void Output(string s, bool isError)
        {
            s += "\r\n";
            this.txtlog.Text += s;
        }

        protected void ImgC2QB_Click(object sender, ImageClickEventArgs e)
        {
            Output("Intiating OAuth2 call.", false);
            try
            {
                if (!dictionary.ContainsKey("accessToken"))
                {
                    List<OidcScopes> scopes = new List<OidcScopes>();
                    scopes.Add(OidcScopes.Accounting);
                    var authorizationRequest = oauthClient.GetAuthorizationURL(scopes);
                    Response.Redirect(authorizationRequest, "_blank", "menubar=0,scrollbars=1,width=780,height=900,top=10");
                }
            }
            catch (Exception ex)
            {
                Output(ex.Message, true);
            }
        }

        /// <summary>
        /// Start code exchange to get the Access Token and Refresh Token
        /// </summary>
        public async System.Threading.Tasks.Task PerformCodeExchange()
        {
            Output("Exchanging code for tokens.", false);
            try
            {
                var tokenResp = await oauthClient.GetBearerTokenAsync(authCode);
                if (!dictionary.ContainsKey("accessToken"))
                    dictionary.Add("accessToken", tokenResp.AccessToken);
                else
                    dictionary["accessToken"] = tokenResp.AccessToken;

                if (!dictionary.ContainsKey("refreshToken"))
                    dictionary.Add("refreshToken", tokenResp.RefreshToken);
                else
                    dictionary["refreshToken"] = tokenResp.RefreshToken;

                if (tokenResp.IdentityToken != null)
                    idToken = tokenResp.IdentityToken;
                if (Request.Url.Query == "")
                {
                    Response.Redirect(Request.RawUrl);
                }
                else
                {
                    Response.Redirect(Request.RawUrl.Replace(Request.Url.Query, ""));
                }
            }
            catch (Exception ex)
            {
                Output("Problem while getting bearer tokens.", true);
            }
        }

        /// <summary>
        /// Test QBO api call
        /// </summary>
        /// 
        /*
        public async System.Threading.Tasks.Task QboApiCall()
        {
            try
            {
                if ((dictionary.ContainsKey("accessToken")) && (dictionary.ContainsKey("realmId")))
                {
                    Output("Making QBO API Call.", false);
                    OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(dictionary["accessToken"]);
                    ServiceContext serviceContext = new ServiceContext(dictionary["realmId"], IntuitServicesType.QBO, oauthValidator);
                    //serviceContext.IppConfiguration.BaseUrl.Qbo = "https://sandbox-quickbooks.api.intuit.com/";
                    //serviceContext.IppConfiguration.BaseUrl.Qbo = "https://quickbooks.api.intuit.com/";//prod
                    serviceContext.IppConfiguration.BaseUrl.Qbo = ConfigurationManager.AppSettings["baseURL"];
                    serviceContext.IppConfiguration.MinorVersion.Qbo = "29";
                    ReportService reportService = new ReportService(serviceContext);



                    //List<String> columndata = new List<String>();
                    //columndata.Add("tx_date");
                    //columndata.Add("dept_name");
                    //string coldata = String.Join(",", columndata);
                    //reportService.columns = coldata;

                    DataService commonServiceQBO = new DataService(serviceContext);
                    Batch batch = commonServiceQBO.CreateNewBatch();
                    batch.Add("select * from Account", "queryAccount");
                    batch.Execute();

                    if (batch.IntuitBatchItemResponses != null && batch.IntuitBatchItemResponses.Count() > 0)
                    {
                        IntuitBatchResponse res = batch.IntuitBatchItemResponses.FirstOrDefault();
                        List<Account> acc = res.Entities.ToList().ConvertAll(item => item as Account);
                    };


                    QueryService<Purchase> inBalance = new QueryService<Purchase>(serviceContext);
                    var bsheet = inBalance.ExecuteIdsQuery("SELECT * FROM Purchase WHERE MetaData.CreateTime >= '2001-10-14T04:05:05-07:00' Order by TxnDate DESC", QueryOperationType.query);

                    decimal tot = decimal.Zero;
                    foreach (var b in bsheet)
                    {
                        foreach (var l in b.Line)
                        {
                            tot += l.Amount;
                        }
                    }
                    //((Intuit.Ipp.Data.AccountBasedExpenseLineDetail)l.AnyIntuitObject).AccountRef.name
                    this.txtlog.Text = "Total Expenses: " + tot.ToString("c");

                    //                 var expenses = bsheet.Where(x => x.Classification == AccountClassificationEnum.Expense).ToList();

                    int a = 1;

                    /*
                    DataService commonServiceQBO = new DataService(serviceContext);
                    QueryService<Invoice> inService = new QueryService<Invoice>(serviceContext);
                    var In = inService.ExecuteIdsQuery("SELECT count(*) FROM Invoice").Count();

                    Batch batch = commonServiceQBO.CreateNewBatch();
                   

                    batch.Add("select count(*) from Account", "queryAccount");
                    batch.Execute();

                    if (batch.IntuitBatchItemResponses != null && batch.IntuitBatchItemResponses.Count() > 0)
                    {
                        IntuitBatchResponse res = batch.IntuitBatchItemResponses.FirstOrDefault();
                        List<Account> acc = res.Entities.ToList().ConvertAll(item => item as Account);
                    };

                    Invoice invoice = QBOHelper.CreateInvoice(serviceContext);
                    //Adding the Invoice
                    Invoice added = Helper.Add<Invoice>(serviceContext, invoice);

                    

                    Output("QBO call successful.", false);
                    //lblQBOCall.Visible = true;
                    //lblQBOCall.Text = "QBO Call successful";
                }

            }
            catch (IdsException ex)
            {
                if (ex.Message == "Unauthorized-401")
                {
                    Output("Invalid/Expired Access Token.", false);

                    var tokenResp = await oauthClient.RefreshTokenAsync(dictionary["refreshToken"]);
                    if (tokenResp.AccessToken != null && tokenResp.RefreshToken != null)
                    {
                        dictionary["accessToken"] = tokenResp.AccessToken;
                        dictionary["refreshToken"] = tokenResp.RefreshToken;
                        await QboApiCall();
                    }
                    else
                    {
                        Output("Error while refreshing tokens: " + tokenResp.Raw, true);
                    }
                }
                else
                {
                    Output(ex.Message, true);
                }
            }
            catch (Exception ex)
            {
                Output("Invalid/Expired Access Token.", true);
            }
        }
        */
        protected async void btnloaddashboard_Click(object sender, EventArgs e)
        {
            if (dictionary.ContainsKey("accessToken") && dictionary.ContainsKey("realmId"))
            {
                _prevperioditems.Clear();
                _items.Clear();
                await CallAll();
            }
            else
            {
                Output("Inside Load Dashboard Click Access token not found.", false);
            }
        }

        public async System.Threading.Tasks.Task CallAll()
        {
            try
            {
                if ((dictionary.ContainsKey("accessToken")) && (dictionary.ContainsKey("realmId")))
                {
                    Output("Making QBO API Call.", false);
                    OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(dictionary["accessToken"]);
                    ServiceContext serviceContext = new ServiceContext(dictionary["realmId"], IntuitServicesType.QBO, oauthValidator);
                    //serviceContext.IppConfiguration.BaseUrl.Qbo = "https://sandbox-quickbooks.api.intuit.com/";
                    //serviceContext.IppConfiguration.BaseUrl.Qbo = "https://quickbooks.api.intuit.com/";//prod
                    serviceContext.IppConfiguration.BaseUrl.Qbo = ConfigurationManager.AppSettings["baseURL"];
                    serviceContext.IppConfiguration.MinorVersion.Qbo = "29";
                    ReportService reportService = new ReportService(serviceContext);
                    Output("Base URL is " + serviceContext.IppConfiguration.BaseUrl.Qbo, false);
                    loadItems(Convert.ToDateTime(dtstart.Text).ToString("yyyy-MM-dd"), Convert.ToDateTime(dtend.Text).ToString("yyyy-MM-dd"), cbcashoraccrual.SelectedValue, ref reportService, ref serviceContext, true);

                    DateTime prevstartdt = Convert.ToDateTime(dtstart.Text);
                    DateTime prevenddt = Convert.ToDateTime(dtend.Text);
                    switch (cbgrowthcompare.SelectedValue)
                    {
                        case "LastYear":
                            prevstartdt = Convert.ToDateTime(dtstart.Text).AddYears(-1);
                            prevenddt = Convert.ToDateTime(dtend.Text).AddYears(-1);
                            break;
                        case "LastMonth":
                            prevstartdt = Convert.ToDateTime(dtstart.Text).AddMonths(-1);
                            prevenddt = Convert.ToDateTime(dtend.Text).AddMonths(-1);
                            break;
                    }
                    loadItems(prevstartdt.ToString("yyyy-MM-dd"), prevenddt.ToString("yyyy-MM-dd"), cbcashoraccrual.SelectedValue, ref reportService, ref serviceContext, false);

                    this.dtdata.DataSource = _items;
                    this.dtdata.DataBind();


                    //lblQBOCall.Visible = true;
                    //lblQBOCall.Text = "QBO Call successful";
                    this.dlitems.DataSource = _items;
                    this.dlitems.DataBind();

                    this.dlitemsprev.DataSource = _prevperioditems;
                    this.dlitemsprev.DataBind();

                    LoadPredictableRevenue();
                    LoadProfitQuilt();
                    LoadProfitReality();

                    LoadRevenueGrowth();
                    LoadExpenseGrowth();
                    LoadProfitGrowth();

                    LoadRevExpenseRatio();

                    LoadRelativeHumidity();

                    this.metrics.Visible = true;
                }

            }
            catch (IdsException ex)
            {
                Output("Exception in CallAll" + ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""), true);
                if (ex.Message == "Unauthorized-401")
                {
                    Output("Invalid/Expired Access Token.", false);

                    var tokenResp = await oauthClient.RefreshTokenAsync(dictionary["refreshToken"]);
                    if (tokenResp.AccessToken != null && tokenResp.RefreshToken != null)
                    {
                        dictionary["accessToken"] = tokenResp.AccessToken;
                        dictionary["refreshToken"] = tokenResp.RefreshToken;
                        await CallAll();
                    }
                    else
                    {
                        Output("Error while refreshing tokens: " + tokenResp.Raw, false);
                    }
                }
                else
                {
                    Output(ex.Message, true);
                }
            }
            catch (Exception ex)
            {
                Output("Exception in CallAll" + ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""), true);
            }
        }


        private void loadItems(string startdt, string enddt, string accttype, ref ReportService reportService, ref ServiceContext serviceContext, bool isCurrentItem = true)
        {
            Output("+++++++++++++++++++++++++ Inside loadItems +++++++++++++++++++++++++", false);
            reportService.accounting_method = accttype;
            reportService.start_date = startdt.ToString();
            reportService.end_date = enddt.ToString();
            reportService.summarize_column_by = "Total";

            Output($"Using {startdt} to {enddt}", false);

            var rptItemSales = reportService.ExecuteReport("ItemSales");
            if (rptItemSales != null)
            {
                Output($"Found {rptItemSales.Rows.Length} Item Sales Rows", false);
            }
            else
                Output("No Item Sales Found", false);


            foreach (var row in rptItemSales.Rows)
            {
                try
                {
                    var cols = (Intuit.Ipp.Data.ColData[])row.AnyIntuitObjects.FirstOrDefault();
                    try
                    {
                        if (!String.Equals("Total", cols[0].value, StringComparison.OrdinalIgnoreCase))
                        {
                            decimal d = Convert.ToDecimal(cols[2].value);
                            if (isCurrentItem)
                                _items.Add(new SnapshotListItem() { AcctType = "PRODUCT", Acct = cols[0].value, Value = d });
                            else
                                _prevperioditems.Add(new SnapshotListItem() { AcctType = "PRODUCT", Acct = cols[0].value, Value = d });
                        }
                    }
                    catch (Exception exc)
                    {
                        Output("Item Sales: " + exc.Message, true);
                        throw exc;
                    }
                }
                catch { }
            }

            var rptGeneralLedger = reportService.ExecuteReport("GeneralLedger");
            if (rptGeneralLedger != null)
                Output($"Found {rptGeneralLedger.Rows.Length} rptGeneralLedger Rows", false);
            else
                Output("No rptGeneralLedger Found", false);

            foreach (var row in rptGeneralLedger.Rows)
            {
                string acct = "";
                string val = "";
                try
                {
                    acct = ((Intuit.Ipp.Data.Header)row.AnyIntuitObjects[0]).ColData[0].value;
                    val = ((Intuit.Ipp.Data.Summary)row.AnyIntuitObjects[2]).ColData[6].value;
                    if (String.IsNullOrEmpty(val))
                        val = "0.00";
                    try
                    {
                        if (isCurrentItem)
                            _items.Add(new SnapshotListItem() { AcctType = "LEDGER", Acct = acct, Value = Convert.ToDecimal(val) });
                        else
                            _prevperioditems.Add(new SnapshotListItem() { AcctType = "LEDGER", Acct = acct, Value = Convert.ToDecimal(val) });
                    }
                    catch (Exception exc)
                    {
                        Output("General Ledger: " + exc.Message, true);
                        throw exc;
                    }
                } catch (Exception exrow) {
                    Output($"Weird Ledger Row found {Newtonsoft.Json.JsonConvert.SerializeObject(row)}", false);
                }
            }


            QueryService<Purchase> inBalance = new QueryService<Purchase>(serviceContext);
            string bsheetqry = $"SELECT * FROM Purchase WHERE TxnDate >= '{startdt}' and TxnDate <= '{enddt}' Order by TxnDate DESC MAXRESULTS 1000";
            Output($"BSheet Query {bsheetqry}", false);
            var bsheet = inBalance.ExecuteIdsQuery(bsheetqry, QueryOperationType.query);

            if (bsheet != null)
                Output($"Found {bsheet.Count} bsheet Rows", false);
            else
                Output("No bsheet Found", false);

            foreach (var b in bsheet)
            {
                foreach (var l in b.Line)
                {
                    try
                    {
                        string acct = string.Empty;
                        if (l.AnyIntuitObject is Intuit.Ipp.Data.ItemBasedExpenseLineDetail)
                            acct = ((Intuit.Ipp.Data.ItemBasedExpenseLineDetail)l.AnyIntuitObject).ItemRef.name;
                        else if (l.AnyIntuitObject is Intuit.Ipp.Data.AccountBasedExpenseLineDetail)
                            acct = ((Intuit.Ipp.Data.AccountBasedExpenseLineDetail)l.AnyIntuitObject).AccountRef.name;
                        else
                            acct = "??" + l.AnyIntuitObject.ToString() + "??";
                        var val = l.Amount;

                        if (isCurrentItem)
                            _items.Add(new SnapshotListItem() { AcctType = "EXPENSE", Acct = acct, Value = Convert.ToDecimal(val) });
                        else
                            _prevperioditems.Add(new SnapshotListItem() { AcctType = "EXPENSE", Acct = acct, Value = Convert.ToDecimal(val) });
                    }
                    catch (Exception exrow)
                    {
                        Output($"Weird Purchase Row found {Newtonsoft.Json.JsonConvert.SerializeObject(l)}", false);
                    }
                }
            }


        }

        


        #region Dashboard Loading Methods

        private void LoadPredictableRevenue()
        {
            string[] labels = new string[] { "Recurring", "Non-Recurring", "Growth" };

            decimal recurringrev = _items.Where(x => x.AcctType == "PRODUCT" && _recurringSalesItems.Contains(x.Acct)).Sum(s => s.Value);
            decimal nonrecurringrev = _items.Where(x => x.AcctType == "PRODUCT" && !_recurringSalesItems.Contains(x.Acct)).Sum(s => s.Value);
            decimal growthrev = _items.Where(x => x.AcctType == "PRODUCT" && _growthSalesItem.Contains(x.Acct)).Sum(s => s.Value);

            decimal[] vals = new decimal[] { recurringrev, nonrecurringrev, growthrev };

            chartPredictableRevenue.Series[0].Points.DataBindXY(labels, vals);

            chartPredictableRevenue.Series[0].PostBackValue = "#SERIESNAME;#INDEX";
            chartPredictableRevenue.Series[0].BorderWidth = 10;
            chartPredictableRevenue.Series[0].ChartType = System.Web.UI.DataVisualization.Charting.SeriesChartType.Pie;
        }

        private void LoadProfitQuilt()
        {
            string[] labels = new string[] { "Expenses", "Revenue", "Profit" };

            decimal rev = _items.Where(x => x.AcctType == "PRODUCT").Sum(s => s.Value);
            decimal expense = _items.Where(x => x.AcctType == "EXPENSE").Sum(s => s.Value);
            decimal profit = rev - expense;

            decimal[] vals = new decimal[] { expense, rev, profit };

            chartProfitQuilt.Series[0].Points.DataBindXY(labels, vals);
            chartProfitQuilt.Series[0].Points[0].Color = System.Drawing.Color.Red;
            chartProfitQuilt.Series[0].Points[2].Color = System.Drawing.Color.Black;
            chartProfitQuilt.Series[0].BorderWidth = 10;
            chartRevenueGrowth.Series[0].LabelFormat = "{c}";
            chartRevenueGrowth.Series[0].IsVisibleInLegend = false;
            chartProfitQuilt.Series[0].ChartType = System.Web.UI.DataVisualization.Charting.SeriesChartType.Bar;
        }

        private void LoadProfitReality()
        {
            int up12 = 4;
            int up3 = 9;
            // TODO:  Get 3 and 12 month profit
            this.imgprofitreality.ImageUrl = $"ProfitReality.aspx?up12={up12}&up3={up3}";
        }

        private void LoadRevenueGrowth()
        {
            string label1 = "";
            string label2 = "";

            switch (cbgrowthcompare.SelectedValue)
            {
                case "LastYear":
                    label1 = "This Year";
                    label2 = "Last Year";
                    break;
                case "LastMonth":
                    label1 = "This Month";
                    label2 = "Last Month";
                    break; 
            }

            string[] labels = new string[] { label1, label2 };

            var numdays = (Convert.ToDateTime(dtend.Text) - Convert.ToDateTime(dtstart.Text)).TotalDays;
            
            decimal chartval1 = _items.Where(x => x.AcctType == "PRODUCT").Sum(s => s.Value);
            decimal chartval2 = _prevperioditems.Where(x => x.AcctType == "PRODUCT").Sum(s => s.Value);

            decimal[] vals = new decimal[] { chartval1/Convert.ToDecimal(numdays), chartval2 / Convert.ToDecimal(numdays) };

            chartRevenueGrowth.Series[0].Points.DataBindXY(labels, vals);
            chartRevenueGrowth.Series[0].Name = "Revenue per day";
            chartRevenueGrowth.Series[0].BorderWidth = 10;
            chartRevenueGrowth.Series[0].IsVisibleInLegend = false;
            chartRevenueGrowth.Series[0].ChartType = System.Web.UI.DataVisualization.Charting.SeriesChartType.Column;
        }

        private void LoadExpenseGrowth()
        {
            string label1 = "";
            string label2 = "";

            switch (cbgrowthcompare.SelectedValue)
            {
                case "LastYear":
                    label1 = "This Year";
                    label2 = "Last Year";
                    break;
                case "LastMonth":
                    label1 = "This Month";
                    label2 = "Last Month";
                    break;
            }

            string[] labels = new string[] { label1, label2 };

            var numdays = (Convert.ToDateTime(dtend.Text) - Convert.ToDateTime(dtstart.Text)).TotalDays;

            decimal chartval1 = _items.Where(x => x.AcctType == "EXPENSE").Sum(s => s.Value);
            decimal chartval2 = _prevperioditems.Where(x => x.AcctType == "EXPENSE").Sum(s => s.Value);

            decimal[] vals = new decimal[] { chartval1 / Convert.ToDecimal(numdays), chartval2 / Convert.ToDecimal(numdays) };

            chartExpenseGrowth.Series[0].Points.DataBindXY(labels, vals);
            chartExpenseGrowth.Series[0].Color = System.Drawing.Color.Red;
            chartExpenseGrowth.Series[0].Name = "Expense per day";
            chartExpenseGrowth.Series[0].BorderWidth = 10;
            chartExpenseGrowth.Series[0].IsVisibleInLegend = false;
            chartExpenseGrowth.Series[0].ChartType = System.Web.UI.DataVisualization.Charting.SeriesChartType.Column;
        }

        private void LoadProfitGrowth()
        {
            string label1 = "";
            string label2 = "";

            switch (cbgrowthcompare.SelectedValue)
            {
                case "LastYear":
                    label1 = "This Year";
                    label2 = "Last Year";
                    break;
                case "LastMonth":
                    label1 = "This Month";
                    label2 = "Last Month";
                    break;
            }

            string[] labels = new string[] { label1, label2 };

            var numdays = (Convert.ToDateTime(dtend.Text) - Convert.ToDateTime(dtstart.Text)).TotalDays;

            decimal chartval1rev = _items.Where(x => x.AcctType == "PRODUCT").Sum(s => s.Value);
            decimal chartval1exp = _items.Where(x => x.AcctType == "EXPENSE").Sum(s => s.Value);
            decimal chartval2rev = _prevperioditems.Where(x => x.AcctType == "PRODUCT").Sum(s => s.Value);
            decimal chartval2exp = _prevperioditems.Where(x => x.AcctType == "EXPENSE").Sum(s => s.Value);

            var profit1 = chartval1rev - chartval1exp;
            var profit2 = chartval2rev - chartval2exp;

            decimal[] vals = new decimal[] { profit1 / Convert.ToDecimal(numdays), profit2 / Convert.ToDecimal(numdays) };

            chartProfitGrowth.Series[0].Points.DataBindXY(labels, vals);
            chartProfitGrowth.Series[0].Name = "EBIT per day";
            chartProfitGrowth.Series[0].Color = System.Drawing.Color.Black;
            chartProfitGrowth.Series[0].BorderWidth = 10;
            chartProfitGrowth.Series[0].IsVisibleInLegend = false;
            chartProfitGrowth.Series[0].ChartType = System.Web.UI.DataVisualization.Charting.SeriesChartType.Column;
        }

        private void LoadRevExpenseRatio()
        {
            decimal chartval2rev = _items.Where(x => x.AcctType == "PRODUCT").Sum(s => s.Value);
            decimal chartval2exp = _items.Where(x => x.AcctType == "EXPENSE").Sum(s => s.Value);

            if (chartval2exp != decimal.Zero)
                lblratio.Text = (chartval2rev / chartval2exp).ToString("0.00");
            else
                lblratio.Text = "n/a";

            decimal chartval2rev_prev = _prevperioditems.Where(x => x.AcctType == "PRODUCT").Sum(s => s.Value);
            decimal chartval2exp_prev = _prevperioditems.Where(x => x.AcctType == "EXPENSE").Sum(s => s.Value);

            if (chartval2exp_prev != decimal.Zero)
                lblratio_prev.Text = (chartval2rev_prev / chartval2exp_prev).ToString("0.00");
            else
                lblratio_prev.Text = "n/a";

        }

        private void LoadRelativeHumidity()
        {
            //lbl_relhum_ebita_min
            decimal rev = _items.Where(x => x.AcctType == "PRODUCT").Sum(s => s.Value);
            decimal exp = _items.Where(x => x.AcctType == "EXPENSE").Sum(s => s.Value);
            decimal prof = decimal.Zero;
            if (exp != decimal.Zero)
                prof = rev - exp;


            lbl_relhum_ebita_min.Text = (prof * _sicCodeMults[0]).ToString("c0");
            lbl_relhum_ebita_mid.Text = (prof * _sicCodeMults[1]).ToString("c0");
            lbl_relhum_ebita_max.Text = (prof * _sicCodeMults[2]).ToString("c0");

            lbl_relhum_rev_min.Text = (rev * _sicCodeMults[0]).ToString("c0");
            lbl_relhum_rev_mid.Text = (rev * _sicCodeMults[1]).ToString("c0");
            lbl_relhum_rev_max.Text = (rev * _sicCodeMults[2]).ToString("c0");

            lbl_relhum_sic_min.Text = _sicCodeMults[0].ToString("0.00");
            lbl_relhum_sic_mid.Text = _sicCodeMults[1].ToString("0.00");
            lbl_relhum_sic_max.Text = _sicCodeMults[2].ToString("0.00");

            lblsic.Text = _sicCodeArray[0];
        }

        #endregion

        #region ChartClickEvents

        protected void chartPredictableRevenue_Click(object sender, System.Web.UI.WebControls.ImageMapEventArgs e)
        {
            int a = 1;
        }

        #endregion

        protected async void btnlogout_Click(object sender, EventArgs e)
        {
            dictionary = new Dictionary<string, string>();
            await oauthClient.RevokeTokenAsync("refreshToken");
            mainButtons.Visible = true;
            connected.Visible = false;

        }



    }

    public static class ResponseHelper
    {
        public static void Redirect(this HttpResponse response, string url, string target, string windowFeatures)
        {
            if ((String.IsNullOrEmpty(target) || target.Equals("_self", StringComparison.OrdinalIgnoreCase)) && String.IsNullOrEmpty(windowFeatures))
            {
                response.Redirect(url);
            }
            else
            {
                Page page = (Page)HttpContext.Current.Handler;
                if (page == null)
                {
                    throw new InvalidOperationException("Cannot redirect to new window outside Page context.");
                }
                url = page.ResolveClientUrl(url);
                string script;
                if (!String.IsNullOrEmpty(windowFeatures))
                {
                    script = @"window.open(""{0}"", ""{1}"", ""{2}"");";
                }
                else
                {
                    script = @"window.open(""{0}"", ""{1}"");";
                }
                script = String.Format(script, url, target, windowFeatures);
                ScriptManager.RegisterStartupScript(page, typeof(Page), "Redirect", script, true);
            }
        }
    }

    public class SnapshotListItem
    {
        public int id { get; set; }
        public string Acct { get; set; }
        public string AcctType { get; set; }
        public decimal Value { get; set; } = decimal.Zero;
    }
}
