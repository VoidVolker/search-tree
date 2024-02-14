#pragma warning disable CS8600

    /// <summary>
    /// Data search Tree via array: key -> index. Default array size is 0xFF.
    /// Use for >1-10 millions of records. More records - more effectife memory usage. 
    /// Less records - less effective memory usage.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Calculate one level memory consumption:
    ///     (1) node_size ^ level_index-1<br/>
    /// 
    /// Calculate total memory consumption: 
    ///     (2) total_nodes * (node_size * cell_size + internal_fields_size)<br/>
    ///     internal_fields_size for x86 and x64 is: 32 and 56 bytes<br/>
    ///     
    /// Calculate total items per level:    
    ///     (3) node_size ^ level (from 1)<br/>
    /// 
    /// Example for 4 levels:<br/> 
    ///     Level 1: 1 ^ 0 = 1 node                 256 ^ 1 items<br/>
    ///     Level 2: 256 ^ 1 = 256 nodes            256 ^ 2 items (65 536)<br/>
    ///     Level 3: 256 ^ 2 = 65 536 nodes         256 ^ 3 items (16 777 216)<br/>
    ///     Level 4: 256 ^ 3 = 16 777 216 nodes     256 ^ 4 items (4 294 967 295)<br/>
    ///     Total nodes: 16 843 009<br/>
    ///     Total for x86: 16 843 009 * 1056 = 17 786 217 504 bytes (~17 Gb)<br/>
    ///     Total for x64: 16 843 009 * 2104 = 35 437 690 936 bytes (~33 Gb)
    /// </para><para>
    /// Node total size x86: 1056<br/>
    /// Levels 1-4 nodes: 1 /256 / 65536/ 16777216<br/>
    /// Levels 1-4 sizes: 1056 / 270336 / 69206016 / 17716740096<br/>
    /// Tree 1-4 nodes: 1 / 257 / 65793 / 16843009<br/>
    /// Tree 1-4 sizes: 1056 / 271392 / 69477408 / 17786217504
    /// </para><para>
    /// Node total size x64: 2104<br/>
    /// Level 1-4 nodes: 1 / 256 / 65536 / 16777216<br/>
    /// Level 1-4 sizes: 2104 / 538624 / 137887744 / 35299262464<br/>
    /// Tree 1-4 nodes: 1 / 257 / 65793 / 16843009<br/>
    /// Tree 1-4 sizes: 2104 / 540728 / 138428472 /  35437690936
    /// </para>
    /// </remarks>
    /// <typeparam name="TValue">Node value type</typeparam>    
    public class ArrayTree<TValue>
    {
        /// <summary>
        /// Default node size
        /// </summary>
#pragma warning disable CA2211
        public static uint DefaultNodeSize = 256;
#pragma warning restore CA2211
        /// <summary>
        /// Node size
        /// </summary>
        public readonly uint NodeSize = 256;
        /// <summary>
        /// Root node
        /// </summary>
        public TreeNode Root;

        /// <summary>
        /// Create new ArrayTree instance with default node size
        /// </summary>
        public ArrayTree() : this(DefaultNodeSize) { }

        /// <summary>
        /// Create new ArrayTree instance with specified node size
        /// </summary>
        public ArrayTree(uint nodeSize)
        {
            NodeSize = nodeSize;
            Root = new TreeNode(NodeSize);
        }

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
            public readonly TreeNode[] Nodes;
            /// <summary>
            /// Nodes counter - required for nodes deleting
            /// </summary>
            public byte Count = 0;

            /// <summary>
            /// Create new node instance
            /// </summary>
            /// <param name="size"></param>
            public TreeNode(uint size)
            {
                Nodes = new TreeNode[size];
            }
        }

        /// <summary>
        /// Add value to the tree or overwrite existing value
        /// </summary>
        /// <param name="path">Value path</param>
        /// <param name="value">Value</param>
        /// <returns>Node</returns>
        public TreeNode Add(byte[] path, TValue value)
        {
            TreeNode node = Root
                , nextNode
            ;

            foreach (var key in path)
            {
                // Get next node
                nextNode = node.Nodes[key];
                // Node doesn't exists
                if (nextNode == null)
                {
                    // Create new node
                    nextNode = new TreeNode(NodeSize);
                    // Incrase counter
                    node.Count++;
                    // Add new node to current node
                    node.Nodes[key] = nextNode;
                }
                // Select next node
                node = nextNode;
            }
            // Set node value
            node.Value = value;
            // Return node
            return node;
        }

        /// <summary>
        /// Try find value specified by path in the tree
        /// </summary>
        /// <param name="path">Value path</param>
        /// <param name="value">Value</param>
        /// <returns>Is value founded</returns>
        public bool TryGetValue(byte[] path, out TValue value)
        {
            TreeNode node = Root
                , nextNode
            ;

            foreach (var key in path)
            {
                nextNode = node.Nodes[key];
                if (nextNode == null)
                {
                    value = default;
                    return false;
                }
                node = nextNode;
            }
            value = node.Value;
            return true;
        }

        /// <summary>
        /// Try find node in the tree specified by path 
        /// </summary>
        /// <param name="path">Node path</param>
        /// <param name="foundedNode">Node</param>
        /// <returns>Is node founded</returns>
        public bool TryGetNode(byte[] path, out TreeNode foundedNode)
        {
            TreeNode node = Root
                , nextNode
            ;

            foreach (var key in path)
            {
                nextNode = node.Nodes[key];
                if (nextNode == null)
                {
                    foundedNode = null;
                    return false;
                }
                node = nextNode;
            }
            foundedNode = node;
            return true;
        }

        /// <summary>
        /// Remove value specified by path with all empty nodes in the path
        /// </summary>
        /// <param name="path">Value path</param>
        /// <returns>Removed value</returns>
        public TValue Remove(byte[] path)
        {
            int len = path.Length
                , index = 0;

            if (len == 0) return default;

            // Nodes path
            TreeNode[] nodes = new TreeNode[len];
            TreeNode node = Root
                , nextNode
            ;

            // Value search
            foreach (var key in path)
            {
                nodes[index++] = node;
                nextNode = node.Nodes[key];
                if (nextNode == null)
                {
                    return default;
                }
                else
                {
                    node = nextNode;
                }

            }

            // Empty nodes removing
            for (int i = len - 1; i >= 0; i--)
            {
                nextNode = nodes[i];
                nextNode.Nodes[path[i]] = null;
                nextNode.Count--;
                if (nextNode.Count > 0 | (!EqualityComparer<TValue>.Default.Equals(nextNode.Value, default)))
                {
                    break;
                }
            }

            return node.Value;
        }

