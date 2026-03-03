$(document).ready(function () {
    var pageURL = $(location).attr("href");
    var lastIndex = pageURL.lastIndexOf('/');
    var id = pageURL.substring(lastIndex + 1);
    if (id > 0) {
        $.ajax({
            url: "/InventoryModule/IR/GetIRDetail",
            type: "GET",
            data: { id: id },
            success: function (data) {
                GetIRDetail(data);
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

    function GetIRDetail(data) {
        var mainrow = $('#orderdetailsItems');
        var crtDate = new Date();
        $.each(data, function (index, item) {
            //assign master values           
            $('#customerId').val(item.customerId);
            $('#IRCode').val(item.irCode);
            $('#IRDate').val(ToJavaScriptDate(item.irdate));
            $('#Remarks').val(item.irRemarks);
           
            //assign details values
            var $newRow = $('#mainrow').clone().removeAttr('id');
            $('.ic', $newRow).val(item.categoryId);
            $('.pc', $newRow).append($('<option></option>').val(item.productId).html(item.productName));
            $('.pc :selected', $newRow).remove();
            $('.bc', $newRow).append($('<option></option>').val(item.batchno).html(item.batchno));
            $('.bc :selected', $newRow).remove();
            $('#mfgDate', $newRow).val(ToJavaScriptDate(item.mfgDate));
            $('#expiryDate', $newRow).val(ToJavaScriptDate(item.expiryDate));
            $('#availableQty', $newRow).val(item.totalQty);
            $('#reqQty', $newRow).val(item.reqQty);
            $('#delvrQty', $newRow).val(item.delQty);
            $('#suppliment', $newRow).val(item.suppliment);

            $('#add', $newRow).addClass('remove').val('Remove').removeClass('btn-success').addClass('btn-danger');
            $('#ItemCategory,#productCategory,#batchno,#mfgDate,#expiryDate,#availableQty,#reqQty,#delvrQty,#suppliment,#add', $newRow).removeAttr('id');
            $('span.error', $newRow).remove();

            mainrow.append($newRow);

        });
    }


    $('#add').click(function () {
        var isAllValid = true;
        if ($('#ItemCategory').val() == 0) {
            isAllValid = false;
            $('#ItemCategory').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#ItemCategory').siblings('span.error').css('visibility', 'hidden');
        }

        if ($('#productCategory').val() == 0) {
            isAllValid = false;
            $('#productCategory').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#productCategory').siblings('span.error').css('visibility', 'hidden');
        }

        if ($('#batchno').val() == 0) {
            isAllValid = false;
            $('#batchno').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#batchno').siblings('span.error').css('visibility', 'hidden');
        }

        if (!($('#reqQty').val().trim() != '' && (parseInt($('#reqQty').val()) || 0))) {
            isAllValid = false;
            $('#reqQty').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#reqQty').siblings('span.error').css('visibility', 'hidden');
        }

        if (parseInt($('#reqQty').val().trim()) > parseInt($('#availableQty').val().trim())) {
            isAllValid = false;
            alert('Request Qty must be less then order Qty');
        }       

        if (!($('#delvrQty').val().trim() != '' && (parseInt($('#delvrQty').val()) || 0))) {
            isAllValid = false;
            $('#delvrQty').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#delvrQty').siblings('span.error').css('visibility', 'hidden');
        }


        if (isAllValid) {
            var $newRow = $('#mainrow').clone().removeAttr('id');
            $('.ic', $newRow).val($('#ItemCategory').val());
            $('.pc', $newRow).val($('#productCategory').val());
            $('.bc', $newRow).val($('#batchno').val());

            $('#add', $newRow).addClass('remove').val('Remove').removeClass('btn-success').addClass('btn-danger');

            $('#ItemCategory,#productCategory,#batchno,#mfgDate,#expiryDate,#availableQty,#reqQty,#delvrQty,#suppliment,#add', $newRow).removeAttr('id');
            $('span.error', $newRow).remove();

            $('#orderdetailsItems').append($newRow);
            $('#ItemCategory').val(0);
            $('#productCategory').val(0);
            $('#batchno').val(0);
            $('#availableQty,#reqQty,#delvrQty,#suppliment').val('0');
            $('#mfgDate,#expiryDate').val('');
            $('#orderItemError').empty();

        }

    });
    $('#orderdetailsItems').on('click', '.remove', function () {
        $(this).parents('tr').remove();
    });

    $('#submit').click(function () {
        var isAllValid = true;
        $('#orderItemError').text('');
        var list = [];
        var stocklist = [];
        var errorItemCount = 0;

        $('#orderdetailsItems tbody tr').each(function (index, ele) {
            var batchno, mfgdate, expirydate;
            if ($('select.ic', this).val() == 0 ||
              $('select.pc', this).val() == 0 ||
              (parseInt($('.reqQty', this).val()) || 0) == 0 ||
               (parseInt($('.delvrQty', this).val()) || 0) == 0) {
                errorItemCount++;
                $(this).addClass('error');
            } else {
                var IRItems = {
                    ProductId: $('select.pc', this).val(),
                    StockQty:parseInt($('.availableQty', this).val()),
                    ReqQty: parseInt($('.reqQty', this).val()),
                    DelvQty: parseInt($('.delvrQty', this).val()),
                    batchno: $('select.bc', this).val(),
                    MfgDate: $('.mfgDate', this).val(),
                    expiryDate: $('.expiryDate', this).val(),
                    Suppliment:  parseInt($('.suppliment', this).val()),
                }

                var _IR = 'Stock Out';
                var StockItems = {
                    ProductId: $('select.pc', this).val(),
                    RefCode: $('#IRCode').val(),
                    StockDate: $('#IRDate').val(),
                    batchno: $('select.bc', this).val(),
                    MfgDate: $('.mfgDate', this).val(),
                    ExpiryDate: $('.expiryDate', this).val(),
                    QtyIn: 0,
                    QtyOut: (parseInt($('.delvrQty', this).val()) + parseInt($('.suppliment', this).val())),
                    IssuedTo:$("#customerId option:selected").text(),
                    tranType: _IR
                }

                list.push(IRItems);
                stocklist.push(StockItems);
            }
        });

        if (errorItemCount > 0) {
            $('#orderItemError').text(errorItemCount + " invalid entry in item list.");
            isAllValid = false;
        }

        if (list.length == 0) {
            $('#orderItemError').text('At least 1 item required.');
            isAllValid = false;
        }

        if ($('#customerId').val() == 0) {
            $('#customerId').siblings('span.error').css('visibility', 'visible');
            isAllValid = false;
        }
        else {
            $('#customerId').siblings('span.error').css('visibility', 'hidden');
        }

        if ($('#IRDate').val().trim() == '') {
            $('#IRDate').siblings('span.error').css('visibility', 'visible');
            isAllValid = false;
        }
        else {
            $('#IRDate').siblings('span.error').css('visibility', 'hidden');
        }

        if (isAllValid) {
            var createddate = new Date();
            var data = {
                customerId: $('#customerId').val(),
                IRCode: $('#IRCode').val().trim(),
                IRDate: $('#IRDate').val(),
                Remarks: $('#Remarks').val(),
                CreatedDate : createddate,
                IRDetails: list,
                Stocks: stocklist
            }

            $(this).val('Please wait...');

            $.ajax({
                contentType: 'application/json',
                type: 'POST',
                dataType: 'json',
                url: '/InventoryModule/IR/save',
                data: JSON.stringify(data),
                success: function (data) {
                    if (data.status) {
                        alert('Successfully saved');
                        list = [];
                        stocklist = [];
                        $('#customerId').val(0);
                        $('#IRCode').val('');
                        $('#IRDate').val('');
                        $('#Remarks').val('');
                        window.location.href = '/InventoryModule/IR/Index'
                    }
                    else {
                        alert('Error!');
                    }
                    $('#submit').val('Save');
                },
                error: function (error) {
                    alert('Error!');
                    console.log(error);
                    $('#submit').val('Save');
                }
            });
        }

    });

});