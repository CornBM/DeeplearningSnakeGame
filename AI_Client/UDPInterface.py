import socket
import struct

class UDPInterface:
    def __init__(self, host, port):
        self.host = host
        self.port = port
        self.socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        self.socket.settimeout(10)  # 设置超时时间，根据需要调整

    def send_and_receive(self, message):
        """
        发送一个字符串信息，并接收响应。
        :param message: 要发送的字符串信息。
        :return: 解析为UTF-8字符串的响应包。
        """
        # 将字符串编码为bytes
        message_encoded = message.encode('utf-8')
        
        # 发送数据
        self.socket.sendto(message_encoded, (self.host, self.port))
        
        try:
            # 接收数据
            response, _ = self.socket.recvfrom(1024)  # 1024字节缓冲区大小，根据需要调整
            return response.decode('ascii')  # 解码为ascii字符串
        except socket.timeout:
            print("接收数据超时")
            return None

    def close(self):
        """关闭socket连接"""
        self.socket.close()

# 使用示例
if __name__ == "__main__":
    # 假设有一个UDP服务器运行在本地的9999端口
    udp_interface = UDPInterface('localhost', 1145)
    
    # 发送字符串信息并接收响应
    response = udp_interface.send_and_receive("- 1")
    if response is not None:
        print("接收到的响应:", response)
    
    # 关闭连接
    udp_interface.close()