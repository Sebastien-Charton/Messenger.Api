import ws from 'k6/ws';
import {check} from 'k6';
import http from "k6/http";
import {
    basicOptions,
    containerEnvName,
    getAuthApiBaseUrl,
    getMessengerApiBaseUrl,
    localEnvName,
    generateEmail,
    generatePassword,
    generateUserName
} from "./global-config.js";

export default function user2 () {
    // Init vars

    const authApiUrl = getAuthApiBaseUrl(containerEnvName);
    const messengerUrl = getMessengerApiBaseUrl(localEnvName);

    // Login users

    const loginSecondUserPayload = {
        email: "user2@example.com",
        password: "Password1!"
    };

    let loginSecondUserResponse = http.post(`${authApiUrl}user/login`, JSON.stringify(loginSecondUserPayload), basicOptions);

    // Get auth token

    let secondUserAuthToken = JSON.parse(loginSecondUserResponse.body).token;
    let secondUserId = JSON.parse(loginSecondUserResponse.body).userId;

    // Send message

    const secondSocketUrl = `wss://${messengerUrl}/test-chat?access_token=${secondUserAuthToken}`;

    const wsResponse = ws.connect(secondSocketUrl, socket => {
        socket.on('open', () => {
            // ESSENTIAL
            // from: https://stackoverflow.com/a/76677753
            socket.send(JSON.stringify({protocol: 'json', version: 1}) + '\x1e')
        });

        socket.on('message', (data) => {
            console.log(`Received message for user 2: ${data}`);
        });
    });
}