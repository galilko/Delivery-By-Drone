using System;
using IDAL.DO;
using DalObject;

namespace ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            int choice1 = 0, choice2 = 1;
            DalObject.DalObject dal = new DalObject.DalObject();
            //   dal.GetDronesArr();
            // dal.GetCustomersArr();
            // dal.PrintBaseStations();
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
---------------------------------"
                    );
                choice1 = Convert.ToInt32(Console.ReadLine());
                switch (choice1)
                {
                    case 1:
                        {
                            Console.WriteLine(@"---------------------------------
FOR ADDING A BASE STATION PRESS 1
FOR ADDING A DRONE PRESS 2
FOR ADDING A CUSTOMER PRESS 3
FOR ADDING A PARCEL PRESS 4
---------------------------------"
         );
                            choice2 = Convert.ToInt32(Console.ReadLine());
                            Console.WriteLine("---------------------------------");
                            switch (choice2)
                            {
                                case 1:
                                    {
                                        Console.WriteLine("ENTER BASE-STATION DETAILS\n" +
                                            "---------------------------------");
                                        Console.Write("Name:\t");
                                        string name = Console.ReadLine();
                                        Console.Write("Latitude:\t");
                                        double latitude = Convert.ToSingle(Console.ReadLine());
                                        Console.Write("Longitude:\t");
                                        double longitude = Convert.ToSingle(Console.ReadLine());
                                        Console.Write("Number of free charger slots:\t");
                                        int freeSlots = Int32.Parse(Console.ReadLine());
                                        dal.AddBaseStation(name, latitude, longitude, freeSlots);
                                        break;
                                    }
                                case 2:
                                    {
                                        Console.WriteLine("ENTER DRONE DETAILS\n" +
                                            "---------------------------------");
                                        Console.Write("Id:\t");
                                        int id = Int32.Parse(Console.ReadLine());
                                        Console.Write("Model:\t");
                                        string model = Console.ReadLine();
                                        Console.Write("Weight category:\t");
                                        if (!Enum.TryParse(Console.ReadLine(), out WeightCategories weight))
                                        {
                                            Console.WriteLine("---------------------------------\n" +
                                                "ERROR - invalid weight category");
                                            break;
                                        }
                                        Console.Write("Status:\t");
                                        if (!Enum.TryParse(Console.ReadLine(), out DroneStatusCategories status))
                                        {
                                            Console.WriteLine("---------------------------------\n" +
                                                "ERROR - invalid status category");
                                            break;
                                        }
                                        Console.Write("Battery:\t");
                                        double battery = Convert.ToSingle(Console.ReadLine());
                                        dal.AddDrone(id, model, weight, status, battery);
                                        break;
                                    }
                                case 3:
                                    {
                                        Console.WriteLine("ENTER CUSTOMER DETAILS\n" +
                                           "---------------------------------");
                                        Console.Write("Id:\t");
                                        int id = Int32.Parse(Console.ReadLine());
                                        Console.Write("Name:\t");
                                        string name = Console.ReadLine();
                                        Console.Write("Phone:\t");
                                        string phone = Console.ReadLine();
                                        Console.Write("Latitude:\t");
                                        double latitude = Convert.ToSingle(Console.ReadLine());
                                        Console.Write("Longitude:\t");
                                        double longitude = Convert.ToSingle(Console.ReadLine());
                                        dal.AddCustomer(id, name, phone, latitude, longitude);
                                        break;
                                    }
                                case 4:
                                    {
                                        Console.WriteLine("ENTER PARCEL DETAILS\n" +
                                          "---------------------------------");
                                        Console.Write("SENDER-ID:\t");
                                        Int32.TryParse(Console.ReadLine(), out int senderId);
                                        Console.Write("TARGET-ID:\t");
                                        Int32.TryParse(Console.ReadLine(), out int targetId);
                                        Console.Write("WEIGHT:\t");
                                        if (!Enum.TryParse(Console.ReadLine(), out WeightCategories weight))
                                        {
                                            Console.WriteLine("---------------------------------\n" +
                                                "ERROR - invalid weight category");
                                            break;
                                        }
                                        Console.Write("PRIORITY:\t");
                                        if (!Enum.TryParse(Console.ReadLine(), out Priorities priority))
                                        {
                                            Console.WriteLine("---------------------------------\n" +
                                                "ERROR - invalid priority category");
                                            break;
                                        }
                                        dal.AddParcel(senderId, targetId, weight, priority);
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
---------------------------------"
    );
                            choice2 = Convert.ToInt32(Console.ReadLine());
                            Console.WriteLine("---------------------------------");
                            switch (choice2)
                            {
                                case 1:
                                    {
                                        dal.PrintAllBaseStations();
                                        break;
                                    }
                                case 2:
                                    {
                                        dal.PrintAllDrones();
                                        break;
                                    }
                                case 3:
                                    {
                                        dal.PrintAllCustomers();
                                        break;
                                    }
                                case 4:
                                    {
                                        dal.PrintAllParcels();
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
