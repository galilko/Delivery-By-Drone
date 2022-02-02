using DO;
using System;
using System.Collections.Generic;

namespace ConsoleDal
{
    class Program
    {
        public static void myPrint<T>(T t)
        {
            Console.WriteLine(t.ToString(), Console.ForegroundColor = ConsoleColor.Green);
        }
        static void Main(string[] args)
        {

            int choice1 = 0, choice2 = 1;
            bool properConversion;
            DalApi.IDal dal = DalApi.DalFactory.GetDal();
            do
            {
                Console.WriteLine(@"---------------------------------
MAIN MENU
---------------------------------
FOR ADDING PRESS 1
FOR UPDATING PRESS 2
FOR VIEWING PRESS 3
FOR LIST-VIEWING PRESS 4
FOR EXIT PRESS 5
---------------------------------");
                properConversion = int.TryParse(Console.ReadLine(), out choice1);
                switch (choice1)
                {
                    case 1:
                        {
                            Console.WriteLine(@"---------------------------------
FOR ADDING A BASE STATION PRESS 1
FOR ADDING A DRONE PRESS 2
FOR ADDING A CUSTOMER PRESS 3
FOR ADDING A PARCEL PRESS 4
---------------------------------");
                            properConversion = int.TryParse(Console.ReadLine(), out choice2);
                            Console.WriteLine("---------------------------------");
                            switch (choice2)
                            {
                                case 1:
                                    {
                                        Console.WriteLine("ENTER BASE-STATION DETAILS\n" +
                                            "---------------------------------");
                                        Console.Write("ID:\t\t");
                                        int.TryParse(Console.ReadLine(), out int id);
                                        Console.Write("Name:\t");
                                        string name = Console.ReadLine();
                                        Console.Write("Latitude:\t");
                                        double.TryParse(Console.ReadLine(), out double latitude);
                                        Console.Write("Longitude:\t");
                                        double.TryParse(Console.ReadLine(), out double longitude);
                                        Console.Write("Number of free charger slots:\t");
                                        int.TryParse(Console.ReadLine(), out int freeSlots);
                                        BaseStation myBaseStation = new() { Id = id, Name = name, Latitude = latitude, Longitude = longitude, FreeChargeSlots = freeSlots };
                                        dal.AddBaseStation(myBaseStation);
                                        break;
                                    }
                                case 2:
                                    {
                                        Console.WriteLine("ENTER DRONE DETAILS\n" +
                                            "---------------------------------");
                                        Console.Write("Id:\t");
                                        int.TryParse(Console.ReadLine(), out int id);
                                        Console.Write("Model:\t");
                                        string model = Console.ReadLine();
                                        Console.Write("Weight category [1-Light|2-Medium|3-Heavy]:\t");
                                        int.TryParse(Console.ReadLine(), out int weightCat);
                                        WeightCategories maxWeight = (WeightCategories)weightCat;
                                        Drone myDrone = new() { Id = id, Model = model, MaxWeight = maxWeight };
                                        dal.AddDrone(myDrone);
                                        break;
                                    }
                                case 3:
                                    {
                                        Console.WriteLine("ENTER CUSTOMER DETAILS\n" +
                                           "---------------------------------");
                                        Console.Write("Id:\t");
                                        int.TryParse(Console.ReadLine(), out int id);
                                        Console.Write("Name:\t");
                                        string name = Console.ReadLine();
                                        Console.Write("Phone:\t");
                                        string phone = Console.ReadLine();
                                        Console.Write("Latitude:\t");
                                        double.TryParse(Console.ReadLine(), out double latitude);
                                        Console.Write("Longitude:\t");
                                        double.TryParse(Console.ReadLine(), out double longitude);
                                        Customer myCustomer = new() { Id = id, Name = name, Phone = phone, Latitude = latitude, Longitude = longitude };
                                        dal.AddCustomer(myCustomer);
                                        break;
                                    }
                                case 4:
                                    {
                                        Parcel myParcel = new();
                                        Console.WriteLine("ENTER PARCEL DETAILS\n" +
                                          "---------------------------------");
                                        Console.Write("SENDER-ID:\t");
                                        int.TryParse(Console.ReadLine(), out int senderId);
                                        myParcel.SenderId = senderId;
                                        Console.Write("TARGET-ID:\t");
                                        int.TryParse(Console.ReadLine(), out int targetId);
                                        myParcel.TargetId = targetId;
                                        Console.Write("Weight category [1-Light|2-Medium|3-Heavy]:\t");
                                        int.TryParse(Console.ReadLine(), out int weightCat);
                                        myParcel.Weight = (WeightCategories)weightCat;
                                        Console.Write("Priority [1-Normal|2-Fast|3-Emengercy]:\t");
                                        int.TryParse(Console.ReadLine(), out int priority);
                                        myParcel.Priority = (Priorities)priority;
                                        dal.AddParcel(myParcel);
                                        break;
                                    }
                                default:
                                    break;
                            }
                            break;
                        }
                    case 2:
                        {
                            Console.WriteLine(@"-------------------------------
FOR ASSINGING A PARCEL TO A DRONE PRESS 1
FOR PICK UP A PARCEL BY DRONE PRESS 2
FOR DELIVERING A PARCEL TO THE CUSTOMER PRESS 3
FOR SENDING A DRONE FOR CHARGING AT BASE STATION PRESS 4
FOR RELEASING A DRONE FROM CHARGING PRESS 5
-------------------------------- ");
                            properConversion = int.TryParse(Console.ReadLine(), out choice2);
                            Console.WriteLine("---------------------------------");
                            switch (choice2)
                            {
                                case 1:
                                    {
                                        Console.WriteLine("ENTER PARCEL ID AND DRONE ID FOR SCHEDULING:");
                                        Console.WriteLine("---------------------------------");
                                        Console.Write("PARCEL ID:\t");
                                        int.TryParse(Console.ReadLine(), out int parcelId);
                                        Console.Write("DRONE ID:\t");
                                        int.TryParse(Console.ReadLine(), out int droneId);
                                        dal.ScheduleDroneForParcel(parcelId, droneId);
                                        break;
                                    }
                                case 2:
                                    {
                                        Console.Write("ENTER PARCEL ID FOR PICKING UP:\t");
                                        int.TryParse(Console.ReadLine(), out int parcelId);
                                        dal.PickingUpAParcel(parcelId);
                                        break;
                                    }
                                case 3:
                                    {
                                        Console.Write("ENTER PARCEL ID FOR DELIVERY:\t");
                                        int.TryParse(Console.ReadLine(), out int parcelId);
                                        dal.DeliverAParcel(parcelId);
                                        break;
                                    }
                                case 4:
                                    {
                                        List<BaseStation> myList = (List<BaseStation>)dal.GetBaseStations(b => b?.FreeChargeSlots > 0);
                                        if (myList.Count <= 0)
                                            Console.WriteLine("THERE AREN'T BASE-STATIONS WITH FREE SLOTS", Console.ForegroundColor = ConsoleColor.Red);
                                        else
                                        {
                                            myList.ForEach(myPrint);
                                            Console.ResetColor();
                                            Console.WriteLine("ENTER DRONE ID AND BASE-STATION ID FOR CHARGING:\t");
                                            Console.WriteLine("---------------------------------");
                                            Console.Write("DRONE ID:\t");
                                            int.TryParse(Console.ReadLine(), out int droneId);
                                            Console.Write("BASE-STATION ID:\t");
                                            int.TryParse(Console.ReadLine(), out int baseStationId);
                                            Console.WriteLine("---------------------------------");
                                            dal.ChargeDrone(droneId, baseStationId);
                                        }
                                        Console.ResetColor();
                                        break;
                                    }
                                case 5:
                                    {
                                        Console.WriteLine("ENTER DRONE ID FOR RELEASING FROM CHARGE:\t");
                                        int.TryParse(Console.ReadLine(), out int droneId);
                                        dal.ReleaseDroneFromCharge(droneId);
                                        break;
                                    }
                                default:
                                    break;
                            }
                            break;
                        }
                    case 3:
                        {
                            Console.WriteLine(@"---------------------------------
FOR VIEWING A BASE STATION PRESS 1
FOR VIEWING A DRONE PRESS 2
FOR VIEWING A CUSTOMER PRESS 3
FOR VIEWING A PARCEL PRESS 4
---------------------------------");
                            properConversion = int.TryParse(Console.ReadLine(), out choice2);
                            Console.WriteLine("---------------------------------");
                            switch (choice2)
                            {
                                case 1:
                                    {
                                        Console.Write("ENTER BASE-STATION ID FOR VIEWING:\t");
                                        int.TryParse(Console.ReadLine(), out int baseStationId);
                                        Console.WriteLine("---------------------------------");
                                        BaseStation myBase = dal.GetBaseStation(baseStationId);
                                        if (myBase.Equals(default(BaseStation)))
                                            Console.Write($"BASE-STATION {baseStationId} WASN'T FOUND", Console.ForegroundColor = ConsoleColor.Red);

                                        else
                                            Console.Write(myBase.ToString(), Console.ForegroundColor = ConsoleColor.Green);
                                        Console.ResetColor(); break;
                                    }
                                case 2:
                                    {
                                        Console.Write("ENTER DRONE ID FOR VIEWING:\t");
                                        int.TryParse(Console.ReadLine(), out int droneId);
                                        Console.WriteLine("---------------------------------");
                                        Drone myDrone = dal.GetDrone(droneId);
                                        if (myDrone.Equals(default(Drone)))
                                            Console.WriteLine($"DRONE {droneId} WASN'T FOUND", Console.ForegroundColor = ConsoleColor.Red);

                                        else
                                            Console.WriteLine(myDrone.ToString(), Console.ForegroundColor = ConsoleColor.Green);
                                        Console.ResetColor(); break;
                                    }
                                case 3:
                                    {
                                        Console.Write("ENTER CUSTOMER ID FOR VIEWING:\t");
                                        int.TryParse(Console.ReadLine(), out int customerId);
                                        Console.WriteLine("---------------------------------");
                                        Customer myCustomer = dal.GetCustomer(customerId);
                                        if (myCustomer.Equals(default(Customer)))
                                            Console.WriteLine($"CUSTOMER {customerId} WASN'T FOUND", Console.ForegroundColor = ConsoleColor.Red);

                                        else
                                            Console.WriteLine(myCustomer.ToString(), Console.ForegroundColor = ConsoleColor.Green);
                                        Console.ResetColor();
                                        break;
                                    }
                                case 4:
                                    {
                                        Console.Write("ENTER PARCEL ID FOR VIEWING:\t");
                                        int.TryParse(Console.ReadLine(), out int parcelId);
                                        Console.WriteLine("---------------------------------");
                                        Parcel myParcel = dal.GetParcel(parcelId);
                                        if (myParcel.Equals(default(Parcel)))
                                            Console.WriteLine($"PARCEL {parcelId} WASN'T FOUND", Console.ForegroundColor = ConsoleColor.Red);
                                        else
                                            Console.WriteLine(myParcel.ToString(), Console.ForegroundColor = ConsoleColor.Green);
                                        Console.ResetColor();
                                        break;
                                    }
                                default:
                                    break;
                            }
                            break;
                        }
                    case 4:
                        {
                            Console.WriteLine(@"---------------------------------
FOR VIEWING ALL BASE-STATIONS PRESS 1
FOR VIEWING ALL DRONES PRESS 2
FOR VIEWING ALL CUSTOMERS PRESS 3
FOR VIEWING ALL PARCELS PRESS 4
FOR VIEWING ALL NON-SCHEDULED PARCELS PRESS 5
FOR VIEWING ALL FREE BASE-STATIONS PRESS 6
---------------------------------");
                            properConversion = int.TryParse(Console.ReadLine(), out choice2);
                            Console.WriteLine("---------------------------------");
                            switch (choice2)
                            {
                                case 1:
                                    {
                                        ((List<BaseStation>)dal.GetBaseStations()).ForEach(myPrint);
                                        Console.ResetColor();
                                        break;
                                    }
                                case 2:
                                    {
                                        ((List<Drone>)dal.GetDrones()).ForEach(myPrint);
                                        Console.ResetColor();
                                        break;
                                    }
                                case 3:
                                    {
                                        ((List<Customer>)dal.GetCustomers()).ForEach(myPrint);
                                        Console.ResetColor();
                                        break;
                                    }
                                case 4:
                                    {
                                        ((List<Parcel>)dal.GetParcels()).ForEach(myPrint);
                                        Console.ResetColor();
                                        break;
                                    }
                                case 5:
                                    {
                                        List<Parcel> myList = (List<Parcel>)dal.GetParcels(x=>x?.Scheduled == null);
                                        if (myList.Count > 0)
                                            myList.ForEach(myPrint);
                                        else
                                            Console.WriteLine("THERE AREN'T NONE-SCHEDULED PARCELS", Console.ForegroundColor = ConsoleColor.Red);
                                        Console.ResetColor();
                                        break;
                                    }
                                case 6:
                                    {
                                        List<BaseStation> myList = (List<BaseStation>)dal.GetBaseStations(b=>b?.FreeChargeSlots > 0);
                                        if (myList.Count > 0)
                                            myList.ForEach(myPrint);
                                        else
                                            Console.WriteLine("THERE AREN'T BASE-STATIONS WITH FREE SLOTS", Console.ForegroundColor = ConsoleColor.Red);
                                        Console.ResetColor();
                                        break;
                                    }
                                default:
                                    break;
                            }
                            break;

                        }
                    case 5:
                        {
                            Console.WriteLine("---------------------------------\nGOOD-BYE");
                            break;
                        }
                    default:
                        break;
                }
            }
            while (choice1 != 5);

        }
    }
}
