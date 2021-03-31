let ajaxDate = new Date();
let fromDate = moment().subtract(7, "d");
let Id = null;
let toDate = moment();

function formatDate(d) {
  (month = "" + (d.getMonth() + 1)), (day = "" + d.getDate()), (year = d.getFullYear());

  if (month.length < 2) month = "0" + month;
  if (day.length < 2) day = "0" + day;

  return [year, month, day].join("-");
}

function toggleConnectStatus(status) {
  const loadingRef = document.getElementById("connectStatus");
  const errorRef = document.getElementById("errorSymbol");
  const onlineRef = document.getElementById("onlineSymbom");
  switch (status) {
    case "ok":
      loadingRef.style.display = "none";
      errorRef.style.display = "none";
      onlineRef.style.display = "inline-block";
      break;
    case "loading":
      loadingRef.style.display = "inline-block";
      errorRef.style.display = "none";
      onlineRef.style.display = "none";
      break;
    case "error":
      loadingRef.style.display = "none";
      errorRef.style.display = "inline-block";
      onlineRef.style.display = "none";
      break;
  }
}

const preRideInput = function (data, type, full, meta) {
  return '<input contenteditable="true" type="text" id="preRideInput" onfocusout=updateRow(this) class=" form-control" value="' + data + '"  />';
};

const preRideInput_fat = function (data, type, full, meta) {
  return '<textarea id="preRideInput" onfocusout=updateRow(this) style="background: white;" class=" form-control" rows="2" cols="50">' + data + "</textarea >";
};

const postRideInput = function (data, type, full, meta) {
  return '<input contenteditable="true" type="text" id="postRideInput" onfocusout=updateRow(this) class=" form-control" value="' + data + '"  />';
};

const postRideInput_fat = function (data, type, full, meta) {
  return '<textarea id="postRideInput" onfocusout=updateRow(this) style="background: white;" class=" form-control" rows="2" cols="50">' + data + "</textarea >";
};

const convert_keyStatus_toSelectbox = function (data, type, full, meta) {
  const selectBox = [];

  switch (data) {
    case 0:
      selectBox.push(
        `<div><select data-width="fit" ${bootstrapSelectDropdownStyle} data-style="btn-danger" id="convert_keyStatus_toSelectbox" onChange=colorBasedOnLoadingValue(this) class="form-control">`
      );
      break;
    case 1:
      selectBox.push(
        `<div><select data-width="fit" ${bootstrapSelectDropdownStyle} data-style="btn-warning" id="convert_keyStatus_toSelectbox" onChange=colorBasedOnLoadingValue(this) class="form-control">`
      );
      break;
    case 2:
      selectBox.push(
        `<div><select data-width="fit" ${bootstrapSelectDropdownStyle} data-style="btn-success" id="convert_keyStatus_toSelectbox" onChange=colorBasedOnLoadingValue(this) class="form-control">`
      );
      break;
    case 3:
      selectBox.push(
        `<div><select data-width="fit" ${bootstrapSelectDropdownStyle} data-style="btn-primary" id="convert_keyStatus_toSelectbox" onChange=colorBasedOnLoadingValue(this) class="form-control">`
      );
      break;
  }

  for (i = 0; i < 4; i++) {
    if (i === 0) {
      if (data === 0) {
        selectBox.push("<option selected>Ej påbörjad</option>");
      } else {
        selectBox.push("<option>Ej påbörjad</option>");
      }
    }

    if (i === 1) {
      if (data === 1) {
        selectBox.push("<option selected>Lastas</option>");
      } else {
        selectBox.push("<option>Lastas</option>");
      }
    }

    if (i === 2) {
      if (data === 2) {
        selectBox.push("<option selected>Kontor</option>");
      } else {
        selectBox.push("<option>Kontor</option>");
      }
    }

    if (i === 3) {
      if (data === 3) {
        selectBox.push("<option selected>Tomgång</option>");
      } else {
        selectBox.push("<option>Tomgång</option>");
      }
    }
  }

  selectBox.push("</select>");

  return selectBox.join("");
};

