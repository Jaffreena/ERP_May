$(document).ready(function () {
    CalculateTotals();
});

function CalculateTotals() {

    let totalQty = 0;
    let totalAmount = 0;
    let totalGST = 0;

    $("#MyTable tbody tr").each(function () {

        let qty = parseFloat(
            $(this)
                .find(".Qty")
                .text()
                .replace(/,/g, '')
                .trim()
        ) || 0;

        let amount = parseFloat(
            $(this)
                .find(".Amount")
                .text()
                .replace(/,/g, '')
                .trim()
        ) || 0;

        let gst = parseFloat(
            $(this)
                .find(".GST")
                .text()
                .replace(/,/g, '')
                .trim()
        ) || 0;

        totalQty += qty;
        totalAmount += amount;
        totalGST += gst;
    });

    //$("#TotalQty").text(
    //    totalQty.toLocaleString('en-IN', {
    //        minimumFractionDigits: 2,
    //        maximumFractionDigits: 2
    //    })
    //);

    //$("#TotalAmount").text(
    //    totalAmount.toLocaleString('en-IN', {
    //        minimumFractionDigits: 2,
    //        maximumFractionDigits: 2
    //    })
    //);

    //$("#TotalGST").text(
    //    totalGST.toLocaleString('en-IN', {
    //        minimumFractionDigits: 2,
    //        maximumFractionDigits: 2
    //    })
    //);
}