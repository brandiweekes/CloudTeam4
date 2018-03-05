<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SUFS_ClientApplication._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>My AWS Enabled Application - SUFS_ClientApplication</title>
    <link rel="stylesheet" href="styles/styles.css" type="text/css" media="screen" charset="utf-8"/>
</head>

<body>
    <!--
<div id="content" class="container">
    <div class="section grid grid5 s3">
        <h2>Create File</h2>
        <input text=""/>
    </div>
    <div class="section grid grid5 sdb">
        <h2>Amazon SimpleDB Domains:</h2>
        <ul>
            <asp:Label ID="sdbPlaceholder" runat="server"></asp:Label>
                
        </ul>
    </div>

    <div class="section grid grid5 gridlast ec2">
        <h2>Amazon EC2 Instances:</h2>
        <ul>
            <asp:Label ID="ec2Placeholder" runat="server"></asp:Label>

        </ul>
    </div>
</div> -->
    <h1><center> <b> <u>SU File System </u></b></center></h1>
    <form id="form1" runat="server">
        <p>
            <label>Enter Bucket Name:</label> <br/>
            <input type= "text" name="bucketName"/> <br/><br/>
            <label>Enter Key Name:</label> <br/>
            <input type= "text" name="keyName"/> <br/><br/>
            <asp:Button id="button1" runat="server" Text="Create File" OnClick="button1Clicked" />
        </p>
        <br/>
    </form>
   </body>
</html>
