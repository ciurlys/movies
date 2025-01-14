"use strict";

var connection = new signalR.HubConnectionBuilder()
  .withUrl("/chat").build();

document.getElementById("sendButton").disabled = true;

connection.start().then(function () {
  document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
  return console.error(err.toString());
});

connection.on("ReceiveMessage", function (received) {
  var li = document.createElement("li");
  document.getElementById("messages").appendChild(li);

  li.textContent =
    `${received.from}: ${received.message}`;
});

document.getElementById("sendButton").addEventListener("click",
  function (event) {
    var messagemodel = {
      from: document.getElementById("from").value,
      message: document.getElementById("message").value
    };
    connection.invoke("SendMessage", messagemodel).catch(function (err) {
      return console.error(err.toString());
    });
    event.preventDefault();
  });