using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.Logging.Interfaces;
using GoogleAnalyticsTracker.Core;
using GoogleAnalyticsTracker.Core.Interface;
using GoogleAnalyticsTracker.WebAPI2;
using GoogleAnalyticsTracker.WebAPI2.Interface;
using GoogleAnalyticsTracker.Core.TrackerParameters;
using UHub.CoreLib.Tools.Extensions;
using UHub.CoreLib.Logging.Management;

namespace UHub.CoreLib.Logging.Providers
{
    internal sealed class UsageGAnalyticsProvider : IUsageLogProvider
    {
        static ITrackerEnvironment _env = null;

        private ITrackerEnvironment GetEnv()
        {
            if (_env == null)
            {
                var tempEnv = new AspNetWebApiTrackerEnvironment();
                tempEnv.Hostname = System.Environment.MachineName;
                tempEnv.OsPlatform = "MS Server 2012 R2";

                _env = tempEnv;
            }


            return _env;
        }


        public void CreatePageActionLog(string ResourceUrl, UsageLogData Data)
        {
            try
            {
                var acct = LoggingFactory.Singleton.Properties.GoogleAnalyticsKey;

                var GenParams = new PageLog();

                Tracker t = new Tracker(acct, GetEnv());
                t.EndpointUrl = ResourceUrl;
                if (Data.UserAgent != null)
                {
                    t.UserAgent = Data.UserAgent;
                }


                GenParams.UserId = Data.UserID;
                GenParams.IpOverride = Data.ClientIP;
                GenParams.UserAgent = Data.UserAgent;
                GenParams.UserLanguage = Data.Languages;
                GenParams.ApplicationVersion = LoggingFactory.Singleton.Properties.ApplicationVersionNumber.ToString();
                GenParams.DocumentReferrer = Data.UrlReferrer;
                GenParams.CustomDimension1 = Data.HostIP;


                t.TrackAsync(GenParams);
            }
            catch
            {

            }

        }


        public void CreateApiActionLog(string ResourceUrl, UsageLogData Data)
        {
            try
            {
                var acct = LoggingFactory.Singleton.Properties.GoogleAnalyticsKey;

                var GenParams = new ItemLog();

                Tracker t = new Tracker(acct, GetEnv());
                t.EndpointUrl = ResourceUrl;
                if (Data.UserAgent != null)
                {
                    t.UserAgent = Data.UserAgent;
                }



                GenParams.UserId = Data.UserID;
                GenParams.IpOverride = Data.ClientIP;
                GenParams.UserAgent = Data.UserAgent;
                GenParams.UserLanguage = Data.Languages;
                GenParams.ApplicationVersion = LoggingFactory.Singleton.Properties.ApplicationVersionNumber.ToString();
                GenParams.DocumentReferrer = Data.UrlReferrer;
                GenParams.CustomDimension1 = Data.HostIP;


                t.TrackAsync(GenParams);
            }
            catch
            {

            }
        }


        public void CreateClientEventLog(string ResourceUrl, UsageLogType EventType, UsageLogData Data)
        {
            try
            {
                var acct = LoggingFactory.Singleton.Properties.GoogleAnalyticsKey;

                var GenParams = new EventLog();

                Tracker t = new Tracker(acct, GetEnv());
                t.EndpointUrl = ResourceUrl;
                if (Data.UserAgent != null)
                {
                    t.UserAgent = Data.UserAgent;
                }



                GenParams.UserId = Data.UserID;
                GenParams.IpOverride = Data.ClientIP;
                GenParams.UserAgent = Data.UserAgent;
                GenParams.UserLanguage = Data.Languages;
                GenParams.ApplicationVersion = LoggingFactory.Singleton.Properties.ApplicationVersionNumber.ToString();
                GenParams.DocumentReferrer = Data.UrlReferrer;
                GenParams.CustomDimension1 = Data.HostIP;
                GenParams.CustomDimension2 = EventType.ToString();


                t.TrackAsync(GenParams);
            }
            catch
            {

            }
        }




        private class EventLog : GeneralParameters
        {
            public override HitType HitType => HitType.Event;
        }

        private class ItemLog : GeneralParameters
        {
            public override HitType HitType => HitType.Item;
        }

        private class PageLog : GeneralParameters
        {
            public override HitType HitType => HitType.Pageview;
        }
    }
}
