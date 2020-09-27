namespace LogCollectorLibrary
{
    public class MessageShowMethod
    {
        public delegate void Show(string message);
        private static Show showMethod;
        public static Show ShowMethod
        {
            get => showMethod ?? ((message) => { });
            set
            {
                showMethod = value;
            }
        }
    }
}
