import torch
import torch.nn as nn
import torch.nn.functional as F
import numpy as np

class SnakeNet(nn.Module):
    def __init__(self, num_classes):
        super(SnakeNet, self).__init__()
        # 定义网络结构
        self.conv1 = nn.Conv2d(1, 32, kernel_size=3, padding=1)  # 假设输入通道为1
        self.conv2 = nn.Conv2d(32, 64, kernel_size=3, padding=1)
        self.avgpool = nn.AvgPool2d(2, 2)  # 使用平均池化
        self.fc1 = nn.Linear(64 * 2 * 2, 128)  # 假设经过两次卷积和池化后的特征图大小为2*2
        self.fc2 = nn.Linear(128, num_classes)
        
    def forward(self, x):
        # 假设输入x的形状为(batch_size, 1, 10, 10)
        x = F.relu(self.conv1(x)) # 10*10 -> 10*10
        x = self.avgpool(x)  # 应用平均池化 10*10 -> 5*5
        x = F.relu(self.conv2(x)) # 5*5
        x = self.avgpool(x)  # 应用平均池化 2*2
        
        # 展平特征图用于全连接层
        x = x.view(-1, 64 * 2 * 2)  # 展平后的特征数量
        
        x = F.relu(self.fc1(x))
        x = self.fc2(x)  # 输出原始类别分数
        return x


if __name__ == '__main__':
    # 实例化网络
    num_classes = 4  # 输出类别数
    net = SnakeNet(num_classes)

    # 假设有一个随机初始化的输入矩阵，形状为(batch_size, channels, height, 10, width, 10)
    input = np.random.randn(10, 10)
    input = torch.from_numpy(input).unsqueeze(0).unsqueeze(0).float()  # 增加batch和channel维度

    # 前向传播获取one-hot编码输出
    output = net(input)
    label = torch.argmax(output, dim=1).numpy()[0]  # 输出类别索引
    # 转为int类型

    print(output[0][label])  # 应该输出(batch_size, num_classes)