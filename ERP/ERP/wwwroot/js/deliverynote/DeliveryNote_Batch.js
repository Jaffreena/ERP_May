//#region GLOBAL ARRAY


let DeliveryNoteBatchList = [];
let DeliveryNoteItemBatchList = [];
let CurrentBatchItemRow = null;

//#endregion

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


//#region ASSIGN UNIQUE ROW ID
function AssignItemRowID() {

    $("#ItemTable tbody tr.NewRow").each(function () {

        let rowID =
            $(this).attr("data-rowid");

        // ADD ONLY IF NOT EXISTS
        if (!rowID || rowID === '1') {       

            rowID =
                new Date().getTime() +
                Math.floor(Math.random() * 1000);

            $(this).attr(
                "data-rowid",
                rowID
            );
        }

    });

}

//function AssignItemRowID() {

//    $("#TableBody tr.NewRow").each(function (index) {

//        let row = $(this);

//        if (row.attr("data-rowid"))
//            return;

//        row.attr(
//            "data-rowid",
//            "ROW_" + new Date().getTime() + "_" + index
//        );
//    });
//}

// INITIAL
AssignItemRowID();

//#endregion



//#region VALIDATE ITEM GRID CHECKBOX

function ValidateSelectedItemRow() {

    let checkedRows =
        $("#TableBody .ItemCheckbox:checked");

    // NO ROW SELECTED
    if (checkedRows.length == 0) {

        showAlert('Please check at least one item row')

       

        return false;
    }

    // MORE THAN ONE SELECTED
    if (checkedRows.length > 1) {
        showAlert('Please check only one item row')

         

        return false;
    }

    return true;
}

//#endregion



//#region SINGLE CHECKBOX ONLY

$(document).on(
    "change",
    ".ItemCheckbox",
    function () {

        $(".ItemCheckbox")
            .not(this)
            .prop("checked", false);

        let row =
            $(this).closest("tr");

        let rowID =
            row.attr("data-rowid");

        console.log("SELECTED ROW ID");
        console.log(rowID);

    });

//#endregion







//#region VALIDATE EXISTING BATCH ROWS

function ValidateExistingBatchRows() {

    let isValid = true;

    $("#DeliveryNoteBatchTableBody tr.DeliveryNoteBatchRow")
        .each(function () {

            let row = $(this);

            row.removeClass("table-danger");

            let batchNo =
                row.find(".JIDNI_BCH_BatchNo")
                    .val()
                    ?.trim();

            let qty =
                row.find(".JIDNI_BCH_QtyInvoice")
                    .val()
                    ?.trim();

            let price =
                row.find(".JIDNI_BCH_BatchUnitPrice")
                    .val()
                    ?.trim();

            if (
                batchNo == "" ||
                qty == "" ||
                qty == "0" ||
                price == "" ||
                price == "0"
            ) {

                row.addClass("table-danger");

                isValid = false;

                return false;
            }

        });

    return isValid;
}

//#endregion



//#region ADD BATCH ROW BUTTON

$(document).on(
    "click",
    "#AddBatchRow",
    function () {

        // VALIDATE EXISTING ROWS
        let valid =
            ValidateExistingBatchRows();

        if (!valid) {
            showAlert('Please fill all existing batch rows before adding new row')

          

            return;
        }

       // AddDeliveryNoteBatchRow();

    });

//#endregion



//#region REMOVE BATCH ROW

$(document).on(
    "click",
    ".RemoveBatchRow",
    function () {

        $(this)
            .closest("tr")
            .remove();

        CalculateBatchFooter();

    });

//#endregion



//#region BATCH VALUE CALCULATION

