<%@ Page Language="C#" MasterPageFile="~/BoilerPlateWeb.Master" AutoEventWireup="true" CodeBehind="CRUD.aspx.cs" Inherits="BoilerPlateWeb.CRUD" %>
<asp:Content ID="content" ContentPlaceHolderID="cph" runat="server">
    <div>
        <asp:Label ID="alertL" runat="server" ForeColor="Red" Font-Bold="true" />
    </div>
    <div>
        <h2>Notifications</h2>
    </div>
    <div>
        <asp:GridView ID="gridview" runat="server" Width="100px" >
        </asp:GridView>
    </div>
    <br />
    <br />
    <div>
        <h3>New:</h3>
        <asp:Table ID="addNewTbl" runat="server" />
    </div>
    <div>
        <asp:Button ID="addB" runat="server" Text="Add" OnClick="addB_Click" CssClass="Button" />
    </div>
</asp:Content>