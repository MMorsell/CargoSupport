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