using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wallet.API.Controllers;
using Wallet.Business;
using Wallet.Business.Logic;
using Wallet.Entities;
using Xunit;

namespace Wallet.Test
{
    public class TransactionsControllerTest : TestBase
    {
        //data definition
        static TransactionsController transactionsController;
        static readonly TransactionFilterModel filterModel = new();
        static TransactionCreateModel createModel = new()
        {
            Amount = 100,
            Concept = "Recarga",
            Type = "Topup"
        };
        //constructor
        public TransactionsControllerTest() : base()
        {
            var transactionsBusiness = new TransactionBusiness(_unitOfWork, _mapper, _ratesBusiness, _accountBusiness);
            transactionsController = new TransactionsController(transactionsBusiness)
            {
                ControllerContext = _controllerContext
            };
        }

        [Fact]
        public async void Created_New_Ok()
        {
            var result = await transactionsController.Create(createModel);

            Assert.IsType<StatusCodeResult>(result);
            var statusCodeResult = (StatusCodeResult)result;

            Assert.Equal(201, statusCodeResult.StatusCode);
        }

        [Fact]
        public async void Created_NoBalance()
        {
            // Act
            createModel.Type = "Payment";
            createModel.Amount = 10005;
            Task result() => transactionsController.Create(createModel);

            var ex = await Assert.ThrowsAsync<CustomException>(result);
            Assert.Equal(400, ex.StatusCode);
            Assert.Equal("No hay saldo suficiente para realizar la transacción", ex.Error);
        }

        [Fact]
        public async void Get_All_Ok()
        {
            var result = await transactionsController.GetAll(1, filterModel);

            //check object (list returned by controller)
            Assert.IsType<ObjectResult>(result);
            var objectResult = (ObjectResult)result;

            //check status code ok
            Assert.Equal(200, objectResult.StatusCode);

            //check which list has transactions
            var list = (IEnumerable<TransactionModel>)objectResult.Value;
            Assert.True(list.Any());
        }

        [Fact]
        public async void Get_Filter_ByAccountId_Ok()
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
        public async void Get_Filter_ByType_Ok()
        {
            filterModel.Type = "Topup";
            var result = await transactionsController.GetAll(1, filterModel);

            Assert.IsType<ObjectResult>(result);
            var objectResult = (ObjectResult)result;

            Assert.True(objectResult.StatusCode == 200);

            var list = (IEnumerable<TransactionModel>)objectResult.Value;
            Assert.True(list.Any());
        }

    }
}
