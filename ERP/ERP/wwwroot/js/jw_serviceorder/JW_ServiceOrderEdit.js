$(document).ready(function () {

    //#region Initialize Flatpickr
    InitializeGstFlatpickrs();

    function InitializeGstFlatpickrs() {
        $(".datepicker").flatpickr({
            dateFormat: "d-M-Y",   // 30-Apr-2026
            altInput: true,        // shows formatted date
            altFormat: "d-M-Y",   // display format
            allowInput: true,     // user can type manually
            defaultDate: new Date() // optional: today default
        });
    }
    DateBind();
    //#region onkeypress qty and unit
    $(document).on("blur", ".JISVOI_Qty", function () {

        let $row = $(this).closest("tr");

        let invoicedQty = parseFloat($row.find(".InvoicedQty").val()) || 0;
        let invoiceToBeRaised = parseFloat($row.find(".InvoiceToBeRaised").val()) || 0;
        let amendQty = parseFloat($(this).val()) || 0;

        let minQty = invoicedQty + invoiceToBeRaised;

        if (amendQty < minQty) {
            alert(`Amend Qty cannot be less than ${minQty}`);
            $(this).val(minQty);
            $(this).focus(); // optional: focus back
        }
    });
    $(document).on("keyup change", ".JISVOI_Qty, .JISVOI_UnitPrice", function () {

        let row = $(this).closest("tr");    

        let qty = parseFloat(row.find(".JISVOI_Qty").val()) || 0;
        let price = parseFloat(row.find(".JISVOI_UnitPrice").val()) || 0;

        let amount = qty * price;

        // Only set row amount (read-only field)
        row.find(".JISVOI_Amount").val(amount.toFixed(2));

        // update footer totals separately
        calculateTotal();

        // auto add row
        autoAddRow(row);

    });
    //#endregion
    //#region add row item grid
    let rowIndex = 1; // start from 1 because 0 already exists

    $("#AddRowButton").on("click", function () {

        let isValid = true;

        $("#ItemTable tbody tr.NewRow:last").find("input, select").each(function () {

            let el = $(this);

            // skip hidden delete flag
            if (el.hasClass("JISVOI_IsDeleted")) return;

            if (el.hasClass("JISVOI_Item_Code")) {
                if (!el.val()) {
                    isValid = false;
                    el.focus();
                    return false;
                }
            }

            if (el.hasClass("JISVOI_Qty")) {
                if (!el.val() || parseFloat(el.val()) <= 0) {
                    isValid = false;
                    el.focus();
                    return false;
                }
            }

            if (el.hasClass("JISVOI_UnitPrice")) {
                if (!el.val() || parseFloat(el.val()) <= 0) {
                    isValid = false;
                    el.focus();
                    return false;
                }
            }

            if (el.hasClass("JISVOI_PRS_Number")) {
                if (!el.val() || el.val() === "0") {
                    isValid = false;
                    el.focus();
                    return false;
                }
            }

        });

        if (!isValid) {
            alert("Please fill required fields before adding new row.");
            return;
        }

        let $newRow = $("#TempRow").clone();

        $newRow.removeAttr("id");
        $newRow.removeAttr("style");
        $newRow.addClass("NewRow");

        $newRow.find("input, select").each(function () {

            let el = $(this);

            if (el.attr("type") === "checkbox") {
                el.prop("checked", false);
            }

            if (!el.hasClass("JISVOI_IsDeleted")) {
                el.val("");
            }

            let name = el.attr("name");
            if (name) {
                let updatedName = name.replace(/\[\d+\]/, `[${rowIndex}]`);
                el.attr("name", updatedName);
            }
        });

        let rowID = new Date().getTime();

        $newRow.attr("data-rowid", rowID);

        $("#TableBody").append($newRow);

        rowIndex++;

        calculateTotal();
    });
    //#endregion add row item grid

    $(document).on("click", ".RowRemove", function () {

        let row = $(this).closest("tr");

        row.find(".JISVOI_IsDeleted").val("1");
        row.hide();

        calculateTotal();
    });

    //#region Save Function
    $("#btnSave").on("click", function (e) {

        if (!validateHeaderById()) {
            e.preventDefault();
            return false;
        }

        let model = CreateServiceOrderModel();

        console.log(JSON.stringify(model));

        $.ajax({
            url: '/ServiceOrder/SaveServiceOrder',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(model),

            success: function (response) {

                if (response.success) {
                    showAlert('Record Inserted');
                    ClearAll();
                    DateBind();
                    console.log(model);
                }
            },

            error: function (xhr) {
                console.log(xhr.responseText);
            }
        });

    });
    //#endregion

    //#region remove checked rows
    $("#RemoveItemRowButton").on("click", function () {

        let checkedRows =
            $("#ItemTable tbody tr.NewRow:visible")
                .has(".CheckItem:checked");

        let totalVisibleRows =
            $("#ItemTable tbody tr.NewRow:visible").length;

        if (checkedRows.length === 0) {
            alert("Please select row.");
            return;
        }

        if ((totalVisibleRows - checkedRows.length) <= 0) {
            alert("At least one row required.");
            return;
        }

        if (checkedRows.length > 1) {
            alert("Please select only one row");
            return;
        }

        checkedRows.each(function () {

            let currentRow = $(this);

            let itemNumber =
                currentRow.find(".JISVOI_Number").val();

            // already saved row → soft delete
            if (itemNumber && itemNumber !== "0") {

                currentRow
                    .find(".JISVOI_IsDeleted")
                    .val("1");

                currentRow.hide();
            }
            else {
                // new unsaved row → hard delete
                currentRow.remove();
            }

        });

        calculateTotal();
    });
    //#endregion

    const params =
        new URLSearchParams(
            window.location.search);

    const siNo =
        params.get(
            "SI_No");

    if (siNo) {
        GetServiceOrder(siNo);
    }
});
//#region auto add row function
function autoAddRow(currentRow) {

    let qty = parseFloat(currentRow.find(".JISVOI_Qty").val()) || 0;
    let price = parseFloat(currentRow.find(".JISVOI_UnitPrice").val()) || 0;

    let itemCode = currentRow.find(".JISVOI_Item_Code").val();
    let prsNo = currentRow.find(".JISVOI_PRS_Number").val();

    // validate current row
    let isRowValid =
        itemCode &&
        qty > 0 &&
        price > 0 &&
        prsNo &&
        prsNo !== "0";

    // allow only last row
    let isLastRow =
        currentRow.is("#ItemTable tbody tr.NewRow:last");

    if (isRowValid && isLastRow) {

        // prevent multiple empty rows
        let nextRow = currentRow.next("tr");

        if (nextRow.length === 0) {

            $("#AddRowButton").trigger("click");
        }
    }
}
//#endregion auto add row function

