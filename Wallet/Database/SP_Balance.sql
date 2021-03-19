USE WALLET

GO

CREATE PROCEDURE SP_Balance(
	@user_id int,
	@currency nvarchar(MAX)
)
AS
BEGIN
		SELECT 
			ISNULL((
			SELECT sum(Amount)
			FROM Transactions AS t
			JOIN Accounts AS a ON t.Account_Id = a.Id
			WHERE a.Currency = @currency
				AND t.Type = 'Topup'
				AND a.User_Id = @user_id
			), 0) - 
			ISNULL((
			SELECT sum(Amount)
			FROM Transactions AS t
			JOIN Accounts AS a ON t.Account_Id = a.Id
			WHERE a.Currency = @currency
				AND t.Type = 'Payment'
				AND a.User_Id = @user_id
			), 0)
		 AS Balance
END
GO