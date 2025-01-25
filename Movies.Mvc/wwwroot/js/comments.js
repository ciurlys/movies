const commentButtons = document.querySelectorAll('a[data-bs-toggle="comment"]');
    commentButtons.forEach(function(button) {
        const movieId = button.getAttribute('data-id');
        
        fetch(`/Comment/GetUnreadCommentsCount?movieid=${movieId}`)
            .then(response => response.json())
            .then(unreadCount => {
                if (unreadCount > 0) {
                    button.classList.remove('btn-success');
                    button.classList.add('btn-warning');
                    button.textContent = `Comment (${unreadCount})`;
                }
            })
            .catch(error => {
                console.error("Error checking comments: ", error);
            });
    });
