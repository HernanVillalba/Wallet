using System;
using System.Collections.Generic;
using System.Text;

namespace Wallet.Entities
{
    public class InterestsCalculationModel
    {
        public string moneda { get; set; }
        public double montoInicial { get; set; }
        public double montoFinal { get; set; }
        public double intereses { get; set; }
    }
}
