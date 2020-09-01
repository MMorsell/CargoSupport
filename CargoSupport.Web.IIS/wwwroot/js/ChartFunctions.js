function deleteAllDatasetsOnChart(chartReference) {
    /*
     * Removes all old datasets
     */
    while (chartReference.data.datasets.length > 0) {
        chartReference.data.datasets.pop()
    }
}

function updateChartReference(chartReference) {
    chartReference.update();
}