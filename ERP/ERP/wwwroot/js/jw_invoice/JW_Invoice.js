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

var DeliveryNoteMap = {};
let ItemGSTMap = {};
let CurrentGSTRow = null;
$(document).ready(function () {
    //#region call service order onclick
    $(document).on("focus", ".JISVII_JISVOH_Number", function () {
        console.log("dropdown focused");

        let dropdown = $(this);
        LoadServiceOrderDropdown(dropdown);
    });
    //#endregion




    //#region Row Click - Single Selection
    $("#ItemTable").on("click", "tbody tr.NewRow", function (e) {

        // If checkbox clicked directly, skip row selection logic
        if ($(e.target).closest(".CheckItem").length) {
            return;
        }

        // Uncheck all row checkboxes
        $("#ItemTable .CheckItem").prop("checked", false);

        // Check only clicked row
        $(this).find(".CheckItem").prop("checked", true);
    });
    //#endregion


    //#region Checkbox Click - Multiple Selection
    $("#ItemTable").on("click", ".CheckItem", function (e) {

        // Prevent checkbox click from triggering row click
        e.stopPropagation();
    });
    //#endregion

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
    $(document).on("click", "#btnClearAll", function () {
        ClearAll();
    });
    $(document).on("click", "#ItemTable input", function (e) {
        e.stopPropagation();

        let input = this;
        input.focus();

        setTimeout(function () {
            input.select();
        }, 10);
    });
 
   // LoadJWCAddress();
  //  LoadServiceOrders();
    //#region CLICK ADDRESS BUTTON, ADD ADDRESS ROW, DELETE ADDRESS ROW
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

    //#endregion CLICK ADDRESS BUTTON, ADD ADDRESS ROW, DELETE ADDRESS ROW
    $(document).on('click', '#RemoveItemRowButton', function () {

        //#region REMOVE CHECKED ROWS

        $("#TableBody tr.NewRow").each(function () {

            var isChecked = $(this)
                .find(".CheckItem")
                .is(":checked");

            if (isChecked) {

                $(this).hide();
                $(this).attr("data-deleted", "1");
                CheckAndRemoveEmptyHeaders();
            }

        });

        //#endregion

    });

    $("#Header_JISVIH_InvoiceDate").on("change", function () {
       // console.log("Date changed:", $(this).val());

        loadTaxCluster(); // your function
    });
    $("#Header_JISVIH_JW_Customer_Number").change(function () {
        LoadServiceOrders();
    });

    //#region gst popup

    $(document).on('click', '.GSTView', function () {
        let CheckedCheckbox = document.querySelector('.CheckItem:checked');
        if (CheckedCheckbox) {
            var currentRow = $(CheckedCheckbox).closest('tr.NewRow');
            CurrentGSTRow = currentRow.index();
            var ItemNumber = currentRow.find('input.JISVII_Item_Number').val();
            //var Index = currentRow.find('input.SII_Index').val();
            var SACNumber = currentRow.find('input.SAC_Number').val();
           

            var Cluster = $("#Header_JISVIH_TCT_Number").val();
            var SIHDate = $("#Header_JISVIH_InvoiceDate").val();

            var qty = parseFloat(removeCommas(currentRow.find("input.JISVII_Qty").val())) || 0;
            var unitPrice = parseFloat(removeCommas(currentRow.find("input.JISVII_UnitPrice").val())) || 0;
            var Amount = parseFloat(removeCommas(currentRow.find("input.JISVII_Amount").val())) || 0;

            var BaseAmount = parseFloat(removeCommas(Amount)) ;

            if (ItemNumber && SACNumber) {
                $.ajax({
                    type: "get",
                    url: "/gst/view",
                    data: { Cluster: Cluster, SIHDate: SIHDate, SAC: SACNumber, BaseAmount: BaseAmount },
                  
                   
                    success: function (data) {
                        const Table = document.getElementById('GSTTableView');
                        Table.innerHTML = "";

                        if (Table) {
                            const TaxView = ClusterTaxView(data);
                            Table.appendChild(TaxView);
                        }

                        new bootstrap.Modal($("#GSTView")).show();
                        $('#GSTView').on('shown.bs.modal', function () {
                            $(this).find('[autofocus]').focus();
                        });
                    }
                });
            }
        }
    });

    //#endregion

    $(document).on("keyup change", ".JISVII_Qty, .JISVII_UnitPrice", function () {

        var row = $(this).closest("tr");

        var qty = parseFloat(
            row.find(".JISVII_Qty").val()
        ) || 0;

        var unitPrice = parseFloat(
            row.find(".JISVII_UnitPrice").val()
        ) || 0;

        var amount = qty * unitPrice;

        row.find(".JISVII_Amount")
            .val(amount.toFixed(2));

        CalculateTotals();
    });

    $(document).on("keyup change", ".JISVII_Qty", function () {

        var row = $(this).closest("tr");

        var deliveredQty = parseFloat(
            row.find(".JISVII_DeliveredQty").text()
        ) || 0;

        var prevInvoiceQty = parseFloat(
            row.find(".JISVII_PrevInvoiceQty").text()
        ) || 0;

        var currentQty = parseFloat(
            row.find(".JISVII_Qty").val()
        ) || 0;

        var balanceQty = deliveredQty - prevInvoiceQty;

        // Validation
        if (currentQty > balanceQty) {

            alert("Current Invoice Qty cannot exceed Balance Qty (" + balanceQty + ")");

            row.find(".JISVII_Qty").val(balanceQty);

            currentQty = balanceQty;

        } else {

           
            let jisvohNumber = row.find(".JISVII_ServiceOrderHidden").val() || 0;
            console.log('---if value is there in so:' + jisvohNumber)
            if (jisvohNumber > 0) {
                GetAllowedQty(
                    jisvohNumber,
                    row.find(".JISVII_PRS_Number").val() || 0,
                    row.find(".JISVII_Item_Number").val() || 0,
                    row.find(".JISVII_UoM_Number").val() || 0,
                    function (allowedQty) {

                        console.log("Allowed Qty:", allowedQty);

                        if (currentQty > allowedQty) {
                            alert("Allowed Qty: " + allowedQty);
                           // row.find(".JISVII_Qty").val(allowedQty);
                        }
                    }
                );
            }
        }

        // Prevent negative values
        if (currentQty < 0) {

            row.find(".JISVII_Qty").val(0);

            currentQty = 0;

        }

        var unitPrice = parseFloat(
            row.find(".JISVII_UnitPrice").val()
        ) || 0;

        var amount = currentQty * unitPrice;

        row.find(".JISVII_Amount")
            .val(amount.toFixed(2));

        CalculateTotals();

    });

    $(document).on("input change", ".JISVII_Qty, .JISVII_UnitPrice", async function () {

        const $row = $(this).closest("tr");

        const qty = parseFloat($row.find(".JISVII_Qty").val()) || 0;

        const unitPrice = parseFloat($row.find(".JISVII_UnitPrice").val()) || 0;

        const baseAmount = qty * unitPrice;

        const cluster = $("#Header_JISVIH_TCT_Number").val();

        const invoiceDate = $("#Header_JISVIH_InvoiceDate").val();

        const sacNumber = $row.find("input.SAC_Number").val();

        let gstAmount = 0;

        if (cluster && sacNumber) {

            gstAmount = await GetGSTAmount(
                cluster,
                invoiceDate,
                sacNumber,
                baseAmount
            );
        }

        gstAmount = parseFloat(gstAmount || 0).toFixed(2);

        $row.find(".JISVII_GST_Amount").val(gstAmount);

    });


});

