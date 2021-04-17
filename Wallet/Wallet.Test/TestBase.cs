using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Security.Claims;
using Wallet.Business.Logic;
using Wallet.Business.Profiles;
using Wallet.Data.Models;
using Wallet.Data.Repositories;
using Wallet.Data.Repositories.Interfaces;

namespace Wallet.Test
{
    public class TestBase
    {
        #region Declaration of variables
        protected readonly IMapper _mapper;
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IAccountBusiness _accountBusiness;
        protected readonly IRatesBusiness _ratesBusiness;
        protected ClaimsIdentity _identity;
        protected ClaimsPrincipal _user;
        protected ControllerContext _controllerContext;
        protected WALLETContext context;
        #endregion 

        #region set properties
        public TestBase(int userId = 1)
        {

            //Set Mock identity
            _identity = new ClaimsIdentity();
            _user = new ClaimsPrincipal(_identity);
            _controllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = _user } };
            _identity.AddClaims(new[]
            {
                new Claim("UserId", userId.ToString()),
            });
            // Set Database in memory
            context = new WALLETContext(GetDbOptionsBuilder().Options);
            DataInitializer.Initialize(context);
            var c = context.Users.ToList();

            // Set Unit of Work
            _unitOfWork = new UnitOfWork(context);

            // Set account businness
            _accountBusiness = new AccountBusiness(_unitOfWork, _mapper);

            // Set rates
            _ratesBusiness = new RatesBusiness(_unitOfWork, _mapper);

            // Set Mapper
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AutoMapperProfile());
            });
            _mapper = mappingConfig.CreateMapper();
        }
        #endregion

        #region Configuration new DB instance
        //Configure DB to create new instance for every test
        private static DbContextOptionsBuilder<WALLETContext> GetDbOptionsBuilder()
        {
            // The key to keeping the databases unique and not shared is 
            // generating a unique db name for each.
            string dbName = Guid.NewGuid().ToString();

            // Create a fresh service provider, and therefore a fresh 
            // InMemory database instance.
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // Create a new options instance telling the context to use an
            // InMemory database and the new service provider.
            var builder = new DbContextOptionsBuilder<WALLETContext>();
            builder.UseInMemoryDatabase(dbName)
                   .UseInternalServiceProvider(serviceProvider);
            return builder;
        }
        #endregion
    }
}
