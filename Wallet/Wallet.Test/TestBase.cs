using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Wallet.API.Controllers;
using Wallet.Business.Logic;
using Wallet.Business.Profiles;
using Wallet.Data.Models;
using Wallet.Data.Repositories;
using Wallet.Data.Repositories.Interfaces;
using Wallet.Entities;

namespace Wallet.Test
{
    public class TestBase
    {
        protected readonly IMapper _mapper;
        protected readonly IUnitOfWork _unitOfWork;
        protected ClaimsIdentity _identity;
        protected ClaimsPrincipal _user;
        protected ControllerContext _controllerContext;
        protected WALLETContext context;
        public TestBase()
        {
            //Set Mock identity
            _identity = new ClaimsIdentity();
            _user = new ClaimsPrincipal(_identity);
            _controllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = _user } };
            _identity.AddClaims(new[]
            {
                new Claim("UserId", "1"),
            });
            // Set Database in memory
            context = new WALLETContext(GetDbOptionsBuilder().Options);
            DataInitializer.Initialize(context);
            var c = context.Users.ToList();
            // Set Unit of Work
            _unitOfWork = new UnitOfWork(context);

            // Set Mapper
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AutoMapperProfile());
            });
            _mapper = mappingConfig.CreateMapper();
        }
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
    }
}
