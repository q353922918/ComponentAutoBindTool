namespace Third_Party.ComponentAutoBindTool.Example.Scripts
{
    public partial class AutoBindTest : AutoBindTestBase
    {
        private void Start()
        {
            GetBindComponents(gameObject);

            view.txtTest3.text = "Hello World!";
        }
    }
}
