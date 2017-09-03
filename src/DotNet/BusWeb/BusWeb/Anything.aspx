<%@ Page Title="" Language="C#" MasterPageFile="~/BusWebFront.Master" AutoEventWireup="true"
    CodeBehind="Anything.aspx.cs" Inherits="BusWeb.Anything" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table width="100%" border="0">
        <tr>
            <td valign="top" align="left" width="0%">
                指令：
            </td>
            <td width="100%">
                <asp:TextBox ID="txtCmd" runat="server" Rows="20" TextMode="MultiLine" Width="100%"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                受影響資料量：
            </td>
            <td align="center">
                <asp:Label ID="lblRowCount" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="2" align="center">
                <asp:Button ID="btnRun" runat="server" Text="Execute" OnClick="btnRun_Click" OnClientClick="if (confirm('可能有不可挽回的可怕後果, 確定要執行嗎?') == false) return false;" />
                &nbsp;<asp:Button ID="btnQry" runat="server" OnClick="btnQry_Click" Text="Query" />
            </td>
        </tr>
        <tr>
            <td valign="top">
                資料內容：
            </td>
            <td align="center">
                &nbsp;
                <asp:GridView ID="gvData" runat="server" BackColor="LightGoldenrodYellow" 
                    BorderColor="Tan" BorderWidth="1px" CellPadding="2" ForeColor="Black" 
                    GridLines="None" Width="100%">
                    <FooterStyle BackColor="Tan" />
                    <PagerStyle BackColor="PaleGoldenrod" ForeColor="DarkSlateBlue" 
                        HorizontalAlign="Center" />
                    <SelectedRowStyle BackColor="DarkSlateBlue" ForeColor="GhostWhite" />
                    <HeaderStyle BackColor="Tan" Font-Bold="True" />
                    <AlternatingRowStyle BackColor="PaleGoldenrod" />
                </asp:GridView>
            </td>
        </tr>
    </table>
</asp:Content>
