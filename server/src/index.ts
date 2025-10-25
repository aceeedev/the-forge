import http from "http";
import express from "express";
import cors from "cors";
import { Server } from "colyseus";
import { WebSocketTransport } from "@colyseus/ws-transport";
import { MyRoom } from "./rooms/MyRoom";

// Custom server class to expose presence.
class CustomServer extends Server {
  getPresence() {
    return this.presence;
  }
}

// Create Express app.
const app = express();

// Enable CORS for development (allow all origins for local network testing)
app.use(cors({
  origin: true, // Allow any origin in development
  credentials: true
}));

app.use(express.json());

// Create HTTP server from Express app.
const httpServer = http.createServer(app);

// create the Colyseus game server
const gameServer = new CustomServer({
  transport: new WebSocketTransport({
    server: httpServer,
    pingInterval: 10000,
    pingMaxRetries: 3,
  }),
});

// Register rooms handler.
gameServer.define("hello", MyRoom);

// Start listening on port 2567 on all network interfaces.
httpServer.listen(2567, '0.0.0.0', () => {
  console.log("Colyseus server running on ws://0.0.0.0:2567");
  console.log("Access from your network at ws://<your-ip>:2567");
});
