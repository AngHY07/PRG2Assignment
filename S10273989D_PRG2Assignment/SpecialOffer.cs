using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//==========================================================
// Student Number : S10273989D
// Student Name : Ang Hao Yi
// Partner Name : Choo Yi Zehn
//==========================================================
namespace S10273989D_PRG2Assignment
{
    internal class SpecialOffer
    {
        public string OfferCode { get; set; }

        public string OfferDesc { get; set; }

        public double Discount { get; set; }

        public SpecialOffer(string offercode, string offerdesc, double discount)
        {
            this.OfferCode = offercode;
            this.OfferDesc = offerdesc;
            this.Discount = discount;
        }

    }
}
