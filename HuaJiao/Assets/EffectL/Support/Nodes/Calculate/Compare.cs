using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EffectL.Support.NodeSupport
{
    [Node(false,"比较","Calculate", NodeType.Calc)]
    public class Compare:Node
    {
        [EffectLTransitConvertAsset("RConvert")]
        [EffectLTransitAsset]
        public EffectLNumeric numA;
        [EffectLTransitConvertAsset("RConvert")]
        [EffectLTransitAsset]
        public EffectLNumeric numB;
        public enum CompareNumricType
        {
            大于,
            小于,
            等于,
        }

        [EffectLDisplayHandleAssets] public CompareNumricType compareNumricType;
        bool Evaluate()
        {
            if (compareNumricType == CompareNumricType.大于)
            {
                return (System.Convert.ToDouble(numA.Val()) > System.Convert.ToDouble(numB.Val()));
            }
            else if (compareNumricType == CompareNumricType.小于)
            {
                return (System.Convert.ToDouble(numA.Val()) < System.Convert.ToDouble(numB.Val()));
            }
            else
            {
                return (System.Convert.ToDouble(numA.Val()) == System.Convert.ToDouble(numB.Val()));
            }
        }

        object RConvert(object old)
        {
            EffectLNumeric n=new EffectLNumeric();
            if (old.GetType() == typeof(int))
            {
                n.type = EffectLNumeric.NumericType.真值;
            }
            else if (old.GetType() == typeof(float))
            {
                n.type = EffectLNumeric.NumericType.实值;
            }
            else
            {
                n.type = EffectLNumeric.NumericType.Double;
            }
            n.SetVal(old);
            return n;
        }
    }
}
