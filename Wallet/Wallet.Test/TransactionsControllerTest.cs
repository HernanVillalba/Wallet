using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Wallet.API.Controllers;
using Wallet.Business;
using Wallet.Business.Logic;
using Wallet.Data.Models;
using Wallet.Entities;
using Xunit;

namespace Wallet.Test
{
    public class TransactionsControllerTest : TestBase
    {
        //data definition
        static TransactionsController _controller;
        static TransactionCreateModel createModel = new()
        {
            Amount = 100,
            Concept = "Recarga",
        };
        //constructor
        public TransactionsControllerTest() : base()
        {
            var transactionsBusiness = new TransactionBusiness(_unitOfWork, _mapper, _ratesBusiness, _accountBusiness);
            context.ChangeTracker.Clear();
            _controller = new TransactionsController(transactionsBusiness)
            {
                ControllerContext = _controllerContext
            };
        }

        [Fact]
        [Trait("Create", "Ok")]
        public async void Create_New_Ok()
        {
            createModel.Type = "Topup";
            var result = await _controller.Create(createModel);

            Assert.IsType<StatusCodeResult>(result);
            var statusCodeResult = (StatusCodeResult)result;

            Assert.Equal(201, statusCodeResult.StatusCode);
        }

        [Fact]
        [Trait("Created", "Error")] // not balance
        public async void Create_Error()
        {
            // Values
            createModel.Concept = "Pago prueba not balance";
            createModel.Type = "Payment";
            createModel.Amount = 10005;

            // Act
            static Task result() => _controller.Create(createModel);

            // Check if exception was thrown
            var ex = await Assert.ThrowsAsync<CustomException>(result);
            Assert.Equal(400, ex.StatusCode);
            Assert.Equal("No hay saldo suficiente para realizar la transacción", ex.Error);
        }

        [Theory]
        [InlineData(1, "", "", 0)] // get all without filter
        [InlineData(1, "", "", 2)] // filter by accound id
        [InlineData(1, "", "Topup", 0)] // filter by type
        [Trait("Get All", "Ok")]

