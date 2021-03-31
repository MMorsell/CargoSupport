/**
 * Generates a FlatPickr element and binds it to the given @param domIdToBindTo.
 *
 * @param {string}    domIdToBindTo                 The id of the dom element to bind to including '#' in the beginning.
 * @param {Date}      maxDateAsDate                 Max date the user can select.
 * @param {Date}      minDateAsDate                 Min date the user can select.
 * @param {Date}      defaultDateAsDate             The default date.
 * @param {function}  functionToExecuteOnChange     Function to run on the onChange event.
 */
const generateFlatPickrElement = (domIdToBindTo, maxDateAsDate, minDateAsDate, defaultDateAsDate, functionToExecuteOnChange) => {
  flatpickr(domIdToBindTo, {
    locale: "sv",
    maxDate: maxDateAsDate,
    minDate: minDateAsDate,
    defaultDate: defaultDateAsDate,
    onChange: functionToExecuteOnChange,
  });
};
