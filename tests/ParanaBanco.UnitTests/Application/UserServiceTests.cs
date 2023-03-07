using Moq;
using ParanaBanco.Application.DTOs;
using ParanaBanco.Application.Services;
using ParanaBanco.Domain.Entities;
using ParanaBanco.Domain.Exceptions;
using ParanaBanco.Domain.Interfaces.Repositories;
using ParanaBanco.Domain.ValueObjects;
using ParanaBanco.UnitTests.Helper;
using Xunit;

namespace ParanaBanco.UnitTests.Application
{
    public class UserServiceTests
    {
        [Fact]
        public async void GetById_Return_User()
        {
            //Arrange
            var user = new User("Unit Test", new Email("test1@gmail.com"));

            var userRepositoryMock = new Mock<IUserRepository>();
            var mapper = TestMapper.GetMapper();

            userRepositoryMock.Setup(f => f.GetById(1, false)).ReturnsAsync(user);

            var userService = new UserService(userRepositoryMock.Object, mapper);

            //Act
            var resultado = await userService.GetById(1, false);

            //Assert  
            Assert.NotNull(resultado);
            Assert.IsAssignableFrom<UserDTO>(resultado);

            userRepositoryMock.Verify(c => c.GetById(It.IsAny<int>(), It.IsAny<bool>()), Times.Once());
        }

        [Fact]
        public async void GetById_Return_DeletedUser()
        {
            //Arrange
            var user = new User("Unit Test", new Email("test1@gmail.com"));

            user.Delete();

            Assert.True(user.IsDeleted);
            Assert.NotNull(user.LastModifiedAt);

            var userRepositoryMock = new Mock<IUserRepository>();
            var mapper = TestMapper.GetMapper();

            userRepositoryMock.Setup(f => f.GetById(1, true)).ReturnsAsync(user);

            var userService = new UserService(userRepositoryMock.Object, mapper);

            //Act
            var resultado = await userService.GetById(1, true);

            //Assert  
            Assert.NotNull(resultado);
            Assert.IsAssignableFrom<UserDTO>(resultado);
            Assert.True(user.IsDeleted);
            Assert.NotNull(user.LastModifiedAt);

            userRepositoryMock.Verify(c => c.GetById(It.IsAny<int>(), It.IsAny<bool>()), Times.Once());
        }

        [Fact]
        public async void GetById_Return_RestoredUser()
        {
            //Arrange
            var user = new User("Unit Test", new Email("test1@gmail.com"));

            user.Delete();

            Assert.True(user.IsDeleted);
            Assert.NotNull(user.LastModifiedAt);

            user.Restore();
            Assert.False(user.IsDeleted);

            var userRepositoryMock = new Mock<IUserRepository>();
            var mapper = TestMapper.GetMapper();

            userRepositoryMock.Setup(f => f.GetById(1, true)).ReturnsAsync(user);

            var userService = new UserService(userRepositoryMock.Object, mapper);

            //Act
            var resultado = await userService.GetById(1, true);

            //Assert  
            Assert.NotNull(resultado);
            Assert.IsAssignableFrom<UserDTO>(resultado);
            Assert.False(user.IsDeleted);
            Assert.NotNull(user.LastModifiedAt);

            userRepositoryMock.Verify(c => c.GetById(It.IsAny<int>(), It.IsAny<bool>()), Times.Once());
        }

        [Fact]
        public async void GetById_Return_Null()
        {
            //Arrange
            var userRepositoryMock = new Mock<IUserRepository>();
            var mapper = TestMapper.GetMapper();

            userRepositoryMock.Setup(f => f.GetById(1, true)).ReturnsAsync(() => null);

            var userService = new UserService(userRepositoryMock.Object, mapper);

            //Act
            var resultado = await userService.GetById(1, true);

            //Assert  
            Assert.Null(resultado);

            userRepositoryMock.Verify(c => c.GetById(It.IsAny<int>(), It.IsAny<bool>()), Times.Once());
        }

