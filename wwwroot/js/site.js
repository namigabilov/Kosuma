let connection = null;

async function startConnection(group) {
    if (!connection) {
        // Create a new SignalR connection if none exists
        connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

        try {
            // Start the connection and wait for it to complete
            await connection.start();
            console.log("Connected to SignalR hub with ID:", connection.connectionId);

            // Save the connection ID in session storage for reuse
            sessionStorage.setItem('signalRConnectionId', connection.connectionId);

            // Join the specified group after the connection is started
            await connection.invoke("JoinVideo", group);

            setupReceiveMessageHandler();
        } catch (err) {
            console.error("SignalR Connection Error:", err.toString());
        }
    }
    return connection;
}

async function setupReceiveMessageHandler() {
    connection.on("ReceiveMessage", function (data) {
        const li = document.createElement("li");
        li.style.padding = "8px";
        li.style.borderBottom = "1px solid #ccc";

        const strong = document.createElement("strong");
        strong.textContent = data.user + " : ";
        li.appendChild(strong);

        const messageText = document.createTextNode(data.message);
        li.appendChild(messageText);

        const messageList = document.getElementById("messagesList");
        if (messageList) {
            messageList.appendChild(li);
            messageList.scrollTop = messageList.scrollHeight;
        }
    });
}


async function sendMessageToGroup(message, group) {
    const connectionInstance = await startConnection(group);
    const data = {
        Message: message,
        Second: 15,
        StreamId: group
    };
    return connectionInstance.invoke("SendMessage", data)
        .catch(err => console.error("Error sending message:", err.toString()));
}