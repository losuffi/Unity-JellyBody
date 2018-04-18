using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace EffectL.Support.NodeSupport
{
    [Serializable]
    public class EffectLCommand
    {
        [Multiline]
        public string Content;

        public EffectLCommand(string content)
        {
            Content = content;
        }
    }

    [Serializable]
    public class EffectLGameObject
    {
        public string Path=string.Empty;

        private GameObject _value;
        public GameObject value
        {
            get
            {
                if(_value==null)
                    _value=GameObject.Find(Path);
                return _value;
            }
            set
            {
                _value = value;
                Path = EffectUtility.GetGameObjectPath(_value);
            }
        }
    }

    [Serializable]
    public class EffectLTransformObject
    {
        public string Path = string.Empty;

        private Transform _value;
        public Transform value
        {
            get
            {
                if (_value == null)
                {
                    var obj = GameObject.Find(Path);
                    if (obj != null)
                    {
                        _value = obj.transform;
                    }
                    else
                    {
                        _value = null;
                    }
                }
                return _value;
            }
            set
            {
                _value = value;
                Path = EffectUtility.GetTransfromPath(value);
            }
        }
    }

    [Serializable]
    public class EffectLNumeric
    {
        public enum NumericType
        {
            真值,
            实值,
            Double,
        }

        public NumericType type;
        public Numeric num;



        public EffectLNumeric()
        {
            num=new Numeric();
        }

        public object Val()
        {
            switch (type)
            {
                case NumericType.真值:
                    return num.iVal;
                case NumericType.实值:
                    return num.fVal;
                case NumericType.Double:
                    return num.dVal;
                default:
                    return num.dVal;
            }
        }

        public void SetVal(object obj)
        {
            switch (type)
            {
                case NumericType.真值:
                     num.iVal=(int)obj;
                     break;
                case NumericType.实值:
                    num.fVal = (float)obj;
                    break;
                case NumericType.Double:
                    num.dVal = (double)obj;
                    break;
                default:
                    num.dVal = (double)obj;
                    break;
            }
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Explicit,Size = 8)]
    public struct Numeric
    {
        [FieldOffset(0)] public int iVal;
        [FieldOffset(0)] public float fVal;
        [FieldOffset(0)] public double dVal;
    }

    [Serializable]
    public class EffectLPopup
    {
        public int index;
        public string[] popup;

        public EffectLPopup(int index,params string[] popup)
        {
            this.index = index;
            this.popup = popup;
        }
    }
}
