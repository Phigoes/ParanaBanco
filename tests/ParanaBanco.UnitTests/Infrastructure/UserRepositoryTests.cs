using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using ParanaBanco.Domain.Entities;
using ParanaBanco.Domain.ValueObjects;
using ParanaBanco.Infrastructure;
using ParanaBanco.Infrastructure.Persistence.Repositories;
using ParanaBanco.UnitTests.Helper;
using Xunit;

namespace ParanaBanco.UnitTests.Infrastructure
{
    public class UserRepositoryTests
    {
        [Fact]
        public async void GetById_Return_User()
        {
            //Arrange
            var user = new User("Unit Test", new Email("test@gmail.com"));

            var dbContextMock = new Mock<ParanaBancoDbContext>();
            var dbSetMock = new Mock<DbSet<User>>();

            dbSetMock.Setup(s => s.FindAsync(It.IsAny<int>())).ReturnsAsync(user);
            dbContextMock.Setup(s => s.Set<User>()).Returns(dbSetMock.Object);

            var userRepository = new UserRepository(dbContextMock.Object);

            //Act
            var result = await userRepository.GetById(1, false);

            //Assert  
            Assert.NotNull(result);
            Assert.IsAssignableFrom<User>(result);

            dbSetMock.Verify(c => c.FindAsync(It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public async void GetById_Return_Null()
        {
            //Arrange
            var dbContextMock = new Mock<ParanaBancoDbContext>();
            var dbSetMock = new Mock<DbSet<User>>();

            dbSetMock.Setup(s => s.FindAsync(It.IsAny<int>())).ReturnsAsync(() => null);
            dbContextMock.Setup(s => s.Set<User>()).Returns(dbSetMock.Object);

            var userRepository = new UserRepository(dbContextMock.Object);

            //Act
            var result = await userRepository.GetById(1, false);

            //Assert  
            Assert.Null(result);

            dbSetMock.Verify(c => c.FindAsync(It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public async void GetById_Return_DeletedUser()
        {
            //Arrange
            var user = new User("Unit Test", new Email("test@gmail.com"));
            user.Delete();

            var dbContextMock = new Mock<ParanaBancoDbContext>();
            var dbSetMock = new Mock<DbSet<User>>();

            dbSetMock.Setup(s => s.FindAsync(It.IsAny<int>())).ReturnsAsync(user);
            dbContextMock.Setup(s => s.Set<User>()).Returns(dbSetMock.Object);

            var userRepository = new UserRepository(dbContextMock.Object);

            //Act
            var result = await userRepository.GetById(1, true);

            //Assert  
            Assert.NotNull(result);
            Assert.True(result.IsDeleted);
            Assert.NotNull(result.LastModifiedAt);
            Assert.IsAssignableFrom<User>(result);

            dbSetMock.Verify(c => c.FindAsync(It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public async void GetById_DoesNotReturn_DeletedUser()
        {
            //Arrange
            var user = new User("Unit Test", new Email("test@gmail.com"));
            user.Delete();

            var dbContextMock = new Mock<ParanaBancoDbContext>();
            var dbSetMock = new Mock<DbSet<User>>();

            dbSetMock.Setup(s => s.FindAsync(It.IsAny<int>())).ReturnsAsync(user);
            dbContextMock.Setup(s => s.Set<User>()).Returns(dbSetMock.Object);

            var userRepository = new UserRepository(dbContextMock.Object);

            //Act
            var result = await userRepository.GetById(1, false);

            //Assert  
            Assert.Null(result);

            dbSetMock.Verify(c => c.FindAsync(It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public async void GetById_Return_RestoredUser()
        {
            //Arrange
            var user = new User("Unit Test", new Email("test@gmail.com"));
            user.Delete();
            user.Restore();

            var dbContextMock = new Mock<ParanaBancoDbContext>();
            var dbSetMock = new Mock<DbSet<User>>();

            dbSetMock.Setup(s => s.FindAsync(It.IsAny<int>())).ReturnsAsync(user);
            dbContextMock.Setup(s => s.Set<User>()).Returns(dbSetMock.Object);

            var userRepository = new UserRepository(dbContextMock.Object);

            //Act
            var result = await userRepository.GetById(1, false);

            //Assert  
            Assert.NotNull(result);
            Assert.False(result.IsDeleted);
            Assert.IsAssignableFrom<User>(result);

            dbSetMock.Verify(c => c.FindAsync(It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public async void GetAll_Return_UsersList()
        {
            //Arrange
            var usersList = new List<User>()
            {
                new User("Unit Test 1", new Email("test1@gmail.com")),
                new User("Unit Test 2", new Email("test2@gmail.com")),
                new User("Unit Test 3", new Email("test3@gmail.com")),
            }.AsQueryable();

            var dbContextMock = new Mock<ParanaBancoDbContext>();
            var dbSetMock = new Mock<DbSet<User>>();

            dbSetMock.As<IAsyncEnumerable<User>>()
                .Setup(d => d.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new AsyncEnumerator<User>(usersList.GetEnumerator()));

            dbSetMock.As<IQueryable<User>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<User>(usersList.Provider));

            dbSetMock.As<IQueryable<User>>()
                .Setup(m => m.Expression)
                .Returns(usersList.Expression);
            dbSetMock.As<IQueryable<User>>()
                .Setup(m => m.ElementType)
                .Returns(usersList.ElementType);
            dbSetMock.As<IQueryable<User>>()
                .Setup(m => m.GetEnumerator())
                .Returns(() => usersList.GetEnumerator());

            dbContextMock.Setup(s => s.Set<User>()).Returns(dbSetMock.Object);

            var userRepository = new UserRepository(dbContextMock.Object);

            //Act
            var result = await userRepository.GetAll(false);

            //Assert  
            Assert.Equal(usersList, result);
            Assert.True(result.Any());
            Assert.IsAssignableFrom<IEnumerable<User>>(result);
        }

        [Fact]
        public async void GetAll_Return_UsersListEmpty()
        {
            //Arrange
            var usersList = new List<User>()
            {
            }.AsQueryable();

            var dbContextMock = new Mock<ParanaBancoDbContext>();
            var dbSetMock = new Mock<DbSet<User>>();

            dbSetMock.As<IAsyncEnumerable<User>>()
                .Setup(d => d.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new AsyncEnumerator<User>(usersList.GetEnumerator()));

            dbSetMock.As<IQueryable<User>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<User>(usersList.Provider));

            dbSetMock.As<IQueryable<User>>()
                .Setup(m => m.Expression)
                .Returns(usersList.Expression);
            dbSetMock.As<IQueryable<User>>()
                .Setup(m => m.ElementType)
                .Returns(usersList.ElementType);
            dbSetMock.As<IQueryable<User>>()
                .Setup(m => m.GetEnumerator())
                .Returns(() => usersList.GetEnumerator());

            dbContextMock.Setup(s => s.Set<User>()).Returns(dbSetMock.Object);

            var userRepository = new UserRepository(dbContextMock.Object);

            //Act
            var result = await userRepository.GetAll(false);

            //Assert  
            Assert.Equal(usersList, result);
            Assert.False(result.Any());
            Assert.IsAssignableFrom<IEnumerable<User>>(result);
        }

        [Fact]
        public async void GetAll_Return_DeletedUsersList()
        {
            //Arrange
            var usersList = new List<User>()
            {
                new User("Unit Test 1", new Email("test1@gmail.com")),
                new User("Unit Test 2", new Email("test2@gmail.com")),
                new User("Unit Test 3", new Email("test3@gmail.com")),
            }.AsQueryable();

            foreach (var user in usersList)
                user.Delete();

            var dbContextMock = new Mock<ParanaBancoDbContext>();
            var dbSetMock = new Mock<DbSet<User>>();

            dbSetMock.As<IAsyncEnumerable<User>>()
                .Setup(d => d.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new AsyncEnumerator<User>(usersList.GetEnumerator()));

            dbSetMock.As<IQueryable<User>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<User>(usersList.Provider));

            dbSetMock.As<IQueryable<User>>()
                .Setup(m => m.Expression)
                .Returns(usersList.Expression);
            dbSetMock.As<IQueryable<User>>()
                .Setup(m => m.ElementType)
                .Returns(usersList.ElementType);
            dbSetMock.As<IQueryable<User>>()
                .Setup(m => m.GetEnumerator())
                .Returns(() => usersList.GetEnumerator());

            dbContextMock.Setup(s => s.Set<User>()).Returns(dbSetMock.Object);

            var userRepository = new UserRepository(dbContextMock.Object);

            //Act
            var result = await userRepository.GetAll(true);

            //Assert  
            Assert.Equal(usersList, result);
            Assert.True(result.Any());
            Assert.IsAssignableFrom<IEnumerable<User>>(result);
            Assert.All(usersList, u => Assert.True(u.IsDeleted));
            Assert.All(usersList, u => Assert.NotNull(u.LastModifiedAt));
        }

        [Fact]
        public async void GetAll_DoesNotReturn_DeletedUsersList()
        {
            //Arrange
            var usersList = new List<User>()
            {
                new User("Unit Test 1", new Email("test1@gmail.com")),
                new User("Unit Test 2", new Email("test2@gmail.com")),
                new User("Unit Test 3", new Email("test3@gmail.com")),
            }.AsQueryable();

            foreach (var user in usersList)
                user.Delete();

            var dbContextMock = new Mock<ParanaBancoDbContext>();
            var dbSetMock = new Mock<DbSet<User>>();

            dbSetMock.As<IAsyncEnumerable<User>>()
                .Setup(d => d.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new AsyncEnumerator<User>(usersList.GetEnumerator()));

            dbSetMock.As<IQueryable<User>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<User>(usersList.Provider));

            dbSetMock.As<IQueryable<User>>()
                .Setup(m => m.Expression)
                .Returns(usersList.Expression);
            dbSetMock.As<IQueryable<User>>()
                .Setup(m => m.ElementType)
                .Returns(usersList.ElementType);
            dbSetMock.As<IQueryable<User>>()
                .Setup(m => m.GetEnumerator())
                .Returns(() => usersList.GetEnumerator());

            dbContextMock.Setup(s => s.Set<User>()).Returns(dbSetMock.Object);

            var userRepository = new UserRepository(dbContextMock.Object);

            //Act
            var result = await userRepository.GetAll(false);

            //Assert  
            Assert.NotEqual(usersList, result);
            Assert.False(result.Any());
        }

        [Fact]
        public async void GetAll_Return_RestoreUsersList()
        {
            //Arrange
            var usersList = new List<User>()
            {
                new User("Unit Test 1", new Email("test1@gmail.com")),
                new User("Unit Test 2", new Email("test2@gmail.com")),
                new User("Unit Test 3", new Email("test3@gmail.com")),
            }.AsQueryable();

            foreach (var user in usersList) user.Delete();
            Assert.All(usersList, u => Assert.True(u.IsDeleted));

            foreach (var user in usersList) user.Restore();

            var dbContextMock = new Mock<ParanaBancoDbContext>();
            var dbSetMock = new Mock<DbSet<User>>();

            dbSetMock.As<IAsyncEnumerable<User>>()
                .Setup(d => d.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new AsyncEnumerator<User>(usersList.GetEnumerator()));

            dbSetMock.As<IQueryable<User>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<User>(usersList.Provider));

            dbSetMock.As<IQueryable<User>>()
                .Setup(m => m.Expression)
                .Returns(usersList.Expression);
            dbSetMock.As<IQueryable<User>>()
                .Setup(m => m.ElementType)
                .Returns(usersList.ElementType);
            dbSetMock.As<IQueryable<User>>()
                .Setup(m => m.GetEnumerator())
                .Returns(() => usersList.GetEnumerator());

            dbContextMock.Setup(s => s.Set<User>()).Returns(dbSetMock.Object);

            var userRepository = new UserRepository(dbContextMock.Object);

            //Act
            var result = await userRepository.GetAll(false);

            //Assert  
            Assert.Equal(usersList, result);
            Assert.True(result.Any());
            Assert.IsAssignableFrom<IEnumerable<User>>(result);
            Assert.All(usersList, u => Assert.False(u.IsDeleted));
            Assert.All(usersList, u => Assert.NotNull(u.LastModifiedAt));
        }

        [Fact]
        public async void Add_Verify_IfAddAsyncIsExecuted()
        {
            //Arrange
            var dbContextMock = new Mock<ParanaBancoDbContext>();
            var dbSetMock = new Mock<DbSet<User>>();
            var user = new User("Unit Test", new Email("test@gmail.com"));

            dbSetMock.Setup(s => s.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Callback((User model, CancellationToken token) => { })
                .ReturnsAsync(It.IsAny<EntityEntry<User>>());

            dbContextMock.Setup(s => s.Set<User>()).Returns(dbSetMock.Object);

            var userRepository = new UserRepository(dbContextMock.Object);

            //Act
            var result = await userRepository.Add(user);

            //Assert
            dbSetMock.Verify(c => c.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async void Update_Verify_IfUpdateIsExecuted()
        {
            //Arrange
            var dbContextMock = new Mock<ParanaBancoDbContext>();
            var dbSetMock = new Mock<DbSet<User>>();
            var user = new User("Unit Test", new Email("test@gmail.com"));

            dbSetMock.Setup(s => s.Update(It.IsAny<User>()))
                .Returns(It.IsAny<EntityEntry<User>>());

            dbContextMock.Setup(s => s.Set<User>()).Returns(dbSetMock.Object);

            var userRepository = new UserRepository(dbContextMock.Object);

            //Act
            await userRepository.Update(user);

            //Assert
            dbSetMock.Verify(c => c.Update(It.IsAny<User>()), Times.Once());
        }

        [Fact]
        public async void Delete_Verify_IfDeleteIsExecuted()
        {
            //Arrange
            var dbContextMock = new Mock<ParanaBancoDbContext>();
            var dbSetMock = new Mock<DbSet<User>>();
            var user = new User("Unit Test", new Email("test@gmail.com"));

            dbSetMock.Setup(s => s.Remove(It.IsAny<User>()))
                .Returns(It.IsAny<EntityEntry<User>>());

            dbContextMock.Setup(s => s.Set<User>()).Returns(dbSetMock.Object);

            var userRepository = new UserRepository(dbContextMock.Object);

            //Act
            await userRepository.Delete(user);

            //Assert
            dbSetMock.Verify(c => c.Remove(It.IsAny<User>()), Times.Once());
        }
    }
}
