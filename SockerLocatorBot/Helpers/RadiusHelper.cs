namespace SockerLocatorBot.Helpers
{
    public static class RadiusHelper
    {
        const double EarthRadiusKm = 6371.0;

        public static double KmToDegrees(int kilometers) => (kilometers / EarthRadiusKm) * (180.0 / Math.PI);
    }
}
