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
    }
});

/*
 * vanilla datepicker from
 */
flatpickr('#calendar-from-vanilla', {
    "locale": "sv",
    defaultDate: new Date().fp_incr(-7),
    onChange: function (selectedDates, dateStr, instance) {
        fromDate = moment(dateStr);
        reloadDatatableAjax();
    }
});

/*
 * vanilla datepicker to
 */
flatpickr('#calendar-to-vanilla', {
    "locale": "sv",
    defaultDate: new Date(),
    onChange: function (selectedDates, dateStr, instance) {
        toDate = moment(dateStr);
        reloadDatatableAjax();
    }
});

/*
 * Datepicker for graph dashboard
 */
flatpickr('#calendar-from-graphs-dashboard', {
    "locale": "sv",
    "maxDate": new Date(),
    defaultDate: new Date(),
    onChange: function (selectedDates, dateStr, instance) {
        fromDate = moment(dateStr, timeFormat);
        getDataBetweenDates(mainApiEndpoint);
    }
});
/*
 * Datepicker to graph dashboard
 */
flatpickr('#calendar-to-graphs-dashboard', {
    "locale": "sv",
    "maxDate": new Date(),
    defaultDate: new Date(),
    onChange: function (selectedDates, dateStr, instance) {
        toDate = moment(dateStr, timeFormat);
        getDataBetweenDates(mainApiEndpoint);
    }
});

/*
 * Datepicker for grouped data by boss
 */
flatpickr('#calendar-from-group-dashboard', {
    "locale": "sv",
    "maxDate": new Date(),
    defaultDate: new Date().fp_incr(-7),
    onChange: function (selectedDates, dateStr, instance) {
        fromDate = moment(dateStr, timeFormat);
        redrawTableById(null, null);
    }
});
/*
 * Datepicker for grouped data by boss
 */
flatpickr('#calendar-to-group-dashboard', {
    "locale": "sv",
    "maxDate": new Date(),
    defaultDate: new Date(),
    onChange: function (selectedDates, dateStr, instance) {
        toDate = moment(dateStr, timeFormat);
        redrawTableById(null, null);
    }
});

/*
 * Datepicker for update pin order
 */
flatpickr('#calendar-pin-order', {
    "locale": "sv",
    "maxDate": new Date(),
    defaultDate: new Date(),
    onChange: function (selectedDates, dateStr, instance) {
        //toDate = moment(dateStr, timeFormat);
        //redrawTableById(null, null);
    }
});