$(document).on(
    "input",
    ".JIDNI_BCH_QtyInvoice, .JIDNI_BCH_BatchUnitPrice",
    function () {

        let row =
            $(this).closest("tr");

        let qty =
            parseFloat(
                row.find(".JIDNI_BCH_QtyInvoice").val()
            ) || 0;

        let price =
            parseFloat(
                row.find(".JIDNI_BCH_BatchUnitPrice").val()
            ) || 0;

        let value = qty * price;

        row.find(".JIDNI_BCH_BatchValue")
            .val(value.toFixed(2));

        CalculateBatchFooter();

    });

//#endregion



//#region FOOTER TOTAL

function CalculateBatchFooter() {

    let totalQty = 0;
    let totalReservedQty = 0;
    let totalValue = 0;
    let totalAvailableQty = 0;

    $("#DeliveryNoteBatchTableBody tr.DeliveryNoteBatchRow")
        .each(function () {

            totalQty +=
                parseFloat(
                    $(this)
                        .find(".JIDNI_BCH_QtyInvoice")
                        .val()
                ) || 0;
            totalAvailableQty +=
                parseFloat(
                    $(this)
                        .find(".JIDNI_BCH_QtyAvailable")
                        .val()
                ) || 0;

            totalValue +=
                parseFloat(
                    $(this)
                        .find(".JIDNI_BCH_BatchValue")
                        .val()
                ) || 0;
            totalReservedQty += parseFloat($(this).find(".JIDNI_BCH_QtyReserved").val()) || 0;

        });

    $("#TotalBatchQty")
        .val(totalQty.toFixed(2));

    $("#TotalBatchValue")
        .val(totalValue.toFixed(2));
    $("#TotalAvailableQty")
        .val(totalAvailableQty.toFixed(2));
    $("#TotalReservedQty").val(totalReservedQty.toFixed(2));
}

//#endregion



 


//#region SAVE BATCH

$(document).on(
    "click",
    "#SaveBatchButton",
    function (e) {

        if (CurrentBatchItemRow == null)
            return;

        // VALIDATE BATCH ROWS
        let valid =
            ValidateExistingBatchRows();
        if (!ValidateBatchQty()) {
            e.preventDefault();
            return false;
        }

        CloseDeliveryNoteBatchModal();

        //if (!valid) {
        //
        //    showAlert(
        //        "Please fill all batch rows before saving"
        //    );
        //
        //    return;
        //}

        let rowID =
            CurrentBatchItemRow
                .attr("data-rowid");

        let batchList = [];

        $("#DeliveryNoteBatchTableBody tr.DeliveryNoteBatchRow")
            .each(function () {

                let row = $(this);

                let batchObj = {

                    RowID: rowID,

                    JIDNI_BCH_WH_Number:
                        row.find(".JIDNI_BCH_WH_Number").val(),
                    JIDNI_BCH_JIDNI_Number:
                        row.find(".JIDNI_BCH_JIDNI_Number").val(),
                    JIDNI_BCH_WH_Name:
                        row.find(".JIDNI_BCH_WH_Name").val(),

                    JIDNI_BCH_BatchDate:
                        row.find(".JIDNI_BCH_BatchDate").val(),

                    JIDNI_BCH_Number:
                        row.find(".JIDNI_BCH_Number").val(),

                    JIDNI_BCH_BatchNo:
                        row.find(".JIDNI_BCH_BatchNo").val(),

                    JIDNI_BCH_QtyAvailable:
                        row.find(".JIDNI_BCH_QtyAvailable").val(),

                    JIDNI_BCH_QtyReserved:
                        row.find(".JIDNI_BCH_QtyReserved").val(),

                    JIDNI_BCH_QtyInvoice:
                        row.find(".JIDNI_BCH_QtyInvoice").val(),

                    JIDNI_BCH_BatchUnitPrice:
                        row.find(".JIDNI_BCH_BatchUnitPrice").val(),

                    JIDNI_BCH_BatchValue:
                        row.find(".JIDNI_BCH_BatchValue").val()
                };

                batchList.push(batchObj);
            //    SaveTempBatch(row);

            });

        // REMOVE OLD SAME ROW DATA
        DeliveryNoteItemBatchList =
            DeliveryNoteItemBatchList.filter(
                x => x.RowID != rowID
            );

        // SAVE NEW DATA
        DeliveryNoteItemBatchList.push({

            RowID: rowID,

            BatchList: batchList
        });

        //#region CONSOLE

       // console.clear();

        console.log("CURRENT ROW ID");
        console.log(rowID);

        SaveTempBatch(batchList);

        //#endregion

        $("#DeliveryNoteBatchModal")
            .modal("hide");

    });

