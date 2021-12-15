namespace DO
{
    public struct DroneCharge
    {
        public int DroneId { get; set; }
        public int StationId { get; set; }
        public override string ToString()
        {
            string result = "";
            result += $"Drone Id: {DroneId} \n ";
            result += $"Station Id: {StationId} \n ";
            return result;
        }
    }
}

