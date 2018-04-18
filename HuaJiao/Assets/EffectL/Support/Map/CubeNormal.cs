using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EffectL.Support.NodeSupport
{
    [Node(false,"普通块","Cube", NodeType.Cube,120,120)]
    public class CubeNormal:Cube
    {
        [EffectLDisplayHandleAssets] public EffectLCommand com;
    }
}
