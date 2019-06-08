namespace TrueSync
{
    public static class TSVector2Extension
    {
        public static TSVector2 Rotate(this TSVector2 v, FP degrees)
        {
            FP sin = FP.Sin(degrees * FP.Deg2Rad);
            FP cos = FP.Cos(degrees * FP.Deg2Rad);

            FP tx = v.x;
            FP ty = v.y;
            v.x = (cos * tx) - (sin * ty);
            v.y = (sin * tx) + (cos * ty);
            return v;
        }
    }
}