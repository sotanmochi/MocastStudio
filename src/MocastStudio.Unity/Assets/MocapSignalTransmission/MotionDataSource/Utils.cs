namespace MocapSignalTransmission.MotionDataSource
{
    public static class Utils
    {
        /// <summary>
        /// Indicates whether the value is a power of two.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsPowerOfTwo(int value)
        {
            // If the value is a power of two, the result of bitwise AND operation with (value - 1) is zero.
            return (value > 1) && (value & (value - 1)) is 0;
        }
    }
}
