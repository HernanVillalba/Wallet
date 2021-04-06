USE WALLET

GO

CREATE OR ALTER PROCEDURE SP_GetTransactionsUser(
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

CREATE OR ALTER PROCEDURE SP_GetUserFixedTermDeposits (@user_id int)
AS
BEGIN

SELECT * FROM FixedTermDeposits
WHERE FixedTermDeposits.Account_Id IN 
	(SELECT Accounts.Id FROM Accounts 
	 WHERE Accounts.User_Id = @user_id)
ORDER BY FixedTermDeposits.Creation_Date DESC

END

GO

CREATE OR ALTER PROCEDURE SP_GetBalance(
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

CREATE OR ALTER PROCEDURE SP_GetPagedUsers(	
	@PageNumber INT,
	@RowsOfPage INT
)
AS
BEGIN

SELECT id,
	First_Name,
	Last_Name,
	Email
FROM Users
ORDER BY Last_Name 
OFFSET(@PageNumber - 1) * @RowsOfPage ROWS
FETCH NEXT @RowsOfPage ROWS ONLY

END