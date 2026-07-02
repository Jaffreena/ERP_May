var addressIndex = 0;

$(document).ready(function () {

 

    //#region Unit Price Format

    $(document).on("focusout", ".JIDNI_UnitPrice", function () {

        // Get current input value
        var value = $(this).val().trim();

        // If empty or invalid, set default value
        if (value === "" || isNaN(value)) {
            $(this).val("0.00");
            return;
        }

        // Convert to 2 decimal format
        $(this).val(parseFloat(value).toFixed(2));
    });

    //#endregion

    //#region INPUT CLICK SELECT ALL
    $(document).on("click", "#DeliveryNoteBatchList input", function (e) {
        e.stopPropagation();

        let input = this;
        input.focus();

        setTimeout(function () {
            input.select();
        }, 10);
    });
    $(document).on("click", "#ItemTable input", function (e) {
        e.stopPropagation();

        let input = this;
        input.focus();

        setTimeout(function () {
            input.select();
        }, 10);
    });
    //#endregion

    //#region Row Click - Single Selection
    $("#ItemTable").on("click", "tbody tr.NewRow", function (e) {

        // Ignore direct checkbox click
        if ($(e.target).closest(".CheckItem").length) {
            return;
        }

        // Uncheck all row checkboxes
        $("#ItemTable .CheckItem").prop("checked", false);

        // Check only current row
        $(this).find(".CheckItem").prop("checked", true);
    });
    //#endregion


    //#region Checkbox Click - Multiple Selection
    $("#ItemTable").on("click", ".CheckItem", function (e) {

        // Prevent row click event
        e.stopPropagation();
    });
    //#endregion

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
    //#endregion Initialize Flatpickr

    //#region onkeypress qty and unit
    $(document).on("keyup change", ".JIDNI_Qty, .JIDNI_UnitPrice", function () {

        let row = $(this).closest("tr");

        let qty = parseFloat(row.find(".JIDNI_Qty").val()) || 0;
        let price = parseFloat(row.find(".JIDNI_UnitPrice").val()) || 0;

        let amount = qty * price;

        // Only set row amount (read-only field)
        row.find(".JIDNI_Amount").val(amount.toFixed(2));

        // update footer totals separately
        calculateTotal();
        // auto add row
        autoAddRow(row);


    });
    //#endregion onkeypress qty and unit

    $(document).on("click", "#btnClearAll", function () {
        ClearAll();
    });

    //#region auto add row function
    function autoAddRow(currentRow) {

        let qty = parseFloat(currentRow.find(".JIDNI_Qty").val()) || 0;
        let price = parseFloat(currentRow.find(".JIDNI_UnitPrice").val()) || 0;

        let itemCode = currentRow.find(".JIDNI_Item_Code").val();
        let prsNo = currentRow.find(".JIDNI_PRS_Number").val();

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

    //#region add row item grid
    let rowIndex = 1; // start from 1 because 0 already exists

    $("#AddRowButton").on("click", function () {

        // 1. Validate last row before adding new row
        let isValid = true;
     

        $("#ItemTable tbody tr.NewRow:last").find("input, select").each(function () {

            let el = $(this);

            // skip hidden delete flag
            if (el.hasClass("JIDNI_IsDeleted")) return;

            if (el.hasClass("JIDNI_Item_Code")) {
                if (!el.val()) {
                    isValid = false;
                    el.focus();
                    return false;
                }
            }

            if (el.hasClass("JIDNI_Qty")) {
                if (!el.val() || parseFloat(el.val()) <= 0) {
                    isValid = false;
                    el.focus();
                    return false;
                }
            }

            if (el.hasClass("JIDNI_UnitPrice")) {
                if (!el.val() || parseFloat(el.val()) <= 0) {
                    isValid = false;
                    el.focus();
                    return false;
                }
            }

            if (el.hasClass("JIDNI_PRS_Number")) {

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

        // 2. Clone template row
        let $newRow = $("#TempRow").clone();

        $newRow.removeAttr("id");
        $newRow.removeAttr("style");
        $newRow.addClass("NewRow");

        // 3. Clear values + update indexes
        $newRow.find("input, select").each(function () {

            let el = $(this);

            // reset checkbox
            if (el.attr("type") === "checkbox") {
                el.prop("checked", false);
            }

            // reset value except hidden template fields
            if (!el.hasClass("JIDNI_IsDeleted")) {
                el.val("");
            }

            // update name index Items[0] -> Items[1]
            let name = el.attr("name");
            if (name) {
                let updatedName = name.replace(/\[\d+\]/, `[${rowIndex}]`);
                el.attr("name", updatedName);
            }
        });
        let rowID =
            new Date().getTime();

        $newRow.attr(
            "data-rowid",
            rowID
        );
        // 4. Append row
        $("#TableBody").append($newRow);

        rowIndex++;

        // 5. Recalculate totals (optional hook)
        calculateTotal();
        //region item grid row focus out event
        //$("#ItemTable").on(
        //    "focusout",
        //    "tr.NewRow",
        //    function (e) {

        //        let row = $(this);

        //        setTimeout(() => {

        //            // check next focused element
        //            if (!row.find(document.activeElement).length) {

        //                // document.getElementById('SaveBatchButton').click();

        //            }

        //        }, 0);

        //    }
        //);

        //#endregion
      
      
    });
    //#endregion add row item grid

    //#region jwc address
  

    $("#AddressButton").click(function () {

        var count = GetVisibleAddressRowCount();
        console.log('--visibleRowCount--' + count);
        if (count === 0) {
            LoadJWCAddress();
        } else {
            $("#BuyerAddress").modal("show");
        }

    });
    function GetVisibleAddressRowCount() {

        return $("#AddTableBody tr.AddNewRow").filter(function () {
            var style = ($(this).attr("style") || "")
                .replace(/\s/g, "")
                .toLowerCase();

            return !style.includes("display:none");
        }).length;
    }

    //#region CLICK ADDRESS BUTTON, ADD ADDRESS ROW, DELETE ADDRESS ROW


    $("#AddressAddButton").on("click", function () {

        if (!validateTempRow()) return;

        addAddressRow();
    });
    $(document).on("click", ".AddRowRemove", function () {

        let row = $(this).closest("tr");

        row.find(".JIDNA_IsDeleted").val("1");
        row.hide();
    });
    //#endregion CLICK ADDRESS BUTTON



    //#region CHANGE ADDRESS TYPE
    function isDuplicateAddress(type, currentRow) {
        var isDuplicate = false;

        $('tr.AddNewRow').not(currentRow).each(function () {

            var rowType = $(this).find('select.JIDNA_ADTP_Number').val();
            var isDeleted = parseInt($(this).find("input.JIDNA_IsDeleted").val());

            if (isDeleted !== 1) {
                if (rowType === type) {
                    isDuplicate = true;
                    return false; // break loop
                }
            }
        });

        return isDuplicate;
    }
    $(document).on('change', 'tr.AddNewRow select.JIDNA_ADTP_Number', function () {

        var currentRow = $(this).closest('tr.AddNewRow');

        var ADTPNumber = currentRow.find('.JIDNA_ADTP_Number').val();
        var Buyer = $('#Header_JIDNH_JW_Customer_Number').val(); // keep if same field exists

        var ADDAddress = currentRow.find('.JIDNA_Address');
        var ADDCity = currentRow.find('.JIDNA_City');
        var ADDState = currentRow.find('.JIDNA_State');
        var ADDCountry = currentRow.find('.JIDNA_Country');
        var ADDPin = currentRow.find('.JIDNA_PIN');
        var ADDGSTIN = currentRow.find('.JIDNA_GSTIN');

        if (ADTPNumber && isDuplicateAddress(ADTPNumber, currentRow)) {
            alert('This Address Type already exists!');
            $(this).val('');
            $(this).focus();
            return;
        }

        $.ajax({
            type: "GET",
            url: "/jobinward/transactions/delivery-note/buyer/address",
            data: { Buyer: Buyer, ADTPNumber: ADTPNumber },
            dataType: "json",
            success: function (data) {

                var AddressID = data.buyerAddressId;
                var AddressDefault = data.buyerAddress;

                var $AddressDropdown = currentRow.find('.JIDNA_Address_ID');

                // reset dropdown
                $AddressDropdown.empty();
                $AddressDropdown.append($('<option>', {
                    value: '',
                    text: ''
                }));

                // fill address list
                AddressID.forEach(function (item) {
                    $AddressDropdown.append($('<option>', {
                        value: item.buY_ADD_AddressID,
                        text: item.buY_ADD_AddressID
                    }));
                });

                // set default + fill fields
                if (AddressDefault != null) {
                    $AddressDropdown.val(AddressDefault.buY_ADD_AddressID);

                    ADDAddress.val(AddressDefault.buY_ADD_Address);
                    ADDCity.val(AddressDefault.buY_ADD_City);
                    ADDState.val(AddressDefault.buY_ADD_State);
                    ADDCountry.val(AddressDefault.buY_ADD_Country);
                    ADDPin.val(AddressDefault.buY_ADD_Pin);
                    ADDGSTIN.val(AddressDefault.buY_ADD_GSTIN);
                }
            }
        });
    });
    //#endregion CHANGE ADDRESS TYPE

  

    //#region Save Function
    $("#btnSave").on("click", function (e) {

        if (!validateHeaderById()) {
            e.preventDefault();
            return false;
        }
        else if (batchMismatchData.length > 0) {
            var rowIds = GetBatchMismatchRowIds();
            alert("Batch Qty mismatch exists in rows: " + rowIds);
            e.preventDefault();
            return false;

           

            // continue save
        }

        else {

            var model = CreateDeliveryNoteModel();

            console.log(JSON.stringify(model));

            $.ajax({

                url: '/DeliveryNote/SaveDeliveryNote',

                type: 'POST',

                contentType: 'application/json',

                data: JSON.stringify(model),

                success: function (response) {

                    if (response.success) {
                        ClearAll();
                        showAlert('Record Inserted')
                        DateBind();
                        window.location.reload();
                     //  window.location.href = response.redirectUrl;
                        console.log(JSON.stringify(model));
                    }

                },

                error: function (xhr) {

                    console.log(xhr.responseText);

                }

            });

        }

    }); 
    function GetBatchMismatchRowIds() {
        return batchMismatchData
            .map(x => x.rowId)
            .join(",");
    }
    function CreateDeliveryNoteBatchModel() {

        let deliveryNoteBatches = [];

        $.each(DeliveryNoteItemBatchList, function () {

            let itemBatch = this;

            // Skip empty batch list
            if (
                !itemBatch.BatchList ||
                itemBatch.BatchList.length <= 0
            ) {
                return true;
            }

            $.each(itemBatch.BatchList, function () {

                let batch = this;

                // Skip empty qty
                if (
                    !batch.JIDNI_BCH_QtyInvoice ||
                    parseFloat(batch.JIDNI_BCH_QtyInvoice) <= 0
                ) {
                    return true;
                }

                //#region FORMAT DATE

                let formattedBatchDate = null;

                if (batch.JIDNI_BCH_BatchDate) {

                    let date =
                        new Date(batch.JIDNI_BCH_BatchDate);

                    if (!isNaN(date.getTime())) {

                        formattedBatchDate =
                            date.toISOString();
                    }
                }

                //#endregion

                let deliveryNoteBatch = {

                    JIDNI_BCH_Number:
                        parseInt(batch.JIDNI_BCH_Number) || 0,

                    JIDNI_BCH_JIDNH_Number:
                        parseInt(batch.JIDNI_BCH_JIDNH_Number) || 0,

                    JIDNI_BCH_JIDNI_Number:
                        parseInt(batch.JIDNI_BCH_JIDNI_Number) || 0,

                    JIDNI_BCH_WH_Number:
                        parseInt(batch.JIDNI_BCH_WH_Number) || 0,

                    // FIXED DATE FORMAT
                    JIDNI_BCH_BatchDate:
                        formattedBatchDate,

                    JIDNI_BCH_BatchNo:
                        batch.JIDNI_BCH_BatchNo || "",

                    JIDNI_BCH_BatchQty:
                        parseFloat(batch.JIDNI_BCH_QtyInvoice) || 0,

                    JIDNI_BCH_BatchUnitPrice:
                        parseFloat(batch.JIDNI_BCH_BatchUnitPrice) || 0,

                    JIDNI_BCH_BatchValue:
                        parseFloat(batch.JIDNI_BCH_BatchValue) || 0
                };

                deliveryNoteBatches.push(
                    deliveryNoteBatch
                );

            });

        });

        return deliveryNoteBatches;
    }
    function CreateDeliveryNoteModel() {

        // =====================================
        // HEADER
        // =====================================
        var header = {

            JIDNH_Number:
                parseInt($("#Header_JIDNH_Number").val()) || 0,

            JIDNH_DN_No:
                $("#Header_JIDNH_DN_No").val(),

            JIDNH_DN_Date:
                new Date($("#Header_JIDNH_DN_Date").val())
                    .toISOString(),

            JIDNH_MS_Number:
                parseInt($("#Header_JIDNH_MS_Number").val()) || 0,

            JIDNH_JW_Customer_Number:
                parseInt($("#Header_JIDNH_JW_Customer_Number").val()) || 0,

            JIDNI_Item_Code:
                $("#Header_JIDNI_Item_Code").val(),

            JIDNH_JW_Customer_Name:
                $("#Header_JIDNH_JW_Customer_Name").val(),

            JIDNH_Currency_Number:
                parseInt($("#Header_JIDNH_Currency_Number").val()) || 0,

            JIDNH_WH_Number:
                parseInt($("#Header_JIDNH_WH_Number").val()) || 0,

            JIDNH_PaymentTerms:
                $("#Header_JIDNH_PaymentTerms").val(),

            JIDNH_DeliveryTerms:
                $("#Header_JIDNH_DeliveryTerms").val(),

            JIDNH_DeliveryMode:
                $("#Header_JIDNH_DeliveryMode").val(),

            JIDNH_DespatchDocumentNo:
                $("#Header_JIDNH_DespatchDocumentNo").val(),

            JIDNH_DespatchedThrough:
                $("#Header_JIDNH_DespatchedThrough").val(),

            JIDNH_Remarks:
                $("#Header_JIDNH_Remarks").val(),

            DN_Id:
                parseInt($("#Header_DN_Id").val()) || null,

            DN_CUS_Number:
                parseInt($("#Header_DN_CUS_Number").val()) || null,

            DN_ADD_ADTP_Number:
                parseInt($("#Header_DN_ADD_ADTP_Number").val()) || null
        };

        // =====================================
        // ITEMS
        // =====================================
        var items = [];

        $("#ItemTable tbody tr.NewRow").each(function () {

            let row = $(this);

            // Skip deleted rows
            if (row.find(".JIDNI_IsDeleted").val() == "true") {
                return;
            }

            // Skip empty rows
            if (!row.find(".JIDNI_Item_Number").val()) {
                return;
            }

            let item = {
                JISVOH_Number:
                    parseInt(row.find(".JISVOH_Number").val()) || 0,
                JIDNI_JIDNH_Number:
                    parseInt(row.find(".JIDNI_JIDNH_Number").val()) || 0,

                JIDNI_Number:
                    parseInt(row.find(".JIDNI_Number").val()) || 0,

                JIDNI_PRS_Number:
                    parseInt(row.find(".JIDNI_PRS_Number").val()) || 0,

                JIDNI_Item_Number:
                    parseInt(row.find(".JIDNI_Item_Number").val()) || 0,

                JIDNI_WH_Number:
                    parseInt(row.find(".JIDNI_WH_Number").val()) || 0,

                JIDNI_UoM_Number:
                    parseInt(row.find(".JIDNI_UoM_Number").val()) || 0,

                JIDNI_Qty:
                    parseFloat(row.find(".JIDNI_Qty").val()) || 0,

                JIDNI_UnitPrice:
                    parseFloat(row.find(".JIDNI_UnitPrice").val()) || 0,

                JIDNI_Amount:
                    parseFloat(row.find(".JIDNI_Amount").val()) || 0,

                JIDNI_JW_InvoiceTracking:
                    row.find(".JIDNI_JW_InvoiceTracking").is(":checked")
                        ? "Yes"
                        : "No"
            };

            items.push(item);

        });

        // =====================================
        // ADDRESS
        // =====================================
        var addresses = [];

        $("#AddTableBody tr.AddNewRow").each(function () {

            let row = $(this);

            // Skip deleted rows
            if (row.find(".JIDNA_IsDeleted").val() == "1") {
                return;
            }

            // Skip empty rows
            if (!row.find(".JIDNA_Address_ID").val()) {
                return;
            }

            let address = {

                JIDNA_JIDNH_Number:
                    parseInt(row.find(".JIDNA_JIDNH_Number").val()) || 0,

                JIDNA_Number:
                    parseInt(row.find(".JIDNA_Number").val()) || 0,

                JIDNA_ADTP_Number:
                    parseInt(row.find(".JIDNA_ADTP_Number").val()) || 0,

                JIDNA_Address_ID:
                    row.find(".JIDNA_Address_ID").val(),

                JIDNA_Address:
                    row.find(".JIDNA_Address").val(),

                JIDNA_City:
                    row.find(".JIDNA_City").val(),

                JIDNA_State:
                    row.find(".JIDNA_State").val(),

                JIDNA_Country:
                    row.find(".JIDNA_Country").val(),

                JIDNA_PIN:
                    row.find(".JIDNA_PIN").val(),

                JIDNA_GSTIN:
                    row.find(".JIDNA_GSTIN").val()
            };

            addresses.push(address);

        });


        // =====================================
        // FINAL MODEL
        // =====================================
        var deliveryNoteModel = {

            Header: header,
            Items: items,
            deliveryNoteBatches: CreateDeliveryNoteBatchModel(),
            Addresses: addresses
          
        };
        console.log(deliveryNoteModel)

        return deliveryNoteModel;
       
    }

    //#endregion Save Function

    //#region remove checked rows
    $("#RemoveItemRowButton").on("click", function () {

        let checkedRows =
            $("#ItemTable tbody tr.NewRow:visible")
                .has(".CheckItem:checked");

        // minimum one row should exist
        let totalVisibleRows =
            $("#ItemTable tbody tr.NewRow:visible").length;

        if (checkedRows.length === 0) {

            alert("Please select row.");
            return;
        }

        if ((totalVisibleRows - checkedRows.length) < 0) {

            alert("At least one row required.");
            return;
        }
        if (checkedRows.length > 1) {
            alert("Please select only one row");
            return false;
        }
        checkedRows.each(function () {

            let currentRow = $(this);

            // visible row index
            let ItemGridindex =
                currentRow.index(
                    "#ItemTable tbody tr.NewRow:visible"
                )+1;



           

            $.ajax({

                url: '/DeliveryNote/DeleteTempDeliveryBatchRow',

                type: 'POST',

                data: { index: ItemGridindex },

                success: function (response) {
                    // remove selected row
                    currentRow.remove();
                    calculateTotal();
                },

                error: function (xhr) {

                    console.log(xhr.responseText);
                }
            });
       
        });
      
      

    });
    //#endregion remove checked rows


    $(document).on(
        "change",
        ".JIDNI_JW_InvoiceTracking, .JIDNI_PRS_Number, .JIDNI_Item_Code, .JIDNI_UoM_Number",
        function () {

            let row = $(this).closest("tr");

            if (row.find(".JIDNI_JW_InvoiceTracking").is(":checked")) {

                BindServiceOrder(
                    $("#Header_JIDNH_JW_Customer_Number").val(),
                    row.find(".JIDNI_PRS_Number").val(),
                    row.find(".JIDNI_Item_Number").val(),
                    row.find(".JIDNI_UoM_Number").val()
                );

            } else {
                row.find(".JISVOH_Number").html('<option value="0"></option>');
            }
        }
    );

});



//#region ADD  ADDRESS ROW GRID ,VALIDATE ADDRESS GRID,VALIDATE TEMP ROW


function DateBind() {
    var today = new Date();

    var day = String(today.getDate()).padStart(2, '0');
    var months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun",
        "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

    var formattedDate = day + "-" + months[today.getMonth()] + "-" + today.getFullYear();

    var fp = document.getElementById("Header_JIDNH_DN_Date")._flatpickr;
    if (fp) fp.setDate(formattedDate, true, "d-M-Y");
}

//#region delete grid
function DeleteItemRowTempTable(inputElement) {
    let ItemGridindex =
        $(inputElement)
            .closest("tr")
            .index("#ItemTable tbody tr.NewRow:visible") + 1;
    $.ajax({

        url: '/DeliveryNote/TempDeliveryBatchDeleteChangeItemDBRow',

        type: 'POST',

        data: { index: ItemGridindex },

        success: function (response) {
          
            calculateTotal();
        },

        error: function (xhr) {

            console.log(xhr.responseText);
        }
    });
}



//#endregion
 
function addAddressRow() {

    let i = addressIndex;

    let $row = $("#AddTempRow").clone();

    $row.removeAttr("id");
    $row.addClass("AddNewRow");
    $row.show();

    // 1. Address Type
    $row.find(".JIDNA_ADTP_Number")
        .val("")
        .attr("name", `Addresses[${i}].JIDNA_ADTP_Number`);

    // 2. Address ID
    $row.find(".JIDNA_Address_ID")
        .val("")
        .attr("name", `Addresses[${i}].JIDNA_Address_ID`);

    // 3. Address
    $row.find(".JIDNA_Address")
        .val("")
        .attr("name", `Addresses[${i}].JIDNA_Address`);

    // 4. City
    $row.find(".JIDNA_City")
        .val("")
        .attr("name", `Addresses[${i}].JIDNA_City`);

    // 5. State
    $row.find(".JIDNA_State")
        .val("")
        .attr("name", `Addresses[${i}].JIDNA_State`);

    // 6. Country
    $row.find(".JIDNA_Country")
        .val("")
        .attr("name", `Addresses[${i}].JIDNA_Country`);

    // 7. PIN
    $row.find(".JIDNA_PIN")
        .val("")
        .attr("name", `Addresses[${i}].JIDNA_PIN`);

    // 8. GSTIN
    $row.find(".JIDNA_GSTIN")
        .val("")
        .attr("name", `Addresses[${i}].JIDNA_GSTIN`);

    // 9. Delete flag
    $row.find(".JIDNA_IsDeleted")
        .val("0")
        .attr("name", `Addresses[${i}].JIDNA_IsDeleted`);

    $("#AddTableBody").append($row);

    addressIndex++;
}

function validateAddressGrid() {

    let hasRow = false;
    let valid = true;

    $("#AddTableBody tr.AddNewRow").each(function () {

        let row = $(this);

        if (row.find(".JIDNA_IsDeleted").val() === "1") return;

        let type = row.find(".JIDNA_ADTP_Number").val();
        let addr = row.find(".JIDNA_Address_ID").val();

        if (type && addr) {
            hasRow = true;
        }

        if (type && !addr) {
            showAlert('Address ID required');
            row.find(".JIDNA_Address_ID").focus();
            valid = false;
            return false;
        }

        if (!type && addr) {
            showAlert('Address Type required');
            row.find(".JIDNA_ADTP_Number").focus();
            valid = false;
            return false;
        }
    });

    if (!hasRow) {
        showAlert('Please add at least one address');
        return false;
    }

    return valid;
}


function validateTempRow() {

    let isValid = true;

    $("#AddTableBody tr.AddNewRow:visible").each(function () {

        let row = $(this);
        console.log('JIDNA_ADTP_Number:' + row.find(".JIDNA_ADTP_Number").val())
        if (!row.find(".JIDNA_ADTP_Number").val()) {
            showAlert('Address Type is required');
            isValid = false;
            return false; // break each
        }

        if (!row.find(".JIDNA_Address_ID").val()) {
            showAlert('Address ID is required');
            isValid = false;
            return false;
        }
    });

    return isValid;
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
                     //   BindServiceOrder(cust.cuS_Number);
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


//#region Delivered Qty Validation

$(document).on("focusout", ".JIDNI_Qty", function () {

    let row = $(this).closest("tr");
    let jisvohNumber = row.find(".JISVOH_Number").val();  
    if (!jisvohNumber) return;
    let prsNumber = row.find(".JIDNI_PRS_Number").val();
    let itemNumber = row.find(".JIDNI_Item_Number").val();
    let uomNumber = row.find(".JIDNI_UoM_Number").val();
    let originalQty = parseFloat(row.find(".JIDNI_Qty").val()) || 0;
    let rowIndex = row.index();
    $.get("/DeliveryNote/CheckDeliveredQtyExceeded", {
        jisvohNumber,
        prsNumber,
        itemNumber,
        uomNumber
    }, function (res) {
        if (!res || res.length === 0) return;
        let deliveredQty = parseFloat(res[0].deliveredQty) || 0;
        let jisvoiQty = parseFloat(res[0].jisvoiQty) || 0;
        if ((deliveredQty + originalQty) > jisvoiQty) {         

            alert("Qty Allowed: " + (jisvoiQty - deliveredQty));
            setTimeout(function () {
                row.find(".JIDNI_Qty")
                    .focus()
                    .select();
                row.find(".JISVOH_Number").val("0");
            }, 300);            
        }
    });
});

$(document).on("change", ".JISVOH_Number", function () {

    let row = $(this).closest("tr");
    let jisvohNumber = $(this).val();
    row.find(".JISVOH_Number").val(jisvohNumber)  

    let prsNumber = row.find(".JIDNI_PRS_Number").val();
    let itemNumber = row.find(".JIDNI_Item_Number").val();
    let uomNumber = row.find(".JIDNI_UoM_Number").val();
    let originalQty = parseFloat(row.find(".JIDNI_Qty").val()) || 0;
    let rowIndex = row.index();

    $.get("/DeliveryNote/CheckDeliveredQtyExceeded", {
        jisvohNumber,
        prsNumber,
        itemNumber,
        uomNumber
    }, function (res) {

        if (!res || res.length === 0) return;

        let deliveredQty = parseFloat(res[0].deliveredQty) || 0;
        let jisvoiQty = parseFloat(res[0].jisvoiQty) || 0;

        if ((deliveredQty + originalQty) > jisvoiQty) {
          

            alert("Qty Allowed: " + (jisvoiQty - deliveredQty));
            setTimeout(function () {
                row.find(".JIDNI_Qty")
                    .focus()
                    .select();
                row.find(".JISVOH_Number").val("0");
            }, 300);
        
          
        }
    });
});

//#endregion
function BindServiceOrder(customerId, prsNumber = null, itemNumber = null, uomNumber = null) {
    $(".JISVOH_Number").html('<option value="0"></option>');
    if (!customerId) return;

    $.get("/DeliveryNote/GetServiceOrder",
        { customerId, prsNumber, itemNumber, uomNumber },
        data => $.each(data, (_, item) =>
            $(".JISVOH_Number").append(`<option value="${item.value}">${item.text}</option>`)
        )
    );
}


//#region Calculate Total
function calculateTotal() {

    let totalQty = 0;
    let totalAmount = 0;

    // Loop through each row (only active rows)
    $("#ItemTable tbody tr.NewRow").each(function () {

        let row = $(this);

        // Skip deleted rows
        if (row.find(".JIDNI_IsDeleted").val() === "1" ||
            row.find(".JIDNI_IsDeleted").val() === "true") {
            return;
        }

        // Get Qty
        let qty = parseFloat(row.find(".JIDNI_Qty").val()) || 0;

        // Get Unit Price
        let unitPrice = parseFloat(row.find(".JIDNI_UnitPrice").val()) || 0;

        // Row Amount = Qty × Unit Price
        let amount = qty * unitPrice;

        // Set row amount field
        row.find(".JIDNI_Amount").val(amount.toFixed(2));

        // Add to totals
        totalQty += qty;
        totalAmount += amount;
    });

    // Footer totals
    $("#TotalQty").val(totalQty.toFixed(2));
    $("#TotalAmount").val(totalAmount.toFixed(2));
}
//#endregion Calculate Total

//#region item grid fetch item details
function OnInputItem(inputElement) {
    searchItemJIDNI(inputElement);
}

function OnFocusItem(inputElement) {

    var value = inputElement.value;

    if (!value) {
        searchItemJIDNI(inputElement);
    } else {
        $(inputElement).select();
    }
}

function searchItemJIDNI(inputElement) {

    let itemCode = inputElement.value;
    let row = $(inputElement).closest("tr");
    let resultsDiv = row.find(".search-results");

    let material = $("#Header_JIDNH_MS_Number").val();


    if (!material) return;

    $.ajax({
        url: '/jobinward/transactions/delivery-note/item',
        type: 'GET',
        data: {
            ItemCode: itemCode,
            MS: material
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

                    // CLICK SELECT
                    tr.on("click", function () {

                        // ✔ Visible field
                        row.find(".JIDNI_Item_Code").val(item.itemCode);

                        // ✔ Hidden fields
                        row.find(".JIDNI_Item_Number").val(item.itemNumber);
                        row.find(".JIDNI_Number").val(item.itemNumber);

                        // ✔ Fill details
                        row.find(".JIDNI_Item_Description").val(item.itemDescription);
                        row.find(".JIDNI_OuterDia").val(item.outerDia);
                        row.find(".JIDNI_Thickness").val(item.thickness);
                        row.find(".JIDNI_Length").val(item.length);
                        row.find(".JIDNI_Width").val(item.width);
                        row.find(".JIDNI_MaterialGrade").val(item.materialGrade);
                        row.find(".JIDNI_ItemGroup").val(item.itemGroup);

                        // ✔ Dropdowns
                        row.find(".JIDNI_UoM_Number").val(item.uoM);
                        row.find(".JIDNI_WH_Number").val(item.saleWarehouse);

                        // ✔ Move to Qty
                        let qtyInput = row.find(".JIDNI_Qty");
                        let qtyUnitprice = row.find(".JIDNI_UnitPrice");
                        qtyInput.focus();

                        setTimeout(function () {
                            qtyInput.select();
                            DeleteItemRowTempTable(inputElement);
                        }, 100);

                        // ✔ Decimal format (if needed)
                        let decimalPlaces = item.decimalPlaces || 2;

                        let qtyVal = qtyInput.val();
                        let qtyUnitpriceVal = qtyUnitprice.val();
                        qtyInput.val(QtyDecimalRupees(qtyVal, decimalPlaces));
                        qtyUnitprice.val(QtyDecimalRupees(qtyUnitpriceVal, decimalPlaces));

                        resultsDiv.hide();
                    });

                    table.find("tbody").append(tr);
                });

                // CLOSE BUTTON
                let closeButton = $(`
                    <div class="card-header bg-primary py-1 px-1">
                        <button type="button" class="p-0 float-end btn btn-sm btn-primary bg-opacity-10">
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

//#endregion item grid fetch item details



//#region COMMON FUNCTIONS
function removeCommas(value) {
    return (value || '').toString().replace(/,/g, '');
}

function DecimalIndianRupees(value) {
    if (value === "" || isNaN(value)) {
        return "0.00";
    }

    var formattedValue = parseFloat(value).toFixed(2);

    var parts = formattedValue.split(".");
    parts[0] = parts[0].replace(/(\d)(?=(\d\d)+\d$)/g, "$1,");
    return parts.join(".");
}
function QtyDecimalRupees(value, decimalPlaces) {
    if (value === "" || isNaN(value)) return "0";

    var formattedValue = parseFloat(value).toFixed(decimalPlaces);
    var parts = formattedValue.split(".");
    if (parts.length > 1) {
        parts[1] = parts[1].replace(/0+$/, "");
        if (parts[1].length === 0) parts.pop();
    }

    parts[0] = parts[0].replace(/(\d)(?=(\d\d)+\d$)/g, "$1,");

    return parts.join(".");
}
function UnitDecimalRupees(value, UnitDecimalPlaces) {
    if (value === "" || isNaN(value)) return "0";

    var num = parseFloat(value);

    var formattedValue = num.toFixed(UnitDecimalPlaces);
    var parts = formattedValue.split(".");

    if (parts.length > 1) {
        parts[1] = parts[1].replace(/0+$/, "");

        if (parts[1].length < 2) {
            parts[1] = parts[1].padEnd(2, "0");
        }
    } else {
        parts.push("00");
    }

    parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");

    return parts.join(".");
}
//#endregion COMMON FUNCTIONS

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

//#region SUBMIT VALIDATION
function validateHeaderById() {

    // 1. DN No
    if ($("#Header_JIDNH_DN_No").val().trim() === "") {
        showAlert('Delivery Note No. is required','#Header_JIDNH_DN_No');
        
        return false;
    }

    // 2. DN Date
    if ($("#Header_JIDNH_DN_Date").val().trim() === "") {
        showAlert('DN Date is required','#Header_JIDNH_DN_Date');
        
        return false;
    }

    // 3. Material Segregation
    if ($("#Header_JIDNH_MS_Number").val() === "" || $("#Header_JIDNH_MS_Number").val() === "0") {
        showAlert('Material Segregation is required','#Header_JIDNH_MS_Number');
        
        return false;
    }

    // 4. JW Customer
    if ($("#Header_JIDNH_JW_Customer_Number").val().trim() === "" ||
        $("#Header_JIDNH_JW_Customer_Name").val().trim() === "") {

        showAlert('JW Customer is required','#Header_JIDNH_JW_Customer_Name');
       
        return false;
    }

    // 5. Currency
    if ($("#Header_JIDNH_Currency_Number").val() === "" || $("#Header_JIDNH_Currency_Number").val() === "0") {
        showAlert('Currency is required','#Header_JIDNH_Currency_Number');
       
        return false;
    }

    // 6. Warehouse
    if ($("#Header_JIDNH_WH_Number").val() === "" || $("#Header_JIDNH_WH_Number").val() === "0") {
        showAlert('Warehouse is required','#Header_JIDNH_WH_Number');
     
        return false;
    }

    //// 7. Payment Terms
    //if ($("#Header_JIDNH_PaymentTerms").val().trim() === "") {
    //    showAlert('Payment Terms is required','#Header_JIDNH_PaymentTerms');
        
    //    return false;
    //}

    //// 8. Delivery Terms
    //if ($("#Header_JIDNH_DeliveryTerms").val().trim() === "") {
    //    showAlert('Delivery Terms is required','#Header_JIDNH_DeliveryTerms');
      
    //    return false;
    //}

    //// 9. Delivery Mode
    //if ($("#Header_JIDNH_DeliveryMode").val().trim() === "") {
    //    showAlert('Delivery Mode is required','#Header_JIDNH_DeliveryMode');
        
    //    return false;
    //}

    //// 10. Despatch Document No
    //if ($("#Header_JIDNH_DespatchDocumentNo").val().trim() === "") {
    //    showAlert('Despatch Document No is required','#Header_JIDNH_DespatchDocumentNo');
       
    //    return false;
    //}

    //// 11. Despatched Through
    //if ($("#Header_JIDNH_DespatchedThrough").val().trim() === "") {
    //    showAlert('Despatched Through is required','#Header_JIDNH_DespatchedThrough');
      
    //    return false;
    //}

    //// 12. Remarks
    //if ($("#Header_JIDNH_Remarks").val().trim() === "") {
    //    showAlert('Remarks is required','#Header_JIDNH_Remarks');
     
    //    return false;
    //}
    // =========================
    // GRID VALIDATION CALL
    // =========================
    if (!validateItemGrid()) {
        return false;
    }
    if (!validateAddressGrid()) {
        return false;
    }
    if (!validateDeliveryNoteBatchList()) {
        return false;
    }

    return true;
}
//#endregion

//#region VALIDATE ITEM GRID,batchgrid
 
function validateItemGrid() {

    let hasValidRow = false;

    let isValid = true;

    $("#ItemTable tbody tr").each(function () {

        let row = $(this);

        // skip template row
        if (row.hasClass("TempRow")) return;

        // skip deleted row
        if (row.find(".JIDNI_IsDeleted").val() === "1") return;

        let process = row.find(".JIDNI_PRS_Number").val();
        let itemCode = row.find(".JIDNI_Item_Code").val();
        let qty = row.find(".JIDNI_Qty").val();
        let unitPrice = row.find(".JIDNI_UnitPrice").val();

        // check if row has ANY data
        let isRowStarted =
            (process && process.trim() !== "") ||
            (itemCode && itemCode.trim() !== "") ||
            (qty && qty.trim() !== "") ||
            (unitPrice && unitPrice.trim() !== "");

        // if row is empty → skip
        if (!isRowStarted) return;

        // row is considered active
        hasValidRow = true;

        // validate Process
        if (!process || process.trim() === "" || process === "0") {
            showAlert(
                'Process is required',
                row.find(".JIDNI_PRS_Number")
            );
            isValid = false;
            return false; // break loop
        }

        // validate Item Code
        if (!itemCode || itemCode.trim() === "") {
            showAlert('Item Code is required', row.find(".JIDNI_Item_Code"));
         
            isValid = false;
            return false;
        }

        // validate Qty
        if (!qty || qty.trim() === "" || qty.trim() === "0") {
            showAlert('Qty is required', row.find(".JIDNI_Qty"));
         
            isValid = false;
            return false;
        }

        // validate Unit Price
        if (!unitPrice || unitPrice.trim() === "" || unitPrice.trim() === "0") {
            showAlert('Unit Price is required', row.find(".JIDNI_UnitPrice"));
          
            isValid = false;
            return false;
        }

    });

    // no valid row added
    if (!hasValidRow) {
        showAlert('Please add at least one item in grid', "#ItemTable tbody tr:first .JIDNI_PRS_Number");
        //row.find(".JIDNI_PRS_Number").focus();
        return false;
    }

    return isValid;
}


//#endregion VALIDATE ITEM GRID

//#region VALIDATE DELIVERY NOTE BATCH LIST

function validateDeliveryNoteBatchList() {

    let batchRows =
        $("#DeliveryNoteBatchList tbody tr")
            .not("#DeliveryNoteBatchTemplateRow");

    let hasValidQty = false;

    batchRows.each(function () {

        let row = $(this);

        let qty =
            row.find(".JIDNI_BCH_QtyInvoice").val();

        qty = parseFloat(qty) || 0;

        if (qty > 0) {

            hasValidQty = true;

            return false;
        }

    });

    if (!hasValidQty) {

        showAlert(
            "Please enter Delivered Qty in batch details",
            '#DeliveryNoteBatchList tbody tr:visible:first .JIDNI_BCH_QtyInvoice'
        );

        return false;
    }

    return true;
}
//#endregion

//#region TEMP DELIVERY BATCH MODEL
function CreateTempDeliveryBatchModel(row) {

    return {

        DBCH_Number:
            parseInt(row.find(".JIDNI_BCH_Number").val()) || 0,

        DBCH_Index:
            parseInt(row.index()) || 0,

        DBCH_DBCH_Number:
            parseInt(row.find(".JIDNI_BCH_Number").val()) || null,

        DBCH_Item_Number:
            parseInt($("#Header_JIDNI_Item_Number").val()) || 0,

        DBCH_Warehouse_Number:
            parseInt(row.find(".JIDNI_BCH_WH_Number").val()) || 0,

        DBCH_Date:
            row.find(".JIDNI_BCH_BatchDate").val(),

        DBCH_No:
            row.find(".JIDNI_BCH_BatchNo").val(),

        DBCH_Qty:
            parseFloat(
                row.find(".JIDNI_BCH_QtyInvoice").val()
            ) || 0,

        DBCH_UnitPrice:
            parseFloat(
                row.find(".JIDNI_BCH_BatchUnitPrice").val()
            ) || 0,

        DBCH_Value:
            parseFloat(
                row.find(".JIDNI_BCH_BatchValue").val()
            ) || 0,

        Mode: 1,

        CreatorCode: 1,

        CreatorDate:
            new Date().toISOString()
    };
}

//#endregion TEMP DELIVERY BATCH MODEL


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

//#region datebind
function DateBind() {
    var today = new Date();

    var day = String(today.getDate()).padStart(2, '0');
    var months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun",
        "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

    var formattedDate = day + "-" + months[today.getMonth()] + "-" + today.getFullYear();

    var fp = document.getElementById("Header_JIDNH_DN_Date")._flatpickr;
    if (fp) fp.setDate(formattedDate, true, "d-M-Y");
}
//#endregion

function LoadJWCAddress() {
    var jwcNumber = $("#Header_JIDNH_JW_Customer_Number").val();

    $.ajax({
        url: '/JobworkInvoice/GetJWCAddress',
        type: 'GET',
        data: { JWCNumber: jwcNumber },
        success: function (response) {
            console.log(JSON.stringify(response));
            if (!response || !response.length) return;

            var rowCount = 0;

            response.forEach(function (addr) {
                if (addr.jwC_ADD_Default != 1) return;

                addAddressRow(); // always create new row
                var row = $("#AddTableBody tr.AddNewRow:last");

                row.find(".JIDNA_ADTP_Number").val(addr.jwC_ADD_ADTP_Number).trigger("change");
                row.find(".JIDNA_Address_ID").val(addr.jwC_ADD_Address_ID);
                row.find(".JIDNA_Address").val(addr.jwC_ADD_Address);
                row.find(".JIDNA_City").val(addr.jwC_ADD_City);
                row.find(".JIDNA_State").val(addr.jwC_ADD_State);
                row.find(".JIDNA_Country").val(addr.jwC_ADD_Country);
                row.find(".JIDNA_PIN").val(addr.jwC_ADD_PIN);
                row.find(".JIDNA_GSTIN").val(addr.jwC_ADD_GSTIN);

                row.show();
            });

            $("#BuyerAddress").modal("show");
        }
    });
}
