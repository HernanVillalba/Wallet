using System;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wallet.API.Controllers;
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
        static UsersController usersController;
        static UnitTest1()
        {
            //Debe haber una forma mejor de convertir AutoMapperProfile a IMapper, pero no encontre
            //var mapper = (IMapper)new AutoMapperProfile();
            var configuration = new MapperConfiguration(cfg => {
                cfg.CreateMap<Users, RegisterModel>().ReverseMap();
                cfg.CreateMap<Users, LoginModel>().ReverseMap();
                cfg.CreateMap<Users, UserContact>().ReverseMap();
                cfg.CreateMap<Transactions, TransactionCreateModel>().ReverseMap();
                cfg.CreateMap<FixedTermDeposits, FixedTermDepositCreateModel>().ReverseMap();
                cfg.CreateMap<FixedTermDeposits, FixedTermDepositModel>().ReverseMap();
                cfg.CreateMap<Transactions, TransactionFilterModel>().ReverseMap();
                cfg.CreateMap<Users, UserFilterModel>().ReverseMap();
                cfg.CreateMap<Transactions, TransactionDetailsModel>().ReverseMap();
                cfg.CreateMap<TransactionLog, TransactionLogModel>().ReverseMap();
                cfg.CreateMap<Rates, RateModel>().ReverseMap();
                cfg.CreateMap<RefundRequest, RefundRequestModel>().ReverseMap();
                cfg.CreateMap<Transactions, TransactionModel>().ReverseMap();
            });
            var mapper = new Mapper(configuration);


            var options = new DbContextOptionsBuilder<WALLETContext>().UseInMemoryDatabase(databaseName: "WalletDB").Options;
            var context = new WALLETContext(options);
            var unitOfWork = new UnitOfWork(context);
            var userBusiness = new UserBusiness(unitOfWork, mapper);
            usersController = new UsersController(userBusiness);
        }

        [Fact]
        public async void TestRegister()
        {
            var registerModel = new RegisterModel()
            {
                FirstName = "Juanpi",
                LastName = "Taladro",
                Email = "jt@mail.com",
                Password = "Pass1234!"
            };

            var result = await usersController.Register(registerModel);

            Assert.IsType<StatusCodeResult>(result);
            var statusCodeResult = (StatusCodeResult)result;
            Assert.Equal(201, statusCodeResult.StatusCode);
        }
    }
}
