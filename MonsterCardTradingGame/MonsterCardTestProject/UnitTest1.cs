
using NUnit;

namespace MonsterCardTestProject
{

    using NUnit.Framework;
    using NUnit.Framework;
    using MonsterCardTradingGame.BusinessLogic;
    using System.Diagnostics.Metrics;
    using NUnit.Framework;
    using MonsterCardTradingGame.Server.Routes;
    using MonsterCardTradingGame.Server;
    using MonsterCardTradingGame.Server.Sessions;
    using MonsterCardTradingGame.DataBase.Repositories;
    using MonsterCardTradingGame.BusinessLogic;
    using global::MonsterCardTradingGame.Server.Sessions;

    public class Tests
    {

        private RouteConfig _routeConfig;
        private Router _router;
        private GameManager _gameManager;
        private UserRepository _userRepository;
        private PackageRepository _packageRepository;
        private CardsRepository _cardsRepository;
        private TradingsRepository _tradingsRepository;
        private DeckRepository _deckRepository;
        private StatsRepository _statsRepository;
        [SetUp]
        public void Setup()
        {
            _router = new Router();
            _gameManager = GameManager.Instance;
            _userRepository = new UserRepository("Host=localhost;Username=myuser;Password=mypassword;Database=mydb");
            _packageRepository = new PackageRepository("Host = localhost; Username = myuser; Password = mypassword; Database = mydb");
            _cardsRepository = new CardsRepository("Host = localhost; Username = myuser; Password = mypassword; Database = mydb");
            _tradingsRepository = new TradingsRepository("Host = localhost; Username = myuser; Password = mypassword; Database = mydb");
            _deckRepository = new DeckRepository("Host = localhost; Username = myuser; Password = mypassword; Database = mydb");
            _statsRepository = new StatsRepository("Host = localhost; Username = myuser; Password = mypassword; Database = mydb");
            _routeConfig = new RouteConfig(_router);
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }

        [Test]
        public void TestApckageGeneration()
        {
            // Act
            // Assert
            // Nehmen wir an, GeneratePackage sollte eine Liste von Karten zurückgeben und Sie erwarten, dass die Liste nicht leer ist.
            
        }

        [Test]
        public void TestUserRegistration()
        {

        }

        

        [TestFixture]
        public class ParserTests
        {
            private Parser _parser;

            [SetUp]
            public void Setup()
            {
                _parser = new Parser();
            }

            [Test]
            public void ParseCards_ValidCardName_ReturnsExpectedElementAndCreature()
            {
                var result = _parser.ParseCards("Fire Goblin");
                Assert.AreEqual(("Fire", "Goblin"), result);
            }

            [Test]
            public void ParseUrl_ValidRequestLine_ReturnsExpectedUrlPart()
            {
                var result = _parser.ParseUrl("GET /api/cards HTTP/1.1", 2);
                Assert.AreEqual("cards", result);
            }

            [Test]
            public void IsValidJson_ValidJson_ReturnsTrue()
            {
                var result = _parser.IsValidJson("{\"name\":\"Test\"}");
                Assert.IsTrue(result);
            }

            [Test]
            public void IsValidJson_InvalidJson_ReturnsFalse()
            {
                var result = _parser.IsValidJson("Invalid Json");
                Assert.IsFalse(result);
            }

            [Test]
            public void GenerateToken_ShouldReturnCorrectFormat()
            {
                var username = "testUser";
                var expectedToken = username + "-mctgToken";

                var token = SessionManager.Instance.GenerateToken(username);

                Assert.AreEqual(expectedToken, token);
            }

            [Test]
            public void CreateSession_ShouldAddNewSession()
            {
                var token = "testUser-mctgToken";
                var userId = 1;

                var sessionId = SessionManager.Instance.CreateSession(token, userId);

                var session = SessionManager.Instance.GetSessionByToken(token);

                Assert.NotNull(session);
                Assert.AreEqual(sessionId, session.SessionId);
                Assert.AreEqual(token, session.Token);
                Assert.AreEqual(userId, session.UserID);
            }

            [Test]
            public void GetSessionByToken_ShouldReturnCorrectSession()
            {
                var token = "testUser-mctgToken";
                var userId = 1;

                var sessionId = SessionManager.Instance.CreateSession(token, userId);

                var session = SessionManager.Instance.GetSessionByToken(token);

                Assert.NotNull(session);
                Assert.AreEqual(sessionId, session.SessionId);
                Assert.AreEqual(token, session.Token);
                Assert.AreEqual(userId, session.UserID);
            }

            [Test]
            public void GetUserIdBySessionId_ShouldReturnCorrectUserId()
            {
                var token = "testUser-mctgToken";
                var userId = 1;

                var sessionId = SessionManager.Instance.CreateSession(token, userId);

                var retrievedUserId = SessionManager.Instance.GetUserIdBySessionId(sessionId);

                Assert.AreEqual(userId, retrievedUserId);
            }

            [Test]
            public void GetUserIDByToken_ShouldReturnCorrectUserId()
            {
                var token = "testUser-mctgToken";
                var userId = 1;

                SessionManager.Instance.CreateSession(token, userId);

                var retrievedUserId = SessionManager.Instance.GetUserIDByToken(token);

                Assert.AreEqual(userId, retrievedUserId);
            }
        }
    }
}
