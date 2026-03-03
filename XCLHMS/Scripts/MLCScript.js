$(document).ready(function () {

    var pageURL = $(location).attr("href");
    var lastIndex = pageURL.lastIndexOf('/');
    var id = pageURL.substring(lastIndex + 1);
    if (id > 0) {
        $.ajax({
            url: "/MLCModule/MLC/GetMLCDetail",
            type: "GET",
            data: { id: id },
            success: function (data) {
                GetMLCDetail(data);
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


    function GetMLCDetail(data) {
        var mainrow = $('#orderdetailsItems');
        $.each(data, function (index, item) {
            $('#MlcNo').val(item.mlcNo);
            $('#PatientName').val(item.patientName);
            $('#Title').val(item.title);
            $('#GuardianName').val(item.guardianName);
            $('#MLCType').val(item.mlctype);
            $('#Age').val(item.age);
            $('#Gender').val(item.gender);
            $('#PS').val(item.ps);
            $('#Address').val(item.address);
            $('#IdentifyMarks').val(item.identifyMarks);
            $('#LetterNo').val(item.letterNo);
            $('#Dated').val(ToJavaScriptDate(item.dated));
            $('#IncidentDateTime').val(ToJavaScriptDate(item.incidentDt));
            $('#ArrivalDateTime').val(ToJavaScriptDate(item.arrivalDt));
            $('#Place').val(item.place);
            $('#Blonging').val(item.blonging);
            $('#History').val(item.history);
            $('#Injuries').val(item.injury);
            $('#Xray').val(item.xray);

            var $newRow = $('#mainrow').clone().removeAttr('id');
            $('#injuryNo', $newRow).val(item.injuryNo);
            $('#injuryNature', $newRow).val(item.injuryNature);
            $('#weapon', $newRow).val(item.weapon);
            $('#injuryDur', $newRow).val(item.duration);

            $('#add', $newRow).addClass('remove').val('Remove').removeClass('btn-success').addClass('btn-danger');
            $('#injuryNo,#injuryNature,#weapon,#injuryDur,#add', $newRow).removeAttr('id');
            $('span.error', $newRow).remove();

            mainrow.append($newRow);

        });
    }

    $('#add').click(function () {
        var isAllValid = true;
        if (!($('#injuryNo').val().trim() != '')) {
            isAllValid = false;
            $('#injuryNo').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#injuryNo').siblings('span.error').css('visibility', 'hidden');
        }

        if (!($('#injuryNature').val().trim() != '')) {
            isAllValid = false;
            $('#injuryNature').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#injuryNature').siblings('span.error').css('visibility', 'hidden');
        }

        if (!($('#weapon').val().trim() != '')) {
            isAllValid = false;
            $('#weapon').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#weapon').siblings('span.error').css('visibility', 'hidden');
        }

        if (!($('#injuryDur').val().trim() != '')) {
            isAllValid = false;
            $('#injuryDur').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#injuryDur').siblings('span.error').css('visibility', 'hidden');
        }

        if (isAllValid) {
            var $newRow = $('#mainrow').clone().removeAttr('id');
            $('#add', $newRow).addClass('remove').val('Remove').removeClass('btn-success').addClass('btn-danger');
            $('#injuryNo,#injuryNature,#weapon,#injuryDur,#add', $newRow).removeAttr('id');
            $('span.error', $newRow).remove();
            $('#orderdetailsItems').append($newRow);
            $('#injuryNo,#injuryNature,#weapon,#injuryDur').val('');
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
            if ($('.injuryNo', this).val() == "" || $('.injuryNature', this).val() == "" ||
                $('.weapon', this).val() == "" || $('.injuryDur', this).val() == "") {
                errorItemCount++;
                $(this).addClass('error');
            } else {
                var Items = {
                    InjuryNo: $('.injuryNo', this).val(),
                    InjuryNature: $('.injuryNature', this).val(),
                    Weapon: $('.weapon', this).val(),
                    Duration: $('.injuryDur', this).val(),
                }
                list.push(Items);
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

        if ($('#PatientName').val() == '') {
            $('#PatientName').siblings('span.error').css('visibility', 'visible');
            isAllValid = false;
        }
        else {
            $('#PatientName').siblings('span.error').css('visibility', 'hidden');
        }

        if ($('#LetterNo').val() == '') {
            $('#LetterNo').siblings('span.error').css('visibility', 'visible');
            isAllValid = false;
        }
        else {
            $('#LetterNo').siblings('span.error').css('visibility', 'hidden');
        }

        if (isAllValid) {
            var data = {
                MlcNo: $('#MlcNo').val(),
                PatientName: $('#PatientName').val(),
                Title: $('#Title').val(),
                GuardianName: $('#GuardianName').val(),
                MLCType: $('#MLCType').val(),
                Age: $('#Age').val(),
                Gender: $('#Gender').val(),
                PS: $('#PS').val(),
                Address: $('#Address').val(),
                IdentifyMarks: $('#IdentifyMarks').val(),
                LetterNo: $('#LetterNo').val(),
                Dated: $('#Dated').val(),
                IncidentDateTime: $('#IncidentDateTime').val(),
                ArrivalDateTime: $('#ArrivalDateTime').val(),
                Place: $('#Place').val(),
                Blonging: $('#Blonging').val(),
                History: $('#History').val(),
                Injuries: $('#Injuries').val(),
                Xray: $('#Xray').val(),
                MLCDetails: list
            }

            $(this).val('Please wait...');

            $.ajax({
                contentType: 'application/json',
                type: 'POST',
                dataType: 'json',
                url: '/MLCModule/MLC/save',
                data: JSON.stringify(data),
                success: function (data) {
                    if (data.status) {
                        alert('Successfully saved');
                        list = [];
                        $('#MlcNo').val('');
                        $('#PatientName').val('');
                        $('#Title').val(0);
                        $('#GuardianName').val('');
                        $('#MLCType').val(0);
                        $('#Age').val('');
                        $('#Gender').val(0);
                        $('#PS').val('');
                        $('#Address').val('');
                        $('#IdentifyMarks').val('');
                        $('#LetterNo').val('');
                        $('#Dated').val('');
                        $('#IncidentDateTime').val('');
                        $('#ArrivalDateTime').val('');
                        $('#Place').val('');
                        $('#Blonging').val('');
                        $('#History').val('');
                        $('#Injuries').val('');
                        $('#Xray').val('');


                        $('#orderdetailsItems').empty();
                        window.location.href = '/MLCModule/MLC/Index'
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