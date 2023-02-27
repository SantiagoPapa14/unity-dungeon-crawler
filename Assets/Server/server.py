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
registeredAccounts.append("Red Monk")

#Declare Functions
def broadcastMsg(msg, index):
    global clients
    for client in clients:
        client.sendall(msg)

def handleClient(conn, add, index):
    while True:
        buf = conn.recv(1024) 
        msg = buf.decode()
        try:
            jMsg = json.loads(msg)
            if jMsg["username"] in registeredAccounts:
                conn.sendall(buf) 
        except Exception as e:
            print("Got the error: ", e)
        finally:
            broadcastMsg(buf, index)


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
    


	