using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Wallet.API.Controllers;
using Wallet.Business.Logic;
using Wallet.Data.Models;
using Wallet.Entities;
using Xunit;

namespace Wallet.Test
{
    public class TransactionsControllerTest : TestBase
    {
        static TransactionsController transactionsController;
        //data definition
        static TransactionFilterModel filterModel = new TransactionFilterModel();
        static TransactionCreateModel createModel = new TransactionCreateModel()
        {
            Amount = 100,
            Concept = "Recarga",
            Type = "Topup"
        };
        //constructor
        public TransactionsControllerTest() : base()
        {
            var transactionsBusiness = new TransactionBusiness(_unitOfWork, _mapper);
            transactionsController = new TransactionsController(transactionsBusiness)
            {
                ControllerContext = _controllerContext
            };
        }

        [Fact]
        public async void Test01Created()
        {
            var result = await transactionsController.Create(createModel);

            Assert.IsType<StatusCodeResult>(result);

            var statusCodeResult = (StatusCodeResult)result;
            Assert.Equal(201, statusCodeResult.StatusCode);
        }

        [Fact]
        public async void Test02GetAll()
        {
            var result = await transactionsController.GetAll(1, filterModel);

            //check status code ok

            //check object (list returned by controller)
            Assert.IsType<ObjectResult>(result);
            var objectResult = (ObjectResult)result;

            Assert.Equal(200, objectResult.StatusCode);

            //check which list has transactions
            var list = (IEnumerable<TransactionModel>)objectResult.Value;
            Assert.True(list.Any());
        }
        [Fact]
        public async void Test03FilterByAccountId()
        {
            filterModel.AccountId = 2;
            var result = await transactionsController.GetAll(1, filterModel);

            Assert.IsType<ObjectResult>(result);
            var objectResult = (ObjectResult)result;
            Assert.Equal(200, objectResult.StatusCode);

            var list = (IEnumerable<TransactionModel>)objectResult.Value;
            Assert.NotEmpty(list);
        }
        [Fact]
        public async void Test04FilterByType()
        {
            filterModel.Type = "Topup";
            var result = await transactionsController.GetAll(1, filterModel);

            Assert.IsType<ObjectResult>(result);
            var objectResult = (ObjectResult)result;

            Assert.True(objectResult.StatusCode == 200);

            var list = (IEnumerable<TransactionModel>)objectResult.Value;
            Assert.NotEmpty(list);
        }

    }
}
