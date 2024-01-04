using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MonsterCardTradingGame.DataBase.Repositories;
using MonsterCardTradingGame.Server.Sessions;

namespace MonsterCardTradingGame.BusinessLogic
{
    internal class UserManager
    {
        private UserRepository _userRepository;
        private StatsRepository _statsRepository;

        public UserManager()
        {
            _userRepository = new UserRepository("Host=localhost;Username=myuser;Password=mypassword;Database=mydb");
            _statsRepository = new StatsRepository("Host=localhost;Username=myuser;Password=mypassword;Database=mydb");
        }


        public string RegisterUser(string requestBody, string requestParameter)
        {
            Console.WriteLine($"Handling client on thread: {Thread.CurrentThread.ManagedThreadId}");
            try
            {
                var userData = JsonSerializer.Deserialize<Dictionary<string, string>>(requestBody);

                if (userData == null || !userData.TryGetValue("Username", out var username) ||
                    !userData.TryGetValue("Password", out var password))
                {
                    return "HTTP/1.0 400 Bad Request\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                           JsonSerializer.Serialize(new { Message = "Missing Username or Password" });
                }
                string bio = userData.TryGetValue("Bio", out var bioValue) ? bioValue : "";
                string image = userData.TryGetValue("Image", out var imageValue) ? imageValue : "";
                _userRepository.AddUser(username, password, image, bio);
                return "HTTP/1.0 201 Created\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                       JsonSerializer.Serialize(new { Message = "User successfully created" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return
                    "HTTP/1.0 409 Internal Server Error\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                    JsonSerializer.Serialize(new { Message = $"An error occurred: {e.Message}" });
            }
        }


        public string GetUserData(string requestBody, string requestParameter, int userId)
        {
            try
            {
                if (_userRepository.GetUserId(requestParameter) != userId)
                {
                    return "HTTP/1.0 401 Bad Request\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                           JsonSerializer.Serialize(new { Message = "It is not allowed to edit other users!" });
                }
                var userData = _userRepository.GetUserData(requestParameter);
                var userAsJson = JsonSerializer.Serialize(userData);
                return "HTTP/1.0 200 OK\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" + userAsJson;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return "HTTP/1.0 401 Bad Request\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                   JsonSerializer.Serialize(new { Message = "It is not allowed to edit other users!" });
        }

        public string UpdateUserData(string requestBody, string requestParameter, int userId)
        {
            try
            {
                var userData = JsonSerializer.Deserialize<Dictionary<string, string>>(requestBody);

                if (String.IsNullOrEmpty(requestParameter))
                {
                    return "HTTP/1.0 201 Created\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                           JsonSerializer.Serialize(new { Message = "Missing username" });
                }

                if (_userRepository.GetUserId(requestParameter) != userId)
                {
                    return "HTTP/1.0 401 Bad Request\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                           JsonSerializer.Serialize(new { Message = "It is not allowed to edit other users!" });
                }
                string username = userData.TryGetValue("Name", out var usernameValue) ? usernameValue : "";
                string bio = userData.TryGetValue("Bio", out var bioValue) ? bioValue : "";
                string image = userData.TryGetValue("Image", out var imageValue) ? imageValue : "";
                string password = userData.TryGetValue("Password", out var passwordValue) ? passwordValue : "";

                _userRepository.UpdateUser(username, requestParameter, password, image, bio);
                return "HTTP/1.0 201 Created\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                       JsonSerializer.Serialize(new { Message = "User updated successfully" });
            }
            catch (Exception ex)
            {
                return
                    "HTTP/1.0 500 Internal Server Error\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                    JsonSerializer.Serialize(new { Message = $"An error occurred: {ex.Message}" });
            }
        }

        public string HandleLogin(string requestBody, string requestParameter)
        {
            var userData = JsonSerializer.Deserialize<Dictionary<string, string>>(requestBody);
            
            if (userData == null || !userData.TryGetValue("Username", out var username) ||
                !userData.TryGetValue("Password", out var password))
            {
                return "HTTP/1.0 401 Bad Request\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                       JsonSerializer.Serialize(new { Message = "Missing Username or Password" });
            }

            if (password != _userRepository.getPasswordByUsername(username))
            {
                Console.WriteLine(password);
                Console.WriteLine(_userRepository.getPasswordByUsername(username));
            }
            else
            {
                var sessionManager = SessionManager.Instance;
                var token = sessionManager.GenerateToken(username); // Token generieren
                int userIdByUserName = _userRepository.GetUserId(username).Value;
                var sessionId = sessionManager.CreateSession(token, userIdByUserName);
                return "HTTP/1.0 200 OK\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                       JsonSerializer.Serialize(new { Message = "User successfully logged in", Token = token, SessionId = sessionId });
            }

            return "HTTP/1.0 401 Bad Request\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                   JsonSerializer.Serialize(new { Message = "Internal Error" });
        }

        internal string GetUserStats(int userId)
        {
            return
                "HTTP/1.0 500 Internal Server Error\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                JsonSerializer.Serialize(new { Message = _statsRepository.GetStatsFromDB(userId) });
        }

        internal string GetSocreBoard()
        {
            return
                "HTTP/1.0 500 Internal Server Error\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                JsonSerializer.Serialize(new { Message = _statsRepository.GetAllStatsOrderedByElo() });
        }
    }
}
