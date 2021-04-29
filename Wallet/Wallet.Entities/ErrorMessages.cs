using System;
using System.Collections.Generic;
using System.Text;

namespace Wallet.Entities
{
    public static class ErrorMessages
    {
        public const string User_Data_Not_Found = "Datos del usuario no encontrados"; // id's
        public const string Incorrect_Data = "Datos ingresados incorrectos"; // input data by user
        public const string Not_Enough_Balance = "No hay suficiente dinero para realizar la operación"; // not balance
        public const string Resource_Not_Found = "Recurso no encontrado";
        public const string Operation_Cannot_Be_Performed = "No se puede realizar la operación";
        // TODO: complete
    }
}
