#pragma warning disable CS8600
    /// <summary>
    /// Data search Tree via linked list (dictionary)
    /// </summary>
    /// <typeparam name="TKey">Key type</typeparam>
    /// <typeparam name="TValue">Node value type</typeparam>
    public unsafe class ListTree<TKey, TValue> where TKey : notnull
    {
        /// <summary>
        /// Root node
        /// </summary>
        public TreeNode Root = new();

        /// <summary>
        /// Tree node
        /// </summary>
        public class TreeNode
        {
            /// <summary>
            /// Node value
            /// </summary>
            public TValue Value;
            /// <summary>
            /// Node childs
            /// </summary>
            public readonly SortedDictionary<TKey, TreeNode> Nodes = [];
        }

        /// <summary>
        /// Add value to the tree or overwrite existing value
        /// </summary>
        /// <param name="path">Value path</param>
        /// <param name="value">Value</param>
        /// <returns>Node</returns>
        public TreeNode Add(IEnumerable<TKey> path, TValue value)
        {
            var node = Root;

            foreach (var key in path)
            {
                if (!node.Nodes.TryGetValue(key, out TreeNode nextNode))
                {
                    nextNode = new TreeNode();
                    node.Nodes[key] = nextNode;
                }
                node = nextNode;
            }

            node.Value = value;
            return node;
        }

        /// <summary>
        /// Try find value specified by path in the tree
        /// </summary>
        /// <param name="path">Value path</param>
        /// <param name="value">Value</param>
        /// <returns>Is value founded</returns>
        public bool TryGetValue(IEnumerable<TKey> path, out TValue value)
        {
            var node = Root;

            foreach (var level in path)
            {
                if (node.Nodes.TryGetValue(level, out TreeNode nextNode))
                {
                    node = nextNode;
                }
                else
                {
                    value = default;
                    return false;
                }
            }
            value = node.Value;
            return true;
        }


        /// <summary>
        /// Try find node specified by path in the tree
        /// </summary>
        /// <param name="path">Value path</param>
        /// <param name="foundedNode">Node</param>
        /// <returns>Is node founded</returns>
        public bool TryGetNode(IEnumerable<TKey> path, out TreeNode foundedNode)
        {
            var node = Root;

            foreach (var level in path)
            {
                if (node.Nodes.TryGetValue(level, out TreeNode nextNode))
                {
                    node = nextNode;
                }
                else
                {
                    foundedNode = null;
                    return false;
                }
            }
            foundedNode = node;
            return true;
        }

        /// <summary>
        /// Remove value specified by path with all empty nodes in the path
        /// </summary>
        /// <param name="path">Value path</param>
        /// <returns>Removed value</returns>
        public TValue Remove(IEnumerable<TKey> path)
        {
            int len = path.Count()
                , index = 0;

            if (len == 0) return default;

            TreeNode[] nodes = new TreeNode[len];
            TKey[] keys = new TKey[len];
            TreeNode node = Root
                , n
            ;

            // Node search
            foreach (var key in path)
            {
                keys[index] = key;
                nodes[index++] = node;
                if (node.Nodes.TryGetValue(key, out TreeNode nextNode))
                {
                    node = nextNode;
                }
                else
                {
                    return default;
                }

            }

            // Nodes removing
            for (int i = len - 1; i >= 0; i--)
            {
                n = nodes[i];
                n.Nodes.Remove(keys[i]);

                if (n.Nodes.Any() | (!EqualityComparer<TValue>.Default.Equals(n.Value, default)))
                {
                    break;
                }
            }

            return node.Value;
        }

    }
#pragma warning restore CS8600