//#region clear all
function ClearAll() {
    $(".left-menu")
        .find("input, textarea, select")
        .each(function () {

            if ($(this).is(":hidden")) {
                $(this).val("");
            }
            else if ($(this).is("select")) {
                $(this).prop("selectedIndex", 0);
            }
            else {
                $(this).val("");
            }
        });
    $("#ItemTable tbody").empty();
    $(".jwcustomer-search-results").hide().html("");

}
//#endregion
function DateBind() {
    var today = new Date();

    var day = String(today.getDate()).padStart(2, '0');

    var months = [
        "Jan", "Feb", "Mar", "Apr", "May", "Jun",
        "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
    ];

    var formattedDate =
        day + "-" + months[today.getMonth()] + "-" + today.getFullYear();

    var regDate = document.getElementById("Header_JISVOH_RegDate")?._flatpickr;
    if (regDate)
        regDate.setDate(formattedDate, true, "d-M-Y");

    var serviceOrderDate = document.getElementById("Header_JISVOH_ServiceOrderDate")?._flatpickr;
    if (serviceOrderDate)
        serviceOrderDate.setDate(formattedDate, true, "d-M-Y");
    console.log(formattedDate +'--formattedDate')
}

function CreateServiceOrderModel() {

    // =========================
    // HEADER
    // =========================
    let header = {

        JISVOH_Number:
            parseInt($("#Header_JISVOH_Number").val()) || 0,

        JISVOH_RegNo:
            $("#Header_JISVOH_RegNo").val(),

        JISVOH_RegDate:
            $("#Header_JISVOH_RegDate").val()
                ? new Date($("#Header_JISVOH_RegDate").val()).toISOString()
                : null,

        JISVOH_ServiceOrderNo:
            $("#Header_JISVOH_ServiceOrderNo").val(),

        JISVOH_ServiceOrderDate:
            $("#Header_JISVOH_ServiceOrderDate").val()
                ? new Date($("#Header_JISVOH_ServiceOrderDate").val()).toISOString()
                : null,

        JISVOH_JW_Customer_Number:
            parseInt($("#Header_JISVOH_JW_Customer_Number").val()) || 0,

        JISVOH_JW_Customer_Name:
            $("#Header_JISVOH_JW_Customer_Name").val(),

        JISVOH_Currency_Number:
            parseInt($("#Header_JISVOH_Currency_Number").val()) || 0,

        JISVOH_PaymentTerms:
            $("#Header_JISVOH_PaymentTerms").val(),

        JISVOH_DeliveryTerms:
            $("#Header_JISVOH_DeliveryTerms").val(),

        JISVOH_DeliveryMode:
            $("#Header_JISVOH_DeliveryMode").val(),

        JISVOH_Tax:
            $("#Header_JISVOH_Tax").val(),

        JISVOH_TDC:
            $("#Header_JISVOH_TDC").val(),

        JISVOH_Remarks:
            $("#Header_JISVOH_Remarks").val(),

        SVO_Id:
            parseInt($("#Header_SVO_Id").val()) || 0,

        JISVOI_Item_Code:
            $("#Header_JISVOI_Item_Code").val()
    };

    // =========================
    // ITEMS
    // =========================
    let items = [];

    $("#ItemTable tbody tr.NewRow").each(function () {

        let row = $(this);

        // deleted rows skip
        if (row.find(".JISVOI_IsDeleted").val() == "1") {
            return;
        }

        // empty rows skip
        if (!row.find(".JISVOI_Item_Number").val()) {
            return;
        }

        let item = {

            JISVOI_JISVOH_Number:
                parseInt(row.find(".JISVOI_JISVOH_Number").val()) || 0,

            JISVOI_Number:
                parseInt(row.find(".JISVOI_Number").val()) || 0,

            JISVOI_PRS_Number:
                parseInt(row.find(".JISVOI_PRS_Number").val()) || 0,

            JISVOI_Item_Number:
                parseInt(row.find(".JISVOI_Item_Number").val()) || 0,

            JISVOI_UoM_Number:
                parseInt(row.find(".JISVOI_UoM_Number").val()) || 0,

            JISVOI_Qty:
                parseFloat(row.find(".JISVOI_Qty").val()) || 0,

            JISVOI_UnitPrice:
                parseFloat(row.find(".JISVOI_UnitPrice").val()) || 0,

            JISVOI_Amount:
                parseFloat(row.find(".JISVOI_Amount").val()) || 0,

            JISVOI_DeliveryDate:
                row.find(".JISVOI_DeliveryDate").val()
                    ? new Date(
                        row.find(".JISVOI_DeliveryDate").val()
                    ).toISOString()
                    : null
        };

        items.push(item);
    });

    // =========================
    // FINAL MODEL
    // =========================
    let serviceOrderModel = {
        Header: header,
        Items: items
    };

    console.log(serviceOrderModel);

    return serviceOrderModel;
}

