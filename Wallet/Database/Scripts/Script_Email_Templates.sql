USE [WALLET]
GO
INSERT [dbo].[EmailTemplates] ( [Title], [Body]) 
VALUES (N'Plazo fijo cerrado correctamente!', 
N'<h1>El plazo fijo #{0} se ha cerrado correctamente!</h1>
<br>
Monto inicial: ${1}
<br>
Monto rescatado: ${2}
<br>
Intereses generados: ${3}')

GO
INSERT [dbo].[EmailTemplates] ([Title], [Body]) 
VALUES (N'Solicitud de reembolso', 
N'<h1>Solicitud de reembolso</h1> 
<p>El usuario ID {0} {1}, te ha solicitado que le reembolses ${2} por la transacción de ID {3}</p>')
