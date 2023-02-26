import socket,os
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)  
sock.bind(('127.0.0.1', 1404))  
sock.listen()
print("Listening")
connection,address = sock.accept() 
print("Got a user!")   
while True:  
    buf = connection.recv(1024)  
    print(buf)


	