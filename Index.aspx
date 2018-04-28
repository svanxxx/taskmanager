<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Index.aspx.cs" Inherits="_Default" ValidateRequest="False" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Hi! What are you working on right now???</title>
    <link rel="stylesheet" type="text/css" href="TR.css" />
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
    <form id="form1" runat="server">
    <div>
    	 <table style="width:100%;">
			 <tr>
				 <td
					 style="font-family: Arial; font-size: x-large; text-align: center;" 
					 valign="top" colspan="2">
					 <asp:Panel ID="Panel2" runat="server" Height="25px" 
						 HorizontalAlign="Center" CssClass="textpanel">
						 <asp:Label ID="Label2" runat="server" 
	Text="What are you working on right now?" Font-Names="Arial" Font-Size="Large"></asp:Label>
					 </asp:Panel>
				 </td>
			 </tr>
			 <tr>
				 <td rowspan="2" align="center" valign="middle" 
					 style="width: 100px">
		 <asp:Calendar ID="Calendar1" runat="server" Font-Names="Verdana" 
			 Font-Size="8pt" BackColor="White" BorderColor="#3366CC" BorderWidth="1px" CellPadding="1" 
						 DayNameFormat="Shortest" ForeColor="#003399" Height="139px" 
						 onselectionchanged="Calendar1_SelectionChanged" 
						 Width="181px">
			 <SelectedDayStyle BackColor="#009999" Font-Bold="True" ForeColor="#CCFF99" />
			 <SelectorStyle BackColor="#99CCCC" ForeColor="#336666" />
			 <WeekendDayStyle BackColor="#CCCCFF" />
			 <TodayDayStyle BackColor="#99CCCC" ForeColor="White" />
			 <OtherMonthDayStyle ForeColor="#999999" />
			 <NextPrevStyle Font-Size="8pt" ForeColor="#CCCCFF" />
			 <DayHeaderStyle BackColor="#99CCCC" ForeColor="#336666" Height="1px" />
			 <TitleStyle BackColor="#003399" BorderColor="#3366CC" BorderWidth="1px" 
				 Font-Bold="True" Font-Size="10pt" ForeColor="#CCCCFF" Height="25px" />
					 </asp:Calendar>
    				 <br />
					 <asp:HyperLink ID="SearchLink" runat="server" NavigateUrl="~/DailySearch.aspx" 
						 Font-Names="Arial" Font-Size="8pt">Search...</asp:HyperLink>
    			</td>
				 <td valign="top" style="width: 100%">
		 			 <asp:Button ID="BackButton" runat="server" onclick="BackButton_Click" 
						 Text="&lt;&lt; Back" CssClass="button" />
					 <asp:Button ID="TodayButton" runat="server" onclick="TodayButton_Click" 
						 Text="Today!" CssClass="button" />
					 <asp:Button ID="NextButton" runat="server" onclick="NextButton_Click" 
						 Text="Next &gt;&gt;" CssClass="button" />
					 <br />
					 <asp:Label ID="MissingPersons" runat="server" Text="Missing Persons:" 
						 Font-Names="Arial" Font-Size="8pt"></asp:Label>
				 	 <asp:Literal ID="Literal1" runat="server" Text="&lt;br/&gt;"></asp:Literal>
					 <asp:Label ID="LatePersons" runat="server" Text="Late Today:" 
						 Font-Names="Arial" Font-Size="8pt"></asp:Label>
				 	<br />
					 <asp:ScriptManager ID="ScriptManager1" runat="server">
					 </asp:ScriptManager>
					 <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
						 <ContentTemplate>
							 <asp:UpdateProgress ID="UpdateProgress1" runat="server">
								 <ProgressTemplate>
									 Storing data...
								 </ProgressTemplate>
							 </asp:UpdateProgress>
							 <asp:ImageButton ID="SaveButton" runat="server" 
	ImageUrl="~/IMAGES/save.GIF" onclick="SaveButton_Click" Height="16px" Visible="False" />
							 <asp:ImageButton ID="EditButton" runat="server" ImageUrl="~/IMAGES/edit.ico" 
								 onclick="EditButton_Click" />
							 <br />
							 <asp:TextBox ID="TMESSAGET" runat="server" Height="100px" TextMode="MultiLine" 
								 Width="100%" ReadOnly="True"></asp:TextBox>
						 </ContentTemplate>
					 </asp:UpdatePanel>
				 </td>
			 </tr>
			 <tr>
				 <td style="height: 10px">
	 				<table style="width:100%;">
						<tr style="width:30%;">
							<td>
	 <asp:HyperLink ID="HyperLink2" runat="server" Font-Names="Arial" 
		 Font-Size="8pt" NavigateUrl="~/Index.aspx">View report home page...</asp:HyperLink>
		 			 	  </td>
							<td style="width:30%;">
	 <asp:HyperLink ID="HyperLink3" runat="server" Font-Names="Arial" 
		 Font-Size="8pt" NavigateUrl="~/dailyentry.aspx">Daily Entry...</asp:HyperLink>
		 			 	  </td>
							<td style="width:30%;">
	 <asp:HyperLink ID="HyperLink4" runat="server" Font-Names="Arial" 
		 Font-Size="8pt" NavigateUrl="~/EditPlan.aspx">Edit Plan</asp:HyperLink>
		 			 	  </td>
							<td style="width:30%;" id="ExpandButton">
								<input id="ExpandButton" onclick = "expandall();" type="button" value="Expand all..." /></td>
						</tr>
					 </table>
		 			 </td>
			 </tr>
			 <tr>
				 <td colspan="2">
    				 <asp:Panel ID="Panel1" runat="server" Height="25px" 
						 HorizontalAlign="Center" CssClass="textpanel">
						 <asp:Label ID="Label1" runat="server" Text="Label" Font-Bold="False" 
							 Font-Names="arial" Font-Size="Large"></asp:Label>
					 </asp:Panel>
    			</td>
			 </tr>
			 <tr>
				 <td colspan="2">
    				 <asp:GridView ID="GridView1" runat="server" BackColor="White" 
		 BorderColor="#3366CC" BorderStyle="None" BorderWidth="1px" CellPadding="4" 
		 DataSourceID="ReportDataSource2" Font-Names="Arial" 
		 Font-Size="Small" AutoGenerateColumns="False" onrowdatabound="GridView1_RowDataBound" 
						 AllowSorting="True">
						 <RowStyle BackColor="White" ForeColor="#003399" />
						 <Columns>
							 <asp:BoundField DataField="PERSON_NAME" HeaderText="Person" 
								 SortExpression="PERSON_NAME" />
							 <asp:BoundField DataField="TIME_START" DataFormatString="{0:HH:mm}" 
								 HeaderText="Time In" SortExpression="TIME_START" />
							 <asp:BoundField DataField="TIME_END" DataFormatString="{0:HH:mm}" 
								 HeaderText="Time Out" SortExpression="TIME_END" />
							 <asp:BoundField DataField="PREV_REPORT_DONE" HeaderText="Yesterday" 
								 SortExpression="PREV_REPORT_DONE" />
							 <asp:BoundField DataField="CUR_REPORT_DONE" HeaderText="Today" 
								 SortExpression="CUR_REPORT_DONE" />
							 <asp:BoundField DataField="REPORT_FUTURE" HeaderText="Next" 
								 SortExpression="REPORT_FUTURE" />
						 </Columns>
						 <FooterStyle BackColor="#99CCCC" ForeColor="#003399" />
						 <PagerStyle BackColor="#99CCCC" ForeColor="#003399" HorizontalAlign="Left" />
						 <SelectedRowStyle BackColor="#009999" Font-Bold="True" ForeColor="#CCFF99" />
						 <HeaderStyle BackColor="#003399" Font-Bold="True" ForeColor="#CCCCFF" />
						 <EditRowStyle Font-Names="Arial" />
					 </asp:GridView>
    			</td>
			 </tr>
		 </table>
    </div>
    <asp:SqlDataSource ID="ReportDataSource2" runat="server" 
		 ConnectionString="<%$ ConnectionStrings:ConnectionString %>" 
		 ProviderName="System.Data.OleDb" 
		 SelectCommand="">
	 </asp:SqlDataSource>
    <hr />
	 <asp:Panel ID="Panel3" runat="server" CssClass="textpanel">
		 <asp:Label ID="Intro" runat="server" 
		 Text="Office work report."></asp:Label>
	 </asp:Panel>
    <br />
    </form>
</body>
</html>
