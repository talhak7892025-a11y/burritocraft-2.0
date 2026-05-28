using UnityEngine;

namespace Burritocraft.Voxel
{
    [System.Serializable]
    public class BlockType
    {
        public enum BlockId : byte
        {
            Air = 0,
            Stone = 1,
            Dirt = 2,
            Grass = 3,
            Sand = 4,
            Water = 5,
            Lava = 6,
            Wood = 7,
            Leaves = 8,
            Bedrock = 9,
            // Add more block types as needed
        }

        public BlockId id;
        public string name;
        public Color color;
        public bool isSolid = true;
        public bool isTransparent = false;
        public float hardness = 1f;
        public string textureName;

        public BlockType(BlockId blockId, string blockName, Color blockColor, bool solid = true)
        {
            id = blockId;
            name = blockName;
            color = blockColor;
            isSolid = solid;
        }
    }
}
