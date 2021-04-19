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
    public class SessionControllerTest : TestBase
    {
        private readonly UsersController usersController;
        private readonly SessionController sessionController;
        private readonly RegisterModel userToRegister= new()
        {
            FirstName = "alice",
            LastName = "prueba",
            Email = "alice@mail.com",
            Password = "Pass1234!"
        };

        public SessionControllerTest()
        {
            var userBusiness = new UserBusiness(_unitOfWork, _mapper);
            context.ChangeTracker.Clear();
            // Create user controller
            usersController = new UsersController(userBusiness)
            {
                ControllerContext = _controllerContext
            };
            var sessionBusiness = new SessionBusiness(_unitOfWork, _configuration, _mapper);
            sessionController = new SessionController(sessionBusiness)
            {
                ControllerContext = _controllerContext
            };
        }

        [Fact]
        public async  void Login_Registered_Ok()
        {
            //Arrange
            //Register user to login 
            await usersController.Register(userToRegister);
            //Create login model for the user to log in
            LoginModel userToLogIn = new LoginModel()
            {
                Email = userToRegister.Email,
                Password = userToRegister.Password
            };
            //Act
            //Login the user
            var result = await sessionController.Login(userToLogIn);
            //Assert
            //Test correct type and cast it for other tests
            Assert.IsType<OkObjectResult>(result);
            var resultOk = (OkObjectResult)result;
            //Test correct status code
            Assert.Equal(200, resultOk.StatusCode);
            //Test token creation
            Assert.NotNull(resultOk.Value);
            Assert.Contains("tokenString", resultOk.Value.ToString() );    
        }

        [Theory]
        [InlineData("error@mail.com", "Pass1234!")] /*Wrong email, correct password*/
        [InlineData("alice@mail.com", "IncorrectPassword")] /* Correct email, wrong password*/
        [InlineData("error@mail.com", "IncorrectPassword")] /*Wrong email, wrong password*/
        public async void Login_IncorrectEmail_Error(string email, string password)
        {
            //Arrange
            //Register user to login 
            UserFilterModel user = new();
            await usersController.Register(userToRegister);            
            //Create login model with wrong Email
            LoginModel userToLogIn = new LoginModel()
            {
                Email = email,
                Password = password
            };
            //Act
            //Try log in with incorrect email
            Func<Task> result = () => sessionController.Login(userToLogIn);
            //Assert
            var exception = await Assert.ThrowsAsync<CustomException>(result);
            Assert.Equal(400, exception.StatusCode);
            Assert.Equal("Alguno de los datos ingresados es incorrecto", exception.Error);
        }

    }

}