function GetAllowedQty(jisvohNumber, prsNumber, itemNumber, uomNumber, callback) {
    $.get("/DeliveryNote/CheckDeliveredQtyExceeded", {
        jisvohNumber: jisvohNumber,
        prsNumber: prsNumber,
        itemNumber: itemNumber,
        uomNumber: uomNumber
    }, function (res) {

        if (!res || res.length === 0) {
            callback(0);
            return;
        }

        let deliveredQty = parseFloat(res[0].deliveredQty) || 0;
        let jisvoiQty = parseFloat(res[0].jisvoiQty) || 0;

        let allowedQty = jisvoiQty - deliveredQty;

        callback(allowedQty);
    });
}

function DateBind() {
    var today = new Date();

    var day = String(today.getDate()).padStart(2, '0');
    var months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun",
        "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

    var formattedDate = day + "-" + months[today.getMonth()] + "-" + today.getFullYear();

    var fp = document.getElementById("Header_JISVIH_InvoiceDate")._flatpickr;
    if (fp) fp.setDate(formattedDate, true, "d-M-Y");
}
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
 
async function GetGSTAmount(cluster, invoiceDate, sacNumber, baseAmount) {

    const response = await $.ajax({

        url: '/income/gst',

        type: 'GET',

        data: {
            Cluster: cluster,
            SIHDate: invoiceDate,
            SAC: sacNumber,
            BaseAmount: baseAmount
        }

    });

    return response;
}
function CalculateTotals() {

    var totalDeliveredQty = 0;
    var totalPrevInvoiceQty = 0;
    var totalQty = 0;
    var totalAmount = 0;
    var totalGSTAmount = 0;

    $("#TableBody tr.NewRow:visible").each(function () {

        totalDeliveredQty += parseFloat(
            $(this).find(".JISVII_DeliveredQty").text()
        ) || 0;

        totalPrevInvoiceQty += parseFloat(
            $(this).find(".JISVII_PrevInvoiceQty").text()
        ) || 0;

        totalQty += parseFloat(
            $(this).find(".JISVII_Qty").val()
        ) || 0;

        totalAmount += parseFloat(
            $(this).find(".JISVII_Amount").val()
        ) || 0;

        totalGSTAmount += parseFloat(
            $(this).find(".JISVII_GST_Amount").val()
        ) || 0;

    });

    $("#TotalDeliveredQty").val(totalDeliveredQty.toFixed(2));
    $("#TotalPrevInvoiceQty").val(totalPrevInvoiceQty.toFixed(2));
    $("#TotalQty").val(totalQty.toFixed(2));
    $("#TotalAmount").val(totalAmount.toFixed(2));
    $("#TotalGSTAmount").val(totalGSTAmount.toFixed(2));

}

function ClusterTaxView(data) {
    const table = document.createElement('table');
    table.classList.add('table', 'table-bordered', 'table-hover', 'align-middle', 'w-100', 'mb-0');

    const thead = table.createTHead();
    const headerRow = thead.insertRow();
    const headers = ["Tax Category", "Tax Type", "Tax Index", "Tax Element", "Assessable Value", "Tax Rate", "Tax Amount", "Load on Inventory", "Load on Inventory %",];
    headers.forEach(headerText => {
        const th = document.createElement('th');
        th.textContent = headerText;
        th.classList.add('table-info');

        if (headerText.includes('Assessable Value') || headerText.includes('Tax Rate') || headerText.includes('Tax Amount') || headerText.includes('Load on Inventory %')) {
            th.classList.add('text-end', 'table-width-xl');
        } else if (headerText.includes('Tax Index') || headerText.includes('Load on Inventory')) {
            th.classList.add('text-center', 'table-width-md');
        }

        headerRow.appendChild(th);
    });

    const tbody = table.createTBody();
    let totalAssessable = 0;
    let totalTaxAmount = 0;


    data.forEach(tax => {
        const row = tbody.insertRow();
        // ADD HERE
        row.dataset.gstc = tax.gstcNumber;
        row.dataset.gstt = tax.gsttNumber;
        row.dataset.gste = tax.gsteNumber;
        row.dataset.taxindex = tax.taxIndex;
        const CategoryCell = row.insertCell();
        const TypeCell = row.insertCell();
        const TaxIndexCell = row.insertCell();
        const TaxElementCell = row.insertCell();
        const AssessableCell = row.insertCell();
        const PercentageCell = row.insertCell();
        const AmountCell = row.insertCell();
        const LoadonCell = row.insertCell();
        const LoadonPerCell = row.insertCell();

        CategoryCell.textContent = tax.taxCategory;
        TypeCell.textContent = tax.taxType;

        TaxIndexCell.textContent = tax.taxIndex;
        TaxIndexCell.classList.add("text-center", "table-width-md");

        TaxElementCell.textContent = tax.taxElement;

        AssessableCell.textContent = tax.assessableValue.toFixed(2);
        AssessableCell.classList.add("text-end", "table-width-xl");

        PercentageCell.textContent = tax.percentage.toFixed(2);
        PercentageCell.classList.add("text-end", "table-width-xl");

        AmountCell.textContent = tax.amount.toFixed(2);
        AmountCell.classList.add("text-end", "table-width-xl");

        LoadonCell.textContent = tax.loadonInventory;
        LoadonCell.classList.add("text-center", "table-width-md");

        LoadonPerCell.textContent = tax.loadonInventoryPercent;
        LoadonPerCell.classList.add("text-end", "table-width-xl");

        totalAssessable += tax.assessableValue;
        totalTaxAmount += tax.amount;
    });

    const tfoot = table.createTFoot();
    const footerRow = tfoot.insertRow();

    const footerCells = headers.map(() => footerRow.insertCell());
    footerCells.forEach(cell => {
        cell.classList.add('table-info');
    });

    footerCells[footerCells.length - 4].textContent = `Total`;
    footerCells[footerCells.length - 4].style.textAlign = 'right';
    footerCells[footerCells.length - 3].textContent = totalTaxAmount.toFixed(2);
    footerCells[footerCells.length - 3].style.textAlign = 'right';

    return table;
}