//#endregion

//#region CLOSE POPUP

$(document).on(
    "click",
    "#DeliveryNoteBatchClose",
    function () {

        $("#DeliveryNoteBatchModal")
            .modal("hide");

    });

//#endregion

//#region SHOW BATCH

$(document).on("click", ".OpenBatchPopup", function (e) {



    
        e.preventDefault();
        //console.log("ROW ID :", rowID);
        AssignItemRowID();

        let checkedCheckbox = $(".CheckItem:checked");

        //#region VALIDATION

        if (checkedCheckbox.length <= 0) {
            alert("Please select at least one row");
            return false;
        }

        if (checkedCheckbox.length > 1) {
            alert("Please select only one row");
            return false;
        }

        //#endregion

        let selectedRow = checkedCheckbox.closest("tr");

        CurrentBatchItemRow = selectedRow;

        let rowID = CurrentBatchItemRow.attr("data-rowid");

        let ItemGridindex =
            $("#TableBody tr[data-rowid='" + rowID + "']").index();

        //#region GET VALUES

        let fromWarehouse =
            selectedRow.find(".JIDNI_WH_Number").val();

        let lineItemNumber =
            selectedRow.find(".JIDNI_Item_Number").val();

        let invoiceQty =
            selectedRow.find(".JIDNI_Qty").val();

        $("#BatchPopupQty").text(invoiceQty);

        //#endregion


        // CLEAR TEMP ARRAY
        DeliveryNoteBatchList = [];

        // CLEAR OLD ROWS
        $("#DeliveryNoteBatchTableBody")
            .find(".DeliveryNoteBatchRow")
            .remove();

        //#region AJAX

        $.ajax({

            url: "/DeliveryNote/GetBatchDetails",

            type: "GET",

            data: {
                FromWarehouse: fromWarehouse,
                LineItem_Number: lineItemNumber,
                ItemGridIndex: ItemGridindex
            },

            success: function (response) {

                console.log(response);

                DeliveryNoteBatchList = [];

                if (response && response.length > 0) {

                    $.each(response, function (i, batch) {

                        DeliveryNoteBatchList.push({

                            JIDNI_BCH_WH_Number: batch.fromWarehouse,
                            JIDNI_BCH_JIDNI_Number: lineItemNumber,
                            JIDNI_BCH_WH_Name: batch.wareHouseCode,
                            JIDNI_BCH_BatchDate: batch.batchDate,
                            JIDNI_BCH_BatchNo: batch.batchNo,
                            JIDNI_BCH_QtyAvailable: batch.availableQty,
                            JIDNI_BCH_QtyReserved: batch.reservedQty,
                            JIDNI_BCH_QtyInvoice: batch.deliveredQty,
                            JIDNI_BCH_BatchUnitPrice: batch.batchUnitPrice,
                            JIDNI_BCH_BatchValue: batch.batchValue,
                            JIDNI_BCH_Number: batch.lineBatch_Number
                        });

                    });

                } else {

                    DeliveryNoteBatchList.push({});
                }

                BindDeliveryNoteBatchTable();
                BindOtherBatch(fromWarehouse, lineItemNumber, ItemGridindex);

                //#region logic for mismatch qty
                var currentItemGridSelectedRow = GetCheckedRowId();
                ApplyBatchValues(currentItemGridSelectedRow);
                //#endregion

        

                $("#DeliveryNoteBatchModal")
                    .modal("show");
            },

            error: function (xhr, status, error) {

                console.log("Status:", status);
                console.log("Error:", error);
                console.log("Response Text:", xhr.responseText);

                alert("Error loading batch details");
            }

        });

        //#endregion
   
  

});

