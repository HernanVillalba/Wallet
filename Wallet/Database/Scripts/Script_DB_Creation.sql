CREATE DATABASE WALLET
GO
USE WALLET
GO
CREATE TABLE Users(
Id int not null primary key identity(1,1),	   
First_Name nvarchar(50) not null,
Last_Name nvarchar(50) not null,
Email nvarchar(100) not null UNIQUE,
Password nvarchar(MAX) not null
)
GO
CREATE TABLE Accounts(
Id int not null identity(1,1) primary key, 
Currency nvarchar(3) CHECK(Currency='USD' or Currency='ARS'),
User_Id int not null foreign key references Users(Id)
)
GO
CREATE TABLE Categories(
Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
Type NVARCHAR(50) NOT NULL UNIQUE,
Editable BIT NULL DEFAULT(1)
)
GO
INSERT INTO Categories(Type, Editable)
Values ('Regular', 1),
	   ('Buy currency',0),
	   ('Fixed term deposit',0),
	   ('Transfer',0)
GO
CREATE TABLE Transactions(
Id int not null identity(1,1) primary key,
Amount float not null check(Amount>0),
Concept nvarchar(50) not null,
Date Datetime not null DEFAULT(GETDATE()),
Type nvarchar(10) not null CHECK(Type='Topup' or Type='Payment'),
Account_Id int not null foreign key references Accounts(Id),
Category_Id INT NOT NULL DEFAULT(1) FOREIGN KEY REFERENCES Categories(Id)
)
GO
CREATE TABLE FixedTermDeposits(
Id int not null identity(1,1) primary key,
Amount float not null check(Amount>0),
Creation_Date Datetime not null DEFAULT(GETDATE()),
Closing_Date Datetime null,
Account_Id int not null FOREIGN KEY REFERENCES Accounts(Id)
)
GO
CREATE TABLE TransactionLog	(
Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
Transaction_Id INT NOT NULL	FOREIGN KEY REFERENCES Transactions(Id),
New_Value NVARCHAR(100) NOT NULL,
Modification_Date DATETIME NOT NULL DEFAULT(GETDATE())
)
GO
CREATE TABLE Rates (
    Id int not null identity(1,1) primary key,
    Date Datetime not null DEFAULT(GETDATE()),
    Selling_price float not null,
    Buying_price float not null
)
GO
CREATE TABLE RefundRequest(
Id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
Transaction_Id INT NOT NULL FOREIGN KEY REFERENCES Transactions(Id),
Status NVARCHAR(10) NOT NULL DEFAULT('Pending') 
	CHECK(Status = 'Accepted' OR Status = 'Canceled' OR Status = 'Rejected' OR Status = 'Pending'),
Source_Account_Id INT NOT NULL FOREIGN KEY REFERENCES Accounts(Id),
Target_Account_Id INT NOT NULL FOREIGN KEY REFERENCES Accounts(Id)
)
GO
CREATE TABLE Transfers(
Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
Origin_Transaction_Id INT NOT NULL FOREIGN KEY REFERENCES Transactions(Id),
Destination_Transaction_Id INT NOT NULL	FOREIGN KEY REFERENCES Transactions(Id)
)
GO
CREATE TABLE EmailTemplates(
Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
Title NVARCHAR(46),
Body NVARCHAR(4000) NOT NULL
)