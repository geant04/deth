using System;

namespace Util
{
    public class Easing
    {
        public static float EaseOutExpo(float num)
        {
            return num >= 1 ? 1 : (float) (1 - Math.Pow(2, -10 * num));
        }
    }
}