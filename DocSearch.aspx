<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DocSearch.aspx.cs" Inherits="DocSearch" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server" defaultfocus="SearchText">
    <div>
    
    <div>
    
		 <asp:Panel ID="Panel2" runat="server" HorizontalAlign="Center">
			 <asp:Image ID="Image1" runat="server" ImageUrl="~/IMAGES/datasearch.PNG" />
			 <br />
			 <asp:TextBox ID="SearchText" runat="server" Width="340px" 
				 ontextchanged="SearchText_TextChanged"></asp:TextBox>
			 <asp:Button ID="SearchButton" runat="server" 
	Text="Search" onclick="SearchButton_Click" />
		 </asp:Panel>
		 <asp:Panel ID="Panel3" runat="server" HorizontalAlign="Center">
			 <asp:GridView ID="GridView1" runat="server" 
	onrowdatabound="GridView1_RowDataBound" AutoGenerateColumns="False" 
				 GridLines="Horizontal" Width="100%">
				 <RowStyle Font-Names="Arial" Font-Size="8pt" />
				 <Columns>
					 <asp:BoundField DataField="System.ItemPathDisplay" HeaderText="Document">
					 <ItemStyle HorizontalAlign="Left" />
					 </asp:BoundField>
				 </Columns>
				 <HeaderStyle Font-Names="Arial" Font-Size="8pt" />
			 </asp:GridView>
		 </asp:Panel>
    
    </div>
    </div>
    </form>
</body>
</html>
