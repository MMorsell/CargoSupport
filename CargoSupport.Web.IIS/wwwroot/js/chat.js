"use strict";

const connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.on("Upsert", function (upsertDirectory) {
  updateSingleRecord(upsertDirectory);
});

connection.on("ReloadDataTable", function () {
  reloadDatatableAjax();
});

connection
  .start()
  .then(function () {})
  .catch(function (err) {
    toggleConnectStatus("error");
    return console.error(err.toString());
  });
