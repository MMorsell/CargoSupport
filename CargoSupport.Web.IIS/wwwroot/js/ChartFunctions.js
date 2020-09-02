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
            "splitData": function () { return splitData() }
        },
        success: function (responseData) {
            reloadGraphs(responseData);
        }
    });
}

function getRandomColor() {
    var r = Math.floor(Math.random() * 255);
    var g = Math.floor(Math.random() * 255);
    var b = Math.floor(Math.random() * 255);
    return "rgb(" + r + "," + g + "," + b + ")";
};

function splitData() {
    if (document.getElementById('split-data') === null) {
        return false;
    }
    else {
        return document.getElementById('split-data').checked;
    }
}