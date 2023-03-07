using ParanaBanco.Domain.Entities;
using ParanaBanco.Domain.Exceptions;
using ParanaBanco.Domain.ValueObjects;
using Xunit;

namespace ParanaBanco.UnitTests.Domain.Entities
{
    public class UserTests
    {
        [Fact]
        public void Create_Return_AllFieldsFilled()
        {
            var user = new User("Unit Test", new Email("test@gmail.com"));

            Assert.NotEmpty(user.FullName);
            Assert.NotEmpty(user.Email.Address);
            Assert.NotEqual(user.CreatedAt, DateTime.MinValue);
            Assert.False(user.IsDeleted);
            Assert.Null(user.LastModifiedAt);
        }

        [Fact]
        public void Modify_Return_LastModifiedAtFilled()
        {
            var user = new User("Unit Test", new Email("test@gmail.com"));

            user.Modify();

            Assert.NotEmpty(user.FullName);
            Assert.NotEmpty(user.Email.Address);
            Assert.NotEqual(user.CreatedAt, DateTime.MinValue);
            Assert.False(user.IsDeleted);
            Assert.NotNull(user.LastModifiedAt);
        }

        [Fact]
        public void Delete_Return_IsDeletedTrue()
        {
            var user = new User("Unit Test", new Email("test@gmail.com"));

            user.Delete();

            Assert.NotEmpty(user.FullName);
            Assert.NotEmpty(user.Email.Address);
            Assert.NotEqual(user.CreatedAt, DateTime.MinValue);
            Assert.True(user.IsDeleted);
            Assert.NotNull(user.LastModifiedAt);
        }

        [Fact]
        public void Restore_Return_IsDeletedFalse()
        {
            var user = new User("Unit Test", new Email("test@gmail.com"));

            user.Delete();

            Assert.True(user.IsDeleted);

            user.Restore();

            Assert.NotEmpty(user.FullName);
            Assert.NotEmpty(user.Email.Address);
            Assert.NotEqual(user.CreatedAt, DateTime.MinValue);
            Assert.False(user.IsDeleted);
            Assert.NotNull(user.LastModifiedAt);
        }

        [Fact]
        public void EmailEmpty_Return_InvalidEmailException()
        {
            Assert.Throws<InvalidEmailException>(() => new User("Unit Test", new Email("")));
        }

        [Fact]
        public void EmailLengthLessThan5_Return_InvalidEmailException()
        {
            Assert.Throws<InvalidEmailException>(() => new User("Unit Test", new Email("test")));
        }

        [Fact]
        public void NotEmail_Return_InvalidEmailException()
        {
            Assert.Throws<InvalidEmailException>(() => new User("Unit Test", new Email("www.test.com")));
        }
    }
}
