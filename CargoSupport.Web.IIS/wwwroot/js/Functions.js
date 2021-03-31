const generateFlatPickrElement = (domIdToBindTo, maxDateAsDate, minDateAsDate, defaultDateAsDate, functionToExecuteOnChange) => {
  flatpickr(domIdToBindTo, {
    locale: "sv",
    maxDate: maxDateAsDate,
    minDate: minDateAsDate,
    defaultDate: defaultDateAsDate,
    onChange: functionToExecuteOnChange,
  });
};
