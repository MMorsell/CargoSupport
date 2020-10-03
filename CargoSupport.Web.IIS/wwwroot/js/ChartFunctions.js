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

function updateSingleRecord(newObject) {
    const result = getDataTableRowByInternalId(newObject["_Id"]);

    if (result[0] !== -1) {
        const object = result[0];
        const index = result[1];

        if (window.location.href.includes('Transport')) {
            //Transport view only cares about updates with these properties
            if (newObject["preRideInput"] !== undefined) {
                object.preRideAnnotation = newObject["preRideInput"];
            }
            else if (newObject["postRideInput"] !== undefined) {
                object.postRideAnnotation = newObject["postRideInput"];
            }
            else if (newObject["port_selectBox"] !== undefined) {
                object.portNumber = parseInt(newObject["port_selectBox"], 10);
            }
            else if (newObject["carNumber_selectBox"] !== undefined) {
                object.carNumber = newObject["carNumber_selectBox"];
            }
            else if (newObject["convert_loadingLevel_toSelectbox"] !== undefined) {
                object.loadingLevel = newObject["convert_loadingLevel_toSelectbox"];
            }
            else if (newObject["driver_select"] !== undefined) {
                object.driver.id = newObject["driver_select"];
            }
            else if (newObject["numberOfColdBoxes_input"] !== undefined) {
                object.numberOfColdBoxes = newObject["numberOfColdBoxes_input"];
            }
            else if (newObject["restPicking_input"] !== undefined) {
                object.restPlock = newObject["restPicking_input"];
            }
            else if (newObject["numberOfFrozenBoxes_input"] !== undefined) {
                object.numberOfFrozenBoxes = newObject["numberOfFrozenBoxes_input"];
            }
            else if (newObject["numberOfBreadBoxes_input"] !== undefined) {
                object.numberOfBreadBoxes = newObject["numberOfBreadBoxes_input"];
            }
            else if (newObject["controlIsDone_input"] !== undefined) {
                object.controlIsDone = newObject["controlIsDone_input"];
            }
        } else if (window.location.href.includes('Plock')) {
            //Plock view only cares about updates with these properties
            if (newObject["numberOfColdBoxes_input"] !== undefined) {
                object.numberOfColdBoxes.value = newObject["numberOfColdBoxes_input"];
            }
            else if (newObject["carNumber_selectBox"] !== undefined) {
                object.carNumber = newObject["carNumber_selectBox"];
            }
            else if (newObject["port_selectBox"] !== undefined) {
                object.portNumber = newObject["port_selectBox"];
            }
            else if (newObject["restPicking_input"] !== undefined) {
                object.restPicking.value = newObject["restPicking_input"];
            }
            else if (newObject["numberOfFrozenBoxes_input"] !== undefined) {
                object.numberOfFrozenBoxes.value = newObject["numberOfFrozenBoxes_input"];
            }
            else if (newObject["numberOfBreadBoxes_input"] !== undefined) {
                object.numberOfBreadBoxes.value = newObject["numberOfBreadBoxes_input"];
            }
            else if (newObject["controlIsDone_input"] !== undefined) {
                object.controlIsDone.value = newObject["controlIsDone_input"];
            }
            else if (newObject["convert_loadingLevel_toSelectbox"] !== undefined) {
                object.loadingLevel = newObject["convert_loadingLevel_toSelectbox"];
            }
        }
        else {
            //Medarbetare view only cares about updates with these properties
            if (newObject["preRideInput"] !== undefined) {
                object.preRideAnnotation = newObject["preRideInput"];
            }
            else if (newObject["port_selectBox"] !== undefined) {
                object.portNumber = newObject["port_selectBox"];
            }
            else if (newObject["carNumber_selectBox"] !== undefined) {
                object.carNumber = newObject["carNumber_selectBox"];
            }
            else if (newObject["convert_loadingLevel_toSelectbox"] !== undefined) {
                object.loadingLevel = newObject["convert_loadingLevel_toSelectbox"];
            }
            else if (newObject["driver_select"] !== undefined) {
                object.driver.fullName = newObject["driver_fullName"];
            }
            else if (newObject["numberOfBreadBoxes_input"] !== undefined) {
                object.numberOfBreadBoxes = newObject["numberOfBreadBoxes_input"];
            }
            else if (newObject["restPicking_input"] !== undefined) {
                object.restPlock = newObject["restPicking_input"];
            }
            else if (newObject["numberOfFrozenBoxes_input"] !== undefined) {
                object.numberOfFrozenBoxes = newObject["numberOfFrozenBoxes_input"];
            }
            else if (newObject["numberOfBreadBoxes"] !== undefined) {
                object.numberOfBreadBoxes = newObject["numberOfBreadBoxes"];
            }
            else if (newObject["numberOfColdBoxes_input"] !== undefined) {
                object.numberOfColdBoxes = newObject["numberOfColdBoxes_input"];
            }
            else if (newObject["controlIsDone_input"] !== undefined) {
                object.controlIsDone = newObject["controlIsDone_input"];
            }
        }

        table.row(index).invalidate();
        //table.row(index).remove().draw()
        //table.row.add(object).draw()
        //$('#dataTable').dataTable().fnUpdate(object, index, undefined, false);
        //$("tr:eq( " + (index + 1) + " )").removeClass("updateanimation");
        //setTimeout(function () { $("tr:eq( " + (index + 1) + " )").addClass("updateanimation"); }, 100);
        console.log('updated single record')
    }
}