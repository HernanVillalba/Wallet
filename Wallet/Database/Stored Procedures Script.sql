USE WALLET

GO

CREATE PROCEDURE SP_GetTransactionsUser(
	 @user_id int
)
AS
BEGIN
	select T.Id,
		   T.Amount,
		   T.Concept,
		   T.Date,
		   T.Type,
		   T.Account_Id
	from Users as U
	Join Accounts as A on A.User_Id=U.Id
	join Transactions as T on T.Account_Id=A.Id
	WHERE A.User_Id = @user_id
	ORDER BY T.Date DESC
END

GO

