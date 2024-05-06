namespace ApproachableANN.Senses
{
    /// <summary>
    /// ANN Sense that returns the value of a public float variable
    /// </summary>
    public sealed class FloatSense : ANNSense
    {
        /// <summary>
        /// Value for the ANN Unit to consider when making decisions
        /// </summary>
        public float senseValue;

        public override float GetSenseValue()
        {
            return senseValue;
        }
    }
}
