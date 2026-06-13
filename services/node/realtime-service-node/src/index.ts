import http from "node:http";

const server = http.createServer((_request, response) => {
  response.writeHead(200, { "content-type": "application/json" });
  response.end(JSON.stringify({ status: "ok" }));
});

server.listen(3000, () => {
  console.log("SaptariX realtime service listening on 3000");
});
