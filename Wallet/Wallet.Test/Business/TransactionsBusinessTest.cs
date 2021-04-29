using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
    public class TransactionsBusinessTest : TestBase
    {
        //data definition
        static TransactionsController _controller;
        static TransactionBusiness _business;
        static TransactionCreateModel createModel = new()
        {
            Amount = 100,
            Concept = "Recarga",
        };
        //constructor
        public TransactionsBusinessTest() : base()
        {
            _business = new TransactionBusiness(_unitOfWork, _mapper, _ratesBusiness, _accountBusiness);
            context.ChangeTracker.Clear();
            _controller = new TransactionsController(_business)
            {
                ControllerContext = _controllerContext
            };
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
            static Task result() => _controller.CreateAsync(createModel);

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
            var result = await _controller.GetAllAsync(page, model);

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
        [Trait("DetailsAsync", "Ok")]
        public async void Details_Ok()
        {
            int id = 1;
            // Act
            var result = await _controller.DetailsAsync(id);

            // Check status code ok
            Assert.IsType<ObjectResult>(result);
            var objResult = (ObjectResult)result;

            //Assert.Equal(200, objResult.StatusCode);

            // Check type and if what transaction is not null
            Assert.IsType<TransactionDetailsModel>(objResult.Value);
            var transaction = (TransactionDetailsModel)objResult.Value;
            Assert.NotNull(transaction);
        }

        [Theory]
        [InlineData(100, 150, "Recarga de prueba, no le pertenece", "Topup", 4)] // Don't belong to you 
        [InlineData(-1, 0, "", "", 4)] // Id not valid
        [Trait("DetailsAsync", "Error")]
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
                async Task result() => await _controller.DetailsAsync(t.Id);

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
                async Task result() => await _controller.DetailsAsync(t.Id);

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
            var resultDetails = await _controller.DetailsAsync(1);

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

            //// check message error
            //switch (id)
            //{
            //    case -2:
            //        Assert.Equal("Id no válido", ex.Error);
            //        break;
            //    case 2:
            //        Assert.Equal("No se encontró la transacción", ex.Error);
            //        break;
            //    case 10:
            //        Assert.Equal("La transacción no es editable", ex.Error);
            //        break;
            //}
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

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        public async void Transfer_AccountsExist_Ok(int amount)
        {
            //Arrange
            //Users, accounts and topup transaction already craeted in DataInitializer
            TransferModel transferData = new()
            {
                AccountId = 2,
                Amount = amount,
                RecipientAccountId = 4
            };
            //Act
            var result = await _controller.TransferAsync(transferData);
            //Assert
            Assert.IsType<StatusCodeResult>(result);
            var resultContent = (StatusCodeResult)result;
            Assert.Equal(201, resultContent.StatusCode);
        }

        [Theory]
        [InlineData(0,0)] // Both accounts don't exist
        [InlineData(Int32.MaxValue,Int32.MinValue)] //extreme values accounts don't exist
        [InlineData(1,5)] //first account exists, second account doesn't
        [InlineData(6,4)] //first account doesn't exists, second account exists
        [InlineData(-1,-1)] //negative value accounts don't exist
        public async void Transfer_AccountsNonexistent_Error(int senderId,int recipientId)
        {
            //Arrange
            TransferModel transferData = new()
            {
                AccountId = senderId,
                Amount = 10,
                RecipientAccountId = recipientId
            };
            //Act
            Func<Task> result = () => _controller.TransferAsync(transferData);
            //Assert
            var exception = await Assert.ThrowsAsync<CustomException>(result);
            Assert.Equal(404, exception.StatusCode);
            Assert.Equal("Alguna de las cuentas ingresadas no existe", exception.Error);
        }

        [Theory]
        [InlineData(1,1)] //Same account
        [InlineData(2,3)] //Pesos to dollars
        [InlineData(1,4)] //Dollar to Pesos
        [InlineData(3,4)] //Account not owned by logged-in user
        public async void Transfer_IncorrectData_Error(int senderId,int recipientId)
        {
            //Arrange
            TransferModel transferData = new()
            {
                AccountId = senderId,
                Amount = 10,
                RecipientAccountId = recipientId
            };
            //Act
            Func<Task> result = () => _controller.TransferAsync(transferData);
            //Assert
            var exception = await Assert.ThrowsAsync<CustomException>(result);
            Assert.Equal(400, exception.StatusCode);
            Assert.Equal("Alguno de los datos ingresados es incorrecto", exception.Error);
        }

        [Theory]
        [InlineData(1, double.Epsilon, 3)]//Dollar account, smallest value above the total balance
        [InlineData(2, double.Epsilon, 4)] //Pesos account, smallest value above the total balance
        [InlineData(1, Int32.MaxValue, 3)]    //Largest value
        [InlineData(2, 10, 4)]   //Normal value above the total balance
        public async void Transfer_InsufficientBalance_Error(int senderId, double amount, int recipientId)
        {
            //Arrange
            //Empty accounts to test extreme values
            Transactions transactionUSD = new()
            {
                Amount = 100,
                Concept = "Recarga por defecto",
                Type = "Payment",
                AccountId = 1
            };
            Transactions transactionARS = new()
            {
                Amount = 100,
                Concept = "Recarga por defecto",
                Type = "Payment",
                AccountId = 2
            };
            context.Transactions.Add(transactionUSD);
            context.Transactions.Add(transactionARS);
            context.SaveChanges();
            TransferModel transferData = new()
            {
                AccountId = senderId,
                Amount = amount,
                RecipientAccountId = recipientId
            };
            //Act
            Func<Task> result = () => _controller.TransferAsync(transferData);
            //Assert
            var exception = await Assert.ThrowsAsync<CustomException>(result);
            Assert.Equal(400, exception.StatusCode);
            Assert.Equal("Saldo insuficiente", exception.Error);
        }

    }
}