const convert_keyStatus_toValue = function (data, type, full, meta) {
  if (data === 0) {
    return '<p style="background-color: red;" class="">Ej påbörjad</p>';
  }

  if (data === 1) {
    return '<p style="background-color: yellow;" class="">Lastas</p>';
  }

  if (data === 2) {
    return '<p style="background-color: lightgreen;"class="">Kontor</p>';
  }

  if (data === 3) {
    return '<p style="background-color: blue; color: white;" class="">Tomgång</p>';
  }
};
const convert_loadingLevel_toSelectbox = function (data, type, full, meta) {
  const selectBox = [];

  switch (data) {
    case 0:
      selectBox.push(
        `<div><select data-width="fit" ${bootstrapSelectDropdownStyle} data-style="btn-danger" id="convert_loadingLevel_toSelectbox" onChange=colorBasedOnLoadingValue(this) class="form-control">`
      );
      break;
    case 1:
      selectBox.push(
        `<div><select data-width="fit" ${bootstrapSelectDropdownStyle} data-style="btn-warning" id="convert_loadingLevel_toSelectbox" onChange=colorBasedOnLoadingValue(this) class="form-control">`
      );
      break;
    case 2:
      selectBox.push(
        `<div><select data-width="fit" ${bootstrapSelectDropdownStyle} data-style="btn-warning" id="convert_loadingLevel_toSelectbox" onChange=colorBasedOnLoadingValue(this) class="form-control">`
      );
      break;
    case 3:
      selectBox.push(
        `<div><select data-width="fit" ${bootstrapSelectDropdownStyle} data-style="btn-success" id="convert_loadingLevel_toSelectbox" onChange=colorBasedOnLoadingValue(this) class="form-control">`
      );
      break;
  }

  for (i = 0; i < 4; i++) {
    if (i === 0) {
      if (data === 0) {
        selectBox.push("<option selected>Ej påbörjad</option>");
      } else {
        selectBox.push("<option>Ej påbörjad</option>");
      }
    }

    if (i === 1) {
      if (data === 1) {
        selectBox.push("<option selected>Återanvända</option>");
      } else {
        selectBox.push("<option>Återanvända</option>");
      }
    }

    if (i === 2) {
      if (data === 2) {
        selectBox.push("<option selected>Påbörjad</option>");
      } else {
        selectBox.push("<option>Påbörjad</option>");
      }
    }

    if (i === 3) {
      if (data === 3) {
        selectBox.push("<option selected>Klar</option>");
      } else {
        selectBox.push("<option>Klar</option>");
      }
    }
  }

  selectBox.push("</select>");

  return selectBox.join("");
};

const convert_loadingLevel_toValue = function (data, type, full, meta) {
  if (data === 0) {
    return '<p style="background-color: red;" class="">Ej påbörjad</p>';
  }

  if (data === 1) {
    return '<p style="background-color: orange;" class="">Återanvända</p>';
  }

  if (data === 2) {
    return '<p style="background-color: yellow;"class="">Påbörjad</p>';
  }

  if (data === 3) {
    return '<p style="background-color: lightgreen;" class="">Klar</p>';
  }
};

const pinstart_render = function (data, type, full, meta) {
  const pinStartTime = new Date(full.pinStartTime);
  const pinEndTime = new Date(full.pinEndTime);
  return `<p>${pinStartTime.toLocaleTimeString("sv-SE", { hour: "2-digit", minute: "2-digit" })}-${pinEndTime.toLocaleTimeString("sv-SE", { hour: "2-digit", minute: "2-digit" })}
        <br>(${full.kilos} Kilo)
        <br>${full.kilos + full.numberOfColdBoxes * 2.5} Kilo</p>`;
};

