using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Business.Profiles;
using Wallet.Data.Models;
using Wallet.Data.Repositories;
using Wallet.Data.Repositories.Interfaces;

namespace Wallet.Test
{
     public class TestBase
    {
        public static readonly IMapper _mapper;
        public static readonly IUnitOfWork _unitOfWork;

        static TestBase()
        {
            // Set Database in memory
            var options = new DbContextOptionsBuilder<WALLETContext>().UseInMemoryDatabase(databaseName: "WalletDB").Options;
            var context = new WALLETContext(options);

            // Set Unit of Work
            _unitOfWork = new UnitOfWork(context);

            // Set Mapper
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AutoMapperProfile());
            });
            _mapper = mappingConfig.CreateMapper();
	    }
        
    }
}
