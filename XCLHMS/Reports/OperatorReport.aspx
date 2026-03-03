<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OperatorReport.aspx.cs" Inherits="XCLHMS.Reports.OperatorReport" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<html>
<head>
    <link href="../Content/bootstrap.css" rel="stylesheet" />
    <link href="../Content/jquery-ui.css" rel="stylesheet" />
    <link href="../Content/Site.css" rel="stylesheet" />
    <link href="../Content/style.min.css" rel="stylesheet" />


    <script src="../Scripts/jquery-1.12.4.js"></script>
    <script src="../Scripts/jquery-ui.js"></script>

    <script>
        jQuery(function () {
            jQuery(".date").datepicker();
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="col-md-8 col-md-offset-1">

            <div class="row">
                <div class="col-sm-4">
                    <div class="form-group">
                        <label class="margin-Zero">
                            Select Operator:
                        </label>
                        <asp:DropDownList ID="ddlOperator" runat="server" CssClass="form-control" Width="">
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="col-sm-2">
                    <div class="form-group">
                        <label class="margin-Zero">
                            Start Date:
                        </label>
                        <asp:TextBox ID="txtStart" runat="server" CssClass="date form-control"></asp:TextBox>
                    </div>
                </div>
                <div class="col-sm-2">
                    <div class="form-group">
                        <label class="margin-Zero">
                            End Date:
                        </label>
                        <asp:TextBox ID="txtEnd" runat="server" CssClass="date form-control"></asp:TextBox>
                    </div>
                </div>
                <div class="col-sm-4">
                    <br />
                    <div class="form-group">
                        <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" CssClass="btn btn-primary" />
                        <input type="button" id="printBtn" value="Print" class="btn btn-primary" />
                    </div>
                </div>
            </div>
            <div>
                <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
                <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="104%" Height="590px"></rsweb:ReportViewer>
            </div>
        </div>
    </form>
</body>
</html>
<script type="text/javascript">
    $(document).ready(function () {
        function printReport(report_ID) {
            var rv1 = $('#' + report_ID);
            var iDoc = rv1.parents('html');

            // Reading the report styles
            var styles = iDoc.find("head style[id$='ReportControl_styles']").html();
            if ((styles == undefined) || (styles == '')) {
                iDoc.find('head script').each(function () {
                    var cnt = $(this).html();
                    var p1 = cnt.indexOf('ReportStyles":"');
                    if (p1 > 0) {
                        p1 += 15;
                        var p2 = cnt.indexOf('"', p1);
                        styles = cnt.substr(p1, p2 - p1);
                    }
                });
            }
            if (styles == '') { alert("Cannot generate styles, Displaying without styles.."); }
            styles = '<style type="text/css">' + styles + "</style>";

            // Reading the report html
            var table = rv1.find("div[id$='_oReportDiv']");
            if (table == undefined) {
                alert("Report source not found.");
                return;
            }

            // Generating a copy of the report in a new window
            var docType = '<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/loose.dtd">';
            var docCnt = styles + table.parent().html();
            var docHead = '<head><title>Printing ...</title><style>body{margin:5;padding:0;}</style></head>';
            var winAttr = "location=yes, statusbar=no, directories=no, menubar=no, titlebar=no, toolbar=no, dependent=no, width=720, height=600, resizable=yes, screenX=200, screenY=200, personalbar=no, scrollbars=yes";;
            var newWin = window.open("", "_blank", winAttr);
            writeDoc = newWin.document;
            writeDoc.open();
            writeDoc.write(docType + '<html>' + docHead + '<body onload="window.print();">' + docCnt + '</body></html>');
            writeDoc.close();

            // The print event will fire as soon as the window loads
            newWin.focus();
            // uncomment to autoclose the preview window when printing is confirmed or canceled.
            // newWin.close();
        };

        $('#printBtn').click(function () {
            printReport('ReportViewer1');
        });
    });
</script>
