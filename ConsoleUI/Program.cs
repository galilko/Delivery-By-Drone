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
            dal.PrintBaseStations();
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
                                Console.Write("Please enter the base station's name:\t");
                                string name = Console.ReadLine();
                                Console.Write("Please enter the base station's latitude:\t");
                                double latitude = Convert.ToSingle(Console.ReadLine());
                                Console.Write("Please enter the base station's longitude:\t");
                                double longitude = Convert.ToSingle(Console.ReadLine());
                                Console.Write("Please enter number of free charger slots:\t");
                                int freeSlots = Int32.Parse(Console.ReadLine());
                                dal.AddBaseStation(name, latitude, longitude, freeSlots);
                                break;
                            case 2:
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
                                        "ERROR - invalid weight category");
                                    break;
                                }
                                Console.Write("Battery:\t");
                                double battery = Convert.ToSingle(Console.ReadLine());
                                dal.AddDrone(id, model, weight, status, battery);
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
            while (choice1 != 5);
            dal.PrintBaseStations();
            dal.GetDronesArr();
            
            Console.WriteLine("cscc");
        }
    }
}
