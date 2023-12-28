import ws from 'k6/ws';
import { check } from 'k6';
import http, { get } from "k6/http";

export default function () {

  let url = 'localhost:5001';

    var sendMessagex = {
    "arguments": ["sebastien","Hello world"],
    "target":"sendmessage",
    "type": 1
  };

  const socketUrl = `wss://${url}/test-chat`;

  const wsResponse = ws.connect(socketUrl, socket => {
    socket.on('open', () => {
      // ESSENTIAL
      // from: https://stackoverflow.com/a/76677753
      socket.send(JSON.stringify({ protocol: 'json', version: 1 }) + '\x1e')
    });

    socket.setInterval(function timeout() {
      socket.send(JSON.stringify(sendMessagex) + '\x1e');
    }, 1000);

    socket.on('message', (data) => {
      // data example: [{field1: "value 1"}]
      // const msg = JSON.parse(data) // backend returns objects as string

      console.log(data);
    });
  });

  check(wsResponse, {'Status is 101': (r)=> r && r.status === 101});
}