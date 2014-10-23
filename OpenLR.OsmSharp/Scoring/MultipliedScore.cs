namespace OpenLR.OsmSharp.Scoring
{
    /// <summary>
    /// Represents a mulitplied score, a score that is the multiplication of two others.
    /// </summary>
    public class MultipliedScore : Score
    {
        /// <summary>
        /// Holds the left score.
        /// </summary>
        private Score _left;

        /// <summary>
        /// Holds the right score.
        /// </summary>
        private Score _right;

        /// <summary>
        /// Creates a new multiplied score.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        internal MultipliedScore(Score left, Score right)
        {
            _left = left;
            _right = right;
        }

        /// <summary>
        /// Returns the left-side.
        /// </summary>
        public Score Left
        {
            get
            {
                return _left;
            }
        }

        /// <summary>
        /// Returns the right side.
        /// </summary>
        public Score Right
        {
            get
            {
                return _right;
            }
        }

        /// <summary>
        /// Returns the name of this score.
        /// </summary>
        public override string Name
        {
            get { return "[" + this.Left.Name + "] * [" + this.Right.Name + "]"; }
        }

        /// <summary>
        /// Returns the description of this score.
        /// </summary>
        public override string Description
        {
            get { return "[" + this.Left.Description + "] * [" + this.Right.Description + "]"; }
        }

        /// <summary>
        /// Returns the value of this score.
        /// </summary>
        public override double Value
        {
            get { return this.Left.Value * this.Right.Value; }
        }

        /// <summary>
        /// Returns the reference value of this score.
        /// </summary>
        public override double Reference
        {
            get { return this.Left.Reference * this.Right.Reference; }
        }
    }
}