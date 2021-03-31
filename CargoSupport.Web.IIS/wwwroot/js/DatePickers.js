const generateFlatPickrElement = (domIdToBindTo, maxDateAsDate, minDateAsDate, defaultDateAsDate, functionToExecuteOnChange) => {
  flatpickr(domIdToBindTo, {
    locale: "sv",
    maxDate: maxDateAsDate,
    minDate: minDateAsDate,
    defaultDate: defaultDateAsDate,
    onChange: functionToExecuteOnChange,
  });
};

/*
 * datepicker for index driver
 */
flatpickr("#calendar-to-driverIndex", {
  locale: "sv",
  defaultDate: new Date(),
  onChange: function (selectedDates, dateStr, instance) {
    toDate = moment(dateStr);
    reloadDatatableWithNoStatusAjax();
  },
});

/*
 * Datepicker for graph dashboard
 */
flatpickr("#calendar-from-graphs-dashboard", {
  locale: "sv",
  maxDate: new Date(),
  defaultDate: new Date(),
  onChange: function (selectedDates, dateStr, instance) {
    fromDate = moment(dateStr, timeFormat);
    getDataBetweenDates(mainApiEndpoint);
  },
});
/*
 * Datepicker to graph dashboard
 */
flatpickr("#calendar-to-graphs-dashboard", {
  locale: "sv",
  maxDate: new Date(),
  defaultDate: new Date(),
  onChange: function (selectedDates, dateStr, instance) {
    toDate = moment(dateStr, timeFormat);
    getDataBetweenDates(mainApiEndpoint);
  },
});

/*
 * Datepicker for detailed discrete driver
 */
flatpickr("#calendar-from-graphs-discrete", {
  locale: "sv",
  maxDate: new Date(),
  defaultDate: new Date().fp_incr(-7),
  onChange: function (selectedDates, dateStr, instance) {
    fromDate = moment(dateStr, timeFormat);
    getDataBetweenDates(mainApiEndpoint);
    redrawTableById(null, null);
  },
});
/*
 * Datepicker for detailed discrete driver
 */
flatpickr("#calendar-to-graphs-discrete", {
  locale: "sv",
  defaultDate: new Date(),
  onChange: function (selectedDates, dateStr, instance) {
    toDate = moment(dateStr, timeFormat);
    getDataBetweenDates(mainApiEndpoint);
    redrawTableById(null, null);
  },
});

/*
 * Datepicker for grouped data by boss
 */
flatpickr("#calendar-from-group-dashboard", {
  locale: "sv",
  maxDate: new Date(),
  defaultDate: new Date().fp_incr(-7),
  onChange: function (selectedDates, dateStr, instance) {
    fromDate = moment(dateStr, timeFormat);
    redrawTableById(null, null);
  },
});
/*
 * Datepicker for grouped data by boss
 */
flatpickr("#calendar-to-group-dashboard", {
  locale: "sv",
  maxDate: new Date(),
  defaultDate: new Date(),
  onChange: function (selectedDates, dateStr, instance) {
    toDate = moment(dateStr, timeFormat);
    redrawTableById(null, null);
  },
});

/*
 * Datepicker for update pin order
 */
flatpickr("#calendar-pin-order", {
  locale: "sv",
  defaultDate: new Date(),
  onChange: function (selectedDates, dateStr, instance) {},
});
