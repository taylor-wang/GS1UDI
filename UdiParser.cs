using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Material
{
    /// <summary>
    /// Unique Device Identification
    /// 基于GS1标准的UDI编制结构解析器
    /// </summary>
    public class UdiParser
    {
        /// <summary>
        /// 应用标识符：全球贸易项目代码，GTIN (Global Trade Item Number)
        /// </summary>
        public const string AI_GTIN = "01";

        /// <summary>
        /// 应用标识符：物流单元内贸易项目代码
        /// </summary>
        public const string AI_UNIT_TIN = "02";

        /// <summary>
        /// 应用标识符：生产批号(不超过20位的字符数字)
        /// </summary>
        public const string AI_PRODUCT_BATCH = "10";

        /// <summary>
        /// 应用标识符：生产日期（6位数字，YYMMDD）
        /// </summary>
        public const string AI_PRODUCE_DATE = "11";

        /// <summary>
        /// 应用标识符：包装日期（6位数字，YYMMDD）
        /// </summary>
        public const string AI_PACK_DATE = "13";

        /// <summary>
        /// 应用标识符：保质期（6位数字，YYMMDD）
        /// </summary>
        public const string AI_QUALITY_DATE = "15";

        /// <summary>
        /// 应用标识符：失效日期（6位数字，YYMMDD）
        /// </summary>
        public const string AI_EXPIRE_DATE = "17";

        /// <summary>
        /// 应用标识符：序列号
        /// </summary>
        public const string AI_SN = "21";

        /// <summary>
        /// 应用标识符：变量贸易项目中的项目数量
        /// </summary>
        public const string AI_VAR_ITEM_NUM = "30";

        /// <summary>
        /// 应用标识符：物流单元内项目数量
        /// </summary>
        public const string AI_UNIT_ITEM_NUM = "37";

        protected string mCode = null;
        protected string mGTIN = null;
        protected bool mIsValid = false;

        /// <summary>
        /// 应用标识列表
        /// </summary>
        protected Dictionary<string, string> mApplications = new Dictionary<string, string>();

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid
        {
            get
            {
                return mIsValid;
            }
        }

        /// <summary>
        /// 14位GTIN
        /// </summary>
        public string GTIN
        {
            get
            {
                return mGTIN;
            }
        }

        /// <summary>
        /// 原始编码
        /// </summary>
        public string Code
        {
            get
            {
                return mCode;
            }
        }

        /// <summary>
        /// 取得应用标识
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string this[string key]
        {
            get
            {
                if (mApplications.ContainsKey(key))
                {
                    return mApplications[key];
                }
                return null;
            }
        }

        public UdiParser(string code)
        {
            mCode = code;
            mIsValid = Parse();
        }

        /// <summary>
        /// 对编码进行解析
        /// </summary>
        /// <returns></returns>
        protected bool Parse()
        {
            if (string.IsNullOrEmpty(mCode))
            {
                return false;
            }

            //把中文括号替换为英文
            string code = mCode.Replace('（', '(').Replace('）', ')');

            code = code.Trim().Trim('(');
            string[] parts = code.Split('(');
            if (parts.Length < 1)
            {
                return false;
            }

            for (int i = 0; i < parts.Length; i++)
            {
                if (string.IsNullOrEmpty(parts[i]))
                {
                    continue;
                }
                string[] pair = parts[i].Split(')');
                if (pair.Length < 1 || pair.Length > 2)
                {
                    continue;
                }

                if (((pair.Length == 1) && (i == 0)) || ((pair.Length == 2) && AI_GTIN.Equals(pair[0])))
                {
                    //解析GTIN
                    GtinGs1Parser gtin = new GtinGs1Parser(pair[pair.Length - 1]);
                    if (gtin.IsValid)
                    {
                        mGTIN = gtin.GTIN;
                        mApplications[AI_GTIN] = mGTIN;
                    }
                }
                else if(pair.Length == 2)
                {
                    //解析应用
                    if (!(string.IsNullOrEmpty(pair[0]) || string.IsNullOrEmpty(pair[1])))
                    {
                        mApplications[pair[0]] = pair[1];
                    }
                }
            }

            if (string.IsNullOrEmpty(mGTIN))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 将6位数字的应用标识日期解析为时间类型
        /// </summary>
        /// <param name="dateNumber"></param>
        /// <returns></returns>
        public DateTime? GetDateBy6NumberInAI(string dateNumber)
        {
            if (string.IsNullOrEmpty(dateNumber) || (dateNumber.Length != 6))
            {
                return null;
            }
            Regex reg = new Regex(@"^\d{6}$");
            if (!reg.IsMatch(dateNumber))
            {
                return null;
            }

            int year = Convert.ToInt32(dateNumber.Substring(0, 2));
            int month = Convert.ToInt32(dateNumber.Substring(2, 2));
            int day = Convert.ToInt32(dateNumber.Substring(4, 2));
            if ((month < 1) || (month > 12) || (day < 1) || (day > 31))
            {
                return null;
            }

            return new DateTime(2000 + year, month, day);
        }
    }
}
