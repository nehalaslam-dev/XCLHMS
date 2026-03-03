
$(document).ready(function () {

    var _totalAmount = 0.00;
    var pageURL = $(location).attr("href");
    var lastIndex = pageURL.lastIndexOf('/');
    var id = pageURL.substring(lastIndex + 1);
    if (id > 0) {
        $.ajax({
            url: "/InventoryModule/GRN/GetGRNDetail",
            type: "GET",
            data: { id: id },
            success: function (data) {
                GetGRNDetail(data);
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

    function GetGRNDetail(data) {
        var mainrow = $('#orderdetailsItems');
        $.each(data, function (index, item) {
            //assign master values           
            $('#POId').val(item.poId);
            $('#GRNNO').val(item.grnNo);
            $('#GRNDate').val(ToJavaScriptDate(item.grnDate));
            $('#TotalAmount').val(item.totalAmt);
            $('input:checkbox[value=' + item.isComplete + ']').prop('checked', 'checked');

            //assign details values
            var $newRow = $('#mainrow').clone().removeAttr('id');
            $('.ic', $newRow).val(item.catId);
            $('.pc', $newRow).append($('<option></option>').val(item.productId).html(item.productName));
            $('.pc :selected', $newRow).remove();
            $('#description', $newRow).val(item.description);
            $('#orderquantity', $newRow).val(item.orderQty);
            $('#quantity', $newRow).val(item.qty);
            $('#rejectquantity', $newRow).val(item.rejectQty);
            $('#unitcost', $newRow).val(item.costPrice);
            $('#batchno', $newRow).val(item.batchNo);
            $('#mfgDate', $newRow).val(ToJavaScriptDate(item.mfgDate));
            $('#expiryDate', $newRow).val(ToJavaScriptDate(item.expiryDate));

            $('#add', $newRow).addClass('remove').val('Remove').removeClass('btn-success').addClass('btn-danger');
            $('#ItemCategory,#productCategory,#description,#orderquantity,#quantity,#rejectquantity,#unitcost,#batchno,#mfgDate,#expiryDate,#add', $newRow).removeAttr('id');
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

        if (!($('#quantity').val().trim() != '' && (parseInt($('#quantity').val()) || 0))) {
            isAllValid = false;
            $('#quantity').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#quantity').siblings('span.error').css('visibility', 'hidden');
        }


        if (!($('#unitcost').val().trim() != '' && (parseInt($('#unitcost').val()) || 0))) {
            isAllValid = false;
            $('#unitcost').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#unitcost').siblings('span.error').css('visibility', 'hidden');
        }

        if (!($('#batchno').val().trim() != '')) {
            isAllValid = false;
            $('#batchno').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#batchno').siblings('span.error').css('visibility', 'hidden');
        }

        if (!($('#mfgDate').val().trim() != '')) {
            isAllValid = false;
            $('#mfgDate').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#mfgDate').siblings('span.error').css('visibility', 'hidden');
        }

        if (!($('#expiryDate').val().trim() != '')) {
            isAllValid = false;
            $('#expiryDate').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#expiryDate').siblings('span.error').css('visibility', 'hidden');
        }

        var parts = $('#mfgDate').val().split("/");
        var mfgdt = new Date(parseInt(parts[2], 10),
                          parseInt(parts[1], 10) - 1,
                          parseInt(parts[0], 10));
        var partsExp = $('#expiryDate').val().split("/");
        var expdt = new Date(parseInt(partsExp[2], 10),
                          parseInt(partsExp[1], 10) - 1,
                          parseInt(partsExp[0], 10));
  

        if (expdt <= mfgdt) {
            isAllValid = false;
            alert('Expiry date must be greater than Mfg date.');
        }

        if (isAllValid) {
            var $newRow = $('#mainrow').clone().removeAttr('id');
            $('.ic', $newRow).val($('#ItemCategory').val());
            $('.pc', $newRow).val($('#productCategory').val());
            $('#add', $newRow).addClass('remove').val('Remove').removeClass('btn-success').addClass('btn-danger');
            $('#ItemCategory,#productCategory,#description,#quantity,#rejectquantity,#unitcost,#batchno,#mfgDate,#expiryDate,#add', $newRow).removeAttr('id');
            $('span.error', $newRow).remove();

            if ($('#TotalAmount').val().trim() == '') {
                $('#TotalAmount').val('0');
            }

            _totalAmount = parseInt($('#TotalAmount').val()) + parseInt($('#unitcost').val());

            $('#orderdetailsItems').append($newRow);
            $('#ItemCategory').val(0);
            $('#productCategory').val(0);
            $('#description,#quantity,#rejectquantity,#unitcost,#batchno,#mfgDate,#expiryDate').val('');
            $('#orderItemError').empty();

            $('#TotalAmount').val(_totalAmount);

        }
    });

    $('#orderdetailsItems').on('click', '.remove', function () {
        _totalAmount = parseInt($('#TotalAmount').val()) - parseInt($(this).parent().parent().find('.unitcost').val());
        $(this).closest('tr').remove();
        $('#TotalAmount').val(_totalAmount);
        //$(this).parents('tr').remove();
    });

    $('#submit').click(function () {
        var isAllValid = true;
        $('#orderItemError').text('');
        var list = [];
        var stocklist = [];
        var errorItemCount = 0;

        $('#orderdetailsItems tbody tr').each(function (index, ele) {
            if ($('select.ic', this).val() == 0 ||
                $('select.pc', this).val() == 0 ||
                (parseInt($('.quantity', this).val()) || 0) == 0 ||
                $('.unitcost', this).val() == "" || isNaN($('.unitcost', this).val()) ||
                $('.batchno', this).val() == "" || $('.mfgDate', this).val() == "" || $('.expiryDate', this).val() == "") {
                errorItemCount++;
                $(this).addClass('error');
            } else {
                var GRNItems = {
                    ProductId: $('select.pc', this).val(),
                    Qty: parseInt($('.quantity', this).val()),
                    rejectQty: parseInt($('.rejectquantity', this).val()),
                    CostPrice: parseFloat($('.unitcost', this).val()),
                    batchno: $('.batchno', this).val(),
                    MgfDate: $('.mfgDate', this).val(),
                    ExpiryDate: $('.expiryDate', this).val()
                }

                var _GRN = 'Stock In';
                var StockItems = {
                    ProductId: $('select.pc', this).val(),
                    RefCode: $('#GRNNO').val(),
                    StockDate: $('#GRNDate').val(),
                    batchno: $('.batchno', this).val(),
                    MfgDate: $('.mfgDate', this).val(),
                    ExpiryDate: $('.expiryDate', this).val(),
                    QtyIn: parseInt($('.quantity', this).val()),
                    QtyOut: 0,
                    IssuedTo: '',
                    tranType: _GRN

                }

                list.push(GRNItems);
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

        if ($('#POId').val() == 0) {
            $('#POId').siblings('span.error').css('visibility', 'visible');
            isAllValid = false;
        }
        else {
            $('#POId').siblings('span.error').css('visibility', 'hidden');
        }

        if ($('#GRNDate').val().trim() == '') {
            $('#GRNDate').siblings('span.error').css('visibility', 'visible');
            isAllValid = false;
        }
        else {
            $('#GRNDate').siblings('span.error').css('visibility', 'hidden');
        }

        if ($('#TotalAmount').val() == '') {
            $('#TotalAmount').siblings('span.error').css('visibility', 'visible');
            isAllValid = false;
        }
        else {
            $('#TotalAmount').siblings('span.error').css('visibility', 'hidden');
        }

        if (isAllValid) {
            var data = {
                POID: $('#POId').val(),
                GRNNO: $('#GRNNO').val(),
                GRNDate: $('#GRNDate').val(),
                TotalAmount: $('#TotalAmount').val(),
                IsComplete: $('input[name=IsComplete]:checked').val(),
                CreatedDate: new Date($.now()),
                GRNDetails: list,
                Stocks: stocklist
            }

            $(this).val('Please wait...');

            $.ajax({
                contentType: 'application/json',
                type: 'POST',
                dataType: 'json',
                url: '/InventoryModule/GRN/save',
                data: JSON.stringify(data),
                success: function (data) {
                    if (data.status) {
                        alert('Successfully saved');
                        list = [];
                        stocklist = [];
                        $('#POId').val(0);
                        $('#GRNNO').val('');
                        $('#GRNDate').val('');
                        $('#TotalAmount').val('');
                        $('input[name=IsComplete]:checked').val('');
                        $('#orderdetailsItems').empty();
                        window.location.href = '/InventoryModule/GRN/Index'
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