using System;

namespace MonsterCardTradingGame.Server.Sessions
{
    public class UserSession : IEquatable<UserSession>
    {
        public string SessionId { get; set; }
        public string Token { get; set; }

        public bool Equals(UserSession other)
        {
            if (other == null) return false;
            return SessionId == other.SessionId && Token == other.Token;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj is UserSession session)
                return Equals(session);
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SessionId, Token);
        }
    }
}