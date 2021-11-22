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
                    if (!int.TryParse(Console.ReadLine(), out choice1))
                        throw new InputException("WRONG INPUT");
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
                                if (!int.TryParse(Console.ReadLine(), out choice2))
                                    throw new InputException("WRONG INPUT");
                                switch (choice2)
                                {
                                    case 1:
                                        {
                                            BaseStation myBaseStation = new();
                                            Console.WriteLine("ENTER BASE-STATION DETAILS\n" +
                                                "---------------------------------");
                                            Console.Write("ID:\t\t");
                                            if (!int.TryParse(Console.ReadLine(), out int id)) { throw new InputException("Invalid ID"); }
                                            myBaseStation.Id = id;
                                            Console.Write("Name:\t");
                                            myBaseStation.Name = Console.ReadLine();
                                            Console.Write("Latitude:\t");
                                            if (!double.TryParse(Console.ReadLine(), out double latitude)) { throw new InputException("Invalid latitude"); }
                                            Console.Write("Longitude:\t");
                                            if (!double.TryParse(Console.ReadLine(), out double longitude)) { throw new InputException("Invalid longitude"); }
                                            myBaseStation.BSLocation = new Location(latitude, longitude);
                                            Console.Write("Number of free charger slots:\t");
                                            if (!int.TryParse(Console.ReadLine(), out int freeSlots)) { throw new InputException("Invalid number"); }
                                            if (freeSlots < 0) throw new InputException("Free slots count can't be nagative");
                                            myBaseStation.FreeChargeSlots = freeSlots;
                                            myBl.AddBaseStation(myBaseStation);
                                            Console.WriteLine($"---------------------------------\n" +
                                                $"Base Station {id} was added successfully\n" +
                                                $"---------------------------------", Console.ForegroundColor = ConsoleColor.Green);
                                            Console.ResetColor();
                                            break;
                                        }
                                    case 2:
                                        {
                                            DroneToList myDrone = new();
                                            Console.WriteLine("ENTER DRONE DETAILS\n" +
                                                "---------------------------------");
                                            Console.Write("Id:\t");
                                            if (!int.TryParse(Console.ReadLine(), out int id)) { throw new InputException("Invalid ID"); }
                                            myDrone.Id = id;
                                            Console.Write("Model:\t");
                                            myDrone.Model = Console.ReadLine();
                                            Console.Write("Weight category [1-Light|2-Medium|3-Heavy]:\t");
                                            if (!int.TryParse(Console.ReadLine(), out int weightCat)) { throw new InputException("Invalid Number"); }
                                            myDrone.Weight = (WeightCategories)weightCat;
                                            Console.Write("Please enter base station ID to enter:\t");
                                            if (!int.TryParse(Console.ReadLine(), out int baseStationId)) { throw new InputException("Invalid Number"); }
                                            myBl.AddDrone(myDrone, baseStationId);
                                            Console.WriteLine($"---------------------------------\n" +
                                                $"Drone {id} was added successfully\n" +
                                                $"---------------------------------", Console.ForegroundColor = ConsoleColor.Green);
                                            Console.ResetColor();
                                            break;
                                        }
                                    case 3:
                                        {
                                            Customer myCustomer = new();
                                            Console.WriteLine("ENTER CUSTOMER DETAILS\n" +
                                               "---------------------------------");
                                            Console.Write("Id:\t");
                                            if (!int.TryParse(Console.ReadLine(), out int id)) { throw new InputException("Invalid ID"); }
                                            myCustomer.Id = id;
                                            Console.Write("Name:\t");
                                            myCustomer.Name = Console.ReadLine();
                                            Console.Write("Phone:\t");
                                            myCustomer.PhoneNumber = Console.ReadLine();
                                            Console.Write("Latitude:\t");
                                            if (!double.TryParse(Console.ReadLine(), out double latitude)) { throw new InputException("Invalid latitude"); }
                                            myCustomer.CustomerLocation.Latitude = latitude;
                                            Console.Write("Longitude:\t");
                                            if (!double.TryParse(Console.ReadLine(), out double longitude)) { throw new InputException("Invalid longitude"); }
                                            myCustomer.CustomerLocation.Longitude = longitude;
                                            myBl.AddCustomer(myCustomer);
                                            Console.WriteLine($"---------------------------------\n" +
                                                $"Customer {id} was added successfully\n" +
                                                $"---------------------------------", Console.ForegroundColor = ConsoleColor.Green);
                                            Console.ResetColor();
                                            break;
                                        }
                                    case 4:
                                        {
                                            Parcel myParcel = new();
                                            Console.WriteLine("ENTER PARCEL DETAILS\n" +
                                              "---------------------------------");
                                            Console.Write("SENDER-ID:\t");
                                            if (!int.TryParse(Console.ReadLine(), out int senderId)) { throw new InputException("Invalid sender Id"); }
                                            myParcel.Sender = new() { Id = senderId };
                                            Console.Write("TARGET-ID:\t");
                                            if (!int.TryParse(Console.ReadLine(), out int targetId)) { throw new InputException("Invalid target Id"); }
                                            myParcel.Target = new() { Id = targetId };
                                            Console.Write("Weight category [1-Light|2-Medium|3-Heavy]:\t");
                                            if (!int.TryParse(Console.ReadLine(), out int weightCat)) { throw new InputException("Invalid Weight Categories"); }
                                            myParcel.Weight = (WeightCategories)weightCat;
                                            Console.Write("Priority [1-Normal|2-Fast|3-Emengercy]:\t");
                                            if (!int.TryParse(Console.ReadLine(), out int priority)) { throw new InputException("Invalid Priority Categories"); }
                                            myParcel.Priority = (Priorities)priority;
                                            myBl.AddParcel(myParcel);
                                            Console.WriteLine($"---------------------------------\n" +
                                                $"Parcel was added successfully\n" +
                                                $"---------------------------------", Console.ForegroundColor = ConsoleColor.Green);
                                            Console.ResetColor();
                                            break;
                                        }
                                    default:
                                        throw new InputException("WRONG CHOICE");
                                        break;
                                }
                                break;
                            }
                        #endregion
                        #region UPDATE
                        case 2:
                            {
                                Console.WriteLine(@"-------------------------------
FOR UPDATING DRONE DETAILS PRESS 1
FOR UPDATING BASE-STATION DETAILS PRESS 2
FOR UPDATING CUSTOMER DETAILS PRESS 3
FOR SENDING A DRONE FOR CHARGING AT BASE STATION PRESS 4
FOR RELEASING A DRONE FROM CHARGING PRESS 5
FOR ASSINGING A PARCEL TO A DRONE PRESS 6
FOR PICK UP A PARCEL BY DRONE PRESS 7
FOR DELIVERING A PARCEL TO THE CUSTOMER PRESS 8
-------------------------------- ");
                                if (!int.TryParse(Console.ReadLine(), out choice2))
                                    throw new InputException("WRONG INPUT");
                                Console.WriteLine("---------------------------------");
                                switch (choice2)
                                {
                                    case 1:
                                        {
                                            Console.WriteLine("ENTER DRONE ID AND NEW MODEL FOR UPDATING:");
                                            Console.WriteLine("---------------------------------");
                                            Console.Write("DRONE ID:\t");
                                            if (!int.TryParse(Console.ReadLine(), out int droneId)) throw new InputException("Invalid Drone Id");
                                            Console.Write("NEW MODEL NAME:\t");
                                            string newName = Console.ReadLine();
                                            myBl.UpdateDroneModel(droneId, newName);
                                            Console.WriteLine($"---------------------------------\n" +
                                                              $"Drone {droneId} was updated successfully\n" +
                                                              $"---------------------------------", Console.ForegroundColor = ConsoleColor.Green);
                                            Console.ResetColor();
                                            break;
                                        }
                                    case 2:
                                        {
                                            Console.WriteLine("ENTER BASE STATION ID FOR UPDATING:");
                                            Console.WriteLine("---------------------------------");
                                            Console.Write("BASE STATION ID:\t");
                                            if (!int.TryParse(Console.ReadLine(), out int baseStationId)) throw new InputException("Invalid BaseStation Id");
                                            Console.Write("NEW BASE-STATION NAME:\t");
                                            string newName = Console.ReadLine();
                                            Console.Write("BASE STATION SLOTS COUNT:\t");
                                            if (!int.TryParse(Console.ReadLine(), out int slotsCount)) throw new InputException("Invalid number");
                                            myBl.UpdateBaseStation(baseStationId, newName, slotsCount);
                                            Console.WriteLine($"---------------------------------\n" +
                                                $"Base Station {baseStationId} was updated successfully\n" +
                                                $"---------------------------------", Console.ForegroundColor = ConsoleColor.Green);
                                            Console.ResetColor();
                                            break;
                                        }
                                    case 3:
                                        {
                                            Console.WriteLine("ENTER CUSTOMER ID FOR UPDATING:");
                                            Console.WriteLine("---------------------------------");
                                            Console.Write("CUSTOMER ID:\t");
                                            if (!int.TryParse(Console.ReadLine(), out int customerId)) throw new InputException("Invalid Customer Id");
                                            Console.Write("NEW CUSTOMER NAME:\t");
                                            string newName = Console.ReadLine();
                                            Console.Write("NEW NUMBER PHONE:\t");
                                            string newPhone = Console.ReadLine();
                                            myBl.UpdateCustomer(customerId, newName, newPhone);
                                            Console.WriteLine($"---------------------------------\n" +
                                                                $"Customer {customerId} was updated successfully\n" +
                                                                $"---------------------------------", Console.ForegroundColor = ConsoleColor.Green);
                                            Console.ResetColor();
                                            break;
                                        }
                                    case 4:
                                        {
                                            Console.WriteLine("ENTER DRONE ID FOR CHARGING:\t");
                                            Console.WriteLine("---------------------------------");
                                            Console.Write("DRONE ID:\t");
                                            if (!int.TryParse(Console.ReadLine(), out int droneId)) throw new InputException("Invalid Drone Id");
                                            myBl.ChargeDrone(droneId);
                                            Console.WriteLine($"---------------------------------\n" +
                                                              $"Drone {droneId} was charged successfully\n" +
                                                              $"---------------------------------", Console.ForegroundColor = ConsoleColor.Green);
                                            Console.ResetColor();
                                            break;
                                        }
                                    case 5:
                                        {
                                            Console.WriteLine("ENTER DRONE ID FOR RELEASING:\t");
                                            Console.WriteLine("---------------------------------");
                                            Console.Write("DRONE ID:\t");
                                            if (!int.TryParse(Console.ReadLine(), out int droneId)) throw new InputException("Invalid Drone Id");
                                            Console.Write("TIME IN CHARGE:\t");
                                            TimeSpan.TryParse(Console.ReadLine(), out TimeSpan tSpanInCharge);
                                            myBl.releaseDrone(droneId, tSpanInCharge);
                                            Console.WriteLine($"---------------------------------\n" +
                                                              $"Drone {droneId} was released successfully\n" +
                                                              $"---------------------------------", Console.ForegroundColor = ConsoleColor.Green);
                                            Console.ResetColor(); break;
                                        }
                                    case 6:
                                        {
                                            Console.WriteLine("ENTER DRONE ID FOR SCHEDULING:");
                                            Console.WriteLine("---------------------------------");
                                            Console.Write("DRONE ID:\t");
                                            if (!int.TryParse(Console.ReadLine(), out int droneId)) throw new InputException("Invalid Drone Id");
                                            myBl.ScheduleDroneForParcel(droneId);
                                            Console.WriteLine($"---------------------------------\n" +
                  $"Drone {droneId} was scheduled successfully\n" +
                  $"---------------------------------", Console.ForegroundColor = ConsoleColor.Green);
                                            Console.ResetColor();
                                            break;
                                        }

                                    case 7:
                                        {
                                            Console.Write("ENTER DRONE ID FOR PICKING UP:\t");
                                            if (!int.TryParse(Console.ReadLine(), out int droneId)) throw new InputException("Invalid Drone Id");
                                            myBl.PickingUpAParcel(droneId);
                                            Console.WriteLine($"---------------------------------\n" +
                  $"Drone {droneId} picked up a parcel successfully\n" +
                  $"---------------------------------", Console.ForegroundColor = ConsoleColor.Green);
                                            Console.ResetColor();
                                            break;
                                        }
                                    case 8:
                                        {
                                            Console.Write("ENTER DRONE ID FOR DELIVERY:\t");
                                            if (!int.TryParse(Console.ReadLine(), out int droneId)) throw new InputException("Invalid Drone Id");
                                            myBl.DeliverAParcel(droneId);
                                            Console.WriteLine($"---------------------------------\n" +
                                                $"Drone {droneId} delivered a parcel successfully\n" +
                                                $"---------------------------------", Console.ForegroundColor = ConsoleColor.Green);
                                            Console.ResetColor();
                                            break;
                                        }
                                    default:
                                        throw new InputException("WRONG CHOICE");
                                }
                                break;
                            }
                        #endregion
                        #region VIEW ITEM
                        case 3:
                            {
                                Console.WriteLine(@"---------------------------------
FOR VIEWING A BASE STATION PRESS 1
FOR VIEWING A DRONE PRESS 2
FOR VIEWING A CUSTOMER PRESS 3
FOR VIEWING A PARCEL PRESS 4
---------------------------------");
                                if (!int.TryParse(Console.ReadLine(), out choice2)) throw new InputException("WRONG INPUT");
                                Console.WriteLine("---------------------------------");
                                switch (choice2)
                                {
                                    case 1:
                                        {
                                            Console.Write("ENTER BASE-STATION ID FOR VIEWING:\t");
                                            if (!int.TryParse(Console.ReadLine(), out int baseStationId)) throw new InputException("Invalid Base-Station Id");
                                            Console.WriteLine("---------------------------------");
                                            BaseStation myBase = myBl.FindBaseStation(baseStationId);
                                            Console.Write(myBase.ToString(), Console.ForegroundColor = ConsoleColor.Green);
                                            Console.ResetColor(); break;
                                        }
                                    case 2:
                                        {
                                            Console.Write("ENTER DRONE ID FOR VIEWING:\t");
                                            if (!int.TryParse(Console.ReadLine(), out int droneId)) throw new InputException("Invalid Drone Id");
                                            Console.WriteLine("---------------------------------");
                                            Drone myDrone = myBl.FindDrone(droneId);
                                            Console.WriteLine(myDrone.ToString(), Console.ForegroundColor = ConsoleColor.Green);
                                            Console.ResetColor(); break;
                                        }
                                    case 3:
                                        {
                                            Console.Write("ENTER CUSTOMER ID FOR VIEWING:\t");
                                            if (!int.TryParse(Console.ReadLine(), out int customerId)) throw new InputException("Invalid customer Id");
                                            Console.WriteLine("---------------------------------");
                                            Customer myCustomer = myBl.FindCustomer(customerId);
                                            Console.WriteLine(myCustomer.ToString(), Console.ForegroundColor = ConsoleColor.Green);
                                            Console.ResetColor();
                                            break;
                                        }
                                    case 4:
                                        {
                                            Console.Write("ENTER PARCEL ID FOR VIEWING:\t");
                                            if (!int.TryParse(Console.ReadLine(), out int parcelId)) throw new InputException("Invalid parcel Id");
                                            Console.WriteLine("---------------------------------");
                                            Parcel myParcel = myBl.FindParcel(parcelId);
                                            Console.WriteLine(myParcel.ToString(), Console.ForegroundColor = ConsoleColor.Green);
                                            Console.ResetColor();
                                            break;
                                        }
                                    default:
                                        throw new InputException("WRONG CHOICE");
                                        break;
                                }
                                break;
                            }
                        #endregion
                        #region VIEW LISTS
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
                                        throw new InputException("WRONG CHOICE");
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
                            throw new InputException("WRONG CHOICE");
                    }
                }
                catch (Exception ex)
                {
                    msg = "-----------------------------------\n";
                    msg += $"{ex.Message}\n";
                    while (ex.InnerException != null)
                    {
                        ex = ex.InnerException;
                        msg += $"{ex.Message}\n";
                    }
                    msg += "-----------------------------------";
                    Console.WriteLine(msg, Console.ForegroundColor = ConsoleColor.Red);
                    Console.ResetColor();
                }
            }
            while (choice1 != 5);

        }
    }
}
