$(document).ready(function () {
    CalculateTotals();
});

function CalculateTotals() {

    let totalQty = 0;
    let totalAmount = 0;
    let totalGST = 0;

    $("#MyTable tbody tr").each(function () {

        totalQty += parseFloat(
            $(this).find(".Qty").text().replace(/,/g, '').trim()
        ) || 0;

        totalAmount += parseFloat(
            $(this).find(".Amount").text().replace(/,/g, '').trim()
        ) || 0;

        totalGST += parseFloat(
            $(this).find(".GST").text().replace(/,/g, '').trim()
        ) || 0;
    });

    //$("#TotalQty").text(totalQty.toLocaleString('en-IN'));

    //$("#TotalAmount").text(totalAmount.toLocaleString('en-IN', {
    //    minimumFractionDigits: 2,
    //    maximumFractionDigits: 2
    //}));

    //$("#TotalGST").text(totalGST.toLocaleString('en-IN', {
    //    minimumFractionDigits: 2,
    //    maximumFractionDigits: 2
    //}));
}