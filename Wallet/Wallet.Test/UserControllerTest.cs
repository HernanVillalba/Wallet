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
    }
}

