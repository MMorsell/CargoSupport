const customerSatisfaction_render = function (data, type, full, meta) {
    let validObjects = [];

    for (i = 0; i < data.length; i++) {
        if (data[i].satisfactionNumber !== -1) {
            validObjects.push(data[i].satisfactionNumber);
        }
    }
    if (validObjects.length === 0) {
        return '<p></p>'
    }
    const totalValue = validObjects.reduce((a, b) => a + b, 0);
    return `<p>${(totalValue / validObjects.length).toFixed(2)}</p>`;
}

const customerTiming_render = function (data, type, full, meta) {
    let validObjects = [];

    for (i = 0; i < data.length; i++) {
        if (data[i].timingNumber !== -1) {
            validObjects.push(data[i].timingNumber);
        }
    }
    if (validObjects.length === 0) {
        return '<p></p>'
    }
    const totalValue = validObjects.reduce((a, b) => a + b, 0);
    return `<p>${(totalValue / validObjects.length).toFixed(2)}</p>`;
}

const customerDriver_render = function (data, type, full, meta) {
    let validObjects = [];

    for (i = 0; i < data.length; i++) {
        if (data[i].driverNumber !== -1) {
            validObjects.push(data[i].driverNumber);
        }
    }
    if (validObjects.length === 0) {
        return '<p></p>'
    }
    const totalValue = validObjects.reduce((a, b) => a + b, 0);
    return `<p>${(totalValue / validObjects.length).toFixed(2)}</p>`;
}

const customerProduce_render = function (data, type, full, meta) {
    let validObjects = [];

    for (i = 0; i < data.length; i++) {
        if (data[i].produceNumber !== -1) {
            validObjects.push(data[i].produceNumber);
        }
    }
    if (validObjects.length === 0) {
        return '<p></p>'
    }
    const totalValue = validObjects.reduce((a, b) => a + b, 0);
    return `<p>${(totalValue / validObjects.length).toFixed(2)}</p>`;
}