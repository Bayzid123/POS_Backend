using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.ViewModels;
public class OfficeList
{
    public long Id
    {
        get; set;
    }
    public string Name
    {
        get; set;
    }
    public string Address
    {
        get; set;
    }
    public List<Counter> CounterList
    {
        get; set;
    }
}



public class Counter
{
    public long Id
    {
        get; set;
    }
    public string Name
    {
        get; set;
    }
   
}