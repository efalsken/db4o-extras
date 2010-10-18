namespace Db4objects.Db4o.AutoIncrement.Test
{
    public class WithAutoIDs
    {
        [AutoIncrement]
        private int generatedIds;
        private int othterInt;

        public int GeneratedIds
        {
            get { return generatedIds; }
        }

        public int OthterInt
        {
            get { return othterInt; }
        }
    }
    public class WithAutoIdsOnProperty
    {

        [AutoIncrement]
        public int GeneratedIds
        {
            get;
            private set;
        }
    }

    public class InheritedId : WithAutoIDs
    {
    }

    public class WithoutAutoId
    {
        private int othterInt;

        public int OthterInt
        {
            get { return othterInt; }
            set { othterInt = value; }
        }
    }
}