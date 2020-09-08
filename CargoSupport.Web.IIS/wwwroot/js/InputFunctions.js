const availableCars = ["101", "102", "103", "104", "105", "Egen", "Ej Satt"];
const availablePorts = [24, 26, 27, 29, 31];

let ajaxDate = new Date();
const timeFormat = 'YYYY-MM-DD';
let fromDate = moment().subtract(7, 'd');
let toDate = moment();
let Id = null;
function formatDate(d) {
    month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2)
        month = '0' + month;
    if (day.length < 2)
        day = '0' + day;

    return [year, month, day].join('-');
}

function toggleConnectStatus(status) {
    var loadingRef = document.getElementById('connectStatus');
    var errorRef = document.getElementById('errorSymbol');
    var onlineRef = document.getElementById('onlineSymbom');
    switch (status) {
        case 'ok':
            loadingRef.style.display = "none";
            errorRef.style.display = "none";
            onlineRef.style.display = "inline-block";
            break;
        case 'loading':
            loadingRef.style.display = "inline-block";
            errorRef.style.display = "none";
            onlineRef.style.display = "none";
            break;
        case 'error':
            loadingRef.style.display = "none";
            errorRef.style.display = "inline-block";
            onlineRef.style.display = "none";
            break;
    }
}

const preRideInput = function (data, type, full, meta) {
    return '<input contenteditable="true" type="text" id="preRideInput" onfocusout=updateRow(this) class="form-control" value="' +
        data + '"  />';
}
const postRideInput = function (data, type, full, meta) {
    return '<input contenteditable="true" type="text" id="postRideInput" onfocusout=updateRow(this) class="form-control" value="' +
        data + '"  />';
}

const convert_loadingLevel_toSelectbox = function (data, type, full, meta) {
    var selectBox = [];
    selectBox.push(
        '<div><select id="convert_loadingLevel_toSelectbox" onChange=updateRow(this) class="form-control">'
    );
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

const pinstart_render = function (data, type, full, meta) {
    return '<p>' + data + '-' + full.pinEndTimeString + '<br>' + full.kilos + ' Kilo</p> ';
}

const disabled_checkbox = function (data, type, full, meta) {
    var is_checked = data == true ? "checked" : "";
    return '<input type="checkbox" onclick="return false" class="checkbox" ' +
        is_checked + ' />';
}

const disabled_textInput = function (data, type, full, meta) {
    return '<input type="text" readonly style="background: white;" class="form-control" value="' +
        data + '"  />';
}

const disabled_intInput = function (data, type, full, meta) {
    return '<input type="number" readonly class="form-control" value="' +
        data + '"  />';
}

const renderEmptyIfZero = function (data, type, full, meta) {
    if (data === 0 || data === "0") {
        return '<p></p>'
    }
    else {
        return '<p>' + data + '</p>';
    }
}

const hidden_IntIfNull = function (data, type, full, meta) {
    if (data === 0) {
        return '<p>Ej ifyllt</p>';
    }
    else {
        return '<p>' + data + '</p>';
    }
}

function reloadDatatableAjax() {
    toggleConnectStatus('loading');
    console.log("reloading ajax...");
    table.ajax.reload(null, false);
}
function reloadDatatableWithNoStatusAjax() {
    console.log("reloading ajax...");
    table.ajax.reload(null, false);
}

const input_kilos = function (data, type, full, meta) {
    if (type === 'sort' || type === 'filter') {
        return data;
    }

    return '<p>' + data + ' Kilo</p>'
}

function percentageToColorHighIsGood(perc) {
    var r, g, b = 0;
    if (perc < 50) {
        r = 255;
        g = Math.round(5.1 * perc);
    }
    else {
        g = 255;
        r = Math.round(510 - 5.10 * perc);
    }
    var h = r * 0x10000 + g * 0x100 + b * 0x1;
    return '#' + ('000000' + h.toString(16)).slice(-6);
}

function percentageToColorHighIsBad(perc) {
    perc = 100 - perc;
    var r, g, b = 0;
    if (perc < 50) {
        r = 255;
        g = Math.round(5.1 * perc);
    }
    else {
        g = 255;
        r = Math.round(510 - 5.10 * perc);
    }
    var h = r * 0x10000 + g * 0x100 + b * 0x1;
    return '#' + ('000000' + h.toString(16)).slice(-6);
}