function loadTaxCluster() {

    var customerNumber = $("#Header_JISVIH_JW_Customer_Number").val();
    var invoiceDate = $("#Header_JISVIH_InvoiceDate").val();

    if (customerNumber === "" || invoiceDate === "") {
        return;
    }

    $.ajax({
        url: '/JobworkInvoice/Get_JW_Invoice_Taxcluster',
        type: 'GET',
        data: {
            JWC_Number: customerNumber,
            CheckDate: invoiceDate
        },

        success: function (data) {

            var ddl = $("#Header_JISVIH_TCT_Number");

            ddl.empty();
      

            $.each(data, function (i, item) {

                ddl.append(
                    $('<option>', {
                        value: item.jwC_GST_TCT_Number,
                        text: item.cuS_GST_TCT_Name
                    })
                );
            });
        }
    });
}

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
                        $("#Header_JISVIH_JW_Customer_Number").val(cust.cuS_Number);

                     //   $("#Currency_Name").val(cust.cuS_CUR_Name);
                        $("#Header_JISVIH_Currency_Number").val(cust.cuS_CUR_Number);

                     //   $("#Header_JIDNH_WH_Number").val(cust.cuS_WH_Number);
                        //$("#SIH_BUY_LOC_Number").val(cust.cuS_LOC_Number);
                        //$("#SIH_CUR_DecimalPlaces").val(cust.cuS_CUR_DecimalPlaces);
                        //$("#SIH_WHT_Number").val(cust.cuS_WHT_Number);

                        //$("#WH_Number").val(cust.cuS_WH_Number);
                      
                        resultsDiv.hide();
                        loadTaxCluster();
                     //   LoadDeliveryNoteItems();
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



//#region validate unit price
function ValidateUnitPriceAndAmount() {

    var isValid = true;
    var message = "";

    $("#TableBody tr.NewRow").each(function (index) {

        var row = $(this);

        if (row.attr("data-deleted") === "1")
            return true;

        var unitPrice = parseFloat(
            row.find(".JISVII_UnitPrice").val()
        ) || 0;

        var amount = parseFloat(
            row.find(".JISVII_Amount").val()
        ) || 0;

        row.removeClass("error-row");

        if (unitPrice <= 0) {

            row.addClass("error-row");

            message =
                "Row " + (index + 1) +
                " : Unit Price cannot be 0";

            row.find(".JISVII_UnitPrice").focus();

            isValid = false;
            return false;
        }

        if (amount <= 0) {

            row.addClass("error-row");

            message =
                "Row " + (index + 1) +
                " : Amount cannot be 0";

            row.find(".JISVII_Amount").focus();

            isValid = false;
            return false;
        }
    });

    if (!isValid) {
     
        showAlert(message);
        return false;
    }

    return true;
}
//#endregion
//#region Save Function
function ValidateItemTable() {
    let hasRow = false;

    $("#ItemTable tbody tr.NewRow").each(function () {
        let row = $(this);

        if (row.attr("data-deleted") != "1") {
            hasRow = true;
            return false;
        }
    });

    if (!hasRow) {
        return "At least one item row is required";
    }

    return "";
}
function validateHeaderById() {

    // 1. Invoice No
    if ($("#Header_JISVIH_InvoiceNo").val().trim() === "") {

        showAlert(
            'Invoice No is required',
            '#Header_JISVIH_InvoiceNo'
        );

        return false;
    }

    // 2. Invoice Date
    if ($("#Header_JISVIH_InvoiceDate").val().trim() === "") {

        showAlert(
            'Invoice Date is required',
            '#Header_JISVIH_InvoiceDate'
        );

        return false;
    }

    // 3. JW Customer
    if (
        $("#Header_JISVIH_JW_Customer_Number").val().trim() === "" ||
        $("#Header_JISVIH_JW_Customer_Number").val() === "0" ||
        $("#Header_JISVIH_JW_Customer_Name").val().trim() === ""
    ) {

        showAlert(
            'JW Customer is required',
            '#Header_JISVIH_JW_Customer_Name'
        );

        return false;
    }

    // 4. Currency
    if (
        $("#Header_JISVIH_Currency_Number").val() === "" ||
        $("#Header_JISVIH_Currency_Number").val() === "0"
    ) {

        showAlert(
            'Currency is required',
            '#Header_JISVIH_Currency_Number'
        );

        return false;
    }

    // 5. Terms & Conditions
    if (
        $("#Header_JISVIH_TCT_Number").val() === "" ||
        $("#Header_JISVIH_TCT_Number").val() === "0"
    ) {

        showAlert(
            'Terms & Conditions is required',
            '#Header_JISVIH_TCT_Number'
        );

        return false;
    }
    // 5. Message / MS
    if (
        $("#Header_JISVIH_MS_Number").val() === "" ||
        $("#Header_JISVIH_MS_Number").val() === "0"
    ) {

        showAlert(
            'Material Seggregation is required',
            '#Header_JISVIH_MS_Number'
        );

        return false;
    }


    return true;
}

