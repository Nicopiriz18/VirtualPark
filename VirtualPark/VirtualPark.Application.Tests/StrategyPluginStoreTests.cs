// <copyright file="StrategyPluginStoreTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.IO;
using System.Text;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using VirtualPark.Application.Scoring;

namespace VirtualPark.Application.Tests;

[TestClass]
public class StrategyPluginStoreTests
{
    [TestMethod]
    public async Task StoreAsync_NullFile_ThrowsArgumentNullException()
    {
        var pluginsPath = Path.Combine(Path.GetTempPath(), "StrategyPluginStoreTests", Guid.NewGuid().ToString());
        var store = new StrategyPluginStore(pluginsPath);

        await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => store.StoreAsync(null!));
    }

    [TestMethod]
    public async Task StoreAsync_EmptyFile_ThrowsArgumentException()
    {
        var pluginsPath = Path.Combine(Path.GetTempPath(), "StrategyPluginStoreTests", Guid.NewGuid().ToString());
        var store = new StrategyPluginStore(pluginsPath);

        var fileMock = new Mock<IFormFile>(MockBehavior.Strict);
        fileMock.Setup(f => f.Length).Returns(0);
        fileMock.Setup(f => f.FileName).Returns("Empty.dll");

        await Assert.ThrowsExceptionAsync<ArgumentException>(() => store.StoreAsync(fileMock.Object));
    }

    [TestMethod]
    public async Task StoreAsync_InvalidFileName_ThrowsArgumentException()
    {
        var pluginsPath = Path.Combine(Path.GetTempPath(), "StrategyPluginStoreTests", Guid.NewGuid().ToString());
        var store = new StrategyPluginStore(pluginsPath);

        var fileMock = new Mock<IFormFile>(MockBehavior.Strict);
        var content = Encoding.UTF8.GetBytes("dummy");
        fileMock.Setup(f => f.Length).Returns(content.Length);
        fileMock.Setup(f => f.FileName).Returns(string.Empty);
        fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Returns<Stream, CancellationToken>((s, token) => s.WriteAsync(content, 0, content.Length, token));

        await Assert.ThrowsExceptionAsync<ArgumentException>(() => store.StoreAsync(fileMock.Object));
    }

    [TestMethod]
    public async Task StoreAsync_InvalidExtension_ThrowsArgumentException()
    {
        var pluginsPath = Path.Combine(Path.GetTempPath(), "StrategyPluginStoreTests", Guid.NewGuid().ToString());
        var store = new StrategyPluginStore(pluginsPath);

        var fileMock = new Mock<IFormFile>(MockBehavior.Strict);
        var content = Encoding.UTF8.GetBytes("dummy");
        fileMock.Setup(f => f.Length).Returns(content.Length);
        fileMock.Setup(f => f.FileName).Returns("file.txt");
        fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Returns<Stream, CancellationToken>((s, token) => s.WriteAsync(content, 0, content.Length, token));

        await Assert.ThrowsExceptionAsync<ArgumentException>(() => store.StoreAsync(fileMock.Object));
    }

    [TestMethod]
    public async Task StoreAsync_SavesFileSuccessfully()
    {
        var pluginsPath = Path.Combine(Path.GetTempPath(), "StrategyPluginStoreTests", Guid.NewGuid().ToString());
        var store = new StrategyPluginStore(pluginsPath);

        var content = Encoding.UTF8.GetBytes("dummy-plugin");
        var fileName = "CustomStrategy.dll";

        var fileMock = new Mock<IFormFile>(MockBehavior.Strict);
        fileMock.Setup(f => f.Length).Returns(content.Length);
        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Returns<Stream, CancellationToken>((s, token) => s.WriteAsync(content, 0, content.Length, token));

        try
        {
            await store.StoreAsync(fileMock.Object);

            var savedPath = Path.Combine(pluginsPath, fileName);
            Assert.IsTrue(File.Exists(savedPath), "Expected plugin file to be created");
            var savedContent = File.ReadAllBytes(savedPath);
            CollectionAssert.AreEqual(content, savedContent);
        }
        finally
        {
            if (Directory.Exists(pluginsPath))
            {
                Directory.Delete(pluginsPath, true);
            }
        }
    }
}