//#region SUBMIT VALIDATION
function validateHeaderById() {

    // 1. Register No
    if ($("#Header_JISVOH_RegNo").val().trim() === "") {
        showAlert('Register No. is required', '#Header_JISVOH_RegNo');
        return false;
    }

    // 2. Register Date
    if ($("#Header_JISVOH_RegDate").val().trim() === "") {
        showAlert('Register Date is required', '#Header_JISVOH_RegDate');
        return false;
    }

    // 3. Service Order No
    if ($("#Header_JISVOH_ServiceOrderNo").val().trim() === "") {
        showAlert('Service Order No. is required', '#Header_JISVOH_ServiceOrderNo');
        return false;
    }

    // 4. Service Order Date
    if ($("#Header_JISVOH_ServiceOrderDate").val().trim() === "") {
        showAlert('Service Order Date is required', '#Header_JISVOH_ServiceOrderDate');
        return false;
    }

    // 5. JW Customer
    if (
        $("#Header_JISVOH_JW_Customer_Number").val().trim() === "" ||
        $("#Header_JISVOH_JW_Customer_Name").val().trim() === ""
    ) {
        showAlert(
            'JW Customer is required',
            '#Header_JISVOH_JW_Customer_Name'
        );
        return false;
    }

    // 6. Currency
    if (
        $("#Header_JISVOH_Currency_Number").val() === "" ||
        $("#Header_JISVOH_Currency_Number").val() === "0"
    ) {
        showAlert(
            'Currency is required',
            '#Header_JISVOH_Currency_Number'
        );
        return false;
    }

    //// 7. Payment Terms
    //if ($("#Header_JISVOH_PaymentTerms").val().trim() === "") {
    //    showAlert('Payment Terms is required', '#Header_JISVOH_PaymentTerms');
    //    return false;
    //}

    //// 8. Delivery Terms
    //if ($("#Header_JISVOH_DeliveryTerms").val().trim() === "") {
    //    showAlert('Delivery Terms is required', '#Header_JISVOH_DeliveryTerms');
    //    return false;
    //}

    //// 9. Delivery Mode
    //if ($("#Header_JISVOH_DeliveryMode").val().trim() === "") {
    //    showAlert('Delivery Mode is required', '#Header_JISVOH_DeliveryMode');
    //    return false;
    //}

    //// 10. Tax
    //if ($("#Header_JISVOH_Tax").val().trim() === "") {
    //    showAlert('Tax is required', '#Header_JISVOH_Tax');
    //    return false;
    //}

    //// 11. TDC
    //if ($("#Header_JISVOH_TDC").val().trim() === "") {
    //    showAlert('TDC is required', '#Header_JISVOH_TDC');
    //    return false;
    //}

    //// 12. Remarks
    //if ($("#Header_JISVOH_Remarks").val().trim() === "") {
    //    showAlert('Remarks is required', '#Header_JISVOH_Remarks');
    //    return false;
    //}

    // Grid Validation
    if (!validateItemGrid()) {
        return false;
    }

    return true;
}
//#endregion
//#region customer Search Functions
function OnJWCustomerInput(inputElement) {
    // alert('hi')
    SearchJWCustomer(inputElement);
}

