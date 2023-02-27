import socket
import os
from threading import Thread
import json

#Declare Variables
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)  
clients = []
clientsAmmount = -1

registeredAccounts = []
registeredAccounts.append("Blue Saint")

#Declare Functions
def broadcastMsg(msg, index):
    global clients
    for client in clients:
        if client != clients[index]:
            client.sendall(msg)

def handleClient(conn, add, index):
    while True:
        buf = conn.recv(1024) 
        msg = buf.decode()
        try:
            jMsg = json.loads(msg)
            if jMsg["username"] in registeredAccounts:
                conn.sendall(buf) 
        finally:
            print("Received: ", msg, " from ", add, " [",index,"]")
            #broadcastMsg(buf, index)


#Preliminary Actions
sock.bind(('0.0.0.0', 1404))  
sock.listen()
print("Server is Listening...")

#Main Loop
while True:  
    connection,address = sock.accept() 
    print(address, " connected.")
    clientsAmmount+=1
    clients.append(connection)
    clientHandler = Thread(target=handleClient, args=(connection, address, clientsAmmount,))
    clientHandler.start()
    


	