import socket
import os
from threading import Thread
import json

#Declare Variables
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)  
clients = []
clientsAmmount = -1

#Declare Functions
def broadcastMsg(msg, index):
    global clients
    for client in clients:
        client.sendall(msg.encode())

def analyzePacket(sock, data, clientId):
    print(data)
    try:
        broadcastMsg(data, clientId)
    except Exception as e:
        print("We got a trash packet from clientId ", clientId, " reason: ", e)

def handleClient(conn, add, index):
    pack = ""
    while True:
        try: 
            buf = conn.recv(1) 
            c = buf.decode()
            if c!='}':
                pack+=c
            else:
                pack+=c
                analyzePacket(conn, pack, index)
                pack=""
        except:
            print("We lost connection with ", index)
            clients.pop(index)
            break

        


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
    


	