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

function getDataTableIndexByRowInternalId(internalId) {
    let foundindex = -1;
    debugger;
    table.column(0)
        .data()
        .filter(function (value, index) {
            if (value === internalId) {
                foundindex = index;
            }
        });
    return table.rows().indexes()[foundindex];
}

function updateSingleRecord(newObject) {
    if (window.location.href.includes('Transport')) {
        var object = table.rows(getDataTableIndexByRowInternalId(newObject._Id)).data();
        debugger;

        //Transport view only cares about updates with these properties
        if (newObject.preRideAnnotation !== null) {
        }
        if (newObject.postRideAnnotation !== null) {
        }
        if (newObject.portNumber !== null) {
        }
        if (newObject.carNumber !== null) {
        }
        if (newObject.loadingLevel !== null) {
        }
        if (newObject.driver !== null) {
        }
        if (newObject.numberOfColdBoxes !== null) {
        }
        if (newObject.restPicking !== null) {
        }
        if (newObject.numberOfFrozenBoxes !== null) {
        }
        if (newObject.numberOfBreadBoxes !== null) {
        }
        if (newObject.controlIsDone !== null) {
        }
        console.log('updated single record')
    } else if (window.location.href.includes('Plock')) {
        var object = table.rows(getDataTableIndexByRowInternalId(newObject._Id)).data();

        //Plock view only cares about updates with these properties
        if (newObject.numberOfColdBoxes !== null) {
        }
        if (newObject.restPicking !== null) {
        }
        if (newObject.numberOfFrozenBoxes !== null) {
        }
        if (newObject.numberOfBreadBoxes !== null) {
        }
        if (newObject.controlIsDone !== null) {
        }
        debugger;
        console.log('updated single record')
    }
    else {
        var object = table.rows(getDataTableIndexByRowInternalId(newObject._Id)).data();

        //Medarbetare view only cares about updates with these properties
        if (newObject.preRideAnnotation !== null) {
        }
        if (newObject.portNumber !== null) {
        }
        if (newObject.carNumber !== null) {
        }
        if (newObject.loadingLevel !== null) {
        }
        if (newObject.driver !== null) {
        }
        if (newObject.numberOfColdBoxes !== null) {
        }
        if (newObject.restPicking !== null) {
        }
        if (newObject.numberOfFrozenBoxes !== null) {
        }
        if (newObject.numberOfBreadBoxes !== null) {
        }
        if (newObject.controlIsDone !== null) {
        }
        debugger;
        console.log('updated single record')
    }
}