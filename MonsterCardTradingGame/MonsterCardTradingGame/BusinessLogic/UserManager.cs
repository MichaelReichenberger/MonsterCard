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
            try
            {
                var userData = JsonSerializer.Deserialize<Dictionary<string, string>>(requestBody);
                if (userData == null || !userData.TryGetValue("Username", out var username) || !userData.TryGetValue("Password", out var password))
                {
                    return "HTTP/1.0 400 Bad Request\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                           JsonSerializer.Serialize(new { Message = "Missing Username or Password" });
                }

                string bio = userData.TryGetValue("Bio", out var bioValue) ? bioValue : "";
                string image = userData.TryGetValue("Image", out var imageValue) ? imageValue : "";

                
                _userRepository.AddUser(username, password, image, bio); // If an error occurs, it will go to catch block

                
                return "HTTP/1.0 201 Created\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" + "User successfully created: "+
                       JsonSerializer.Serialize(_userRepository.GetUserCredentials(username));
            }
            catch (Exception e)
            { // Log the detailed exception
                Console.WriteLine(e.Message);
                return "HTTP/1.0 409 Bad Request\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                       "User with same username already registered";
            }
        }

        public string GetUserData(string requestBody, string requestParameter, int userId)
        {
            try
            {
                if (_userRepository.GetUserId(requestParameter) != userId && _userRepository.GetRole(userId) != "admin")
                {
                    return "HTTP/1.0 401 Bad Request\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                           "Access token is missing or invalid";
                }
                return "HTTP/1.0 200 OK\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +"Data sucessfully retrieved: "+
                       JsonSerializer.Serialize(_userRepository.GetUserData(userId));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "HTTP/1.0 401 Bad Request\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                       "User not found";
            }
        }

        public string UpdateUserData(string requestBody, string requestParameter, int userId)
        {
            try
            {
                if (_userRepository.GetUserId(requestParameter) != userId && _userRepository.GetRole(userId) != "admin")
                {
                    return "HTTP/1.0 401 Bad Request\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                           "Access token is missing or invalid";
                }
                var userData = JsonSerializer.Deserialize<Dictionary<string, string>>(requestBody);

                if (String.IsNullOrEmpty(requestParameter))
                {
                    return "HTTP/1.0 201 Created\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                           "Missing username";
                }

                if (_userRepository.GetUserId(requestParameter) != userId)
                {
                    return "HTTP/1.0 401 Bad Request\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                           "User not found";
                }
                string username = userData.TryGetValue("Name", out var usernameValue) ? usernameValue : "";
                string bio = userData.TryGetValue("Bio", out var bioValue) ? bioValue : "";
                string image = userData.TryGetValue("Image", out var imageValue) ? imageValue : "";
                string password = userData.TryGetValue("Password", out var passwordValue) ? passwordValue : "";

                _userRepository.UpdateUser(username, requestParameter, password, image, bio);
                return "HTTP/1.0 201 Created\r\nContent-Type: application/json; charset=utf-8\r\n\r\n"+ "User sucessfully updated: " +
                       JsonSerializer.Serialize(_userRepository.GetUserData(userId));
            }
            catch (Exception ex)
            {
                return
                    "HTTP/1.0 500 Internal Server Error\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                    "User not Found";
            }
        }

        public string HandleLogin(string requestBody, string requestParameter)
        {
            var userData = JsonSerializer.Deserialize<Dictionary<string, string>>(requestBody);
            
            if (userData == null || !userData.TryGetValue("Username", out var username) ||
                !userData.TryGetValue("Password", out var password))
            {
                return "HTTP/1.0 401 Bad Request\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                       "Missing Username or Password";
            }

            if (password != _userRepository.GetPasswordByUsername(username))
            {
                Console.WriteLine(password);
                Console.WriteLine(_userRepository.GetPasswordByUsername(username));
            }
            else
            {
                var sessionManager = SessionManager.Instance;
                var token = sessionManager.GenerateToken(username); // Token generieren
                int userIdByUserName = _userRepository.GetUserId(username).Value;
                var sessionId = sessionManager.CreateSession(token, userIdByUserName);
                return "HTTP/1.0 200 OK\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" + $"User login successful: Token:{token} ";
            }

            return "HTTP/1.0 401 Bad Request\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                   "Invalid username/password provided:  Token should be {username}-mctgToken";
        }

        internal string GetUserStats(int userId)
        {
            
            return
                "HTTP/1.0 500 Internal Server Error\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                JsonSerializer.Serialize(_statsRepository.GetStatsFromDB(userId));
        }

        internal string GetUserScoreboard(int userId)
        {

            return
                "HTTP/1.0 500 Internal Server Error\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                JsonSerializer.Serialize(_statsRepository.GetAllStatsOrderedByElo());
        }

    }
}
 