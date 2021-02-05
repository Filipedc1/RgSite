namespace RgSite.Core.Models
{
    public class JsonPayload
    {
        public object Payload { get; set; }
        public bool Errored { get; set; }
        public string Message { get; set; }

        private string _exception;
        public string Exception
        {
            get => _exception;
            set
            {
                Errored = true;
                _exception = value;
            }
        }

        public JsonPayload()
        {

        }
    }
}
