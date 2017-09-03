<%@ Page Title="" Language="C#" MasterPageFile="~/BusWebFront.Master" AutoEventWireup="true"
    CodeBehind="DataManager.aspx.cs" Inherits="BusWeb.DataManager" %>

<%@ Register Src="PageDatePicker.ascx" TagName="PageDatePicker" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="lblServerRootPath" runat="server" Text="Server Path"></asp:Label>
    <table width="100%">
        <tr>
            <td valign="top" align="left" width="40%">
                <table width="100%" cellpadding="0" cellspacing="0" border="0">
                    <tr>
                        <td>
                            站點建立日期在：<table width="100%" cellpadding="0" cellspacing="0" border="0">
                                <tr>
                                    <td>
                                        <uc1:PageDatePicker ID="bgnDatePicker" runat="server" />
                                    </td>
                                    <td>
                                        和
                                    </td>
                                    <td>
                                        <uc1:PageDatePicker ID="endDatePicker" runat="server" />
                                    </td>
                                    <td>
                                        之間
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            單頁資料數：<asp:TextBox ID="txtPageSize" runat="server">20</asp:TextBox>
                            <asp:RangeValidator ID="rvNumber" runat="server" ControlToValidate="txtPageSize"
                                Display="Dynamic" ErrorMessage="必須是 1~500的數字" MaximumValue="500" MinimumValue="1"
                                Type="Integer"></asp:RangeValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnQry" runat="server" OnClick="btnQry_Click" Text="依條件查詢" />
                        </td>
                    </tr>
                </table>
                <div id="gv1" style="height: 600px; overflow: auto">
                    <asp:UpdatePanel ID="updPnl01" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CellPadding="4"
                                ForeColor="#333333" GridLines="None" OnRowCancelingEdit="GridView1_RowCancelingEdit"
                                OnRowCommand="GridView1_RowCommand" OnRowDataBound="GridView1_RowDataBound" OnRowDeleting="GridView1_RowDeleting"
                                OnRowEditing="GridView1_RowEditing" OnRowUpdating="GridView1_RowUpdating" AllowPaging="True"
                                AllowSorting="True" OnPageIndexChanging="GridView1_PageIndexChanging" OnSorting="GridView1_Sorting"
                                PageSize="20">
                                <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
                                <Columns>
                                    <asp:ButtonField ButtonType="Button" DataTextField="LineID" HeaderText="編號" CommandName="ID"
                                        SortExpression="LineID" />
                                    <asp:BoundField DataField="LineName" HeaderText="路線名稱" SortExpression="LineName" />
                                    <asp:BoundField DataField="DateCreated" DataFormatString="{0:yyyyMMdd HH:mm:ss}"
                                        HeaderText="建檔日期" ReadOnly="True" SortExpression="DateCreated" />
                                    <asp:BoundField DataField="StopCount" HeaderText="站點數量" ReadOnly="True" SortExpression="StopCount">
                                        <ItemStyle HorizontalAlign="Right" />
                                    </asp:BoundField>
                                    <asp:CommandField ButtonType="Button" ShowEditButton="True" />
                                    <asp:CommandField ButtonType="Button" ShowDeleteButton="True" />
                                </Columns>
                                <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                                <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                                <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
                                <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                                <AlternatingRowStyle BackColor="White" />
                            </asp:GridView>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="GridView1" EventName="RowDeleting" />
                            <asp:AsyncPostBackTrigger ControlID="GridView1" EventName="Sorting" />
                            <asp:AsyncPostBackTrigger ControlID="GridView1" EventName="PageIndexChanging" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </td>
            <td valign="top" align="left" width="60%">
                <div id="gv2" style="height: 600px; overflow: auto">
                    <asp:UpdatePanel ID="updPnl02" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:Label ID="lblParent" runat="server"></asp:Label>
                            <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="False" BackColor="White"
                                BorderColor="#E7E7FF" BorderStyle="None" BorderWidth="1px" CellPadding="3" GridLines="Horizontal"
                                OnRowCancelingEdit="GridView2_RowCancelingEdit" OnRowCommand="GridView2_RowCommand"
                                OnRowDataBound="GridView2_RowDataBound" OnRowDeleting="GridView2_RowDeleting"
                                OnRowEditing="GridView2_RowEditing" OnRowUpdating="GridView2_RowUpdating" AllowSorting="True"
                                OnSorting="GridView2_Sorting">
                                <RowStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" />
                                <Columns>
                                    <asp:BoundField DataField="StopID" HeaderText="編號" SortExpression="StopID" />
                                    <asp:BoundField DataField="StopName" HeaderText="站點名稱" SortExpression="StopName" />
                                    <asp:ButtonField ButtonType="Link" CommandName="Latitude" DataTextField="Latitude"
                                        HeaderText="緯度" />
                                    <asp:ButtonField ButtonType="Link" CommandName="Longitude" DataTextField="Longitude"
                                        HeaderText="經度" />
                                    <asp:ButtonField CommandName="CreatorLatitude" DataTextField="CreatorLatitude" HeaderText="作者緯度" />
                                    <asp:ButtonField CommandName="CreatorLongitude" DataTextField="CreatorLongitude"
                                        HeaderText="作者經度" />
                                    <asp:BoundField DataField="DateCreated" DataFormatString="{0:yyyyMMdd HH:mm:ss}"
                                        HeaderText="建立日期" SortExpression="DateCreated" />
                                    <asp:BoundField DataField="Owner" HeaderText="所有人" ReadOnly="True" SortExpression="Owner" />
                                    <asp:BoundField HeaderText="正分數" DataField="RatingGood" SortExpression="RatingGood" />
                                    <asp:BoundField HeaderText="負分數" DataField="RatingBad" SortExpression="RatingBad" />
                                    <asp:CommandField ButtonType="Button" ShowEditButton="True" />
                                    <asp:CommandField ButtonType="Button" ShowDeleteButton="True" />
                                </Columns>
                                <FooterStyle BackColor="#B5C7DE" ForeColor="#4A3C8C" />
                                <PagerStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" HorizontalAlign="Right" />
                                <SelectedRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="#F7F7F7" />
                                <HeaderStyle BackColor="#4A3C8C" Font-Bold="True" ForeColor="#F7F7F7" />
                                <AlternatingRowStyle BackColor="#F7F7F7" />
                            </asp:GridView>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="GridView1" EventName="RowCommand" />
                            <asp:AsyncPostBackTrigger ControlID="GridView2" EventName="RowDeleting" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
                <asp:UpdatePanel ID="updPnl03" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <p>
                            站點的地圖：</p>
                        <p>
                            <asp:Literal ID="litMapPreview" runat="server"></asp:Literal></p>
                        <p>
                            作者的地圖：</p>
                        <p>
                            <asp:Literal ID="litCreatorMapPreview" runat="server"></asp:Literal></p>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="GridView2" EventName="RowCommand" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>
</asp:Content>