const disabled_checkbox = function (data, type, full, meta) {
  const is_checked = data == true ? "checked" : "";

  return `<div class="pretty p-icon p-round p-pulse p-jelly p-bigger p-locked">
                    <input type="checkbox" ${is_checked} />
                    <div class="state p-success">
                        <i class="icon mdi mdi-check"></i>
                        <label></label>
                    </div>
                </div>`;
};

const disabled_textInput = function (data, type, full, meta) {
  return '<input type="text" readonly style="background: white;" class="form-control" value="' + data + '"  />';
};
const disabled_fat_textInput = function (data, type, full, meta) {
  return '<textarea readonly style="background: white;" class="form-control" rows="5" cols="50">' + data + "</textarea >";
};

const disabled_intInput = function (data, type, full, meta) {
  return '<input type="number" readonly class="form-control" value="' + data + '"  />';
};

const renderEjValdIfZeroOrEmpty = function (data, type, full, meta) {
  if (data === 0 || data === "0" || data === "" || data === "Ej satt") {
    return "<p>Ej vald</p>";
  } else {
    return "<p>" + data + "</p>";
  }
};

const renderEmptyIfZeroOrEmpty = function (data, type, full, meta) {
  if (data === 0 || data === "0" || data === "" || data === "Ej satt") {
    return "<p></p>";
  } else {
    return "<p>" + data + "</p>";
  }
};

const hidden_IntIfNull = function (data, type, full, meta) {
  if (data === 0) {
    return '<p class="" ></p>';
  } else {
    return '<p class="">' + data + "</p>";
  }
};

const ok_IfNotNull = function (data, type, full, meta) {
  if (data === 0) {
    return "<p></p>";
  } else {
    return "<p>Klart</p>";
  }
};

function reloadDatatableAjax() {
  toggleConnectStatus("loading");
  console.log("reloading ajax...");
  table.ajax.reload(null, false);
}
function reloadDatatableWithNoStatusAjax() {
  console.log("reloading ajax...");
  table.ajax.reload(null, false);
}

const input_kilos = function (data, type, full, meta) {
  if (type === "sort" || type === "filter") {
    return data;
  }

  return "<p>" + data + " Kilo</p>";
};

function percentageToColorHighIsGood(perc) {
  let r,
    g,
    b = 0;
  if (perc < 50) {
    r = 255;
    g = Math.round(5.1 * perc);
  } else {
    g = 255;
    r = Math.round(510 - 5.1 * perc);
  }
  let h = r * 0x10000 + g * 0x100 + b * 0x1;
  return "#" + ("000000" + h.toString(16)).slice(-6);
}

function percentageToColorHighIsBad(perc) {
  perc = 100 - perc;
  let r,
    g,
    b = 0;
  if (perc < 50) {
    r = 255;
    g = Math.round(5.1 * perc);
  } else {
    g = 255;
    r = Math.round(510 - 5.1 * perc);
  }
  let h = r * 0x10000 + g * 0x100 + b * 0x1;
  return "#" + ("000000" + h.toString(16)).slice(-6);
}

