using System;
using System.Collections.Generic;
using System.Text;

namespace AutoFeedbackFschool.Core
{
    public class FeedbackForms
    {
        public Rating SuDungGio { get; set; }
        public Rating KhaNangTruyenDat { get; set; }
        public Rating DamBaoKhoiLuong { get; set; }
        public Rating KhaNangTaoKK { get; set; }
        public Rating HoTro { get; set; }
        public Rating Ktra { get; set; }

        public string Comment { get; set; }
    }

    public enum Rating
    {
        Always = 0,
        Mostly = 1,
        Rarely = 2,
        Never = 3
    }
}
