$(document).ready(function () {

    $('#btnSearch').click(function () {
        if ($('#tokenId').val() != '') {
            var _myValue = $('#tokenId').val();
            var ddltestCategory = $("#LabTestCatId");
            var ddlPatient = $("#PatientId");
            $('#LabTestCatId').prop("disabled", true);
            $('#PatientId').prop("disabled", true);
            $('#TestDate').prop("disabled", true);
            $('#DeliveryDate').prop("disabled", true);
            $('#testCategory').prop("disabled", true);
            $.ajax({
                cache: false,
                type: "GET",
                url: "/LabTestModule/TestResult/GetAllTestDetail",
                data: { "myValue": _myValue },
                success: function (data) {
                    ddltestCategory.html('');
                    ddlPatient.html('');
                    $('#orderdetailsItems').empty();
                   
                    var mainrow = $('#orderdetailsItems');
                    $.each(data, function (id, option) {
                        ddltestCategory.append($('<option></option>').val(option.testcatId).html(option.testCategory));
                        ddlPatient.append($('<option></option>').val(option.patientId).html(option.patientName));
                        $('#regNo').val(option.regNo);
                        $('#TestDate').val(ToJavaScriptDate(option.testdate));
                        $('#DeliveryDate').val(ToJavaScriptDate(option.deliveryDate));

                        var $newRow = $('#mainrow').clone().removeAttr('id');
                        $('.pc', $newRow).val(option.testId);
                        $('#normalRange', $newRow).val(option.normalrange);
                        $('#actualRange', $newRow).val('');
                        $('#testunit', $newRow).val(option.unit);

                        $('#add', $newRow).addClass('remove').val('Remove').removeClass('btn-success').addClass('btn-danger');
                        $('#testCategory,#normalRange,#actualRange,#testUnit,#add', $newRow).removeAttr('id');
                        $('span.error', $newRow).remove();

                        mainrow.append($newRow);
                    });
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert('Found error to load data!!.');
                }
            });
        }
        else {
            alert('Please enter some value.');
        }

    });


    $('#testCategory').change(function () {
        var selectedItemValue = $(this).val();
        $.ajax({
            url: "/LabTestModule/TestResult/GetRangeById",
            type: "GET",
            data: { Id: selectedItemValue },
            success: function (data) {

                $('#normalRange').val(data.normalRange);
                $('#testunit').val(data.unit);

            },
            error: function () {
                alert("Error! Fetching records.")
            }
        });
    });

    var pageURL = $(location).attr("href");
    var lastIndex = pageURL.lastIndexOf('/');
    var id = pageURL.substring(lastIndex + 1);
    if (id > 0) {
        $.ajax({
            url: "/LabTestModule/TestResult/GetTestResultDetailById",
            type: "GET",
            data: { id: id },
            success: function (data) {
                GetTestResultDetail(data);
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

    function GetTestResultDetail(data) {
        var mainrow = $('#orderdetailsItems');
        var ddlPatient = $("#PatientId");
        $.each(data, function (index, item) {
            ddlPatient.html('');
            //assign master values           
            $('#LabTestId').val(item.labtestId);
            $('#LabTestCatId').val(item.labtestcatid);
            $('#PatientId').val(item.patientId);
            //$('#PatientId').text(item.patientName);
             ddlPatient.append($('<option></option>').val(item.patientId).html(item.patientName));
            $('#TestDate').val(ToJavaScriptDate(item.testdate));
            $('#DeliveryDate').val(ToJavaScriptDate(item.deliverydate));
            $('#regNo').val(item.regNo);

            //assign details values
            var $newRow = $('#mainrow').clone().removeAttr('id');
            $('.pc', $newRow).val(item.testId);
            $('#normalRange', $newRow).val(item.normalRange);
            $('#actualRange', $newRow).val(item.actualRange);
            $('#testunit', $newRow).val(item.unitName);

            $('#add', $newRow).addClass('remove').val('Remove').removeClass('btn-success').addClass('btn-danger');
            $('#testCategory,#normalRange,#actualRange,#testUnit,#add', $newRow).removeAttr('id');
            $('span.error', $newRow).remove();

            mainrow.append($newRow);

        });
    }

    $('#add').click(function () {
        var isAllValid = true;
        if ($('#testCategory').val() == 0) {
            isAllValid = false;
            $('#testCategory').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#testCategory').siblings('span.error').css('visibility', 'hidden');
        }

        if (!($('#normalRange').val().trim() != '')) {
            isAllValid = false;
            $('#normalRange').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#normalRange').siblings('span.error').css('visibility', 'hidden');
        }

        if (!($('#actualRange').val().trim() != '')) {
            isAllValid = false;
            $('#actualRange').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#actualRange').siblings('span.error').css('visibility', 'hidden');
        }

        if (!($('#testunit').val().trim() != '')) {
            isAllValid = false;
            $('#testunit').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#testunit').siblings('span.error').css('visibility', 'hidden');
        }



        if (isAllValid) {
            var $newRow = $('#mainrow').clone().removeAttr('id');
            $('.pc', $newRow).val($('#testCategory').val());

            $('#add', $newRow).addClass('remove').val('Remove').removeClass('btn-success').addClass('btn-danger');

            $('#testCategory,#normalRange,#actualRange,#testunit,#add', $newRow).removeAttr('id');
            $('span.error', $newRow).remove();

            $('#orderdetailsItems').append($newRow);
            $('#testCategory').val(0);

            $('#normalRange,#actualRange,#testunit').val('');
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
                $('.normalRange', this).val() == "" ||
                $('.actualRange', this).val() == "" ||
                $('.testUnit', this).val() == ""
                ) {
                errorItemCount++;
                $(this).addClass('error');
            } else {
                var detailsItems = {
                    TestId: $('select.pc', this).val(),
                    NormalRange: $('.normalRange', this).val(),
                    ActualRange: $('.actualRange', this).val(),
                    TestUnit: $('.testunit', this).val()

                }
                list.push(detailsItems);
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

        if ($('#LabTestId').val() == 0) {
            $('#LabTestId').siblings('span.error').css('visibility', 'visible');
            isAllValid = false;
        }
        else {
            $('#LabTestId').siblings('span.error').css('visibility', 'hidden');
        }

        if ($('#LabTestCatId').val() == 0) {
            $('#LabTestCatId').siblings('span.error').css('visibility', 'visible');
            isAllValid = false;
        }
        else {
            $('#LabTestCatId').siblings('span.error').css('visibility', 'hidden');
        }

        if ($('#PatientId').val() == 0) {
            $('#PatientId').siblings('span.error').css('visibility', 'visible');
            isAllValid = false;
        }
        else {
            $('#PatientId').siblings('span.error').css('visibility', 'hidden');
        }

        if ($('#TestDate').val().trim() == '') {
            $('#TestDate').siblings('span.error').css('visibility', 'visible');
            isAllValid = false;
        }
        else {
            $('#TestDate').siblings('span.error').css('visibility', 'hidden');
        }

        if ($('#DeliveryDate').val().trim() == '') {
            $('#DeliveryDate').siblings('span.error').css('visibility', 'visible');
            isAllValid = false;
        }
        else {
            $('#DeliveryDate').siblings('span.error').css('visibility', 'hidden');
        }

        if (isAllValid) {
            var data = {
                LabTestId: $('#LabTestId').val(),
                LabTestCatId: $('#LabTestCatId').val(),
                PatientId: $('#PatientId').val(),
                regNo: $('#regNo').val(),
                TestDate: $('#TestDate').val(),
                DeliveryDate: $('#DeliveryDate').val(),
                TestResultDetails: list
            }

            $(this).val('Please wait...');

            $.ajax({
                contentType: 'application/json',
                type: 'POST',
                dataType: 'json',
                url: '/LabTestModule/TestResult/save',
                data: JSON.stringify(data),
                success: function (data) {
                    if (data.status) {
                        alert('Successfully saved');
                        list = [];
                        $('#LabTestId').val(0);
                        $('#LabTestCatId').val(0);
                        $('#PatientId').val(0);
                        $('#regNo').val('');
                        $('#TestDate').val('');
                        $('#DeliveryDate').val('');
                        $('#orderdetailsItems').empty();
                        window.location.href = '/LabTestModule/TestResult/Index'
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