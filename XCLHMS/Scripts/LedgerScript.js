$(document).ready(function () {

    var pageURL = $(location).attr("href");
    var lastIndex = pageURL.lastIndexOf('/');
    var id = pageURL.substring(lastIndex + 1);
    if (id > 0) {
        $.ajax({
            url: "/Accounts/AccountLedger/GetLedgerDetail",
            type: "GET",
            data: { id: id },
            success: function (data) {
                GetledgerDetail(data);
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

    function GetledgerDetail(data) {
        var mainrow = $('#orderdetailsItems');
        $.each(data, function (index, item) {
            //assign master values                       
            $('#voucherNumber').val(item.voucherNumber);
            $('#voucherDate').val(ToJavaScriptDate(item.voucherDate));
            $('#Description').val(item.description);
            $('input:checkbox[value=' + item.isPosted + ']').prop('checked', 'checked');

            //assign details values
            var $newRow = $('#mainrow').clone().removeAttr('id');
            $('.pc', $newRow).val(item.HeadId);
            $('#debit', $newRow).val(item.debit);
            $('#credit', $newRow).val(item.credit);
            $('#narration', $newRow).val(item.narration);

            $('#add', $newRow).addClass('remove').val('Remove').removeClass('btn-success').addClass('btn-danger');
            $('#accountHeads,#debit,#credit,#narration,#add', $newRow).removeAttr('id');
            $('span.error', $newRow).remove();

            mainrow.append($newRow);

        });
    }

    $('#add').click(function () {
        var isAllValid = true;
        if ($('#accountHeads').val() == 0) {
            isAllValid = false;
            $('#accountHeads').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#accountHeads').siblings('span.error').css('visibility', 'hidden');
        }

        if (!(parseFloat($('#debit').val()) || $('#debit').val() == 0.00)) {
            isAllValid = false;
            $('#debit').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#debit').siblings('span.error').css('visibility', 'hidden');
        }


        if (!(parseFloat($('#credit').val()) || $('#credit').val() == 0.00)) {
            isAllValid = false;
            $('#credit').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#credit').siblings('span.error').css('visibility', 'hidden');
        }

        if ((parseFloat($('#debit').val()) > 0) && (parseFloat($('#credit').val()) > 0)) {
            isAllValid = false;
            $('#debit').siblings('span.error1').css('visibility', 'visible');
        }
        else {
            $('#debit').siblings('span.error1').css('visibility', 'hidden');
        }

        if ((parseFloat($('#credit').val()) > 0) && (parseFloat($('#debit').val()) > 0)) {
            isAllValid = false;
            $('#credit').siblings('span.error1').css('visibility', 'visible');
        }
        else {
            $('#credit').siblings('span.error1').css('visibility', 'hidden');
        }

        if (isAllValid) {
            var $newRow = $('#mainrow').clone().removeAttr('id');
            $('.pc', $newRow).val($('#accountHeads').val());

            $('#add', $newRow).addClass('remove').val('Remove').removeClass('btn-success').addClass('btn-danger');

            $('#accountHeads,#debit,#credit,#add', $newRow).removeAttr('id');
            $('span.error', $newRow).remove();
            $('span.error1', $newRow).remove();

            $('#orderdetailsItems').append($newRow);
            $('#accountHeads').val(0);
            $('#debit').val('0.00');
            $('#credit').val('0.00');
            $('#narration').val('');
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
        var errorItemCount = 0;

        $('#orderdetailsItems tbody tr').each(function (index, ele) {
            if ($('select.pc', this).val() == 0 ||
                $('.debit', this).val() == "" || isNaN($('.debit', this).val()) ||
                $('.credit', this).val() == "" || isNaN($('.credit', this).val())

                ) {
                errorItemCount++;
                $(this).addClass('error');
            } else {
                var LedgerItems = {
                    HeadId: $('select.pc', this).val(),
                    Debit: parseFloat($('.debit', this).val()),
                    Credit: parseFloat($('.credit', this).val()),
                    Narration: $('.narration', this).val()
                }
                list.push(LedgerItems);
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

        if ($('#voucherDate').val().trim() == '') {
            $('#voucherDate').siblings('span.error').css('visibility', 'visible');
            isAllValid = false;
        }
        else {
            $('#voucherDate').siblings('span.error').css('visibility', 'hidden');
        }

        if (isAllValid) {
            var data = {
                voucherDate: $('#voucherDate').val(),
                Description: $('#Description').val(),
                IsPosted: $('input[name=IsPosted]:checked').val(),
                AccountLedgerDetails: list
            }

            $(this).val('Please wait...');
            $.ajax({
                contentType: 'application/json',
                type: 'POST',
                dataType: 'json',
                url: '/Accounts/AccountLedger/save',
                data: JSON.stringify(data),
                success: function (data) {
                    if (data.status) {
                        alert('Successfully saved');
                        list = [];
                        $('#voucherNumber').val('');
                        $('#voucherDate').val('');
                        $('#Description').val('');
                        $('input[name=IsPosted]:checked').val('');
                        $('#orderdetailsItems').empty();
                        window.location.href = '/Accounts/AccountLedger/Index'
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