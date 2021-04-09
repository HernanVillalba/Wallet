USE [WALLET]
GO
SET IDENTITY_INSERT [dbo].[EmailTemplates] ON 

INSERT [dbo].[EmailTemplates] ([Id], [Title], [Body]) 
VALUES (1, N'Plazo fijo creado correctamente!', 
N'
El plazo fijo #{0} se ha cerrado correctamente!
<br>
<br>
Monto inicial: ${1}
<br>
Monto rescatado: ${2}
<br>
Intereses generados: ${3}
')

SET IDENTITY_INSERT [dbo].[EmailTemplates] OFF