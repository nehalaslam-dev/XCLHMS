$(document).ready(function () {
    $("#HeadId").change(function () {
        var selectedItemValue = $(this).val();
        $('#Code').val('');
        $('#AnnualBudget').val('');
        $.ajax({
            url: "/Accounts/AccountRegister/GetBudgetInfoById",
            type: "GET",
            data: { Id: selectedItemValue },
            success: function (data) {
                $.each(data, function (index, item) {
                    $('#Code').val(item.code);
                    $('#AnnualBudget').val(item.budget);
                });
            },
            error: function () {
                alert("Error! Fetching records.")
            }
        });
    });


    $('#addToList').click(function () {
        var isAllValid = true;

        if ($('#HeadId').val() == 0) {
            isAllValid = false;
            $('#HeadId').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#HeadId').siblings('span.error').css('visibility', 'hidden');
        }

        if ($('#VendorId').val() == 0) {
            isAllValid = false;
            $('#VendorId').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#VendorId').siblings('span.error').css('visibility', 'hidden');
        }

        if ($('#ChequeNo').val().trim() == '') {
            isAllValid = false;
            $('#ChequeNo').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#ChequeNo').siblings('span.error').css('visibility', 'hidden');
        }

        if ($('#BillNo').val().trim() == '') {
            isAllValid = false;
            $('#BillNo').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#BillNo').siblings('span.error').css('visibility', 'hidden');
        }

        if ($('#billdate').val().trim() == '') {
            isAllValid = false;
            $('#billdate').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#billdate').siblings('span.error').css('visibility', 'hidden');
        }

        if ($('#BudgetRelease').val().trim() == '') {
            isAllValid = false;
            $('#BudgetRelease').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#BudgetRelease').siblings('span.error').css('visibility', 'hidden');
        }

        if ($('#EntryType').val() == 0) {
            isAllValid = false;
            $('#EntryType').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#EntryType').siblings('span.error').css('visibility', 'hidden');
        }

        //if ($('#SecTaxId').val() == 0) {
        //    isAllValid = false;
        //    $('#SecTaxId').siblings('span.error').css('visibility', 'visible');
        //}
        //else {
        //    $('#SecTaxId').siblings('span.error').css('visibility', 'hidden');
        //}


        if (isAllValid) {
            //alert($("#subTax").val());
            if ($("#EntryType option:selected").val() == "Expense") {
               var billno = $("#BillNo").val(),
               billdate = $("#billdate").val(),
               name = $("#VendorId option:selected").text(),
               vendorId = $("#VendorId option:selected").val(),
               ChequeNo = $("#ChequeNo").val(),
               annualbudget = $("#AnnualBudget").val(),
               releasebudget = $("#BudgetRelease").val(),
               entrytype = $("#EntryType option:selected").val(),
               grossamount = 0.00,
               incometax = (releasebudget * $("#subTax").val()) / 100,
               sst = (releasebudget * $("#SecsubTax").val()) / 100,
               netamt = (releasebudget - (incometax + sst)),
               balance = (annualbudget - releasebudget),
               detailsTableBody = $("#demoGrid tbody");
               $("#AnnualBudget").val(balance);
            }
            else {
                var billno = $("#BillNo").val(),
               billdate = $("#billdate").val(),
               name = $("#VendorId option:selected").text(),
               vendorId = $("#VendorId option:selected").val(),
               ChequeNo = $("#ChequeNo").val(),
               annualbudget = $("#AnnualBudget").val(),
               releasebudget = $("#BudgetRelease").val(),
               entrytype = $("#EntryType option:selected").val(),
               grossamount = 0.00,
               incometax = (releasebudget * $("#subTax").val()) / 100,
               sst = (releasebudget * $("#SecsubTax").val()) / 100,
               netamt = (releasebudget - (incometax + sst)),
               balance = (parseInt(annualbudget) + parseInt(releasebudget)),
               detailsTableBody = $("#demoGrid tbody");
               $("#AnnualBudget").val(balance);
            }


            var productItem = '<tr><td>' + entrytype + '</td><td>' + billno + '</td><td>' + billdate + '</td><td style="display:none;">' + vendorId + '</td><td>' + name + '</td><td>' + ChequeNo + '</td><td>' + annualbudget + '</td><td>' + releasebudget + '</td><td>' + incometax + '</td><td>' + sst + '</td><td>' + netamt + '</td><td>' + balance + '</td><td><a data-itemId="0" href="#" class="deleteItem">Remove</a></td></tr>';
            detailsTableBody.append(productItem);
        }
    });


    $(document).on('click', 'a.deleteItem', function (e) {
        e.preventDefault();
        var $self = $(this);
        if ($(this).attr('data-itemId') == "0") {
            $(this).parents('tr').css("background-color", "#ff6347").fadeOut(800, function () {
                $(this).remove();
            });
        }
    });

    $('#submit').click(function (e) {
        e.preventDefault();
        var list = [];
        list.length = 0;

        $.each($("#demoGrid tbody tr"), function () {
            list.push({
                HeadId: $('#HeadId').val(),
                EntryType: $(this).find('td:eq(0)').html(),
                BillNo: $(this).find('td:eq(1)').html(),
                billdate: $(this).find('td:eq(2)').html(),
                VendorId: $(this).find('td:eq(3)').html(),
                ChequeNo: $(this).find('td:eq(5)').html(),
                AnnualBudget: $(this).find('td:eq(6)').html(),
                BudgetRelease: $(this).find('td:eq(7)').html(),
                GrossAmount: 0.00,
                IncomeTax: $(this).find('td:eq(8)').html(),
                SST: $(this).find('td:eq(9)').html(),
                NetAmount: $(this).find('td:eq(10)').html(),
                Balance: $(this).find('td:eq(11)').html()
            });
        });

        var data = JSON.stringify({
            HeadId: $('#HeadId').val(),
            reg: list
        });

        $.when(savePresc(data)).then(function (response) {
            console.log(response);

        }).fail(function (err) {
            console.log(err);

        });
    });

    function savePresc(data) {
        return $.ajax({
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            type: 'POST',
            url: "/Accounts/AccountRegister/save",
            data: data,
            success: function (result, data) {
                alert(result);
                //location.reload();
                GetAccountRegister(data);
            },
            error: function () {
                alert('Error!')
            }
        });
    }

    var pageURL = $(location).attr("href");
    var lastIndex = pageURL.lastIndexOf('/');
    var id = pageURL.substring(lastIndex + 1);
    if (id > 0) {
        $.ajax({
            url: "/Accounts/AccountRegister/GetAccountRegsiter",
            type: "GET",
            data: { id: id },
            success: function (data) {
                GetAccountRegister(data);
                $('#HeadId').prop("disabled", true);
            },
            error: function () {
                alert("Error! Fetching records.")

            }
        });
    }

    function ToJavaScriptDate(value) {
        var pattern = /Date\(([^)]+)\)/;
        var results = pattern.exec(value);
        var dt = new Date(parseFloat(results[1]));
        return dt.getDate() + "/" + (dt.getMonth() + 1) + "/" + dt.getFullYear();
    }

    function GetAccountRegister(data) {
        var tbldetailsTable = $('#demoGrid');
        $.each(data, function (index, item) {
            $('#HeadId').val(item.headId);
            $('#Code').val(item.accountCode);
            $('#AnnualBudget').val(item.balance);

            var tr = $("<tr></tr>");
            tr.html(("<td>" + item.entryType + "</td>")
                  + " " + ("<td>" + item.billno + "</td>")
                  + " " + ("<td>" + ToJavaScriptDate(item.billdate) + "</td>")
                  + " " + ("<td style=" + "display:none;" + ">" + item.vendorid + "</td>")
                  + " " + ("<td>" + item.vendorname + "</td>")
                  + " " + ("<td>" + item.chequeNo + "</td>")
                  + " " + ("<td>" + item.annualBudget + "</td>")
                  + " " + ("<td>" + item.relBudget + "</td>")
                  + " " + ("<td>" + item.incomeTax + "</td>")
                  + " " + ("<td>" + item.sst + "</td>")
                  + " " + ("<td>" + item.netAmount + "</td>")
                  + " " + ("<td>" + item.balance + "</td>")
                  + " " + ("<td><a data-itemId='0' href='#' class='deleteItem'>Remove</a></td>")
                  );

            tbldetailsTable.append(tr);
        });

    }
});