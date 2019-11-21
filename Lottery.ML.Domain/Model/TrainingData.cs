using Microsoft.ML.Runtime.Api;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lottery.ML.Domain.Model
{
    /// <summary>
    /// 训练数据
    /// </summary>
    public class TrainingData
    {
        [Column(ordinal: "0")]
        public float t0;
        [Column(ordinal: "1")]
        public float t1;
        [Column(ordinal: "2")]
        public float t2;
        [Column(ordinal: "3")]
        public float t3;
        [Column(ordinal: "4")]
        public float t4;
        [Column(ordinal: "5")]
        public float t5;
        [Column(ordinal: "6")]
        public float t6;
        [Column(ordinal: "7")]
        public float t7;
        [Column(ordinal: "8")]
        public float t8;
        [Column(ordinal: "9")]
        public float t9;
        [Column(ordinal: "10")]
        public float t10;
        [Column(ordinal: "11")]
        public float t11;
        [Column(ordinal: "12")]
        public float t12;
        [Column(ordinal: "13")]
        public float t13;
        [Column(ordinal: "14")]
        public float t14;
        [Column(ordinal: "15")]
        public float t15;
        [Column(ordinal: "16")]
        public float t16;
        [Column(ordinal: "17")]
        public float t17;
        [Column(ordinal: "18")]
        public float t18;
        [Column(ordinal: "19")]
        public float t19;
        [Column(ordinal: "20")]
        public float t20;
        [Column(ordinal: "21")]
        public float t21;
        [Column(ordinal: "22")]
        public float t22;
        [Column(ordinal: "23")]
        public float t23;
        [Column(ordinal: "24")]
        public float t24;
        [Column(ordinal: "25")]
        public float t25;
        [Column(ordinal: "26")]
        public float t26;
        [Column(ordinal: "27")]
        public float t27;
        [Column(ordinal: "28")]
        public float t28;
        [Column(ordinal: "29")]
        public float t29;
        [Column(ordinal: "30")]
        public float t30;
        [Column(ordinal: "31")]
        public float t31;
        [Column(ordinal: "32")]
        public float t32;
        [Column(ordinal: "33")]
        public float t33;
        [Column(ordinal: "34")]
        public float t34;
        [Column(ordinal: "35")]
        public float t35;
        [Column(ordinal: "36")]
        public float t36;
        [Column(ordinal: "37")]
        public float t37;
        [Column(ordinal: "38")]
        public float t38;
        [Column(ordinal: "39")]
        public float t39;
        [Column(ordinal: "40")]
        public float t40;
        [Column(ordinal: "41")]
        public float t41;
        [Column(ordinal: "42")]
        public float t42;
        [Column(ordinal: "43")]
        public float t43;
        [Column(ordinal: "44")]
        public float t44;
        [Column(ordinal: "45")]
        public float t45;
        [Column(ordinal: "46")]
        public float t46;
        [Column(ordinal: "47")]
        public float t47;
        [Column(ordinal: "48")]
        public float t48;
        [Column(ordinal: "49")]
        public float t49;
        [Column(ordinal: "50")]
        public float t50;
        [Column(ordinal: "51")]
        public float t51;
        [Column(ordinal: "52")]
        public float t52;
        [Column(ordinal: "53")]
        public float t53;
        [Column(ordinal: "54")]
        public float t54;
        [Column(ordinal: "55")]
        public float t55;
        [Column(ordinal: "56")]
        public float t56;
        [Column(ordinal: "57")]
        public float t57;
        [Column(ordinal: "58")]
        public float t58;
        [Column(ordinal: "59")]
        public float t59;
        [Column(ordinal: "60")]
        public float t60;
        [Column(ordinal: "61")]
        public float t61;
        [Column(ordinal: "62")]
        public float t62;
        [Column(ordinal: "63")]
        public float t63;
        [Column(ordinal: "64")]
        public float t64;
        [Column(ordinal: "65")]
        public float t65;
        [Column(ordinal: "66")]
        public float t66;
        [Column(ordinal: "67")]
        public float t67;
        [Column(ordinal: "68")]
        public float t68;
        [Column(ordinal: "69")]
        public float t69;
        [Column(ordinal: "70")]
        public float t70;
        [Column(ordinal: "71")]
        public float t71;
        [Column(ordinal: "72")]
        public float t72;
        [Column(ordinal: "73")]
        public float t73;
        [Column(ordinal: "74")]
        public float t74;
        [Column(ordinal: "75")]
        public float t75;
        [Column(ordinal: "76")]
        public float t76;
        [Column(ordinal: "77")]
        public float t77;
        [Column(ordinal: "78")]
        public float t78;
        [Column(ordinal: "79")]
        public float t79;
        [Column(ordinal: "80")]
        public float t80;
        [Column(ordinal: "81")]
        public float t81;
        [Column(ordinal: "82")]
        public float t82;
        [Column(ordinal: "83")]
        public float t83;
        [Column(ordinal: "84")]
        public float t84;
        [Column(ordinal: "85")]
        public float t85;
        [Column(ordinal: "86")]
        public float t86;
        [Column(ordinal: "87")]
        public float t87;
        [Column(ordinal: "88")]
        public float t88;
        [Column(ordinal: "89")]
        public float t89;
        [Column(ordinal: "90")]
        public float t90;
        [Column(ordinal: "91")]
        public float t91;
        [Column(ordinal: "92")]
        public float t92;
        [Column(ordinal: "93")]
        public float t93;
        [Column(ordinal: "94")]
        public float t94;
        [Column(ordinal: "95")]
        public float t95;
        [Column(ordinal: "96")]
        public float t96;
        [Column(ordinal: "97")]
        public float t97;
        [Column(ordinal: "98")]
        public float t98;
        [Column(ordinal: "99")]
        public float t99;
        [Column(ordinal: "100", name: "Label")]
        public string Label;

        public static string[] GetColumns()
        {
            return new string[]{
                "t0","t1","t2","t3","t4","t5","t6","t7","t8","t9",
                "t10","t11","t12","t13","t14","t15","t16","t17","t18","t19",
                "t20","t21","t22","t23","t24","t25","t26","t27","t28","t29",
                "t30","t31","t32","t33","t34","t35","t36","t37","t38","t39",
                "t40","t41","t42","t43","t44","t45","t46","t47","t48","t49",
                "t50","t51","t52","t53","t54","t55","t56","t57","t58","t59",
                "t60","t61","t62","t63","t64","t65","t66","t67","t68","t69",
                "t70","t71","t72","t73","t74","t75","t76","t77","t78","t79",
                "t80","t81","t82","t83","t84","t85","t86","t87","t88","t89",
                "t90","t91","t92","t93","t94","t95","t96","t97","t98","t99"
            };
        }

    }
}
