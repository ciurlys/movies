"use strict";

var connection = new signalR.HubConnectionBuilder()
  .withUrl("/chat").build();

document.getElementById("sendButton").disabled = true;

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
    
    fetch('/Chat/GetChatHistory')
        .then(response => response.json())
        .then(messages => {
            messages.forEach(message => {
                var li = document.createElement('li');
                li.style.marginBottom = "10px";
                li.style.maxWidth = "400px";
                li.style.wordWrap = "break-word";
                li.style.overflow = "hidden";
                
                document.getElementById("messages").append(li);
                
                var spanTime = document.createElement("span");
                spanTime.textContent = `Sometime before - `;
                li.appendChild(spanTime);
        
                var spanAuthor = document.createElement("span");
                spanAuthor.textContent = `${message.from}`;
                spanAuthor.style.fontWeight = "bold";
                li.appendChild(spanAuthor);

                var spanMessage = document.createElement("span");
                spanMessage.textContent = `: ${message.message}`;
                li.appendChild(spanMessage);
            });
        }).catch(function (err) {
            return console.error(err.toString());
        });
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("ReceiveMessage", function (received) {
    var date = new Date(Date.now());
    const utcString = date.toUTCString();
    const time = utcString.slice(-11, -4);
    
    var li = document.createElement("li");
    li.style.marginBottom = "10px";
    li.style.maxWidth = "400px";
    li.style.wordWrap = "break-word";
    li.style.overflow = "hidden";
    document.getElementById("messages").prepend(li);
    
    var spanTime = document.createElement("span");
    spanTime.textContent = `${time} - `;
    li.appendChild(spanTime);
    
    var spanAuthor = document.createElement("span");
    spanAuthor.textContent = `${received.from}`;
    spanAuthor.style.fontWeight = "bold";
    li.appendChild(spanAuthor);

    var spanMessage = document.createElement("span");
    spanMessage.textContent = `: ${received.message}`;
    li.appendChild(spanMessage);
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    const inputMessage = document.getElementById("message");
    if (inputMessage.value.length > 400) {
        inputMessage.value = "";
        inputMessage.placeholder = "Max 400 characters";
        return;
    }
    var messagemodel = {
        from: document.getElementById("from").value,
        message: inputMessage.value
    };
    connection.invoke("SendMessage", messagemodel).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});
