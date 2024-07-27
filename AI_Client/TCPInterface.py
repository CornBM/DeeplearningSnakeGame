import socket

class TCPInterface:
    def __init__(self, host, port):
        self.host = host
        self.port = port
        self.socket = None
        self.connect()

    def connect(self):
        """连接到TCP服务器"""
        self.socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.socket.settimeout(10)  # 设置超时时间，根据需要调整
        try:
            self.socket.connect((self.host, self.port))
        except socket.error as e:
            print(f"连接失败: {e}")
            self.close()

    def sAr(self, message):
        """
        发送一个字符串信息，并接收响应。
        :param message: 要发送的字符串信息。
        :return: 解析为UTF-8字符串的响应包。
        """
        if self.socket:
            try:
                # 将字符串编码为bytes
                message_encoded = message.encode('utf-8')
                
                # 发送数据
                self.socket.sendall(message_encoded)
                
                # 接收数据
                response = self.socket.recv(1024)  # 1024字节缓冲区大小，根据需要调整
                return response.decode('utf-8')  # 解码为utf-8字符串
            except socket.timeout:
                print("接收数据超时")
                return None
            except socket.error as e:
                print(f"接收数据失败: {e}")
                return None
        else:
            print("未连接到服务器")
            return None

    def close(self):
        """关闭socket连接"""
        if self.socket:
            self.socket.close()

# 使用示例
if __name__ == "__main__":
    # 假设有一个TCP服务器运行在本地的9999端口
    tcp_interface = TCPInterface('localhost', 5555)
    
    # 发送字符串信息并接收响应
    response = tcp_interface.sAr("+")
    if response is not None:
        print("接收到的响应:", response)
    
    # 关闭连接
    tcp_interface.close()