function fnExcelReport(tableId) {
  let tab_text = "<table border='2px'><tr bgcolor='#87AFC6'>";
  let j = 0;
  tab = document.getElementById(tableId); // id of table

  for (j = 0; j < tab.rows.length; j++) {
    if ($(tab.rows[j]).is(":visible")) {
      tab_text = tab_text + tab.rows[j].innerHTML + "</tr>";
    }
  }

  let $dataWrapper = $("<div>");
  $dataWrapper.html(tab_text);

  //Remove all line break
  $dataWrapper.find("br").each(function () {
    $(this).detach();
  });

  //Replace select with value of select
  $dataWrapper.find("select").each(function () {
    const innerValue = $("this option:selected").text();
    const convertedHtml = "<p>" + innerValue + "</p>";
    $(this).after(convertedHtml);
    $(this).detach();
  });

  //Replace textarea with value
  $dataWrapper.find("textarea").each(function () {
    const innerValue = $(this).text();
    const convertedHtml = "<p>" + innerValue + "</p>";
    $(this).after(convertedHtml);
    $(this).detach();
  });

  //Replace boolean with ja or empty
  $dataWrapper.find(":checkbox").each(function () {
    if ($(this).is(":checked")) {
      const innerValue = "Ja";
      const convertedHtml = "<p>" + innerValue + "</p>";
      $(this).after(convertedHtml);
    }
    $(this).detach();
  });

  tab_text = $dataWrapper[0].innerHTML;

  tab_text = tab_text + "</table>";
  tab_text = tab_text.replace(/<A[^>]*>|<\/A>/g, ""); //remove if u want links in your table
  tab_text = tab_text.replace(/<img[^>]*>/gi, ""); // remove if u want images in your table
  tab_text = tab_text.replace(/<input[^>]*>|<\/input>/gi, ""); // reomves input params

  const ua = window.navigator.userAgent;
  const msie = ua.indexOf("MSIE ");

  if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./)) {
    // If Internet Explorer
    txtArea1.document.open("txt/html", "replace");
    txtArea1.document.write(tab_text);
    txtArea1.document.close();
    txtArea1.focus();
    sa = txtArea1.document.execCommand("SaveAs", true, "Say Thanks to Sumit.xls");
  } //other browser not tested on IE 11
  else sa = window.open("data:application/vnd.ms-excel," + escape(tab_text));

  return sa;
}

$("#downloadPdf").click(function (event) {
  // get size of report page
  const reportPageHeight = $("#reportPage").innerHeight() + 500;
  const reportPageWidth = $("#reportPage").innerWidth() + 500;

  // create a new canvas object that we will populate with all other canvas objects
  const pdfCanvas = $("<canvas />").attr({
    id: "canvaspdf",
    width: reportPageWidth,
    height: reportPageHeight,
  });

  // keep track canvas position
  const pdfctx = $(pdfCanvas)[0].getContext("2d");
  const pdfctxX = 0;
  const pdfctxY = 0;
  const buffer = 100;

  // for each chart.js chart
  debugger;
  $("canvas").each(function (index) {
    // get the chart height/width
    const canvasHeight = $(this).innerHeight();
    const canvasWidth = $(this).innerWidth();

    // draw the chart into the new canvas
    //if (index % 2 === 0) {
    //    pdfctx.drawImage($(this)[0], 1083, 338, canvasWidth, canvasHeight);
    //    pdfctxX += canvasWidth + buffer;
    //}
    //else {
    pdfctx.drawImage($(this)[0], pdfctxX, pdfctxY, canvasWidth, canvasHeight);

    //}

    // our report page is in a grid pattern so replicate that in the new canvas
    if (index % 2 === 1) {
      //pdfctxX = 0;
      pdfctxY += canvasHeight + buffer;
    } else {
      pdfctxX += canvasWidth + buffer;
    }
  });
  let from = document.getElementById("calendar-from-graphs-discrete").value;
  let to = document.getElementById("calendar-to-graphs-discrete").value;

  let oldValue = document.getElementById("mainName").innerText;

  document.getElementById("mainName").innerText = oldValue + ", " + from + " till " + to;
  // create new pdf and add our new canvas as an image
  const doc = new jsPDF("l", "pt", [reportPageWidth, reportPageHeight]);
  const elementHTML = $("#reportPage").html();
  const specialElementHandlers = {
    "#elementH": function (element, renderer) {
      return true;
    },
  };
  doc.fromHTML(elementHTML, 15, 15, {
    width: 1000,
    elementHandlers: specialElementHandlers,
  });

  doc.addImage($(pdfCanvas)[0], "PNG", 0, reportPageHeight - pdfctxY - 200);
  // Save the PDF
  doc.save(`${fileName}.pdf`);

  document.getElementById("mainName").innerText = oldValue;
});