function OnJWCustomerFocus(inputElement) {
    var value = inputElement.value;

    if (!value) {
        SearchJWCustomer(inputElement);
    } else {
        $(inputElement).select();
    }
}

async function SearchJWCustomer(inputElement) {

    var JWCustomer = inputElement.value;
    var RegDate = $("input[name='Header.JISVOH_RegDate']").val();

    var resultsDiv = $(inputElement)
        .closest(".col-sm-7, .col-md-6, .col-lg-7")
        .find(".jwcustomer-search-results");

    $.ajax({
        url: '/jobinward/transactions/delivery-note/cutomer',
        type: 'GET',
        data: {
            Buyer: JWCustomer,
            SIHDate: RegDate
        },
        success: function (data) {

            resultsDiv.empty();

            if (data && data.length > 0) {

                resultsDiv.show();

                var table = $(`
                    <div class="card-body modal-content p-0 w-100 position-absolute start-0 top-100"
                         style="z-index:999;">
                        <table class="table table-bordered table-hover table-fixed table-grid mb-0 w-100">
                            <thead>
                                <tr class="table-info">
                                    <th>JW Customer Name</th>
                                    <th class="text-center">Currency</th>
                                </tr>
                            </thead>
                            <tbody></tbody>
                        </table>
                    </div>
                `);

                data.forEach(function (cust) {

                    var row = $('<tr></tr>');

                    row.append('<td>' + cust.cuS_Name + '</td>');
                    row.append('<td class="text-center">' + cust.cuS_CUR_Name + '</td>');

                    table.find('tbody').append(row);

                    row.on('click', function () {

                        // Display customer name
                        $(inputElement).val(cust.cuS_Name);

                        // Hidden JW Customer ID
                        $("#Header_JISVOH_JW_Customer_Number")
                            .val(cust.cuS_Number);

                        // Currency dropdown
                        $("#Header_JISVOH_Currency_Number")
                            .val(cust.cuS_CUR_Number)
                            .trigger("change");

                        resultsDiv.hide();
                    });
                });

                var closeButton = $(`
                    <div class="card-header bg-primary py-1 px-1 d-flex justify-content-end">
                        <button type="button"
                                class="p-0 btn btn-sm btn-primary bg-opacity-10 d-flex align-items-center justify-content-center">
                            ✖
                        </button>
                    </div>
                `);

                closeButton.on('click', function () {
                    resultsDiv.hide();
                });

                resultsDiv.append(closeButton);
                resultsDiv.append(table);

            } else {
                resultsDiv.hide();
                resultsDiv.empty();
            }
        },
        error: function () {
            resultsDiv.text('Error loading data.').show();
        }
    });
}

