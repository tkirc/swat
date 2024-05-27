using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using TMPro;

public class LobbyInfoTests
{
    private GameObject _lobbyInfoGameObject;
    private LobbyInfo _lobbyInfo;
    private TextMeshProUGUI _lobbyInfoText;
    private TextMeshProUGUI _lobbyAdditionalInfoText;

    [SetUp]
    public void SetUp()
    {
        _lobbyInfoGameObject = new GameObject("LobbyInfo");

        _lobbyInfo = _lobbyInfoGameObject.AddComponent<LobbyInfo>();

        GameObject lobbyInfoTextGameObject = new GameObject("LobbyInfoText");
        GameObject lobbyAdditionalInfoTextGameObject = new GameObject("LobbyAdditionalInfoText");

        _lobbyInfoText = lobbyInfoTextGameObject.AddComponent<TextMeshProUGUI>();
        _lobbyAdditionalInfoText = lobbyAdditionalInfoTextGameObject.AddComponent<TextMeshProUGUI>();

        lobbyInfoTextGameObject.transform.SetParent(_lobbyInfoGameObject.transform);
        lobbyAdditionalInfoTextGameObject.transform.SetParent(_lobbyInfoGameObject.transform);
        
        var lobbyInfoField = typeof(LobbyInfo).GetField("_lobbyInfo", BindingFlags.NonPublic | BindingFlags.Instance);
        lobbyInfoField.SetValue(_lobbyInfo, _lobbyInfoText);

        var lobbyAdditionalInfoField = typeof(LobbyInfo).GetField("_lobbyAdditionalInfo", BindingFlags.NonPublic | BindingFlags.Instance);
        lobbyAdditionalInfoField.SetValue(_lobbyInfo, _lobbyAdditionalInfoText);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_lobbyInfoGameObject);
    }

    [Test]
    public void SetLobbyInfo_SetsTextCorrectly()
    {
        string testText = "Test Lobby";

        _lobbyInfo.SetLobbyInfo(testText);

        Assert.AreEqual("Lobby: " + testText, _lobbyInfoText.text);
    }

    [Test]
    public void SetLobbyInfo_SetsTextToNone_WhenNull()
    {
        _lobbyInfo.SetLobbyInfo(null);

        Assert.AreEqual("Lobby: none", _lobbyInfoText.text);
    }

    [Test]
    public void SetAdditionalInfo_SetsTextCorrectly()
    {
        string testText = "Additional Info";

        _lobbyInfo.SetAdditionalInfo(testText);

        Assert.AreEqual(testText, _lobbyAdditionalInfoText.text);
    }
}