$("#DeliveryNoteBatchModal").on("shown.bs.modal", function () {

    setTimeout(function () {

        let input = document.querySelector(
            '#DeliveryNoteBatchTableBody tr:not([style*="display:none"]) .JIDNI_BCH_QtyInvoice'
        );

        if (input) {
            input.focus();
            input.select();
        }

    }, 200);

});

//#endregion SHOW BATCH

//#region apply batch values
function ApplyBatchValues(rowId) {

    let batchValues = GetBatchValues(rowId);

    if (batchValues.length === 0)
        return;

    let rows = $("#DeliveryNoteBatchTableBody tr")
        .not("#DeliveryNoteBatchTemplateRow");

    if (rows.length !== batchValues.length)
        return;

    rows.each(function (index) {

        $(this)
            .find(".JIDNI_BCH_QtyInvoice")
            .val(batchValues[index]);
    });
    CalculateBatchFooter();
}
//#endregion

//#region ADD DATA TO ARRAY

function AddDeliveryNoteBatchRow(data = {}) {

    let batchObj = {
        JIDNI_BCH_BatchDate: data?.JIDNI_BCH_BatchDate || "",
        JIDNI_BCH_WH_Number: data?.JIDNI_BCH_WH_Number || 0,
        JIDNI_BCH_JIDNI_Number: data?.JIDNI_BCH_JIDNI_Number||0,
        JIDNI_BCH_WH_Name: data?.JIDNI_BCH_WH_Name || "",
        JIDNI_BCH_BatchNo: data?.JIDNI_BCH_BatchNo || "",
        JIDNI_BCH_QtyAvailable: data?.JIDNI_BCH_QtyAvailable || 0,
        JIDNI_BCH_QtyReserved: data?.JIDNI_BCH_QtyReserved || 0,
        JIDNI_BCH_QtyInvoice: data?.JIDNI_BCH_QtyInvoice || 0,
        JIDNI_BCH_BatchUnitPrice: data?.JIDNI_BCH_BatchUnitPrice || 0,
        JIDNI_BCH_BatchValue: data?.JIDNI_BCH_BatchValue || 0
    };

    DeliveryNoteBatchList.push(batchObj);

    BindDeliveryNoteBatchTable();
}

//#endregion