        public async void Get_All_Ok(int page, string concept, string type, int? accoundId)
        {
            TransactionFilterModel model = new()
            {
                Concept = concept,
                Type = type,
                AccountId = accoundId
            };

            // Act
            var result = await _controller.GetAll(page, model);

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
        [Trait("Details", "Ok")]
        public async void Details_Ok()
        {
            int id = 1;
            // Act
            var result = await _controller.Details(id);

            // Check status code ok
            Assert.IsType<ObjectResult>(result);
            var objResult = (ObjectResult)result;
            Assert.Equal(200, objResult.StatusCode);

            // Check type and if what transaction is not null
            Assert.IsType<TransactionDetailsModel>(objResult.Value);
            var transaction = (TransactionDetailsModel)objResult.Value;
            Assert.NotNull(transaction);
        }

        [Theory]
        [InlineData(100, 150, "Recarga de prueba, no le pertenece", "Topup", 4)] // Don't belong to you 
        [InlineData(-1, 0, "", "", 4)] // Id not valid
        [Trait("Details", "Error")]
        public async void Details_Error(int id, double amount, string concept, string type, int accountId)
        { // Transaction does not belong to you
            var t = new Transactions
            {
                Id = id,
                Amount = amount,
                Concept = concept,
                Type = type,
                AccountId = accountId,
                Date = DateTime.Now
            };
            if (t.Id > 0) // dont belong
            {
                context.Transactions.Add(t);
                context.SaveChanges();

                var algo = context.Transactions.ToList();
                // Act
                async Task result() => await _controller.Details(t.Id);

                // Catch exception
                var ex = await Assert.ThrowsAsync<CustomException>(result);

                //status code client error (bad request)
                Assert.Equal(400, ex.StatusCode);

                // Check message exception
                Assert.Equal("No se encontró la transacción", ex.Error);

                // delete the record
                context.Transactions.Remove(t);
                context.SaveChanges();
            }
            else // id invalid
            {
                async Task result() => await _controller.Details(t.Id);

                // Check status code not found
                var ex = await Assert.ThrowsAsync<CustomException>(result);
                Assert.Equal(400, ex.StatusCode);

                // Check custom exception
                Assert.Equal("Id de la transacción no válido", ex.Error);

            }
        }

        [Fact]
        [Trait("Edit", "Ok")]
        public async void Edit_Ok()
        {
            // Declare values
            int id = 1;
            TransactionEditModel model = new() { Concept = "Nuevo concepto" };

            // Act
            var resultEdit = await _controller.Edit(id, model);

            // Check status code ok
            Assert.IsType<StatusCodeResult>(resultEdit);
            var statusCodeResult = (StatusCodeResult)resultEdit;
            Assert.Equal(200, statusCodeResult.StatusCode);

            // Check values change //
            // Act 
            var resultDetails = await _controller.Details(1);

            // Check if result details is type object result 
            Assert.IsType<ObjectResult>(resultDetails);
            var objResult = (ObjectResult)resultDetails;

            // Check status code Ok
            Assert.Equal(200, objResult.StatusCode);

            // Check if found the transactins
            Assert.NotNull(objResult.Value);

            // Check if the concept changed
            var t = (TransactionDetailsModel)objResult.Value;
            Assert.Equal("Nuevo concepto", t.Concept);
        }

        [Theory]
        [InlineData(2, "No se encontró la transaccion")]
        [InlineData(-2, "Id no valido")]
        [InlineData(10, "Compra de divisas - no editable")]
        [Trait("Edit", "Error")]
        public async void Edit_Error(int id, string concept)
        {
            // Declare values
            TransactionEditModel model = new() { Concept = concept };

            // Act
            async Task result() => await _controller.Edit(id, model);

            // Catch exception
            var ex = await Assert.ThrowsAsync<CustomException>(result);

            // Check status code not found
            Assert.Equal(400, ex.StatusCode);

            // check message error
            switch (id)
            {
                case -2:
                    Assert.Equal("Id no válido", ex.Error);
                    break;
                case 2:
                    Assert.Equal("No se encontró la transacción", ex.Error);
                    break;
                case 10:
                    Assert.Equal("La transacción no es editable", ex.Error);
                    break;
            }
        }

        [Fact]
        public async void Buy_Dollars_Ok()
        {
            // Assert
            TransactionBuyCurrency transaction = new TransactionBuyCurrency
            {
                Amount = 0.5,
                Type = "Compra"
            };

            // Act
            var result = await _controller.BuyCurrencyAsync(transaction);

            // Assert
            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(201, ((StatusCodeResult)result).StatusCode);
            var transaction_1 = _unitOfWork.Transactions.GetById(12);
            var transaction_2 = _unitOfWork.Transactions.GetById(13);
            Assert.NotNull(transaction_1);
            Assert.NotNull(transaction_2);
        }

        [Fact]
        public async void Buy_Pesos_Ok()
        {
            // Assert
            Transactions topup = new Transactions
            {
                Type = "Topup",
                AccountId = 1,
                Amount = 1,
                Concept = "carga dolares",
                CategoryId = 4,
                Date = DateTime.Now
            };
            context.Transactions.Add(topup);
            context.SaveChanges();
            TransactionBuyCurrency transaction = new TransactionBuyCurrency
            {
                Amount = 1,
                Type = "Venta"
            };

            // Act
            var result = await _controller.BuyCurrencyAsync(transaction);

            // Assert
            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(201, ((StatusCodeResult)result).StatusCode);
            var transaction_1 = _unitOfWork.Transactions.GetById(13);
            var transaction_2 = _unitOfWork.Transactions.GetById(14);
            Assert.NotNull(transaction_1);
            Assert.NotNull(transaction_2);
        }

        [Fact]
        public async void Buy_Dollars_Fail_Not_Money()
        {
            // Assert
            TransactionBuyCurrency transaction = new TransactionBuyCurrency
            {
                Amount = 10000,
                Type = "Compra"
            };

            // Act
            async Task result() => await _controller.BuyCurrencyAsync(transaction);

            // Assert
            var ex = await Assert.ThrowsAsync<CustomException>(result);
            Assert.Equal(400, ex.StatusCode);
            Assert.Equal("Saldo insuficiente", ex.Error);
        }

        [Fact]
        public async void Buy_Pesos_Fail_Not_Money()
        {
            // Assert
            TransactionBuyCurrency transaction = new TransactionBuyCurrency
            {
                Amount = 10000,
                Type = "Venta"
            };

            // Act
            async Task result() => await _controller.BuyCurrencyAsync(transaction);

            // Assert
            var ex = await Assert.ThrowsAsync<CustomException>(result);
            Assert.Equal(400, ex.StatusCode);
            Assert.Equal("Saldo insuficiente", ex.Error);
        }
    }
}