async function SearchJWCustomer1(inputElement) {

    var JWCustomer = inputElement.value;
    var SIHDate = $("input[name='Header.JIDNH_DN_Date']").val();
    var resultsDiv = $(inputElement).closest(".col-md-6, .col-lg-7")
        .find(".jwcustomer-search-results");



    $.ajax({
        url: '/jobinward/transactions/delivery-note/cutomer',
        type: 'GET',
        data: {
            Buyer: JWCustomer,
            SIHDate: SIHDate
        },
        success: function (data) {

            resultsDiv.empty();

            if (data && data.length > 0) {

                resultsDiv.show();

                var table = $(`
    <div class="card-body modal-content p-0 w-100 position-absolute start-0 top-100"
         style="z-index:999;">
         
        <table class="table table-bordered table-hover table-fixed table-grid mb-0 w-100">
            <thead>
                <tr class="table-info">
                    <th>JW Customer Name</th>
                    <th class="text-center">Currency</th>
                </tr>
            </thead>
            <tbody></tbody>
        </table>

    </div>
`);

                data.forEach(function (cust) {

                    var row = $('<tr></tr>');

                    row.append('<td>' + cust.cuS_Name + '</td>');
                    row.append('<td class="text-center">' + cust.cuS_CUR_Name + '</td>');

                    table.find('tbody').append(row);

                    row.on('click', function () {

                        // DISPLAY VALUE
                        $(inputElement).val(cust.cuS_Name);

                        // HIDDEN FIELDS (CSHTML bindings)
                        $("#Header_JIDNH_JW_Customer_Number").val(cust.cuS_Number);

                        $("#Currency_Name").val(cust.cuS_CUR_Name);
                        $("#Header_JIDNH_Currency_Number").val(cust.cuS_CUR_Number);

                        $("#Header_JIDNH_WH_Number").val(cust.cuS_WH_Number);
                        $("#SIH_BUY_LOC_Number").val(cust.cuS_LOC_Number);
                        $("#SIH_CUR_DecimalPlaces").val(cust.cuS_CUR_DecimalPlaces);
                        $("#SIH_WHT_Number").val(cust.cuS_WHT_Number);

                        $("#WH_Number").val(cust.cuS_WH_Number);

                        resultsDiv.hide();
                    });
                });

                var closeButton = $(`
    <div class="card-header bg-primary py-1 px-1 d-flex justify-content-end">
        <button type="button"
                class="p-0 btn btn-sm btn-primary bg-opacity-10 d-flex align-items-center justify-content-center">
            ✖
        </button>
    </div>
`);

                closeButton.on('click', function () {
                    resultsDiv.hide();
                });

                resultsDiv.append(closeButton);
                resultsDiv.append(table);

            } else {
                resultsDiv.hide();
                resultsDiv.empty();
            }
        },
        error: function () {
            resultsDiv.text('Error loading data.').show();
        }
    });
}

//#endregion customer Search Functions

function OnInputItem(inputElement) {
    SearchServiceOrderItem(inputElement);
}

function OnFocusItem(inputElement) {
    var value = inputElement.value;

    if (!value) {
        SearchServiceOrderItem(inputElement);
    } else {
        $(inputElement).select();
    }
}

