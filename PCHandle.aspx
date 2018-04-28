<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PCHandle.aspx.cs" Inherits="PCHandle" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<asp:SqlDataSource ID="SqlDataSource1" runat="server" 
			ConnectionString="<%$ ConnectionStrings:ConnectionString %>" 
			DeleteCommand="DELETE FROM [PCS] WHERE [ID] = ?" 
			InsertCommand="INSERT INTO [PCS] ([ID], [PCNAME], [IP], [ETHERNETID]) VALUES (?, ?, ?, ?)" 
			ProviderName="System.Data.OleDb" 
			SelectCommand="SELECT * FROM [PCS]" 
			UpdateCommand="UPDATE [PCS] SET [PCNAME] = ?, [IP] = ?, [ETHERNETID] = ? WHERE [ID] = ?">
			<DeleteParameters>
				<asp:Parameter Name="ID" Type="Int32" />
			</DeleteParameters>
			<UpdateParameters>
				<asp:Parameter Name="PCNAME" Type="String" />
				<asp:Parameter Name="IP" Type="String" />
				<asp:Parameter Name="ETHERNETID" Type="String" />
				<asp:Parameter Name="ID" Type="Int32" />
			</UpdateParameters>
			<InsertParameters>
				<asp:Parameter Name="ID" Type="Int32" />
				<asp:Parameter Name="PCNAME" Type="String" />
				<asp:Parameter Name="IP" Type="String" />
				<asp:Parameter Name="ETHERNETID" Type="String" />
			</InsertParameters>
		</asp:SqlDataSource>    
    	<asp:Panel ID="Panel1" runat="server" HorizontalAlign="Center">
			&nbsp;&nbsp;&nbsp;
			<br/>
			<table style="width:100%;">
				<tr>
					<td width="30%">
						<asp:Button ID="ScanButton0" runat="server" onclick="ScanButton_Click" 
							Text="Scan For All PCs..." />
					</td>
					<td width="30%">
						<asp:Button ID="CheckButton0" runat="server" onclick="CheckButton_Click" 
							Text="Check PCs IN Table..." />
					</td>
					<td width="30%">
						<asp:Button ID="CheckButton1" runat="server" onclick="CheckButton1_Click" 
							Text="Check Ethernet IDs..." />
					</td>
				</tr>
			</table>
			<br/>
			<center>
			<asp:GridView ID="GridView1" runat="server" 
				AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="ID" 
				DataSourceID="SqlDataSource1" onrowdatabound="GridView1_RowDataBound">
				<RowStyle Font-Names="Arial" Font-Size="8pt" />
				<Columns>
					<asp:BoundField DataField="ID" HeaderText="ID" InsertVisible="False" 
						ReadOnly="True" SortExpression="ID" />
					<asp:BoundField DataField="PCNAME" HeaderText="PCNAME" 
						SortExpression="PCNAME" />
					<asp:BoundField DataField="IP" HeaderText="IP" SortExpression="IP" />
					<asp:BoundField DataField="ETHERNETID" HeaderText="ETHERNETID" 
						SortExpression="ETHERNETID" />
				</Columns>
				<HeaderStyle Font-Names="Arial" Font-Size="8pt" />
				<EditRowStyle Font-Names="Arial" Font-Overline="True" Font-Size="8pt" />
			</asp:GridView>
			</center>
		 </asp:Panel>
    
    </div>
    </form>
</body>
</html>
