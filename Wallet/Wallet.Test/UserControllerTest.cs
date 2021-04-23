using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wallet.API.Controllers;
using Wallet.Business;
using Wallet.Business.Logic;
using Wallet.Data.Models;
using Wallet.Entities;
using Xunit;

namespace Wallet.Test
{
    public class UserControllerTest : TestBase
    {
        private readonly UsersController usersController;
        //Arrange for register
        private readonly RegisterModel registerModel = new()
        {
            FirstName = "alice",
            LastName = "prueba",
            Email = "alice@mail.com",
            Password = "Pass1234!"
        };
        //List of 9 users to test paging (11 in total considering the one in DataInitializer)
        private readonly List<Users> usersList = new List<Users>
        {
            new Users  { FirstName = "A", LastName = "AA", Email = "A@mail.com", Password = "1234" },
            new Users  { FirstName = "A", LastName = "AA", Email = "A2@mail.com", Password = "1234" },
            new Users  { FirstName = "AA", LastName = "AA", Email = "A3@mail.com", Password = "1234" },
            new Users  { FirstName = "AA", LastName = "AA", Email = "A4@mail.com", Password = "1234" },
            new Users  { FirstName = "AA", LastName = "AA", Email = "A5@mail.com", Password = "1234" },
            new Users  { FirstName = "B", LastName = "BB", Email = "B@mail.com", Password = "1234" },
            new Users  { FirstName = "C", LastName = "CC", Email = "C@mail.com", Password = "1234" },
            new Users  { FirstName = "D", LastName = "DD", Email = "D@mail.com", Password = "1234" },
            new Users  { FirstName = "E", LastName = "EE", Email = "E@mail.com", Password = "1234" },           
        };
        public UserControllerTest()
        {
            var userBusiness = new UserBusiness(_unitOfWork, _mapper);
            context.ChangeTracker.Clear();
            // Create user controller
            usersController = new UsersController(userBusiness)
            {
                ControllerContext = _controllerContext
            };
        }

        [Fact]
        public async void Register_New_Ok()
        {
            //Act        
            var result = await usersController.Register(registerModel);
            //Assert
            Assert.IsType<StatusCodeResult>(result);
            var statusCodeResult = (StatusCodeResult)result;
            // Test correct status code
            Assert.Equal(201, statusCodeResult.StatusCode);
            // Test user creation
            Assert.NotNull(_unitOfWork.Users.GetById(3));
            // Test accounts creation
            Assert.NotNull(_unitOfWork.Accounts.GetById(5));
            Assert.NotNull(_unitOfWork.Accounts.GetById(6));
        }

        [Fact]
        public async void Register_Duplicated_Error()
        {
            //Act
            await usersController.Register(registerModel);
            //Try register same user
            Func<Task> result = () => usersController.Register(registerModel);
            //Assert
            var exception = await Assert.ThrowsAsync<CustomException>(result);
            Assert.Equal(400, exception.StatusCode);
            Assert.Equal("Usuario ya registrado", exception.Error);
        }

        [Fact]
        public void GetById_Exists_Ok()
        {
            //Act
            //Search user already created in data initialization
            var user = usersController.GetUserById(1);
            //Assert
            Assert.IsType<OkObjectResult>(user);
            var result = (OkObjectResult)user;
            //Test correct status code
            Assert.Equal(200, result.StatusCode);
            //Test return type
            Assert.IsType<UserContact>(result.Value);
        }

        [Theory]
        [InlineData(0)]  //zero or less
        [InlineData(-1)]
        [InlineData(Int32.MinValue)]
        public void GetById_InvalidId_Error(int id)
        {
            //Act
            IActionResult result() => usersController.GetUserById(id);
            //Assert
            var exception = Assert.Throws<CustomException>(result);
            Assert.Equal(400, exception.StatusCode);
            Assert.Equal("Id inválido", exception.Error);
        }

        [Theory]
        [InlineData(100)]
        [InlineData(Int32.MaxValue)]
        public void GetById_nonexistentUser_Error(int id)
        {
            //Act
            IActionResult result() => usersController.GetUserById(id);
            //Assert
            var exception = Assert.Throws<CustomException>(result);
            Assert.Equal(404, exception.StatusCode);
            Assert.Equal("Usuario no encontrado", exception.Error);
        }

        //[Theory]
        //[InlineData(0,10)] //page zero or less should return the first page
        //[InlineData(Int32.MinValue,10)]
        //[InlineData(1,10)] 
        //[InlineData(2,1)] 
        [Theory]
        [MemberData(nameof(Data_Paging))]
        public void GetByPage_PageExists_Ok(UserFilterModel filter, int page, int count)
        {
            //Arrange
            //Empty filter to test only paging
            //UserFilterModel filter = new()
            //{
            //    FirstName = "",
            //    LastName = "",
            //    Email = ""
            //};
            //Add users
            context.Users.AddRange(usersList);
            context.SaveChanges();
            //Act
            var result = usersController.GetUsersByPage(page, filter);
            //Assert
            Assert.IsType<OkObjectResult>(result);
            var resultContent = (OkObjectResult)result;
            //Test correct status code
            Assert.Equal(200, resultContent.StatusCode);
            //Test return type
            Assert.IsAssignableFrom<IEnumerable<UserContact>>(resultContent.Value);
            var resultList = (List<UserContact>)resultContent.Value;
            //Test return count
            Assert.Equal(count, resultList.Count);
        }
        public static IEnumerable<object[]> Data_Paging =>
            new List<object[]>
            {
                    //page 0 should return first page (10 first users)
                    new object[]
                    {
                        new UserFilterModel(), 0, 10
                    },
                    //negative numbre page should return first page (10 first users)
                    new object[]
                    {
                        new UserFilterModel(), Int32.MinValue, 10
                    },
                    //first page should return first ten users
                    new object[]
                    {
                        new UserFilterModel(), 1, 10
                    },
                    //second page should return only one user
                    new object[]
                    {
                        new UserFilterModel(), 2, 1
                    },
                    //first page filter by first name "A" should return two users
                    new object[] {
                        new UserFilterModel
                        {
                            FirstName = "A",
                            LastName = "",
                            Email = ""
                        }, 1, 2
                    },
                    //first page filter by first name "AA" and last name "AA" should return three users
                    new object[]
                    {
                        new UserFilterModel
                        {
                            FirstName = "AA",
                            LastName = "AA",
                            Email = ""
                        }, 1, 3
                    },
                    new object[]
                    {
                        new UserFilterModel
                        {
                            FirstName = "",
                            LastName = "",
                            Email = "B@mail.com"
                        }, 1, 1
                    }
            };
    }
}