//#region BIND TABLE
function BindDeliveryNoteOtherBatchTable(response) {

    let tbody = $("#DeliveryNoteOtherBatchTableBody");

    // Clear all rows except template
    tbody.find(".DeliveryNoteOtherBatchRow").remove();

    $.each(response, function (index, data) {

        let row =
            $("#DeliveryNoteOtherBatchTemplateRow")
                .clone()
                .removeAttr("id")
                .removeAttr("style")
                .show()
                .addClass("DeliveryNoteOtherBatchRow");

        row.find(".JIDNI_BCH_Number")
            .val(data.lineBatch_Number);

        row.find(".JIDNI_BCH_WH_Number")
            .val(data.fromWarehouse);

        row.find(".JIDNI_BCH_WH_Name")
            .val(data.wareHouseCode);

        row.find(".JIDNI_BCH_BatchDate")
            .val(data.batchDate);

        row.find(".JIDNI_BCH_BatchNo")
            .val(data.batchNo);

        row.find(".JIDNI_BCH_AvailableQty")
            .val(data.availableQty);

        row.find(".JIDNI_BCH_BatchUnitPrice")
            .val(data.batchUnitPrice);

        row.find(".JIDNI_BCH_BatchValue")
            .val(data.batchValue);

        tbody.append(row);

    });
    if (response.length === 0) {
        tbody.append(`
        <tr class="DeliveryNoteOtherBatchRow">
            <td style="height:25px;"></td>
            <td></td>
            <td></td>
            <td>0</td>
            <td>0</td>
            <td>0</td>
        </tr>
    `);
        return;
    }

    CalculateOtherBatchFooter();
}
function CalculateOtherBatchFooter() {

    let totalQty = 0;
    let totalValue = 0;

    $("#DeliveryNoteOtherBatchTableBody .DeliveryNoteOtherBatchRow").each(function () {

        totalQty += parseFloat($(this)
            .find(".JIDNI_BCH_AvailableQty").val()) || 0;

        totalValue += parseFloat($(this)
            .find(".JIDNI_BCH_BatchValue").val()) || 0;
    });

    $("#TotalBatchQtyOther").val(totalQty.toFixed(2));
    $("#TotalBatchValueOther").val(totalValue.toFixed(2));

    if ($("#DeliveryNoteOtherBatchTableBody .DeliveryNoteOtherBatchRow").length > 0)
        $("#DeliveryNoteOtherBatchList tfoot").show();
    else
        $("#DeliveryNoteOtherBatchList tfoot").hide();
}
function BindOtherBatch(fromWarehouse, lineItemNumber, ItemGridindex) {
    //#region AJAX

    $.ajax({

        url: "/DeliveryNote/GetOtherBatchDetails",

        type: "GET",

        data: {
            FromWarehouse: fromWarehouse,
            LineItem_Number: lineItemNumber,
            ItemGridIndex: ItemGridindex
        },

        success: function (response) {

            console.log(response);
            BindDeliveryNoteOtherBatchTable(response);
          

       

        
        },

        error: function (xhr, status, error) {

            console.log("Status:", status);
            console.log("Error:", error);
            console.log("Response Text:", xhr.responseText);

            alert("Error loading batch details");
        }

    });

    //#endregion
}

function BindDeliveryNoteBatchTable() {

    $("#DeliveryNoteBatchTableBody").find(".DeliveryNoteBatchRow").remove();
   

    $.each(DeliveryNoteBatchList, function (index, data) {

        // Check required values
        if (
            data.JIDNI_BCH_WH_Number != undefined &&
            data.JIDNI_BCH_JIDNI_Number != undefined &&
            data.JIDNI_BCH_WH_Name != undefined &&
            data.JIDNI_BCH_BatchDate != undefined &&
            data.JIDNI_BCH_BatchNo != undefined &&
            data.JIDNI_BCH_QtyAvailable != undefined
        ) {

            let row =
                $("#DeliveryNoteBatchTemplateRow")
                    .clone()
                    .removeAttr("id")
                    .removeAttr("style")
                    .show()
                    .addClass("DeliveryNoteBatchRow");

            row.find(".JIDNI_BCH_WH_Number")
                .val(data.JIDNI_BCH_WH_Number);

            row.find(".JIDNI_BCH_JIDNI_Number")
                .val(data.JIDNI_BCH_JIDNI_Number);

            row.find(".JIDNI_BCH_WH_Name")
                .val(data.JIDNI_BCH_WH_Name);

            row.find(".JIDNI_BCH_BatchDate")
                .val(data.JIDNI_BCH_BatchDate);

            row.find(".JIDNI_BCH_BatchNo")
                .val(data.JIDNI_BCH_BatchNo);
            row.find(".JIDNI_BCH_Number")
                .val(data.JIDNI_BCH_Number);

            row.find(".JIDNI_BCH_QtyAvailable")
                .val(data.JIDNI_BCH_QtyAvailable);

            row.find(".JIDNI_BCH_QtyReserved")
                .val(data.JIDNI_BCH_QtyReserved);

            row.find(".JIDNI_BCH_QtyInvoice")
                .val(data.JIDNI_BCH_QtyInvoice);

            row.find(".JIDNI_BCH_BatchUnitPrice")
                .val(data.JIDNI_BCH_BatchUnitPrice);

            row.find(".JIDNI_BCH_BatchValue")
                .val(data.JIDNI_BCH_BatchValue);

            $("#DeliveryNoteBatchTableBody").append(row);

        } else {

            $("#DeliveryNoteBatchTableBody").append(`
    <tr class="DeliveryNoteBatchRow">
        <td style="height:25px;"></td>
        <td style="height:25px;"></td>
        <td style="height:25px;"></td>
        <td style="height:25px;"></td>
        <td style="height:25px;"></td>
        <td style="height:25px;"></td>
        <td style="height:25px;"></td>
        <td style="height:25px;"></td>
    </tr>
`);
        }

    });

    CalculateBatchFooter();
}

