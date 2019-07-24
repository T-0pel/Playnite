using Moq;
using NUnit.Framework;
using Playnite.SDK.Models;
using Playnite.Tests;
using System;
using System.Collections.Generic;
using System.IO;

namespace GogLibrary.Tests
{
    [TestFixture]
    public class GogLibraryTests
    {
        public Mock<GogLibrary> GetGogLibraryMock()
        {
            var gogLib = new Mock<GogLibrary>(MockBehavior.Loose, PlayniteTests.GetTestingApi().Object);

            const string installedGameId = "installedGameId";
            var installedGame = new GameInfo
            {
                InstallDirectory = @"C:\Temp",
                GameId = installedGameId,
                Source = "GOG",
                Name = "installedGameName",
                IsInstalled = true
            };
            var installedGames = new Dictionary<string, GameInfo> { { installedGameId, installedGame } };

            const string uninstalledGameId = "uninstalledGameId";
            var uninstalledGame = new GameInfo
            {
                InstallDirectory = @"C:\Temp",
                GameId = uninstalledGameId,
                Source = "GOG",
                Name = "uninstalledGameName",
                Playtime = 1,
                LastActivity = DateTime.MinValue
            };
            var libraryGames = new List<GameInfo>
            {
                uninstalledGame,
                new GameInfo { GameId = installedGame.GameId, Playtime = 2, LastActivity = DateTime.MaxValue }
            };

            gogLib.Setup(a => a.GetInstalledGames()).Returns(() => installedGames);
            gogLib.Setup(a => a.GetLibraryGames()).Returns(() => libraryGames);

            return gogLib;
        }

        [Test]
        public void GetInstalledGamesRegistry()
        {
            var games = GetGogLibraryMock().Object.GetInstalledGames();
            Assert.AreNotEqual(0, games.Count);
            CollectionAssert.AllItemsAreUnique(games);

            foreach (var game in games.Values)
            {
                Assert.IsFalse(string.IsNullOrEmpty(game.Name));
                Assert.IsFalse(string.IsNullOrEmpty(game.GameId));
                Assert.IsFalse(string.IsNullOrEmpty(game.InstallDirectory));
                Assert.IsTrue(Directory.Exists(game.InstallDirectory));
                Assert.IsNotNull(game.PlayAction);
            }
        }

        [Test]
        public void ImportOnlyInstalled_WithoutStatsTest()
        {

        }
    }
}