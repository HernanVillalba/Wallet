using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var options = new DbContextOptionsBuilder<WALLETContext>().UseInMemoryDatabase(databaseName: "WalletDB").Options;
            context = new WALLETContext(options);
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
        public List<Users> lista()
        {
            return context.Users.ToList();
        }
        ~TestBase()
        {
            context.Database.EnsureDeleted();
        }
    }
}
