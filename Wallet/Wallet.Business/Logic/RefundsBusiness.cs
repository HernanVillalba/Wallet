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
        public RefundsBusiness(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
