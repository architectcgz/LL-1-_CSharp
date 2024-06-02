using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exp2_WPF
{
    public class StepInfo()
    {
        public int StepCount { get; set; } 
        public string AnalysisStack { get; set; } 
        public string RemainingInput { get; set; }
        public string ProductionUsed { get; set; } 
        public string Action { get; set; }
    }
}