//#endregion

//#region QTY INVOICE VALIDATION

function ValidateBatchQty() {

    let InvoiceQty =
        parseFloat(removeCommas($("#BatchPopupQty").text())) || 0;

    let BatchQty = $("#DeliveryNoteBatchTableBody tr")
        .not("#DeliveryNoteBatchTemplateRow")
        .map(function () {

            return parseFloat(
                removeCommas(
                    $(this).find(".JIDNI_BCH_QtyInvoice").val()
                )
            ) || 0;

        }).get()
        .reduce((sum, qty) => sum + qty, 0);

    console.log("InvoiceQty :", InvoiceQty);
    console.log("BatchQty :", BatchQty);

    if (InvoiceQty !== BatchQty) {

        alert("Qty Mismatch !");
        var ItemGridRowSelected = GetCheckedRowId();
        console.log('----ItemGridRowSelected--------' + ItemGridRowSelected)
        StoreBatchMismatch(ItemGridRowSelected);
        CloseDeliveryNoteBatchModal();

         
        return false;
    } else {

        let rowId = GetCheckedRowId();

        batchMismatchData =
            batchMismatchData.filter(x => x.rowId !== rowId);
    }

    return true;
}

let batchMismatchData = [];
//#endregion

//#region batch mismatch fill grid
function GetBatchValues(rowId) {

    let row = batchMismatchData.find(x => x.rowId === rowId);

    return row ? row.batchValues : [];
}
function StoreBatchMismatch(rowId) {

    let batchValues = $("#DeliveryNoteBatchTableBody tr")
        .not("#DeliveryNoteBatchTemplateRow")
        .map(function () {
            return parseFloat(
                removeCommas(
                    $(this).find(".JIDNI_BCH_QtyInvoice").val()
                )
            ) || 0;
        }).get();

    batchMismatchData =
        batchMismatchData.filter(x => x.rowId !== rowId);

    batchMismatchData.push({
        rowId: rowId,
        batchValues: batchValues
    });
    console.log('-------batchMismatchData--------' + JSON.stringify(batchMismatchData));
   
}
//#endregion
 
//#region  focus item grid on qty mismatch
function FocusItemGridQty() {

    if (!CurrentBatchItemRow)
        return;

    let rowID =
        CurrentBatchItemRow.attr("data-rowid");

    let QtyInput =
        $("#TableBody")
            .find(`tr[data-rowid='${rowID}']`)
            .find(".JIDNI_Qty");

    if (QtyInput.length > 0) {

        QtyInput.focus();
        QtyInput.select();

    }

}
//#endregion

function CloseDeliveryNoteBatchModal() {

    const modal = $("#DeliveryNoteBatchModal");

    modal.one("hidden.bs.modal", function () {

        setTimeout(function () {
            FocusItemGridQty();
        }, 500);

    });

    modal.modal("hide");
}

