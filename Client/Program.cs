using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ClassLibraryLake;

class Client
{
    static async Task Main(string[] args)
    {
        try
        {
            using (Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                Console.WriteLine("Подключение к серверу...");
                await clientSocket.ConnectAsync("127.0.0.1", 3366);
                Console.WriteLine("Клиент подключен к серверу!");

                var request = new LakeRequest
                {
                    RequestType = LakeRequestType.Add, 
                    Key = "Байкал", 
                    Lake = new Lake 
                    { 
                        Name = "Озеро Байкал", 
                        Depth = 1642, 
                        MortalityCount = 0 
                    }
                };

                // Сериализуем запрос в JSON
                string jsonRequest = JsonSerializer.Serialize(request);
                byte[] message = Encoding.UTF8.GetBytes(jsonRequest);

                // Отправляем запрос серверу
                Console.WriteLine("Отправка запроса серверу...");
                await clientSocket.SendAsync(message, SocketFlags.None);
                Console.WriteLine($"Запрос отправлен: {jsonRequest}");

                // --- Ожидаем ответ от сервера ---
                byte[] buffer = new byte[1024];
                int bytesRead = await clientSocket.ReceiveAsync(buffer, SocketFlags.None);
                string jsonResponse = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                // Выводим ответ сервера
                Console.WriteLine($"Ответ от сервера: {jsonResponse}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка клиента: {ex.Message}");
        }
    }
}
