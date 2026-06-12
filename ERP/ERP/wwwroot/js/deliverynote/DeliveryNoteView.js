
$(document).on("click", "#SaveBatchButton", function () {

    $("#DeliveryNoteBatchModal").modal("hide");

});

//#region SHOW BATCH

$(document).on("click", ".OpenBatchPopup", function (e) {

    e.preventDefault();
 

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

    let rowIndex = selectedRow.index();

    // 1-based index
    let itemGridIndex = rowIndex + 1;
  

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

        url: "/DeliveryNote/GetBatchDetailsViewDB",

        type: "GET",

        data: {
            FromWarehouse: fromWarehouse,
            LineItem_Number: lineItemNumber
        },

        success: function (response) {

            console.log(response);

            DeliveryNoteBatchList = [];

            if (response && response.length > 0) {

                $.each(response, function (i, batch) {

                    DeliveryNoteBatchList.push({

                    
                        JIDNI_BCH_WH_Name: batch.wareHouseCode,
                        JIDNI_BCH_BatchDate: batch.batchDate,
                        JIDNI_BCH_BatchNo: batch.batchNo,
                        JIDNI_BCH_QtyAvailable: batch.batchQty,
                  
                        JIDNI_BCH_BatchUnitPrice: batch.batchUnitPrice,
                        JIDNI_BCH_BatchValue: batch.batchValue 
                    });

                });

            } else {

                DeliveryNoteBatchList.push({});
            }

            BindDeliveryNoteBatchTable();
            BindOtherBatch(fromWarehouse, lineItemNumber, ItemGridindex);

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

function BindDeliveryNoteBatchTable() {

    $("#DeliveryNoteBatchTableBody")
        .find(".DeliveryNoteBatchRow")
        .remove();

    $.each(DeliveryNoteBatchList, function (index, data) {

        // Required values check
        if (
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

             

            row.find(".JIDNI_BCH_WH_Name")
                .val(data.JIDNI_BCH_WH_Name || "");

            row.find(".JIDNI_BCH_BatchDate")
                .val(data.JIDNI_BCH_BatchDate || "");

            row.find(".JIDNI_BCH_BatchNo")
                .val(data.JIDNI_BCH_BatchNo || "");

            

            row.find(".JIDNI_BCH_QtyAvailable")
                .val(data.JIDNI_BCH_QtyAvailable || 0);

            row.find(".JIDNI_BCH_BatchUnitPrice")
                .val(data.JIDNI_BCH_BatchUnitPrice || 0);

            row.find(".JIDNI_BCH_BatchValue")
                .val(data.JIDNI_BCH_BatchValue || 0);

            $("#DeliveryNoteBatchTableBody")
                .append(row);
        }

    });

    CalculateBatchFooter();
}