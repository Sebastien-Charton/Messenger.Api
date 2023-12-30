import ws from 'k6/ws';
import {check} from 'k6';
import http from "k6/http";
import {randomString} from 'https://jslib.k6.io/k6-utils/1.2.0/index.js';
import {basicOptions, containerEnvName, getAuthApiBaseUrl, getMessengerApiBaseUrl, localEnvName} from "./global-config.js";

export default function () {

    var authApiUrl = getAuthApiBaseUrl(containerEnvName);

    // Create user and login
    let userName = `${randomString(10)}`
    let email = `${randomString(10)}@example.com`
    let password = "Password1!,";

    const createUserPayload = {
        email: email,
        password: password,
        userName: userName
    };

    let createUserResponse = http.post(`${authApiUrl}user/register`, JSON.stringify(createUserPayload), basicOptions);

    // Login user

    const loginUserPayload = {
        email: email,
        password: password
    };

    let loginUserResponse = http.post(`${authApiUrl}user/login`, JSON.stringify(loginUserPayload), basicOptions);

    let authToken = JSON.parse(loginUserResponse.body).token;

    console.log(authToken);
    // Send message
    let messengerUrl = getMessengerApiBaseUrl(localEnvName);

    var message = {
        "arguments": ["sebastien", "Hello world"],
        "target": "sendmessage",
        "type": 1
    };

    const socketUrl = `wss://${messengerUrl}/test-chat?access_token=${authToken}`;

    const wsResponse = ws.connect(socketUrl, socket => {
        socket.on('open', () => {
            // ESSENTIAL
            // from: https://stackoverflow.com/a/76677753
            socket.send(JSON.stringify({protocol: 'json', version: 1}) + '\x1e')
        });

        socket.setInterval(function timeout() {
            socket.send(JSON.stringify(message) + '\x1e');
        }, 1000);

        socket.on('message', (data) => {
            // data example: [{field1: "value 1"}]
            // const msg = JSON.parse(data) // backend returns objects as string

            console.log(data);
        });
    });

    check(wsResponse, {'Status is 101': (r) => r && r.status === 101});
}