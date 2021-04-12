<%@ Page Language="C#" Async="true" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="OAuth2_Dotnet_UsingSDK.Default" %>
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
    </head>
    <body>
        <form id="form1" runat="server">
            <div>
                 <h3>Profit Progress powered by Mach1</h3>
                <p>&nbsp;</p>
            </div>

                 <div id="mainButtons" runat="server" visible ="false">
            <!-- Sign In With Intuit Button -->
                <br />
                <asp:ImageButton id="btnSIWI" runat="server" AlternateText="Sign In With Intuit"
                   ImageAlign="left"
                   ImageUrl="Images/IntuitSignIn-lg-white@2x.jpg"
                   OnClick="ImgOpenId_Click" Height="40px" Width="200px" Visible="False"/>
                <br /><br /><br />

                <!-- Connect To QuickBooks Button -->
                <b>To use this dashboard, you must first sign into QuickBooks</b><br />
                <asp:ImageButton id="btnOAuth" runat="server" AlternateText="Connect to Quickbooks"
                       ImageAlign="left"
                       ImageUrl="Images/C2QB_white_btn_lg_default.png"
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
               <asp:ImageButton id="btnOpenId" runat="server" AlternateText="Get App Now"
                       ImageAlign="left"
                       ImageUrl="Images/Get_App.png"
                       OnClick="ImgGetAppNow_Click" CssClass="font-size:14px; border: 1px solid grey; padding: 10px; color: red" Height="40px" Width="200px" Visible="False"/>
                 <br /><br /><br />
            </div>
      
            <div>
                <asp:GridView runat="server" ID="dtdata" Visible="false">
                </asp:GridView>
            </div>

            <div id="connected" runat="server">

                <table>
                    <tr>
                        <td><asp:Label runat="server" Text="Start Date"></asp:Label></td>
                        <td><asp:TextBox runat="server" ID="dtstart" Text="2020-11-01" TextMode="Date"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td><asp:Label  runat="server" Text="End Date"></asp:Label></td>
                        <td><asp:TextBox runat="server" ID="dtend" Text="2020-12-3" TextMode="Date"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td><asp:Label  runat="server" Text="Type"></asp:Label></td>
                        <td> 
                            <asp:DropDownList runat="server" ID="cbcashoraccrual">
                                <asp:ListItem Text="Cash" Value="Cash"></asp:ListItem>
                                <asp:ListItem Text="Accrual" Value="Accrual"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>Comparison to</td>
                        <td>
                            <asp:DropDownList runat="server" ID="cbgrowthcompare">
                                <asp:ListItem Text="Last Year" Value="LastYear"></asp:ListItem>
                                <asp:ListItem Text="Last Month" Value="LastMonth"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Button id="btninsertstmt" runat="server" Text="Load Dashboard!" OnClick="btninsertstmt_Click"/>
                        </td>
                    </tr>
                </table>

                
            </div>


            <div id="metrics" runat="server" visible="false">

                <table style="width: 1040px">
                    <tr>
                        <td>
                            <h3>Predictable Revenue</h3>
                            <asp:Chart ID="chartPredictableRevenue" runat="server"  
                                BorderlineWidth="0" Height="360px" Palette="SeaGreen"
                                Width="380px" BorderlineColor="64, 0, 64"
                                >  
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
                        </td>
                        <td>
                             <h3>Profit Quilt</h3>
                             <asp:Chart ID="chartProfitQuilt" runat="server"  
                                BorderlineWidth="0" Height="360px" Palette="SeaGreen"
                                Width="380px" BorderlineColor="64, 0, 64"
                                >  
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
                        </td>
                        <td>
                            <h3>Profit Reality</h3>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <h3>Revenue Growth</h3>
                             <asp:Chart ID="chartRevenueGrowth" runat="server"  
                                BorderlineWidth="0" Height="360px" Palette="SeaGreen"
                                Width="380px" BorderlineColor="64, 0, 64"
                                >  
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
                        </td>
                        <td>
                            <h3>Expense Growth</h3>
                        </td>
                        <td>
                            <h3>EBIT Growth</h3>
                        </td>
                    </tr>
                </table>

              
                
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
                    <asp:TextBox TextMode="MultiLine" Rows="40" Columns="250" runat="server" ID="txtlog" Visible="false"></asp:TextBox>
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
                    <asp:TextBox TextMode="MultiLine" Rows="40" Columns="250" runat="server" ID="TextBox1" Visible="false"></asp:TextBox>
                </div>
            </div>



        


        </form>

       

    </body>
</html>