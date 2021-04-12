<%@ Page Language="C#" Async="true" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="OAuth2_Dotnet_UsingSDK.Default" %>
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

                <asp:Calendar runat="server" ID="dtstart" SelectedDate="1/1/2018" SelectionMode="Day"></asp:Calendar>
                <asp:Calendar runat="server" ID="dtend" SelectedDate="12/31/2021" SelectionMode="Day"></asp:Calendar>


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
                <b>Connect To QuickBooks</b><br />
                <asp:ImageButton id="btnOAuth" runat="server" AlternateText="Connect to Quickbooks"
                       ImageAlign="left"
                       ImageUrl="Images/C2QB_white_btn_lg_default.png"
                       OnClick="ImgC2QB_Click" Height="40px" Width="200px"/>
                 <br />
                <br />
                <br />
                <br /><br />
         

                <asp:GridView runat="server" ID="dtdata">

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

            <div id="connected" runat="server" visible ="false">
                <p><asp:label runat="server" id="lblConnected" visible="false">"Your application is connected!"</asp:label></p> 
                 
                <asp:TextBox ID="txtrpt" runat="server" Visible="False">GeneralLedger</asp:TextBox>
                <asp:Button id="btnQBOAPICall" runat="server" Text="Get QBO Data from API" OnClick="btnQBOAPICall_Click" Visible="False"/>
                 <br />

                  
                <asp:Button id="btninsertstmt" runat="server" Text="Get Data!" OnClick="btninsertstmt_Click"/>
                
                <p><asp:label runat="server" id="lblQBOCall" visible="false"></asp:label></p>
                <br /><br />

                 <asp:Button id="btnUserInfo" runat="server" Text="Get User Info" OnClick="btnUserInfo_Click" Visible="False" />
                <br />

                 <p><asp:label runat="server" id="lblUserInfo" visible="false"></asp:label></p>
                <br /><br />

                 <asp:Button id="btnRefresh" runat="server" Text="Refresh Tokens" OnClick="btnRefresh_Click" Visible="False"/>
                <br /><br /><br />

                <asp:Button id="btnRevoke" runat="server" Text="Revoke Tokens"  OnClick="btnRevoke_Click" Visible="False"/>
                <br /><br /><br /> 
            </div>


        <asp:TextBox TextMode="MultiLine" Rows="40" Columns="250" runat="server" ID="txtlog" Visible="False"></asp:TextBox>


        </form>

       

    </body>
</html>