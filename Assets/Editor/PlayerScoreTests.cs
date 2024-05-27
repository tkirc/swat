using NUnit.Framework;
using UnityEngine;

public class PlayerScoreTests
{
    private GameObject _playerGameObject;
    private PlayerScore _playerScore;

    [SetUp]
    public void SetUp()
    {
        _playerGameObject = new GameObject("Player");
        _playerScore = new PlayerScore(_playerGameObject);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_playerGameObject);
    }

    [Test]
    public void PlayerScore_InitialScoreIsZero()
    {
        Assert.AreEqual(0, _playerScore.getScore());
    }

    [Test]
    public void PlayerScore_AddScore_IncreasesScore()
    {
        _playerScore.addScore(10);

        Assert.AreEqual(10, _playerScore.getScore());
    }

    [Test]
    public void PlayerScore_ReduceScore_DecreasesScore()
    {
        _playerScore.addScore(10);

        _playerScore.reduceScore(5);

        Assert.AreEqual(5, _playerScore.getScore());
    }

    [Test]
    public void PlayerScore_GetPlayer_ReturnsCorrectPlayer()
    {
        Assert.AreEqual(_playerGameObject, _playerScore.getPlayer());
    }

    [Test]
    public void PlayerScore_GetTargetEventChecker_ReturnsNullIfNotPresent()
    {
        Assert.IsNull(_playerScore.getTargetEventChecker());
    }

    [Test]
    public void PlayerScore_GetTargetEventChecker_ReturnsComponentIfPresent()
    {
        var targetEventChecker = _playerGameObject.AddComponent<TargetEventChecker>();

        var result = _playerScore.getTargetEventChecker();

        Assert.AreEqual(targetEventChecker, result);
    }
}
