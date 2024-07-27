import numpy as np
from TCPInterface import TCPInterface
from queue import Queue

def response2data(response):
    message = response.split(' ')
    id = int(message[0])
    score = float(message[1])
    # message[2] 格式为 1,1,1\n1,1,1\n1,1,1
    if 'over' in message[2]:
        id = -1
        return Data(id, score, None)
    else:
        mapData = np.vstack([np.fromstring(line, sep=',') for line in message[2].split('\n')])
    data = Data(id, score, mapData)
    return data

def showQueue(queue):
    while not queue.empty():
        data = queue.get()
        print(data.id, data.score)

class Data:
    def __init__(self, id, score, mapData):
        self.id = id
        self.score = score
        self.mapData = mapData

class Dataloader:
    def __init__(self, ip_address, port):
        self.TCP_interface = TCPInterface(ip_address, port)
        self.queue = Queue()

    def startNewGame(self):
        print("start new game")
        response = self.TCP_interface.sAr("+")
        data = response2data(response)
        if data.mapData is None:
            return
        self.queue.put(data)

    def move(self, id, direction):
        response = self.TCP_interface.sAr(f"{id} {direction}")
        data = response2data(response)
        if data.id == -1:
            self.startNewGame()
        else:
            self.queue.put(data)
        
        data = self.queue.get()
        if data:
            return data.id, data.score, data.mapData
        else:
            return None, None, None

    def stop(self, id):
        self.TCP_interface.sAr(f"- {id}")




if __name__ == '__main__':
    ip_address = "127.0.0.1"
    port = 1145
    data_loader = Dataloader(ip_address, port)

    # data_loader.startNewGame()
    id, score, mapData = data_loader.move(0, 0)
    print(id, score, mapData)