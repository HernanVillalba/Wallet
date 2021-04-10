using AutoMapper;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wallet.Data.Models;
using Wallet.Data.Repositories.Interfaces;
using Wallet.Entities;

namespace Wallet.Business.Logic
{
    public class RefundsBusiness : IRefundsBusiness
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAccountBusiness _accountBusiness;
        public RefundsBusiness(IUnitOfWork unitOfWork, IMapper mapper, IAccountBusiness accountBusiness)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _accountBusiness = accountBusiness;
        }
        public async Task Create(RefundRequestCreateModel refund, int? user_id)
        {
            if (user_id <= 0 || user_id == null) { throw new CustomException(404, "Id de usuario no válido"); }

            //ceuntas del usuario que pide el reembolso
            AccountsUserModel a_ori = _unitOfWork.Accounts.GetAccountsUsers((int)user_id);
            if (!_unitOfWork.Accounts.ValidateAccounts(a_ori)) { throw new CustomException(404, "No se encontró alguna de las cuentas del usuario"); }

            var ori_transaction = _unitOfWork.Transactions.FindTransaction(refund.TransactionId, (int)a_ori.IdUSD, (int)a_ori.IdARS);

            if (ori_transaction != null)
            {
                if (_unitOfWork.RefundRequest.PendingRequestExist(ori_transaction.Id)) { throw new CustomException(400, "Ya existe una solicitud pendiente"); }
                var transfer = _unitOfWork.Transfers.GetTransfer(ori_transaction.Id);
                if (_unitOfWork.Transfers.ValidateTransfer(transfer))
                {
                    var des_transaction = _unitOfWork.Transactions.GetById(transfer.DestinationTransactionId);
                    if (des_transaction == null) { throw new CustomException(404, "No se encontró la transacción de destino"); }
                    //4 es el id de transfer
                    //if (transfer.OriginTransaction.CategoryId != 4 || transfer.DestinationTransaction.CategoryId != 4)
                    if (ori_transaction.CategoryId == 4 || des_transaction.CategoryId == 4)
                    {
                        RefundRequest refundRequest = new RefundRequest()
                        {
                            TransactionId = transfer.OriginTransactionId,
                            Status = "Pending",
                            SourceAccountId = transfer.OriginTransaction.AccountId,
                            TargetAccountId = transfer.DestinationTransaction.AccountId
                        };
                        if (_unitOfWork.RefundRequest.ValidateRefundRequest(refundRequest))
                        {
                            _unitOfWork.RefundRequest.Insert(refundRequest);
                            await _unitOfWork.Complete();
                            return;
                        }
                        else { throw new CustomException(400, "Reembolso no válido"); }
                    }
                    else { throw new CustomException(400, "No se puede pedir reembolso de la transacción"); }

                }
                else { throw new CustomException(404, "El id de alguna de las transacciones no es válido"); }
            }
            else { throw new CustomException(400, "No se encontró la transacción"); }
        }

        public IEnumerable<RefundRequestModel> GetAll(int? user_id)
        {
            if(user_id<=0 || user_id == null) { throw new CustomException(404, "El id de usuario no es válido"); }
            AccountsUserModel accounts = _unitOfWork.Accounts.GetAccountsUsers((int)user_id);
            var listDB = _unitOfWork.RefundRequest.GetAllByAccountsId(accounts);
            IEnumerable<RefundRequestModel> list = _mapper.Map<IEnumerable<RefundRequestModel>>(listDB);
            return list;
        }

        public async Task Accept(int userId, int refundRequestId)
        {
            //Check if request exists and is pending
            RefundRequest refundRequest = _unitOfWork.RefundRequest.GetById(refundRequestId);
            if(refundRequest == null)
            {
                throw new CustomException(404, "Solicitud de reembolso no existente");
            }
            if (refundRequest.Status != "Pending" )
            {
                throw new CustomException(400, "Esta solicitud ya ha sido procesada");
            }
            //Check if the target account is owned by the current user
            var targetAccount = _unitOfWork.Accounts.GetById(refundRequest.TargetAccountId);
            if (targetAccount.UserId != userId)
            {
                throw new CustomException(400, "Solo puede aceptar reembolsos de transacciones realizadas a una cuenta propia");
            }
            //Check if the account has enough money to accept the refund
            var transaction = _unitOfWork.Transactions.GetById(refundRequest.TransactionId);
            var balance = _accountBusiness.GetAccountBalance(targetAccount.UserId, targetAccount.Currency);
            if (balance < transaction.Amount)
            {
                throw new CustomException(400, "No posee saldo suficiente para aceptar el reembolso");
            }
            //Accept the refund
            //Change the status to accepted
            refundRequest.Status = "Accepted";
            _unitOfWork.RefundRequest.Update(refundRequest);
            //Create inverse transactions
            Transactions transferTopup = new Transactions
            {
                Amount = transaction.Amount,
                Concept = "Reembolso de una transacción",
                Type = "Topup",
                AccountId = refundRequest.SourceAccountId,
                CategoryId = 4
            };
            Transactions transferPayment = new Transactions
            {
                Amount = transaction.Amount,
                Concept = "Reembolso de una transacción",
                Type = "Payment",
                AccountId = refundRequest.TargetAccountId,
                CategoryId = 4
            };
            _unitOfWork.Transactions.Insert(transferTopup);
            _unitOfWork.Transactions.Insert(transferPayment);
            await _unitOfWork.Complete();
            //Add to transfers table
            Transfers transfer = new Transfers()
            {
                OriginTransactionId = transferPayment.Id,
                DestinationTransactionId = transferTopup.AccountId
            };
            _unitOfWork.Transfers.Insert(transfer);
            //Save all changes
            await _unitOfWork.Complete();
        }

        public RefundRequestModel Details(int refundRequestId)
        {
            if (refundRequestId <= 0)
                throw new CustomException(400, "Id de reembolso inválido");

            var refundRequest = _unitOfWork.RefundRequest.GetById(refundRequestId);

            if (refundRequest == null)
                throw new CustomException(400, "No se pudo encontrar el reembolso");

            // Maps the refund request db entity to view model entity
            RefundRequestModel refundRequestModel = _mapper.Map<RefundRequestModel>(refundRequest);

            return refundRequestModel;
        }
    }
}
