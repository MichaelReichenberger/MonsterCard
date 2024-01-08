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


        //Generate session token
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual string GenerateToken(string username)
        {
            return username+"-mctgToken";
        }


        //Create session
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual string CreateSession(string token, int userId)
        {

            //Check if user is already logged in
            if (_sessions.FirstOrDefault(session => session.Token == token) != null)
            {
                return "User already logged in";
            }
            var sessionId = Guid.NewGuid().ToString();
            var newSession = new UserSession { SessionId = sessionId, Token = token, UserID = userId};

            //Add session to session list
            _sessions.Add(newSession);
            Console.WriteLine($"Session erstellt: SessionId={newSession.SessionId}, Token={newSession.Token}");
            return sessionId;
        }


        //Get session by Token
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual UserSession GetSessionByToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }
            return _sessions.FirstOrDefault(session => session.Token == token);
        }

        public virtual int GetUserIdBySessionId(string sessionId)
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

        public virtual int GetUserIDByToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return -1;
            }
            else
            {
                Console.WriteLine("UserToken: " + token);
            }
            var session = _sessions.FirstOrDefault(session => session.Token == token);
            if (session == null)
            {
                Console.WriteLine("Session nicht gefunden");
                return -1;
            }
            Console.WriteLine("UserId: "+session.UserID);
            return session.UserID;
        }

        public virtual string GetTokenByUserId(int userId)
        {
            var session = _sessions.FirstOrDefault(session => session.UserID == userId);
            if (session == null)
            {
                return null;
            }
            return session.Token;
        }

        public void RemoveSessionByUserId(int userId)
        {
            var session = _sessions.FirstOrDefault(session => session.UserID == userId);
            if (session != null)
            {
                _sessions.Remove(session);
            }
        }
    }
}