using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokémon.ViewModel.Services
{
    public class ErrorLogService
    {
        public string ErrorMsg { get; set; }
        public string Stacktrace { get; set; }
        public DateTime TimeStamp { get; set; }


    }
}