function SearchServiceOrderItem(inputElement) {

    let itemCode = inputElement.value;
    let row = $(inputElement).closest("tr");
    let resultsDiv = row.find(".search-results");
    let material = $("#Header_JISVOH_MS_Number").val();

    if (!material) return;
    $.ajax({
        url: '/jobinward/transactions/service-order/item',
        type: 'GET',
        data: {
            ItemCode: itemCode, MS: material
        },
        success: function (data) {

            resultsDiv.empty();

            if (data && data.length > 0) {

                resultsDiv.show();

                let table = $(`
                    <div class="card-body modal-content p-0 table-responsive">
                        <table class="table table-bordered table-hover table-fixed mb-0 table-grid">
                            <thead>
                                <tr class="table-info">
                                    <th>Item Code</th>
                                    <th>Description</th>
                                    <th>Item Group</th>
                                </tr>
                            </thead>
                            <tbody></tbody>
                        </table>
                    </div>
                `);

                data.forEach(function (item) {

                    let tr = $(`
                        <tr>
                            <td>${item.itemCode}</td>
                            <td>${item.itemDescription}</td>
                            <td>${item.itemGroup}</td>
                        </tr>
                    `);

                    tr.on("click", function () {

                        row.find(".JISVOI_Item_Code").val(item.itemCode);
                        row.find(".JISVOI_Item_Number").val(item.itemNumber);
                        row.find(".JISVOI_Number").val(item.itemNumber);

                        row.find(".Description").val(item.itemDescription);
                        row.find(".OuterDia").val(item.outerDia);
                        row.find(".Thickness").val(item.thickness);
                        row.find(".Length").val(item.length);
                        row.find(".Width").val(item.width);
                        row.find(".MaterialGrade").val(item.materialGrade);
                        row.find(".ItemGroup").val(item.itemGroup);

                        row.find(".JISVOI_UoM_Number").val(item.uoM);

                        row.find(".JISVOI_Qty").focus();

                        resultsDiv.hide();
                    });

                    table.find("tbody").append(tr);
                });

                let closeButton = $(`
                    <div class="card-header bg-primary py-1 px-1">
                        <button type="button"
                                class="p-0 float-end btn btn-sm btn-primary bg-opacity-10">
                            ✖
                        </button>
                    </div>
                `);

                closeButton.on("click", function () {
                    resultsDiv.hide();
                });

                resultsDiv.append(closeButton);
                resultsDiv.append(table);

            } else {
                resultsDiv.hide();
                resultsDiv.html('<p class="p-2">No results found</p>');
            }
        },
        error: function () {
            resultsDiv.text("Error loading data.");
            resultsDiv.show();
        }
    });
}

//#region Calculate Total
function calculateTotal() {

    let totalQty = 0;
    let totalAmount = 0;

    // Loop through each row (only active rows)
    $("#ItemTable tbody tr.NewRow").each(function () {

        let row = $(this);

        // Skip deleted rows
        if (row.find(".JISVOI_IsDeleted").val() === "1" ||
            row.find(".JISVOI_IsDeleted").val() === "true") {
            return;
        }

        // Get Qty
        let qty = parseFloat(row.find(".JISVOI_Qty").val()) || 0;

        // Get Unit Price
        let unitPrice = parseFloat(row.find(".JISVOI_UnitPrice").val()) || 0;

        // Row Amount = Qty × Unit Price
        let amount = qty * unitPrice;

        // Set row amount field
        row.find(".JISVOI_Amount").val(amount.toFixed(2));

        // Add to totals
        totalQty += qty;
        totalAmount += amount;
    });

    // Footer totals
    $("#TotalQty").val(totalQty.toFixed(2));
    $("#TotalAmount").val(totalAmount.toFixed(2));
}
//#endregion Calculate Total

//#region VALIDATE ITEM GRID

function validateItemGrid() {

    let hasValidRow = false;
    let isValid = true;

    $("#ItemTable tbody tr").each(function () {

        let row = $(this);

        // skip template row
        if (row.hasClass("TempRow")) return;

        // skip deleted row
        if (row.find(".JISVOI_IsDeleted").val() === "1") return;

        let process = row.find(".JISVOI_PRS_Number").val();
        let itemCode = row.find(".JISVOI_Item_Code").val();
        let qty = row.find(".JISVOI_Qty").val();
        let unitPrice = row.find(".JISVOI_UnitPrice").val();

        // check if row has any data
        let isRowStarted =
            (process && process.trim() !== "") ||
            (itemCode && itemCode.trim() !== "") ||
            (qty && qty.trim() !== "") ||
            (unitPrice && unitPrice.trim() !== "");

        // empty row → skip
        if (!isRowStarted) return;

        hasValidRow = true;

        // Process
        if (!process || process.trim() === "" || process === "0") {
            showAlert(
                'Process is required',
                row.find(".JISVOI_PRS_Number")
            );
            isValid = false;
            return false;
        }

        // Item Code
        if (!itemCode || itemCode.trim() === "") {
            showAlert(
                'Item Code is required',
                row.find(".JISVOI_Item_Code")
            );
            isValid = false;
            return false;
        }

        // Qty
        if (!qty || qty.trim() === "" || qty.trim() === "0") {
            showAlert(
                'Qty is required',
                row.find(".JISVOI_Qty")
            );
            isValid = false;
            return false;
        }

        // Unit Price
        if (
            !unitPrice ||
            unitPrice.trim() === "" ||
            unitPrice.trim() === "0"
        ) {
            showAlert(
                'Unit Price is required',
                row.find(".JISVOI_UnitPrice")
            );
            isValid = false;
            return false;
        }

    });

    // no rows
    if (!hasValidRow) {
        showAlert(
            'Please add at least one item in grid',
            "#ItemTable tbody tr:first .JISVOI_PRS_Number"
        );
        return false;
    }

    return isValid;
}

