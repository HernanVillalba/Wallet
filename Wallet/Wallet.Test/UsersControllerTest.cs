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
    public class UsersControllerTest : TestBase
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
        private readonly List<Users> usersList = new()
        {
            new Users { FirstName = "A", LastName = "AA", Email = "A@mail.com", Password = "1234" },
            new Users { FirstName = "B", LastName = "BB", Email = "B@mail.com", Password = "1234" },
            new Users { FirstName = "C", LastName = "CC", Email = "C@mail.com", Password = "1234" },
            new Users { FirstName = "D", LastName = "DD", Email = "D@mail.com", Password = "1234" },
            new Users { FirstName = "E", LastName = "EE", Email = "E@mail.com", Password = "1234" },
            new Users { FirstName = "F", LastName = "FF", Email = "F@mail.com", Password = "1234" },
            new Users { FirstName = "G", LastName = "GG", Email = "G@mail.com", Password = "1234" },
            new Users { FirstName = "H", LastName = "HH", Email = "H@mail.com", Password = "1234" },
            new Users { FirstName = "I", LastName = "II", Email = "I@mail.com", Password = "1234" },

        };
        public UsersControllerTest()
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
        public void GetById_NonexistentUser_Error(int id)
        {
            //Act
            IActionResult result() => usersController.GetUserById(id);
            //Assert
            var exception = Assert.Throws<CustomException>(result);
            Assert.Equal(404, exception.StatusCode);
            Assert.Equal("Usuario no encontrado", exception.Error);
        }

        [Theory]
        [InlineData(0)] //page zero should return the first page
        [InlineData(1)]
        [InlineData(2)]
        public void GetByPage_PageExists_Ok(int page)
        {
            //Arrange
            //Empty filter to test only paging
            UserFilterModel filter = new();
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
        } 
        
        [Theory]
        [InlineData(3)] //Non existent page
        [InlineData(-1)]//Negative value
        [InlineData(Int32.MinValue)] //Max value
        [InlineData(Int32.MaxValue)] //Min value
        public void GetByPage_PageNotFound_Error(int page)
        {
            //Arrange
            UserFilterModel filter = new();
            context.Users.AddRange(usersList);
            context.SaveChanges();
            //Act
            IActionResult result()=> usersController.GetUsersByPage(page, filter);
            //Assert
            var exception = Assert.Throws<CustomException>(result);
            Assert.Equal(404, exception.StatusCode);
            Assert.Equal("Página no encontrada", exception.Error);            
        }
    }
}