$("#btnSave").on("click", function (e) {

    if (!validateHeaderById()) {

        e.preventDefault();
        return false;
    } if (!ValidateUnitPriceAndAmount()) {
        e.preventDefault();
        return false;
    }
    let itemValidation = ValidateItemTable();
    if (itemValidation) {
        e.preventDefault();
        showAlert(itemValidation);
        return false;
    }
    else {

        var model = CreateJobworkInvoiceModel();

        console.log(JSON.stringify(model));

        $.ajax({

            url: '/JobworkInvoice/SaveJobworkInvoice',

            type: 'POST',

            contentType: 'application/json',

            data: JSON.stringify(model),

            success: function (response) {

                if (response.success) {
                    ClearAll();
                    
                  
                  
                    showAlert('Record Inserted');
               
                    DateBind();
                    //window.location.href = response.redirectUrl;

                //    console.log(JSON.stringify(model));
                }
            },

            error: function (xhr) {

                console.log(xhr.responseText);

            }

        });

    }

});

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

//#region CREATE MODEL

function CreateJobworkInvoiceItemModel() {

    var items = [];

    $("#ItemTable tbody tr.NewRow").each(function () {

        let row = $(this);

        if (row.attr("data-deleted") == "1") {
            return;
        }
        console.log('service order dropdown value---'+row.find(".JISVII_ServiceOrderHidden").val());
        let item = {

            JISVII_Number:
                parseInt(row.find(".JISVII_Number").val()) || 0,

            JISVOI_Number:                                    // added
                parseInt(row.find(".JISVOI_Number").val()) || 0,

            JISVII_JISVOH_Number:
                parseInt(row.find(".JISVII_ServiceOrderHidden").val()) || 0,

            JISVII_Item_Number:
                parseInt(row.find(".JISVII_Item_Number").val()) || 0,

            JISVII_DN_No:
                row.find(".JISVII_DN_No").val(),

            JISVII_Process:
                row.find(".PRS_ProcessName").val(),

            JISVII_ItemCode:
                row.find(".JISVII_ItemCode").val(),

            JISVII_ItemDescription:
                row.find(".JISVII_ItemDescription").val(),

            JISVII_OuterDia:
                parseFloat(row.find(".JISVII_OuterDia").val()) || 0,

            JISVII_Thickness:
                parseFloat(row.find(".JISVII_Thickness").val()) || 0,

            JISVII_Length:
                parseFloat(row.find(".JISVII_Length").val()) || 0,

            JISVII_Width:
                parseFloat(row.find(".JISVII_Width").val()) || 0,

            JISVII_MaterialGrade:
                row.find(".JISVII_MaterialGrade").val(),

            JISVII_ItemGroup:
                row.find(".JISVII_ItemGroup").val(),

            JISVII_UoM_Number:
                parseInt(row.find(".JISVII_UoM_Number").val()) || 0,

            JISVII_Qty:
                parseFloat(row.find(".JISVII_Qty").val()) || 0,

            JISVII_UnitPrice:
                parseFloat(row.find(".JISVII_UnitPrice").val()) || 0,

            JISVII_Amount:
                parseFloat(row.find(".JISVII_Amount").val()) || 0,

            JISVII_SAC_Number:
                parseInt(row.find(".SAC_Number").val()) || 0,

            JISVII_GST_Amount:
                parseFloat(row.find(".JISVII_GST_Amount").val()) || 0,

            JISVII_PRS_Number:
                parseInt(row.find(".JISVII_PRS_Number").val()) || 0,

            JISVII_JIDNH_Number:
                parseInt(row.find(".JISVII_JIDNH_Number").val()) || 0,

            JIDNI_Number:
                parseInt(row.find(".JIDNI_Number").val()) || 0
        };

        items.push(item);
    });

    return items;
}


function CreateJobworkInvoiceModel() {

    //=====================================
    // HEADER
    //=====================================

    var header = {

        JISVIH_Number:
            parseInt($("#Header_JISVIH_Number").val()) || 0,

        JISVIH_InvoiceNo:
            $("#Header_JISVIH_InvoiceNo").val(),

        JISVIH_InvoiceDate:
            new Date($("#Header_JISVIH_InvoiceDate").val())
                .toISOString(),
        JISVIH_MS_Number:
            parseInt($("#Header_JISVIH_MS_Number").val()) || 0,
        JISVIH_JW_Customer_Number:
            parseInt($("#Header_JISVIH_JW_Customer_Number").val()) || 0,

        JISVIH_Currency_Number:
            parseInt($("#Header_JISVIH_Currency_Number").val()) || 0,

        JISVIH_TCT_Number:
            parseInt($("#Header_JISVIH_TCT_Number").val()) || 0,

        JISVIH_PaymentTerms:
            $("#Header_JISVIH_PaymentTerms").val(),

        JISVIH_PaymentMethod:
            $("#Header_JISVIH_PaymentMethod").val(),

        JISVIH_Remarks:
            $("#Header_JISVIH_Remarks").val()
    };


    // =====================================
    // ADDRESS
    // =====================================
    var addresses = [];

    $("#AddTableBody tr.AddNewRow").each(function () {

        let row = $(this);

        console.log("Rows found:", $("#AddTableBody tr.AddNewRow:visible").length);
        console.log("Address ID =", row.find(".JIDNA_Address_ID").val());
        console.log("Address =", row.find(".JIDNA_Address").val());
        console.log("City =", row.find(".JIDNA_City").val());
        console.log("State =", row.find(".JIDNA_State").val());

        // Skip deleted rows
        if (row.find(".JIDNA_IsDeleted").val() == "1") {
            return true; // continue next row
        }

        // Skip empty rows
        if (!row.find(".JIDNA_Address_ID").val()) {
            return true;
        } let address = {
            JISVIA_JISVIH_Number:
                parseInt(row.find(".JIDNA_JIDNH_Number").val()) || 0,

            JISVIA_Number:
                parseInt(row.find(".JIDNA_Number").val()) || 0,

            JISVIA_ADTP_Number:
                parseInt(row.find(".JIDNA_ADTP_Number").val()) || 0,

            JISVIA_Address_ID:
                row.find(".JIDNA_Address_ID").val() || "",

            JISVIA_Address:
                row.find(".JIDNA_Address").val() || "",

            JISVIA_City:
                row.find(".JIDNA_City").val() || "",

            JISVIA_State:
                row.find(".JIDNA_State").val() || "",

            JISVIA_Country:
                row.find(".JIDNA_Country").val() || "",

            JISVIA_PIN:
                row.find(".JIDNA_PIN").val() || "",

            JISVIA_GSTIN:
                row.find(".JIDNA_GSTIN").val() || ""
        };
        addresses.push(address);
    });

    console.log(addresses);

    //=====================================
    // FINAL MODEL
    //=====================================

    var jobworkInvoiceModel = {

        Header: header,
        Items: CreateJobworkInvoiceItemModel(),
        Addresses: addresses

    };

    console.log(jobworkInvoiceModel);

    return jobworkInvoiceModel;
}

