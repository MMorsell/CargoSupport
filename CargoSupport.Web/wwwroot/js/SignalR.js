"use strict";

//Connection configuration
var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Starts the connection against the url
connection.start().catch(function (err) {
    return console.error(err.toString());
});

//Listens on Update call from server
connection.on("Update", function () {
    document.getElementById("updateButton").click();
});

//Function to send update to the server
function UpdateMessage() {
    connection.invoke("SendUpdate").catch(function (err) {
        return console.error(err.toString());
    })
};