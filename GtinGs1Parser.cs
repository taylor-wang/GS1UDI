using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Material
{
    /// <summary>
    /// GS1规范的物品号码GSTN解析
    /// </summary>
    public class GtinGs1Parser
    {
        public const int GS1_LENGTH_14 = 14;
        public const int EAN13_LENGTH = 13;

        protected string mCode;
        protected string mGTIN = null;
        protected bool mIsValid = false;

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid
        {
            get {
                return mIsValid;
            }
        }

        /// <summary>
        /// 14位GTIN
        /// </summary>
        public string GTIN {
            get
            {
                return mGTIN;
            }
        }

        public GtinGs1Parser(string code)
        {
            this.mCode = code;
            mIsValid = Verify();
        }

        /// <summary>
        /// 验证校验码
        /// </summary>
        /// <returns></returns>
        protected bool Verify()
        {
            if (string.IsNullOrEmpty(mCode))
            {
                return false;
            }

            //只允许长度为13或14
            if (mCode.Length < EAN13_LENGTH)
            {
                return false;
            }
            if (mCode.Length > GS1_LENGTH_14)
            {
                return false;
            }

            //转成14位
            mGTIN = mCode;
            if (mGTIN.Length < GS1_LENGTH_14)
            {
                mGTIN = mGTIN.PadLeft(GS1_LENGTH_14, '0');
            }

            int checkValue = ComputeCheckCode(mGTIN);
            int checkCode = mGTIN[GS1_LENGTH_14 - 1] - '0';
            if (checkValue == checkCode)
            {
                return true;
            }

            mGTIN = null;
            return false;
        }

        /// <summary>
        /// 计算校验码
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public int ComputeCheckCode(string code, int length = 13)
        {
            int[] addNumbers = new int[length];
            //是否偶数位
            bool isEven = (length % 2) > 0;
            for (int i = 0; i < length; i++)
            {
                addNumbers[i] = code[i] - '0';
                if (isEven)
                {
                    addNumbers[i] *= 3;
                }

                isEven = !isEven;
            }

            int sum = addNumbers.Sum();
            int check = (10 - (sum % 10)) % 10;
            return check;
        }

        /// <summary>
        /// 取得不带校验码的12位编码
        /// </summary>
        /// <returns></returns>
        public string GetNoCheck12Code()
        {
            return string.IsNullOrEmpty(mGTIN) ? null : mGTIN.Substring(1, 12);
        }

        /// <summary>
        /// 取得13位GTIN编码
        /// </summary>
        /// <returns></returns>
        public string GetEAN13()
        {
            string gtin = GetNoCheck12Code();
            if (string.IsNullOrEmpty(gtin))
            {
                return null;
            }

            int check = ComputeCheckCode(gtin, 12);

            return string.Format("{0}{1}", gtin, check);
        }
    }
}
