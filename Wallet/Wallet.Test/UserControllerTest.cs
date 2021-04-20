using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Wallet.API.Controllers;
using Wallet.Business;
using Wallet.Business.Logic;
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
            Assert.NotNull(_unitOfWork.Users.GetById(2));
            // Test accounts creation
            Assert.NotNull(_unitOfWork.Accounts.GetById(3));
            Assert.NotNull(_unitOfWork.Accounts.GetById(4));

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
    }
}

