<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TaxDeduction.aspx.cs" Inherits="XCLHMS.Reports.TaxDeduction" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <title>Tax Deduction Report - SASIMS</title>
    <link href="../Content/bootstrap.css" rel="stylesheet" />
    <link href="../Content/jquery-ui.css" rel="stylesheet" />
    <script src="../Scripts/jquery-1.12.4.js"></script>
    <script src="../Scripts/jquery-ui.js"></script>
    <script>
        $(function () {
            $(".date").datepicker({ 
                dateFormat: 'mm/dd/yy',
                changeMonth: true,
                changeYear: true
            });
        });
    </script>
    <style>
        body { 
            padding-top: 100px; 
            background-color: #f8f9fa; 
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
        }
        .navbar-inverse {
            background-color: #2c3e50;
            border-color: #2c3e50;
        }
        .navbar-brand {
            color: #fff !important;
            font-weight: bold;
            font-size: 1.4em;
        }
        .filter-panel { 
            background: white; 
            padding: 25px; 
            border-radius: 12px; 
            border: 1px solid #e1e4e8;
            box-shadow: 0 4px 6px rgba(0,0,0,0.05); 
            margin-bottom: 25px; 
        }
        .header-title { 
            color: #2c3e50; 
            font-weight: 700;
            margin-bottom: 25px; 
            border-left: 5px solid #3498db; 
            padding-left: 15px;
        }
        .form-control {
            border-radius: 6px;
            border: 1px solid #ced4da;
            transition: border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out;
        }
        .form-control:focus {
            border-color: #80bdff;
            outline: 0;
            box-shadow: 0 0 0 0.2rem rgba(0,123,255,.25);
        }
        .btn-generate {
            background-color: #3498db;
            border-color: #3498db;
            font-weight: 600;
            padding: 10px 25px;
            border-radius: 6px;
            transition: all 0.2s;
        }
        .btn-generate:hover {
            background-color: #2980b9;
            transform: translateY(-1px);
            box-shadow: 0 4px 8px rgba(0,0,0,0.1);
        }
        label {
            font-weight: 600;
            color: #495057;
            margin-bottom: 6px;
        }
        .report-container {
            background: white; 
            padding: 15px; 
            border-radius: 12px; 
            border: 1px solid #e1e4e8;
            box-shadow: 0 4px 6px rgba(0,0,0,0.05);
            overflow-x: auto;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        
        <div class="navbar navbar-inverse navbar-fixed-top">
            <div class="container">
                <div class="navbar-header">
                    <a class="navbar-brand" href="/">Hospital Management System - SASIMS</a>
                </div>
                <div class="navbar-collapse collapse">
                    <ul class="nav navbar-nav navbar-right">
                        <li><a href="/" style="color: #fff;">Dashboard</a></li>
                    </ul>
                </div>
            </div>
        </div>

        <div class="container-fluid">
            <div class="row">
                <div class="col-md-10 col-md-offset-1">
                    <h2 class="header-title">Tax Deduction Detailed Report</h2>
                    
                    <div class="filter-panel">
                        <div class="row">
                            <div class="col-md-2 col-sm-4">
                                <label>Start Date</label>
                                <asp:TextBox ID="txtStart" runat="server" CssClass="date form-control"></asp:TextBox>
                            </div>
                            <div class="col-md-2 col-sm-4">
                                <label>End Date</label>
                                <asp:TextBox ID="txtEnd" runat="server" CssClass="date form-control"></asp:TextBox>
                            </div>
                            <div class="col-md-3 col-sm-4">
                                <label>Object Head</label>
                                <asp:DropDownList ID="ddlHead" runat="server" CssClass="form-control"></asp:DropDownList>
                            </div>
                             <div class="col-md-3 col-sm-6">
                                <label>Vendor</label>
                                <asp:DropDownList ID="ddlVendor" runat="server" CssClass="form-control"></asp:DropDownList>
                            </div>
                            <div class="col-md-2 col-sm-6">
                                <label>Bill #</label>
                                <asp:TextBox ID="txtBillNo" runat="server" CssClass="form-control" placeholder="Search bill..."></asp:TextBox>
                            </div>
                        </div>
                        <div class="row" style="margin-top: 20px;">
                            <div class="col-sm-12 text-right">
                                <asp:Button ID="btnSearch" runat="server" Text="Generate Report" OnClick="btnSearch_Click" CssClass="btn btn-primary btn-generate" />
                                <div style="margin-top: 10px;">
                                    <asp:Label ID="lblMsg" runat="server" CssClass="text-danger font-weight-bold"></asp:Label>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="report-container">
                        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="100%" Height="750px" ZoomMode="Percent" ZoomPercent="100" ShowWaitControlCancelLink="False" BorderStyle="None"></rsweb:ReportViewer>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>