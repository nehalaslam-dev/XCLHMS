<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PatientReport.aspx.cs"
    Inherits="XCLHMS.PatientModule.PatientReport.PatientReport" %>
    <%@ Register
        Assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91"
        Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

        <!DOCTYPE html>
        <html xmlns="http://www.w3.org/1999/xhtml">

        <head runat="server">
            <title>Patient Report | Hospital Management</title>
            <meta name="viewport" content="width=device-width, initial-scale=1.0" />
            <link href="../../Content/bootstrap.min.css" rel="stylesheet" />
            <link href="../../Content/style.min.css" rel="stylesheet" />
            <script src="../../Scripts/jquery-1.12.4.js"></script>
            <style>
                :root {
                    --primary-color: #3498db;
                    --secondary-color: #2c3e50;
                    --success-color: #27ae60;
                    --bg-light: #f4f7f6;
                    --text-main: #2d3436;
                }

                body {
                    background-color: var(--bg-light);
                    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                    color: var(--text-main);
                }

                .page-container {
                    padding: 40px 15px;
                }

                .report-card {
                    background: #ffffff;
                    border-radius: 12px;
                    box-shadow: 0 8px 30px rgba(0, 0, 0, 0.05);
                    border: 1px solid #eef2f1;
                    overflow: hidden;
                    margin-bottom: 30px;
                }

                .report-header {
                    background: var(--secondary-color);
                    color: white;
                    padding: 20px 30px;
                    display: flex;
                    justify-content: space-between;
                    align-items: center;
                }

                .report-header h2 {
                    margin: 0;
                    font-size: 20px;
                    font-weight: 600;
                    letter-spacing: 1px;
                }

                .report-body {
                    padding: 30px;
                }

                .filter-grid {
                    display: grid;
                    grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
                    gap: 20px;
                    background: #f8faf9;
                    padding: 20px;
                    border-radius: 8px;
                    margin-bottom: 30px;
                    border: 1px solid #e8ebe9;
                    align-items: end;
                }

                .form-group {
                    margin-bottom: 0;
                }

                .form-group label {
                    font-weight: 600;
                    font-size: 13px;
                    color: #636e72;
                    margin-bottom: 8px;
                    display: block;
                }

                .form-control {
                    border-radius: 6px;
                    border: 1px solid #dfe6e9;
                    padding: 10px 12px;
                    height: auto;
                    font-size: 14px;
                    transition: all 0.2s;
                }

                .form-control:focus {
                    border-color: var(--primary-color);
                    box-shadow: 0 0 0 3px rgba(52, 152, 219, 0.1);
                }

                .btn-actions {
                    display: flex;
                    gap: 10px;
                }

                .btn-custom {
                    padding: 10px 20px;
                    border-radius: 6px;
                    font-weight: 600;
                    font-size: 13px;
                    text-transform: uppercase;
                    border: none;
                    transition: all 0.2s;
                    cursor: pointer;
                    display: inline-flex;
                    align-items: center;
                    justify-content: center;
                }

                .btn-primary-custom {
                    background-color: var(--primary-color);
                    color: white;
                }

                .btn-primary-custom:hover {
                    background-color: #2980b9;
                    transform: translateY(-1px);
                    color: white;
                }

                .btn-success-custom {
                    background-color: var(--success-color);
                    color: white;
                }

                .btn-success-custom:hover {
                    background-color: #219150;
                    transform: translateY(-1px);
                    color: white;
                }

                .btn-secondary-custom {
                    background-color: #95a5a6;
                    color: white;
                }

                .btn-secondary-custom:hover {
                    background-color: #7f8c8d;
                    transform: translateY(-1px);
                    color: white;
                }

                .viewer-wrapper {
                    background: #fff;
                    border: 1px solid #eef2f1;
                    border-radius: 8px;
                    padding: 10px;
                    min-height: 600px;
                }

                .alert-danger-custom {
                    background-color: #fff5f5;
                    border-left: 4px solid #ff7675;
                    color: #d63031;
                    padding: 15px;
                    border-radius: 0 4px 4px 0;
                    margin-bottom: 20px;
                    font-size: 14px;
                }
            </style>
        </head>

        <body>
            <form id="form1" runat="server">
                <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

                <div class="container page-container">
                    <div class="report-card">
                        <div class="report-header">
                            <h2>PATIENT REPORT</h2>
                            <div class="btn-actions">
                                <asp:Button ID="btnExport" runat="server" Text="Export Excel"
                                    CssClass="btn-custom btn-success-custom" OnClick="btnExport_Click" />
                                <input type="button" id="btnPrint" value="Print Report"
                                    class="btn-custom btn-secondary-custom" />
                            </div>
                        </div>

                        <div class="report-body">
                            <div class="filter-grid">
                                <div class="form-group">
                                    <label>Start Date</label>
                                    <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control"
                                        TextMode="Date"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label>End Date</label>
                                    <asp:TextBox ID="txtEndDate" runat="server" CssClass="form-control" TextMode="Date">
                                    </asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label>Patient Type</label>
                                    <asp:DropDownList ID="ddlPatientType" runat="server" CssClass="form-control">
                                        <asp:ListItem Text="All Types" Value=""></asp:ListItem>
                                        <asp:ListItem Text="Indoor Patient" Value="InPatient"></asp:ListItem>
                                        <asp:ListItem Text="Outdoor Patient" Value="OutPatient"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="form-group">
                                    <asp:Button ID="btnSearch" runat="server" Text="Search Records"
                                        CssClass="btn-custom btn-primary-custom w-100" OnClick="btnSearch_Click" />
                                </div>
                            </div>

                            <asp:Panel ID="pnlError" runat="server" Visible="false">
                                <div class="alert-danger-custom">
                                    <asp:Label ID="lblError" runat="server" />
                                </div>
                            </asp:Panel>

                            <div class="viewer-wrapper">
                                <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="100%" Height="700px"
                                    ZoomMode="PageWidth" ProcessingMode="Local" ShowPrintButton="true"
                                    ShowExportControls="true" InternalBorderColor="Transparent">
                                </rsweb:ReportViewer>
                            </div>
                        </div>
                    </div>
                </div>
            </form>

            <script type="text/javascript">
                $(document).ready(function () {
                    function printReport(report_ID) {
                        var rv1 = $('#' + report_ID);
                        var table = rv1.find("div[id$='_oReportDiv']");
                        if (table.length == 0) {
                            alert("No report data to print. Please perform a search first.");
                            return;
                        }

                        var styles = "";
                        $('style, link[rel="stylesheet"]').each(function () {
                            styles += $(this).get(0).outerHTML;
                        });

                        var docCnt = styles + table.parent().html();
                        var newWin = window.open("", "_blank", "width=1000,height=800");
                        newWin.document.open();
                        newWin.document.write('<html><head><title>Print Report</title></head><body onload="window.print();">' + docCnt + '</body></html>');
                        newWin.document.close();
                    };

                    $('#btnPrint').click(function () {
                        printReport('ReportViewer1');
                    });
                });
            </script>
        </body>

        </html>