//#endregion
function GetDistinctDeliveryNoteHeaders() {

    var headerIds = [];

    $("#TableBody tr.NewRow").each(function () {

        var headerId = $(this).attr("data-dn");

        var isDeleted = $(this).attr("data-deleted");

        // ONLY ACTIVE ROWS
        if (isDeleted != "1") {

            if ($.inArray(headerId, headerIds) === -1) {

                headerIds.push(headerId);

            }

        }

    });

    return headerIds;

}
function CheckAndRemoveEmptyHeaders() {

    $.each(DeliveryNoteMap, function (headerId, itemIds) {

        var activeCount = 0;

        itemIds.forEach(function (itemId) {

            var row = $("#TableBody tr.NewRow[data-item='" + itemId + "']");

            if (row.length > 0 &&
                row.attr("data-deleted") != "1") {

                activeCount++;

            }

        });

        // ALL ITEMS DELETED
        if (activeCount == 0) {

            // REMOVE ROWS
            $("#TableBody tr.NewRow[data-dn='" + headerId + "']")
                .remove();

            // UNCHECK DELIVERY NOTE
            $(".deliverynote-checkbox[value='" + headerId + "']")
                .prop("checked", false);

            // UNCHECK RECOVER
            $(".item-delete-checkbox[value='" + headerId + "']")
                .prop("checked", false);

            // REMOVE MAP
            delete DeliveryNoteMap[headerId];

        }

    });

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
    var Buyer = $('#Header_JISVIH_JW_Customer_Number').val(); // keep if same field exists

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

let addressIndex = 0;

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

//#region jwc address
function LoadJWCAddress() {
    var jwcNumber = $("#Header_JISVIH_JW_Customer_Number").val();

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

 
//#endregion


//#region items
//#region callsp service orderdropdown filter
function LoadServiceOrderDropdown(dropdown) {

    let row = $(dropdown).closest("tr");

    let customerId = $("#Header_JISVIH_JW_Customer_Number").val();
    let prsNumber = row.find(".JISVII_PRS_Number").val();
    let itemNumber = row.find(".JISVII_Item_Number").val();
    let uomNumber = row.find(".JISVII_UoM_Number").val();
    console.log(row.find(".JISVII_PRS_Number").length);
    console.log(row.find(".JISVII_Item_Number").length);
    console.log(row.find(".JISVII_UoM_Number").length);
    $.ajax({
        url: "/DeliveryNote/GetServiceOrder",
        type: "GET",
        data: {
            customerId: customerId,
            prsNumber: prsNumber,
            itemNumber: itemNumber,
            uomNumber: uomNumber
        },
        success: function (response) {
            console.log('--what is response---' + JSON.stringify(response));

            let options = '<option value="0"></option>';

            $.each(response, function (_, item) {
                options += `<option value="${item.value}">
                            ${item.text}
                        </option>`;
            });

            $(dropdown).html(options);
        }, error: function (xhr, status, error) {

            console.log("AJAX Error");
            console.log("Status:", status);
            console.log("Error:", error);
            console.log("Response:", xhr.responseText);

            // alert("Failed to load Service Orders.");
        }
    });
}
//#endregion

//#region LOAD DELIVERY NOTE ITEMS
$("#LoadDeliveryNote").click(function () {

   


    // 4. Material Segregation
    if (
        $("#Header_JISVIH_MS_Number").val() === "" ||
        $("#Header_JISVIH_MS_Number").val() === "0"
    ) {
        showAlert(
            'Material Seggregation is required',
            '#Header_JISVIH_MS_Number'
        );
        return false;
    }
    // 3. JW Customer
    if (
        $("#Header_JISVIH_JW_Customer_Number").val().trim() === "" ||
        $("#Header_JISVIH_JW_Customer_Number").val() === "0" ||
        $("#Header_JISVIH_JW_Customer_Name").val().trim() === ""
    ) {

        showAlert(
            'JW Customer is required',
            '#Header_JISVIH_JW_Customer_Name'
        );

        return false;
    }
    LoadDeliveryNoteItems();
});

// Load delivery note items from SP and fill table
function LoadDeliveryNoteItems() {

    var customerNumber = $("#Header_JISVIH_JW_Customer_Number").val();
    var msNumber = $("#Header_JISVIH_MS_Number").val();
    var resultsDiv = $("#DeliveryNoteTableView");
    var headers = GetDistinctDeliveryNoteHeaders();


    $.ajax({

        url: '/JobworkInvoice/GetDeliveryNote_GroupItem',

        type: 'GET',

        data: {
            CustomerNumber: customerNumber,
            MSNumber: msNumber
        },

        success: function (response) {

            if (response && response.length > 0) {

                resultsDiv.empty();

                //#region TABLE

                var tableHTML = `
    <table class="table table-bordered mb-0 table-hover table-grid">

        <thead>

            <tr class="table-info">

                <th class="px-2 del"></th>

                <th>Delivery Note No</th>

                <th>Delivery Note Date</th>

              

                <th class="text-end">Qty</th>

                <th class="text-center">
                    Recover Deleted Item
                </th>

            </tr>

        </thead>

        <tbody></tbody>

    </table>`;

                var table = $(tableHTML);

                //#endregion

                //#region ROW BINDING

                response.forEach(function (DN) {

                    //#region MAIN CHECKBOX

                    var checkboxCell = $('<td class="px-2 del text-center"></td>');

                    var checkbox = $('<input type="checkbox" class="form-check-input deliverynote-checkbox">');

                    checkbox.val(DN.jidnH_Number);

                    //#region AUTO CHECK IF EXISTS IN GRID

                    if ($.inArray(DN.jidnH_Number.toString(), headers) !== -1) {

                        checkbox.prop('checked', true);

                    }

                    //#endregion

                    checkboxCell.append(checkbox);

                    //#endregion

                    //#region ITEM CHECKBOX

                    var itemCheckboxCell = $('<td class="px-2 del text-center"></td>');

                    var itemCheckbox = $('<input type="checkbox" class="form-check-input item-delete-checkbox">');

                    itemCheckbox.val(DN.jidnH_Number);


                    itemCheckboxCell.append(itemCheckbox);

                    //#endregion

                    //#region ROW

                    var row = $('<tr class="DNCheck"></tr>');

                    row.append(checkboxCell);

                    row.append('<td>' + DN.jidnH_DN_No + '</td>');

                    row.append('<td>' + DN.jidnH_DN_Date + '</td>');

                    row.append('<td class="text-end">' + parseFloat(DN.totalQty).toFixed(2) + '</td>');

                    row.append(itemCheckboxCell);

                    table.find('tbody').append(row);

                    //#endregion

                });

                //#endregion

                var getButton = $(`
                    <div class="w-100 p-2 text-center">

                        <button type="button"
                                class="btn btn-primary"
                                id="GetDeliveryNote">
                            Get
                        </button>

                    </div>`);

                resultsDiv.append(table);

                resultsDiv.append(getButton);

                resultsDiv.find('#GetDeliveryNote').on('click', function () {

                    var selectedDN = $.map(
                        table.find('.deliverynote-checkbox:checked'),
                        function (c) {
                            return c.value;
                        }
                    );
                    // REMOVE UNCHECKED DELIVERY NOTE ROWS

                    $("#TableBody tr.NewRow").each(function () {

                        var dn = $(this).attr("data-dn");

                        if ($.inArray(dn, selectedDN) === -1) {

                            var dn = $(this).attr("data-dn");

                            $(this).remove();

                            delete DeliveryNoteMap[dn];

                        }

                    });
                    // RECOVER HIDDEN ROWS


                    // ✅ Selected recovered/active item checkboxes
                    var selectedRecoveredItems = $.map(
                        table.find('.item-delete-checkbox:checked'),
                        function (c) {
                            return c.value;
                        }
                    );
                    selectedRecoveredItems.forEach(function (dnNo) {

                        $("#TableBody tr.NewRow")
                            .filter("[data-dn='" + dnNo + "'][data-deleted='1']")
                            .show()
                            .attr("data-deleted", "0");

                    });
                    console.log(selectedDN);

                    var selectedDNString = selectedDN.join(',');
                    var recoveredString = selectedRecoveredItems.join(',');

                    InsertDeliveryNoteItems(selectedDNString, selectedRecoveredItems, selectedDN);

                    $("#DNView").modal('hide');

                });

                $("#DNView").modal('show');
            }
            else {

                resultsDiv.html(`
                    <div class="text-center p-3">

                        No Delivery Note Found

                    </div>`);

            }

        }

    });

}
function GetSONOptions() {
    let options = '';
    let isFirst = true;

    sonList.forEach(function (item) {
        if (item.Value !== '') {
            options += `<option value="${item.Value}" ${isFirst ? 'selected' : ''}>
                            ${item.Text}
                        </option>`;
            isFirst = false;
        }
    });

    return options;
}
function LoadServiceOrders() {

    var customerNumber =
        $("#Header_JISVIH_JW_Customer_Number").val();

    $.ajax({
        url: '/ServiceOrder/GetServiceOrderHead',
        type: 'GET',
        data: {
            customerNumber: customerNumber
        },
        success: function (response) {
            sonList = response;
            console.log(sonList);
        }
    });
}


function OnServiceOrderChange(ele) {

    var row = $(ele).closest("tr");
    row.find(".JISVII_ServiceOrderHidden").val($(ele).val());
    var serviceOrderNo = $(ele).val();
    var prsNumber = row.find(".JISVII_PRS_Number").val();
    var itemNumber = row.find(".JISVII_Item_Number").val();
    var uomNumber = row.find(".JISVII_UoM_Number").val();

    $.ajax({
        url: '/JobworkInvoice/GetServiceOrderItemInfo',
        type: 'GET',
        data: {
            JISVOH_Number: serviceOrderNo,
            PRS_Number: prsNumber,
            Item_Number: itemNumber,
            UoM_Number: uomNumber
        },
        success: function (response) {
            console.log(response);
            console.log(JSON.stringify(response));

            var unitPriceBox = row.find(".JISVII_UnitPrice");
            var amountBox = row.find(".JISVII_Amount");
            var serviceOrderItemBox = row.find(".JISVOI_Number");

            if (!response) {

                serviceOrderItemBox.val(0);   // added

                unitPriceBox.val("")
                    .prop("readonly", false);

                amountBox.val("")
                    .prop("readonly", false);

                return;
            }

            // Set JISVOI_Number
            serviceOrderItemBox.val(response.jisvoI_Number || 0);

            // Unit Price
            if (response.unitPrice == null || response.unitPrice === "") {
                unitPriceBox.val("")
                    .prop("readonly", false);
            }
            else {
                unitPriceBox.val(response.unitPrice);
                unitPriceBox.trigger("input");
                unitPriceBox.trigger("change");
                unitPriceBox.prop("readonly", true);
              //  row.find(".JISVII_JISVOH_Number").prop("disabled", true);
                unitPriceBox.off("keydown keypress paste")
                    .on("keydown keypress paste", function (e) {
                        e.preventDefault();
                    });
              
            }

            // Amount
            //if (response.amount == null || response.amount === "") {
            //    amountBox.val("")
            //        .prop("readonly", false);
            //}
            //else {
            //    amountBox.val(response.amount);
            //    amountBox.trigger("input");
            //    amountBox.trigger("change");
            //    amountBox.prop("readonly", true);
            //    amountBox.off("keydown keypress paste")
            //        .on("keydown keypress paste", function (e) {
            //            e.preventDefault();
            //        });
            //}
        },
        error: function (err) {
            console.log(err);
            console.log(JSON.stringify(err));
        }
    });
}
function InsertDeliveryNoteItems(selectedDNString, selectedRecoveredItems, selectedDN) {

    var customerNumber = $("#Header_JISVIH_JW_Customer_Number").val();

    $.ajax({

        url: '/JobworkInvoice/GetDeliveryNote_ForInvoice',

        type: 'GET',

        data: {
            CustomerNumber: customerNumber,
            DNNumbers: selectedDNString
        },

        success: function (response) {

            console.log(response);

            $.each(response, function (index, item) {
                console.log('---first select---' + item)
                console.log(JSON.stringify(item));
                var headerId = item.jidnI_JIDNH_Number.toString();

                var itemId = item.jidnI_Number.toString();

                if (!DeliveryNoteMap[headerId]) {

                    DeliveryNoteMap[headerId] = [];

                }

                if ($.inArray(itemId, DeliveryNoteMap[headerId]) === -1) {

                    DeliveryNoteMap[headerId].push(itemId);

                }

                //#region DUPLICATE CHECK (VISIBLE + HIDDEN BOTH)

                let existingRow = $("#TableBody tr.NewRow").filter(function () {

                    var DNItemNumber = $(this)
                        .find(".JIDNI_Number")
                        .val();

                    return DNItemNumber == item.jidnI_Number;

                }).first();


                // ✅ check recovered list
                let isRecovered = selectedRecoveredItems &&
                    selectedRecoveredItems.includes(item.jidnI_JIDNH_Number.toString());


                // IF EXISTS
                if (existingRow.length > 0) {

                    // recover deleted row
                    if (isRecovered &&
                        existingRow.attr("data-deleted") == "1") {

                        existingRow
                            .show()
                            .attr("data-deleted", "0");

                    }

                    return;
                }

                //#endregion



                //#region ROW COUNT ONLY VISIBLE

                var rowCount = $("#TableBody tr.NewRow").length;

                //#endregion
                var deliveredQty = parseFloat(item.jidnI_Qty) || 0;

                var prevInvoiceQty = parseFloat(item.invoicedQty) || 0;

                var currentInvoiceQty = Math.max(
                    0,
                    deliveredQty - prevInvoiceQty
                );
                //#region condition
                let serviceOrderCell =
                    (item.hasServiceOrder == 1
                        ? `<label class="form-control JISVII_ServiceOrderLabel">
               ${item.serviceOrderNo ?? ''}
           </label>`
                        : `<select name="Items[${rowCount}].JISVII_JISVOH_Number"
                  onchange="OnServiceOrderChange(this)"
                  class="form-select JISVII_JISVOH_Number">
           </select>`)
                    +
                    `<input name="Items[${rowCount}].JISVII_JISVOH_Number"
            type="hidden"
            value="${item.serviceOrderId ?? item.jisvoH_Number ?? 0}"
            class="JISVII_ServiceOrderHidden" />`;

                let unitPriceCell = item.hasServiceOrder == 1
                    ? `<label class="form-control JISVII_UnitPriceLabel text-end">${item.jisvoI_UnitPrice ?? 0} </label>
       <input name="Items[${rowCount}].ServiceOrderId" type="hidden" value="${item.serviceOrderId ?? 0}" class="ServiceOrderId" />
       <input name="Items[${rowCount}].JISVII_UnitPrice" type="hidden" value="${item.jisvoI_UnitPrice ?? 0}" class="JISVII_UnitPrice" />`
         : `<input name="Items[${rowCount}].JISVII_UnitPrice" value="${item.jisvoI_UnitPrice ?? 0}" class="form-control JISVII_UnitPrice text-end" />`;
                //#endregion


                var row = `

<tr class="NewRow"
    data-rowid="${rowCount + 1}"
    data-dn="${item.jidnI_JIDNH_Number}"
    data-item="${item.jidnI_Number}"
    data-deleted="0">

    <td class="p-2 del">

        <input type="checkbox"
               class="CheckItem form-check-input">

    </td>

    

    <!-- SERVICE ORDER -->
   <td>
    ${serviceOrderCell}
</td>

    <!-- DELIVERY NOTE -->
    <td>

        <input name="Items[${rowCount}].JISVII_DN_No"
               value="${item.jidnH_DN_No ?? ''}"
               class="form-control JISVII_DN_No"
               readonly />

    </td>

    <!-- PROCESS -->
    <td>

        <input name="Items[${rowCount}].PRS_ProcessName"
               value="${item.prS_ProcessName ?? ''}"
               class="form-control PRS_ProcessName"
               readonly />

    </td>

    <!-- ITEM CODE -->
    <td>

        <!-- DELIVERY NOTE HEADER -->
        <input type="hidden"
               value="${item.jidnI_JIDNH_Number}"
               class="JISVII_JIDNH_Number" />
         
        <!-- ITEM NUMBER -->
        <input name="Items[${rowCount}].JISVII_Number"
               type="hidden"
               value="${item.jidnI_Number}"
               class="JISVII_Number" />
               
               <input name="Items[${rowCount}].JISVOI_Number"
       type="hidden"
       value="0"
       class="JISVOI_Number" />

               <input name="Items[${rowCount}].JIDNI_Number"
               type="hidden"
               value="${item.jidnI_Number}"
               class="JIDNI_Number" />

        <!-- ITEM -->
        <input name="Items[${rowCount}].JISVII_Item_Number"
               type="hidden"
               value="${item.jidnI_Item_Number}"
               class="JISVII_Item_Number" />

                    <input name="Items[${rowCount}].JISVII_PRS_Number"
               type="hidden"
               value="${item.jidnI_PRS_Number}"
               class="JISVII_PRS_Number" />

                    <input name="Items[${rowCount}].JISVII_UoM_Number"
               type="hidden"
               value="${item.jidnI_UoM_Number}"
               class="JISVII_UoM_Number" />
               

        <input name="Items[${rowCount}].JISVII_ItemCode"
               value="${item.itemCode ?? ''}"
               class="form-control JISVII_ItemCode"
               readonly />

    </td>

    <!-- DESCRIPTION -->
    <td>

        <input name="Items[${rowCount}].JISVII_ItemDescription"
               value="${item.itemDescription ?? ''}"
               class="form-control JISVII_ItemDescription"
               readonly />

    </td>

    <!-- OUTER DIA -->
    <td>

        <input name="Items[${rowCount}].JISVII_OuterDia"
               value="${item.outerDia ?? ''}"
               class="form-control JISVII_OuterDia text-end"
               readonly />

    </td>

    <!-- THICKNESS -->
    <td>

        <input name="Items[${rowCount}].JISVII_Thickness"
               value="${item.thickness ?? ''}"
               class="form-control JISVII_Thickness text-end"
               readonly />

    </td>

    <!-- LENGTH -->
    <td>

        <input name="Items[${rowCount}].JISVII_Length"
               value="${item.length ?? ''}"
               class="form-control JISVII_Length text-end"
               readonly />

    </td>

    <!-- WIDTH -->
    <td>

        <input name="Items[${rowCount}].JISVII_Width"
               value="${item.itm_Width ?? ''}"
               class="form-control JISVII_Width text-end"
               readonly />

    </td>

    <!-- MATERIAL GRADE -->
    <td>

        <input name="Items[${rowCount}].JISVII_MaterialGrade"
               value="${item.materialGrade ?? ''}"
               class="form-control JISVII_MaterialGrade"
               readonly />

    </td>

    <!-- ITEM GROUP -->
    <td>

        <input name="Items[${rowCount}].JISVII_ItemGroup"
               value="${item.itemGroup ?? ''}"
               class="form-control JISVII_ItemGroup"
               readonly />

    </td>

    <!-- UOM -->
    <td>

        <input name="Items[${rowCount}].JISVII_UoM"
               value="${item.uom ?? ''}"
               class="form-control JISVII_UoM text-center"
               readonly />

    </td>

    <!-- QTY 1 -->
    <td class="text-end">

        <input name="Items[${rowCount}].JISVII_Qty"
               type="hidden"
               value="${item.jidnI_Qty ?? 0}" />

        <label class="form-control text-end JISVII_DeliveredQty">

           ${item.jidnI_Qty ?? 0}

        </label>

    </td>

    <!-- QTY 2 -->
    <td class="text-end">

        <input name="Items[${rowCount}].JISVII_Qty"
               type="hidden"
               value="${item.invoicedQty}" />

        <label class="form-control text-end JISVII_PrevInvoiceQty">

            ${item.invoicedQty}

        </label>

    </td>

    <!-- EDITABLE QTY -->
    <td>

        <input name="Items[${rowCount}].JISVII_Qty"
               value="${currentInvoiceQty}"
               class="form-control JISVII_Qty text-end" />

    </td>

    <!-- UNIT PRICE -->
   <td>
    ${unitPriceCell}
</td>

    <!-- AMOUNT -->
    <td>

        <input name="Items[${rowCount}].JISVII_Amount"
               value="${0}"
               class="form-control JISVII_Amount text-end"
               readonly />

    </td>

    <!-- SAC -->
    <td>
     <input name="Items[${rowCount}].SAC_Number"
               value="${item.saC_Number ?? 0}"   type="hidden"
               class="form-control SAC_Number text-end" />

        <label class="form-control text-end SAC">

            ${item.sac ?? 0}

        </label>

    </td>

    <!-- GST -->
    <td>

        <input name="Items[${rowCount}].JISVII_GST_Amount"
               value="0"
               class="form-control JISVII_GST_Amount text-end"
               readonly />

    </td>

</tr>`;

                $("#TableBody").append(row);
               

            });
            $("#TableBody .JISVII_Qty").trigger("change");
            $("#TableBody .JISVII_UnitPrice").trigger("change");
            CalculateTotals();

        }

    });

}


//#endregion


//#region JISVII_JISVOH_Number change
$(document).on("change", ".JISVII_JISVOH_Number", function () {

    let row = $(this).closest("tr");
    let jisvohNumber = $(this).val();

    row.find(".JISVOH_Number").val(jisvohNumber);

    $.get("/DeliveryNote/CheckDeliveredQtyExceeded", {
        jisvohNumber,
        prsNumber: row.find(".JISVII_PRS_Number").val(),
        itemNumber: row.find(".JISVII_Item_Number").val(),
        uomNumber: row.find(".JISVII_UoM_Number").val()
    }, function (res) {

        if (!res || res.length === 0) return;

        let deliveredQty = parseFloat(res[0].deliveredQty) || 0;
        let jisvoiQty = parseFloat(res[0].jisvoiQty) || 0;
        let originalQty = parseFloat(row.find(".JISVII_Qty").val()) || 0;

        if ((deliveredQty + originalQty) > jisvoiQty) {
            alert("Qty Allowed: " + (jisvoiQty - deliveredQty));
            row.find(".JISVII_Qty").focus().select();
        }
    });
});

function CheckDeliveredQtyExceeded(jisvohNumber, prsNumber, itemNumber, uomNumber, originalQty, rowIndex) {
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

            //setTimeout(function () {
            //    $("#ItemTable tbody tr.NewRow")
            //        .eq(rowIndex)
            //        .find(".JISVII_Qty")
            //        .focus()
            //        .select();
            //}, 100);
        }
    });
}

function BindServiceOrder(customerId, prsNumber = null, itemNumber = null, uomNumber = null) {

    $(".JISVII_JISVOH_Number").html('<option value="0"></option>');
    if (!customerId) return;

    $.get("/DeliveryNote/GetServiceOrder",
        { customerId, prsNumber, itemNumber, uomNumber },
        data => $.each(data, (_, item) =>
            $(".JISVII_JISVOH_Number").append(
                `<option value="${item.value}">${item.text}</option>`
            )
        )
    );
}
//#endregion


//#endregion


