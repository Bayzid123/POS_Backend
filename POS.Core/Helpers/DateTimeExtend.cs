﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Core.Helpers;
public static class DateTimeExtend
{
    public const string Bangladesh_Standard_Time = "Bangladesh Standard Time";
    public static int DateTimeMonth(this DateTime dt, DateTime birthday)
    {
        var difference = birthday.Month - dt.Month;
        if (difference < 0)
        {
            difference += 12;
        }
        return difference;
    }
    public static DateTime BD(this DateTime dateTime)
    {
        DateTime utcTime = System.DateTime.UtcNow;
        TimeZoneInfo BdZone = TimeZoneInfo.FindSystemTimeZoneById(Bangladesh_Standard_Time);
        DateTime localDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, BdZone);

        return localDateTime;
    }

    public static string RelativeTime(this DateTime source, long with = 0)
    {
        if (with == 0)
            with = DateTime.Now.BD().Ticks;

        var ts = new TimeSpan(with - source.Ticks);
        var delta = Math.Abs(ts.TotalSeconds);

        if (delta < 60)
        {
            return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";
        }
        if (delta < 60 * 2)
        {
            return "a minute ago";
        }
        if (delta < 45 * 60)
        {
            return ts.Minutes + " minutes ago";
        }
        if (delta < 90 * 60)
        {
            return "an hour ago";
        }
        if (delta < 24 * 60 * 60)
        {
            return ts.Hours + " hours ago";
        }
        if (delta < 48 * 60 * 60)
        {
            return "yesterday";
        }
        if (delta < 30 * 24 * 60 * 60)
        {
            return ts.Days + " days ago";
        }
        if (delta < 12 * 30 * 24 * 60 * 60)
        {
            var months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
            return months <= 1 ? "one month ago" : months + " months ago";
        }
        var years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
        return years <= 1 ? "one year ago" : years + " years ago";
    }
}
