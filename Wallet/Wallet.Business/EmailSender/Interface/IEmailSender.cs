using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Business.EmailSender.Interface
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);

        /*
         * 
         * Tabla: EmailTemplates
         * -Id (PK - int, autoincremental)
         * -Title (nvarchar(46)) -> ej: Cierre de plazo fijo #{0}
         * -Body (nvarchar(4000)) -> ej: Hola {0}! El plazo fijo #{1} ha sido cerrado!
         * 
         * Crear un email:
         * 1) Escribir al final del Script_Email_Templates el INSERT correspondiente al mail,
         *    con {0}, {1}, ... , {n} según corresponda (el body comienza con {0} por mas 
         *    que el titulo ya contenga parametros porque se procesan por separado)
         * 2) Al ser autoincremental, ir al ENUM_MAIL_TEMPLATES y agregar el campo del nuevo
         *    template al final del mismo, respetando el orden del script
         * 3) Correr el make.bat para resetear la base (o ejecutar el Script_EmailTemplates)
         * 4) Ya se tiene el template registrado, solo resta usarlo
         * 
         * Uso:
         * 1) La clase Business que tenga que enviar emails va a tener que ser inyectada 
         *    con la interfaz IEmailSender
         * 2) Suponiendo que la inyeccion de dependencia se hizo sobre el miembro 
         *    _emailSender, el uso es el siguiente:
         *    
         *    EmailTemplate emailTemplate = _unitOfWork.EmailTemplates.GetById((int)EmailTemplatesEnum.Tipo_De_Email);
         *    - Revisar en la base de datos el template a ver qué parámetros se requieren,
         *      tanto en el titulo como en el cuerpo del email
         *    string title = String.Format(emailTemplate.Title, <parametros> );
         *    string body = String.Format(emailTemplate.Body, <parametros> );
         *    await _emailSender.SendEmailAsync(email, title, body);
         */
    }
}
