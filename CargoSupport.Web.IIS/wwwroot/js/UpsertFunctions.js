function updateSingleRecord(newObject) {
    const result = getDataTableRowByInternalId(newObject["_Id"]);

    if (result[0] !== -1) {
        const object = result[0];
        const index = result[1];
        let update = false;

        if (window.location.href.includes('Transport')) {
            //Transport view only cares about updates with these properties
            if (newObject["preRideInput"] !== undefined) {
                object.preRideAnnotation = newObject["preRideInput"];
                update = true;
            }
            else if (newObject["postRideInput"] !== undefined) {
                object.postRideAnnotation = newObject["postRideInput"];
                update = true;
            }
            else if (newObject["port_selectBox"] !== undefined) {
                object.portNumber = parseInt(newObject["port_selectBox"], 10);
                update = true;
            }
            else if (newObject["carNumber_selectBox"] !== undefined) {
                object.carNumber = newObject["carNumber_selectBox"];
                update = true;
            }
            else if (newObject["convert_keyStatus_toSelectbox"] !== undefined) {
                object.keyStatus = newObject["convert_keyStatus_toSelectbox"];
                update = true;
            }
            else if (newObject["convert_loadingLevel_toSelectbox"] !== undefined) {
                object.loadingLevel = newObject["convert_loadingLevel_toSelectbox"];

                if (newObject["convert_loadingLevel_toSelectbox"] === 2 &&
                    object.keyStatus === 0) {
                    object.keyStatus = 1;
                }
                update = true;
            }
            else if (newObject["driver_select"] !== undefined) {
                object.driver.id = newObject["driver_select"];
                update = true;
            }
            else if (newObject["numberOfColdBoxes_input"] !== undefined) {
                object.numberOfColdBoxes = newObject["numberOfColdBoxes_input"];
                update = true;
            }
            else if (newObject["restPicking_input"] !== undefined) {
                object.restPlock = newObject["restPicking_input"];
                update = true;
            }
            else if (newObject["numberOfFrozenBoxes_input"] !== undefined) {
                object.numberOfFrozenBoxes = newObject["numberOfFrozenBoxes_input"];
                update = true;
            }
            else if (newObject["numberOfBreadBoxes_input"] !== undefined) {
                object.numberOfBreadBoxes = newObject["numberOfBreadBoxes_input"];
                update = true;
            }
            else if (newObject["controlIsDone_input"] !== undefined) {
                object.controlIsDone = newObject["controlIsDone_input"];
                update = true;
            }
        } else if (window.location.href.includes('Plock') && !window.location.href.includes('Plockanalys')) {
            //Plock view only cares about updates with these properties
            if (newObject["numberOfColdBoxes_input"] !== undefined) {
                object.numberOfColdBoxes.value = newObject["numberOfColdBoxes_input"];
                update = true;
            }
            else if (newObject["carNumber_selectBox"] !== undefined) {
                object.carNumber = newObject["carNumber_selectBox"];
                update = true;
            }
            else if (newObject["port_selectBox"] !== undefined) {
                object.portNumber = newObject["port_selectBox"];
                update = true;
            }
            else if (newObject["restPicking_input"] !== undefined) {
                object.restPicking.value = newObject["restPicking_input"];
                update = true;
            }
            else if (newObject["location_input"] !== undefined) {
                object.locationStatus = newObject["location_input"];
                update = true;
            }
            else if (newObject["numberOfFrozenBoxes_input"] !== undefined) {
                object.numberOfFrozenBoxes.value = newObject["numberOfFrozenBoxes_input"];
                update = true;
            }
            else if (newObject["numberOfBreadBoxes_input"] !== undefined) {
                object.numberOfBreadBoxes.value = newObject["numberOfBreadBoxes_input"];
                update = true;
            }
            else if (newObject["controlIsDone_input"] !== undefined) {
                object.controlIsDone.value = newObject["controlIsDone_input"];
                update = true;
            }
            else if (newObject["convert_loadingLevel_toSelectbox"] !== undefined) {
                object.loadingLevel = newObject["convert_loadingLevel_toSelectbox"];
                update = true;
            }
        } else if (window.location.href.includes('Plockanalys')) {
            toggleConnectStatus('loading');
            table.ajax.reload(null, false);
        }
        else {
            //Medarbetare view only cares about updates with these properties
            if (newObject["preRideInput"] !== undefined) {
                object.preRideAnnotation = newObject["preRideInput"];
                update = true;
            }
            else if (newObject["port_selectBox"] !== undefined) {
                object.portNumber = newObject["port_selectBox"];
                update = true;
            }
            else if (newObject["carNumber_selectBox"] !== undefined) {
                object.carNumber = newObject["carNumber_selectBox"];
                update = true;
            }
            else if (newObject["convert_keyStatus_toSelectbox"] !== undefined) {
                object.keyStatus = newObject["convert_keyStatus_toSelectbox"];
                update = true;
            }
            else if (newObject["convert_loadingLevel_toSelectbox"] !== undefined) {
                object.loadingLevel = newObject["convert_loadingLevel_toSelectbox"];
                if (newObject["convert_loadingLevel_toSelectbox"] === 2 &&
                    object.keyStatus === 0) {
                    object.keyStatus = 1;
                }
                update = true;
            }
            else if (newObject["driver_select"] !== undefined) {
                object.driver.fullName = newObject["driver_fullName"];
                update = true;
            }
            else if (newObject["numberOfBreadBoxes_input"] !== undefined) {
                object.numberOfBreadBoxes = newObject["numberOfBreadBoxes_input"];
                update = true;
            }
            else if (newObject["restPicking_input"] !== undefined) {
                object.restPlock = newObject["restPicking_input"];
                update = true;
            }
            else if (newObject["numberOfFrozenBoxes_input"] !== undefined) {
                object.numberOfFrozenBoxes = newObject["numberOfFrozenBoxes_input"];
                update = true;
            }
            else if (newObject["numberOfBreadBoxes"] !== undefined) {
                object.numberOfBreadBoxes = newObject["numberOfBreadBoxes"];
                update = true;
            }
            else if (newObject["numberOfColdBoxes_input"] !== undefined) {
                object.numberOfColdBoxes = newObject["numberOfColdBoxes_input"];
                update = true;
            }
            else if (newObject["controlIsDone_input"] !== undefined) {
                object.controlIsDone = newObject["controlIsDone_input"];
                update = true;
            }
        }

        if (update) {
            table.row(index).invalidate();
            var row = table.row(index).node();
            $(row).removeClass('updateanimation');
            $(row).find('.selectpicker').selectpicker();
            setTimeout(function () { $(row).addClass('updateanimation'); }, 50);
        }
    }
}