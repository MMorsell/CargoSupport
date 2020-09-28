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
    const index = getDataTableIndexByRowInternalId(newObject["_Id"]);
    if (window.location.href.includes('Transport')) {
        var object = table.rows(index).data();
        debugger;

        //Transport view only cares about updates with these properties
        if (newObject["preRideInput"] !== undefined) {
            object[0].preRideAnnotation = newObject["preRideInput"];
        }
        if (newObject["postRideInput"] !== undefined) {
            object[0].postRideAnnotation = newObject["postRideInput"];
        }
        if (newObject["port_selectBox"] !== undefined) {
            object[0].portNumber = newObject["port_selectBox"];
        }
        if (newObject["carNumber_selectBox"] !== undefined) {
            object[0].carNumber = newObject["carNumber_selectBox"];
        }
        if (newObject["convert_loadingLevel_toSelectbox"] !== undefined) {
            object[0].loadingLevel = newObject["convert_loadingLevel_toSelectbox"];
        }
        if (newObject["driver_select"] !== undefined) {
            object[0].driver = newObject["driver_select"];
        }
        if (newObject["numberOfColdBoxes_input"] !== undefined) {
            object[0].numberOfColdBoxes = newObject["numberOfColdBoxes_input"];
        }
        if (newObject["restPicking_input"] !== undefined) {
            object[0].restPlock = newObject["restPicking_input"];
        }
        if (newObject["numberOfFrozenBoxes_input"] !== undefined) {
            object[0].numberOfFrozenBoxes = newObject["numberOfFrozenBoxes_input"];
        }
        if (newObject["numberOfBreadBoxes_input"] !== undefined) {
            object[0].numberOfBreadBoxes = newObject["numberOfBreadBoxes_input"];
        }
        if (newObject["controlIsDone_input"] !== undefined) {
            object[0].controlIsDone = newObject["controlIsDone_input"];
        }
        debugger;
        table.row(index).invalidate(object);
        console.log('updated single record')
    } else if (window.location.href.includes('Plock')) {
        var object = table.rows(index).data();

        //Plock view only cares about updates with these properties
        if (newObject["numberOfColdBoxes_input"] !== undefined) {
            object[0].numberOfColdBoxes = + newObject["numberOfColdBoxes_input"];
        }
        if (newObject["restPicking_input"] !== undefined) {
            object[0].restPlock = newObject["restPicking_input"];
        }
        if (newObject["numberOfFrozenBoxes_input"] !== undefined) {
            object[0].numberOfFrozenBoxes = newObject["numberOfFrozenBoxes_input"];
        }
        if (newObject["numberOfBreadBoxes_input"] !== undefined) {
            object[0].numberOfBreadBoxes = newObject["numberOfBreadBoxes_input"];
        }
        if (newObject["controlIsDone_input"] !== undefined) {
            object[0].controlIsDone = newObject["controlIsDone_input"];
        }
        debugger;
        table.row(index).invalidate(object);
        console.log('updated single record')
    }
    else {
        var object = table.rows(index).data();

        //Medarbetare view only cares about updates with these properties
        if (newObject["preRideInput"] !== undefined) {
            object[0].preRideAnnotation = newObject["preRideInput"];
        }
        if (newObject["port_selectBox"] !== undefined) {
            object[0].portNumber = newObject["port_selectBox"];
        }
        if (newObject["carNumber_selectBox"] !== undefined) {
            object[0].carNumber = newObject["carNumber_selectBox"];
        }
        if (newObject["convert_loadingLevel_toSelectbox"] !== undefined) {
            object[0].loadingLevel = newObject["convert_loadingLevel_toSelectbox"];
        }
        if (newObject["driver_select"] !== undefined) {
            object[0].driver = newObject["driver_select"];
        }
        if (newObject["numberOfBreadBoxes_input"] !== undefined) {
            object[0].numberOfBreadBoxes = newObject["numberOfBreadBoxes_input"];
        }
        if (newObject["restPicking_input"] !== undefined) {
            object[0].restPicking = newObject["restPicking_input"];
        }
        if (newObject["numberOfFrozenBoxes_input"] !== undefined) {
            object[0].numberOfFrozenBoxes = newObject["numberOfFrozenBoxes_input"];
        }
        if (newObject["numberOfBreadBoxes"] !== undefined) {
            object[0].numberOfBreadBoxes = newObject["numberOfBreadBoxes"];
        }
        if (newObject["numberOfColdBoxes_input"] !== undefined) {
            object[0].numberOfColdBoxes = newObject["numberOfColdBoxes_input"];
        }
        if (newObject["controlIsDone_input"] !== undefined) {
            object[0].controlIsDone = newObject["controlIsDone_input"];
        }
        debugger;
        table.row(index).invalidate(object);
        console.log('updated single record')
    }
}