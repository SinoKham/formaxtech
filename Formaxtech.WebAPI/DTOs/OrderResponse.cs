namespace Formaxtech.WebAPI.DTOs
{
    public class OrderResponse
    {
        public int DriverId { get; set; }
        public int DriverX { get; set; }
        public int DriverY { get; set; }
        public double RouteLength { get; set; }
        public List<RoutePoint> Route { get; set; } = new();
    }
}