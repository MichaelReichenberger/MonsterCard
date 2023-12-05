
using NUnit;

namespace MonsterCardTestProject
{

    using NUnit.Framework;
    using MonsterCardTradingGame.Models.BaseClasses;
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
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
            var package = new Package("Test");
            // Assert
            // Nehmen wir an, GeneratePackage sollte eine Liste von Karten zurückgeben und Sie erwarten, dass die Liste nicht leer ist.
            Assert.IsNotNull(package);
            Assert.IsNotEmpty(package.Cards);
        }

        [Test]
        public void TestUserRegistration()
        {

        }
    }
}