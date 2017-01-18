<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CustomClass._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Data Entity</title>
    <script type="text/javascript" src="MyScript.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        Output
        <br />
        <asp:TextBox ID="txtOutput" runat="server" Height="300px" TextMode="MultiLine" 
            Width="800px"></asp:TextBox>
        <br />
        <br />
        <asp:Button ID="btnGenerate" runat="server" Text="Generate" 
            onclick="btnGenerate_Click" />
    </form>
</body>
</html>
