document.addEventListener('DOMContentLoaded', function () {
    const inputElements = document.querySelectorAll('.Key');

    inputElements.forEach(function (inputElement) {
        inputElement.addEventListener('input', function (event) {
            let value = event.target.value;
            if (value.startsWith(' ')) {
                value = value.trimStart();
            }
            value = value.replace(/\s+/g, ' ');
            event.target.value = value;
        });
    });

    inputElements.forEach(textbox => {
        textbox.addEventListener("blur", function (event) {
            let trimmedValue = textbox.value.trim();
            textbox.value = trimmedValue;
        });
    });
});

document.addEventListener('input', function (event) {
    if (event.target && event.target.classList.contains('number-only')) {
        let value = event.target.value;
        value = value.replace(/[^0-9]/g, '');
        event.target.value = value;
    }
    if (event.target && event.target.classList.contains('decimal')) {
        let value = event.target.value;
        value = value.replace(/[^0-9.]/g, ''); //remove all non-numbers
        const decimalCount = (value.match(/\./g) || []).length; //count number of decimals
        if (decimalCount > 2) { //if more than 1 decimal remove the decimal
            value = value.replace(/\.+$/, '');
        }
        if (value.indexOf('.') > -1) {
            const parts = value.split('.');
            if (parts[1] && parts[1].length > 2) {
                value = parts[0] + "." + parts[1].slice(0, 2);
            }
        }
        event.target.value = value;
    }
    //if (event.target && event.target.classList.contains('UnitDecimal')) {
    //    let value = event.target.value;
    //    value = value.replace(/[^0-9.]/g, ''); //remove all non-numbers
    //    const decimalCount = (value.match(/\./g) || []).length; //count number of decimals
    //    if (decimalCount > 2) { //if more than 1 decimal remove the decimal
    //        value = value.replace(/\.+$/, '');
    //    }
    //    if (value.indexOf('.') > -1) {
    //        const parts = value.split('.');
    //        if (parts[1] && parts[1].length > 2) {
    //            value = parts[0] + "." + parts[1].slice(0, 2);
    //        }
    //    }
    //    event.target.value = value;
    //}
    if (event.target && event.target.classList.contains('Positivedecimal')) {
        let value = event.target.value;
        value = value.replace(/[^0-9.]/g, '');
        const decimalCount = (value.match(/\./g) || []).length;
        if (decimalCount > 2) {
            value = value.replace(/\.+$/, '');
        }
        if (value.indexOf('.') > -1) {
            const parts = value.split('.');
            if (parts[1] && parts[1].length > 2) {
                value = parts[0] + "." + parts[1].slice(0, 2);
            }
        }
        event.target.value = value;
    }
});
