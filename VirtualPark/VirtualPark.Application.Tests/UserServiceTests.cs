// <copyright file="UserServiceTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Moq;
using VirtualPark.Application.Users;
using VirtualPark.Domain;
using VirtualPark.Domain.Enums;
using VirtualPark.Domain.Interfaces.Repositories;
using VirtualPark.Domain.Interfaces.Security;

namespace VirtualPark.Application.Tests;

[TestClass]
public class UserServiceTests
{
    [TestMethod]
    public void GetAll_ReturnsUsersFromRepository()
    {
        var expectedUsers = new List<User>
        {
            new("Ana", "García", "ana@example.com", "pass1", RoleEnum.Operator),
            new("Luis", "Pérez", "luis@example.com", "pass2", RoleEnum.Administrator),
        };
        var repository = new Mock<IUserRepository>(MockBehavior.Strict);
        repository.Setup(r => r.GetAll()).Returns(expectedUsers);
        var passwordHasher = new Mock<IPasswordHasher>();
        var service = new UserService(repository.Object, passwordHasher.Object);

        var users = service.GetAll();

        Assert.AreSame(expectedUsers, users);
        repository.VerifyAll();
    }

    [TestMethod]
    public void GetById_ReturnsUser_WhenRepositoryFindsIt()
    {
        var user = new User("Sofía", "López", "sofia@example.com", "pass", RoleEnum.Operator);
        var repository = new Mock<IUserRepository>(MockBehavior.Strict);
        var userId = Guid.NewGuid();
        repository.Setup(r => r.GetById(userId)).Returns(user);
        var passwordHasher = new Mock<IPasswordHasher>();
        var service = new UserService(repository.Object, passwordHasher.Object);

        var result = service.GetById(userId);

        Assert.AreSame(user, result);
        repository.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void GetById_Throws_WhenRepositoryReturnsNull()
    {
        var repository = new Mock<IUserRepository>(MockBehavior.Strict);
        var userId = Guid.NewGuid();
        repository.Setup(r => r.GetById(userId)).Returns((User?)null);
        var passwordHasher = new Mock<IPasswordHasher>();
        var service = new UserService(repository.Object, passwordHasher.Object);

        _ = service.GetById(userId);
        repository.VerifyAll();
    }

    [TestMethod]
    public void Create_ReturnsUserReturnedByRepository()
    {
        var newUser = new User("María", "Suárez", "maria@example.com", "pass", RoleEnum.Administrator);
        var repository = new Mock<IUserRepository>(MockBehavior.Strict);
        var passwordHasher = new Mock<IPasswordHasher>();
        passwordHasher.Setup(p => p.Hash(It.IsAny<string>())).Returns((string pwd) => $"hashed_{pwd}");
        repository.Setup(r => r.Add(newUser)).Returns(newUser);
        var service = new UserService(repository.Object, passwordHasher.Object);

        var created = service.Create(newUser);

        Assert.AreSame(newUser, created);
        repository.Verify(r => r.Add(newUser), Times.Once);
        repository.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Create_Throws_WhenUserIsNull()
    {
        var repository = new Mock<IUserRepository>(MockBehavior.Strict);
        var passwordHasher = new Mock<IPasswordHasher>();
        var service = new UserService(repository.Object, passwordHasher.Object);

        _ = service.Create(null!);
        repository.VerifyAll();
    }

    [TestMethod]
    public void Update_DelegatesToRepository()
    {
        var existingUser = new User("Pedro", "Martínez", "pedro@example.com", "pass", RoleEnum.Operator);
        var updatedUser = new User("Pedro", "Martínez Updated", "pedro@example.com", "newpass", RoleEnum.Operator);
        var id = existingUser.Id;
        var repository = new Mock<IUserRepository>(MockBehavior.Strict);
        var passwordHasher = new Mock<IPasswordHasher>();
        passwordHasher.Setup(p => p.Hash(It.IsAny<string>())).Returns((string pwd) => $"hashed_{pwd}");
        repository.Setup(r => r.GetById(id)).Returns(existingUser);
        repository.Setup(r => r.Update(id, existingUser)).Returns(true);
        var service = new UserService(repository.Object, passwordHasher.Object);

        var updated = service.Update(id, updatedUser.Name!, updatedUser.Surname!, updatedUser.Email!, "newpass");

        Assert.IsTrue(updated);
        repository.Verify(r => r.GetById(id), Times.Once);
        repository.Verify(r => r.Update(id, existingUser), Times.Once);
        repository.VerifyAll();
    }

    [TestMethod]
    public void Update_ReturnsFalse_WhenRepositoryFails()
    {
        var existingUser = new User("Pedro", "Martínez", "pedro@example.com", "pass", RoleEnum.Operator);
        var id = existingUser.Id;
        var repository = new Mock<IUserRepository>(MockBehavior.Strict);
        var passwordHasher = new Mock<IPasswordHasher>();
        passwordHasher.Setup(p => p.Hash(It.IsAny<string>())).Returns((string pwd) => $"hashed_{pwd}");
        repository.Setup(r => r.GetById(id)).Returns(existingUser);
        repository.Setup(r => r.Update(id, existingUser)).Returns(false);
        var service = new UserService(repository.Object, passwordHasher.Object);

        var updated = service.Update(id, "Pedro", "Martínez Updated", "pedro@example.com", "newpass");

        Assert.IsFalse(updated);
        repository.VerifyAll();
    }

    [TestMethod]
    public void AssignRole_AssignsRoleToUser_WhenUserExists()
    {
        // Arrange
        var user = new User("Carlos", "Rodríguez", "carlos@example.com", "pass", RoleEnum.Operator);
        var userId = user.Id;
        var roleToAssign = RoleEnum.Administrator;

        var repository = new Mock<IUserRepository>(MockBehavior.Strict);
        repository.Setup(r => r.GetById(userId)).Returns(user);
        repository.Setup(r => r.Update(userId, user)).Returns(true);
        var passwordHasher = new Mock<IPasswordHasher>();

        var service = new UserService(repository.Object, passwordHasher.Object);

        // Act
        service.AssignRole(userId, roleToAssign);

        // Assert
        Assert.IsTrue(user.Roles.Contains(roleToAssign));
        repository.Verify(r => r.GetById(userId), Times.Once);
        repository.Verify(r => r.Update(userId, user), Times.Once);
        repository.VerifyAll();
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void AssignRole_ThrowsKeyNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roleToAssign = RoleEnum.Operator;

        var repository = new Mock<IUserRepository>(MockBehavior.Strict);
        repository.Setup(r => r.GetById(userId)).Returns((User?)null);
        var passwordHasher = new Mock<IPasswordHasher>();

        var service = new UserService(repository.Object, passwordHasher.Object);

        // Act
        service.AssignRole(userId, roleToAssign);

        // Assert is handled by ExpectedException
    }

    [TestMethod]
    public void AssignRole_DoesNotDuplicateRole_WhenRoleAlreadyExists()
    {
        // Arrange
        var user = new User("Laura", "Fernández", "laura@example.com", "pass", RoleEnum.Operator);
        var userId = user.Id;
        var roleToAssign = RoleEnum.Operator;

        var repository = new Mock<IUserRepository>(MockBehavior.Strict);
        repository.Setup(r => r.GetById(userId)).Returns(user);
        repository.Setup(r => r.Update(userId, user)).Returns(true);
        var passwordHasher = new Mock<IPasswordHasher>();

        var service = new UserService(repository.Object, passwordHasher.Object);

        // Act
        service.AssignRole(userId, roleToAssign);

        // Assert - el rol no debe estar duplicado
        var rolesCount = user.Roles.Count(r => r == roleToAssign);
        Assert.AreEqual(1, rolesCount);
        repository.VerifyAll();
    }

    [TestMethod]
    public void AssignRole_CanAssignMultipleRoles()
    {
        // Arrange
        var user = new User("Roberto", "Gómez", "roberto@example.com", "pass", RoleEnum.Operator);
        var userId = user.Id;

        var repository = new Mock<IUserRepository>(MockBehavior.Strict);
        repository.Setup(r => r.GetById(userId)).Returns(user);
        repository.Setup(r => r.Update(userId, user)).Returns(true);
        var passwordHasher = new Mock<IPasswordHasher>();

        var service = new UserService(repository.Object, passwordHasher.Object);

        // Act
        service.AssignRole(userId, RoleEnum.Administrator);

        // Assert
        Assert.AreEqual(2, user.Roles.Count);
        Assert.IsTrue(user.Roles.Contains(RoleEnum.Administrator));
        Assert.IsTrue(user.Roles.Contains(RoleEnum.Operator));
        repository.VerifyAll();
    }

    [TestMethod]
    public void Create_WithEmptyPassword_DoesNotHashPassword()
    {
        // Arrange
        var newUser = new User("Test", "User", "test@example.com", "temppass", RoleEnum.Operator);
        newUser.Password = string.Empty; // Set to empty after construction to bypass validation
        var repository = new Mock<IUserRepository>(MockBehavior.Strict);
        var passwordHasher = new Mock<IPasswordHasher>();
        repository.Setup(r => r.Add(newUser)).Returns(newUser);
        var service = new UserService(repository.Object, passwordHasher.Object);

        // Act
        var created = service.Create(newUser);

        // Assert
        Assert.AreSame(newUser, created);
        passwordHasher.Verify(p => p.Hash(It.IsAny<string>()), Times.Never);
        repository.VerifyAll();
    }

    [TestMethod]
    public void Update_WithoutPasswordChange_DoesNotUpdatePassword()
    {
        // Arrange
        var existingUser = new User("Pedro", "Martínez", "pedro@example.com", "oldHashedPass", RoleEnum.Operator);
        var id = existingUser.Id;
        var originalPassword = existingUser.Password;
        var repository = new Mock<IUserRepository>(MockBehavior.Strict);
        var passwordHasher = new Mock<IPasswordHasher>();
        repository.Setup(r => r.GetById(id)).Returns(existingUser);
        repository.Setup(r => r.Update(id, existingUser)).Returns(true);
        var service = new UserService(repository.Object, passwordHasher.Object);

        // Act
        var updated = service.Update(id, "Pedro", "Martínez Updated", "pedro@example.com", null);

        // Assert
        Assert.IsTrue(updated);
        Assert.AreEqual(originalPassword, existingUser.Password);
        passwordHasher.Verify(p => p.Hash(It.IsAny<string>()), Times.Never);
        repository.VerifyAll();
    }

    [TestMethod]
    public void Update_WithWhitespacePassword_DoesNotUpdatePassword()
    {
        // Arrange
        var existingUser = new User("Pedro", "Martínez", "pedro@example.com", "oldHashedPass", RoleEnum.Operator);
        var id = existingUser.Id;
        var originalPassword = existingUser.Password;
        var repository = new Mock<IUserRepository>(MockBehavior.Strict);
        var passwordHasher = new Mock<IPasswordHasher>();
        repository.Setup(r => r.GetById(id)).Returns(existingUser);
        repository.Setup(r => r.Update(id, existingUser)).Returns(true);
        var service = new UserService(repository.Object, passwordHasher.Object);

        // Act
        var updated = service.Update(id, "Pedro", "Martínez", "pedro@example.com", "   ");

        // Assert
        Assert.IsTrue(updated);
        Assert.AreEqual(originalPassword, existingUser.Password);
        passwordHasher.Verify(p => p.Hash(It.IsAny<string>()), Times.Never);
        repository.VerifyAll();
    }

    [TestMethod]
    public void AssignRole_ConvertsUserToVisitor_WithCustomMembershipLevel()
    {
        // Arrange
        var user = new User("Carlos", "Rodríguez", "carlos@example.com", "pass", RoleEnum.Operator);
        var userId = user.Id;
        var birthDate = new DateTime(1990, 5, 15);
        var membershipLevel = MembershipLevel.Premium;
        var visitor = new Visitor("Carlos", "Rodríguez", "carlos@example.com", "pass", birthDate, membershipLevel, Guid.NewGuid());

        var repository = new Mock<IUserRepository>(MockBehavior.Strict);
        repository.Setup(r => r.GetById(userId)).Returns(user);
        repository.Setup(r => r.ConvertToVisitor(userId, user, birthDate, membershipLevel, It.IsAny<Guid>())).Returns(visitor);
        var passwordHasher = new Mock<IPasswordHasher>();

        var service = new UserService(repository.Object, passwordHasher.Object);

        // Act
        service.AssignRole(userId, RoleEnum.Visitor, birthDate, membershipLevel);

        // Assert
        repository.Verify(r => r.ConvertToVisitor(userId, user, birthDate, membershipLevel, It.IsAny<Guid>()), Times.Once);
        repository.VerifyAll();
    }

    [TestMethod]
    public void AssignRole_WhenUserAlreadyVisitor_JustAssignsRole()
    {
        // Arrange
        var birthDate = new DateTime(1990, 5, 15);
        var visitor = new Visitor("Ana", "García", "ana@example.com", "pass", birthDate, MembershipLevel.Standard, Guid.NewGuid());
        var userId = visitor.Id;

        var repository = new Mock<IUserRepository>(MockBehavior.Strict);
        repository.Setup(r => r.GetById(userId)).Returns(visitor);
        repository.Setup(r => r.Update(userId, visitor)).Returns(true);
        var passwordHasher = new Mock<IPasswordHasher>();

        var service = new UserService(repository.Object, passwordHasher.Object);

        // Act
        service.AssignRole(userId, RoleEnum.Visitor);

        // Assert
        Assert.IsTrue(visitor.Roles.Contains(RoleEnum.Visitor));
        repository.Verify(r => r.Update(userId, visitor), Times.Once);
        repository.Verify(r => r.ConvertToVisitor(It.IsAny<Guid>(), It.IsAny<User>(), It.IsAny<DateTime>(), It.IsAny<MembershipLevel>(), It.IsAny<Guid>()), Times.Never);
        repository.VerifyAll();
    }
}