//#endregion


//#region ALERT MESSAGE
function showAlert(message, focusSelector = null) {

    $('#AlertMessage').html(message);

    const modalElement = document.getElementById('ModelAlert');
    const modal = new bootstrap.Modal(modalElement);

    modal.show();

    if (focusSelector) {

        $(modalElement).off('hidden.bs.modal').on('hidden.bs.modal', function () {

            $(focusSelector).focus();

        });
    }
}
//#endregion ALERT MESSAGE

//#region GET SERVICE ORDER

function GetServiceOrder(serviceOrderNumber) {

    $.ajax({
        url: '/ServiceOrder/GetServiceOrder',
        type: 'GET',
        data: { JISVOH_Number: serviceOrderNumber },

        success: function (data) {
            console.log(data)
            BindHeader(data.Header[0]);   // or data.header if SP uses WITHOUT_ARRAY_WRAPPER
            BindItems(data.Items);

         
        },

        error: function (xhr) {
            console.log("Error:", xhr);
        }
    });
}

//#endregion

//#region BIND HEADER EDIT

function BindHeader(header) {

    if (!header) return;

    $("#Header_JISVOH_RegNo")
        .val(header.jisvoh_RegNo ?? header.JISVOH_RegNo);

    $("#Header_JISVOH_RegDate")
        .val(header.jisvoh_RegDate ?? header.JISVOH_RegDate);

    $("#Header_JISVOH_ServiceOrderNo")
        .val(header.jisvoh_ServiceOrderNo ?? header.JISVOH_ServiceOrderNo);

    $("#Header_JISVOH_ServiceOrderDate")
        .val(header.jisvoh_ServiceOrderDate ?? header.JISVOH_ServiceOrderDate);

    $("#Header_JISVOH_JW_Customer_Number")
        .val(header.jisvoh_JW_Customer_Number ?? header.JISVOH_JW_Customer_Number).trigger("change");
    $("#Header_JISVOH_JW_Customer_Name")
        .val(header.JISVOH_JW_Customer_Name);

    $("#Header_JISVOH_Currency_Number")
        .val(header.jisvoh_Currency_Number ?? header.JISVOH_Currency_Number);

    $("#Header_JISVOH_PaymentTerms")
        .val(header.jisvoh_PaymentTerms ?? header.JISVOH_PaymentTerms);

    $("#Header_JISVOH_DeliveryTerms")
        .val(header.jisvoh_DeliveryTerms ?? header.JISVOH_DeliveryTerms);

    $("#Header_JISVOH_DeliveryMode")
        .val(header.jisvoh_DeliveryMode ?? header.JISVOH_DeliveryMode);

    $("#Header_JISVOH_Tax")
        .val(header.jisvoh_Tax ?? header.JISVOH_Tax);

    $("#Header_JISVOH_TDC")
        .val(header.jisvoh_TDC ?? header.JISVOH_TDC);

    $("#Header_JISVOH_Remarks")
        .val(header.jisvoh_Remarks ?? header.JISVOH_Remarks);
}

