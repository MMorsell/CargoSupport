﻿AllColors = {};
AllColors.names = {
    blue: "#0000ff",
    black: "#000000",
    brown: "#a52a2a",
    cyan: "#00ffff",
    darkblue: "#00008b",
    darkcyan: "#008b8b",
    darkgrey: "#a9a9a9",
    darkgreen: "#006400",
    darkkhaki: "#bdb76b",
    darkmagenta: "#8b008b",
    darkolivegreen: "#556b2f",
    darkorange: "#ff8c00",
    darkorchid: "#9932cc",
    darkred: "#8b0000",
    darksalmon: "#e9967a",
    darkviolet: "#9400d3",
    fuchsia: "#ff00ff",
    gold: "#ffd700",
    green: "#008000",
    indigo: "#4b0082",
    khaki: "#f0e68c",
    lightblue: "#add8e6",
    lightcyan: "#e0ffff",
    lightgreen: "#90ee90",
    lightgrey: "#d3d3d3",
    lightpink: "#ffb6c1",
    lightyellow: "#ffffe0",
    lime: "#00ff00",
    magenta: "#ff00ff",
    maroon: "#800000",
    navy: "#000080",
    olive: "#808000",
    orange: "#ffa500",
    pink: "#ffc0cb",
    purple: "#800080",
    red: "#ff0000",
    silver: "#c0c0c0",
    white: "#ffffff",
    yellow: "#ffff00"
};
AllColors.hex = [
    "#0000ff",
    "#000000",
    "#a52a2a",
    "#00ffff",
    "#00008b",
    "#008b8b",
    "#a9a9a9",
    "#006400",
    "#bdb76b",
    "#8b008b",
    "#556b2f",
    "#ff8c00",
    "#9932cc",
    "#8b0000",
    "#e9967a",
    "#9400d3",
    "#ff00ff",
    "#ffd700",
    "#008000",
    "#4b0082",
    "#f0e68c",
    "#add8e6",
    "#e0ffff",
    "#90ee90",
    "#d3d3d3",
    "#ffb6c1",
    "#ffffe0",
    "#00ff00",
    "#ff00ff",
    "#800000",
    "#000080",
    "#808000",
    "#ffa500",
    "#ffc0cb",
    "#800080",
    "#ff0000",
    "#c0c0c0",
    "#ffffff",
    "#ffff00"
];
AllColors.rgba = [
    "rgba(0, 0, 255",
    "rgba(0, 0, 0",
    "rgba(165, 42, 42",
    "rgba(0, 255, 255",
    "rgba(0, 0, 139",
    "rgba(0, 139, 139",
    "rgba(169, 169, 169",
    "rgba(0, 100, 0",
    "rgba(189, 183, 107",
    "rgba(139, 0, 139",
    "rgba(85, 107, 47",
    "rgba(255, 140, 0",
    "rgba(153, 50, 204",
    "rgba(139, 0, 0",
    "rgba(233, 150, 122",
    "rgba(148, 0, 211",
    "rgba(255, 0, 255",
    "rgba(255, 215, 0",
    "rgba(0, 128, 0",
    "rgba(75, 0, 130",
    "rgba(240, 230, 140",
    "rgba(173, 216, 230",
    "rgba(224, 255, 255",
    "rgba(144, 238, 144",
    "rgba(211, 211, 211",
    "rgba(255, 182, 193",
    "rgba(255, 255, 224",
    "rgba(0, 255, 0",
    "rgba(255, 0, 255",
    "rgba(128, 0, 0",
    "rgba(0, 0, 128",
    "rgba(128, 128, 0",
    "rgba(255, 165, 0",
    "rgba(255, 192, 203",
    "rgba(128, 0, 128",
    "rgba(255, 0, 0",
    "rgba(192, 192, 192",
    "rgba(255, 255, 255",
    "rgba(255, 255, 0"
];
AllColors.rgb = [
    "rgb(0, 0, 255)",
    "rgb(0, 0, 0)",
    "rgb(165, 42, 42)",
    "rgb(0, 255, 255)",
    "rgb(0, 0, 139)",
    "rgb(0, 139, 139)",
    "rgb(169, 169, 169)",
    "rgb(0, 100, 0)",
    "rgb(189, 183, 107)",
    "rgb(139, 0, 139)",
    "rgb(85, 107, 47)",
    "rgb(255, 140, 0)",
    "rgb(153, 50, 204)",
    "rgb(139, 0, 0)",
    "rgb(233, 150, 122)",
    "rgb(148, 0, 211)",
    "rgb(255, 0, 255)",
    "rgb(255, 215, 0)",
    "rgb(0, 128, 0)",
    "rgb(75, 0, 130)",
    "rgb(240, 230, 140)",
    "rgb(173, 216, 230)",
    "rgb(224, 255, 255)",
    "rgb(144, 238, 144)",
    "rgb(211, 211, 211)",
    "rgb(255, 182, 193)",
    "rgb(255, 255, 224)",
    "rgb(0, 255, 0)",
    "rgb(255, 0, 255)",
    "rgb(128, 0, 0)",
    "rgb(0, 0, 128)",
    "rgb(128, 128, 0)",
    "rgb(255, 165, 0)",
    "rgb(255, 192, 203)",
    "rgb(128, 0, 128)",
    "rgb(255, 0, 0)",
    "rgb(192, 192, 192)",
    "rgb(255, 255, 255)",
    "rgb(255, 255, 0)"
];

function getrgbaColorWithOpacity(index, opacity) {
    return AllColors.rgba[index] + ", " + opacity + ")"
}
function getrgbColor(index) {
    return AllColors.rgb[index]
}