#if DEBUG
        /// <summary>
        /// Calculate node size using GC. Run only in one thread without additional parallel tasks. Use for debug only.
        /// </summary>
        /// <param name="nodeSize"></param>
        /// <returns>Bytes</returns>
        public static long GetTotalNodeSize(uint nodeSize)
        {
            var GCStart = GC.GetTotalMemory(true);
#pragma warning disable IDE0059
            var node = new TreeNode(nodeSize);
#pragma warning restore IDE0059
            var GCEnd = GC.GetTotalMemory(true);
            return GCEnd - GCStart;
        }

        /// <summary>
        /// Calculate maximum available nodes count for one level
        /// </summary>
        /// <param name="levelIndex"></param>
        /// <param name="nodeSize"></param>
        /// <returns>Nodes</returns>
        public static long GetLevelNodesMaxCount(uint levelIndex, uint nodeSize)
        {
            return (long)Math.Pow(nodeSize, levelIndex - 1);
        }

        /// <summary>
        /// Calculate maximum level size in bytes
        /// </summary>
        /// <param name="levelIndex"></param>
        /// <param name="nodeSize"></param>
        /// <returns>Bytes</returns>
        public static long GetLevelMaxSize(uint levelIndex, uint nodeSize)
        {
            var nodeTotalSize = GetTotalNodeSize(nodeSize);
            var nodesCount = GetLevelNodesMaxCount(levelIndex, nodeSize);
            return nodesCount * nodeTotalSize;
        }

        /// <summary>
        /// Calculate maximum tree size in bytes
        /// </summary>
        /// <param name="levelsCount"></param>
        /// <param name="nodeSize"></param>
        /// <returns>Bytes</returns>
        public static long GetTreeMaxSize(uint levelsCount, uint nodeSize)
        {
            long cnt = 0;
            for (uint i = 1; i <= levelsCount; i++)
            {
                cnt += GetLevelMaxSize(i, nodeSize);
            }
            return cnt;
        }

        /// <summary>
        /// Calculate maximum tree nodes count
        /// </summary>
        /// <param name="levelsCount"></param>
        /// <param name="nodeSize"></param>
        /// <returns>Nodes</returns>
        public static long GetTreeNodesMaxCount(uint levelsCount, uint nodeSize)
        {
            long cnt = 0;
            for (uint i = 1; i <= levelsCount; i++)
            {
                cnt += GetLevelNodesMaxCount(i, nodeSize);
            }
            return cnt;
        }
#endif

#pragma warning restore CS8600
