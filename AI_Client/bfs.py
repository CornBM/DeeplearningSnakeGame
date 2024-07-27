import numpy as np
from queue import Queue

class Point:
    def __init__(self, position, last):
        self.position = position
        self.last = last

def getHead(mapData):
    max = np.max(mapData)
    return np.where(mapData == max)

def getFood(mapData):
    return np.where(mapData == -1)

def getTail(mapData):
    return np.where(mapData == 1)

def positionToIndex(position, shape):
    return (position[0] % shape[0], position[1] % shape[1])

# 给出p2相对于p1的方向 
def getDirection(p1, p2, shape):
    xGap = (p2[1] - p1[1]) % shape[1]
    yGap = (p2[0] - p1[0]) % shape[0]

    if yGap == shape[1] - 1:
        return 0
    elif yGap == 1:
        return 1
    elif xGap == shape[0] - 1:
        return 2
    elif xGap == 1:
        return 3
    else:
        return -1

def bfs(map):
    queue = Queue()
    food = getFood(map)
    tail = getTail(map)
    head = getHead(map)
    visited = np.zeros(map.shape)
    queue.put(Point(head, None))

    foodPoint = None
    tailPoint = None

    while queue.qsize() > 0:
        current = queue.get()
        if visited[current.position] == 1:
            continue

        if map[current.position] > 1 and current.position != head:
            continue

        if current.position == food:
            foodPoint = current
        
        if current.position == tail:
            tailPoint = current

        if foodPoint and tailPoint:
            break

        queue.put(Point(positionToIndex((current.position[0] - 1, current.position[1]), map.shape), current))
        queue.put(Point(positionToIndex((current.position[0] + 1, current.position[1]), map.shape), current))
        queue.put(Point(positionToIndex((current.position[0], current.position[1] - 1), map.shape), current))
        queue.put(Point(positionToIndex((current.position[0], current.position[1] + 1), map.shape), current))

        visited[current.position] = 1


    path = np.zeros(map.shape)
    goalPoint = None
    if foodPoint:
        goalPoint = foodPoint
    elif tailPoint:
        goalPoint = tailPoint
    else:
        print("No path found")

    # if goalPoint:
    #     while goalPoint.last:
    #         path[goalPoint.position] = 1
    #         goalPoint = goalPoint.last
    #     path[goalPoint.position] = 1

    # return path

    nextPoint = None
    #print(goalPoint.position)
    if goalPoint:
        while True:
            if goalPoint.last.last == None:
                nextPoint = goalPoint.position
                break
            goalPoint = goalPoint.last

    return getDirection(head, nextPoint, map.shape)

def test():
    map = np.array([[ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                    [ 0, 0,-1, 0, 0, 0, 0, 0, 0, 0],
                    [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                    [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                    [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                    [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                    [ 0, 0, 0, 0, 0, 3, 0, 0, 0, 0],
                    [ 0, 0, 0, 0, 0, 2, 0, 0, 0, 0],
                    [ 0, 0, 0, 0, 0, 1, 0, 0, 0, 0],
                    [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]])

    path = bfs(map)
    print(path)

if __name__ == '__main__':
    test()

    
