using System;
using System.Collections;
using System.Collections.Generic;

public static class LogicMetricGlobalSetup
{
    /*
 !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
 !!!!         LOGIC AND METRICS GET LOGGED HERE         !!!!    
 !!!!                PLEASE FOLLOW SUIT                 !!!!
 !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
*/

    public static List<ILogic> LogicMapper = new List<ILogic>()
    {
        new DistanceLogic(),
        new DirectionLogic(),
        new VelocityLogic(),
        new DampeningLogic(),
    };

    public static List<IMetric> MetricMapper = new List<IMetric>()
    {
        new DistanceMetric(),
        new DirectionMetric(),
        new VelocityMetric(),
    };

    public static ILogic CreateLogicInstance(int type, float weight)
    {
        switch (type)
        {
            case 0: return new DistanceLogic() { Weight = weight };
            case 1: return new DirectionLogic() { Weight = weight };
            case 2: return new VelocityLogic() { Weight = weight };
            case 3: return new DampeningLogic() { Weight = 1, SmoothingFactor = weight };
            default: throw new ArgumentOutOfRangeException(nameof(type), "Invalid type index.");
        }
    }

    /*
      !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
      !!!!         END OF LOGIC METRIC ADDITIONS             !!!!
      !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    */
}
