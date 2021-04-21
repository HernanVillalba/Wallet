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
        [Trait("Created", "Not balance")]
        public async void Created_Error_NoBalance()
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

        [Fact]
        [Trait("Get All", "Ok")]
        public async void Get_All_Ok()
        {
            var result = await _controller.GetAll(1, filterModel);

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
            var result = await _controller.GetAll(1, filterModel);

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
            var result = await _controller.GetAll(1, filterModel);

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

        [Fact]
        [Trait("Details", "Not found")]
        public async void Details_Error_NotFound()
        { // Transaction does not belong to you
            var t = new Transactions
            {
                Id = 100,
                Amount = 150,
                Concept = "Recarga por defecto sin usuario",
                Type = "Topup",
                AccountId = 4,
                Date = DateTime.Now
            };
            context.Transactions.Add(t);
            context.SaveChanges();

            var algo = context.Transactions.ToList();
            // Act
            async Task result() => await  _controller.Details(t.Id);

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

        [Fact]
        [Trait("Details", "Id not valid")]
        public async void Details_Error_IdNotValid()
        {
            // Act
            int id = 0;
            async Task result() => await _controller.Details(id);

            // Check status code not found
            var ex = await Assert.ThrowsAsync<CustomException>(result);
            Assert.Equal(400, ex.StatusCode);

            // Check custom exception
            Assert.Equal("Id de la transacción no válido", ex.Error);

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

        [Fact]
        [Trait("Edit","Error")]
        public async void Edit_NotFound()
        {
            // Declare values
            int id = 2;
            TransactionEditModel model = new() { Concept = "Nuevo concepto 2" };

            // Act
            async Task result() => await _controller.Edit(id, model);

            // Catch exception
            var ex = await Assert.ThrowsAsync<CustomException>(result);

            // Check status code not found
            Assert.Equal(400, ex.StatusCode);

            // Check message
            Assert.Equal("No se encontró la transacción", ex.Error);
        }

        [Fact]
        [Trait("Edit", "Error")]
        public async void Edit_IdNotValid()
        {
            // DEclare values
            int id = -2; // or zero
            TransactionEditModel model = new() { Concept = "Nuevo concepto 3" };

            // Act
            async Task result() => await _controller.Edit(id, model);

            // Catch exception
            var ex = await Assert.ThrowsAsync<CustomException>(result);

            // Check status code bad request
            Assert.Equal(400, ex.StatusCode);

            // Check message
            Assert.Equal("Id no válido", ex.Error);
        }

        [Fact]
        [Trait("Edit", "Error")]
        public async void Edit_NotEditable()
        {
            // Declare values
            Transactions t = new()
            {
                Id = 10,
                Concept = "Compra de divisas",
                AccountId = 1,
                Category = new Categories() { Id = 2, Editable = false }
            };
            context.Transactions.Add(t);
            context.SaveChanges();
            TransactionEditModel model = new() { Concept = "Compra de divisas - Nuevo concepto" };

            // Act
            async Task result() => await _controller.Edit(t.Id, model);

            // Catch exception
            var ex = await Assert.ThrowsAsync<CustomException>(result);

            // Check status code
            Assert.Equal(400, ex.StatusCode);

            // Check exception message
            Assert.Equal("La transacción no es editable", ex.Error);


            // Remove values of DB
            context.Transactions.Remove(t);
            context.SaveChanges();
        }

        [Fact]
        public async void Buy_Dollars_Ok()
        {
            // Assert
            TransactionBuyCurrency petition = new TransactionBuyCurrency
            {
                Amount = 0.5,
                Type = "Compra"
            };

            // Act
            var result = await _controller.BuyCurrencyAsync(petition);

            // Assert
            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(201, ((StatusCodeResult)result).StatusCode);
            var transaction_1 = _unitOfWork.Transactions.GetById(2);
            var transaction_2 = _unitOfWork.Transactions.GetById(3);
            Assert.NotNull(transaction_1);
            Assert.NotNull(transaction_2);
        }

        [Fact]
        public async void Sell_Dollars_Ok()
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
            TransactionBuyCurrency petition = new TransactionBuyCurrency
            {
                Amount = 1,
                Type = "Venta"
            };

            // Act
            var result = await _controller.BuyCurrencyAsync(petition);

            // Assert
            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(201, ((StatusCodeResult)result).StatusCode);
            var transaction_1 = _unitOfWork.Transactions.GetById(3);
            var transaction_2 = _unitOfWork.Transactions.GetById(4);
            Assert.NotNull(transaction_1);
            Assert.NotNull(transaction_2);
        }
    }
}
