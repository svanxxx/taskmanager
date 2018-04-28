<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:Panel ID="Panel1" runat="server" Height="699px" 
		 HorizontalAlign="Center" BackImageUrl="~/IMAGES/matrix.gif">
		 <br />
		 <br />
		 <br />
		 <br />
		 <br />
		 <br />
		 <br />
		 <br />
		 <br />
		 <br />
		 <br />
		 <center>
		 <asp:Login ID="Login1" runat="server" BackColor="#F7F6F3" BorderColor="Black" 
			 BorderPadding="4" BorderStyle="Solid" BorderWidth="5px" Font-Names="Verdana" 
			 Font-Size="0.8em" ForeColor="#333333" Height="170px" 
			 onauthenticate="Login1_Authenticate" Width="285px" RememberMeSet="True">
			 <TextBoxStyle Font-Size="0.8em" />
			 <LoginButtonStyle BackColor="#FFFBFF" BorderColor="#CCCCCC" BorderStyle="Solid" 
				 BorderWidth="1px" Font-Names="Verdana" Font-Size="0.8em" ForeColor="#284775" />
			 <InstructionTextStyle Font-Italic="True" ForeColor="Black" />
			 <TitleTextStyle BackColor="#5D7B9D" Font-Bold="True" Font-Size="0.9em" 
				 ForeColor="White" />
		 </asp:Login>
		 </center>
	 </asp:Panel>
    </form>
</body>
</html>
