
using System.Transactions;
using MonsterCardTradingGame.DataAccess.Repositories;
using MonsterCardTradingGame.DataBase.Repositories;
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
    using MonsterCardTradingGame.DataAccess.Repositories;
    using MonsterCardTradingGame.BusinessLogic;
    using global::MonsterCardTradingGame.Server.Sessions;
    using MonsterCardTradingGame.Models;
    using Moq;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Text.Json;
    using System;
    using static GameManager;
    
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
        private Mock<Parser> _parserMock;
        private Mock<DeckRepository> _deckRepositoryMock;
        private Mock<CardsRepository> _cardsRepositoryMock;
        private Mock<TradingsRepository> _tradingsRepositoryMock;
        private Mapper _mapper;
        private Mock<UserRepository> _userRepositoryMock;
        private Mock<SessionManager> _sessionManagerMock;
        private UserManager _userManager;
        private TransactionManager _transactionManager;
        private Parser _parser;
        // private Mock<PackageRepository> _packageRepositoryMock;

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
            _mapper = new Mapper();
            _deckRepositoryMock = new Mock<DeckRepository>();
            _cardsRepositoryMock = new Mock<CardsRepository>("Host = localhost; Username = myuser; Password = mypassword; Database = mydb");
            _parserMock = new Mock<Parser>();
            _transactionManager = new TransactionManager();
            _tradingsRepositoryMock = new Mock<TradingsRepository>();
            _userManager = new UserManager();
            _userRepositoryMock = new Mock<UserRepository>("Host = localhost; Username = myuser; Password = mypassword; Database = mydb");
            _sessionManagerMock = new Mock<SessionManager>();
            _parser = new Parser();
        }



        //Database Repository Tests
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        [Test]
        public void TestAddingUserAndGettingCredentials_UserIsAdded()
        {
            string username = "testUser2341";
            string password = "testPassword1";
            string image = null;
            string bio = null;
            _userRepository.AddUser(username, password, image, bio, null);
            User user = _userRepository.GetUserCredentials(username);
            Assert.AreEqual(user.Username, username);
        }


        [Test]
        public void UpdatingUser_UserIsUpdated()
        {
            _userRepository.AddUser("testUser3", "testPassword3", null, null, null);
            string username = "testUser3";
            string newUsername = "TEst";
            string password = "testPassword3";
            string image = "-";
            string bio = "-";
            _userRepository.UpdateUser(newUsername,username, password, image, bio);
            User user = _userRepository.GetUserData(_userRepository.GetUserIdForTest(username));
            Assert.AreEqual(user.Name, newUsername);
        }

        [Test]
        public void AddUser_UserIsAdded()
        {
            _userRepository.AddUser("testUser44", "testPassword44", null, null, null);
            User user = _userRepository.GetUserData(_userRepository.GetUserIdForTest("testUser44"));
            Assert.AreEqual(user.Name, "-");
            Assert.AreEqual(user.Coins, 20);
        }


        [Test]
        public void SubstractCoins_CoinsAreSubstracted()
        {
            _userRepository.AddUser("testUser45", "testPassword45", null, null, null);
            _userRepository.SubstractCoins(_userRepository.GetUserIdForTest("testUser45"), 5);
            User user = _userRepository.GetUserData(_userRepository.GetUserIdForTest("testUser45"));
            Assert.AreEqual(user.Name, "-");
            Assert.AreEqual(user.Coins, 15);
        }

        [Test]
        public void HandleLogin_WithCorrectCredentials_ReturnsSuccess()
        {
            
            _userRepository.AddUser("testUser2", "testPassword2", null, null, null);
            var correctUsername = "testUser2";
            var correctPassword = "testPassword2";
            var requestBody = JsonSerializer.Serialize(new Dictionary<string, string>
            {
                { "Username", correctUsername },
                { "Password", correctPassword }
            });
            _userRepositoryMock.Setup(repo => repo.GetPasswordByUsername(correctUsername)).Returns(correctPassword);
            _sessionManagerMock.Setup(sm => sm.GenerateToken(It.IsAny<string>())).Returns("testUser-mctgToken");
            _sessionManagerMock.Setup(sm => sm.CreateSession(It.IsAny<string>(), It.IsAny<int>())); 

            var result = _userManager.HandleLogin(requestBody, string.Empty);

            Assert.IsTrue(result.Contains("200 OK"));
            Assert.IsTrue(result.Contains("User login successful: Token:testUser2-mctgToken"));
        }

        [Test]
        public void HandleLogin_WithMissingCredentials_ReturnsBadRequest()
        {
            
            var requestBody = "{}"; 
            var result = _userManager.HandleLogin(requestBody, string.Empty);
            Assert.IsTrue(result.Contains("401 Bad Request"));
            Assert.IsTrue(result.Contains("Missing Username or Password"));
        }


        [Test]
        public void HandleLogin_WithIncorrectCredentials_ReturnsBadRequest()
        {
            
            var incorrectUsername = "wrongUser";
            var incorrectPassword = "wrongPass";
            var requestBody = JsonSerializer.Serialize(new Dictionary<string, string>
            {
                { "Username", incorrectUsername },
                { "Password", incorrectPassword }
            });
            _userRepositoryMock.Setup(repo => repo.GetPasswordByUsername(incorrectUsername)).Returns((string)null); 

            
            var result = _userManager.HandleLogin(requestBody, string.Empty);

            
            Assert.IsTrue(result.Contains("401 Bad Request"));
            Assert.IsTrue(result.Contains("Invalid username/password provided"));
        }


        [Test]
        public void RegisterUser_WithCorrectData_ReturnsSuccess()
        {
            
            string requestBody = JsonSerializer.Serialize(new Dictionary<string, string>
            {
                { "Username", "NewUser" },
                { "Password", "NewPassword" }
            });

            _userRepositoryMock.Setup(repo => repo.AddUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            _userRepositoryMock.Setup(repo => repo.GetUserCredentials("NewUser")).Returns(new User() /* Mocked user credentials */);

            string result = _userManager.RegisterUser(requestBody, string.Empty);

            
            Assert.IsTrue(result.Contains("201 Created"));
            Assert.IsTrue(result.Contains("User successfully created"));
        }


        [Test]
        public void RegisterUser_WithMissingUsernameOrPassword_ReturnsBadRequest()
        {
            
            string requestBody = JsonSerializer.Serialize(new Dictionary<string, string>
            {
                
            });

            string result = _userManager.RegisterUser(requestBody, string.Empty);

            
            Assert.IsTrue(result.Contains("400 Bad Request"));
            Assert.IsTrue(result.Contains("Missing Username or Password"));
        }

        [Test]
        public void RegisterUser_WithDuplicateUsername_ReturnsConflict()
        {
            string requestBody = JsonSerializer.Serialize(new Dictionary<string, string>
            {
                { "Username", "ExistingUser" },
                { "Password", "NewPassword" }
            });

            
            string username = "ExistingUser";
            string password = "NewPassword";
            string image = null;
            string bio = null;
            _userRepository.AddUser(username, password, image, bio, null);

            string result = _userManager.RegisterUser(requestBody, string.Empty);

            
            Assert.IsTrue(result.Contains("409 Bad Request"));
            Assert.IsTrue(result.Contains("User with same username already registered"));
        }



        [Test]
        public void CreateOrExecuteTradingDeal_CreatesDeal_WhenDataIsValid()
        {
            
            int userId = 1;
            string requestBody = JsonSerializer.Serialize(new Dictionary<string, string>
            {
                { "Id", "dealId1" },
                { "CardToTrade", "card1"},
                {"Type", "Monster"},
                { "MinimumDamage", "50" }
            });
            string requestParameter = "/";

            _tradingsRepositoryMock.Setup(repo => repo.CheckIfDealExists("dealId1")).Returns(false);
            _cardsRepositoryMock.Setup(cards => cards.GetCardsFromDB(userId)).Returns(new List<Card> { new Card("CardName", GameManager.Element.Fire, 50, "card1") });
            var userCards = _cardsRepositoryMock.Object.GetCardsFromDB(userId);
            Console.WriteLine("Mock returned the following cards:");
            foreach (var card in userCards)
            {
                Console.WriteLine($"Card Name: {card.Name}, Element: {card.Element}, Damage: {card.Damage}, UniqueId: {card.UniqueId}");
            }

            string result = _transactionManager.CreateOrExecuteTradingDeal(requestBody, requestParameter, userId);

            Assert.IsTrue(result.Contains("201 Created"));
            Assert.IsTrue(result.Contains("Trading deal successfully created"));
        }

        [Test]
        public void GetScoreBoard_ScoreBoordIsOrderedByElo()
        {
            _userRepository.AddUser("testUser4", "testPassword", null, null, null);
            _userRepository.AddUser("testUser5", "testPassword", null, null, null);
            _statsRepository.UpdateWinStatsInDB(_userRepository.GetUserIdForTest("testUser5"));
            List<UserStats> userStats = _statsRepository.GetAllStatsOrderedByElo();
            
            for (int i = 1; i < userStats.Count; i++)
            {
                Assert.IsTrue(userStats[i].Elo >= userStats[i - 1].Elo, "Die Liste ist nicht richtig nach Elo geordnet.");
            }
        }

        [Test]
        public void GetStats_EloIsCorrect()
        {
            _userRepository.AddUser("testUser10", "testPassword", null, null,null);
            UserStats stats = _statsRepository.GetStatsFromDB(_userRepository.GetUserIdForTest("testUser10"));
            Assert.AreEqual(100, stats.Elo);
        }


        [Test]
        public void CreatePackage_AsAdmin_SuccessfullyCreatesPackage()
        {
            string requestBody = "[\r\n    {\"Id\":\"845f0dc7-37d0-426e-994e-43fc3ac83c08\", \"Name\":\"WaterGoblin\", \"Damage\": 10.0}, \r\n    {\"Id\":\"99f8f8dc-e25e-4a95-aa2c-782823f36e2a\", \"Name\":\"Dragon\", \"Damage\": 50.0}, \r\n    {\"Id\":\"e85e3976-7c86-4d06-9a80-641c2019a79f\", \"Name\":\"WaterSpell\", \"Damage\": 20.0}, \r\n    {\"Id\":\"1cb6ab86-bdb2-47e5-b6e4-68c5ab389334\", \"Name\":\"Ork\", \"Damage\": 45.0}, \r\n    {\"Id\":\"dfdd758f-649c-40f9-ba3a-8657f4b3439f\", \"Name\":\"FireSpell\", \"Damage\": 25.0}\r\n]\r\n"; // Replace with actual valid JSON string
            _userRepository.AddUser("admin", "istrator", null, null, null);
            int userId = _userRepository.GetUserIdForTest("admin");
            TransactionManager transaction = new TransactionManager();
            string result = transaction.CreatePackage(requestBody, "", userId);
            Assert.IsTrue(result.Contains("201 Created"));
        }


        [Test]
        public void CardFight_GoblinVsDragon_DragonWins()
        {
           Card Goblin = new Card("Goblin", Element.Fire, 10, "1");
           Card Dragon = new Card("Dragon", Element.Water, 5, "2");
           int result = _gameManager.CardFight(Goblin, Dragon);
           Assert.AreEqual(2, result);
        }

        [Test]
        public void CardFight_KrakenVsSpell_KrakenWins()
        {
            Card Kraken = new Card("Kraken", Element.Normal, 10, "1");
            Card Spell = new Card("Spell", Element.Water, 5, "2");
            int result = _gameManager.CardFight(Kraken, Spell);
            Assert.AreEqual(1, result);
        }


        [Test]
        public void CardFight_WaterSpellVsFireOrc_SpellWins()
        {
            Card Spell = new Card("Spell", Element.Water, 5, "1");
            Card Orc = new Card("Goblin", Element.Fire, 10, "2");
            int result = _gameManager.CardFight(Spell, Orc);
            Assert.AreEqual(1, result);
        }

        [Test]
        public void CardFight_DamageTie_Draw()
        {
            Card Goblin = new Card("Goblin", Element.Normal, 10, "1");
            Card Goblin2 = new Card("Goblin", Element.Water, 10, "2");
            int result = _gameManager.CardFight(Goblin, Goblin2);
            Assert.AreEqual(0, result);
        }


        [Test]
        public void CardFight_WizardVsOrc_WizardWins()
        {
            Card Wizard = new Card("Wizard", Element.Normal, 10, "1");
            Card Ork = new Card("Ork", Element.Water, 60, "2");
            int result = _gameManager.CardFight(Ork, Wizard);
            Assert.AreEqual(2, result);
        }

        [Test]
        public void CardFight_FireElveVsDragon_FireElveWins()
        {
            Card FireElve = new Card("Elve", Element.Fire, 10, "1");
            Card Dragon = new Card("Dragon", Element.Fire, 60, "2");
            int result = _gameManager.CardFight(Dragon, FireElve);
            Assert.AreEqual(2, result);
        }


        [Test]
        public void CardFight_BoostedCardWins()
        {
            Card Goblin = new Card("Goblin", Element.Normal, 10, "1");
            Card Goblin2 = new Card("Goblin", Element.Water, 10, "2");
            Goblin2.CurrentWins = 6;
            int result = _gameManager.CardFight(Goblin, Goblin2);
            Assert.AreEqual(2, result);
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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
                var token = "kienboec-mctgToken";
                var userId = 2174;

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
