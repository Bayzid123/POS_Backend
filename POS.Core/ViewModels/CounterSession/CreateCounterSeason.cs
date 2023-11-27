using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POS.Core.Models;

namespace POS.Core.ViewModels.CounterSession;
public class CreateCounterSeason
{
    public CounterSessionDTO Session
    {
        get; set;
    }
    public List<CounterSessionDetailsDTO> SessionDetails
    {
        get; set;
    }
}
public class CreateCounterSessionDetails
{
    public CounterSessionDTO session
    {
        get; set;
    }
    public List<CounterSessionDetailsDTO> details
    {
        get; set;
    }

}
