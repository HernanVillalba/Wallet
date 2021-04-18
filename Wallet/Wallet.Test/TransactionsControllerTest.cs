using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel;
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
        [Trait("Create", "Ok")]
        public async void Create_New_Ok()
        {
            createModel.Type = "Topup";
            var result = await transactionsController.Create(createModel);

            Assert.IsType<StatusCodeResult>(result);
            var statusCodeResult = (StatusCodeResult)result;

            Assert.Equal(201, statusCodeResult.StatusCode);
        }

        [Fact]
        [Trait("Created", "Not balance")]
        public async void Created_Error_NoBalance()
        {
            // Values
            createModel.Concept = "Pago prueba not balance";
            createModel.Type = "Payment";
            createModel.Amount = 10005;

            // Act
            static Task result() => transactionsController.Create(createModel);

            // Check if exception was thrown
            var ex = await Assert.ThrowsAsync<CustomException>(result);
            Assert.Equal(400, ex.StatusCode);
            Assert.Equal("No hay saldo suficiente para realizar la transacción", ex.Error);
        }

        [Fact]
        [Trait("Get All", "Ok")]
        public async void Get_All_Ok()
        {
            var result = await transactionsController.GetAll(1, filterModel);

            // Check object (list returned by controller)
            Assert.IsType<ObjectResult>(result);
            var objectResult = (ObjectResult)result;

            // Check if status code is OK
            Assert.Equal(200, objectResult.StatusCode);

            // Check that the list has transactions
            var list = (IEnumerable<TransactionModel>)objectResult.Value;
            Assert.True(list.Any());
        }

        [Fact]
        [Trait("Filter", "Ok")]
        public async void Get_Filter_ByAccountId_Ok()
        {
            // Declare values
            filterModel.AccountId = 2;

            // Act
            var result = await transactionsController.GetAll(1, filterModel);

            //
            Assert.IsType<ObjectResult>(result);
            var objectResult = (ObjectResult)result;

            // Check status code is OK
            Assert.Equal(200, objectResult.StatusCode);

            var list = (IEnumerable<TransactionModel>)objectResult.Value;
            Assert.NotEmpty(list);
        }
        [Fact]
        [Trait("Filter", "Ok")]
        public async void Get_Filter_ByType_Ok()
        {
            // Declare values
            filterModel.Type = "Topup";

            // Act
            var result = await transactionsController.GetAll(1, filterModel);

            // Check type
            Assert.IsType<ObjectResult>(result);
            var objResult = (ObjectResult)result;
            
            // Check status code OK
            Assert.Equal(200, objResult.StatusCode);

            // Check if list have some transaction
            var list = (IEnumerable<TransactionModel>)objResult.Value;
            Assert.True(list.Any());
        }
        [Fact]
        [Trait("Details", "Ok")]
        public async void Details_Ok()
        {
            int id = 1;
            // Act
            var result = await transactionsController.Details(id);

            // Check status code ok
            Assert.IsType<ObjectResult>(result);
            var objResult = (ObjectResult)result;
            Assert.Equal(200, objResult.StatusCode);

            // Check type and if what transaction is not null
            Assert.IsType<TransactionDetailsModel>(objResult.Value);
            var transaction = (TransactionDetailsModel)objResult.Value;
            Assert.NotNull(transaction);
        }

        [Fact]
        [Trait("Details", "Not found")]
        public async void Details_Error_NotFound()
        { // Transaction does not belong to you

            // Act
            int id = 2;
            Task result() => transactionsController.Details(id);

            // Catch exception
            var ex = await Assert.ThrowsAsync<CustomException>(result);

            //status code client error (bad request)
            Assert.Equal(400, ex.StatusCode);

            // Check message exception
            Assert.Equal("No se encontró la transacción", ex.Error);
        }

        [Fact]
        [Trait("Details", "Id not valid")]
        public async void Details_Error_IdNotValid()
        {
            // Act
            int id = 0;
            async Task result() => await transactionsController.Details(id);

            // Check status code not found
            var ex = await Assert.ThrowsAsync<CustomException>(result);
            Assert.Equal(400, ex.StatusCode);

            // Check custom exception
            Assert.Equal("Id de la transacción no válido", ex.Error);

        }
    }
}
