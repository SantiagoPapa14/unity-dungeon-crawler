import socket
import os
from threading import Thread
import json

class Client():
    def __init__(self, address, socket, index, server):
        self.address = address
        self.socket = socket
        self.index = index
        self.username = None
        self.thread = None
        self.server = server
    def disconnect(self):
        pass
    def receivePacket(self):
        packet = ""
        receiving = True
        while True:
            try: 
                buf = self.socket.recv(1) 
                c = buf.decode()
                if c == '{':
                    receiving = True
                elif c=='}':
                    receiving = False
                packet+=c
                if not receiving:
                    return json.loads(packet)
            except Exception as e:
                print("We lost connection with ", self.index, " due to ", e)
                self.disconnect()
                break
    def sendPacket(self, packet):
        if(packet["username"] != self.username):
            self.socket.sendall(json.dumps(packet).encode())
    def handler(self):
        while True:
            packet = self.receivePacket()
            if(self.username == None):
                self.username = packet["username"]
            self.server.analyzePacket(packet, self)
    def manage(self):
        self.thread = Thread(target=self.handler)
        self.thread.start()

class Server():
    def __init__(self, address, port):
        self.clients = []
        self.address = (address, port)
        self.socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM) 
        print("Server Created.")    
    def initialize(self):
        self.socket.bind(self.address)
        self.socket.listen()
        print("Initialized Server.") 
    def broadcast(self, packet):
        for client in self.clients:
            client.sendPacket(packet)
    def analyzePacket(self, packet, client):
        if packet["type"] == 1:
            self.broadcast(packet)
    def run(self):
        while True:
            connection,address = self.socket.accept()
            newClient = Client(address, connection, len(self.clients), self)
            self.clients.append(newClient)
            self.clients[-1].manage()
            print(newClient.address, end=" ")
            print("is now connected.")

server = Server("0.0.0.0", 1404)
server.initialize()
server.run()
