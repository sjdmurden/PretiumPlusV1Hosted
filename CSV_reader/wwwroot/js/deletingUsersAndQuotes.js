/*document.addEventListener("DOMContentLoaded", function () {

    document.querySelectorAll(".delete-user-btn").forEach(button => {

        button.addEventListener("click", async function () {

            const row = this.closest("tr");
            const userEmail = row.dataset.userEmail;

            if (!confirm(`Delete user "${userEmail}"?`)) return;

            const response = await fetch("/Profile/DeleteUser", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({ userEmail })
            });

            if (response.ok) {
                row.remove(); // remove row from UI
            } else {
                alert("Failed to delete user.");
            }
        });
    });
});*/