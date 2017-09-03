<%@ Page Title="" Language="C#" MasterPageFile="~/BusWebFront.Master" AutoEventWireup="true"
    CodeBehind="AddNew.aspx.cs" Inherits="BusWeb.AddNew" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table width="100%">
        <tr>
            <td>
                <p>
                    選擇路線：<asp:DropDownList ID="ddlLine" runat="server" DataTextField="LineName" DataValueField="LineID">
                    </asp:DropDownList>
                </p>
                <p>
                    或</p>
                <p>
                    輸入新路線名稱：<asp:TextBox ID="txtNewLineName" runat="server"></asp:TextBox>
                </p>
                <p>
                    &nbsp;</p>
                <p>
                    輸入新站點名稱：<asp:TextBox ID="txtNewStopName" runat="server"></asp:TextBox>
                </p>
            </td>
            <td>
                <p>
                    輸入該點的 Google Map 網址：<asp:TextBox ID="txtGMapURL" runat="server" Width="100%"></asp:TextBox>
                </p>
                <p>
                    <asp:Button ID="btnSend" runat="server" OnClick="btnSend_Click" Text="儲存" />
                </p>
            </td>
        </tr>
    </table>
</asp:Content>
