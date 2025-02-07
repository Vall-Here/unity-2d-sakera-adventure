namespace Node
{
    public class ActionNode : BTNode
    {
        private readonly System.Action action;

        public ActionNode(System.Action action)
        {
            this.action = action;
        }

        public override bool Execute()
        {
            action();
            return true;  // Action berhasil dijalankan
        }
    }
}
