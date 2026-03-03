
$(document).ready(function () {

    //var _totalAmount = 0.00;

    var id = $('#POId').val();
    if (id && id > 0) {
        $.ajax({
            url: "/InventoryModule/PO/GetPODetail",
            type: "GET",
            data: { id: id },
            success: function (data) {
                GetPODetail(data);
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

    function GetPODetail(data) {
        var mainrow = $('#orderdetailsItems');
        $.each(data, function (index, item) {
            $('#VendorId').val(item.vId);
            $('#PONO').val(item.poNo);
            $('#PQDate').val(ToJavaScriptDate(item.poDate));

            var $newRow = $('#mainrow').clone().removeAttr('id');
            $newRow.find('span.error').remove();

            // Tender
            $('.Tender', $newRow).val(item.Tender_ || '');

            // AU
            var $auSelect = $('.AU', $newRow);
            if ($auSelect.find('option[value="' + item.AUId + '"]').length === 0) {
                $auSelect.append($('<option>').val(item.AUId).text(item.AUName));
            }
            $auSelect.val(item.AUId);

            // Populate Product based on AU before setting value
            var $prod = $('.pc', $newRow);
            $.ajax({
                cache: false,
                async: false,
                type: "GET",
                url: "/InventoryModule/PO/GetItemsById",
                data: { id: item.AUId },
                success: function (products) {
                    $prod.empty().append($('<option></option>').val(0).text("--Select--"));
                    $.each(products, function (id, option) {
                        $prod.append($('<option></option>').val(option.id).text(option.name));
                    });
                    $prod.val(item.productId);
                }
            });

            // Dosage
            var $dos = $('.d', $newRow);
            $dos.empty().append($('<option></option>').val(item.DosageId).text(item.DosageName)).val(item.DosageId);

            // Brand
            var $bn = $('.BN', $newRow);
            $bn.empty().append($('<option></option>').val(item.BrandId).text(item.BrandName)).val(item.BrandId);

            // Manufacture
            var $mfg = $('.MFG', $newRow);
            $mfg.empty().append($('<option></option>').val(item.ManufactureId).text(item.ManufactureName)).val(item.ManufactureId);

            // Quantity + Remarks
            $('.quantity', $newRow).val(item.qty);
            $('.rmks', $newRow).prop('checked', Boolean(item.Remarks && item.Remarks != "0"));

            // Change Add -> Remove button
            $('#add', $newRow).addClass('remove').val('Remove').removeClass('btn-success').addClass('btn-danger');
            $newRow.find('#Tender,#Dosage,#AU,#productCategory,#BrandName,#Manufacture,#quantity,#add').removeAttr('id');
            mainrow.append($newRow);
        });
    }

    // ✅ When AU changes, load products
    $(document).on('change', '.AU', function () {
        var $row = $(this).closest('tr');
        var selectedAU = $(this).val();
        var $productDropdown = $row.find('.pc');

        if (selectedAU && selectedAU != "0") {
            $.ajax({
                type: "GET",
                url: "/InventoryModule/PO/GetItemsById",
                data: { id: selectedAU },
                success: function (data) {
                    $productDropdown.empty().append($('<option>').val(0).text("--Select--"));
                    $.each(data, function (i, option) {
                        $productDropdown.append($('<option>').val(option.id).text(option.name));
                    });
                    $productDropdown.prop('disabled', false);
                },
                error: function () {
                    alert('Error loading products for selected AU.');
                }
            });
        } else {
            $productDropdown.empty().append($('<option>').val(0).text("--Select--")).prop('disabled', true);
        }
    });


    // ✅ When Product changes, load Dosage / Brand / Manufacture
    // ✅ When Product changes, load Dosage / Brand / Manufacture properly
    $(document).on('change', '.pc', function () {
        var $row = $(this).closest('tr');
        var productId = $(this).val();

        var $dos = $row.find('.d');
        var $bn = $row.find('.BN');
        var $mfg = $row.find('.MFG');

        if (productId && productId != "0") {
            $.ajax({
                url: '/InventoryModule/PO/GetItemDetailsById',
                type: 'GET',
                data: { id: productId },
                success: function (response) {
                    if (response.success && response.data) {
                        // ✅ Populate but keep them disabled
                        $dos.empty().append($('<option></option>').val(response.data.DosageId).text(response.data.DosageName))
                            .prop('disabled', true);
                        $bn.empty().append($('<option></option>').val(response.data.BrandId).text(response.data.BrandName))
                            .prop('disabled', true);
                        $mfg.empty().append($('<option></option>').val(response.data.ManufactureId).text(response.data.ManufactureName))
                            .prop('disabled', true);
                    } else {
                        $dos.val(0); $bn.val(0); $mfg.val(0);
                    }
                },
                error: function () {
                    alert('Error fetching product details.');
                }
            });
        } else {
            $dos.val(0); $bn.val(0); $mfg.val(0);
        }
    });








    $('#add').click(function () {
        var isAllValid = true;
        if ($('#AU').val() == 0) {
            isAllValid = false;
            $('#AU').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#AU').siblings('span.error').css('visibility', 'hidden');
        }

        if ($('#productCategory').val() == 0) {
            isAllValid = false;
            $('#productCategory').siblings('span.error').css('visibility', 'visible');
        }
        else {
            $('#productCategory').siblings('span.error').css('visibility', 'hidden');
        }

        var qtyVal = $.trim($('#quantity').val());
        if (qtyVal === '' || isNaN(qtyVal) || parseInt(qtyVal) <= 0) {
            isAllValid = false;
            $('#quantity').siblings('span.error').css('visibility', 'visible');
        } else {
            $('#quantity').siblings('span.error').css('visibility', 'hidden');
        }

        if (isAllValid) {
            var $newRow = $('#mainrow').clone().removeAttr('id');
            $('.d', $newRow).val($('#Dosage').val());
            $('.AU', $newRow).val($('#AU').val());
            $('.pc', $newRow).val($('#productCategory').val());
            $('.BN', $newRow).val($('#BrandName').val());
            $('.MFG', $newRow).val($('#Manufacture').val());

            var qtyValue = $('#quantity').val();
            console.log("Copying quantity:", qtyValue);
            $newRow.find('.quantity').val(qtyValue);

            $('.rmks', $newRow).prop('checked', $('#Remarks').is(':checked'));

            $('#add', $newRow).addClass('remove').val('Remove').removeClass('btn-success').addClass('btn-danger');

            $newRow.find('#Tender,#Dosage,#AU,#productCategory,#BrandName,#Manufacture,#quantity,#add').removeAttr('id');
            $('span.error', $newRow).remove();

            $('#orderdetailsItems tbody').append($newRow);

            $('#Dosage').val(0);
            $('#AU').val(0);
            $('#productCategory').val(0);
            console.log("Quantity entered:", $('#quantity').val());
            console.log("Quantity cloned to new row:", $('.quantity', $newRow).val());
            $('#Tender,#quantity').val('');
            $('#BrandName').val(0);
            $('#Manufacture').val(0);
            $('#orderItemError').empty();
        }
    });

    $('#orderdetailsItems').on('click', '.remove', function () {
        $(this).closest('tr').remove();
    });


    $('#submit').click(function () {
        var isAllValid = true;
        $('#orderItemError').text('');
        var list = [];
        var errorItemCount = 0;

        $('#orderdetailsItems tbody tr').each(function (index, ele) {
            if ($('select.Tender', this).val() == 0 ||
                $('select.AU', this).val() == 0 ||
                $('select.d', this).val() == 0 ||
                $('select.pc', this).val() == 0 ||
                (parseInt($('.quantity', this).val()) || 0) == 0 ||
                $('select.BN', this).val() == 0 ||
                $('select.MFG', this).val() == 0
            ) {
                errorItemCount++;
                $(this).addClass('error');
            } else {
                var POItems = {
                    Tender_: $('.Tender', this).val(),
                    DosageId: $('select.d', this).val(),
                    AUId: $('select.AU', this).val(),
                    ProductId: $('select.pc', this).val(),
                    BrandId: $('select.BN', this).val(),
                    ManufactureId: $('select.MFG', this).val(),
                    Qty: parseInt($('.quantity', this).val()),
                    Remarks: $('.rmks', this).is(':checked')

                }
                list.push(POItems);
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


        if ($('#VendorId').val() == 0) {
            $('#VendorId').siblings('span.error').css('visibility', 'visible');
            isAllValid = false;
        }
        else {
            $('#VendorId').siblings('span.error').css('visibility', 'hidden');
        }


        if ($('#PQDate').val().trim() == '') {
            $('#PQDate').siblings('span.error').css('visibility', 'visible');
            isAllValid = false;
        }
        else {
            $('#PQDate').siblings('span.error').css('visibility', 'hidden');
        }

        if (isAllValid) {
            var data = {
                Id: $('#POId').val(),
                VendorID: $('#VendorId').val(),
                PONO: $('#PONO').val().trim(),
                PQDate: $('#PQDate').val(),
                PODetails: list
            }

            $(this).val('Please wait...');

            $.ajax({
                contentType: 'application/json',
                type: 'POST',
                dataType: 'json',
                url: '/InventoryModule/PO/save',
                data: JSON.stringify(data),
                success: function (data) {
                    if (data.status) {
                        alert('Successfully saved');
                        list = [];
                        $('#VendorId').val(0);
                        $('#PONO').val('');
                        $('#PQDate').val('');
                        $('#orderdetailsItems').empty();
                        window.location.href = '/InventoryModule/PO/Index'
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

