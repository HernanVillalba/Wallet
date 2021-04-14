using System;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wallet.API.Controllers;
using Wallet.Business;
using Wallet.Business.Logic;
using Wallet.Business.Profiles;
using Wallet.Data.Models;
using Wallet.Data.Repositories;
using Wallet.Entities;
using Xunit;

namespace Wallet.Test
{
    public class UnitTest1
    {
        private static IMapper _mapper;
        static UsersController usersController;
        static RegisterModel registerModel1 = new RegisterModel()
        {
            FirstName = "Juanpi",
            LastName = "Taladro",
            Email = "jt@mail.com",
            Password = "Pass1234!"
        };

        static UnitTest1()
        {
            // Set Database in memory
            var options = new DbContextOptionsBuilder<WALLETContext>().UseInMemoryDatabase(databaseName: "WalletDB").Options;
            var context = new WALLETContext(options);

            // Set Unit of Work
            var unitOfWork = new UnitOfWork(context);

            // Set Mapper
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AutoMapperProfile());
            });

            _mapper = mappingConfig.CreateMapper();

            // Create Business
            var userBusiness = new UserBusiness(unitOfWork, _mapper);

            // Create user controller
            usersController = new UsersController(userBusiness);
        }

        [Fact]
        public async void Test01Register()
        {
            //RegisterModel registerModel1 = new RegisterModel()
            //{
            //    FirstName = "Juanpi",
            //    LastName = "Taladro",
            //    Email = "jt@mail.com",
            //    Password = "Pass1234!"
            //};

            var result = await usersController.Register(registerModel1);

            Assert.IsType<StatusCodeResult>(result);
            var statusCodeResult = (StatusCodeResult)result;
            Assert.Equal(201, statusCodeResult.StatusCode);
        }

        [Fact]
        public async void Test02RegisterDuplicated()
        {
            //RegisterModel registerModel1 = new RegisterModel()
            //{
            //    FirstName = "Juanpi",
            //    LastName = "Taladro",
            //    Email = "jt@mail.com",
            //    Password = "Pass1234!"
            //};

            try
            {
                _ = await usersController.Register(registerModel1);
            }
            catch (Exception ex)
            {
                Assert.IsType<CustomException>(ex);
                var customException = (CustomException)ex;
                Assert.Equal("Usuario ya registrado", customException.Error);
            }
        }

        [Fact]
        public void Test03GetUserById()
        {
            var userId = 1;

            var response = usersController.GetUserById(userId);

            Assert.IsType<OkObjectResult>(response);

            var responseOkResut = (OkObjectResult)response;
            Assert.IsType<UserContact>(responseOkResut.Value);

            var userContact = (UserContact)responseOkResut.Value;
            Assert.Equal(registerModel1.Email, userContact.Email);
        }
    }
}