        [Fact]
        public async void GetByEmail_Return_User()
        {
            //Arrange
            var email = "test@gmail.com";
            var user = new User("Unit Test", new Email("test@gmail.com"));

            var userRepositoryMock = new Mock<IUserRepository>();
            var mapper = TestMapper.GetMapper();

            userRepositoryMock.Setup(f => f.GetByEmail(It.IsAny<string>())).ReturnsAsync(user);

            var userService = new UserService(userRepositoryMock.Object, mapper);

            //Act
            var resultado = await userService.GetByEmail(email);

            //Assert  
            Assert.NotNull(resultado);
            Assert.IsAssignableFrom<UserDTO>(resultado);

            userRepositoryMock.Verify(c => c.GetByEmail(It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public async void GetAll_Return_NotDeletedUser()
        {
            //Arrange
            var usersList = new List<User>()
            {
                new User("Unit Test 1", new Email("test1@gmail.com")),
                new User("Unit Test 2", new Email("test2@gmail.com")),
                new User("Unit Test 3", new Email("test3@gmail.com")),
            };

            var userRepositoryMock = new Mock<IUserRepository>();
            var mapper = TestMapper.GetMapper();

            userRepositoryMock.Setup(f => f.GetAll(It.IsAny<bool>())).ReturnsAsync(usersList);

            var userService = new UserService(userRepositoryMock.Object, mapper);

            //Act
            var result = await userService.GetAll(false);

            //Assert
            Assert.True(result.Any());
            Assert.IsAssignableFrom<IEnumerable<UserDTO>>(result);

            userRepositoryMock.Verify(c => c.GetAll(It.IsAny<bool>()), Times.Once());
        }

        [Fact]
        public async void GetAll_Return_AllUser()
        {
            //Arrange
            var usersList = new List<User>()
            {
                new User("Unit Test 1", new Email("test1@gmail.com")),
                new User("Unit Test 2", new Email("test2@gmail.com")),
                new User("Unit Test 3", new Email("test3@gmail.com")),
            };

            Random random = new Random();
            usersList[random.Next(0, 2)].Delete();

            var userRepositoryMock = new Mock<IUserRepository>();
            var mapper = TestMapper.GetMapper();

            userRepositoryMock.Setup(f => f.GetAll(It.IsAny<bool>())).ReturnsAsync(usersList);

            var userService = new UserService(userRepositoryMock.Object, mapper);

            //Act
            var result = await userService.GetAll(true);

            //Assert
            Assert.True(result.Any());
            Assert.True(result.Count() == usersList.Count());
            Assert.IsAssignableFrom<IEnumerable<UserDTO>>(result);

            userRepositoryMock.Verify(c => c.GetAll(It.IsAny<bool>()), Times.Once());
        }

        [Fact]
        public async void GetOnlyDeleted_Return_DeletedUsers()
        {
            //Arrange
            var usersList = new List<User>()
            {
                new User("Unit Test 1", new Email("test1@gmail.com")),
                new User("Unit Test 2", new Email("test2@gmail.com")),
                new User("Unit Test 3", new Email("test3@gmail.com")),
            };

            foreach (var user in usersList) user.Delete();

            var userRepositoryMock = new Mock<IUserRepository>();
            var mapper = TestMapper.GetMapper();

            userRepositoryMock.Setup(f => f.GetOnlyDeleted()).ReturnsAsync(usersList);

            var userService = new UserService(userRepositoryMock.Object, mapper);

            //Act
            var result = await userService.GetOnlyDeleted();

            //Assert
            Assert.True(result.Any());
            Assert.True(result.Count() == usersList.Count());
            Assert.IsAssignableFrom<IEnumerable<UserDTO>>(result);

            userRepositoryMock.Verify(c => c.GetOnlyDeleted(), Times.Once());
        }

        [Fact]
        public async void Add_Verify_IfAddIsExecuted()
        {
            //Arrange
            int userId = 1;
            var userDTO = new UserDTO()
            {
                FullName = "Unit Test",
                Email = "test@gmail.com"
            };

            var userRepositoryMock = new Mock<IUserRepository>();
            var mapper = TestMapper.GetMapper();

            userRepositoryMock.Setup(f => f.GetByEmail(It.IsAny<string>())).ReturnsAsync(() => null);
            userRepositoryMock.Setup(f => f.Add(It.IsAny<User>())).Returns(Task.FromResult(userId));

            var userService = new UserService(userRepositoryMock.Object, mapper);

            //Act
            int id = await userService.Add(userDTO);

            //Assert  
            Assert.True(id > 0);

            userRepositoryMock.Verify(c => c.GetByEmail(It.IsAny<string>()), Times.Once());
            userRepositoryMock.Verify(c => c.Add(It.IsAny<User>()), Times.Once());
        }

        [Fact]
        public async void Add_Return_EmailAlreadyRegisteredException()
        {
            //Arrange
            int userId = 1;
            var user = new User("Unit Test", new Email("test@gmail.com"));
            var userDTO = new UserDTO()
            {
                FullName = "Unit Test",
                Email = "test@gmail.com"
            };

            var userRepositoryMock = new Mock<IUserRepository>();
            var mapper = TestMapper.GetMapper();

            userRepositoryMock.Setup(f => f.GetByEmail(It.IsAny<string>())).ReturnsAsync(user);
            userRepositoryMock.Setup(f => f.Add(It.IsAny<User>())).Returns(Task.FromResult(userId));

            var userService = new UserService(userRepositoryMock.Object, mapper);

            //Act
            var result = userService.Add(userDTO);

            //Assert  
            await Assert.ThrowsAsync<EmailAlreadyRegisteredException>(async () => await result);
        }

        [Fact]
        public async void Update_Verify_IfUpdateIsExecuted()
        {
            //Arrange
            var user = new User("Unit Test", new Email("test@gmail.com"));
            var userDTO = new UserDTO()
            {
                Id = 1,
                FullName = "Unit Test Update",
                Email = "test@gmail.com"
            };

            var userRepositoryMock = new Mock<IUserRepository>();
            var mapper = TestMapper.GetMapper();

            userRepositoryMock.Setup(f => f.GetById(1, false)).ReturnsAsync(user);
            userRepositoryMock.Setup(f => f.GetByEmail(It.IsAny<string>())).ReturnsAsync(() => null);
            userRepositoryMock.Setup(f => f.Update(It.IsAny<User>())).Returns(Task.CompletedTask);

            var userService = new UserService(userRepositoryMock.Object, mapper);

            //Act
            await userService.Update(userDTO);

            //Assert
            Assert.Equal(user.FullName, userDTO.FullName);
            Assert.NotNull(user.LastModifiedAt);

            userRepositoryMock.Verify(c => c.GetById(It.IsAny<int>(), It.IsAny<bool>()), Times.Once());
            userRepositoryMock.Verify(c => c.GetByEmail(It.IsAny<string>()), Times.Once());
            userRepositoryMock.Verify(c => c.Update(It.IsAny<User>()), Times.Once());
        }

        [Fact]
        public async void Update_Return_UserNotFoundException()
        {
            //Arrange
            var user = new User("Unit Test", new Email("test@gmail.com"));
            var userDTO = new UserDTO()
            {
                Id = 1,
                FullName = "Unit Test Update",
                Email = "test@gmail.com"
            };

            var userRepositoryMock = new Mock<IUserRepository>();
            var mapper = TestMapper.GetMapper();

            userRepositoryMock.Setup(f => f.GetById(1, false)).ReturnsAsync(() => null);

            var userService = new UserService(userRepositoryMock.Object, mapper);

            //Act
            var result = userService.Update(userDTO);

            //Assert
            await Assert.ThrowsAsync<UserNotFoundException>(async () => await result);
        }

        [Fact]
        public async void Update_Return_EmailAlreadyRegisteredException()
        {
            //Arrange
            var user = new User("Unit Test", new Email("test@gmail.com"));
            var userDTO = new UserDTO()
            {
                Id = 1,
                FullName = "Unit Test Update",
                Email = "test@gmail.com"
            };

            var userRepositoryMock = new Mock<IUserRepository>();
            var mapper = TestMapper.GetMapper();

            userRepositoryMock.Setup(f => f.GetById(1, false)).ReturnsAsync(user);
            userRepositoryMock.Setup(f => f.GetByEmail(It.IsAny<string>())).ReturnsAsync(user);

            var userService = new UserService(userRepositoryMock.Object, mapper);

            //Act
            var result = userService.Update(userDTO);

            //Assert
            await Assert.ThrowsAsync<EmailAlreadyRegisteredException>(async () => await result);
        }

        [Fact]
        public async void Delete_Verify_IfDeleteIsExecuted()
        {
            //Arrange
            var user = new User("Unit Test", new Email("test@gmail.com"));
            var userDTO = new UserDTO()
            {
                Id = 1,
                FullName = "Unit Test Update",
                Email = "test@gmail.com"
            };

            var userRepositoryMock = new Mock<IUserRepository>();
            var mapper = TestMapper.GetMapper();

            userRepositoryMock.Setup(f => f.GetById(1, false)).ReturnsAsync(user);
            userRepositoryMock.Setup(f => f.Delete(It.IsAny<User>())).Returns(Task.CompletedTask);

            var userService = new UserService(userRepositoryMock.Object, mapper);

            //Act
            await userService.Delete(userDTO);

            //Assert
            Assert.True(user.IsDeleted);
            Assert.NotNull(user.LastModifiedAt);

            userRepositoryMock.Verify(c => c.GetById(It.IsAny<int>(), It.IsAny<bool>()), Times.Once());
            userRepositoryMock.Verify(c => c.Update(It.IsAny<User>()), Times.Once());
        }

        [Fact]
        public async void Restore_Verify_IfRestoreIsExecuted()
        {
            //Arrange
            var user = new User("Unit Test", new Email("test@gmail.com"));
            var userDTO = new UserDTO()
            {
                Id = 1,
                FullName = "Unit Test Update",
                Email = "test@gmail.com"
            };

            var userRepositoryMock = new Mock<IUserRepository>();
            var mapper = TestMapper.GetMapper();

            userRepositoryMock.Setup(f => f.GetById(1, true)).ReturnsAsync(user);
            userRepositoryMock.Setup(f => f.Update(It.IsAny<User>())).Returns(Task.CompletedTask);

            var userService = new UserService(userRepositoryMock.Object, mapper);

            //Act
            await userService.Restore(userDTO);

            //Assert
            Assert.False(user.IsDeleted);
            Assert.NotNull(user.LastModifiedAt);

            userRepositoryMock.Verify(c => c.GetById(It.IsAny<int>(), It.IsAny<bool>()), Times.Once());
            userRepositoryMock.Verify(c => c.Update(It.IsAny<User>()), Times.Once());
        }

        [Fact]
        public async void Restore_Return_UserNotFoundException()
        {
            //Arrange
            var user = new User("Unit Test", new Email("test@gmail.com"));
            var userDTO = new UserDTO()
            {
                Id = 1,
                FullName = "Unit Test Update",
                Email = "test@gmail.com"
            };

            var userRepositoryMock = new Mock<IUserRepository>();
            var mapper = TestMapper.GetMapper();

            userRepositoryMock.Setup(f => f.GetById(1, true)).ReturnsAsync(() => null);

            var userService = new UserService(userRepositoryMock.Object, mapper);

            //Act
            var result = userService.Restore(userDTO);

            //Assert
            await Assert.ThrowsAsync<UserNotFoundException>(async () => await result);
        }
    }
}
