<%@ Page Language="C#"  MasterPageFile="~/BoilerPlateWeb.Master" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="BoilerPlateWeb.Login" %>

<asp:Content ID="content" ContentPlaceHolderID="cph" runat="server">
    <div "ChildMainDiv">
        <div>
            <h1>ASL Reporting Portal Login</h1>
        </div>
        <div>
            <asp:Label runat="server" ID="alertLabel" ForeColor="Red" Font-Bold="true" />
            <table>
                <tr>
                    <td>
                        User ID:
                    </td>
                    <td>
                        <asp:TextBox ID="userTB" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                        Password:
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="pwdTB" TextMode="Password" />
                    </td>
                </tr>
            </table>
        </div>
        <div>
             <asp:Button ID="loginButton" runat="server" Text="Sign In" OnClick="loginButton_Click" />
        </div>
    </div>
</asp:Content>