using System;
using System.Collections.Generic;
using System.Linq;

namespace MonsterCardTradingGame.Server.Sessions
{
    public class SessionManager
    {
        private static SessionManager _instance;
        private HashSet<UserSession> _sessions;

        private SessionManager()
        {
            _sessions = new HashSet<UserSession>();
        }

        public static SessionManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SessionManager();
                }
                return _instance;
            }
        }

        public string GenerateToken(string username)
        {
            return username+"-mctgToken"; // Erzeugt einen zufälligen, einzigartigen String
        }

        public string CreateSession(string token, int userId)
        {
            var sessionId = Guid.NewGuid().ToString();
            var newSession = new UserSession { SessionId = sessionId, Token = token, UserID = userId};
            _sessions.Add(newSession);
            Console.WriteLine($"Session erstellt: SessionId={newSession.SessionId}, Token={newSession.Token}");
            return sessionId;
        }

        public UserSession GetSessionByToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }
            return _sessions.FirstOrDefault(session => session.Token == token);
        }

        public int GetUserIdBySessionId(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                return -1;
            }
            var session = _sessions.FirstOrDefault(session => session.SessionId == sessionId);
            if (session == null)
            {
                return -1;
            }
            return session.UserID;
        }

        public int GetUserIDByToken(string token)
        {
            Console.WriteLine(token);
            if (string.IsNullOrEmpty(token))
            {
                return -1;
            }
            var session = _sessions.FirstOrDefault(session => session.Token == token);
            if (session == null)
            {
                Console.WriteLine("Session nicht gefunden");
                return -1;
            }
            return session.UserID;
        }
    }
}