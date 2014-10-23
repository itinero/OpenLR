namespace OpenLR.OsmSharp.Scoring
{
    /// <summary>
    /// Represents a score with a single value and a single name.
    /// </summary>
    public class SimpleScore : Score
    {
        /// <summary>
        /// Holds the name.
        /// </summary>
        private string _name;

        /// <summary>
        /// Holds the description.
        /// </summary>
        private string _description;

        /// <summary>
        /// Holds the value.
        /// </summary>
        private double _value;

        /// <summary>
        /// Holds the reference.
        /// </summary>
        private double _reference;

        /// <summary>
        /// Creates a new simple score.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="value"></param>
        /// <param name="reference"></param>
        internal SimpleScore(string name, string description, double value, double reference)
        {
            _name = name;
            _description = description;
            _value = value;
            _reference = reference;
        }

        /// <summary>
        /// Gets the name of this score.
        /// </summary>
        public override string Name { get { return _name; } }

        /// <summary>
        /// Gets the description of this score.
        /// </summary>
        public override string Description { get { return _description; } }

        /// <summary>
        /// Gets the value of this score.
        /// </summary>
        public override double Value { get { return _value; } }

        /// <summary>
        /// Gets the references of this score.
        /// </summary>
        public override double Reference { get { return _reference; } }

        /// <summary>
        /// Gets a score by name.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override Score GetByName(string key)
        {
            if(this.Name == key)
            {
                return this;
            }
            return null;
        }
    }
}