using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DragonTD
{
    static class Util
    {
        static public float Distance(Vector2 v1, Vector2 v2)
        {
            return (v2 - v1).Length();
        }
    }

    public struct Point3D
    {
        public int X, Y, Z;
    }
}
