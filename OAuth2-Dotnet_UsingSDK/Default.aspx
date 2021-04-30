<%@ Page Language="C#" Async="true" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ProfitProgress.web.Default"   %>
<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"  
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>  

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title></title>
        <% if (dictionary.ContainsKey("accessToken"))
            {
                Response.Write("<script> window.opener.location.reload();window.close(); </script>");
            }
        %> 

        <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css" integrity="sha384-ggOyR0iXCbMQv3Xipma34MD+dH/1fQ784/j6cY/iJTQUOhcWr7x9JvoRxT2MZw1T" crossorigin="anonymous" />
        <script src="https://code.jquery.com/jquery-3.3.1.slim.min.js" integrity="sha384-q8i/X+965DzO0rT7abK41JStQIAqVgRVzpbzo5smXKp4YfRvH+8abtTE1Pi6jizo" crossorigin="anonymous"></script>
        <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.7/umd/popper.min.js" integrity="sha384-UO2eT0CpHqdSJQ6hJty5KVphtPhzWj9WO1clHTMGa3JDZwrnQq4sF86dIHNDz0W1" crossorigin="anonymous"></script>
        <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/js/bootstrap.min.js" integrity="sha384-JjSmVgyd0p3pXB1rRibZUAYoIIy6OrQ6VrjIEaFf/nJGzIxFDsf4x0xIM+B07jRM" crossorigin="anonymous"></script>

    </head>
    <body class="container">
        <div class="row">
            <div class="col-xs-12">
                <form id="form1" runat="server">
                    <div>
                         <h3>Profit Progress</h3>
                        <p>&nbsp;</p>
                    </div>

                         <div id="mainButtons" runat="server" visible ="false">

                        <!-- Connect To QuickBooks Button -->
                        <b>To use this dashboard, you must first sign into QuickBooks</b><br />
                        <asp:ImageButton id="btnOAuth" runat="server" AlternateText="Connect to Quickbooks"
                               ImageAlign="left"
                               ImageUrl="Images/QBO/C2QB_green_btn_med_hover.png"
                               OnClick="ImgC2QB_Click" Height="40px" Width="200px"/>
                         <br />
                        <br />
                        <br />
                        <br /><br />
         

                        <asp:GridView runat="server" ID="GridView1">

                        </asp:GridView>

                        <br />
                        <br />

                       <!-- Get App Now -->
                        <br />
              
                    </div>
      
                    <div>
                        <asp:GridView runat="server" ID="dtdata">
                        </asp:GridView>
                    </div>

                    <div id="connected" runat="server">

                         <div class="form-group right ">
                            <asp:Button id="btnlogout" runat="server" Text="Disconnect from QuickBooks" OnClick="btnlogout_Click" CssClass="btn btn-danger btn-sm"/>
                        </div>

                        <div class="form-group form-inline">
                            <label for="dtstart" style="width: 150px">Start Date</label>
                            <asp:TextBox runat="server" ID="dtstart" Text="2020-11-01" TextMode="Date" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="form-group form-inline">
                            <label for="dtend" style="width: 150px">End Date</label>
                            <asp:TextBox runat="server" ID="dtend" Text="2020-11-01" TextMode="Date" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="form-group form-inline">
                            <label for="dtstart" style="width: 150px">Type</label>
                            <asp:DropDownList runat="server" ID="cbcashoraccrual" CssClass="form-control">
                                <asp:ListItem Text="Cash" Value="Cash"></asp:ListItem>
                                <asp:ListItem Text="Accrual" Value="Accrual"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="form-group form-inline">
                             <label for="dtstart" style="width: 150px">Comparison to</label>
                             <asp:DropDownList runat="server" ID="cbgrowthcompare" CssClass="form-control">
                                <asp:ListItem Text="Last Year" Value="LastYear"></asp:ListItem>
                                <asp:ListItem Text="Last Month" Value="LastMonth"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <asp:Button id="btnloaddashboard" runat="server" Text="Load Dashboard!" OnClick="btnloaddashboard_Click" CssClass="btn btn-primary"/>
                        </div>
                        
                
                    </div>




                    <div id="metrics" runat="server" visible="false">
                        <div class="row">
                            <div class="col-xs-4">
                                <div class="card">
                                    <div class="card-body">
                                    <h5 class="card-title">Predictable Revenue</h5>
                                    <h6 class="card-subtitle mb-2 text-muted"> </h6>
                                    <p class="card-text">
                                            <asp:Chart OnClick="chartPredictableRevenue_Click"  ID="chartPredictableRevenue" runat="server"  
                                                BorderlineWidth="0" Palette="SeaGreen" BorderlineColor="64, 0, 64"
                                                >  
                                                <Titles>  
                                                    <asp:Title ShadowOffset="10" Name="Items" />  
                                                </Titles>  
                                                <Legends>  
                                                    <asp:Legend Alignment="Center" Docking="Bottom" IsTextAutoFit="False" Name="Default"  
                                                        LegendStyle="Row" />  
                                                </Legends>  
                                                <Series>  
                                                    <asp:Series Name="Default" IsVisibleInLegend="false" />  
                                                </Series>  
                                                <ChartAreas>  
                                                    <asp:ChartArea Name="ChartArea1" BorderWidth="0" />  
                                                </ChartAreas>  
                                    </asp:Chart>  


                                    </p>
                                    <a href="#" class="card-link">More details</a>
                                    </div>
                                </div>
                            </div>
                            <div class="col-xs-4">
                                 <div class="card">
                                      <div class="card-body">
                                        <h5 class="card-title">Profit Quilt</h5>
                                        <h6 class="card-subtitle mb-2 text-muted"> </h6>
                                        <p class="card-text">
                                             <asp:Chart ID="chartProfitQuilt" runat="server" BorderlineWidth="1" Palette="SeaGreen" BorderlineColor="64, 0, 64" >  
                                                <Titles>  
                                                    <asp:Title ShadowOffset="10" Name="Items" />  
                                                </Titles>  
                                                <Legends>  
                                                    <asp:Legend Alignment="Center" Docking="Bottom" IsTextAutoFit="False" Name="Default"  
                                                        LegendStyle="Row" />  
                                                </Legends>  
                                                <Series>  
                                                    <asp:Series IsVisibleInLegend="false" />  
                                                </Series>  
                                                <ChartAreas>  
                                                    <asp:ChartArea Name="ChartArea1" BorderWidth="10" />  
                                                </ChartAreas>  
                                            </asp:Chart> 
                                        </p>
                                        <a href="#" class="card-link">More details</a>
                                      </div>
                                    </div>
                            </div>
                            <div class="col-xs-4">
                                 <div class="card">
                                      <div class="card-body">
                                        <h5 class="card-title">Profit Reality</h5>
                                        <h6 class="card-subtitle mb-2 text-muted"> </h6>
                                        <p class="card-text">
                                            <asp:Image runat="server" ID="imgprofitreality" Width="300px" Height="300px" />
                                        </p>
                                        <a href="#" class="card-link">More details</a>
                                      </div>
                                    </div>
                            </div>

                            <div class="col-xs-4">
                                 <div class="card">
                                      <div class="card-body">
                                        <h5 class="card-title">Revenue Growth</h5>
                                        <h6 class="card-subtitle mb-2 text-muted"> </h6>
                                        <p class="card-text">
                                             <asp:Chart ID="chartRevenueGrowth" runat="server" BorderlineWidth="0" Palette="SeaGreen" BorderlineColor="64, 0, 64">  
                                                <Titles>  
                                                    <asp:Title ShadowOffset="10" Name="Items" />  
                                                </Titles>  
                                                <Legends>  
                                                    <asp:Legend Alignment="Center" Docking="Bottom" IsTextAutoFit="False" Name="Default"  
                                                        LegendStyle="Row" />  
                                                </Legends>  
                                                <Series>  
                                                    <asp:Series Name="Default" />  
                                                </Series>  
                                                <ChartAreas>  
                                                    <asp:ChartArea Name="ChartArea1" BorderWidth="0" />  
                                                </ChartAreas>  
                                            </asp:Chart> 
                                        </p>
                                        <a href="#" class="card-link">More details</a>
                                      </div>
                                    </div>
                              </div>
                            <div class="col-xs-4">
                                 <div class="card">
                                      <div class="card-body">
                                        <h5 class="card-title">Expense Growth</h5>
                                       <h6 class="card-subtitle mb-2 text-muted"> </h6>
                                        <p class="card-text">
                                             <asp:Chart ID="chartExpenseGrowth" runat="server" BorderlineWidth="0" Palette="SeaGreen" BorderlineColor="64, 0, 64">  
                                                <Titles>  
                                                    <asp:Title ShadowOffset="10" Name="Items" />  
                                                </Titles>  
                                                <Legends>  
                                                    <asp:Legend Alignment="Center" Docking="Bottom" IsTextAutoFit="False" Name="Default"  
                                                        LegendStyle="Row" />  
                                                </Legends>  
                                                <Series>  
                                                    <asp:Series Name="Default" />  
                                                </Series>  
                                                <ChartAreas>  
                                                    <asp:ChartArea Name="ChartArea1" BorderWidth="0" />  
                                                </ChartAreas>  
                                            </asp:Chart> 
                                        </p>
                                        <a href="#" class="card-link">More details</a>
                                      </div>
                                    </div>
                              </div>
                            <div class="col-xs-4">
                                 <div class="card">
                                      <div class="card-body">
                                        <h5 class="card-title">EBIT Growth</h5>
                                        <h6 class="card-subtitle mb-2 text-muted"> </h6>
                                        <p class="card-text">
                                             <asp:Chart ID="chartProfitGrowth" runat="server" BorderlineWidth="0" Palette="SeaGreen" BorderlineColor="64, 0, 64">  
                                                <Titles>  
                                                    <asp:Title ShadowOffset="10" Name="Items" />  
                                                </Titles>  
                                                <Legends>  
                                                    <asp:Legend Alignment="Center" Docking="Bottom" IsTextAutoFit="False" Name="Default"  
                                                        LegendStyle="Row" />  
                                                </Legends>  
                                                <Series>  
                                                    <asp:Series Name="Default" />  
                                                </Series>  
                                                <ChartAreas>  
                                                    <asp:ChartArea Name="ChartArea1" BorderWidth="0" />  
                                                </ChartAreas>  
                                            </asp:Chart> 
                                        </p>
                                        <a href="#" class="card-link">More details</a>
                                      </div>
                                    </div>
                              </div>

                            <div class="col-xs-4">
                                <div class="card">
                                      <div class="card-body">
                                        <h5 class="card-title">Revenue to Expense Ratio (This Term)</h5>
                                        <h6 class="card-subtitle mb-2 text-muted"> </h6>
                                        <p class="card-text">
                                             <asp:Label runat="server" Width="300px" ID="lblratio" Font-Size="82" ForeColor="Green" Text="1.34"></asp:Label>
                                        </p>
                                        <a href="#" class="card-link">More details</a>
                                      </div>
                                    </div>
                            </div>
                            <div class="col-xs-4">
                                <div class="card">
                                      <div class="card-body">
                                        <h5 class="card-title">Revenue to Expense Ratio (Last Term)</h5>
                                        <h6 class="card-subtitle mb-2 text-muted"> </h6>
                                        <p class="card-text">
                                             <asp:Label runat="server" Width="300px" ID="lblratio_prev" Font-Size="82" ForeColor="LightGreen" Text="1.34"></asp:Label>
                                        </p>
                                        <a href="#" class="card-link">More details</a>
                                      </div>
                                    </div>
                            </div>
                            <div class="col-xs-12" style="min-width: 600px">
                                <div class="card">
                                      <div class="card-body">
                                        <h5 class="card-title">Relative Humidity Check</h5>
                                        <h6 class="card-subtitle mb-2 text-muted"> </h6>
                                         <table class="table" style="padding: 2px">
                                                 <thead>
                                                     <tr>
                                                         <td>Measurement</td>
                                                         <td>Min</td>
                                                         <td>Mid</td>
                                                         <td>Max</td>
                                                     </tr>
                                                 </thead>
                                                 <tbody>
                                                     <tr>
                                                         <td>By EBITA</td>
                                                         <td><asp:Label runat="server" ID="lbl_relhum_ebita_min" Font-Size="12" Text="1.34"></asp:Label></td>
                                                         <td><asp:Label runat="server" ID="lbl_relhum_ebita_mid" Font-Size="12" Text="1.34"></asp:Label></td>
                                                         <td><asp:Label runat="server" ID="lbl_relhum_ebita_max" Font-Size="12" Text="1.34"></asp:Label></td>
                                                     </tr>
                                                     <tr>
                                                         <td>By Revenue</td>
                                                         <td><asp:Label runat="server" ID="lbl_relhum_rev_min" Font-Size="12" Text="1.34"></asp:Label></td>
                                                         <td><asp:Label runat="server" ID="lbl_relhum_rev_mid" Font-Size="12" Text="1.34"></asp:Label></td>
                                                         <td><asp:Label runat="server" ID="lbl_relhum_rev_max" Font-Size="12" Text="1.34"></asp:Label></td>
                                                     </tr>
                                                     <tr>
                                                         <td>By SIC <asp:Label runat="server" ID="lblsic" Font-Size="12" /></td>
                                                         <td><asp:Label runat="server" ID="lbl_relhum_sic_min" Font-Size="12" Text="1.34"></asp:Label></td>
                                                         <td><asp:Label runat="server" ID="lbl_relhum_sic_mid" Font-Size="12" Text="1.34"></asp:Label></td>
                                                         <td><asp:Label runat="server"  ID="lbl_relhum_sic_max" Font-Size="12" Text="1.34"></asp:Label></td>
                                                     </tr>
                                                 </tbody>
                                             </table>
                                      </div>
                                    </div>
                            </div>

                        </div> <!-- end of row -->
                        <hr />

              
                        <div>
                            <div>
                                <h4>List of Items pulled (development only)</h4>
                                <asp:DataList runat="server" ID="dlitems" RepeatLayout="Table">
                                    <ItemTemplate>
                                        <table class="table" style="width: 800px">  
                                            <tr>  
                                                <td style="min-width: 400px">  
                                                    <%# Eval("Acct") %>  
                                                </td>
                                                <td style="min-width: 100px">  
                                                    <%# Eval("AcctType") %>
                                                </td>  
                                                <td style="min-width: 100px">  
                                                    <%# Eval("Value") %> 
                                                </td>  
                                            </tr>  
                                        </table>  
                                    </ItemTemplate>
                                </asp:DataList>
                                <h4>Log</h4>
                                <asp:TextBox TextMode="MultiLine" Rows="40" Columns="250" runat="server" ID="txtlog" Visible="true"></asp:TextBox>
                            </div>
                            <div>
                                <h4>List of Previous Period Items pulled (development only)</h4>
                                <asp:DataList runat="server" ID="dlitemsprev" RepeatLayout="Table">
                                    <ItemTemplate>
                                        <table class="table" style="width: 800px">  
                                            <tr>  
                                                <td style="min-width: 400px">  
                                                    <%# Eval("Acct") %>  
                                                </td>
                                                <td style="min-width: 100px">  
                                                    <%# Eval("AcctType") %>
                                                </td>  
                                                <td style="min-width: 100px">  
                                                    <%# Eval("Value") %> 
                                                </td>  
                                            </tr>  
                                        </table>  
                                    </ItemTemplate>
                                </asp:DataList>
                                <h4>Log</h4>
                                <asp:TextBox TextMode="MultiLine" Rows="40" Columns="250" runat="server" ID="TextBox1" Visible="true"></asp:TextBox>
                            </div>
                        </div>
                    </div> <!-- end of metrics -->


        


                </form>
            </div>
        </div>
        
        <% if (dictionary.ContainsKey("accessToken"))
            {
                Response.Write("<script> window.opener.location.reload();window.close(); </script>");
            }
        %> 
       

    </body>
</html>