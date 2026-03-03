$(document).ready(function () {

    $('#addToList').click(function () {
        var isAllValid = true;

        if ($('#PatientId').val() == 0) {
            isAllValid = false;
            $('#PatientId').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#PatientId').siblings('span.error').css('visibility', 'hidden');
        }

        if ($('#RegDate').val().trim() == '') {
            isAllValid = false;
            $('#RegDate').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#RegDate').siblings('span.error').css('visibility', 'hidden');
        }

        if ($('#deliveryDate').val().trim() == '') {
            isAllValid = false;
            $('#deliveryDate').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#deliveryDate').siblings('span.error').css('visibility', 'hidden');
        }

        if ($('#PrescribedById').val() == 0) {
            isAllValid = false;
            $('#PrescribedById').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#PrescribedById').siblings('span.error').css('visibility', 'hidden');
        }

        if ($('#TestConductById').val() == 0) {
            isAllValid = false;
            $('#TestConductById').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#TestConductById').siblings('span.error').css('visibility', 'hidden');
        }

        if ($('#CategoryId').val() == 0) {
            isAllValid = false;
            $('#CategoryId').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#CategoryId').siblings('span.error').css('visibility', 'hidden');
        }

        if ($('#TestId').val().trim() == '') {
            isAllValid = false;
            $('#TestId').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#TestId').siblings('span.error').css('visibility', 'hidden');
        }

        if (isAllValid) {
            if ($('#TestId').val() == '-1') {
                $.ajax({
                    url: "/LabTestModule/LabRegistration/GetAllTest",
                    type: "GET",
                    data: { Id: $('#CategoryId').val() },
                    success: function (data) {
                        GetAllTest(data);
                    },
                    error: function () {
                        alert("Error! Fetching records.")

                    }
                });
            }
            else {
                var patientId = $("#PatientId option:selected").val(),
                    pateintName = $("#PatientId option:selected").text(),
                    regNo = $("#RegNo").val(),
                    regDate = $("#RegDate").val(),
                    deliveryDate = $("#deliveryDate").val(),
                    PresId = $("#PrescribedById option:selected").val(),
                    PrescribedBy = $("#PrescribedById option:selected").text(),
                    CondId = $("#TestConductById option:selected").val(),
                    ConductBy = $("#TestConductById option:selected").text(),
                    testId = $("#TestId option:selected").val(),
                    testName = $("#TestId option:selected").text(),
                    detailsTableBody = $("#demoGrid tbody");

                var productItem = '<tr><td style="display:none;">' + patientId + '</td><td>' + pateintName + '</td><td>' + regNo + '</td><td>' + regDate + '</td><td>' + deliveryDate + '</td><td style="display:none;">' + testId + '</td><td>' + testName + '</td><td style="display:none;">' + PresId + '</td><td>' + PrescribedBy + '</td><td style="display:none;">' + CondId + '</td><td>' + ConductBy + '</td><td><a data-itemId="0" href="#" class="deleteItem">Remove</a></td></tr>';
                detailsTableBody.append(productItem);
            }

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
                PatientId: $(this).find('td:eq(0)').html(),
                RegNo: $(this).find('td:eq(2)').html(),
                RegDate: $(this).find('td:eq(3)').html(),
                deliveryDate: $(this).find('td:eq(4)').html(),
                TestId: $(this).find('td:eq(5)').html(),
                PrescribedById: $(this).find('td:eq(7)').html(),
                TestConductById: $(this).find('td:eq(9)').html(),
            });
        });

        var data = JSON.stringify({
            reg: list
        });

        $.when(savePresc(data)).then(function (response) {
            console.log(response);
            window.location.href = '/LabTestModule/LabRegistration/Index'

        }).fail(function (err) {
            console.log(err);
        });


        function savePresc(data) {
            return $.ajax({
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                type: 'POST',
                url: "/LabTestModule/LabRegistration/save",
                data: data,
                success: function (result) {
                    alert(result);
                    location.reload();
                },
                error: function () {
                    alert('Error!')
                }
            });
        }
    });

    var pageURL = $(location).attr("href");
    var lastIndex = pageURL.lastIndexOf('/');
    var regno = pageURL.substring(lastIndex + 1);
    if (regno != 'Create') {
        $.ajax({
            url: "/LabTestModule/LabRegistration/GetLabRecordByRegNo",
            type: "GET",
            data: { RegNo: regno },
            success: function (data) {
                GetLabRegistration(data);
                $('#PatientId').prop("disabled", true);
                $('#RegDate').prop("disabled", true);
                $('#deliveryDate').prop("disabled", true);
                $('#PrescribedById').prop("disabled", true);
                $('#TestConductById').prop("disabled", true);
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

    function GetAllTest(data) {
        var tbldetailsTable = $('#demoGrid');
        var pId = $("#PatientId option:selected").val();
        var pName = $("#PatientId option:selected").text();
        var rNo;
        if ($("#RegNo").val().trim() == '') {
            rNo = '';
        }
        else {
            rNo = $("#RegNo").val();
        }
        var rDate = $("#RegDate").val();
        var dDate = $("#deliveryDate").val();
        var prsId = $("#PrescribedById option:selected").val();
        var prsby = $("#PrescribedById option:selected").text();
        var condId = $("#TestConductById option:selected").val();
        var condBy = $("#TestConductById option:selected").text();
        $.each(data, function (index, item) {
            var tr = $("<tr></tr>");
            tr.html(("<td style=" + "display:none;" + ">" + pId + "</td>")
                 + " " + ("<td>" + pName + "</td>")
                 + " " + ("<td>" + rNo + "</td>")
                 + " " + ("<td>" + rDate + "</td>")
                 + " " + ("<td>" + dDate + "</td>")
                 + " " + ("<td style=" + "display:none;" + ">" + item.testId + "</td>")
                 + " " + ("<td>" + item.testName + "</td>")
                 + " " + ("<td style=" + "display:none;" + ">" + prsId + "</td>")
                 + " " + ("<td>" + prsby + "</td>")
                 + " " + ("<td style=" + "display:none;" + ">" + condId + "</td>")
                 + " " + ("<td>" + condBy + "</td>")
                 + " " + ("<td><a data-itemId='0' href='#' class='deleteItem'>Remove</a></td>")
                 );
            tbldetailsTable.append(tr);

        });
    }


    function GetLabRegistration(data) {
        var tbldetailsTable = $('#demoGrid');
        $.each(data, function (index, item) {
            $('#PatientId').val(item.patientId);
            $('#RegNo').val(item.regno);
            $('#RegDate').val(ToJavaScriptDate(item.regdate));
            $('#deliveryDate').val(ToJavaScriptDate(item.deliveryDate));
            $('#PrescribedById').val(item.prescribId);
            $('#TestConductById').val(item.conductbyId);


            var tr = $("<tr></tr>");
            tr.html(("<td style=" + "display:none;" + ">" + item.patientId + "</td>")
                  + " " + ("<td>" + item.patientName + "</td>")
                  + " " + ("<td>" + item.regno + "</td>")
                  + " " + ("<td>" + ToJavaScriptDate(item.regdate) + "</td>")
                  + " " + ("<td>" + ToJavaScriptDate(item.deliveryDate) + "</td>")
                  + " " + ("<td style=" + "display:none;" + ">" + item.testId + "</td>")
                  + " " + ("<td>" + item.testName + "</td>")
                  + " " + ("<td style=" + "display:none;" + ">" + item.prescribId + "</td>")
                  + " " + ("<td>" + item.prescribBy + "</td>")
                  + " " + ("<td style=" + "display:none;" + ">" + item.conductbyId + "</td>")
                  + " " + ("<td>" + item.conductedBy + "</td>")
                  + " " + ("<td><a data-itemId='0' href='#' class='deleteItem'>Remove</a></td>")
                  );
            tbldetailsTable.append(tr);
        });
    }
});