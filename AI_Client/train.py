from SnakeNet import SnakeNet
import numpy as np
import signal
import sys
import torch
import torch.optim as optim
from dataloader import Dataloader, Data
import threading
import time
import math
from bfs import bfs
import torch.nn as nn

def score2loss(score):
    return 1 * (1.1 ** -score) + 100

def train():
    dataloader.startNewGame()
    data = dataloader.queue.get()
    id = data.id
    mapData = data.mapData
    while flag:
        optimizer.zero_grad()

        intput = torch.from_numpy(mapData).unsqueeze(0).unsqueeze(0).float().to(device)
        output = net(intput)
        direction = torch.argmax(output, dim=1).cpu().numpy()[0]
        bfsDirection = bfs(mapData)
        # 接收数据
        id, Score, mapData = dataloader.move(id, direction)
        # loss = score2loss(Score) + 10* output[0][direction]
        loss = nn.CrossEntropyLoss()(output, torch.tensor([bfsDirection]).to(device)) * 10
        print(f"loss: {loss:.4f}")
        loss.backward()
        optimizer.step()
        time.sleep(0.1)

    print("Finished Training")
    torch.save(net.state_dict(), "snake2.pth")

def bfsRun():
    dataloader.startNewGame()
    data = dataloader.queue.get()
    id = data.id
    mapData = data.mapData
    while flag:
        direction = bfs(mapData)
        # 接收数据
        id, Score, mapData = dataloader.move(id, direction)

    print("Finished Training")

if __name__ == "__main__":
    # 训练网络
    batch_size = 1
    epochs = 10
    
    device = torch.device("cuda:0" if torch.cuda.is_available() else "cpu")
    print(torch.cuda.is_available())

    # 初始化网络和优化器
    net = SnakeNet(4)

    # 加载参数
    net.load_state_dict(torch.load("snake1.pth"))

    net.to(device)
    dataloader = Dataloader('127.0.0.1', 5555)
    optimizer = optim.Adam(net.parameters(), lr=0.1)

    flag = False
    while True:
        # 主线程等待用户输入
        key = input()
        if key.lower() == 'c':
            flag = False  # 设置事件，通知训练线程停止
            train_thread.join()  # 等待训练线程结束
            break

        elif key.lower() == 't':
            flag = True
            train_thread = threading.Thread(target=train, args=())
            train_thread.start()

        elif key.lower() == 'r':
            flag = True
            bfs_thread = threading.Thread(target=bfsRun, args=())
            bfs_thread.start()

