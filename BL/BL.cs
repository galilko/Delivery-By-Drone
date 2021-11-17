using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBL.BO;


public class BL : IBL.IBL
{
    IDAL.IDal MyDal;
    internal static double BatteryUseFREE;
    internal static double BatteryUseLight;
    internal static double BatteryUseMedium;
    internal static double BatteryUseHeavy;
    internal static double BatteryChargeRate;
    internal static BL b;
    public BL()
    {

        MyDal = new DalObject.DalObject();
        BatteryUseFREE = MyDal.GetBatteryUse()[0];
        BatteryUseLight = MyDal.GetBatteryUse()[1];
        BatteryUseMedium = MyDal.GetBatteryUse()[2];
        BatteryUseHeavy = MyDal.GetBatteryUse()[3];
        BatteryChargeRate = MyDal.GetBatteryUse()[4];
        List<DroneToList>
        List<> Drones = (List<IDAL.DO.Drone>)MyDal.AllDrones();
        foreach (var item in Drones)
        {
            if
        }
    }
    public int aaa()
    { return 1; }
}
}

