using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ClassLibraryLake;

namespace Server
{
    class Server
    {
        private static ConcurrentDictionary<string, Lake> _lakes = new ConcurrentDictionary<string, Lake>();

        static async Task Main(string[] args)
        {
            IPHostEntry ipHost = Dns.GetHostEntry("127.0.0.1");  // Указываем явный IP
            IPAddress ipAddr = ipHost.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork); 
            if (ipAddr == null)
            {
                throw new Exception("Не найден доступный IPv4 адрес.");
            }
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 3366);

            Socket listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(ipEndPoint);
                listener.Listen(10);
                Console.WriteLine("Ожидаем подключения через порт {0}", ipEndPoint.Port);

                while (true)
                {
                    Socket handler = await listener.AcceptAsync();
                    int userPort = ((IPEndPoint)handler.RemoteEndPoint).Port;
                    Console.WriteLine($"Клиент {userPort} подключился");
                    HandleClientAsync(handler);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }

        private static async void HandleClientAsync(Socket socket)
        {
            byte[] buffer = new byte[1024];

            try
            {
                while (true)
                {
                    int bytesRead = await socket.ReceiveAsync(buffer, SocketFlags.None);
                    if (bytesRead == 0)
                        break;

                    string json = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine("Получено: " + json);

                    LakeResponse response = ProcessRequest(json);

                    string jsonResponse = JsonSerializer.Serialize(response);
                    byte[] msg = Encoding.UTF8.GetBytes(jsonResponse);
                    await socket.SendAsync(msg);
                    Console.WriteLine("Ответ отправлен: " + jsonResponse);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка клиента: " + ex.Message);
            }
            finally
            {
                int userPort = ((IPEndPoint)socket.RemoteEndPoint).Port;
                Console.WriteLine($"Клиент {userPort} отключился");
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
        }

        private static LakeResponse ProcessRequest(string json)
        {
            LakeResponse response = new LakeResponse { IsSuccess = false };

            try
            {
                var request = JsonSerializer.Deserialize<LakeRequest>(json);
                if(request != null)
                {
                    response.Key = request.Key;
                    switch (request.RequestType)
                    {
                        case LakeRequestType.Get:
                            if (_lakes.TryGetValue(request.Key, out var lake))
                            {
                                response.Lake = lake;
                                response.IsSuccess = true;
                            }
                            else
                                response.ErrorMessage = "Ключ не найден.";
                            break;

                        case LakeRequestType.Add:
                            if (_lakes.TryAdd(request.Key, request.Lake))
                                response.IsSuccess = true;
                            else
                                response.ErrorMessage = "Ключ уже существует.";
                            break;

                        case LakeRequestType.Update:
                            if (_lakes.ContainsKey(request.Key))
                            {
                                _lakes[request.Key] = request.Lake;
                                response.IsSuccess = true;
                            }
                            else
                                response.ErrorMessage = "Ключ не сущесвует.";
                            break;

                        case LakeRequestType.Remove:
                            if (_lakes.TryRemove(request.Key, out _))
                                response.IsSuccess = true;
                            else
                                response.ErrorMessage = "Ключ не найден.";
                            break;

                        default:
                            response.ErrorMessage = "Неизвестный тип операции";
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                response.ErrorMessage = "Ошибка: " + ex.Message;
            }

            return response;
        }
    }
}