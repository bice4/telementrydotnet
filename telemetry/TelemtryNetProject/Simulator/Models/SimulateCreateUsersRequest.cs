using System.ComponentModel.DataAnnotations;

namespace Simulator.Models;

public class SimulateCreateUsersRequest
{
    [Range(0, Int32.MaxValue, ErrorMessage = "Please enter a value bigger than {1}.")]
    public int UserCount { get; set; }

    public float ErrorRate { get; set; }
    
    public int? DelayInSec { get; set; }

    public bool? InParallel { get; set; }


    public SimulateCreateUsersRequest(int userCount, float errorRate, int? delayInSec, bool? inParallel)
    {
        UserCount = userCount;
        ErrorRate = errorRate;
        DelayInSec = delayInSec;
        InParallel = inParallel;
    }
}