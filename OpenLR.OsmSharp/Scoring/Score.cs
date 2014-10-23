namespace OpenLR.OsmSharp.Scoring
{
    /// <summary>
    /// Represents a score for a specific purpose.
    /// </summary>
    public abstract class Score
    {
        /// <summary>
        /// Gets the name of this score.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the description of this score.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// Gets or the score of this score.
        /// </summary>
        public abstract double Value { get; }

        /// <summary>
        /// Gets or sets the reference value of this score.
        /// </summary>
        /// <remarks>If value is max 1, this contains 1.</remarks>
        public abstract double Reference { get; }

        /// <summary>
        /// Creates a new simple score.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="value"></param>
        /// <param name="reference"></param>
        /// <returns></returns>
        public static SimpleScore New(string name, string description, double value, double reference)
        {
            return new SimpleScore(name, description, value, reference);
        }

        /// <summary>
        /// Multiplies the two given scores.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static MultipliedScore operator *(Score left, Score right)
        {
            return new MultipliedScore(left, right);
        }

        /// <summary>
        /// Adds the two given scores.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static AddedScore operator +(Score left, Score right)
        {
            return new AddedScore(left, right);
        }

        /// <summary>
        /// Returns a description of this score.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} {1}/{2}", this.Name, this.Value, this.Reference);
        }
        
        /// <summary>
        /// Returns the hashcode.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.Name.GetHashCode() ^
                this.Description.GetHashCode() ^
                this.Value.GetHashCode() ^
                this.Reference.GetHashCode();
        }

        /// <summary>
        /// Returns true if the given object equals this object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var otherScore = obj as Score;
            if(otherScore != null)
            {
                return otherScore.Name.Equals(this.Name) &&
                    otherScore.Description.Equals(this.Description) &&
                    otherScore.Value.Equals(this.Value) &&
                    otherScore.Reference.Equals(this.Reference);
            }
            return false;
        }
    }
}