function BindItems(items) {

    console.log('------------------------binditems-------------------');
    console.log(JSON.stringify(items));

    $("#TableBody tr.NewRow").remove();

    if (!items || items.length === 0)
        return;

    $.each(items, function (index, item) {

        var row = `
<tr class="NewRow" data-rowid="${index + 1}">

    <td class="p-2 del">
        <input type="checkbox" class="CheckItem form-check-input">
    </td>

    <td>
        <select name="Items[${index}].JISVOI_PRS_Number"
                class="form-select JISVOI_PRS_Number">
            ${$("#TempRow .JISVOI_PRS_Number").html()}
        </select>
    </td>

    <td>
        <input name="Items[${index}].JISVOI_Number"
               value="${item.JISVOI_Number ?? 0}"
               type="hidden"
               class="JISVOI_Number" />

        <input name="Items[${index}].JISVOI_IsDeleted"
               value="false"
               type="hidden"
               class="JISVOI_IsDeleted" />

        <input name="Items[${index}].JISVOI_Item_Number"
               value="${item.JISVOI_Item_Number ?? 0}"
               type="hidden"
               class="JISVOI_Item_Number" />

        <input name="Items[${index}].JISVOI_Item_Code"
               value="${item.JISVOI_Item_Code ?? ''}"
               autocomplete="off"
               class="form-control JISVOI_Item_Code"
               oninput="OnInputItem(this)"
               onfocus="OnFocusItem(this)" />

        <div class="search-results card" style="display:none"></div>
    </td>

    <td>
        <input name="Items[${index}].Description"
               value="${item.Description ?? ''}"
               class="form-control Description"
               readonly />
    </td>

    <td>
        <input name="Items[${index}].OuterDia"
               value="${item.OuterDia ?? ''}"
               class="form-control OuterDia text-end"
               readonly />
    </td>

    <td>
        <input name="Items[${index}].Thickness"
               value="${item.Thickness ?? ''}"
               class="form-control Thickness text-end"
               readonly />
    </td>

    <td>
        <input name="Items[${index}].Length"
               value="${item.Length ?? ''}"
               class="form-control Length text-end"
               readonly />
    </td>

    <td>
        <input name="Items[${index}].Width"
               value="${item.Width ?? ''}"
               class="form-control Width text-end"
               readonly />
    </td>

    <td>
        <input name="Items[${index}].MaterialGrade"
               value="${item.MaterialGrade ?? ''}"
               class="form-control MaterialGrade"
               readonly />
    </td>

    <td>
        <input name="Items[${index}].ItemGroup"
               value="${item.ItemGroup ?? ''}"
               class="form-control ItemGroup"
               readonly />
    </td>

    <td>
        <select name="Items[${index}].JISVOI_UoM_Number"
                class="form-select JISVOI_UoM_Number">
            ${$("#TempRow .JISVOI_UoM_Number").html()}
        </select>
    </td>

    <td>
        <input name="Items[${index}].SVO_Qty"
               value="${item.JISVOI_Qty ?? 0}"
               class="form-control SVO_Qty text-end"
               readonly />
    </td>

    <td>
        <input name="Items[${index}].InvoicedQty"
               value="${item.InvoicedQty ?? 0}"
               class="form-control InvoicedQty text-end"
               readonly />
    </td>
    <td>
    <input name="Items[${index}].InvoiceToBeRaised"
           value="${1}"
           class="form-control InvoiceToBeRaised text-end"
           readonly />
</td>

    <td>
        <input name="Items[${index}].JISVOI_Qty"
               value="${item.JISVOI_Qty ?? 0}"
               class="form-control JISVOI_Qty text-end" />
    </td>

    <td>
        <input name="Items[${index}].JISVOI_UnitPrice"
               value="${item.JISVOI_UnitPrice ?? 0}"
               class="form-control JISVOI_UnitPrice text-end" />
    </td>

    <td>
        <input name="Items[${index}].JISVOI_Amount"
               value="${item.JISVOI_Amount ?? 0}"
               class="form-control JISVOI_Amount text-end"
               readonly />
    </td>

    <td>
        <input name="Items[${index}].JISVOI_DeliveryDate"
               value="${item.JISVOI_DeliveryDate?.split('T')[0] || ''}"
               type="date"
               class="form-control JISVOI_DeliveryDate" />
    </td>

</tr>`;

        $("#TableBody").append(row);

        let $row = $("#TableBody tr.NewRow:last");

        $row.find(".JISVOI_PRS_Number")
            .val(item.JISVOI_PRS_Number).trigger("change");

        $row.find(".JISVOI_UoM_Number")
            .val(item.JISVOI_UoM_Number);
    });

    CalculateTotals();
}

//#endregion
