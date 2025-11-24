// <copyright file="UserTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;
using VirtualPark.Domain.Enums;

namespace VirtualPark.Domain.Tests;

[TestClass]
public class UserTests
{
    private User? user;

    [TestInitialize]
    public void Setup()
    {
        this.user = new User("Juan", "Perez", "email", "123", RoleEnum.Operator);
    }

    [TestMethod]
    public void UserConstructorTestAll()
    {
        Assert.AreEqual("Juan", this.user.Name);
        Assert.AreEqual("Perez", this.user.Surname);
        Assert.AreEqual("email", this.user.Email);
        Assert.AreEqual("123", this.user.Password);
        Assert.AreEqual(1, this.user.Roles.Count);
        Assert.IsTrue(this.user.Roles.Contains(RoleEnum.Operator));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void UserConstructorTestNullName()
    {
        _ = new User(null!, "Perez", "email", "123", RoleEnum.Operator);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void UserConstructorTestNullSurname()
    {
        _ = new User("Juan", null!, "email", "123", RoleEnum.Operator);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void UserConstructorTestNullEmail()
    {
        _ = new User("Juan", "n", null!, "123", RoleEnum.Operator);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void UserConstructorTestNullPassword()
    {
        _ = new User("Juan", "n", "email", null!, RoleEnum.Operator);
    }
}
