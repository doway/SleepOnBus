<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BusLogin.aspx.cs" Inherits="BusWeb.BusLogin" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        密碼：<asp:TextBox ID="txtPwd" runat="server" TextMode="Password"></asp:TextBox>
        <asp:Button ID="btnLogin" runat="server" onclick="btnLogin_Click" Text="登入" />
    
    </div>
    </form>
</body>
</html>