$(document).on('input', ".JIDNI_BCH_QtyInvoice", function (event) {

    var row = $(this).closest("tr");

    var QtyAvailable =
        parseFloat(removeCommas(row.find(".JIDNI_BCH_QtyAvailable").val())) || 0;

    var QtyReserved =
        parseFloat(removeCommas(row.find(".JIDNI_BCH_QtyReserved").val())) || 0;

    var QtyInvoiceInput = row.find(".JIDNI_BCH_QtyInvoice");

    var QtyInvoice =
        parseFloat(removeCommas(QtyInvoiceInput.val())) || 0;

    var BalanceQty = QtyAvailable - QtyReserved;

    if (QtyInvoice > BalanceQty) {

        alert(
            "Invoice Qty (" + QtyInvoice +
            ") cannot be greater than Available Qty - Reserved Qty (" + BalanceQty + ").\n" +
            "It will be reset to maximum allowed: " + BalanceQty
        );

        QtyInvoiceInput.val(DecimalIndianRupees(BalanceQty));
        QtyInvoice = BalanceQty;

        QtyInvoiceInput.focus().select();
    }

    CalculateBatchFooter();
});

//#endregion

//#region SAVE TEMP BATCH (BULK VERSION)
function SaveTempBatch() {

    let batchList = [];

    $("#DeliveryNoteBatchTableBody tr.DeliveryNoteBatchRow:visible").each(function () {

        let currentRow = $(this);

        if (currentRow.length == 0)
            return;

        let Qty =
            parseFloat(currentRow.find(".JIDNI_BCH_QtyInvoice").val()) || 0;

        if (Qty <= 0)
            return;

        let rowID = CurrentBatchItemRow.attr("data-rowid");

        let ItemGridindex =
            $("#TableBody tr[data-rowid='" + rowID + "']").index();

        let model = {
            DBCH_RowGuid: rowID,
            DBCH_Number:
                parseInt(currentRow.find(".JIDNI_BCH_Number").val()) || 0,

            DBCH_Index:
                parseInt(ItemGridindex) || 0,

            DBCH_DBCH_Number:
                parseInt(currentRow.find(".JIDNI_BCH_Number").val()) || 0,

            DBCH_Item_Number:
                parseInt(currentRow.find(".JIDNI_BCH_JIDNI_Number").val()) || 0,

            DBCH_Warehouse_Number:
                parseInt(currentRow.find(".JIDNI_BCH_WH_Number").val()) || 0,

            DBCH_Date:
                new Date(currentRow.find(".JIDNI_BCH_BatchDate").val()).toISOString(),

            DBCH_No:
                currentRow.find(".JIDNI_BCH_BatchNo").val(),

            DBCH_Qty: Qty,

            DBCH_UnitPrice:
                parseFloat(currentRow.find(".JIDNI_BCH_BatchUnitPrice").val()) || 0,

            DBCH_Value:
                parseFloat(currentRow.find(".JIDNI_BCH_BatchValue").val()) || 0,
            JIDNI_NUMBER:
                parseInt(currentRow.find(".JIDNI_Number").val()) || 0,

            JIDNH_NUMBER:
                parseInt(currentRow.find(".JIDNH_Number").val()) || 0,

            RefBatch_Number:
                parseInt(currentRow.find(".RefBatch_Number").val()) || 0,
            Mode: 1,
            CreatorCode: 1,
            CreatorDate: new Date().toISOString()
        };

        batchList.push(model);
    });

    if (batchList.length === 0) {
        console.log("No valid batch data to save.");
        return;
    }

    $.ajax({

        url: '/DeliveryNote/SaveTempDeliveryBatch',

        type: 'POST',

        contentType: 'application/json',

        data: JSON.stringify(batchList),

        success: function (response) {

            console.log('Batch save completed');
            console.log(batchList);
            console.log(response);
        },

        error: function (xhr) {

            console.log(xhr.responseText);
        }
    });
}
//#endregion

//#region get item checked row
function GetCheckedRowId() {

    let rowId = -1;

    $("#TableBody tr.NewRow:visible").each(function (index) {

        if ($(this).find(".CheckItem").prop("checked")) {
            rowId = index + 1;
            return false; // break
        }
    });

    return rowId;
}
//#endregion
