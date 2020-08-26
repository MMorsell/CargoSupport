const convert_loadingLevel_toSelectbox = function (data, type, full, meta) {
    var selectBox = [];
    selectBox.push(
        '<div class="selectpicker show-tick"><select class="dropdown bootstrap-select">'
    );
    debugger;
    for (i = 0; i < 4; i++) {
        if (i === 0) {
            if (data === 0) {
                selectBox.push('<option selected>Ej påbörjad</option>')
            }
            else {
                selectBox.push('<option>Ej påbörjad</option>')
            }
        }

        if (i === 1) {
            if (data === 1) {
                selectBox.push('<option selected>Återanvända</option>')
            }
            else {
                selectBox.push('<option>Återanvända</option>')
            }
        }

        if (i === 2) {
            if (data === 2) {
                selectBox.push('<option selected>Påbörjad</option>')
            }
            else {
                selectBox.push('<option>Påbörjad</option>')
            }
        }

        if (i === 3) {
            if (data === 3) {
                selectBox.push('<option selected>Klar</option>')
            }
            else {
                selectBox.push('<option>Klar</option>')
            }
        }
    }

    selectBox.push(
        '</select>'
    );

    return selectBox.join("");
}

const convert_loadingLevel_toValue = function (data, type, full, meta) {
    if (data === 0) {
        return '<p>Ej påbörjad</p>';
    }

    if (data === 1) {
        return '<p>Återanvända</p>';
    }

    if (data === 2) {
        return '<p>Påbörjad</p>';
    }

    if (data === 3) {
        return '<p>Klar</p>';
    }
}

const disabled_checkbox = function (data, type, full, meta) {
    var is_checked = data == true ? "checked" : "";
    return '<input type="checkbox" disabled class="checkbox" ' +
        is_checked + ' />';
}

const disabled_textInput = function (data, type, full, meta) {
    return '<input type="text" readonly class="form-control" value="' +
        data + '"  />';
}

const disabled_intInput = function (data, type, full, meta) {
    return '<input type="number" readonly class="form-control" value="' +
        data + '"  />';
}




/*
 * Datepicker for basic output table
 */
flatpickr('#calendar-from-table', {
    "locale": "sv",
    "maxDate": new Date().fp_incr(1),
    "minDate": new Date(),
    defaultDate: new Date(),
    onChange: function (selectedDates, dateStr, instance) {
        ajaxDate = new Date(dateStr);
        reloadDatatableAjax();

        //TODO Add post update
    }
});

/*
 * Transport datepicker
 */
flatpickr('#calendar-from-table-tr', {
    "locale": "sv",
    "maxDate": new Date().fp_incr(3),
    defaultDate: new Date(),
    onChange: function (selectedDates, dateStr, instance) {
        ajaxDate = new Date(dateStr);
        reloadDatatableAjax();

        //TODO Add post update
    }
});


function reloadDatatableAjax() {
    table.ajax.reload(null, false);
}