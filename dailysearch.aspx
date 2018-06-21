<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DailySearch.aspx.cs" Inherits="DailySearch" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
	 <script type="text/javascript">
	 	function expandall() {
	 		var inputs, index;
	 		inputs = document.getElementsByTagName('button');
	 		for (index = 0; index < inputs.length; ++index) {
	 			if (inputs[index].id.substring(0, 4) == "btn_")
	 				inputs[index].style.display='none';
	 		}
	 		inputs = document.getElementsByTagName('div');
	 		for (index = 0; index < inputs.length; ++index) {
	 			if (inputs[index].id.substring(0, 6) == "child_")
	 				inputs[index].style.display = 'inline';
	 		} 	
	 	}
	 </script>
</head>
<body>
    <form id="form1" defaultfocus="SearchText" runat="server">
    <div>
    
    	<br />
		 <asp:Panel ID="Panel2" runat="server" HorizontalAlign="Center">
			 <table style="width:100%;">
				 <tr>
					 <td>
						 <br />
					 </td>
					 <td>
						 <asp:Image ID="Image1" runat="server" ImageUrl="~/IMAGES/search.PNG" />
					 </td>
					 <td>
						 &nbsp;</td>
				 </tr>
			 </table>
			 <br />
			 	<asp:Label ID="Label2" runat="server" Font-Names="Arial" Font-Size="10pt" 
				 Text="Type:"></asp:Label>
			 	<asp:DropDownList ID="OptionsList" runat="server" Width="150pt">
					<asp:ListItem>Day</asp:ListItem>
					<asp:ListItem>Tomorrow</asp:ListItem>
					<asp:ListItem>All</asp:ListItem>
				 </asp:DropDownList>
				 <asp:Label ID="Label3" runat="server" Font-Names="Arial" Font-Size="10pt" 
				 Text="User:"></asp:Label>
				 <asp:DropDownList ID="PersonsList" runat="server" Width="150pt">
				 </asp:DropDownList>
			 <asp:Label ID="Label4" runat="server" Font-Names="Arial" Font-Size="10pt" 
				 Text="Text:"></asp:Label>
			 <asp:TextBox ID="SearchText" runat="server" Width="340px" 
				 ontextchanged="SearchText_TextChanged"></asp:TextBox>
			 <asp:Button ID="SearchButton" runat="server" 
	Text="Search" onclick="SearchButton_Click" />
		 	 <br />
		 </asp:Panel>
								<input id="ExpandButton" onclick = "expandall();" type="button" value="Expand all..." /><br />
    
    </div>
		 <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
			 ConnectionString="<%$ ConnectionStrings:ConnectionString %>" 
			 ProviderName="System.Data.OleDb" 
			 
		 
		 SelectCommand="SELECT REPORTS.TIME_START, REPORTS.TIME_END, REPORTS.REPORT_DATE, REPORTS.REPORT_DONE, REPORTS.REPORT_FUTURE, PERSONS.PERSON_NAME FROM PERSONS , REPORTS WHERE PERSONS.PERSON_ID = REPORTS.PERSON_ID AND ( REPORTS.REPORT_DONE LIKE '%memo%' OR REPORTS.REPORT_FUTURE LIKE '%memo%')"></asp:SqlDataSource>
		 <center>
		 <asp:GridView ID="SearchGridView" runat="server" AutoGenerateColumns="False" 
			 DataSourceID="SqlDataSource1" AllowPaging="True" AllowSorting="True" 
			 onrowdatabound="SearchGridView_RowDataBound" PageSize="30">
			 <PagerSettings Mode="NumericFirstLast" Position="TopAndBottom" />
			 <RowStyle Font-Names="Arial" Font-Size="10pt" />
			 <Columns>
				 <asp:TemplateField HeaderText="#"></asp:TemplateField>
				 <asp:BoundField DataField="PERSON_NAME" HeaderText="PERSON_NAME" 
					 SortExpression="PERSON_NAME" />
				 <asp:TemplateField HeaderText="REPORT_DATE" SortExpression="REPORT_DATE">
					 <EditItemTemplate>
						 <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("REPORT_DATE") %>'></asp:TextBox>
					 </EditItemTemplate>
					 <ItemTemplate>
						 <asp:Label ID="Label1" runat="server" Text='<%# Bind("REPORT_DATE") %>'></asp:Label>
					 </ItemTemplate>
				 </asp:TemplateField>
				 <asp:BoundField DataField="REPORT_DONE" HeaderText="REPORT_DONE" 
					 SortExpression="REPORT_DONE" />
				 <asp:BoundField DataField="REPORT_FUTURE" HeaderText="REPORT_FUTURE" 
					 SortExpression="REPORT_FUTURE" />
			 </Columns>
		 	 <PagerStyle BackColor="#3399FF" Font-Names="Arial" Font-Size="10pt" 
				 HorizontalAlign="Center" VerticalAlign="Middle" />
		 	<HeaderStyle Font-Names="Arial" Font-Size="10pt" />
		 </asp:GridView>
		 </center>
    </form>
</body>
</html>
