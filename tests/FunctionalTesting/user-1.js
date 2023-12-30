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

export default function user1() {
    // Init vars

    const authApiUrl = getAuthApiBaseUrl(containerEnvName);
    const messengerUrl = getMessengerApiBaseUrl(localEnvName);

    // Login users

    const loginFirstUserPayload = {
        email: "user1@example.com",
        password: "Password1!"
    };

    const loginSecondUserPayload = {
        email: "user2@example.com",
        password: "Password1!"
    };

    let loginFirstUserResponse = http.post(`${authApiUrl}user/login`, JSON.stringify(loginFirstUserPayload), basicOptions);
    let loginSecondUserResponse = http.post(`${authApiUrl}user/login`, JSON.stringify(loginSecondUserPayload), basicOptions);

    // Get auth token

    let firstUserAuthToken = JSON.parse(loginFirstUserResponse.body).token;
    let secondUserAuthToken = JSON.parse(loginSecondUserResponse.body).token;
    let firstUserId = JSON.parse(loginFirstUserResponse.body).userId;
    let secondUserId = JSON.parse(loginSecondUserResponse.body).userId;

    // Send message

    const firstSocketUrl = `wss://${messengerUrl}/test-chat?access_token=${firstUserAuthToken}`;

    var message = {
        "arguments": [secondUserId, `Hello world send by first user ${firstUserId} to second user ${secondUserId}`],
        "target": "SendMessageToSpecificUser",
        "type": 1
    };

    const wsResponse = ws.connect(firstSocketUrl, socket => {
        socket.on('open', () => {
            // ESSENTIAL
            // from: https://stackoverflow.com/a/76677753
            socket.send(JSON.stringify({protocol: 'json', version: 1}) + '\x1e')
        });

        socket.setInterval(function timeout() {
            socket.send(JSON.stringify(message) + '\x1e');
        }, 1000);

        socket.on('message', (data) => {
            console.log(`Received message for user 1: ${data}`);
        });
    });
}
