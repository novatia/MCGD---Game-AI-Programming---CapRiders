namespace WiFiInput.Client
{
    public abstract class WiFiClientController
    {
        public virtual void OnUpdate()
        {
            mapInputToDataStream();
        }

        protected abstract void mapInputToDataStream();
    }
}
