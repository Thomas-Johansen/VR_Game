using UnityEngine;

namespace DefaultNamespace
{
    public static class Planets
    {
        public static Vector3 EarthCenter = new Vector3(0, 0, 0);
        public static bool IsActiveEarth = true;
        
        public static Vector3 MoonCenter = new Vector3(500, 800, 0);
        public static bool IsActiveMoon = true;
        
        public static Vector3 SunCenter = new Vector3(0, 10000, -9000);
        public static bool IsActiveSun = true;
        
        public static Vector3 KaiCenter = new Vector3(-7000, 9000, -1500);
        public static bool IsActiveKai = true;
        
        
        
        
        public static void makeUnactive(string planet)
        {
            switch (planet)
            {
                case "Earth":
                    IsActiveEarth = false;
                    break;
                case "Moon":
                    IsActiveMoon = false;
                    break;
                case "King Kai":
                    IsActiveKai = false;
                    break;
                default:
                {
                    break;
                }
            }
            
        }
    }
}