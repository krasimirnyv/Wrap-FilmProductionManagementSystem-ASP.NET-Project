namespace Wrap.Services.Tests;

using System.Text;

using Microsoft.AspNetCore.Http;

using Moq;
using NUnit.Framework;
using Newtonsoft.Json;

using Core.Utilities;

[TestFixture]
public class SessionJsonExtensionsTests
{
    [Test]
    public void SetJson_WhenCalled_SerializesAndStoresJsonUnderKey()
    {
        // Arrange
        const string key = "myKey";

        TestDto value = new TestDto
        {
            Name = "John",
            Age = 25
        };

        Mock<ISession> sessionMock = new Mock<ISession>(MockBehavior.Strict);

        byte[]? capturedBytes = null;

        sessionMock
            .Setup(s => s.Set(
                key,
                It.IsAny<byte[]>()))
            .Callback<string, byte[]>((_, bytes) => capturedBytes = bytes);

        // Act
        SessionJsonExtensions.SetJson(sessionMock.Object, key, value);

        // Assert
        Assert.That(capturedBytes, Is.Not.Null);

        string json = Encoding.UTF8.GetString(capturedBytes);

        TestDto? deserialized = JsonConvert.DeserializeObject<TestDto>(json);
        Assert.That(deserialized, Is.Not.Null);
        Assert.That(deserialized.Name, Is.EqualTo("John"));
        Assert.That(deserialized.Age, Is.EqualTo(25));

        sessionMock.Verify(s => s.Set(key, It.IsAny<byte[]>()), Times.Once);
        sessionMock.VerifyNoOtherCalls();
    }

    [Test]
    public void GetJson_WhenKeyDoesNotExist_ReturnsDefault()
    {
        // Arrange
        const string key = "missingKey";

        Mock<ISession> sessionMock = new Mock<ISession>(MockBehavior.Strict);

        sessionMock
            .Setup(s => s.TryGetValue(key, out It.Ref<byte[]>.IsAny!))
            .Returns(false);

        // Act
        TestDto? result = SessionJsonExtensions.GetJson<TestDto>(sessionMock.Object, key);

        // Assert
        Assert.That(result, Is.Null);

        sessionMock.Verify(s => s.TryGetValue(key, out It.Ref<byte[]>.IsAny!), Times.Once);
        sessionMock.VerifyNoOtherCalls();
    }

    [Test]
    public void GetJson_WhenKeyExists_ReturnsDeserializedValue()
    {
        // Arrange
        const string key = "existingKey";

        TestDto original = new TestDto
        {
            Name = "Alice",
            Age = 30
        };

        string json = JsonConvert.SerializeObject(original);
        byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

        Mock<ISession> sessionMock = new Mock<ISession>(MockBehavior.Strict);

        sessionMock
            .Setup(s => s.TryGetValue(key, out It.Ref<byte[]>.IsAny!))
            .Callback(new TryGetValueCallback((string _, out byte[] valueBytes) =>
            {
                valueBytes = jsonBytes;
            }))
            .Returns(true);

        // Act
        TestDto? result = SessionJsonExtensions.GetJson<TestDto>(sessionMock.Object, key);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Name, Is.EqualTo("Alice"));
        Assert.That(result.Age, Is.EqualTo(30));

        sessionMock.Verify(s => s.TryGetValue(key, out It.Ref<byte[]>.IsAny!), Times.Once);
        sessionMock.VerifyNoOtherCalls();
    }

    [Test]
    public void GetJson_WhenStoredJsonIsInvalid_ThrowsJsonException()
    {
        // Arrange
        const string key = "badJsonKey";

        byte[] jsonBytes = "{ not valid json"u8.ToArray();

        Mock<ISession> sessionMock = new Mock<ISession>(MockBehavior.Strict);

        sessionMock
            .Setup(s => s.TryGetValue(key, out It.Ref<byte[]>.IsAny!))
            .Callback(new TryGetValueCallback((string _, out byte[] valueBytes) =>
            {
                valueBytes = jsonBytes;
            }))
            .Returns(true);

        // Act + Assert
        Assert.Throws<JsonReaderException>(() =>
            SessionJsonExtensions.GetJson<TestDto>(sessionMock.Object, key));

        sessionMock.Verify(s => s.TryGetValue(key, out It.Ref<byte[]>.IsAny!), Times.Once);
        sessionMock.VerifyNoOtherCalls();
    }

    // Delegate type for Moq out-parameter callback
    private delegate void TryGetValueCallback(string key, out byte[] value);

    private class TestDto
    {
        public string Name { get; set; } = null!;
        
        public int Age { get; set; }
    }
}