document.addEventListener("DOMContentLoaded", function () {
  const voteConnection = new signalR.HubConnectionBuilder()
    .withUrl("/voteDateHub")
    .withAutomaticReconnect()
    .build();

  voteConnection.on("ReceiveVoteUpdate", function (updatedVotes) {
    var voteElement = document.querySelector(
      `[data-date-dateid="${updatedVotes.dateId}"] .vote-count`,
    );

    var listElement = document.querySelector(
      `.concrete-date[data-date-dateid="${updatedVotes.dateId}"]`,
    );
    listElement.dataset.votes = updatedVotes.votes;

      voteElement.textContent = `${updatedVotes.votes} votes.`;

    var allVoteElements = Array.from(
      document.querySelectorAll(".concrete-date"),
    ).sort((aEl, bEl) => {
      let voteComparison = bEl.dataset.votes - aEl.dataset.votes;
      if (voteComparison == 0) return aEl.dataset.day - bEl.dataset.day;
      return voteComparison;
    });

    var ul = document.querySelector(".date-list");
    ul.innerHTML = "";
    allVoteElements.forEach((el) => ul.appendChild(el));
  });

  document.querySelectorAll(".vote-button").forEach((button) => {
    const dateId = button.dataset.dateId;

    button.addEventListener("click", function () {
	if (button.classList.contains('btn-success')) {
	    button.classList.remove('btn-success');
	    button.classList.add('btn-danger');
	    button.textContent = 'Revoke';
	} else {
	    button.classList.remove('btn-danger');
	    button.classList.add('btn-success');
	    button.textContent = 'Vote'; 
	}

      voteConnection
        .invoke("SendVote", { DateId: parseInt(dateId) })
        .catch((err) => {
          console.error(err);
        });
    });
  });

  voteConnection
    .start()
    .then(() => console.log("SignalR Connected"))
    .catch((err) => console.error(err));

  const connection = new signalR.HubConnectionBuilder()
    .withUrl("/proposeDateHub")
    .withAutomaticReconnect()
    .build();

  connection.start();

  connection.on("ReceiveProposedDate", function (received) {
    var time = new Date(received.proposedDate);
    var day = time.getDate();

      var formattedDate = formatDate(time);

      var li = document.createElement("li");
      li.style.marginBottom = "16px";
    li.dataset.day = day;
    li.dataset.votes = 0;
    li.className = "concrete-date";
    li.dataset.dateDateid = received.dateId;
          
    var firstSpan = document.createElement("span");
    firstSpan.textContent = formattedDate + " has ";

    var secondSpan = document.createElement("span");
    secondSpan.className = "vote-count fw-bold";
    secondSpan.textContent = "0 votes.";

    var button = document.createElement("button");
    button.className = "vote-button btn btn-success";
    button.dataset.dateId = received.dateId;
    button.textContent = "Vote";
      
    button.addEventListener("click", function () {

	if (button.classList.contains('btn-success')) {
	    button.classList.remove('btn-success');
	    button.classList.add('btn-danger');
	    button.textContent = 'Revoke';
	} else {
	    button.classList.remove('btn-danger');
	    button.classList.add('btn-success');
	    button.textContent = 'Vote'; 
	}
	
      voteConnection
        .invoke("SendVote", { DateId: parseInt(received.dateId) })
        .catch((err) => {
          console.error(err);
        });
    });

    li.appendChild(firstSpan);
    li.appendChild(secondSpan);
    li.appendChild(button);

    var ul = document.querySelector(".date-list");
    ul.prepend(li);

    var allVoteElements = Array.from(
      document.querySelectorAll(".concrete-date"),
    ).sort((aEl, bEl) => {
      let voteComparison = bEl.dataset.votes - aEl.dataset.votes;
      if (voteComparison == 0) return aEl.dataset.day - bEl.dataset.day;
      return voteComparison;
    });

    ul.innerHTML = "";
    allVoteElements.forEach((el) => ul.appendChild(el));
  });

  document
    .getElementById("sendButton")
    .addEventListener("click", function (event) {
      const inputDate = document.getElementById("date").value;

      connection
        .invoke("ProposeDate", inputDate)
        .catch((err) => console.error(err.toString()));

      event.preventDefault();
    });
});

function formatDate(date) {
  const year = date.getFullYear();
  const month = (date.getMonth() + 1).toString().padStart(2, '0');
  const day = date.getDate().toString().padStart(2, '0');
  const hour = date.getHours().toString().padStart(2, '0');
  const minute = date.getMinutes().toString().padStart(2, '0');
  
  return `${year}/${month}/${day} ${hour}:${minute}`;
}
