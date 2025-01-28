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

    if (voteElement) {
      voteElement.textContent = `${updatedVotes.votes} votes.`;
    }

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

    UpdateVoteState(button, dateId);

    button.addEventListener("click", function () {
      button.style.color = button.style.color === "red" ? "green" : "red";

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

    var formattedDate = time.toLocaleString("en-US", {
      year: "numeric",
      month: "numeric",
      day: "numeric",
      hour: "numeric",
      minute: "numeric",
      second: "numeric",
      hour12: true,
    });

    var li = document.createElement("li");
    li.dataset.day = day;
    li.dataset.votes = 0;
    li.className = "concrete-date";
    li.dataset.dateDateid = received.dateId;

    var firstSpan = document.createElement("span");
    firstSpan.textContent = formattedDate + " has ";

    var secondSpan = document.createElement("span");
    secondSpan.className = "vote-count";
    secondSpan.textContent = "0 votes.";

    var button = document.createElement("button");
    button.className = "vote-button";
    button.dataset.dateId = received.dateId;
    button.textContent = "Vote";

    UpdateVoteState(button, received.dateId);

    button.addEventListener("click", function () {
      button.style.color = button.style.color === "red" ? "green" : "red";

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

// Check what button state we should print. 1 - has voted, else - has notes
function UpdateVoteState(button, dateId) {
  fetch(`/Vote/CheckUserVoteDate?votedateid=${dateId}`)
    .then((response) => response.json())
    .then((returnValue) => {
      if (returnValue > 0) {
        button.style.color = "red";
      } else {
        button.style.color = "green";
      }
    })
    .catch(() => console.error("Error encountered trying to fetch vote state"));
}
