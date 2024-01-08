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
    public class UserManager
    {
        private UserRepository _userRepository;
        private StatsRepository _statsRepository;

        public UserManager()
        {
            _userRepository = new UserRepository("Host=localhost;Username=myuser;Password=mypassword;Database=mydb");
            _statsRepository = new StatsRepository("Host=localhost;Username=myuser;Password=mypassword;Database=mydb");
        }


        //Add a new User to the system
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string RegisterUser(string requestBody, string requestParameter)
        {
            try
            {
                var userData = JsonSerializer.Deserialize<Dictionary<string, string>>(requestBody);
                //Check for missing credentials
                if (userData == null || !userData.TryGetValue("Username", out var username) || !userData.TryGetValue("Password", out var password))
                {
                    return "HTTP/1.0 400 Bad Request\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                           JsonSerializer.Serialize(new { Message = "Missing Username or Password" });
                }

                //initialize optional parameters
                string bio = userData.TryGetValue("Bio", out var bioValue) ? bioValue : "";
                string image = userData.TryGetValue("Image", out var imageValue) ? imageValue : "";
                _userRepository.AddUser(username, password, image, bio, null); // If an error occurs, it will go to catch block

                return "HTTP/1.0 201 Created\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" + "User successfully created\r\n " +
                       JsonSerializer.Serialize(_userRepository.GetUserCredentials(username));
            }
            catch (Exception e)
            { 
                Console.WriteLine(e.Message);
                return "HTTP/1.0 409 Bad Request\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                       "User with same username already registered";
            }
        }

        //Get the data for a specific user
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string GetUserData(string requestBody, string requestParameter, int userId)
        {
            try
            {
                //Check if user is authorized to get the data (right user or admin)
                if (_userRepository.GetUserId(requestParameter) != userId && _userRepository.GetRole(userId) != "admin")
                {
                    return "HTTP/1.0 401 Bad Request\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                           "Access token is missing or invalid";
                }
                return "HTTP/1.0 200 OK\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" + "Data sucessfully retrieved\r\n " +
                       JsonSerializer.Serialize(_userRepository.GetUserData(userId));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "HTTP/1.0 401 Bad Request\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                       "User not found";
            }
        }


        //Update the data for a specific user
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string UpdateUserData(string requestBody, string requestParameter, int userId)
        {
            try
            {
                //Check if user is authorized to update the data (right user or admin)
                if (_userRepository.GetUserId(requestParameter) != userId && _userRepository.GetRole(userId) != "admin")
                {
                    return "HTTP/1.0 401 Unauthorized\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                           "Access token is missing or invalid";
                }
                var userData = JsonSerializer.Deserialize<Dictionary<string, string>>(requestBody);

                //check if username is empty
                if (String.IsNullOrEmpty(requestParameter))
                {
                    return "HTTP/1.0 404 Not found\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                           "User not found - Username is empty";
                }

                //check if user exists
                if (_userRepository.GetUserId(requestParameter) != userId)
                {
                    return "HTTP/1.0 404 Bad Not found\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                           "User not found";
                }
                
                //initialize optional parameters
                string username = userData.TryGetValue("Name", out var usernameValue) ? usernameValue : "";
                string bio = userData.TryGetValue("Bio", out var bioValue) ? bioValue : "";
                string image = userData.TryGetValue("Image", out var imageValue) ? imageValue : "";
                string password = userData.TryGetValue("Password", out var passwordValue) ? passwordValue : "";

                //Update the user in DB
                _userRepository.UpdateUser(username, requestParameter, password, image, bio);
                return "HTTP/1.0 201 Created\r\nContent-Type: application/json; charset=utf-8\r\n\r\n"+ "User sucessfully updated\r\n " +
                       JsonSerializer.Serialize(_userRepository.GetUserData(userId));
            }
            catch (Exception ex)
            {
                return
                    "HTTP/1.0 404 Not found\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                    "User not Found";
            }
        }

        //Handle the login of a user
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public string HandleLogin(string requestBody, string requestParameter)
        {
            //Serialize the request body
            var userData = JsonSerializer.Deserialize<Dictionary<string, string>>(requestBody);
            
            //Check for missing credentials
            if (userData == null || !userData.TryGetValue("Username", out var username) ||
                !userData.TryGetValue("Password", out var password))
            {
                return "HTTP/1.0 401 Bad Request\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                       "Missing Username or Password";
            }

            //Check if password is correct
            if (password != _userRepository.GetPasswordByUsername(username))
            {
                Console.WriteLine(password);
                Console.WriteLine(_userRepository.GetPasswordByUsername(username));
            }
            else
            {
                var sessionManager = SessionManager.Instance;
                //Generate Token
                var token = sessionManager.GenerateToken(username); 
                int userIdByUserName = _userRepository.GetUserId(username).Value;
                //Create Session
                string sessionId = sessionManager.CreateSession(token, userIdByUserName);
                if (sessionId != "User already logged in")
                {
                    return "HTTP/1.0 200 OK\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" + $"User login successful: Token:{token} ";
                }
                else
                {
                    return "HTTP/1.0 409 Conflict OK\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" + "User already logged in";
                }
            }

            return "HTTP/1.0 401 Bad Request\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" +
                   "Invalid username/password provided:  Token should be {username}-mctgToken";
        }


        //Get stats from a specific user
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        internal string GetUserStats(int userId)
        {
            
            return
                "HTTP/1.0 200 OK\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" + "The stats could be retrieved successfully\r\n" +
                JsonSerializer.Serialize(_statsRepository.GetStatsFromDB(userId));
        }

        //Get all Stats from all users (Scoreboard)
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        internal string GetUserScoreboard(int userId)
        {

            return
                "HTTP/1.0 200 OK\r\nContent-Type: application/json; charset=utf-8\r\n\r\n" + "The scoreboard could be retrieved successfully\r\n" + 
                JsonSerializer.Serialize(_statsRepository.GetAllStatsOrderedByElo());
        }

    }
}
 