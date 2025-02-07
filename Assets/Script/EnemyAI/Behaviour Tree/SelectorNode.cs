using System.Collections.Generic;

namespace Node
{
    public class SelectorNode : BTNode
    {
        private readonly List<BTNode> nodes;

        public SelectorNode(List<BTNode> nodes)
        {
            this.nodes = nodes;
        }

        public override bool Execute()
        {
            foreach (var node in nodes)
            {
                if (node.Execute())
                    return true;  // Jika salah satu aksi berhasil, SelectorNode berhenti
            }
            return false;
        }
    }
}
