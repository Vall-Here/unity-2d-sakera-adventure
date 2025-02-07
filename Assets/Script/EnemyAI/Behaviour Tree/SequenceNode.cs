using UnityEngine;
using System.Collections.Generic;

namespace Node
{
    public class SequenceNode : BTNode
    {
        private readonly List<BTNode> nodes;

        public SequenceNode(List<BTNode> nodes)
        {
            this.nodes = nodes;
        }

        public override bool Execute()
        {
            foreach (var node in nodes)
            {
                if (!node.Execute())
                {
                    return false;  
                }
            }
            return true; 
        }
    }
}
