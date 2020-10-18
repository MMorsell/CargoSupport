let tabs = [];
function deleteAllDatasetsOnChart(chartReference) {
    /*
     * Removes all old datasets
     */
    while (chartReference.data.datasets.length > 0) {
        chartReference.data.datasets.pop()
    }
}
function deleteAllLabelsOnChart(chartReference) {
    /*
     * Removes all old labels
     */
    while (chartReference.data.labels.length > 0) {
        chartReference.data.labels.pop()
    }
}

function updateChartReference(chartReference) {
    chartReference.update();
}

function getDataBetweenDates(apiUrl) {
    $.ajax({
        type: "GET",
        url: apiUrl,
        data: {
            "fromDate": function () { return fromDate.format(timeFormat) },
            "toDate": function () { return toDate.format(timeFormat) },
            "splitData": function () { return splitData() },
            "driverId": function () { return driverId() }
        },
        success: function (responseData) {
            reloadGraphs(responseData);
        }
    });
}
function getDataBetweenDatesAndReportingToString(apiUrl, sectionId) {
    $.ajax({
        type: "GET",
        url: apiUrl,
        data: {
            "fromDate": function () { return fromDate.format(timeFormat) },
            "toDate": function () { return toDate.format(timeFormat) },
            "splitData": function () { return splitData() },
            "sectionId": sectionId
        },
        success: function (responseData) {
            reloadDatatable(responseData);
        }
    });
}

function splitData() {
    if (document.getElementById('split-data') === null) {
        return false;
    }
    else {
        return document.getElementById('split-data').checked;
    }
}
function driverId() {
    if (Id === null) {
        return 0;
    }
    else {
        return Id;
    }
}

function getDataTableRowByInternalId(internalId) {
    for (i = 0; i < table.rows()[0].length; i++) {
        if (table.row(i).data()._Id == internalId) {
            return [table.row(i).data(), i];
        }
    }
    return -1;
}

function colorBasedOnLoadingValue(thisRef) {
    switch (thisRef.value) {
        case "Ej påbörjad": thisRef.style.backgroundColor = "red"; break;
        case "Återanvända": thisRef.style.backgroundColor = "orange"; break;
        case "Påbörjad": thisRef.style.backgroundColor = "yellow"; break;
        case "Klar": thisRef.style.backgroundColor = "lightgreen"; break;
    }

    updateRow(thisRef)
}

function hideEverythingButThisClass(className) {
    $(`#dataTable tr`).hide();
    $(`#dataTable tr.${className}`).show();
    $(`#dataTable thead tr`).show();
}

function generateTabs() {
    let firstTabHasBeenSelected = false;
    var tabDomRef = document.getElementById('dataTableTabs');
    $(tabDomRef).empty();
    if (tabs.includes("Morgon")) {
        let t = $(`<button class="tablinks" onclick="hideEverythingButThisClass('Morgon')">Morgon</button>`);
        $(tabDomRef).append(t);
    }

    if (tabs.includes("Kväll")) {
        let t = $(`<button class="tablinks" onclick="hideEverythingButThisClass('Kväll')">Kväll</button>`);
        $(tabDomRef).append(t);
    }

    if (tabs.includes("Hämtas")) {
        let t = $(`<button class="tablinks" onclick="hideEverythingButThisClass('Hämtas')">Hämtas</button>`);
        $(tabDomRef).append(t);
    }

    if (tabs.includes("Returer")) {
        let t = $(`<button class="tablinks" onclick="hideEverythingButThisClass('Returer')">Returer</button>`);
        $(tabDomRef).append(t);
    }

    for (var i = 0; i < tabs.length; i++) {
        if (tabs[i] !== "Morgon" &&
            tabs[i] !== "Kväll" &&
            tabs[i] !== "Hämtas" &&
            tabs[i] !== "Returer") {

            let t = $(`<button class="tablinks" onclick="hideEverythingButThisClass('${tabs[i]}')">${tabs[i]}</button>`);
            $(tabDomRef).append(t);
        }
    }
    let t = $(`<button class="tablinks" onclick="hideEverythingButThisClass('Alla')">Alla</button>`);
    $(tabDomRef).append(t);
    $(`#dataTableTabs button`).first().click()
}

function getGroupFromFirstColumn(thisRef) {
    if (thisRef.cells[0].innerText.match(afternoonRegex)) {
        if (!tabs.includes("Kväll")) {
            tabs.push("Kväll");
        }
        $(thisRef).addClass('Kväll');
    } else if (thisRef.cells[0].innerText.match(morningRegex)) {
        if (!tabs.includes("Morgon")) {
            tabs.push("Morgon");
        }
        $(thisRef).addClass('Morgon');
    } else if (thisRef.cells[0].innerText.match(hamtasRegex)) {
        if (!tabs.includes("Hämtas")) {
            tabs.push("Hämtas");
        }
        $(thisRef).addClass('Hämtas');
    } else if (thisRef.cells[0].innerText.match(returRegex)) {
        if (!tabs.includes("Returer")) {
            tabs.push("Returer");
        }
        $(thisRef).addClass('Returer');
    } else {
        if (!tabs.includes(thisRef.cells[0].innerText)) {
            tabs.push(thisRef.cells[0].innerText);
        }
        $(thisRef).addClass(thisRef.cells[0].innerText);
    }
    $(thisRef).addClass('Alla');
    $(thisRef).hide();
}