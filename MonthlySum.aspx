<%@ Page Language="C#" AutoEventWireup="true" CodeFile="MonthlySum.aspx.cs" Inherits="MonthlySum" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    	<asp:Panel ID="Panel1" runat="server" HorizontalAlign="Center">
			<asp:Button ID="PrevButton" runat="server" 
	Text="( &lt;&lt; ) Prev. Month" onclick="PrevButton_Click" />
			<asp:Button ID="ThisButton" runat="server" 
	Text="This Month" onclick="ThisButton_Click" />
			<asp:Button ID="NextButton" runat="server" 
	Text="Next Month ( &gt;&gt; )" onclick="NextButton_Click" />
		 </asp:Panel>
		 <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
			 ConnectionString="<%$ ConnectionStrings:ConnectionString %>" 
			 ProviderName="System.Data.OleDb" 
			 SelectCommand="SELECT [PERSON_NAME], [REPORT_DATE], (TIME_END - TIME_START) AS DURATION FROM GTO">
		 </asp:SqlDataSource>
		 <br />
		<center>
		   <asp:Label ID="TitleLabel" runat="server" Font-Names="arial" Font-Size="15pt" 
				Text="Label"></asp:Label>
			<br />
			<br />
		 <asp:GridView ID="GridView1" runat="server" AllowSorting="True" 
				DataSourceID="SqlDataSource1" style="margin-right: 0px" 
				onrowdatabound="GridView1_RowDataBound" AutoGenerateColumns="False">
			 <RowStyle Font-Names="arial" Font-Size="10pt" />
			 <Columns>
				 <asp:TemplateField HeaderText="#">
					  <ItemTemplate>
						 <%# Container.DataItemIndex + 1 %>
					  </ItemTemplate>				 
				 </asp:TemplateField>
				 <asp:BoundField DataField="NAME" HeaderText="NAME" />
				 <asp:BoundField DataField="DAYS" DataFormatString="{0:F3}" HeaderText="DAYS" />
			 	 <asp:BoundField DataField="HOURS" DataFormatString="{0:F3}" HeaderText="HOURS" />
			 </Columns>
			 <HeaderStyle Font-Names="arial" Font-Size="10pt" />
		 </asp:GridView>
		</center>
    </div>
    </form>
</body>
</html>
