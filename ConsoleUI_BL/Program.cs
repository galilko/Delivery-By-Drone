using System;
using IBL.BO;
using System.Collections.Generic;
namespace ConsoleUI_BL
{
    class Program
    {
        public static void myPrint<T>(T t)
        {
            Console.WriteLine(t.ToString(), Console.ForegroundColor = ConsoleColor.Green);
        }
        static void Main(string[] args)
        {
            
            string msg;
            int choice1 = 0, choice2 = 1;
            bool properConversion;
            BL myBl = new();
            do
            {
                try
                {
                    msg = "";
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
                    if (properConversion)
                    {
                        switch (choice1)
                        {
                            #region ADD
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
                                    if (properConversion)
                                    {
                                        switch (choice2)
                                        {
                                            case 1:
                                                {

                                                    BaseStation myBaseStation = new();
                                                    Console.WriteLine("ENTER BASE-STATION DETAILS\n" +
                                                        "---------------------------------");
                                                    Console.Write("ID:\t\t");
                                                    properConversion = int.TryParse(Console.ReadLine(), out int id);
                                                    if (!properConversion) { throw new Exception("Invalid ID"); }
                                                    myBaseStation.Id = id;
                                                    Console.Write("Name:\t");
                                                    myBaseStation.Name = Console.ReadLine();
                                                    Console.Write("Latitude:\t");
                                                    properConversion = double.TryParse(Console.ReadLine(), out double latitude);
                                                    if (!properConversion) { throw new Exception("Invalid latitude"); }
                                                    Console.Write("Longitude:\t");
                                                    properConversion = double.TryParse(Console.ReadLine(), out double longitude);
                                                    if (!properConversion) { throw new Exception("Invalid longitude"); }
                                                    myBaseStation.BSLocation = new Location(latitude, longitude);
                                                    Console.Write("Number of free charger slots:\t");
                                                    int.TryParse(Console.ReadLine(), out int freeSlots);
                                                    if (!properConversion) { throw new Exception("Invalid number"); }
                                                    myBaseStation.FreeChargeSlots = freeSlots;
                                                    myBl.AddBaseStation(myBaseStation);

                                                    break;
                                                }
                                            case 2:
                                                {

                                                    DroneToList myDrone = new();
                                                    Console.WriteLine("ENTER DRONE DETAILS\n" +
                                                        "---------------------------------");
                                                    Console.Write("Id:\t");
                                                    properConversion = int.TryParse(Console.ReadLine(), out int id);
                                                    if (!properConversion) { throw new Exception("Invalid ID"); }
                                                    myDrone.Id = id;
                                                    Console.Write("Model:\t");
                                                    myDrone.Model = Console.ReadLine();
                                                    Console.Write("Weight category [1-Light|2-Medium|3-Heavy]:\t");
                                                    properConversion = int.TryParse(Console.ReadLine(), out int weightCat);
                                                    if (!properConversion) { throw new Exception("Invalid Number"); }
                                                    myDrone.Weight = (WeightCategories)weightCat;
                                                    Console.Write("Please enter base station ID to enter:\t");
                                                    properConversion = int.TryParse(Console.ReadLine(), out int baseStationId);
                                                    if (!properConversion) { throw new Exception("Invalid Number"); }
                                                    /*Console.Write("Status:\t");
                                                    if (!Enum.TryParse(Console.ReadLine(), out DroneStatusCategories status))
                                                    {
                                                        Console.WriteLine("---------------------------------\n" +
                                                            "ERROR - invalid status category");
                                                        break;
                                                    }
                                                    Console.Write("Battery:\t");
                                                    double.TryParse(Console.ReadLine(), out double battery);*/

                                                    myBl.AddDrone(myDrone, baseStationId);

                                                    break;
                                                }
                                            case 3:
                                                {
                                                    Customer myCustomer = new();
                                                    Console.WriteLine("ENTER CUSTOMER DETAILS\n" +
                                                       "---------------------------------");
                                                    Console.Write("Id:\t");
                                                    properConversion = int.TryParse(Console.ReadLine(), out int id);
                                                    if (!properConversion) { throw new Exception("Invalid ID"); }
                                                    myCustomer.Id = id;
                                                    Console.Write("Name:\t");
                                                    if (!properConversion) { throw new Exception("Invalid ID"); }
                                                    myCustomer.Name = Console.ReadLine();
                                                    Console.Write("Phone:\t");
                                                    myCustomer.PhoneNumber = Console.ReadLine();
                                                    Console.Write("Latitude:\t");
                                                    properConversion = double.TryParse(Console.ReadLine(), out double latitude);
                                                    if (!properConversion) { throw new Exception("Invalid number"); }
                                                    myCustomer.CustomerLocation.Latitude = latitude;
                                                    Console.Write("Longitude:\t");
                                                    properConversion = double.TryParse(Console.ReadLine(), out double longitude);
                                                    if (!properConversion) { throw new Exception("Invalid number"); }
                                                    myCustomer.CustomerLocation.Longitude = longitude;
                                                    myBl.AddCustomer(myCustomer);
                                                    break;
                                                }
                                            case 4:
                                                {
                                                    Parcel myParcel = new();
                                                    myParcel.Sender = new CustomerAtParcel();
                                                    myParcel.Target = new CustomerAtParcel();
                                                    Console.WriteLine("ENTER PARCEL DETAILS\n" +
                                                      "---------------------------------");
                                                    Console.Write("SENDER-ID:\t");
                                                    properConversion = int.TryParse(Console.ReadLine(), out int senderId);
                                                    if (!properConversion) { throw new Exception("Invalid number"); }
                                                    myParcel.Sender.Id = senderId;
                                                    Console.Write("TARGET-ID:\t");
                                                    int.TryParse(Console.ReadLine(), out int targetId);
                                                    myParcel.Target.Id = targetId;
                                                    Console.Write("Weight category [1-Light|2-Medium|3-Heavy]:\t");
                                                    int.TryParse(Console.ReadLine(), out int weightCat);
                                                    myParcel.Weight = (WeightCategories)weightCat;
                                                    Console.Write("Priority [1-Normal|2-Fast|3-Emengercy]:\t");
                                                    int.TryParse(Console.ReadLine(), out int priority);
                                                    myParcel.Priority = (Priorities)priority;
                                                    myBl.AddParcel(myParcel);
                                                    break;
                                                }
                                            default:
                                                break;
                                        }
                                    }
                                    else
                                        Console.WriteLine("WRONG CHOICE");
                                    break;
                                }
                            #endregion
                            
                        case 2:
                            {
                                Console.WriteLine(@"-------------------------------
FOR UPDATE THE DRONE MODEL PRESS 1
FOR UPDATE THE BASE STATION DETAILS PRESS 2
FOR UPDAT THE CUSTOMER DETAILS PRESS 3
FOR ASSINGING A PARCEL TO A DRONE PRESS 4
FOR PICK UP A PARCEL BY DRONE PRESS 5
FOR DELIVERING A PARCEL TO THE CUSTOMER PRESS 6
FOR SENDING A DRONE FOR CHARGING AT BASE STATION PRESS 7
FOR RELEASING A DRONE FROM CHARGING PRESS 8
-------------------------------- ");
                                properConversion = int.TryParse(Console.ReadLine(), out choice2);
                                Console.WriteLine("---------------------------------");
                                switch (choice2)
                                    {
                                    case 1:
                                        {
                                             properConversion = int.TryParse(Console.ReadLine(), out int id);
                                             if (!properConversion) { throw new Exception("Invalid ID"); }

                                             break;
                                        }
                                    case 4:
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

                                    case 5:
                                        {
                                            Console.Write("ENTER PARCEL ID FOR PICKING UP:\t");
                                            int.TryParse(Console.ReadLine(), out int parcelId);
                                            dal.PickingUpAParcel(parcelId);
                                            break;
                                        }
                                    case 6:
                                        {
                                            Console.Write("ENTER PARCEL ID FOR DELIVERY:\t");
                                            int.TryParse(Console.ReadLine(), out int parcelId);
                                            dal.DeliverAParcel(parcelId);
                                            break;
                                        }
                                    case 7:
                                        {
                                            List<BaseStation> myList = (List<BaseStation>)dal.FreeSlotsBaseStations();
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
                                    case 8:
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
                      /*  case 3:
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
                                            BaseStation myBase = myBl.FindBaseStation(baseStationId);
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
                                            Drone myDrone = dal.FindDrone(droneId);
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
                                            Customer myCustomer = dal.FindCustomer(customerId);
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
                                            Parcel myParcel = dal.FindParcel(parcelId);
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
                            }*/
                            #region print lists
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
                                                ((List<BaseStationToList>)myBl.AllBlBaseStations()).ForEach(myPrint);
                                                Console.ResetColor();
                                                break;
                                            }
                                    case 2:
                                        {
                                                ((List<DroneToList>)myBl.AllBlDrones()).ForEach(myPrint);
                                                Console.ResetColor();
                                                break;
                                            }
                                    case 3:
                                        {
                                            ((List<CustomerToList>)myBl.AllBlCustomers()).ForEach(myPrint);
                                            Console.ResetColor();
                                            break;
                                        }
                                    case 4:
                                        {
                                            ((List<ParcelToList>)myBl.AllBlParcels()).ForEach(myPrint);
                                            Console.ResetColor();
                                            break;
                                        }
                                    case 5:
                                        {
                                           ((List<ParcelToList>)myBl.NoneScheduledParcels()).ForEach(myPrint);
                                            
                                            Console.ResetColor();
                                            break;
                                        }
                                    case 6:
                                        {
                                            ((List<BaseStationToList>)myBl.FreeSlotsBaseStations()).ForEach(myPrint);
                                          
                                            Console.ResetColor();
                                            break;
                                        }
                                    default:
                                        break;
                                }
                                break;

                            }
                            #endregion
                        case 5:
                            {
                                Console.WriteLine("---------------------------------\nGOOD-BYE");
                                break;
                            }
                            default:
                                break;
                        }
                    }
                    else
                        Console.WriteLine("WRONG CHOICE");
                }

                catch (Exception ex)
                {
                    msg = $"{ex.Message}\n";
                    while (ex.InnerException != null)
                    {
                        ex = ex.InnerException;
                        msg += $"{ex.Message}\n";
                    }
                    Console.WriteLine(msg);
                }
            }
            while (choice1 != 5);

        }
    }
}
