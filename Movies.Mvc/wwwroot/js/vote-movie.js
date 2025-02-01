document.addEventListener("DOMContentLoaded", function () {
  const connection = new signalR.HubConnectionBuilder()
    .withUrl("/voteMovieHub")
    .withAutomaticReconnect()
    .build();

  connection.on("ReceiveMovieVoteUpdate", function (updatedMovieVotes) {
    var voteElement = document.querySelector(
      `[data-movie-id="${updatedMovieVotes.movieId}"] .movie-vote-count`,
    );

    var listElement = document.querySelector(
      `.concrete-movie[data-movie-id="${updatedMovieVotes.movieId}"`,
    );
    listElement.dataset.votes = updatedMovieVotes.votes;

    if (voteElement) {
      voteElement.textContent = `${updatedMovieVotes.votes} votes.`;
    }

    var allVoteElements = Array.from(
      document.querySelectorAll(".concrete-movie"),
    ).sort((aEl, bEl) => {
      let voteComparison = bEl.dataset.votes - aEl.dataset.votes;

      if (voteComparison == 0) return aEl.dataset.day - bEl.dataset.day;

      return voteComparison;
    });

    var ul = document.querySelector(".movie-list");
    ul.innerHTML = "";
    allVoteElements.forEach((el) => ul.appendChild(el));
  });

  document.querySelectorAll(".movie-vote-button").forEach((button) => {
    const movieId = button.dataset.movieId;

    button.addEventListener("click", function () {
      button.style.color == "red"
        ? (button.style.color = "green")
        : (button.style.color = "red");

      connection
        .invoke("SendMovieVote", {
          MovieId: parseInt(movieId),
        })
        .catch((err) => {
          console.error(err);
        });
    });
  });
  connection
    .start()
    .then(() => console.log("SignalR Connected"))
    .catch((err) => console.error(err));
});
